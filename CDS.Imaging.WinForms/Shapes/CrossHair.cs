using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDS.Imaging.WinForms.Shapes
{
    public partial class CrossHair : Component
    {
        public float Diameter { get; set; } = 20.0f;
        public float CentreGap { get; set; } = 6.0f;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public SimplePen EllipsePen => ellipsePen;


        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public SimplePen LinePen => linePen;


        public CrossHair()
        {
            InitializeComponent();
        }

        public CrossHair(IContainer container)
        {
            container.Add(this);

            InitializeComponent();
        }


        public void Draw(Graphics graphics, PointF centre)
        {
            var radius = Diameter / 2.0f;
            RectangleF crossHairRect = new RectangleF(centre, new SizeF(Diameter, Diameter));
            crossHairRect.Offset(-radius, -radius);
            graphics.DrawEllipse(ellipsePen, crossHairRect);

            var halfCrossHairCentreGap = CentreGap / 2.0f;
            var lineLength = radius - halfCrossHairCentreGap;

            var verticalLineX = crossHairRect.X + radius;
            var verticalLineTopY1 = crossHairRect.Top;
            var verticalLineTopY2 = verticalLineTopY1 + lineLength;
            var verticalLineBottomY2 = crossHairRect.Bottom;
            var verticalLineBottomY1 = verticalLineBottomY2 - lineLength;

            graphics.DrawLine(linePen, verticalLineX, verticalLineTopY1, verticalLineX, verticalLineTopY2);
            graphics.DrawLine(linePen, verticalLineX, verticalLineBottomY1, verticalLineX, verticalLineBottomY2);

            var horizLineLeftX1 = crossHairRect.Left;
            var horizLineLeftX2 = horizLineLeftX1 + lineLength;
            var horizLineRightX2 = crossHairRect.Right;
            var horizLineRightX1 = horizLineRightX2 - lineLength;
            var horizLineY = crossHairRect.Top + radius;

            graphics.DrawLine(linePen, horizLineLeftX1, horizLineY, horizLineLeftX2, horizLineY);
            graphics.DrawLine(linePen, horizLineRightX1, horizLineY, horizLineRightX2, horizLineY);
        }
    }
}
