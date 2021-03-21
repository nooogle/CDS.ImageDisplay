using System;
using System.Drawing;
using System.Windows.Forms;

namespace CDS.Imaging.WinForms.BitmapDisplayControl
{
    internal class DragManager
    {
        Point dragStartLocation;
        RectangleF initialRenderRect;
        Action<RectangleF> SetNewRenderRect;


        public bool IsDragging { get; private set; }


        public DragManager(Action<RectangleF> setNewRenderRect)
        {
            SetNewRenderRect = setNewRenderRect;
        }

        public void OnMouseDown(BitmapDisplayMode imageDisplayMode, MouseEventArgs mouseEventArgs, RectangleF renderRect)
        {
            if (!IsDragging && (imageDisplayMode == BitmapDisplayMode.Free))
            {
                dragStartLocation = mouseEventArgs.Location;
                IsDragging = true;
                initialRenderRect = renderRect;
                System.Diagnostics.Debug.WriteLine("Dragging");
            }
        }


        public void OnMouseUp(MouseEventArgs mouseEventArgs)
        {
            if (IsDragging)
            {
                IsDragging = false;
                System.Diagnostics.Debug.WriteLine("Dragging complete");
            }
        }

        public void OnMouseMove(MouseEventArgs mouseEventArgs)
        {
            if (!IsDragging) { return; }

            var dragVector = new Point(
                x: mouseEventArgs.Location.X - dragStartLocation.X,
                y: mouseEventArgs.Location.Y - dragStartLocation.Y);

            var newRenderRect = initialRenderRect;
            newRenderRect.Offset(dragVector);
            SetNewRenderRect(newRenderRect);
        }
    }
}
