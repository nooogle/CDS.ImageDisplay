using System;
using System.Drawing;

namespace CDS.Imaging.WinForms
{
    public class VirtualImageOnDisplay
    {
        private Size imageSize;
        private Size displaySize;
        private PointF targetImageCentre;
        private PointF targetDisplayCentre;
        private float zoom = 1;
        private RectangleF paintRect;
        private BitmapDisplayMode mode = BitmapDisplayMode.FitToWindowCentred;
        private System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();

        public event OnPaintRectChangedEvent OnPaintRectChanged;

        public BitmapDisplayMetrics TimingMetrics { get; } = new BitmapDisplayMetrics();

        public bool AnythingToDisplay => !imageSize.IsEmpty && !displaySize.IsEmpty;

        public float Zoom
        {
            get => zoom;

            set
            {
                if(zoom != value)
                {
                    zoom = value;
                    RecalculatePaintRect();
                }
            }
        }

        public PointF TargetImageCentre
        {
            get => targetImageCentre;

            set
            {
                if(targetImageCentre != value)
                {
                    targetImageCentre = value;
                    RecalculatePaintRect();
                }
            }
        }

        public PointF TargetDisplayCentre
        {
            get => targetDisplayCentre;

            set
            {
                if(targetDisplayCentre != value)
                {
                    targetDisplayCentre = value;
                    RecalculatePaintRect();
                }
            }
        }


        public RectangleF PaintRect
        {
            get => paintRect;

            private set
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
                    targetImageCentre = new PointF(imageSize.Width / 2.0f, imageSize.Height / 2.0f);
                    ForceApplyCurrentAutomaticMode();
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
                    targetDisplayCentre = new PointF(displaySize.Width / 2.0f, displaySize.Height / 2.0f);
                    ForceApplyCurrentAutomaticMode();
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
                    ForceApplyCurrentAutomaticMode();
                }
            }
        }


        private void ForceApplyCurrentAutomaticMode()
        {
            if(!AnythingToDisplay) { return; }

            switch (mode)
            {
                case BitmapDisplayMode.ActualSizeCentred:
                    ForceActualSizeCentred();
                    break;

                case BitmapDisplayMode.FitToWindowCentred:
                    ForceFitToWindowCentred();
                    break;

                default:
                    break;
            }
        }

        public VirtualImageOnDisplay()
        {
        }


        private void RecalculatePaintRect()
        {
            if (AnythingToDisplay)
            {
                PaintRect = new RectangleF(
                    x: targetDisplayCentre.X - (targetImageCentre.X * zoom),
                    y: targetDisplayCentre.Y - (targetImageCentre.Y * zoom),
                    width: imageSize.Width * zoom,
                    height: imageSize.Height * zoom);
            }
            else
            {
                PaintRect = Rectangle.Empty;
            }
        }


        private static double DisplayLocationFromImageLocation1D(
            double imageLocation,
            double imageSize,
            double paintLocation,
            double paintSize)
        {
            var displayLocation = paintLocation + (imageLocation / imageSize * paintSize);
            return displayLocation;
        }


        public PointF MapImageToDisplay(PointF imageLocation)
        {
            if (!AnythingToDisplay) { return PointF.Empty; }

            var x = DisplayLocationFromImageLocation1D(
                imageLocation: imageLocation.X,
                imageSize: imageSize.Width,
                paintLocation: paintRect.X,
                paintSize: paintRect.Width);

            var y = DisplayLocationFromImageLocation1D(
                imageLocation: imageLocation.Y,
                imageSize: imageSize.Height,
                paintLocation: paintRect.Y,
                paintSize: paintRect.Height);

            return new PointF((float)x, (float)y);
        }


        private static double ImageLocationFromDisplayLocation1D(
            double displayLocation,
            double imageSize,
            double paintLocation,
            double paintSize)
        {
            var zoom = paintSize / imageSize;
            var imageLocation = (displayLocation - paintLocation) / zoom;
            return imageLocation;
        }


        public PointF MapDisplayToImage(PointF displayLocation)
        {
            if (!AnythingToDisplay) { return PointF.Empty; }

            var x = ImageLocationFromDisplayLocation1D(
                displayLocation: displayLocation.X,
                imageSize: imageSize.Width,
                paintLocation: paintRect.X,
                paintSize: paintRect.Width);

            var y = ImageLocationFromDisplayLocation1D(
                displayLocation: displayLocation.Y,
                imageSize: imageSize.Height,
                paintLocation: paintRect.Y,
                paintSize: paintRect.Height);

            var imageLocation = new PointF((float)x, (float)y);

            return imageLocation;
        }

        public RectangleF MapDisplayToImage(RectangleF displayRect)
        {
            if (!AnythingToDisplay) { return RectangleF.Empty; }

            var bottomRight = new PointF(displayRect.Right, displayRect.Bottom);
            var imageTopLeft = MapDisplayToImage(displayRect.Location);
            var imageBottomRight = MapDisplayToImage(bottomRight);

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
            if (AnythingToDisplay && (mode == BitmapDisplayMode.Free))
            {
                ForceActualSizeCentred();
            }
        }

        private void ForceActualSizeCentred()
        {
            zoom = 1;
            ForceCentre();
        }


        public void Centre()
        {
            if (AnythingToDisplay && (mode == BitmapDisplayMode.Free))
            {
                ForceCentre();
            }
        }

        private void ForceCentre()
        {
            targetDisplayCentre = new PointF(displaySize.Width / 2.0f, displaySize.Height / 2.0f);
            targetImageCentre = new PointF(imageSize.Width / 2.0f, imageSize.Height / 2.0f);
            RecalculatePaintRect();
        }

        public void FitToWindowCentred()
        {
            if (AnythingToDisplay && (mode == BitmapDisplayMode.Free))
            {
                ForceFitToWindowCentred();
            }
        }


        private void ForceFitToWindowCentred()
        {
            var imageToDisplayHorizRatio = (double)imageSize.Width / displaySize.Width;
            var imageToDisplayVerticalRatio = (double)imageSize.Height / displaySize.Height;


            if (imageToDisplayHorizRatio < imageToDisplayVerticalRatio)
            {
                zoom = (float)displaySize.Height / imageSize.Height;
            }
            else
            {
                zoom = (float)displaySize.Width / imageSize.Width;
            }

            ForceCentre();
        }

        public RectangleF MapImageToDisplay(RectangleF imageRect)
        {
            if(!AnythingToDisplay) { return RectangleF.Empty; }

            var bottomRight = new PointF(imageRect.Right, imageRect.Bottom);
            var displayTopLeft = MapImageToDisplay(imageRect.Location);
            var displayBottomRight = MapImageToDisplay(bottomRight);

            var displayRect = RectangleF.FromLTRB(
                left: displayTopLeft.X,
                top: displayTopLeft.Y,
                right: displayBottomRight.X,
                bottom: displayBottomRight.Y);

            return displayRect;
        }
    }
}
