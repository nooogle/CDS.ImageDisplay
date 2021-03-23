using System;
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

        public bool AnythingToDisplay => !imageSize.IsEmpty && !displaySize.IsEmpty;


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


        public PointF? MapImageToDisplay(PointF imageLocation)
        {
            if (!AnythingToDisplay) { return null; }

            var displayLocation = BitmapDisplayControl.DisplayMaths.DisplayLocationFromImageLocation(
                imageLocation: imageLocation,
                imageSize: imageSize,
                paintRect: paintRect);

            return displayLocation;
        }


        public PointF? MapDisplayToImage(PointF displayLocation)
        {
            if (!AnythingToDisplay) { return null; }

            var imageLocation = BitmapDisplayControl.DisplayMaths.ImageLocationFromDisplayLocation(
                displayLocation: displayLocation,
                imageSize: imageSize,
                paintRect: paintRect);

            return imageLocation;
        }

        public RectangleF? MapDisplayToImage(RectangleF displayRect)
        {
            if (!AnythingToDisplay) { return null; }

            var bottomRight = new PointF(displayRect.Right, displayRect.Bottom);
            var imageTopLeft = MapDisplayToImage(displayRect.Location).Value;
            var imageBottomRight = MapDisplayToImage(bottomRight).Value;

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

        public RectangleF? MapImageToDisplay(RectangleF imageRect)
        {
            if(!AnythingToDisplay) { return null; }

            var bottomRight = new PointF(imageRect.Right, imageRect.Bottom);
            var displayTopLeft = MapImageToDisplay(imageRect.Location).Value;
            var displayBottomRight = MapImageToDisplay(bottomRight).Value;

            var displayRect = RectangleF.FromLTRB(
                left: displayTopLeft.X,
                top: displayTopLeft.Y,
                right: displayBottomRight.X,
                bottom: displayBottomRight.Y);

            return displayRect;
        }
    }
}
