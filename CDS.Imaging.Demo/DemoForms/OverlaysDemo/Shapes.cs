using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;

namespace CDS.Imaging.Demo.DemoForms.OverlaysDemo
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    class Shapes
    {
        public WinForms.Draw.Shapes.IShapeOverlay[]? All { get; private set; }



        public WinForms.Draw.Shapes.EllipseOverlay Ellipse { get; set; } = new WinForms.Draw.Shapes.EllipseOverlay 
        { 
            Centre = new PointF(10, 5), 
            MajorAxis = 20, 
            MinorAxis = 10, 
            MajorAxisAngleDegrees = 0,
        };


        public WinForms.Draw.Shapes.LineOverlay[] Lines { get; set; } =
        {
            new WinForms.Draw.Shapes.LineOverlay { Start = new PointF(0, 5), End = new PointF(20, 5) },
            new WinForms.Draw.Shapes.LineOverlay { Start = new PointF(10, 0), End = new PointF(10, 10) },
        };


        public WinForms.Draw.Shapes.TextOverlay[] TextMessages { get; set; } =
        [
            new WinForms.Draw.Shapes.TextOverlay { Text = "Look at the ellipse at the top-left!", Location = new PointF(40, 30) },
            new WinForms.Draw.Shapes.TextOverlay { Text = "The two lines should (by default) cross the ellipse axis", Location = new PointF(60, 50) },
        ];


        public WinForms.Draw.Shapes.CircleOverlay[] Circles { get; set; } =
        {
            new WinForms.Draw.Shapes.CircleOverlay { Centre = new PointF(500, 300), Radius = 50 },
            new WinForms.Draw.Shapes.CircleOverlay { Centre = new PointF(300, 500), Radius = 50 },
        };



        public WinForms.Draw.Shapes.RectangleOverlay[] Rectangles { get; set; } =
        [
            new WinForms.Draw.Shapes.RectangleOverlay()
            {
                Rect= new RectangleF(100, 100, 100, 100),
            },

            new WinForms.Draw.Shapes.RectangleOverlay()
            {
                Rect= new RectangleF(200, 200, 100, 100),
            },
        ];


        public WinForms.Draw.Shapes.PolygonOverlay Polygon { get; set; } = new WinForms.Draw.Shapes.PolygonOverlay()
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


        public WinForms.Draw.Shapes.CrossHairOverlay CrossHair { get; set; } = new WinForms.Draw.Shapes.CrossHairOverlay()
        {
            Centre = new PointF(400, 400),
            Length = 40,
            CentreGap = 10,
        };


        public void PostLoadConfigure(RenderingSettings drawingSettings)
        {
            var allShapes = new List<WinForms.Draw.Shapes.IShapeOverlay>();

            foreach (var rectangle in Rectangles)
            {
                rectangle.Rendering = drawingSettings.Rectangles;
                allShapes.Add(rectangle);
            }

            foreach (var circle in Circles)
            {
                circle.Rendering = drawingSettings.Circles;
                allShapes.Add(circle);
            }

            foreach (var line in Lines)
            {
                line.Rendering = drawingSettings.Lines;
                allShapes.Add(line);
            }

            Ellipse.Rendering = drawingSettings.Ellipses;
            allShapes.Add(Ellipse);

            foreach (var text in TextMessages)
            {
                text.Rendering = drawingSettings.Text;
                allShapes.Add(text);
            }

            Polygon.Rendering = drawingSettings.Polygons;
            allShapes.Add(Polygon);

            CrossHair.Rendering = drawingSettings.CrossHair;
            allShapes.Add(CrossHair);

            All = allShapes.ToArray();
        }
    }
}
