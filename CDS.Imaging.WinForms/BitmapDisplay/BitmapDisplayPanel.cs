using System;
using System.Drawing;
using System.Windows.Forms;


namespace CDS.Imaging.WinForms.BitmapDisplay
{
    /// <summary>
    /// Displays a bitmap
    /// </summary>
    public partial class BitmapDisplayPanel : UserControl, IBitmapDisplay
    {
        private Bitmap? displayBitmap;
        private VirtualDisplay virtualDisplay;
        private System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
        private DragManager dragManager;
        private ZoomManager zoomManager;


        /// <summary>
        /// Provides access to all the custom features
        /// </summary>
        /// <remarks>
        /// Allows code to use myPanel.CDS.XXX rather than myPanel.XXX - this makes
        /// it a little easier to discover and use the custom features of this panel 
        /// since a .Net control presents 100's of properties, events and methods!
        /// </remarks>
        public IBitmapDisplay CDS => this;


        /// <inheritdoc/>
        public BitmapDisplayMetrics TimingMetrics { get; } = new BitmapDisplayMetrics();


        /// <inheritdoc/>
        public SizeF SizeOfHalfDisplayPixel => virtualDisplay.SizeOfHalfDisplayPixel;


        /// <inheritdoc/>
        public RectangleF PaintRect => virtualDisplay.PaintRect;


        /// <inheritdoc/>
        public event PaintOverEvent? PaintOver;


        /// <inheritdoc/>
        public event PaintUnderEvent? PaintUnder;


        /// <inheritdoc/>
        public event ModeChangedEvent? DisplayModeChanged;


        /// <inheritdoc/>
        public Image? Image => displayBitmap as Image;


        /// <inheritdoc/>
        public bool AnythingToDisplay => virtualDisplay.AnythingToDisplay;


        /// <inheritdoc/>
        public BitmapDisplayMode Mode
        {
            get => virtualDisplay.Mode;
            set
            {
                if (virtualDisplay.Mode != value)
                {
                    virtualDisplay.Mode = value;
                    DisplayModeChanged?.Invoke(this);
                }
            }
        }


        /// <inheritdoc/>
        public PointF TargetImageCentre
        {
            get => virtualDisplay.TargetImageCentre;
            set => virtualDisplay.TargetImageCentre = value;
        }


        /// <inheritdoc/>
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


        /// <inheritdoc/>
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
        }


        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }

                DropBitmap();
            }

            base.Dispose(disposing);
        }


        /// <summary>
        /// Drop the current image
        /// </summary>
        private void DropBitmap()
        {
            displayBitmap?.Dispose();
            displayBitmap = null;
        }


        /// <inheritdoc/>
        public void ClearImage()
        {
            SetNullImage();
        }


        /// <inheritdoc/>
        public void SetImage(Bitmap image)
        {
            if (image == null)
            {
                SetNullImage();
            }
            else
            {
                SetNonNullBitmap(image);
            }
        }


        /// <summary>
        /// Takes a copy of a new image
        /// </summary>
        private void SetNonNullBitmap(Bitmap newBitmap)
        {
            DropBitmapIfFormatOrSizeChanged(newBitmap);
            var shouldCreateNewBitmap = (displayBitmap == null);

            if (shouldCreateNewBitmap)
            {
                CreateNewBitmapFromBitmap(newBitmap);
            }
            else
            {
                CopyBitmapToExistingBitmap(newBitmap);
            }
        }


        /// <summary>
        /// Copies the contents of a new image to our existing image store
        /// </summary>
        private void CopyBitmapToExistingBitmap(Bitmap image)
        {
            if(displayBitmap == null)
            {
                throw new NullReferenceException("The display bitmap has not been created!");
            }

            Rectangle rect = new Rectangle(0, 0, displayBitmap.Width, displayBitmap.Height);

            var existingBitmapData = displayBitmap.LockBits(rect, System.Drawing.Imaging.ImageLockMode.WriteOnly, image.PixelFormat);
            var newBitmapData = image.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadOnly, image.PixelFormat);

            try
            {
                var bytesToCopy = newBitmapData.Stride * newBitmapData.Height;

                unsafe
                {
                    Buffer.MemoryCopy(
                        source: existingBitmapData.Scan0.ToPointer(),
                        destination: newBitmapData.Scan0.ToPointer(),
                        destinationSizeInBytes: bytesToCopy,
                        sourceBytesToCopy: bytesToCopy);
                }
            }
            finally
            {
                image.UnlockBits(newBitmapData);
                displayBitmap.UnlockBits(existingBitmapData);
            }

            Invalidate(Rectangle.Round(virtualDisplay.PaintRect));
        }


        /// <summary>
        /// Creates a copy of the new image
        /// </summary>
        private void CreateNewBitmapFromBitmap(Bitmap newBitmap)
        {
            displayBitmap = (Bitmap)newBitmap.Clone();
            virtualDisplay.ImageSize = displayBitmap.Size;
            Invalidate();
        }


        /// <summary>
        /// Configures for no image
        /// </summary>
        private void SetNullImage()
        {
            DropBitmap();
            Invalidate();
        }


        /// <summary>
        /// Drops the current image store if the format or size is different
        /// to the new bitmap
        /// </summary>
        private void DropBitmapIfFormatOrSizeChanged(Bitmap image)
        {
            if (displayBitmap == null) { return; }

            var doesExistingBitmapFormatMatchNewBitmapFormat =
                (displayBitmap.PixelFormat == image.PixelFormat) &&
                (displayBitmap.Size == image.Size);

            if (!doesExistingBitmapFormatMatchNewBitmapFormat)
            {
                DropBitmap();
            }
        }


        /// <inheritdoc/>
        public PointF MapImageToDisplay(PointF imageLocation)
        {
            return virtualDisplay.MapImageToDisplay(imageLocation);
        }


        /// <inheritdoc/>
        public RectangleF MapImageToDisplay(RectangleF imageRect)
        {
            return virtualDisplay.MapImageToDisplay(imageRect);
        }


        /// <inheritdoc/>
        public PointF MapDisplayToImage(PointF displayLocation)
        {
            return virtualDisplay.MapDisplayToImage(displayLocation);
        }


        /// <inheritdoc/>
        public RectangleF MapDisplayToImage(RectangleF displayRect)
        {
            return virtualDisplay.MapDisplayToImage(displayRect);
        }


        /// <inheritdoc/>
        public void Centre()
        {
            virtualDisplay.Centre();
        }


        /// <inheritdoc/>
        public void ActualSizeCentred()
        {
            virtualDisplay.ActualSizeCentred();
        }


        /// <inheritdoc/>
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

            PaintUnder?.Invoke(
                sender: this,
                graphics: paintEventArgs.Graphics);

            if (AnythingToDisplay)
            {
                PaintBitmap(paintEventArgs);
            }

            PaintOver?.Invoke(
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
            if (displayBitmap == null)
            {
                throw new NullReferenceException("The display bitmap has not been created!");
            }

            var graphicsState = paintEventArgs.Graphics.Save();

            paintEventArgs.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
            paintEventArgs.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;
            paintEventArgs.Graphics.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.Half;

            paintEventArgs.Graphics.DrawImage(
                image: displayBitmap,
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
                    imageDisplayMode: Mode,
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
            base.OnMouseMove(mouseEventArgs);

            if (AnythingToDisplay)
            {
                dragManager.OnMouseMove(mouseEventArgs);
            }
        }


        /// <summary>
        /// Mouse button down - we can use this for dragging
        /// </summary>
        protected override void OnMouseDown(MouseEventArgs mouseEventArgs)
        {
            base.OnMouseDown(mouseEventArgs);

            if (AnythingToDisplay)
            {
                dragManager.OnMouseDown(
                    imageDisplayMode: Mode,
                    mouseEventArgs: mouseEventArgs,
                    currentTargetDisplayCentre: virtualDisplay.TargetDisplayCentre);
            }
        }


        /// <summary>
        /// Mouse button is up - we can use this for dragging
        /// </summary>
        /// <param name="mouseEventArgs"></param>
        protected override void OnMouseUp(MouseEventArgs mouseEventArgs)
        {
            base.OnMouseUp(mouseEventArgs);

            if (AnythingToDisplay)
            {
                dragManager.OnMouseUp(mouseEventArgs);
            }
        }



        /// <inheritdoc/>
        public void ResetZoom()
        {
            virtualDisplay.Zoom = 1;
        }


        /// <inheritdoc/>
        public void ZoomIn()
        {
            virtualDisplay.Zoom *= 2.0f;
        }



        /// <inheritdoc/>
        public void ZoomOut()
        {
            virtualDisplay.Zoom /= 2.0f;
        }

    }
}
