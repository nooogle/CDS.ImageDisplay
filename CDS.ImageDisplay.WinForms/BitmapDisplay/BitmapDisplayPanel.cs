using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Threading;
using System.Windows.Forms;
using CDS.ImageDisplay.WinForms.Utils;

namespace CDS.ImageDisplay.WinForms.BitmapDisplay;

/// <summary>
/// Displays a bitmap
/// </summary>
public partial class BitmapDisplayPanel : UserControl, ICoordinateMapper
{
    private const string s_categoryCDS = "CDS";

    private readonly UIDispatcher _uiDispatcher = new();
    private readonly object _imageLock = new();

    /// <summary>
    /// The image being displayed. Only read or modified on the UI thread; the
    /// field itself is only reassigned (swapped with <see cref="_pendingDisplayImage"/>)
    /// on the UI thread while holding <see cref="_imageLock"/>.
    /// </summary>
    private ImageWrapper _displayImage = new();

    /// <summary>
    /// The latest image set from a non-UI thread, waiting to be swapped in by the
    /// UI thread. Guarded by <see cref="_imageLock"/>.
    /// </summary>
    private ImageWrapper _pendingDisplayImage = new();

    /// <summary>
    /// True if <see cref="_pendingDisplayImage"/> holds an image that has not yet
    /// been displayed. Guarded by <see cref="_imageLock"/>.
    /// </summary>
    private bool _hasPendingImage;

    /// <summary>
    /// True if a callback to apply the pending image has been posted to the UI
    /// thread and has not yet run. Guarded by <see cref="_imageLock"/>.
    /// </summary>
    private bool _isApplyPendingImageQueued;

    /// <summary>
    /// True once the control has been disposed; images set after this are ignored.
    /// Guarded by <see cref="_imageLock"/>.
    /// </summary>
    private bool _isDisposed;

    private readonly VirtualDisplay _virtualDisplay;
    private readonly Stopwatch _stopwatch = new();
    private readonly DragManager _dragManager;
    private readonly ZoomManager _zoomManager;


    /// <summary>
    /// Greater than 0 if dragging is suppressed.
    /// </summary>
    /// <remarks>
    /// This is useful if a something is hooking mouse down/move/up events and wants
    /// to prevent the image being dragged around while they are performing their
    /// own operation (such as selected a region of interest).
    /// </remarks>
    private int _suppressDraggingCounter;


    /// <summary>
    /// True if dragging is allowed
    /// </summary>
    private bool IsDraggingAllowed => _suppressDraggingCounter <= 0;


    /// <summary>
    /// Timing metrics
    /// </summary>
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [Category(s_categoryCDS)]
    public BitmapDisplayMetrics TimingMetrics { get; } = new();


    /// <summary>
    /// The size of half a displayed pixel
    /// </summary>
    /// <remarks>
    /// Use this as an offset when drawing with a large zoom and where the 
    /// drawing locations should be in the middle of an image pixel. E.g. with
    /// a zoom of 11, each image pixel will take 11*11 pixels on the screen. The
    /// half pixel size will be 5.5. Calling <see cref="MapImageToDisplay(PointF, DisplayPixelAlign)"/>
    /// will return the location of the top-left of this 11*11 block for a particular
    /// image pixel; adding this offset of 5.5 will allow drawing to start in the middle
    /// of this 11*11 block.
    /// </remarks>
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [Category(s_categoryCDS)]
    public SizeF SizeOfHalfDisplayPixel => _virtualDisplay.SizeOfHalfDisplayPixel;


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
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [Category(s_categoryCDS)]
    public float MapImageToDisplayScaleFactor
    {
        get => _virtualDisplay.MapImageToDisplayScaleFactor;
        set => _virtualDisplay.MapImageToDisplayScaleFactor = value;
    }


    /// <summary>
    /// Gets the size of the image currently being displayed, or <see cref="Size.Empty"/> if no image is loaded.
    /// </summary>
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [Category(s_categoryCDS)]
    public Size ImageSize => _virtualDisplay.ImageSize;


    /// <summary>
    /// Gets the paint rectangle. This is based on the current image size,
    /// display size, zoom, target image centre and target display centre.
    /// </summary>
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [Category(s_categoryCDS)]
    public RectangleF PaintRect => _virtualDisplay.PaintRect;


    /// <summary>
    /// Called when the image has been painted; gives a client an opportunity
    /// to paint graphics on top of the image. This will be flicker-free as long
    /// as the control uses double buffering
    /// </summary>
    [Category(s_categoryCDS)]
    [Description(
        "Called when the image has been painted; gives a client an opportunity " +
        "to paint flicker-free graphics on top of the image.")]
    public event EventHandler<PaintOverEventArgs>? PaintOver;


    /// <summary>
    /// Called after the background has been painted and before the image has been painted;
    /// gives a client an opportunity to paint graphics under the image.
    /// This will be flicker-free as long as the control uses double buffering.
    /// </summary>
    [Category(s_categoryCDS)]
    [Description(
        "Called after the background has been painted and before the image has been painted; " +
        "to paint flicker-free graphics under the image.")]
    public event EventHandler<PaintUnderEventArgs>? PaintUnder;


    /// <summary>
    /// Called when the display mode is changed.
    /// </summary>
    [Category(s_categoryCDS)]
    [Description("Called when the display mode is changed.")]
    public event EventHandler<DisplayModeChangedEventArgs>? DisplayModeChanged;


    /// <summary>
    /// Fired when the paint rectangle of the display is changed
    /// </summary>
    [Category(s_categoryCDS)]
    [Description("Called when the paint rectangle is changed.")]
    public event EventHandler<PaintRectChangedEventArgs>? PaintRectChanged;


    /// <summary>
    /// Fired when the image size changes
    /// </summary>
    public event EventHandler<ImageSizeChangedEventArgs>? ImageSizeChanged;


    /// <summary>
    /// Fired when the zoom level changes.
    /// </summary>
    [Category(s_categoryCDS)]
    [Description("Called when the zoom level changes.")]
    public event EventHandler<ZoomChangedEventArgs>? ZoomChanged;


    /// <summary>
    /// Gets the image currently being displayed. 
    /// </summary>
    /// <remarks>
    /// The display owns this image and may dispose or reuse it at any time if a new
    /// (pending) image is being swapped in; therefore, callers should
    /// use this method with caution since it's more of a diagnostics 
    /// tool than for sharing image data. Only access it from the UI thread and
    /// don't hold on to the reference.
    /// </remarks>
    [Category(s_categoryCDS)]
    public Bitmap? DisplayImage => _displayImage.Image;


    /// <summary>
    /// A copy of the image is taken and then set as the image to be displayed.
    /// This takes immediate effect when called from the UI thread;
    /// otherwise takes place asap by invoking an update procedure on the UI thread
    /// and returning immediately.
    /// </summary>
    /// <remarks>
    /// The <see cref="TargetImageCentre"/> is reset if an image is currently being 
    /// displayed and a new image of a different size is set.
    /// 
    /// When called on a non-UI thread this returns immediately; it will be a small
    /// amount of time later that the image is finally set as the display image.
    /// </remarks>
    [Category(s_categoryCDS)]
    public void SetImage(IImageSource? imageSource)
    {
        // Thread identity is used rather than InvokeRequired, which returns false on
        // every thread while the control has no window handle.
        if (_uiDispatcher.IsOnUIThread)
        {
            SetImageDirectlyFromUIThread(imageSource);
        }
        else
        {
            SetImageIndirectlyFromNonUIThread(imageSource);
        }
    }


    /// <summary>
    /// A copy of the image is taken and then set as the image to be displayed.
    /// This takes immediate effect when called from the UI thread;
    /// otherwise takes place asap by invoking an update procedure on the UI thread
    /// and returning immediately.
    /// </summary>
    /// <remarks>
    /// The <see cref="TargetImageCentre"/> is reset if an image is currently being 
    /// displayed and a new image of a different size is set.
    /// 
    /// When called on a non-UI thread this returns immediately; it will be a small
    /// amount of time later that the image is finally set as the display image.
    /// </remarks>
    [Category(s_categoryCDS)]
    public void SetImage(Bitmap? image)
    {
        using var imageSource = new BitmapImageSource(image);
        SetImage(imageSource);
    }


    /// <summary>
    /// Removes any image currently being displayed.
    /// This takes immediate effect when called from the UI thread;
    /// otherwise takes place asap by invoking an update procedure on the UI thread
    /// and returning immediately.
    /// </summary>
    [Category(s_categoryCDS)]
    public void ClearImage() => SetImage((IImageSource?)null);


    /// <summary>
    /// Copies the new image into the pending image; then posts an update
    /// to the UI thread to get this pending image onto the display
    /// </summary>
    private void SetImageIndirectlyFromNonUIThread(IImageSource? imageSource)
    {
        lock (_imageLock)
        {
            if (_isDisposed)
            {
                return;
            }

            // Always store the latest frame so the UI thread picks up the most
            // recent image when it processes the posted callback (last-writer-wins).
            _pendingDisplayImage.SetNewImage(imageSource);
            _hasPendingImage = true;

            // If a callback is already queued to apply the pending image, don't
            // post another one — the existing callback will use the image we just
            // stored above. This keeps at most one callback pending, preventing
            // message-loop buildup regardless of input frame rate.
            if (_isApplyPendingImageQueued)
            {
                return;
            }

            // Posting never waits for the UI thread so it's safe under the lock. If
            // it can't be posted (no UI context yet) the image stays pending and is
            // applied when the handle is created or on the next successful post.
            _isApplyPendingImageQueued = _uiDispatcher.TryPost(OnApplyPendingImageCallback);
        }
    }


    /// <summary>
    /// Runs on the UI thread when posted by <see cref="SetImageIndirectlyFromNonUIThread"/>.
    /// </summary>
    private void OnApplyPendingImageCallback()
    {
        lock (_imageLock)
        {
            _isApplyPendingImageQueued = false;
        }

        ApplyPendingImage();
    }


    /// <summary>
    /// If a non-UI thread has set an image that hasn't been displayed yet, swap it
    /// in as the display image. Must be called on the UI thread.
    /// </summary>
    /// <remarks>
    /// The pending and display images are swapped rather than copied, so the lock
    /// is only held briefly and each frame is copied once. The old display image
    /// becomes the pending buffer for the next frame, reusing its allocation.
    /// </remarks>
    private void ApplyPendingImage()
    {
        Size originalImageSize = _virtualDisplay.ImageSize;

        lock (_imageLock)
        {
            if (_isDisposed || !_hasPendingImage)
            {
                return;
            }

            (_displayImage, _pendingDisplayImage) = (_pendingDisplayImage, _displayImage);
            _hasPendingImage = false;
        }

        // Raise events outside the lock so handlers can't deadlock a producer thread
        OnDisplayImageReplaced(originalImageSize);
    }


    /// <summary>
    /// We have a new image to display and we're on the UI thread meaning
    /// we won't be mid-paint; we can directly clone or copy the new image
    /// and repaint
    /// </summary>
    private void SetImageDirectlyFromUIThread(IImageSource? imageSource)
    {
        lock (_imageLock)
        {
            if (_isDisposed)
            {
                return;
            }

            // This image is newer than any image a non-UI thread has queued, so
            // make sure a queued callback doesn't replace it with an older frame.
            _hasPendingImage = false;
        }

        // Only the UI thread touches _displayImage, so no lock is needed here
        Size originalImageSize = _virtualDisplay.ImageSize;
        _displayImage.SetNewImage(imageSource);
        OnDisplayImageReplaced(originalImageSize);
    }


    /// <summary>
    /// Updates the virtual display after the display image has changed, raises
    /// <see cref="ImageSizeChanged"/> if needed and repaints. UI thread only.
    /// </summary>
    private void OnDisplayImageReplaced(Size originalImageSize)
    {
        _virtualDisplay.ImageSize = _displayImage.ImageSize;

        if (originalImageSize != _virtualDisplay.ImageSize)
        {
            ImageSizeChanged?.Invoke(this, new ImageSizeChangedEventArgs(originalImageSize, _virtualDisplay.ImageSize));
        }

        Invalidate();
    }


    /// <summary>
    /// The handle is always created on the UI thread, so (re)capture it here; then
    /// display any image that a non-UI thread set before a callback could be posted.
    /// </summary>
    protected override void OnHandleCreated(EventArgs e)
    {
        _uiDispatcher.Capture();
        base.OnHandleCreated(e);
        ApplyPendingImage();
    }


    /// <summary> 
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            // Under the lock so a non-UI thread can't be copying into the pending
            // image while it's disposed, nor set a new image afterwards.
            lock (_imageLock)
            {
                _isDisposed = true;
                _displayImage.Dispose();
                _pendingDisplayImage.Dispose();
            }

            components?.Dispose();
        }

        base.Dispose(disposing);
    }


    /// <summary>
    /// True if there's anything to display
    /// </summary>
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [Category(s_categoryCDS)]
    public bool AnythingToDisplay => _virtualDisplay.AnythingToDisplay;


    /// <summary>
    /// The image display mode
    /// </summary>
    [Category(s_categoryCDS)]
    [Description("The image display mode")]
    [DefaultValue(BitmapDisplayMode.FitToWindowCentred)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
    public BitmapDisplayMode DisplayMode
    {
        get => _virtualDisplay.Mode;
        set
        {
            if (_virtualDisplay.Mode != value)
            {
                _virtualDisplay.Mode = value;
                DisplayModeChanged?.Invoke(this, new DisplayModeChangedEventArgs(value));
            }
        }
    }


    /// <summary>
    /// Suppress dragging
    /// </summary>
    public void SuppressDragging() => _suppressDraggingCounter++;


    /// <summary>
    /// Allow dragging. (Note this will only have an effect if the counter is
    /// reduced to 0).
    /// </summary>
    public void UnsuppressDragging() => _suppressDraggingCounter--;


    /// <summary>
    /// Target image centre.
    /// </summary>
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [Category(s_categoryCDS)]
    public PointF TargetImageCentre
    {
        get => _virtualDisplay.TargetImageCentre;
        set => _virtualDisplay.TargetImageCentre = value;
    }


    /// <summary>
    /// Target display centre.
    /// </summary>
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [Category(s_categoryCDS)]
    public PointF TargetDisplayCentre
    {
        get => _virtualDisplay.TargetDisplayCentre;
        set => _virtualDisplay.TargetDisplayCentre = value;
    }


    /// <summary>
    /// Initialise - configures for double buffered display
    /// </summary>
    public BitmapDisplayPanel()
    {
        InitializeComponent();
        SetStyle(ControlStyles.AllPaintingInWmPaint, true);
        SetStyle(ControlStyles.UserPaint, true);
        SetStyle(ControlStyles.OptimizedDoubleBuffer, true);

        SetStyle(ControlStyles.Selectable, true); // Ensure the control is selectable
        TabStop = true; // Enable tab stop so it can gain focus

        _dragManager = new DragManager(DragManager_SetNewTargetDisplayCentre);
        _zoomManager = new ZoomManager(ZoomManager_SetNewZoom);
        _virtualDisplay = new VirtualDisplay(VirtualImageOnDisplay_OnPaintRectChanged, VirtualDisplay_OnZoomChanged);

        // Controls are constructed on the UI thread, and creating the first control on a
        // thread installs its WindowsFormsSynchronizationContext, so it can be captured
        // now; this lets non-UI threads post images before the handle is created.
        _uiDispatcher.Capture();
    }


    /// <summary>
    /// Ensure arrow keys are treated as input keys so they reach <see cref="OnKeyDown"/>.
    /// </summary>
    protected override bool IsInputKey(Keys keyData)
    {
        if ((keyData & Keys.KeyCode) == Keys.Left ||
            (keyData & Keys.KeyCode) == Keys.Right ||
            (keyData & Keys.KeyCode) == Keys.Up ||
            (keyData & Keys.KeyCode) == Keys.Down)
        {
            return true;
        }

        return base.IsInputKey(keyData);
    }


    /// <summary>
    /// Arrow keys pan the image when in <see cref="BitmapDisplayMode.Free"/> mode.
    /// </summary>
    protected override void OnKeyDown(KeyEventArgs e)
    {
        if (e == null) { throw new ArgumentNullException(nameof(e)); }

        base.OnKeyDown(e);

        if (e.Handled || !AnythingToDisplay || DisplayMode != BitmapDisplayMode.Free)
        {
            return;
        }

        const float panStep = 20f;
        PointF centre = _virtualDisplay.TargetDisplayCentre;

        switch (e.KeyCode)
        {
            case Keys.Left:  centre.X += panStep; break;
            case Keys.Right: centre.X -= panStep; break;
            case Keys.Up:    centre.Y += panStep; break;
            case Keys.Down:  centre.Y -= panStep; break;
            default: return;
        }

        _virtualDisplay.TargetDisplayCentre = centre;
        e.Handled = true;
    }


    /// <summary>
    /// Drag manager wants to set a new display centre
    /// </summary>
    private void DragManager_SetNewTargetDisplayCentre(PointF targetDisplayCentre) =>
        _virtualDisplay.TargetDisplayCentre = targetDisplayCentre;


    /// <summary>
    /// Zoom manager wants to set a new zoom
    /// </summary>
    private void ZoomManager_SetNewZoom(float zoom, PointF targetDisplayCentre, PointF targetImageCentre)
    {
        _virtualDisplay.Zoom = zoom;
        _virtualDisplay.TargetDisplayCentre = targetDisplayCentre;
        _virtualDisplay.TargetImageCentre = targetImageCentre;
    }


    /// <summary>
    /// Set/get the zoom level. The limits in the <see cref="Consts"/> class are
    /// used.
    /// </summary>
    [Category(s_categoryCDS)]
    [DefaultValue(1.0f)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
    public float Zoom
    {
        get => _virtualDisplay.Zoom;
        set => _virtualDisplay.Zoom = value;
    }


    /// <summary>
    /// Palette mode applied to 8bpp indexed images.
    /// </summary>
    /// <remarks>
    /// Only affects images with <see cref="System.Drawing.Imaging.PixelFormat.Format8bppIndexed"/>;
    /// has no visible effect on 24/32-bit images. Changing the mode triggers a repaint.
    /// </remarks>
    [Category(s_categoryCDS)]
    [Description("Palette mode applied to 8bpp indexed images.")]
    [DefaultValue(GreyscalePaletteMode.Standard)]
    public GreyscalePaletteMode GreyscalePaletteMode
    {
        get => _displayImage.GreyscalePaletteMode;

        set
        {
            if (_displayImage.GreyscalePaletteMode != value)
            {
                // Keep both buffers in step since they are swapped when a
                // non-UI thread's image is applied.
                lock (_imageLock)
                {
                    _pendingDisplayImage.GreyscalePaletteMode = value;
                }

                _displayImage.GreyscalePaletteMode = value;
                Invalidate();
            }
        }
    }


    /// <summary>
    /// The virtual display wants to set a new paint rect
    /// </summary>
    private void VirtualImageOnDisplay_OnPaintRectChanged(VirtualDisplay sender, RectangleF paintRect)
    {
        Invalidate();
        PaintRectChanged?.Invoke(this, new PaintRectChangedEventArgs(this, paintRect));
    }


    /// <summary>
    /// The virtual display zoom has changed
    /// </summary>
    private void VirtualDisplay_OnZoomChanged(float zoom) =>
        ZoomChanged?.Invoke(this, new ZoomChangedEventArgs(zoom));


    /// <summary>
    /// Returns the display location where a pixel at <paramref name="imageLocation"/> 
    /// would be drawn. The location is the centre of the pixel. This is useful
    /// when the image is zoomed in such as a single image pixel takes many display
    /// pixels and you want to start your drawing in the centre of this block.
    /// </summary>
    /// <param name="imageLocation">A region on the image</param>
    /// <param name="pixelAdjust">The pixel adjustment</param>
    /// <returns>A location on the display or an empty point if there's nothing to display</returns>
    public Point MapImageToDisplay(PointF imageLocation, DisplayPixelAlign pixelAdjust)
    {
        PointF displayCoordinate = _virtualDisplay.MapImageToDisplay(imageLocation);

        if (pixelAdjust == DisplayPixelAlign.Centre)
        {
            displayCoordinate.X += SizeOfHalfDisplayPixel.Width;
            displayCoordinate.Y += SizeOfHalfDisplayPixel.Height;
        }

        return Point.Round(displayCoordinate);
    }


    /// <summary>
    /// Returns the display location where a rectangle at <paramref name="imageRect"/> would 
    /// have been drawn, with the location being the centre of the pixels. 
    /// centre of this block.
    /// </summary>
    /// <remarks>
    /// This is useful when the image is zoomed in such that individual image pixels
    /// are displayed as a block of display pixels and you want to start drawing in the
    /// </remarks>
    /// <param name="imageRect">A region on the image</param>
    /// <param name="pixelAdjust">The pixel adjustment</param>
    /// <returns>A region on the display or an empty rectangle if there's nothing to display</returns>
    public Rectangle MapImageToDisplay(RectangleF imageRect, DisplayPixelAlign pixelAdjust)
    {
        RectangleF displayRect = _virtualDisplay.MapImageToDisplay(imageRect);

        if (pixelAdjust == DisplayPixelAlign.Centre)
        {
            displayRect.X += SizeOfHalfDisplayPixel.Width;
            displayRect.Y += SizeOfHalfDisplayPixel.Height;
        }

        return Rectangle.Round(displayRect);
    }


    /// <summary>
    /// Returns the display location where a rectangle at <paramref name="imageRect"/> would
    /// have been drawn, as a <see cref="RectangleF"/> preserving sub-pixel precision.
    /// </summary>
    /// <param name="imageRect">A region on the image</param>
    /// <param name="pixelAdjust">The pixel adjustment</param>
    /// <returns>A region on the display or an empty rectangle if there's nothing to display</returns>
    public RectangleF MapImageToDisplayF(RectangleF imageRect, DisplayPixelAlign pixelAdjust)
    {
        RectangleF displayRect = _virtualDisplay.MapImageToDisplay(imageRect);

        if (pixelAdjust == DisplayPixelAlign.Centre)
        {
            displayRect.X += SizeOfHalfDisplayPixel.Width;
            displayRect.Y += SizeOfHalfDisplayPixel.Height;
        }

        return displayRect;
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
    /// <returns>A location on the image or an empty point if there's nothing to display</returns>
    public PointF MapDisplayToImage(PointF displayLocation) =>
        _virtualDisplay.MapDisplayToImage(displayLocation);


    /// <summary>
    /// Returns the image location where a pixel at <paramref name="displayLocation"/> would
    /// have been drawn.
    /// </summary>
    /// <remarks>
    /// This is useful when you want to determine the image location
    /// under the current mouse location.
    /// </remarks>
    /// <param name="displayLocation">A location on the display</param>
    /// <returns>A location on the image or an empty point if there's nothing to display</returns>
    public PointF MapDisplayToImage(Point displayLocation) =>
        _virtualDisplay.MapDisplayToImage(displayLocation);


    /// <summary>
    /// Returns the image location where a pixel at <paramref name="displayLocation"/> would
    /// have been drawn, ignoring any <see cref="MapImageToDisplayScaleFactor"/>.
    /// </summary>
    /// <remarks>
    /// Use this when you need the raw image-pixel coordinate under a display location — for example,
    /// when computing the zoom pivot point from a mouse position.
    /// </remarks>
    /// <param name="displayLocation">A location on the display</param>
    /// <returns>A location on the image or an empty point if there's nothing to display</returns>
    public PointF MapDisplayToImageIgnoringScaleFactor(Point displayLocation) =>
        _virtualDisplay.MapDisplayToImage(displayLocation, ignoreScaleFactor: true);


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
    /// <returns>A region on the display or an empty rectangle if there's nothing to display</returns>
    public RectangleF MapDisplayToImage(RectangleF displayRect) =>
        _virtualDisplay.MapDisplayToImage(displayRect);


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
    /// <returns>A region on the display or an empty rectangle if there's nothing to display</returns>
    public RectangleF MapDisplayToImage(Rectangle displayRect) =>
        _virtualDisplay.MapDisplayToImage(displayRect);


    /// <summary>
    /// Centres the image on the display, retaining the existing zoom level.
    /// Only applied if the display mode is <see cref="BitmapDisplayMode.Free"/>,
    /// no-op otherwise.
    /// </summary>
    public void CentreImage() => _virtualDisplay.Centre();


    /// <summary>
    /// Centres the image on the display and sets the zoom to 1.
    /// Only applied if the display mode is <see cref="BitmapDisplayMode.Free"/>,
    /// no-op otherwise.
    /// </summary>
    public void CentreImageActualSize() => _virtualDisplay.ActualSizeCentred();


    /// <summary>
    /// Centres the image on the display and adjusts the zoom so that the 
    /// image fills the display as much as possible.
    /// Only applied if the display mode is <see cref="BitmapDisplayMode.Free"/>,
    /// no-op otherwise.
    /// </summary>
    public void FitToWindowCentred() => _virtualDisplay.FitToWindowCentred();


    /// <summary>
    /// Paint the background (optimised when there's nothing to display)
    /// </summary>
    protected override void OnPaintBackground(PaintEventArgs e)
    {
        if (e == null)
        {
            return;
        }

        _stopwatch.Restart();

        RectangleF clippedDrawingRect = _virtualDisplay.PaintRect;
        clippedDrawingRect.Intersect(ClientRectangle);
        bool shouldPaintBackground = _virtualDisplay.PaintRect.IsEmpty || (e.ClipRectangle != Rectangle.Truncate(clippedDrawingRect));
        if (shouldPaintBackground)
        {
            base.OnPaintBackground(e);
        }

        _stopwatch.Stop();
        TimingMetrics.BackgroundPaint = _stopwatch.Elapsed;
    }


    /// <summary>
    /// Paint the image
    /// </summary>
    protected override void OnPaint(PaintEventArgs e)
    {
        if (e == null) { throw new ArgumentNullException(nameof(e)); }

        _stopwatch.Restart();

        PaintUnder?.Invoke(this, new PaintUnderEventArgs(this, e.Graphics));

        if (AnythingToDisplay)
        {
            PaintBitmap(e);
        }
        else if (BackgroundImage is null)
        {
            PaintNoImageHatch(e);
        }

        PaintOver?.Invoke(this, new PaintOverEventArgs(this, e.Graphics));

        _stopwatch.Stop();
        TimingMetrics.ForegroundPaint = _stopwatch.Elapsed;
    }


    /// <summary>
    /// Draws the image
    /// </summary>
    private void PaintBitmap(PaintEventArgs paintEventArgs)
    {
        if (_displayImage.Image == null)
        { return; }

        GraphicsState graphicsState = paintEventArgs.Graphics.Save();

        paintEventArgs.Graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
        paintEventArgs.Graphics.SmoothingMode = SmoothingMode.None;
        paintEventArgs.Graphics.PixelOffsetMode = PixelOffsetMode.Half;

        paintEventArgs.Graphics.DrawImage(
            image: _displayImage.Image,
            rect: _virtualDisplay.PaintRect);

        paintEventArgs.Graphics.Restore(graphicsState);
    }


    /// <summary>
    /// Draws a low-contrast red cross-hatch over the client area when no image is loaded.
    /// </summary>
    private void PaintNoImageHatch(PaintEventArgs paintEventArgs)
    {
        using var hatchBrush = new HatchBrush(
            HatchStyle.LargeGrid,
            foreColor: Color.FromArgb(255, 210, 160, 160),
            backColor: BackColor);
        paintEventArgs.Graphics.FillRectangle(hatchBrush, ClientRectangle);
    }


    /// <summary>
    /// Use has used the mouse wheel - we use this for zoom
    /// </summary>
    protected override void OnMouseWheel(MouseEventArgs e)
    {
        if (e == null) { throw new ArgumentNullException(nameof(e)); }

        base.OnMouseWheel(e);

        if (AnythingToDisplay)
        {
            Point mouseLocationInDisplayUnits = e.Location;
            PointF mouseLocationInImageUnits = MapDisplayToImageIgnoringScaleFactor(mouseLocationInDisplayUnits);

            _zoomManager.OnMouseWheel(
                imageDisplayMode: DisplayMode,
                currentZoom: _virtualDisplay.Zoom,
                mouseLocationInDisplayUnits: mouseLocationInDisplayUnits,
                mouseLocationInImageUnits: mouseLocationInImageUnits,
                mouseEventArgs: e);
        }
    }


    /// <summary>
    /// Size has changed - refresh the display
    /// </summary>
    protected override void OnSizeChanged(EventArgs e)
    {
        base.OnSizeChanged(e);
        _virtualDisplay.DisplaySize = ClientSize;
    }


    /// <summary>
    /// Mouse has moved - we can use this for dragging
    /// </summary>
    protected override void OnMouseMove(MouseEventArgs e)
    {
        if (e == null) { throw new ArgumentNullException(nameof(e)); }

        base.OnMouseMove(e);

        if (_dragManager.IsDragging)
        {
            _dragManager.OnMouseMove(e);
        }
    }


    /// <summary>
    /// Mouse button down - we can use this for dragging
    /// </summary>
    protected override void OnMouseDown(MouseEventArgs e)
    {
        if (e == null) { throw new ArgumentNullException(nameof(e)); }

        base.OnMouseDown(e);

        if (AnythingToDisplay && IsDraggingAllowed)
        {
            _dragManager.OnMouseDown(
                imageDisplayMode: DisplayMode,
                mouseEventArgs: e,
                currentTargetDisplayCentre: _virtualDisplay.TargetDisplayCentre);
        }
    }


    /// <summary>
    /// Mouse button is up - we can use this for dragging
    /// </summary>
    protected override void OnMouseUp(MouseEventArgs e)
    {
        if (e == null) { throw new ArgumentNullException(nameof(e)); } base.OnMouseUp(e);

        base.OnMouseUp(e);

        if (AnythingToDisplay && _dragManager.IsDragging)
        {
            _dragManager.OnMouseUp(e);
        }
    }


    /// <summary>
    /// Reset the zoom to 1:1
    /// </summary>
    public void ResetZoom() => _virtualDisplay.Zoom = 1;


    /// <summary>
    /// Zoom in
    /// </summary>
    public void ZoomIn() => _virtualDisplay.Zoom *= 2.0f;


    /// <summary>
    /// Zoom out
    /// </summary>
    public void ZoomOut() => _virtualDisplay.Zoom /= 2.0f;


    /// <summary>
    /// Synchronise the zoom and target display centre of this display
    /// from another display.
    /// </summary>
    public void SyncPaintRectFromOther(BitmapDisplayPanel sender)
    {
        if (sender == null) { throw new ArgumentNullException(nameof(sender)); }

        Zoom = sender.Zoom;
        TargetDisplayCentre = sender.TargetDisplayCentre;
        TargetImageCentre = sender.TargetImageCentre;
    }


    /// <summary>
    /// Maps a distance, in image units, to display units
    /// </summary>
    public int MapImageToDisplay(float imageDistance) =>
        (int)Math.Round(_virtualDisplay.MapImageToDisplay(imageDistance: imageDistance));


    PointF ICoordinateMapper.MapPoint(PointF point, DisplayPixelAlign pixelAdjust) =>
        MapImageToDisplay(point, pixelAdjust);

    RectangleF ICoordinateMapper.MapRect(RectangleF rect, DisplayPixelAlign pixelAdjust) =>
        MapImageToDisplayF(rect, pixelAdjust);

    float ICoordinateMapper.MapDistance(float distance) =>
        MapImageToDisplay(distance);
}
