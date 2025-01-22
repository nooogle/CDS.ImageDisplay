using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;

namespace CDS.Imaging.Demo.DemoForms.OverlaysDemo
{

    [TypeConverter(typeof(ExpandableObjectConverter))]
    class Shapes
    {
        [Browsable(false)]
        public Draw.Layer Layer1 { get; } = new();



        public Draw.EllipseShape Ellipse { get; set; } = new Draw.EllipseShape 
        { 
            Centre = new PointF(10, 5), 
            MajorAxis = 20, 
            MinorAxis = 10, 
            MajorAxisAngleDegrees = 0,
        };


        public Draw.LineShape[] Lines { get; set; } =
        {
            new Draw.LineShape { Start = new PointF(0, 5), End = new PointF(20, 5) },
            new Draw.LineShape { Start = new PointF(10, 0), End = new PointF(10, 10) },
        };


        public Draw.TextShape[] TextMessages { get; set; } =
        [
            new Draw.TextShape { Text = "Look at the ellipse at the top-left!", Location = new PointF(40, 30) },
            new Draw.TextShape { Text = "The two lines should (by default) cross the ellipse axis", Location = new PointF(60, 50) },
        ];


        public Draw.CircleShape[] Circles { get; set; } =
        {
            new Draw.CircleShape { Centre = new PointF(500, 300), Radius = 50 },
            new Draw.CircleShape { Centre = new PointF(300, 500), Radius = 50 },
        };



        public Draw.RectangleShape[] Rectangles { get; set; } =
        [
            new Draw.RectangleShape()
            {
                Rect= new RectangleF(100, 100, 100, 100),
            },

            new Draw.RectangleShape()
            {
                Rect= new RectangleF(200, 200, 100, 100),
            },
        ];


        public Draw.PolygonShape Polygon { get; set; } = new Draw.PolygonShape()
        {
            Points =
                [
                    new PointF(100, 500),
                    new PointF(120, 550),
                    new PointF(160, 550),
                    new PointF(130, 570),
                    new PointF(140, 610),
                    new PointF(100, 590),
                    new PointF(60, 610),
                    new PointF(70, 570),
                    new PointF(40, 550),
                    new PointF(80, 550),
                ]
        };


        public Draw.CrosshairShape CrossHair { get; set; } = new Draw.CrosshairShape()
        {
            Centre = new PointF(400, 400),
            Length = 40,
            CentreGap = 10,
        };


        public void PostLoadConfigure(RenderingSettings drawingSettings)
        {
            foreach (var rectangle in Rectangles)
            {
                rectangle.Rendering = drawingSettings.Rectangles;
                Layer1.Shapes.Add(rectangle);
            }

            foreach (var circle in Circles)
            {
                circle.Rendering = drawingSettings.Circles;
                Layer1.Shapes.Add(circle);
            }

            foreach (var line in Lines)
            {
                line.Rendering = drawingSettings.Lines;
                Layer1.Shapes.Add(line);
            }

            Ellipse.Rendering = drawingSettings.Ellipses;
            Layer1.Shapes.Add(Ellipse);

            foreach (var text in TextMessages)
            {
                text.Rendering = drawingSettings.Text;
                Layer1.Shapes.Add(text);
            }

            Polygon.Rendering = drawingSettings.Polygons;
            Layer1.Shapes.Add(Polygon);

            CrossHair.Rendering = drawingSettings.CrossHair;
            Layer1.Shapes.Add(CrossHair);
        }
    }
}
