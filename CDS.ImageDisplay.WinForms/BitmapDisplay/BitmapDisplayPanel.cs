using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace CDS.ImageDisplay.BitmapDisplay;

/// <summary>
/// Displays a bitmap
/// </summary>
public partial class BitmapDisplayPanel : UserControl
{
    private const string categoryCDS = "CDS";

    private ImageWrapper _displayImage = new();
    private ImageWrapper _pendingDisplayImage = new();
    private readonly object _imageLock = new();
    private VirtualDisplay _virtualDisplay;
    private Stopwatch _stopwatch = new();
    private DragManager _dragManager;
    private ZoomManager _zoomManager;
    private bool _isWaitingToApplyPendingImage;


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
    private bool IsDraggingAllowed => _suppressDraggingCounter == 0;


    /// <summary>
    /// Timing metrics
    /// </summary>
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [Category(categoryCDS)]
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
    [Category(categoryCDS)]
    public SizeF SizeOfHalfDisplayPixel => _virtualDisplay.SizeOfHalfDisplayPixel;


    /// <summary>
    /// Gets the size of the image currently being displayed, or <see cref="Size.Empty"/> if no image is loaded.
    /// </summary>
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [Category(categoryCDS)]
    public Size ImageSize => _virtualDisplay.ImageSize;


    /// <summary>
    /// Gets the paint rectangle. This is based on the current image size,
    /// display size, zoom, target image centre and target display centre.
    /// </summary>
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [Category(categoryCDS)]
    public RectangleF PaintRect => _virtualDisplay.PaintRect;


    /// <summary>
    /// Called when the image has been painted; gives a client an opportunity
    /// to paint graphics on top of the image. This will be flicker-free as long
    /// as the control uses double buffering
    /// </summary>
    [Category(categoryCDS)]
    [Description(
        "Called when the image has been painted; gives a client an opportunity " +
        "to paint flicker-free graphics on top of the image.")]
    public event PaintOverEvent? OnPaintOver;


    /// <summary>
    /// Called afer the background has been painted and before the image been painted;
    /// gives a client an opportunity to paint graphics under the image. 
    /// This will be flicker-free as long as the control uses double buffering
    /// </summary>
    [Category(categoryCDS)]
    [Description(
        "Called afer the background has been painted and before the image been painted; " +
        "to paint flicker-free graphics under of the image.")]
    public event PaintUnderEvent? OnPaintUnder;


    /// <summary>
    /// Called when the display mode is changed.
    /// </summary>
    [Category(categoryCDS)]
    [Description("Called when the display mode is changed.")]
    public event ModeChangedEvent? OnDisplayModeChanged;


    /// <summary>
    /// Fired when the paint rectangle of the display is changed
    /// </summary>
    [Category(categoryCDS)]
    [Description("Called when the paint rectangle is changed.")]
    public event PaintRectChangedEvent? OnPaintRectChanged;


    /// <summary>
    /// Fired when the image size changes
    /// </summary>
    public event OnImageSizeChangedEvent? OnImageSizeChanged;


    /// <summary>
    /// Gets the image currently being displayed. 
    /// </summary>
    /// <remarks>
    /// The display owns this image and may dispose it at any time if a new
    /// (pending) image is being swapped in; therefore, callers should
    /// use this method with caution since it's more of a diagnostics 
    /// tool than for sharing image data.
    /// </remarks>
    [Category(categoryCDS)]
    public Bitmap? GetDisplayImage() => _displayImage.Image;


    /// <summary>
    /// A copy of the image is taken and then set as image the image to be displayd.
    /// This takes immediate effect when called  from the UI thread, 
    /// otherwise takes place asap by invoking an update procedure on the UI thread 
    /// and returning immediately.
    /// </summary>
    /// <remarks>
    /// The <see cref="TargetImageCentre"/> is reset if an image is currently being 
    /// displayed and a new image of a different size is set.
    /// 
    /// When called on a non-UI thread this returns immediately; it will be a small
    /// amout of time later that the image is finally set as the display image.
    /// </remarks>
    [Category(categoryCDS)]
    public void SetImage(IImageSource? imageSource)
    {
        lock (_imageLock)
        {
            if (InvokeRequired)
            {
                SetImageIndirectlyFromNonUIThread(imageSource);
            }
            else
            {
                SetImageDirectlyFromUIThread(imageSource);
            }
        }
    }


    /// <summary>
    /// A copy of the image is taken and then set as image the image to be displayd.
    /// This takes immediate effect when called  from the UI thread, 
    /// otherwise takes place asap by invoking an update procedure on the UI thread 
    /// and returning immediately.
    /// </summary>
    /// <remarks>
    /// The <see cref="TargetImageCentre"/> is reset if an image is currently being 
    /// displayed and a new image of a different size is set.
    /// 
    /// When called on a non-UI thread this returns immediately; it will be a small
    /// amout of time later that the image is finally set as the display image.
    /// </remarks>
    [Category(categoryCDS)]
    public void SetImage(Bitmap? image)
    {
        using var imageSource = new BitmapImageSource(image);
        SetImage(imageSource);
    }


    /// <summary>
    /// Sets the new image as a pending image; then invokes an update
    /// method to get this pending image onto the display
    /// </summary>
    private void SetImageIndirectlyFromNonUIThread(IImageSource? imageSource)
    {
        // Always store the latest frame so the UI thread picks up the most
        // recent image when it processes the pending invoke (last-writer-wins).
        // We're protected by the imageLock held in SetImage.
        _pendingDisplayImage.SetNewImage(imageSource);

        // If a BeginInvoke is already queued to apply the pending image,
        // don't post another one — the existing callback will use the image
        // we just stored above. This keeps at most one callback pending,
        // preventing message-loop buildup regardless of input frame rate.
        if (_isWaitingToApplyPendingImage) { return; }

        _isWaitingToApplyPendingImage = true;


        // Post the following action on the UI thread and return immedately;
        // this will release the lock (applied in SetImage). When the action
        // is picked up we'll re-acquire the lock so that the flag reset is
        // atomic with respect to non-UI threads checking it in SetImage.
        // Monitor.Enter is reentrant, so SetImage (which also locks _imageLock)
        // works correctly when called from within this outer lock.
        BeginInvoke(() =>
        {
            lock (_imageLock)
            {
                SetImage(_pendingDisplayImage.Image);
                _isWaitingToApplyPendingImage = false;
            }
        });
    }


    /// <summary>
    /// We have a new image to display and we're on the UI thread meaning
    /// we won't be mid-paint; we can directly clone or copy the new image
    /// and repaint
    /// </summary>
    private void SetImageDirectlyFromUIThread(IImageSource? imageSource)
    {
        Size originalImageSize = _virtualDisplay.ImageSize;

        _displayImage.SetNewImage(imageSource);
        _virtualDisplay.ImageSize = _displayImage.ImageSize;

        if (originalImageSize != _virtualDisplay.ImageSize)
        {
            OnImageSizeChanged?.Invoke(this, originalImageSize, _virtualDisplay.ImageSize);
        }

        Invalidate();
    }


    /// <summary> 
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _displayImage?.Dispose();
            _pendingDisplayImage?.Dispose();

            if (components != null)
            {
                components.Dispose();
            }
        }

        base.Dispose(disposing);
    }


    /// <summary>
    /// True if there's anything to display
    /// </summary>
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [Category(categoryCDS)]
    public bool AnythingToDisplay => _virtualDisplay.AnythingToDisplay;


    /// <summary>
    /// The image display mode
    /// </summary>
    [Category(categoryCDS)]
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
                OnDisplayModeChanged?.Invoke(this);
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
    [Category(categoryCDS)]
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
    [Category(categoryCDS)]
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
        _virtualDisplay = new VirtualDisplay(VirtualImageOnDisplay_OnPaintRectChanged);
    }


    /// <summary>
    /// Ensure arrow keys are treated as input keys
    /// </summary>
    protected override bool IsInputKey(Keys keyData)
    {
        if ((keyData & Keys.KeyCode) == Keys.Left ||
            (keyData & Keys.KeyCode) == Keys.Right ||
            (keyData & Keys.KeyCode) == Keys.Up ||
            (keyData & Keys.KeyCode) == Keys.Down)
        {
            return true; // Treat arrow keys as input keys
        }

        return base.IsInputKey(keyData);
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
    [Category(categoryCDS)]
    [DefaultValue(1.0f)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
    public float Zoom
    {
        get => _virtualDisplay.Zoom;
        set => _virtualDisplay.Zoom = value;
    }


    /// <summary>
    /// The virtual display wants to set a new paint rect
    /// </summary>
    private void VirtualImageOnDisplay_OnPaintRectChanged(VirtualDisplay sender, RectangleF paintRect)
    {
        Invalidate();
        OnPaintRectChanged?.Invoke(this);
    }


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
        var displayCoordinate = _virtualDisplay.MapImageToDisplay(imageLocation);

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
        var displayRect = _virtualDisplay.MapImageToDisplay(imageRect);

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
        var displayRect = _virtualDisplay.MapImageToDisplay(imageRect);

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
        _stopwatch.Restart();

        var clippedDrawingRect = _virtualDisplay.PaintRect;
        clippedDrawingRect.Intersect(ClientRectangle);
        var shouldPaintBackground = _virtualDisplay.PaintRect.IsEmpty || (e.ClipRectangle != Rectangle.Truncate(clippedDrawingRect));
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
    protected override void OnPaint(PaintEventArgs paintEventArgs)
    {
        _stopwatch.Restart();

        OnPaintUnder?.Invoke(
            sender: this,
            graphics: paintEventArgs.Graphics);

        if (AnythingToDisplay)
        {
            PaintBitmap(paintEventArgs);
        }
        else if (BackgroundImage is null)
        {
            PaintNoImageHatch(paintEventArgs);
        }

        OnPaintOver?.Invoke(
            sender: this,
            graphics: paintEventArgs.Graphics);

        _stopwatch.Stop();
        TimingMetrics.ForegroundPaint = _stopwatch.Elapsed;
    }


    /// <summary>
    /// Draws the image
    /// </summary>
    private void PaintBitmap(PaintEventArgs paintEventArgs)
    {
        if (_displayImage.Image == null) { return; }

        var graphicsState = paintEventArgs.Graphics.Save();

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
    protected override void OnMouseWheel(MouseEventArgs mouseEventArgs)
    {
        base.OnMouseWheel(mouseEventArgs);

        if (AnythingToDisplay)
        {
            var mouseLocationInDisplayUnits = mouseEventArgs.Location;
            var mouseLocationInImageUnits = MapDisplayToImage(mouseLocationInDisplayUnits);

            _zoomManager.OnMouseWheel(
                imageDisplayMode: DisplayMode,
                currentZoom: _virtualDisplay.Zoom,
                mouseLocationInDisplayUnits: mouseLocationInDisplayUnits,
                mouseLocationInImageUnits: mouseLocationInImageUnits,
                mouseEventArgs: mouseEventArgs);
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
    protected override void OnMouseMove(MouseEventArgs mouseEventArgs)
    {
        if (_dragManager.IsDragging)
        {
            _dragManager.OnMouseMove(mouseEventArgs);
        }
        else
        {
            base.OnMouseMove(mouseEventArgs);
        }
    }


    /// <summary>
    /// Mouse button down - we can use this for dragging
    /// </summary>
    protected override void OnMouseDown(MouseEventArgs mouseEventArgs)
    {
        base.OnMouseDown(mouseEventArgs);

        if (AnythingToDisplay && IsDraggingAllowed)
        {
            _dragManager.OnMouseDown(
                imageDisplayMode: DisplayMode,
                mouseEventArgs: mouseEventArgs,
                currentTargetDisplayCentre: _virtualDisplay.TargetDisplayCentre);
        }
    }


    /// <summary>
    /// Mouse button is up - we can use this for dragging
    /// </summary>
    protected override void OnMouseUp(MouseEventArgs mouseEventArgs)
    {
        base.OnMouseUp(mouseEventArgs);

        if (AnythingToDisplay && _dragManager.IsDragging)
        {
            _dragManager.OnMouseUp(mouseEventArgs);
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
        Zoom = sender.Zoom;
        TargetDisplayCentre = sender.TargetDisplayCentre;
        TargetImageCentre = sender.TargetImageCentre;
    }


    /// <summary>
    /// Maps a distance, in image units, to display units
    /// </summary>
    public int MapImageToDisplay(float imageDistance) =>
        (int)Math.Round(_virtualDisplay.MapImageToDisplay(imageDistance: imageDistance));
}
