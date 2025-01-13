using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;


namespace CDS.Imaging.WinForms.BitmapDisplay
{
    /// <summary>
    /// Displays a bitmap
    /// </summary>
    public partial class BitmapDisplayPanel : UserControl
    {
        private const string categoryCDS = "CDS";

        private ImageWrapper displayImage = new ImageWrapper();
        private ImageWrapper pendingDisplayImage = new ImageWrapper();
        private object imageLock = new object();
        private VirtualDisplay virtualDisplay;
        private System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
        private DragManager dragManager;
        private ZoomManager zoomManager;
        private bool isWaitingToApplyPendingImage;


        /// <summary>
        /// Greater than 0 if dragging is suppressed.
        /// </summary>
        /// <remarks>
        /// This is useful if a something is hooking mouse down/move/up events and wants
        /// to prevent the image being dragged around while they are performing their
        /// own operation (such as selected a region of interest).
        /// </remarks>
        private int suppressDraggingCounter;


        /// <summary>
        /// True if dragging is allowed
        /// </summary>
        private bool IsDraggingAllowed => suppressDraggingCounter == 0;


        /// <summary>
        /// Timing metrics
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Category(categoryCDS)]
        public BitmapDisplayMetrics TimingMetrics { get; } = new BitmapDisplayMetrics();


        /// <summary>
        /// The size of half a displayed pixel
        /// </summary>
        /// <remarks>
        /// Use this as an offset when drawing with a large zoom and where the 
        /// drawing locations should be in the middle of an image pixel. E.g. with
        /// a zoom of 11, each image pixel will take 11*11 pixels on the screen. The
        /// half pixel size will be 5.5. Calling <see cref="MapImageToDisplay(PointF)"/>
        /// will return the location of the top-left of this 11*11 block for a particular
        /// image pixel; adding this offset of 5.5 will allow drawing to start in the middle
        /// of this 11*11 block.
        /// </remarks>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Category(categoryCDS)]
        public SizeF SizeOfHalfDisplayPixel => virtualDisplay.SizeOfHalfDisplayPixel;


        /// <summary>
        /// Gets the paint rectangle. This is based on the current image size,
        /// display size, zoom, target image centre and target display centre.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Category(categoryCDS)]
        public RectangleF PaintRect => virtualDisplay.PaintRect;


        /// <summary>
        /// Called when the image has been rendered; gives a client an opportunity
        /// to paint graphics on top of the image. This will be flicker-free as long
        /// as the control uses double buffering
        /// </summary>
        [Category(categoryCDS)]
        [Description(
            "Called when the image has been rendered; gives a client an opportunity " +
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
        public Bitmap? GetDisplayImage() => displayImage.Image;


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
            lock (imageLock)
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
            using (var imageSource = new BitmapImageSource(image))
            {
                SetImage(imageSource);
            }
        }


        /// <summary>
        /// Sets the new image as a pending image; then invokes an update
        /// method to get this pending image onto the display
        /// </summary>
        private void SetImageIndirectlyFromNonUIThread(IImageSource? imageSource)
        {
            // If we're still waiting to apply a previous non-UI image then
            // let's abandon this one; it's bad becuase we lose an image, but
            // it will stop us loading up the UI message loop with image updates
            // that we obviously can't service fast enough
            if (isWaitingToApplyPendingImage) { return; }


            // Store the new image in the pending image wrapper; we're protected
            // by our lock (in SetImage) so no one else can conflict with this 
            // wrapper
            isWaitingToApplyPendingImage = true;
            pendingDisplayImage.SetNewImage(imageSource);


            // Post the following action on the UI thread and return immedately;
            // this will release the lock (applied in SetImage). When the action
            // is picked up we'll apply the lock again, then take our wrapped
            // pending image and apply it directly as the displayed image.
            BeginInvoke(() =>
            {
                SetImage(pendingDisplayImage.Image);
                isWaitingToApplyPendingImage = false;
            });
        }


        /// <summary>
        /// We have a new image to display and we're on the UI thread meaning
        /// we won't be mid-paint; we can directly clone or copy the new image
        /// and repaint
        /// </summary>
        private void SetImageDirectlyFromUIThread(IImageSource? imageSource)
        {
            Size originalImageSize = virtualDisplay.ImageSize;

            displayImage.SetNewImage(imageSource);
            virtualDisplay.ImageSize = displayImage.ImageSize;

            if (originalImageSize != virtualDisplay.ImageSize)
            {
                OnImageSizeChanged?.Invoke(this, originalImageSize, virtualDisplay.ImageSize);
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
                displayImage?.Dispose();
                pendingDisplayImage?.Dispose();

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
        public bool AnythingToDisplay => virtualDisplay.AnythingToDisplay;


        /// <summary>
        /// The image display mode
        /// </summary>
        [Category(categoryCDS)]
        [Description("The image display mode")]
        public BitmapDisplayMode DisplayMode
        {
            get => virtualDisplay.Mode;
            set
            {
                if (virtualDisplay.Mode != value)
                {
                    virtualDisplay.Mode = value;
                    OnDisplayModeChanged?.Invoke(this);
                }
            }
        }


        /// <summary>
        /// Suppress dragging
        /// </summary>
        public void SuppressDragging()
        {
            suppressDraggingCounter++;
        }


        /// <summary>
        /// Allow dragging. (Note this will only have an effect if the counter is
        /// reduced to 0).
        /// </summary>
        public void UnsuppressDragging()
        {
            suppressDraggingCounter--;
        }


        /// <summary>
        /// The location in the image that should be rendered at 
        /// <see cref="TargetDisplayCentre"/>.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Category(categoryCDS)]
        public PointF TargetImageCentre
        {
            get => virtualDisplay.TargetImageCentre;
            set => virtualDisplay.TargetImageCentre = value;
        }


        /// <summary>
        /// The location on the display that should render the pixel in the image
        /// at location <see cref="TargetImageCentre"/>.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Category(categoryCDS)]
        public PointF TargetDisplayCentre
        {
            get => virtualDisplay.TargetDisplayCentre;
            set => virtualDisplay.TargetDisplayCentre = value;
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

            dragManager = new DragManager(DragManager_SetNewTargetDisplayCentre);
            zoomManager = new ZoomManager(ZoomManager_SetNewZoom);
            virtualDisplay = new VirtualDisplay(VirtualImageOnDisplay_OnPaintRectChanged);
        }


        /// <summary>
        /// Map an image point to a display point
        /// </summary>
        /// <param name="imagePoint">A location on the image, independent of where it's currently being displayed</param>
        /// <returns>
        /// A point on the display
        /// </returns>
        public Point MapImagePointToDisplayPoint(Point imagePoint)
        {
            return Point.Truncate(virtualDisplay.MapImageToDisplay(imagePoint));
        }


        /// <summary>
        /// Map an image rectangle to a display rectangle
        /// </summary>
        /// <param name="imageRectangle"></param>
        /// <returns></returns>
        public Rectangle MapImageRectangleToDisplayRectangle(Rectangle imageRectangle)
        {
            return Rectangle.Truncate(virtualDisplay.MapImageToDisplay(imageRectangle));
        }


        /// <summary>
        /// Map a display point to an image point
        /// </summary>
        /// <param name="displayPoint"></param>
        /// <returns></returns>
        public Point MapDisplayPointToImagePoint(Point displayPoint)
        {
            return Point.Truncate(virtualDisplay.MapDisplayToImage(displayPoint));
        }



        /// <summary>
        /// Drag manager wants to set a new display centre
        /// </summary>
        private void DragManager_SetNewTargetDisplayCentre(PointF targetDisplayCentre)
        {
            virtualDisplay.TargetDisplayCentre = targetDisplayCentre;
        }


        /// <summary>
        /// Zoom manager wants to set a new zoom
        /// </summary>
        private void ZoomManager_SetNewZoom(float zoom, PointF targetDisplayCentre, PointF targetImageCentre)
        {
            virtualDisplay.Zoom = zoom;
            virtualDisplay.TargetDisplayCentre = targetDisplayCentre;
            virtualDisplay.TargetImageCentre = targetImageCentre;
        }


        /// <summary>
        /// Set/get the zoom level. The limits in the <see cref="Consts"/> class are
        /// used.
        /// </summary>
        [Category(categoryCDS)]
        public float Zoom
        {
            get => virtualDisplay.Zoom;
            set => virtualDisplay.Zoom = value;
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
        /// Returns the image location where a pixel at <paramref name="imageLocation"/> would 
        /// have been drawn.
        /// </summary>
        /// <remarks>
        /// This is useful when you have used the mouse to select a region of interest
        /// (rectangle) over the image and want to deteremine the ROI with respect to 
        /// the image.
        /// </remarks>
        /// <param name="imageLocation">A region on the image</param>
        /// <returns>A region on the display or an empty rectangle if there's nothing to display</returns>
        public PointF MapImageToDisplay(PointF imageLocation)
        {
            return virtualDisplay.MapImageToDisplay(imageLocation);
        }


        /// <summary>
        /// Returns the image location where a rectangle at <paramref name="imageRect"/> would 
        /// have been drawn.
        /// </summary>
        /// <remarks>
        /// This is useful when you have used the mouse to select a region of interest
        /// (rectangle) over the image and want to deteremine the ROI with respect to 
        /// the image.
        /// </remarks>
        /// <param name="imageRect">A region on the image</param>
        /// <returns>A region on the display or an empty rectangle if there's nothing to display</returns>
        public RectangleF MapImageToDisplay(RectangleF imageRect)
        {
            return virtualDisplay.MapImageToDisplay(imageRect);
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
        public PointF MapDisplayToImage(PointF displayLocation)
        {
            return virtualDisplay.MapDisplayToImage(displayLocation);
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
        /// <returns>A region on the display or an empty rectangle if there's nothing to display</returns>
        public RectangleF MapDisplayToImage(RectangleF displayRect)
        {
            return virtualDisplay.MapDisplayToImage(displayRect);
        }


        /// <summary>
        /// Centres the image on the display, retaining the existing zoom level.
        /// Only applied if the display mode is <see cref="BitmapDisplayMode.Free"/>,
        /// no-op otherwise.
        /// </summary>
        public void CentreImage()
        {
            virtualDisplay.Centre();
        }


        /// <summary>
        /// Centres the image on the display and sets the zoom to 1.
        /// Only applied if the display mode is <see cref="BitmapDisplayMode.Free"/>,
        /// no-op otherwise.
        /// </summary>
        public void CentreImageActualSize()
        {
            virtualDisplay.ActualSizeCentred();
        }


        /// <summary>
        /// Centres the image on the display and adjusts the zoom so that the 
        /// image fills the display as much as possible.
        /// Only applied if the display mode is <see cref="BitmapDisplayMode.Free"/>,
        /// no-op otherwise.
        /// </summary>
        public void FitToWindowCentred()
        {
            virtualDisplay.FitToWindowCentred();
        }


        /// <summary>
        /// Paint the background (optimised when there's nothing to display)
        /// </summary>
        protected override void OnPaintBackground(PaintEventArgs e)
        {
            stopwatch.Restart();

            var clippedRenderRect = virtualDisplay.PaintRect;
            clippedRenderRect.Intersect(ClientRectangle);
            var shouldPaintBackground = virtualDisplay.PaintRect.IsEmpty || (e.ClipRectangle != clippedRenderRect);
            if (shouldPaintBackground)
            {
                base.OnPaintBackground(e);
            }

            stopwatch.Stop();
            TimingMetrics.BackgroundPaint = stopwatch.Elapsed;
        }


        /// <summary>
        /// Paint the image
        /// </summary>
        protected override void OnPaint(PaintEventArgs paintEventArgs)
        {
            stopwatch.Restart();

            OnPaintUnder?.Invoke(
                sender: this,
                graphics: paintEventArgs.Graphics);

            if (AnythingToDisplay)
            {
                PaintBitmap(paintEventArgs);
            }

            OnPaintOver?.Invoke(
                sender: this,
                graphics: paintEventArgs.Graphics);

            stopwatch.Stop();
            TimingMetrics.ForegroundPaint = stopwatch.Elapsed;
        }


        /// <summary>
        /// Draws the image
        /// </summary>
        private void PaintBitmap(PaintEventArgs paintEventArgs)
        {
            if (displayImage.Image == null) { return; }

            var graphicsState = paintEventArgs.Graphics.Save();

            paintEventArgs.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
            paintEventArgs.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;
            paintEventArgs.Graphics.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.Half;

            paintEventArgs.Graphics.DrawImage(
                image: displayImage.Image,
                rect: virtualDisplay.PaintRect);

            paintEventArgs.Graphics.Restore(graphicsState);
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

                zoomManager.OnMouseWheel(
                    imageDisplayMode: DisplayMode,
                    currentZoom: virtualDisplay.Zoom,
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
            virtualDisplay.DisplaySize = ClientSize;
        }


        /// <summary>
        /// Mouse has moved - we can use this for dragging
        /// </summary>
        protected override void OnMouseMove(MouseEventArgs mouseEventArgs)
        {
            if (dragManager.IsDragging)
            {
                dragManager.OnMouseMove(mouseEventArgs);
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
                dragManager.OnMouseDown(
                    imageDisplayMode: DisplayMode,
                    mouseEventArgs: mouseEventArgs,
                    currentTargetDisplayCentre: virtualDisplay.TargetDisplayCentre);
            }
        }


        /// <summary>
        /// Mouse button is up - we can use this for dragging
        /// </summary>
        protected override void OnMouseUp(MouseEventArgs mouseEventArgs)
        {
            base.OnMouseUp(mouseEventArgs);

            if (AnythingToDisplay && dragManager.IsDragging)
            {
                dragManager.OnMouseUp(mouseEventArgs);
            }
        }


        /// <summary>
        /// Reset the zoom to 1:1
        /// </summary>
        public void ResetZoom()
        {
            virtualDisplay.Zoom = 1;
        }


        /// <summary>
        /// Zoom in
        /// </summary>
        public void ZoomIn()
        {
            virtualDisplay.Zoom *= 2.0f;
        }


        /// <summary>
        /// Zoom out
        /// </summary>
        public void ZoomOut()
        {
            virtualDisplay.Zoom /= 2.0f;
        }


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
    }
}
