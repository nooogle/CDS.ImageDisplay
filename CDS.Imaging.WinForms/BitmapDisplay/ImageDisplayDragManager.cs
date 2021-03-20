using System;
using System.Drawing;
using System.Windows.Forms;

namespace CDS.Imaging.WinForms.BitmapDisplay
{
    class ImageDisplayDragManager
    {
        Point dragStartLocation;
        RectangleF initialRenderRect;
        Action<RectangleF> SetNewRenderRect;


        public bool IsDragging { get; private set; }


        public ImageDisplayDragManager(Action<RectangleF> setNewRenderRect)
        {
            SetNewRenderRect = setNewRenderRect;
        }

        public void OnMouseDown(ImageDisplayMode imageDisplayMode, MouseEventArgs mouseEventArgs, RectangleF renderRect)
        {
            if (!IsDragging && (imageDisplayMode == ImageDisplayMode.Free))
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
