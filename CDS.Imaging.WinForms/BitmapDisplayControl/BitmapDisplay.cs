using System;
using System.Drawing;
using System.Windows.Forms;


namespace CDS.Imaging.WinForms
{
    public partial class BitmapDisplay : UserControl, IBitmapDisplay
    {
        private Bitmap displayBitmap;
        private VirtualImageOnDisplay virtualImageOnDisplay = new VirtualImageOnDisplay();
        private System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
        private BitmapDisplayControl.DragManager dragManager;
        private BitmapDisplayControl.ZoomManager zoomManager;

        public IBitmapDisplay CDS => this;

        public BitmapDisplayMetrics TimingMetrics { get; } = new BitmapDisplayMetrics();

        public RectangleF PaintRect => virtualImageOnDisplay.PaintRect;

        public event PaintOverEvent PaintOver;

        public Image Image => displayBitmap;


        public bool IsDisplayingImage => virtualImageOnDisplay.AnythingToDisplay;


        public BitmapDisplayMode Mode
        {
            get => virtualImageOnDisplay.Mode;
            set => virtualImageOnDisplay.Mode = value;
        }


        public BitmapDisplay()
        {
            InitializeComponent();
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);

            dragManager = new BitmapDisplayControl.DragManager(DragManager_SetNewTargetDisplayCentre);
            zoomManager = new BitmapDisplayControl.ZoomManager(ZoomManager_SetNewZoom);

            virtualImageOnDisplay.OnPaintRectChanged += VirtualImageOnDisplay_OnPaintRectChanged;
        }


        private void DragManager_SetNewTargetDisplayCentre(PointF targetDisplayCentre)
        {
            virtualImageOnDisplay.TargetDisplayCentre = targetDisplayCentre;
        }


        private void ZoomManager_SetNewZoom(float zoom, PointF targetDisplayCentre, PointF targetImageCentre)
        {
            virtualImageOnDisplay.Zoom = zoom;
            virtualImageOnDisplay.TargetDisplayCentre = targetDisplayCentre;
            virtualImageOnDisplay.TargetImageCentre = targetImageCentre;
        }


        private void VirtualImageOnDisplay_OnPaintRectChanged(VirtualImageOnDisplay sender, RectangleF paintRect)
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


        private void DropBitmap()
        {
            displayBitmap?.Dispose();
            displayBitmap = null;
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

            Invalidate(Rectangle.Round(virtualImageOnDisplay.PaintRect));
        }


        private void CreateNewBitmapFromBitmap(Bitmap newBitmap)
        {
            displayBitmap = (Bitmap)newBitmap.Clone();
            virtualImageOnDisplay.ImageSize = displayBitmap.Size;
            Invalidate();
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


        public PointF MapImageToDisplay(PointF imageLocation)
        {
            return virtualImageOnDisplay.MapImageToDisplay(imageLocation);
        }

        public RectangleF MapImageToDisplay(RectangleF imageRect)
        {
            return virtualImageOnDisplay.MapImageToDisplay(imageRect);
        }


        public PointF MapDisplayToImage(PointF displayLocation)
        {
            return virtualImageOnDisplay.MapDisplayToImage(displayLocation);
        }

        
        public RectangleF MapDisplayToImage(RectangleF displayRect)
        {
            return virtualImageOnDisplay.MapDisplayToImage(displayRect);
        }


        /// <summary>
        /// Centre the image and set to 1:1 zoom. Only applies if the 
        /// mode is <see cref="BitmapDisplayMode.Free"/>, otherwise does 
        /// nothing.
        /// </summary>
        public void ActualSizeCentred()
        {
            virtualImageOnDisplay.ActualSizeCentred();
        }

        public void Centre()
        {
            virtualImageOnDisplay.Centre();
        }


        public void FitToWindowCentred()
        {
            virtualImageOnDisplay.FitToWindowCentred();
        }


        protected override void OnPaintBackground(PaintEventArgs e)
        {
            stopwatch.Restart();

            var clippedRenderRect = virtualImageOnDisplay.PaintRect;
            clippedRenderRect.Intersect(ClientRectangle);
            var shouldPaintBackground = virtualImageOnDisplay.PaintRect.IsEmpty || (e.ClipRectangle != clippedRenderRect);
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

            if (IsDisplayingImage)
            {
                PaintBitmap(paintEventArgs);
            }

            PaintOver?.Invoke(
                sender: this,
                graphics: paintEventArgs.Graphics,
                imageSize: (displayBitmap == null) ? Size.Empty : displayBitmap.Size,
                renderRect: virtualImageOnDisplay.PaintRect);

            stopwatch.Stop();
            TimingMetrics.ForegroundPaint = stopwatch.Elapsed;
        }


        private void PaintBitmap(PaintEventArgs paintEventArgs)
        {
            var graphicsState = paintEventArgs.Graphics.Save();

            paintEventArgs.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
            paintEventArgs.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;
            paintEventArgs.Graphics.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.Half;

            paintEventArgs.Graphics.DrawImage(
                image: displayBitmap,
                rect: virtualImageOnDisplay.PaintRect);

            paintEventArgs.Graphics.Restore(graphicsState);
        }


        protected override void OnMouseWheel(MouseEventArgs mouseEventArgs)
        {
            base.OnMouseWheel(mouseEventArgs);

            if (IsDisplayingImage)
            {
                var mouseLocationInDisplayUnits = mouseEventArgs.Location;
                var mouseLocationInImageUnits = MapDisplayToImage(mouseLocationInDisplayUnits);

                zoomManager.OnMouseWheel(
                    imageDisplayMode: Mode,
                    currentZoom: virtualImageOnDisplay.Zoom,
                    mouseLocationInDisplayUnits: mouseLocationInDisplayUnits,
                    mouseLocationInImageUnits: mouseLocationInImageUnits,
                    mouseEventArgs: mouseEventArgs);
            }
        }


        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            virtualImageOnDisplay.DisplaySize = ClientSize;
        }


        protected override void OnMouseMove(MouseEventArgs mouseEventArgs)
        {
            base.OnMouseMove(mouseEventArgs);

            if (IsDisplayingImage)
            {
                dragManager.OnMouseMove(mouseEventArgs);
            }
        }


        protected override void OnMouseDown(MouseEventArgs mouseEventArgs)
        {
            base.OnMouseDown(mouseEventArgs);

            if (IsDisplayingImage)
            {
                dragManager.OnMouseDown(
                    imageDisplayMode: Mode,
                    mouseEventArgs: mouseEventArgs,
                    currentTargetDisplayCentre: virtualImageOnDisplay.TargetDisplayCentre);
            }
        }


        protected override void OnMouseUp(MouseEventArgs mouseEventArgs)
        {
            base.OnMouseUp(mouseEventArgs);

            if (IsDisplayingImage)
            {
                dragManager.OnMouseUp(mouseEventArgs);
            }
        }
    }
}
