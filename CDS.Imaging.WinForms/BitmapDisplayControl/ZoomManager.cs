using System;
using System.Drawing;
using System.Windows.Forms;

namespace CDS.Imaging.WinForms.BitmapDisplayControl
{
    internal class ZoomManager
    {
        private float minZoom;
        private float maxZoom;

        Action<float, PointF, PointF> SetNewZoom;


        public ZoomManager(
            Action<float, PointF, PointF> setNewZoom,
            float minZoom,
            float maxZoom)
        {
            this.minZoom = minZoom;
            this.maxZoom = maxZoom;
            SetNewZoom = setNewZoom;
        }


        public void OnMouseWheel(
            BitmapDisplayMode imageDisplayMode, 
            float currentZoom,
            PointF mouseLocationInDisplayUnits,
            PointF mouseLocationInImageUnits,
            MouseEventArgs mouseEventArgs)
        {
            if ((mouseEventArgs.Delta == 0) || (imageDisplayMode != BitmapDisplayMode.Free)) { return; }

            var change = mouseEventArgs.Delta;
            var changeFactor = 1.0f + (Math.Abs(change) / 100.0f);

            if (change > 0)
            {
                var newZoom = ClipZoom(currentZoom * changeFactor);
                SetNewZoom(newZoom, mouseLocationInDisplayUnits, mouseLocationInImageUnits);
            }
            else if (change < 0)
            {
                var newZoom = ClipZoom(currentZoom / changeFactor);
                SetNewZoom(newZoom, mouseLocationInDisplayUnits, mouseLocationInImageUnits);
            }
        }

        private float ClipZoom(float zoom) => Math.Max(minZoom, Math.Min(maxZoom, zoom));
    }
}
