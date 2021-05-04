using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;


namespace CDS.Imaging.WinForms.BitmapDisplay
{
    /// <summary>
    /// Displays a bitmap
    /// </summary>
    public partial class BitmapDisplayPanel : UserControl, IBitmapDisplay
    {
        private const string categoryCDS = "CDS";
        private Image? image;
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
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public IBitmapDisplay CDS => this;


        /// <inheritdoc/>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public BitmapDisplayMetrics TimingMetrics { get; } = new BitmapDisplayMetrics();


        /// <inheritdoc/>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public SizeF SizeOfHalfDisplayPixel => virtualDisplay.SizeOfHalfDisplayPixel;


        /// <inheritdoc/>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public RectangleF PaintRect => virtualDisplay.PaintRect;


        /// <inheritdoc/>
        [Category(categoryCDS)]
        [Description(
            "Called when the image has been rendered; gives a client an opportunity " +
            "to paint flicker-free graphics on top of the image.")]
        public event PaintOverEvent? PaintOver;


        /// <inheritdoc/>
        [Category(categoryCDS)]
        [Description(
            "Called afer the background has been painted and before the image been painted; " +
            "to paint flicker-free graphics under of the image.")]
        public event PaintUnderEvent? PaintUnder;


        /// <inheritdoc/>
        [Category(categoryCDS)]
        [Description("Called when the display mode is changed.")]
        public event ModeChangedEvent? DisplayModeChanged;


        /// <inheritdoc/>
        [Category(categoryCDS)]
        public Image? Image
        {
            get => image;

            set
            {
                if (image == value) { return; }

                image = value;
                virtualDisplay.ImageSize = (image == null) ? Size.Empty : image.Size;
                Invalidate();               
            }
        }


        /// <inheritdoc/>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool AnythingToDisplay => virtualDisplay.AnythingToDisplay;


        /// <inheritdoc/>
        [Category(categoryCDS)]
        [Description("The image display mode")]
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
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public PointF TargetImageCentre
        {
            get => virtualDisplay.TargetImageCentre;
            set => virtualDisplay.TargetImageCentre = value;
        }


        /// <inheritdoc/>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
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
            if (image == null) { return; }

            var graphicsState = paintEventArgs.Graphics.Save();

            paintEventArgs.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
            paintEventArgs.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;
            paintEventArgs.Graphics.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.Half;

            try
            {
                paintEventArgs.Graphics.DrawImage(
                    image: image,
                    rect: virtualDisplay.PaintRect);
            }
            catch (ObjectDisposedException)
            {
                Image = null;
            }

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
