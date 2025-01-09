using System;
using System.Data.Common;
using System.Drawing;

namespace CDS.Imaging.WinForms.BitmapDisplay
{
    /// <summary>
    /// Renders a rectangle on a graphics object.
    /// </summary>
    public class RectangleRenderer : IDisposable
    {
        private bool isDisposed;
        private Draw.SimplePen outlinePen;
        private Draw.SimplePen grapplePen;
        private Draw.SimpleSolidBrush fillBrush;
        private Draw.SimpleSolidBrush grappleBrush;
        private Rectangle[] grappleRectangles = new Rectangle[8];


        /// <summary>
        /// The pen used to draw the outline of the rectangle.
        /// </summary>
        public Draw.SimplePen OutlinePen => outlinePen;


        /// <summary>
        /// The pen used to draw the grapple points of the rectangle.
        /// </summary>
        public Draw.SimplePen GrapplePen => grapplePen;


        /// <summary>
        /// The brush used to fill the rectangle.
        /// </summary>
        public Draw.SimpleSolidBrush FillBrush => fillBrush;


        /// <summary>
        /// The brush used to fill the grapple points of the rectangle.
        /// </summary>
        public Draw.SimpleSolidBrush GrappleBrush => grappleBrush;


        /// <summary>
        /// Creates a new instance of the <see cref="RectangleRenderer"/> class.
        /// </summary>
        public RectangleRenderer()
        {
            outlinePen = new Draw.SimplePen() {  Color = Color.White };
            fillBrush = new Draw.SimpleSolidBrush() { Color = Color.Transparent };

            grapplePen = new Draw.SimplePen() { Color = Color.Navy };
            grappleBrush = new Draw.SimpleSolidBrush() { Color = Color.Navy };
        }


        /// <summary>
        /// Disposes of the resources used by the <see cref="RectangleRenderer"/>.
        /// </summary>
        public void Dispose()
        {
            if (isDisposed)
            {
                return;
            }

            outlinePen.Dispose();
            fillBrush.Dispose();
            isDisposed = true;
        }


        /// <summary>
        /// Gets or sets a value indicating whether the grapple points of the rectangle should be shown.
        /// </summary>
        public bool ShowGrapples { get; set; }


        /// <summary>
        /// The diameter of the grapple points.
        /// </summary>
        public int GrappleDiameter { get; set; } = 12;


        /// <summary>
        /// Draws the rectangle on the specified graphics object.
        /// </summary>
        /// <param name="graphics"></param>
        /// <param name="rectangle"></param>
        public void Draw(Graphics graphics, Rectangle rectangle)
        {
            if (rectangle.IsEmpty) { return; }
            if (graphics == null) { return; }

            if(ShowGrapples)
            {
                RecalculateGrapplesRectangles(rectangle);
            }

            if (fillBrush.Color != Color.Transparent)
            {
                graphics.FillRectangle(fillBrush, rectangle);

                if (ShowGrapples)
                {
                    for (int grappleIndex = 0; grappleIndex < grappleRectangles.Length; grappleIndex++)
                    {
                        graphics.FillRectangle(grappleBrush, grappleRectangles[grappleIndex]);
                    }
                }
            }

            if (outlinePen.Color != Color.Transparent)
            {
                graphics.DrawRectangle(outlinePen, rectangle);

                if (ShowGrapples)
                {
                    for (int grappleIndex = 0; grappleIndex < grappleRectangles.Length; grappleIndex++)
                    {
                        graphics.DrawRectangle(grapplePen, grappleRectangles[grappleIndex]);
                    }
                }
            }
        }

        private void RecalculateGrapplesRectangles(Rectangle rectangle)
        {
            grappleRectangles[0] = CreateGrappleRect(location: rectangle.Location);
            grappleRectangles[1] = CreateGrappleRect(location: new Point(rectangle.Right, rectangle.Top));
            grappleRectangles[2] = CreateGrappleRect(location: new Point(rectangle.Right, rectangle.Bottom));
            grappleRectangles[3] = CreateGrappleRect(location: new Point(rectangle.Left, rectangle.Bottom));
            grappleRectangles[4] = CreateGrappleRect(location: new Point(rectangle.Left + rectangle.Width / 2, rectangle.Top));
            grappleRectangles[5] = CreateGrappleRect(location: new Point(rectangle.Right, rectangle.Top + rectangle.Height / 2));
            grappleRectangles[6] = CreateGrappleRect(location: new Point(rectangle.Left + rectangle.Width / 2, rectangle.Bottom));
            grappleRectangles[7] = CreateGrappleRect(location: new Point(rectangle.Left, rectangle.Top + rectangle.Height / 2));
        }

        private Rectangle CreateGrappleRect(Point location)
        {
            return new Rectangle(
                x: location.X - GrappleDiameter / 2,
                y: location.Y - GrappleDiameter / 2,
                width: GrappleDiameter,
                height: GrappleDiameter);
        }
    }
}
