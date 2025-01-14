using System.ComponentModel;
using System.Drawing;

namespace CDS.Imaging.WinForms.Draw
{
    /// <summary>
    /// Draws a cross hair
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public partial class SimpleCrossHair : Component
    {
        /// <summary>
        /// String representation
        /// </summary>
        public override string ToString() => "";

        /// <summary>
        /// Diameter
        /// </summary>
        public float Diameter { get; set; } = 20.0f;


        /// <summary>
        /// Gap in the centre of the cross-hair
        /// </summary>
        [Description("Gap in the centre of the cross-hair")]
        public float CentreGap { get; set; } = 6.0f;


        /// <summary>
        /// Pen for drawing the ellipse
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public SimplePen EllipsePen => ellipsePen;


        /// <summary>
        /// Pen for drawing the cross hair lines
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public SimplePen LinePen => linePen;


        /// <summary>
        /// True to draw the ellipse
        /// </summary>
        [Description("True to draw the ellipse")]
        public bool EllipseEnabled { get; set; } = true;


        /// <summary>
        /// Initialise
        /// </summary>
        public SimpleCrossHair()
        {
            InitializeComponent();
        }


        /// <summary>
        /// Initialise
        /// </summary>
        public SimpleCrossHair(IContainer container)
        {
            container.Add(this);

            InitializeComponent();
        }


        /// <summary>
        /// Draw the cross-hair
        /// </summary>
        public void Draw(Graphics graphics, PointF centre)
        {
            var radius = Diameter / 2.0f;
            RectangleF crossHairRect = new RectangleF(centre, new SizeF(Diameter, Diameter));
            crossHairRect.Offset(-radius, -radius);

            var halfCrossHairCentreGap = CentreGap / 2.0f;
            var lineLength = radius - halfCrossHairCentreGap;

            DrawEllipse(graphics, crossHairRect);
            DrawVerticalLines(graphics, radius, crossHairRect, lineLength);
            DrawHorizontalLines(graphics, radius, crossHairRect, lineLength);
        }

        private void DrawEllipse(Graphics graphics, RectangleF crossHairRect)
        {
            if (EllipseEnabled)
            {
                graphics.DrawEllipse(ellipsePen, crossHairRect);
            }
        }


        private void DrawHorizontalLines(Graphics graphics, float radius, RectangleF crossHairRect, float lineLength)
        {
            var horizLineLeftX1 = crossHairRect.Left;
            var horizLineLeftX2 = horizLineLeftX1 + lineLength;
            var horizLineRightX2 = crossHairRect.Right;
            var horizLineRightX1 = horizLineRightX2 - lineLength;
            var horizLineY = crossHairRect.Top + radius;

            graphics.DrawLine(linePen, horizLineLeftX1, horizLineY, horizLineLeftX2, horizLineY);
            graphics.DrawLine(linePen, horizLineRightX1, horizLineY, horizLineRightX2, horizLineY);
        }

        private void DrawVerticalLines(Graphics graphics, float radius, RectangleF crossHairRect, float lineLength)
        {
            var verticalLineX = crossHairRect.X + radius;
            var verticalLineTopY1 = crossHairRect.Top;
            var verticalLineTopY2 = verticalLineTopY1 + lineLength;
            var verticalLineBottomY2 = crossHairRect.Bottom;
            var verticalLineBottomY1 = verticalLineBottomY2 - lineLength;

            graphics.DrawLine(linePen, verticalLineX, verticalLineTopY1, verticalLineX, verticalLineTopY2);
            graphics.DrawLine(linePen, verticalLineX, verticalLineBottomY1, verticalLineX, verticalLineBottomY2);
        }
    }
}
