using CDS.Imaging.Draw;
using System.ComponentModel;
using System.Drawing;

namespace CDS.Imaging.Demo.DemoForms.OverlaysDemo;


[TypeConverter(typeof(SerializableExpandableObjectConverter))]
class OverlaysSettings
{
    public Draw.Layer RootLayer { get; }


    public OverlaysSettings()
    {
        RootLayer = new Draw.Layer()
        {
            Name = "Root",
        };

        var layer1 = new Draw.Layer()
        {
            Name = "Layer1",
        };

        var layer2 = new Draw.Layer()
        {
            Name = "Layer2",
        };

        RootLayer.ChildLayers.Add(layer1);
        RootLayer.ChildLayers.Add(layer2);


        layer1.ChildLayers.Add(CreateRectanglesLayer());
        layer1.ChildLayers.Add(CreateEllipsesLayer());
        layer1.ChildLayers.Add(CreateLinesLayer());
        layer2.ChildLayers.Add(CreateTextLayer());
        layer2.ChildLayers.Add(CreateCirclesLayer());
        layer2.ChildLayers.Add(CreatePolygonsLayer());
        layer2.ChildLayers.Add(CreateCrossHairLayer());
    }

    private Layer CreateEllipsesLayer()
    {
        var ellipseShape = new Draw.EllipseShape
        {
            Centre = new PointF(10, 5),
            MajorAxis = 20,
            MinorAxis = 10,
            MajorAxisAngleDegrees = 0,
        };

        var ellipsesLayer = new Draw.Layer
        {
            Name = "Ellipses",
        };

        ellipsesLayer.Rendering.Fill.Color = Color.FromArgb(64, Color.Purple);
        ellipsesLayer.Rendering.Lines.Color = Color.Purple;
        ellipsesLayer.Rendering.Lines.Width = 2;
        ellipsesLayer.Rendering.Lines.DashStyle = System.Drawing.Drawing2D.DashStyle.Solid;

        ellipsesLayer.Shapes.Add(ellipseShape);
        return ellipsesLayer;
    }

    private Layer CreateLinesLayer()
    {
        var lines = new[]
        {
            new Draw.LineShape { Start = new PointF(0, 5), End = new PointF(20, 5) },
            new Draw.LineShape { Start = new PointF(10, 0), End = new PointF(10, 10) },
        };

        var linesLayer = new Draw.Layer
        {
            Name = "Lines",
        };

        linesLayer.Rendering.Lines.Color = Color.FromArgb(128, Color.Orange);
        linesLayer.Rendering.Lines.Width = 3;
        linesLayer.Rendering.Lines.DashStyle = System.Drawing.Drawing2D.DashStyle.Solid;

        linesLayer.Shapes.AddRange(lines);
        return linesLayer;
    }

    private Layer CreateTextLayer()
    {
        var textMessages = new[]
        {
            new Draw.TextShape { Text = "Look at the ellipse at the top-left!", Location = new PointF(40, 30) },
            new Draw.TextShape { Text = "The two lines should (by default) cross the ellipse axis", Location = new PointF(60, 50) },
        };

        var textLayer = new Draw.Layer
        {
            Name = "Text",
        };

        textLayer.Rendering.Fill.Color = Color.Yellow;
        textLayer.Rendering.Font.FontSize = 12;

        textLayer.Shapes.AddRange(textMessages);
        return textLayer;
    }

    private Layer CreateCirclesLayer()
    {
        var circleShapes = new[]
        {
            new Draw.CircleShape { Centre = new PointF(500, 300), Radius = 50 },
            new Draw.CircleShape { Centre = new PointF(300, 500), Radius = 50 },
        };

        var circlesLayer = new Draw.Layer
        {
            Name = "Circles",
        };

        circlesLayer.Rendering.Fill.Color = Color.FromArgb(64, Color.Orange);
        circlesLayer.Rendering.Lines.Color = Color.Orange;
        circlesLayer.Rendering.Lines.Width = 2;
        circlesLayer.Rendering.Lines.DashStyle = System.Drawing.Drawing2D.DashStyle.Solid;

        circlesLayer.Shapes.AddRange(circleShapes);
        return circlesLayer;
    }
    

    private Layer CreatePolygonsLayer()
    {
        var polygonShape = new PolygonShape()
        {
            Points = new[]
                {
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
                }
        };

        var polygonsLayer = new Draw.Layer
        {
            Name = "Polygons",
        };

        polygonsLayer.Rendering.Fill.Color = Color.FromArgb(64, Color.LightCyan);
        polygonsLayer.Rendering.Lines.Color = Color.DarkCyan;
        polygonsLayer.Rendering.Lines.Width = 2;
        polygonsLayer.Rendering.Lines.DashStyle = System.Drawing.Drawing2D.DashStyle.Dot;
        polygonsLayer.Shapes.Add(polygonShape);
        return polygonsLayer;
    }

    private Layer CreateCrossHairLayer()
    {
        var crossHairShape = new Draw.CrosshairShape()
        {
            Centre = new PointF(400, 400),
            Length = 40,
            CentreGap = 10,
        };
        
        var crossHairLayer = new Draw.Layer
        {
            Name = "CrossHair",
        };
        
        crossHairLayer.Rendering.Lines.Color = Color.FromArgb(128, Color.Sienna);
        crossHairLayer.Rendering.Lines.Width = 1;
        crossHairLayer.Rendering.Lines.DashStyle = System.Drawing.Drawing2D.DashStyle.Solid;
        
        crossHairLayer.Shapes.Add(crossHairShape);

        return crossHairLayer;
    }

    private static Layer CreateRectanglesLayer()
    {
        var rectangleShapes = new[]
        {
            new Draw.RectangleShape()
            {
                Rect = new RectangleF(100, 100, 100, 100),
            },

            new Draw.RectangleShape()
            {
                Rect = new RectangleF(200, 200, 100, 100),
            },
        };

        var rectanglesLayer = new Draw.Layer()
        {
            Name = "Rectangles",
        };

        rectanglesLayer.Rendering.Fill.Color = Color.FromArgb(64, Color.Red);
        rectanglesLayer.Rendering.Lines.Color = Color.Red;
        rectanglesLayer.Rendering.Lines.Width = 2;
        rectanglesLayer.Rendering.Lines.DashStyle = System.Drawing.Drawing2D.DashStyle.Dot;

        rectanglesLayer.Shapes.AddRange(rectangleShapes);
        return rectanglesLayer;
    }
}

