using System;
using System.Drawing;
using System.Windows.Forms;

namespace CDS.Imaging.BitmapDisplay
{
    internal class ZoomManager
    {
        Action<float, PointF, PointF> SetNewZoom;


        public ZoomManager(
            Action<float, PointF, PointF> setNewZoom)
        {
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

            if (change > 0)
            {
                var changeFactor = 1.0f + (change / 500.0f);
                SetNewZoom(currentZoom * changeFactor, mouseLocationInDisplayUnits, mouseLocationInImageUnits);
            }
            else if (change < 0)
            {
                var changeFactor = 1.0f + (-change / 500.0f);
                SetNewZoom(currentZoom / changeFactor, mouseLocationInDisplayUnits, mouseLocationInImageUnits);
            }
        }
    }
}
