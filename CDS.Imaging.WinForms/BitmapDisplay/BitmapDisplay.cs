using System;
using System.Drawing;
using System.Windows.Forms;


namespace CDS.Imaging.WinForms.BitmapDisplay
{
    public partial class BitmapDisplay : UserControl, IBitmapDisplay
    {
        private BitmapOnDisplay bitmapOnDisplay;
        private System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
        private ImageDisplayDragManager dragManager;
        private ImageDisplayZoomManager zoomManager;

        public IBitmapDisplay CDS => this;

        public TimingMetrics TimingMetrics { get; } = new TimingMetrics();


        public event PaintOverEvent PaintOver;

        public Image Image => bitmapOnDisplay.Image;


        public ImageDisplayMode Mode
        {
            get => bitmapOnDisplay.Mode;
            set => bitmapOnDisplay.Mode = value;
        }


        public BitmapDisplay()
        {
            InitializeComponent();
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);

            bitmapOnDisplay = new BitmapOnDisplay(
                invalidateAll: () => Invalidate(),
                invalidateRect: (rect) => Invalidate(rect),
                getClientSize: () => ClientSize);

            dragManager = new ImageDisplayDragManager(bitmapOnDisplay.SetNewRenderRect);
            zoomManager = new ImageDisplayZoomManager(bitmapOnDisplay.SetNewRenderRect);
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

                bitmapOnDisplay?.Dispose();
                bitmapOnDisplay = null;
            }

            base.Dispose(disposing);
        }


        public void SetImage(Bitmap image)
        {
            bitmapOnDisplay.SetImage(image);
        }


        protected override void OnPaintBackground(PaintEventArgs e)
        {
            stopwatch.Restart();

            var clippedRenderRect = bitmapOnDisplay.DisplayRect;
            clippedRenderRect.Intersect(ClientRectangle);
            var shouldPaintBackground = bitmapOnDisplay.DisplayRect.IsEmpty || (e.ClipRectangle != clippedRenderRect);
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

            if (bitmapOnDisplay.AnythingToDisplay)
            {
                var graphicsState = paintEventArgs.Graphics.Save();

                paintEventArgs.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;

                paintEventArgs.Graphics.DrawImage(
                    image: bitmapOnDisplay.Image,
                    rect: bitmapOnDisplay.DisplayRect);

                paintEventArgs.Graphics.Restore(graphicsState);
            }

            PaintOver?.Invoke(
                sender: this,
                graphics: paintEventArgs.Graphics,
                imageSize: (bitmapOnDisplay.Image == null) ? Size.Empty : bitmapOnDisplay.Image.Size,
                renderRect: bitmapOnDisplay.DisplayRect);

            stopwatch.Stop();
            TimingMetrics.ForegroundPaint = stopwatch.Elapsed;
        }


        protected override void OnMouseWheel(MouseEventArgs mouseEventArgs)
        {
            base.OnMouseWheel(mouseEventArgs);

            if (bitmapOnDisplay.AnythingToDisplay)
            {
                zoomManager.OnMouseWheel(
                    imageDisplayMode: Mode,
                    imageSize: bitmapOnDisplay.Image.Size,
                    renderRect: bitmapOnDisplay.DisplayRect,
                    mouseEventArgs: mouseEventArgs);
            }
        }


        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            bitmapOnDisplay.OnClientSizeChanged();
        }


        protected override void OnMouseMove(MouseEventArgs mouseEventArgs)
        {
            base.OnMouseMove(mouseEventArgs);

            if (bitmapOnDisplay.AnythingToDisplay)
            {
                dragManager.OnMouseMove(mouseEventArgs);
            }
        }


        protected override void OnMouseDown(MouseEventArgs mouseEventArgs)
        {
            base.OnMouseDown(mouseEventArgs);

            if (bitmapOnDisplay.AnythingToDisplay)
            {
                dragManager.OnMouseDown(
                    Mode,
                    mouseEventArgs,
                    bitmapOnDisplay.DisplayRect);
            }
        }


        protected override void OnMouseUp(MouseEventArgs mouseEventArgs)
        {
            base.OnMouseUp(mouseEventArgs);

            if (bitmapOnDisplay.AnythingToDisplay)
            {
                dragManager.OnMouseUp(mouseEventArgs);
            }
        }

        public bool IsDisplayingImage => bitmapOnDisplay.AnythingToDisplay;


        public PointF DisplayLocationFrom(PointF imageLocation) =>
            bitmapOnDisplay.DisplayLocationFromImageLocation(imageLocation) ?? PointF.Empty;


        public PointF ImageLocationFrom(PointF displayLocation) =>
            bitmapOnDisplay.ImageLocationFromDisplayLocation(displayLocation) ?? PointF.Empty;


        public void FitToWindowCentred()
        {
            bitmapOnDisplay.FitToWindow();
        }

        public void ActualSizeCentred()
        {
            bitmapOnDisplay.ActualSizeCentred();
        }

        public void Centre()
        {
            bitmapOnDisplay.Centre();
        }
    }
}
