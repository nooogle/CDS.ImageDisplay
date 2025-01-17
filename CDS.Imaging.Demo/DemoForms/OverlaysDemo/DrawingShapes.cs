using System.CodeDom;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.CompilerServices;

namespace CDS.Imaging.Demo.DemoForms.OverlaysDemo
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    class DrawingShapes
    {
        public Ellipse[] Ellipses { get; set; } =
        {
            new Ellipse { Centre = new PointF(10, 5), MajorAxis = 20, MinorAxis = 10, MajorAxisAngleDegrees = 0 },
        };


        public Line[] Lines { get; set; } =
        {
            new Line { Start = new PointF(0, 5), End = new PointF(20, 5) },
            new Line { Start = new PointF(10, 0), End = new PointF(10, 10) },
        };


        public TextMessage[] TextMessages { get; set; } =
        [
            new TextMessage { Text = "Look at the ellipse at the top-left!", Location = new PointF(40, 30) },
            new TextMessage { Text = "The two lines should (by default) cross the ellipse axis", Location = new PointF(60, 50) },
        ];

        public Circle[] Circles { get; set; } =
        {
            new Circle { Centre = new PointF(500, 300), Radius = 50 },
            new Circle { Centre = new PointF(300, 500), Radius = 50 },
        };


        public RectangleFEditable[] Rectangles { get; set; } =
        {
            new RectangleFEditable(600, 100, 100, 50),
            new RectangleFEditable(700, 200, 50, 200),
        };


        public Polygon[] Polygons { get; set; } =
        [
            new Polygon()
            {
                Points =
                [
                    new PointFEditable(500, 100),
                    new PointFEditable(600, 200),
                    new PointFEditable(500, 300),
                    new PointFEditable(400, 200),
                ]
            }
        ];


        public CrossHair CrossHair { get; set; } = new CrossHair
        {
            Centre = new PointF(400, 400),
        };
    }
}
     