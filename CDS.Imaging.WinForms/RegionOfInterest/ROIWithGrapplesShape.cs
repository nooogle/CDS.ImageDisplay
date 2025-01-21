using CDS.Imaging.WinForms.BitmapDisplay;
using CDS.Imaging.WinForms.Draw;
using System.ComponentModel;
using System.Drawing;

namespace CDS.Imaging.WinForms.RegionOfInterest
{
    /// <summary>
    /// Renders a rectangle on a graphics object. Supports grapples.
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class ROIWithGrapplesShape : ISingleROIDescriptor
    {
        /// <summary>
        /// Last known region of interest (in display coordinates).
        /// </summary>
        private Rectangle lastROIOnDisplay = Rectangle.Empty;


        /// <summary>
        /// Cached grapple rectangles.
        /// </summary>
        private Rectangle[] grappleRectangles = new Rectangle[8];


        /// <inheritdoc/>
        public bool Visible { get; set; } = true;


        /// <summary>
        /// True if the rectangle is locked and cannot be moved.
        /// </summary>
        public bool Locked { get; set; } = false;


        /// <inheritdoc/>
        public string Name { get; set; } = "";


        /// <inheritdoc/>
        public Rectangle ROI { get; set; }


        /// <inheritdoc/>
        public Size MinimumSize { get; set; } = new Size(1, 1);


        /// <inheritdoc/>
        public Size MaximumSize { get; set; } = new Size(1000000, 1000000);


        /// <summary>
        /// Specification for how to draw the rectangle and grapples when the rectangle is disabled.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public Draw.RenderingSpec Rendering { get; set; } = new RenderingSpec();


        /// <summary>
        /// The diameter of the grapple points.
        /// </summary>
        public int GrappleDiameter { get; set; } = 6;


        /// <inheritdoc/>
        public DisplayPixelAlign PixelAlign { get; set; } = DisplayPixelAlign.TopLeft;


        /// <summary>
        /// Default constructor.
        /// </summary>
        public ROIWithGrapplesShape()
        {
        }


        /// <inheritdoc/>
        public void Draw(BitmapDisplayPanel sender, Graphics graphics)
        {
            if (!Visible) { return; }
            if (ROI.IsEmpty) { return; }

            var roiOnDisplay = sender.MapImageToDisplay(ROI, PixelAlign);
            RecalculateGrapplesRectangles(roiOnDisplay);

            var pen = RenderingToolsPool.GetPen(Rendering.Lines);
            var brush = RenderingToolsPool.GetBrush(Rendering.Fill);

            graphics.FillRectangle(brush, roiOnDisplay);
            graphics.DrawRectangle(pen, roiOnDisplay);


            for (int grappleIndex = 0; grappleIndex < grappleRectangles.Length; grappleIndex++)
            {
                var grappleRect = grappleRectangles[grappleIndex];
                graphics.FillEllipse(brush, grappleRect);
                graphics.DrawEllipse(pen, grappleRect);
            }
        }


        /// <summary>
        /// Recalculates the rectangles for the grapple points.
        /// </summary>
        private void RecalculateGrapplesRectangles(Rectangle roiOnDisplay)
        {
            if(lastROIOnDisplay == roiOnDisplay) { return; }

            lastROIOnDisplay = roiOnDisplay;

            grappleRectangles[0] = CreateGrappleRect(location: roiOnDisplay.Location);
            grappleRectangles[1] = CreateGrappleRect(location: new Point(roiOnDisplay.Right, roiOnDisplay.Top));
            grappleRectangles[2] = CreateGrappleRect(location: new Point(roiOnDisplay.Right, roiOnDisplay.Bottom));
            grappleRectangles[3] = CreateGrappleRect(location: new Point(roiOnDisplay.Left, roiOnDisplay.Bottom));
            grappleRectangles[4] = CreateGrappleRect(location: new Point(roiOnDisplay.Left + roiOnDisplay.Width / 2, roiOnDisplay.Top));
            grappleRectangles[5] = CreateGrappleRect(location: new Point(roiOnDisplay.Right, roiOnDisplay.Top + roiOnDisplay.Height / 2));
            grappleRectangles[6] = CreateGrappleRect(location: new Point(roiOnDisplay.Left + roiOnDisplay.Width / 2, roiOnDisplay.Bottom));
            grappleRectangles[7] = CreateGrappleRect(location: new Point(roiOnDisplay.Left, roiOnDisplay.Top + roiOnDisplay.Height / 2));
        }


        /// <summary>
        /// Creates a rectangle for a grapple point.
        /// </summary>
        private Rectangle CreateGrappleRect(Point location)
        {
            return new Rectangle(
                x: location.X - GrappleDiameter / 2,
                y: location.Y - GrappleDiameter / 2,
                width: GrappleDiameter,
                height: GrappleDiameter);
        }


        /// <summary>
        /// String representation of the object.
        /// </summary>
        public override string ToString() => "";
    }
}
