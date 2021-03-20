using System;
using System.Drawing;
using System.Windows.Forms;

namespace CDS.Imaging.WinForms.BitmapDisplay
{
    class ImageDisplayZoomManager
    {
        Action<RectangleF> SetNewRenderRect;


        public ImageDisplayZoomManager(Action<RectangleF> setNewRenderRect)
        {
            SetNewRenderRect = setNewRenderRect;
        }


        public void OnMouseWheel(ImageDisplayMode imageDisplayMode, Size imageSize, RectangleF renderRect, MouseEventArgs mouseEventArgs)
        {
            if ((mouseEventArgs.Delta == 0) || (imageDisplayMode != ImageDisplayMode.Free)) { return; }

            var currentZoom = ImageDisplayMaths.CalcZoom(
                imageWidth: imageSize.Width,
                renderWidth: renderRect.Width);

            var imageLocation = ImageDisplayMaths.ImageLocationFromDisplayLocation(
                displayLocation: mouseEventArgs.Location,
                imageSize: imageSize,
                renderRect: renderRect);

            var change = mouseEventArgs.Delta;
            var zoomIn = (change > 0);
            var changeFactor = 1.0 + (Math.Abs(change) / 100.0);

            if (change > 0)
            {
                ZoomIn(imageSize, mouseEventArgs, currentZoom, imageLocation, changeFactor);
            }
            else if (change < 0)
            {
                ZoomOut(imageSize, mouseEventArgs, currentZoom, imageLocation, changeFactor);
            }
        }


        private void ZoomOut(Size imageSize, MouseEventArgs mouseEventArgs, double currentZoom, PointF imageLocation, double changeFactor)
        {
            var newZoom = ClipZoom(currentZoom / changeFactor);
            System.Diagnostics.Debug.WriteLine($"ZoomOut: {changeFactor}: {currentZoom} => {newZoom}");


            var newRenderRect = ImageDisplayMaths.CalcDrawRect(
                imageSize: imageSize,
                imageZoom: newZoom,
                targetDisplayCentre: mouseEventArgs.Location,
                targetImageCentre: imageLocation);

            SetNewRenderRect(newRenderRect);
        }

        private void ZoomIn(Size imageSize, MouseEventArgs mouseEventArgs, double currentZoom, PointF imageLocation, double changeFactor)
        {
            var newZoom = ClipZoom(currentZoom * changeFactor);
            System.Diagnostics.Debug.WriteLine($"ZoomIn: {changeFactor}: {currentZoom} => {newZoom}");

            var newRenderRect = ImageDisplayMaths.CalcDrawRect(
                imageSize: imageSize,
                imageZoom: newZoom,
                targetDisplayCentre: mouseEventArgs.Location,
                targetImageCentre: imageLocation);

            SetNewRenderRect(newRenderRect);
        }

        private double ClipZoom(double zoom) => Math.Max(0.01, Math.Min(100, zoom));
    }
}
