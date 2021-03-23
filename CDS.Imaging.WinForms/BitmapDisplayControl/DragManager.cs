using System;
using System.Drawing;
using System.Windows.Forms;

namespace CDS.Imaging.WinForms.BitmapDisplayControl
{
    internal class DragManager
    {
        Point dragStartLocation;
        PointF initialTargetDisplayCentre;
        Action<PointF> SetTargetDisplayCentre;


        public bool IsDragging { get; private set; }


        public DragManager(Action<PointF> setTargetDisplayCentre)
        {
            SetTargetDisplayCentre = setTargetDisplayCentre;
        }

        public void OnMouseDown(BitmapDisplayMode imageDisplayMode, MouseEventArgs mouseEventArgs, PointF currentTargetDisplayCentre)
        {
            if (!IsDragging && (imageDisplayMode == BitmapDisplayMode.Free))
            {
                dragStartLocation = mouseEventArgs.Location;
                IsDragging = true;
                initialTargetDisplayCentre = currentTargetDisplayCentre;
            }
        }


        public void OnMouseUp(MouseEventArgs mouseEventArgs)
        {
            if (IsDragging)
            {
                IsDragging = false;
            }
        }

        public void OnMouseMove(MouseEventArgs mouseEventArgs)
        {
            if (!IsDragging) { return; }

            var xDrag = mouseEventArgs.Location.X - dragStartLocation.X;
            var yDrag = mouseEventArgs.Location.Y - dragStartLocation.Y;

            var newTargetDisplayCentre = new PointF(
                x: initialTargetDisplayCentre.X + xDrag,
                y: initialTargetDisplayCentre.Y + yDrag);

            SetTargetDisplayCentre(newTargetDisplayCentre);
        }
    }
}
