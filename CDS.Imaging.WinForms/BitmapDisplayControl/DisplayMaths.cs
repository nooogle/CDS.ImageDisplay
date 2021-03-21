using System.Drawing;


namespace CDS.Imaging.WinForms.BitmapDisplayControl
{
    internal static class DisplayMaths
    {
        private static double DisplayLocationFromImageLocation1D(
            double imageLocation,
            double imageSize,
            double renderLocation,
            double renderSize)
        {
            var displayLocation = renderLocation + (imageLocation / imageSize * renderSize);
            return displayLocation;
        }


        public static PointF DisplayLocationFromImageLocation(
            PointF imageLocation,
            Size imageSize,
            RectangleF renderRect)
        {
            var x = DisplayLocationFromImageLocation1D(
                imageLocation: imageLocation.X,
                imageSize: imageSize.Width,
                renderLocation: renderRect.X,
                renderSize: renderRect.Width);

            var y = DisplayLocationFromImageLocation1D(
                imageLocation: imageLocation.Y,
                imageSize: imageSize.Height,
                renderLocation: renderRect.Y,
                renderSize: renderRect.Height);

            return new PointF((float)x, (float)y);
        }

        private static double ImageLocationFromDisplayLocation1D(
            double displayLocation,
            double imageSize,
            double renderLocation,
            double renderSize)
        {
            var zoom = renderSize / imageSize;
            var imageLocation = (displayLocation - renderLocation) / zoom;
            return imageLocation;
        }


        public static PointF ImageLocationFromDisplayLocation(
            PointF displayLocation, 
            Size imageSize, 
            RectangleF renderRect)
        {
            var x = ImageLocationFromDisplayLocation1D(
                displayLocation: displayLocation.X,
                imageSize: imageSize.Width,
                renderLocation: renderRect.X,
                renderSize: renderRect.Width);

            var y = ImageLocationFromDisplayLocation1D(
                displayLocation: displayLocation.Y,
                imageSize: imageSize.Height,
                renderLocation: renderRect.Y,
                renderSize: renderRect.Height);

            var imageLocation = new PointF((float)x, (float)y);
            return imageLocation;
        }


        /// <summary>
        /// Calcualtes the effective zoom level for the given 
        /// image size and rendering rectangle
        /// </summary>
        /// <returns>Effective zoom or 0 if not applicable</returns>
        public static double CalcZoom(int imageWidth, float renderWidth)
        {
            if(imageWidth <= 0.0f) { return 0; }

            var zoom = renderWidth / imageWidth;
            return zoom;
        }


        public static RectangleF CalcDrawRect(
            Size imageSize,
            double imageZoom,
            PointF targetDisplayCentre,
            PointF targetImageCentre)
        {
            var renderRect = new RectangleF(
                x: (float)(targetDisplayCentre.X - (targetImageCentre.X * imageZoom)),
                y: (float)(targetDisplayCentre.Y - (targetImageCentre.Y * imageZoom)),
                width: (float)(imageSize.Width * imageZoom),
                height: (float)(imageSize.Height * imageZoom));

            return renderRect;
        }


        public static RectangleF CalcActualSizeCentredRect(
            Size imageSize,
            Size displaySize)
        {
            var rect = new RectangleF(
                x: (displaySize.Width / 2) - (imageSize.Width / 2),
                y: (displaySize.Height / 2) - (imageSize.Height / 2),
                width: imageSize.Width,
                height: imageSize.Height);

            return rect;
        }


        public static RectangleF CalcFitToWindowRect(
            Size imageSize,
            Size displaySize)
        {
            double zoom;

            var imageToDisplayHorizRatio = (double)imageSize.Width / (double)displaySize.Width;
            var imageToDisplayVerticalRatio = (double)imageSize.Height / (double)displaySize.Height;


            if (imageToDisplayHorizRatio < imageToDisplayVerticalRatio)
            {
                zoom = (double)displaySize.Height / (double)imageSize.Height;
            }
            else
            {
                zoom = (double)displaySize.Width / (double)imageSize.Width;
            }

            var targetImageCentre = new Point(imageSize.Width / 2, imageSize.Height / 2);
            var targetDisplayCentre = new Point(displaySize.Width / 2, displaySize.Height / 2);

            var rect = CalcDrawRect(
                imageSize: imageSize,
                imageZoom: zoom,
                targetDisplayCentre: targetDisplayCentre,
                targetImageCentre: targetImageCentre);

            return rect;
        }

        


        public static RectangleF CalcRenderRect(
            BitmapDisplayMode mode, 
            Size imageSize, 
            Size displaySize, 
            RectangleF existingRenderRect)
        {
            RectangleF renderRect = existingRenderRect;

            if (renderRect.IsEmpty || (mode == BitmapDisplayMode.FitToWindowCentred))
            {
                renderRect = DisplayMaths.CalcFitToWindowRect(
                    imageSize: imageSize,
                    displaySize: displaySize);
            }
            else if (mode == BitmapDisplayMode.ActualSizeCentred)
            {
                renderRect = DisplayMaths.CalcActualSizeCentredRect(
                    imageSize: imageSize,
                    displaySize: displaySize);
            }

            return renderRect;
        }

        public static RectangleF CalcCentredRect(Size displaySize, RectangleF existingRect, Size imageSize)
        {
            var displayCentre = new PointF(displaySize.Width / 2, displaySize.Height / 2);
            var imageCentre = new PointF(imageSize.Width / 2, imageSize.Height / 2);
            var existingZoom = CalcZoom(imageWidth: imageSize.Width, renderWidth: existingRect.Width);

            var displayRect = CalcDrawRect(
                imageSize: imageSize,
                imageZoom: existingZoom,
                targetDisplayCentre: displayCentre,
                targetImageCentre: imageCentre);

            return displayRect;
        }
    }
}
