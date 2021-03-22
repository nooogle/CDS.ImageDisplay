using System.Drawing;

namespace CDS.Imaging.WinForms
{
    public class VirtualImageOnDisplay
    {
        private Size imageSize;
        private Size displaySize;
        private RectangleF paintRect;
        private BitmapDisplayMode mode = BitmapDisplayMode.FitToWindowCentred;
        private System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();

        public event OnPaintRectChangedEvent OnPaintRectChanged;

        public BitmapDisplayMetrics TimingMetrics { get; } = new BitmapDisplayMetrics();

        bool AnythingToDisplay => !imageSize.IsEmpty && !displaySize.IsEmpty;


        public RectangleF PaintRect
        {
            get => paintRect;

            set
            {
                if (paintRect != value)
                {
                    paintRect = value;
                    OnPaintRectChanged?.Invoke(this, paintRect);
                }
            }
        }

        public Size ImageSize
        {
            get => imageSize;

            set
            {
                if (imageSize != value)
                {
                    imageSize = value;
                    RecalculatePaintRect();
                }
            }
        }


        public Size DisplaySize
        {
            get => displaySize;

            set
            {
                if (displaySize != value)
                {
                    displaySize = value;
                    RecalculatePaintRect();
                }
            }
        }


        public BitmapDisplayMode Mode
        {
            get => mode;

            set
            {
                if (mode != value)
                {
                    mode = value;
                    RecalculatePaintRect(displayMode: this.mode);
                }
            }
        }


        public VirtualImageOnDisplay()
        {
        }


        private void RecalculatePaintRect()
        {
            RecalculatePaintRect(displayMode: this.mode);
        }


        private void RecalculatePaintRect(BitmapDisplayMode displayMode)
        {
            if (AnythingToDisplay)
            {
                PaintRect = BitmapDisplayControl.DisplayMaths.CalcPaintRect(
                    displayMode,
                    imageSize: imageSize,
                    displaySize: displaySize,
                    existingPaintRect: paintRect);
            }
            else
            {
                PaintRect = Rectangle.Empty;
            }
        }



        public void MovePaintRect(PointF topLeft)
        {
            if (!AnythingToDisplay || (mode != BitmapDisplayMode.Free))
            {
                return;
            }

            PaintRect = new RectangleF(topLeft, paintRect.Size);
        }


        public PointF? DisplayLocationFromImageLocation(PointF imageLocation)
        {
            if (!AnythingToDisplay) { return null; }

            var drawingLocation = BitmapDisplayControl.DisplayMaths.DisplayLocationFromImageLocation(
                imageLocation: imageLocation,
                imageSize: imageSize,
                paintRect: paintRect);

            return drawingLocation;
        }


        public PointF? ImageLocationFromDisplayLocation(PointF displayLocation)
        {
            if (!AnythingToDisplay) { return null; }

            var imageLocation = BitmapDisplayControl.DisplayMaths.ImageLocationFromDisplayLocation(
                displayLocation: displayLocation,
                imageSize: imageSize,
                paintRect: paintRect);

            return imageLocation;
        }


        /// <summary>
        /// Centre the image and set to 1:1 zoom. Only applies if the 
        /// mode is <see cref="BitmapDisplayMode.Free"/>, otherwise does 
        /// nothing.
        /// </summary>
        public void ActualSizeCentred()
        {
            if (!AnythingToDisplay || (mode != BitmapDisplayMode.Free)) { return; }

            RecalculatePaintRect(displayMode: BitmapDisplayMode.ActualSizeCentred);
        }


        public void Centre()
        {
            if (!AnythingToDisplay || (mode != BitmapDisplayMode.Free)) { return; }

            PaintRect = BitmapDisplayControl.DisplayMaths.CalcCentredRect(
                displaySize: displaySize,
                existingRect: paintRect,
                imageSize: imageSize);
        }


        public void FitToWindowCentred()
        {
            if (!AnythingToDisplay || (mode != BitmapDisplayMode.Free)) { return; }

            RecalculatePaintRect(displayMode: BitmapDisplayMode.FitToWindowCentred);
        }
    }
}
