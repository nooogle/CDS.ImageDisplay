using CDS.Imaging.BitmapDisplay;
using CDS.Imaging.Draw;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;

namespace CDS.Imaging.Demo.DemoForms.LayersDemo;

class X
{
    CDS.Imaging.Draw.Layer AllLayers = new CDS.Imaging.Draw.Layer();
}

[TypeConverter(typeof(SerializableExpandableObjectConverter))]
static class ShapesFactory
{
    private static Random random = new Random();
    private const int shapesPerLayer = 100;

    public static CDS.Imaging.Draw.Layer Create(Size imageSize)
    {
        CDS.Imaging.Draw.Layer root = new Imaging.Draw.Layer() { Name = "Root" };

        root.ChildLayers.Add(CreateLayers(imageSize, "Small", minSize: 10, maxSize: 50));
        root.ChildLayers.Add(CreateLayers(imageSize, "Large", minSize: 100, maxSize: 200));

        var textLayer = new Imaging.Draw.Layer() { Name = "Text" };
        AddTextToLayer(textLayer, imageSize);
        root.ChildLayers.Add(textLayer);

        return root;
    }

    private static Layer CreateLayers(Size imageSize, string name, int minSize, int maxSize)
    {
        Layer layer = new Layer() { Name = name };

        AddRectanglesToLayer(layer, imageSize, minSize, maxSize);
        AddEllipsesToLayer(layer, imageSize, minSize, maxSize);
        AddLinesTolayer(layer, imageSize);

        return layer;
    }

    private static void AddRectanglesToLayer(Layer layer, Size imageSize, int minSize, int maxSize)
    {
        for (int i = 0; i < shapesPerLayer; i++)
        {
            var rectangle = new Draw.RectangleShape
            {
                Rect = new RectangleF(random.Next(0, imageSize.Width), random.Next(0, imageSize.Height), random.Next(minSize, maxSize), random.Next(minSize, maxSize)),
            };

            rectangle.Rendering.Fill.Color = Color.FromArgb(64, random.Next(0, 255), random.Next(0, 255), random.Next(0, 255));
            rectangle.Rendering.Lines.Color = Color.FromArgb(128, random.Next(0, 255), random.Next(0, 255), random.Next(0, 255));

            layer.Shapes.Add(rectangle);
        }
    }

    private static void AddEllipsesToLayer(Layer layer, Size imageSize, int minSize, int maxSize)
    {
        for (int i = 0; i < shapesPerLayer; i++)
        {
            var ellipse = new Draw.EllipseShape
            {
                Centre = new PointF(random.Next(0, imageSize.Width), random.Next(0, imageSize.Height)),
                MajorAxis = random.Next(minSize, maxSize),
                MinorAxis = random.Next(minSize, maxSize),
                MajorAxisAngleDegrees = random.Next(0, 360),
            };

            ellipse.Rendering.Fill.Color = Color.FromArgb(64, random.Next(0, 255), random.Next(0, 255), random.Next(0, 255));
            ellipse.Rendering.Lines.Color = Color.FromArgb(128, random.Next(0, 255), random.Next(0, 255), random.Next(0, 255));
            layer.Shapes.Add(ellipse);
        }
    }


    private static void AddLinesTolayer(Layer layer, Size imageSize)
    {
        for (int i = 0; i < shapesPerLayer; i++)
        {
            var line = new Draw.LineShape
            {
                Start = new PointF(random.Next(0, imageSize.Width), random.Next(0, imageSize.Height)),
                End = new PointF(random.Next(0, imageSize.Width), random.Next(0, imageSize.Height)),
            };

            line.Rendering.Lines.Color = Color.FromArgb(128, random.Next(0, 255), random.Next(0, 255), random.Next(0, 255));
            layer.Shapes.Add(line);
        }
    }


    private static void AddTextToLayer(Layer layer, Size imageSize)
    {
        var words = new string[] { "Hello", "World", "CDS", "Imaging", "Library", "Shapes", "Text", "Overlay" };

        for (int i = 0; i < shapesPerLayer; i++)
        {
            var text = new Draw.TextShape
            {
                Text = words[random.Next(0, words.Length)],
                Location = new PointF(random.Next(0, imageSize.Width), random.Next(0, imageSize.Height)),
            };

            text.Rendering.Fill.Color = Color.FromArgb(128, random.Next(0, 255), random.Next(0, 255), random.Next(0, 255));
            text.Rendering.Lines.Color = Color.FromArgb(128, random.Next(0, 255), random.Next(0, 255), random.Next(0, 255));

            layer.Shapes.Add(text);
        }
    }
}
