using System;
using System.Drawing;

namespace CDS.ImageDisplay.WinForms.BitmapDisplay;

/// <summary>
/// Provides the drawing rectangle needed to paint an image 
/// under specific zoom and location conditions.
/// </summary>
/// <remarks>
/// <para>
/// The paint rectangle is calculated such that the <see cref="TargetImageCentre"/> 
/// will be painted at the <see cref="TargetDisplayCentre"/>, with the size of the 
/// rectangle being based on the <see cref="ImageSize"/> and <see cref="Zoom"/> level.
/// </para>
/// <para>
/// The paint rectangle calculation is:
///     x: targetDisplayCentre.X - (targetImageCentre.X * zoom)
///     y: targetDisplayCentre.Y - (targetImageCentre.Y * zoom)
///     width: imageSize.Width* zoom
///     height: imageSize.Height* zoom
/// </para>
/// </remarks>
public class VirtualDisplay
{
    private Size imageSize;
    private Size displaySize;
    private PointF targetImageCentre;
    private PointF targetDisplayCentre;
    private float zoom = 1;
    private RectangleF paintRect;
    private float _mapImageToDisplayScaleFactor = 1.0f;


    /// <summary>
    /// A scale factor applied when mapping image coordinates to display coordinates.
    /// The inverse is applied when mapping display coordinates to image coordinates.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Clamped to the range 0.01 to 100.0. Default is 1.0. This does not affect
    /// drawing of the image itself, only clients that use the coordinate-mapping functions.
    /// </para>
    /// <para>
    /// Useful when overlays are generated at a different resolution than the displayed image.
    /// For example, overlays generated against a full-size image that is being displayed at
    /// half size require a scale factor of 0.5.
    /// </para>
    /// </remarks>
    public float MapImageToDisplayScaleFactor
    {
        get => _mapImageToDisplayScaleFactor;
        set => _mapImageToDisplayScaleFactor = Math.Max(0.01f, Math.Min(100.0f, value));
    }


    /// <summary>
    /// Called whenever <see cref="PaintRect"/> changes
    /// </summary>
    private readonly OnPaintRectChangedCallback onPaintRectChanged;

    /// <summary>
    /// Called whenever <see cref="Zoom"/> changes
    /// </summary>
    private readonly Action<float>? onZoomChanged;


    /// <summary>
    /// The size of half a display pixel
    /// </summary>
    /// <remarks>
    /// When the zoom is large a single image pixel will occupy several display
    /// pixels. Often, when drawing overlays, you want to use the centre of 
    /// these display pixels to map to the centre of the image pixel; this
    /// property gives the adjustment to make to a display location for this
    /// offset.
    /// </remarks>
    public SizeF SizeOfHalfDisplayPixel { get; private set; }


    /// <summary>
    /// True if there is anything to display
    /// </summary>
    public bool AnythingToDisplay => !imageSize.IsEmpty && !displaySize.IsEmpty;


    /// <summary>
    /// The current zoom level, clipped to <see cref="Consts.MinZoom"/> and 
    /// <see cref="Consts.MaxZoom"/>. If the target is within <see cref="Consts.SnapToZoom1Tolerance"/>
    /// it will snap to 1.0f.
    /// </summary>
    public float Zoom
    {
        get => zoom;

        set
        {
            float clippedValue = Math.Max(Consts.MinZoom, Math.Min(Consts.MaxZoom, value));
            float diffFrom1 = Math.Abs(1.0f - clippedValue);
            if (diffFrom1 <= Consts.SnapToZoom1Tolerance)
            {
                clippedValue = 1.0f;
            }

            if (clippedValue != zoom)
            {
                zoom = clippedValue;
                RecalculatePaintRect();
                onZoomChanged?.Invoke(zoom);
            }
        }
    }



    /// <summary>
    /// Gets/sets the target image centre. Changes are only accepted
    /// in <see cref="BitmapDisplayMode.Free"/> mode; otherwise they are
    /// ignored. The <see cref="PaintRect"/> will be recalculated and 
    /// <see cref="onPaintRectChanged"/> called if it changes.
    /// </summary>
    public PointF TargetImageCentre
    {
        get => targetImageCentre;

        set
        {
            if ((Mode == BitmapDisplayMode.Free) && (targetImageCentre != value))
            {
                targetImageCentre = value;
                RecalculatePaintRect();
            }
        }
    }


    /// <summary>
    /// Gets/sets the target display centre. Changes are only accepted
    /// in <see cref="BitmapDisplayMode.Free"/> mode; otherwise they are
    /// ignored. The <see cref="PaintRect"/> will be recalculated and 
    /// <see cref="onPaintRectChanged"/> called if it changes.
    /// </summary>
    public PointF TargetDisplayCentre
    {
        get => targetDisplayCentre;

        set
        {
            if ((Mode == BitmapDisplayMode.Free) && (targetDisplayCentre != value))
            {
                targetDisplayCentre = value;
                RecalculatePaintRect();
            }
        }
    }


    /// <summary>
    /// Gets the paint rectangle
    /// </summary>
    public RectangleF PaintRect
    {
        get => paintRect;

        private set
        {
            if (paintRect != value)
            {
                paintRect = value;
                onPaintRectChanged?.Invoke(this, paintRect);
            }
        }
    }


    /// <summary>
    /// Gets/sets the image size. The target image centre is recalculated to
    /// be the centre of the image. The <see cref="PaintRect"/> is recalculated.
    /// </summary>
    public Size ImageSize
    {
        get => imageSize;

        set
        {
            if (imageSize != value)
            {
                imageSize = value;
                targetImageCentre = new PointF(imageSize.Width / 2.0f, imageSize.Height / 2.0f);
                ForceApplyCurrentAutomaticMode();
            }
        }
    }


    /// <summary>
    /// Gets/sets the display size. The target display centre is recalculated to
    /// be the centre of the display. The <see cref="PaintRect"/> is recalculated.
    /// </summary>
    public Size DisplaySize
    {
        get => displaySize;

        set
        {
            if (displaySize != value)
            {
                displaySize = value;
                targetDisplayCentre = new PointF(displaySize.Width / 2.0f, displaySize.Height / 2.0f);
                ForceApplyCurrentAutomaticMode();
            }
        }
    }


    /// <summary>
    /// Gets/sets the display mode. The <see cref="PaintRect"/> is recalculated.
    /// </summary>
    public BitmapDisplayMode Mode
    {
        get;

        set
        {
            if (field != value)
            {
                field = value;
                ForceApplyCurrentAutomaticMode();
            }
        }
    } = BitmapDisplayMode.FitToWindowCentred;


    /// <summary>
    /// Recalculates the paint rect based on the current mode. E.g. even if the 
    /// mode has not been changed, and we're in fit to window, we'll recalculate the
    /// appropriate rectangle based on the current image/display size and target 
    /// centres. Does nothing if mode is <see cref="BitmapDisplayMode.Free"/>.
    /// </summary>
    private void ForceApplyCurrentAutomaticMode()
    {
        if (!AnythingToDisplay)
        { return; }

        switch (Mode)
        {
            case BitmapDisplayMode.ActualSizeCentred:
                ForceActualSizeCentred();
                break;

            case BitmapDisplayMode.FitToWindowCentred:
                ForceFitToWindowCentred();
                break;

            default:
                RecalculatePaintRect();
                break;
        }
    }


    /// <summary>
    /// Initialise
    /// </summary>
    /// <param name="onPaintRectChanged">
    /// A callback which is fired whenever the <see cref="PaintRect"/> has changed.
    /// </param>
    /// <param name="onZoomChanged">
    /// An optional callback fired whenever the <see cref="Zoom"/> level changes.
    /// </param>
    public VirtualDisplay(OnPaintRectChangedCallback onPaintRectChanged, Action<float>? onZoomChanged = null)
    {
        this.onPaintRectChanged = onPaintRectChanged;
        this.onZoomChanged = onZoomChanged;
    }


    /// <summary>
    /// Recalculates the paint rect, or sets to empty if there's nothing to paint.
    /// </summary>
    private void RecalculatePaintRect()
    {
        SizeOfHalfDisplayPixel = new SizeF(zoom / 2, zoom / 2);

        PaintRect = AnythingToDisplay
            ? new RectangleF(
                x: targetDisplayCentre.X - (targetImageCentre.X * zoom),
                y: targetDisplayCentre.Y - (targetImageCentre.Y * zoom),
                width: imageSize.Width * zoom,
                height: imageSize.Height * zoom)
            : (RectangleF)Rectangle.Empty;
    }



    /// <summary>
    /// Returns the display location where a pixel at <paramref name="imageLocation"/> would be drawn.
    /// </summary>
    /// <remarks>
    /// This is useful when you want to overlay graphics on the image with respect to the 
    /// image locations regardless of the current zoom and display settings.
    /// 
    /// When the zoom is large each image pixel will occupy several display pixels. This 
    /// returned location is at the top-left of this region.
    /// </remarks>
    /// <param name="imageLocation">A location on the image</param>
    /// <returns>A location on the display or an empty point if there's nothing to display</returns>
    public PointF MapImageToDisplay(PointF imageLocation)
    {
        if (!AnythingToDisplay)
        { return PointF.Empty; }

        float scaledX = imageLocation.X * _mapImageToDisplayScaleFactor;
        float scaledY = imageLocation.Y * _mapImageToDisplayScaleFactor;

        var displayLocation = new PointF(
            x: paintRect.X + (paintRect.Width * scaledX / imageSize.Width),
            y: paintRect.Y + (paintRect.Height * scaledY / imageSize.Height));

        return displayLocation;
    }


    /// <summary>
    /// Returns the image location where a pixel at <paramref name="displayLocation"/> would 
    /// have been drawn.
    /// </summary>
    /// <remarks>
    /// This is useful when you want to determine the image location
    /// under the current mouse location.
    /// </remarks>
    /// <param name="displayLocation">A location on the display</param>
    /// <param name="ignoreScaleFactor">
    /// When <see langword="true"/>, <see cref="MapImageToDisplayScaleFactor"/> is not applied.
    /// Use this to recover raw image-pixel coordinates (e.g. for zoom pivot calculation).
    /// </param>
    /// <returns>A location on the image or an empty point if there's nothing to display</returns>
    public PointF MapDisplayToImage(PointF displayLocation, bool ignoreScaleFactor = false)
    {
        if (!AnythingToDisplay)
        { return PointF.Empty; }

        var imageLocation = new PointF(
            x: (displayLocation.X - paintRect.X) / Zoom,
            y: (displayLocation.Y - paintRect.Y) / Zoom);

        if (!ignoreScaleFactor && _mapImageToDisplayScaleFactor != 1.0f)
        {
            imageLocation = new PointF(
                x: imageLocation.X / _mapImageToDisplayScaleFactor,
                y: imageLocation.Y / _mapImageToDisplayScaleFactor);
        }

        return imageLocation;
    }



    /// <summary>
    /// Returns the image location where a rectangle at <paramref name="displayRect"/> would 
    /// have been drawn.
    /// </summary>
    /// <remarks>
    /// This is useful when you have used the mouse to select a region of interest
    /// (rectangle) over the image and want to deteremine the ROI with respect to 
    /// the image.
    /// </remarks>
    /// <param name="displayRect">A region on the image</param>
    /// <param name="ignoreScaleFactor">
    /// When <see langword="true"/>, <see cref="MapImageToDisplayScaleFactor"/> is not applied.
    /// </param>
    /// <returns>A region on the display or an empty rectangle if there's nothing to display</returns>
    public RectangleF MapDisplayToImage(RectangleF displayRect, bool ignoreScaleFactor = false)
    {
        if (!AnythingToDisplay)
        { return RectangleF.Empty; }

        var bottomRight = new PointF(displayRect.Right, displayRect.Bottom);
        PointF imageTopLeft = MapDisplayToImage(displayRect.Location, ignoreScaleFactor);
        PointF imageBottomRight = MapDisplayToImage(bottomRight, ignoreScaleFactor);

        var imageRect = RectangleF.FromLTRB(
            left: imageTopLeft.X,
            top: imageTopLeft.Y,
            right: imageBottomRight.X,
            bottom: imageBottomRight.Y);

        return imageRect;
    }


    /// <summary>
    /// Centre the image and set to 1:1 zoom. Only applies if the 
    /// mode is <see cref="BitmapDisplayMode.Free"/>, otherwise does 
    /// nothing.
    /// </summary>
    public void ActualSizeCentred()
    {
        if (AnythingToDisplay && (Mode == BitmapDisplayMode.Free))
        {
            ForceActualSizeCentred();
        }
    }


    /// <summary>
    /// Force the image be be centred and drawn with 1:1 zoom,
    /// regardless of the current mode
    /// </summary>
    private void ForceActualSizeCentred()
    {
        float previousZoom = zoom;
        zoom = 1;
        ForceCentre();
        if (zoom != previousZoom)
        {
            onZoomChanged?.Invoke(zoom);
        }
    }


    /// <summary>
    /// Centres the image. Only applies if the 
    /// mode is <see cref="BitmapDisplayMode.Free"/>, otherwise does 
    /// nothing.
    /// </summary>
    public void Centre()
    {
        if (AnythingToDisplay && (Mode == BitmapDisplayMode.Free))
        {
            ForceCentre();
        }
    }


    /// <summary>
    /// Forces the image to be centred on the display, regardless of the current
    /// display mode
    /// </summary>
    private void ForceCentre()
    {
        targetDisplayCentre = new PointF(displaySize.Width / 2.0f, displaySize.Height / 2.0f);
        targetImageCentre = new PointF(imageSize.Width / 2.0f, imageSize.Height / 2.0f);
        RecalculatePaintRect();
    }


    /// <summary>
    /// Centres the image and adjusts the zoom so the image is as large as possible 
    /// within the display. Only applies if the 
    /// mode is <see cref="BitmapDisplayMode.Free"/>, otherwise does 
    /// nothing.
    /// </summary>
    public void FitToWindowCentred()
    {
        if (AnythingToDisplay && (Mode == BitmapDisplayMode.Free))
        {
            ForceFitToWindowCentred();
        }
    }


    /// <summary>
    /// Centres the image and adjusts the zoom so the image is as large as possible
    /// within the display, regardless of the current display mode.
    /// </summary>
    private void ForceFitToWindowCentred()
    {
        float previousZoom = zoom;

        double imageToDisplayHorizRatio = (double)imageSize.Width / displaySize.Width;
        double imageToDisplayVerticalRatio = (double)imageSize.Height / displaySize.Height;
        bool shouldMaximiseHeight = imageToDisplayHorizRatio < imageToDisplayVerticalRatio;

        float rawZoom = shouldMaximiseHeight
            ? (float)displaySize.Height / imageSize.Height
            : (float)displaySize.Width / imageSize.Width;

        zoom = Math.Min(Math.Max(rawZoom, Consts.MinZoom), Consts.MaxZoom);

        ForceCentre();
        if (zoom != previousZoom)
        {
            onZoomChanged?.Invoke(zoom);
        }
    }


    /// <summary>
    /// Returns the display location where a rectangle at <paramref name="imageRect"/> would be drawn.
    /// </summary>
    /// <remarks>
    /// This is useful when you want to overlay graphics on the image with respect to the 
    /// image regardless of the current zoom and display settings.
    /// 
    /// When the zoom is large each image pixel will occupy several display pixels. The location
    /// of the returned rectangle is at the top-left of this region.
    /// </remarks>
    /// <param name="imageRect">A rectangle on the image</param>
    /// <returns>A rectangle on the display or an empty rectangle if there's nothing to display</returns>
    public RectangleF MapImageToDisplay(RectangleF imageRect)
    {
        if (!AnythingToDisplay)
        { return RectangleF.Empty; }

        var bottomRight = new PointF(imageRect.Right, imageRect.Bottom);
        PointF displayTopLeft = MapImageToDisplay(imageRect.Location);
        PointF displayBottomRight = MapImageToDisplay(bottomRight);

        var displayRect = RectangleF.FromLTRB(
            left: displayTopLeft.X,
            top: displayTopLeft.Y,
            right: displayBottomRight.X,
            bottom: displayBottomRight.Y);

        return displayRect;
    }


    /// <summary>
    /// Maps a distance, in image units, to display units.
    /// </summary>
    /// <param name="imageDistance">
    /// Distance in image units
    /// </param>
    /// <returns>
    /// Distance in display units, or 0 if there's nothing to display
    /// </returns>
    public float MapImageToDisplay(float imageDistance) =>
        !AnythingToDisplay ? 0 : imageDistance * _mapImageToDisplayScaleFactor * zoom;
}
