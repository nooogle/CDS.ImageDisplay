using System;
using System.Drawing;
using System.Windows.Forms;


namespace CDS.Imaging.WinForms
{
    public partial class BitmapDisplay : UserControl, IBitmapDisplay
    {
        private Bitmap displayBitmap;
        RectangleF DisplayRect;
        private BitmapDisplayMode mode = BitmapDisplayMode.FitToWindowCentred;
        private System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
        private BitmapDisplayControl.DragManager dragManager;
        private BitmapDisplayControl.ZoomManager zoomManager;

        public IBitmapDisplay CDS => this;

        public BitmapDisplayMetrics TimingMetrics { get; } = new BitmapDisplayMetrics();


        public event PaintOverEvent PaintOver;

        public Image Image => displayBitmap;


        bool AnythingToDisplay => !DisplayRect.IsEmpty;


        public BitmapDisplayMode Mode
        {
            get => mode;

            set
            {
                if (mode != value)
                {
                    mode = value;
                    RecalculateRenderRect(displayMode: this.mode);
                    Invalidate();
                }
            }
        }


        public BitmapDisplay()
        {
            InitializeComponent();
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);

            dragManager = new BitmapDisplayControl.DragManager(SetNewRenderRect);
            zoomManager = new BitmapDisplayControl.ZoomManager(SetNewRenderRect);
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


        private void DropBitmap()
        {
            displayBitmap?.Dispose();
            displayBitmap = null;
            DisplayRect = Rectangle.Empty;
        }


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

        private void SetNonNullBitmap(Bitmap newBitmap)
        {
            bool invalidateAll = true;

            DropBitmapIfFormatOrSizeChanged(newBitmap);

            var createNewBitmap = (displayBitmap == null);

            if (createNewBitmap)
            {
                CreateNewBitmapFromBitmap(newBitmap);
            }
            else
            {
                CopyBitmapToExistingBitmap(newBitmap);
                invalidateAll = false;
            }

            RecalculateRenderRect(displayMode: this.mode);

            if (invalidateAll)
            {
                Invalidate();
            }
            else
            {
                Invalidate(Rectangle.Round(DisplayRect));
            }
        }


        private void CopyBitmapToExistingBitmap(Bitmap image)
        {
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
        }


        private void CreateNewBitmapFromBitmap(Bitmap newBitmap)
        {
            displayBitmap = (Bitmap)newBitmap.Clone();
        }


        private void SetNullImage()
        {
            DropBitmap();
            Invalidate();
        }


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


        private void RecalculateRenderRect(BitmapDisplayMode displayMode)
        {
            if (displayBitmap != null)
            {
                DisplayRect = BitmapDisplayControl.DisplayMaths.CalcRenderRect(
                    displayMode,
                    imageSize: displayBitmap.Size,
                    displaySize: ClientSize,
                    existingRenderRect: DisplayRect);
            }
            else
            {
                DisplayRect = Rectangle.Empty;
            }
        }



        public void SetNewRenderRect(RectangleF newRenderRect)
        {
            if ((displayBitmap == null) || (DisplayRect == newRenderRect)) { return; }

            DisplayRect = newRenderRect;
            Invalidate();
        }


        public PointF? DisplayLocationFromImageLocation(PointF imageLocation)
        {
            if (!AnythingToDisplay) { return null; }

            var drawingLocation = BitmapDisplayControl.DisplayMaths.DisplayLocationFromImageLocation(
                imageLocation: imageLocation,
                imageSize: displayBitmap.Size,
                renderRect: DisplayRect);

            return drawingLocation;
        }


        public PointF? ImageLocationFromDisplayLocation(PointF displayLocation)
        {
            if (!AnythingToDisplay) { return null; }

            var imageLocation = BitmapDisplayControl.DisplayMaths.DisplayLocationFromImageLocation(
                imageLocation: displayLocation,
                imageSize: displayBitmap.Size,
                renderRect: DisplayRect);

            return imageLocation;
        }


        /// <summary>
        /// Centre the image and set to 1:1 zoom. Only applies if the 
        /// mode is <see cref="BitmapDisplayMode.Free"/>, otherwise does 
        /// nothing.
        /// </summary>
        public void ActualSizeCentred()
        {
            if (mode != BitmapDisplayMode.Free) { return; }

            var originalDisplayRect = DisplayRect;

            RecalculateRenderRect(displayMode: BitmapDisplayMode.ActualSizeCentred);

            if (originalDisplayRect != DisplayRect)
            {
                Invalidate();
            }
        }

        public void Centre()
        {
            if (mode != BitmapDisplayMode.Free) { return; }

            var originalDisplayRect = DisplayRect;

            if (displayBitmap != null)
            {
                DisplayRect = BitmapDisplayControl.DisplayMaths.CalcCentredRect(
                    displaySize: ClientSize,
                    existingRect: DisplayRect,
                    imageSize: displayBitmap.Size);
            }
            else
            {
                DisplayRect = Rectangle.Empty;
            }

            if (originalDisplayRect != DisplayRect)
            {
                Invalidate();
            }
        }


        public void FitToWindowCentred()
        {
            if (mode != BitmapDisplayMode.Free) { return; }

            var originalDisplayRect = DisplayRect;

            RecalculateRenderRect(displayMode: BitmapDisplayMode.FitToWindowCentred);

            if (originalDisplayRect != DisplayRect)
            {
                Invalidate();
            }
        }


        protected override void OnPaintBackground(PaintEventArgs e)
        {
            stopwatch.Restart();

            var clippedRenderRect = DisplayRect;
            clippedRenderRect.Intersect(ClientRectangle);
            var shouldPaintBackground = DisplayRect.IsEmpty || (e.ClipRectangle != clippedRenderRect);
            if (shouldPaintBackground)
            {
                base.OnPaintBackground(e);
            }

            stopwatch.Stop();
            TimingMetrics.BackgroundPaint = stopwatch.Elapsed;
        }



        protected override void OnPaint(PaintEventArgs paintEventArgs)
        {
            stopwatch.Restart();

            if (AnythingToDisplay)
            {
                var graphicsState = paintEventArgs.Graphics.Save();

                paintEventArgs.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;

                paintEventArgs.Graphics.DrawImage(
                    image: displayBitmap,
                    rect: DisplayRect);

                paintEventArgs.Graphics.Restore(graphicsState);
            }

            PaintOver?.Invoke(
                sender: this,
                graphics: paintEventArgs.Graphics,
                imageSize: (displayBitmap == null) ? Size.Empty : displayBitmap.Size,
                renderRect: DisplayRect);

            stopwatch.Stop();
            TimingMetrics.ForegroundPaint = stopwatch.Elapsed;
        }


        protected override void OnMouseWheel(MouseEventArgs mouseEventArgs)
        {
            base.OnMouseWheel(mouseEventArgs);

            if (AnythingToDisplay)
            {
                zoomManager.OnMouseWheel(
                    imageDisplayMode: Mode,
                    imageSize: displayBitmap.Size,
                    renderRect: DisplayRect,
                    mouseEventArgs: mouseEventArgs);
            }
        }


        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            RecalculateRenderRect(displayMode: this.mode);
            Invalidate();
        }


        protected override void OnMouseMove(MouseEventArgs mouseEventArgs)
        {
            base.OnMouseMove(mouseEventArgs);

            if (AnythingToDisplay)
            {
                dragManager.OnMouseMove(mouseEventArgs);
            }
        }


        protected override void OnMouseDown(MouseEventArgs mouseEventArgs)
        {
            base.OnMouseDown(mouseEventArgs);

            if (AnythingToDisplay)
            {
                dragManager.OnMouseDown(
                    Mode,
                    mouseEventArgs,
                    DisplayRect);
            }
        }


        protected override void OnMouseUp(MouseEventArgs mouseEventArgs)
        {
            base.OnMouseUp(mouseEventArgs);

            if (AnythingToDisplay)
            {
                dragManager.OnMouseUp(mouseEventArgs);
            }
        }

        public bool IsDisplayingImage => AnythingToDisplay;
    }
}
