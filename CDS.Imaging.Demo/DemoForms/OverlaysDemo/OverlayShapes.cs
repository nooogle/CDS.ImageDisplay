using CDS.Imaging.Utils;
using System;
using System.ComponentModel;
using System.Drawing;

namespace CDS.Imaging.Demo.DemoForms.OverlaysDemo
{
    [TypeConverter(typeof(SerializableExpandableObjectConverter))]
    public class OverlayShapes
    {
        private Random random = new Random();

        [Browsable(false)]
        public Bubble[] Bubbles { get; private set; } = [];


        public OverlayShapes()
        {
            RecreateBubbles(new Size(100, 100));
        }

        public void RecreateBubbles(Size displaySize)
        {
            if(displaySize.Width < 200 || displaySize.Height < 200)
            {
                return;
            }

            Bubbles = new Bubble[20];

            for (int i = 0; i < Bubbles.Length; i++)
            {
                Bubbles[i] = new Bubble(
                    x: random.Next(50, displaySize.Width - 100), 
                    y: random.Next(50, displaySize.Height - 100), 
                    radius: random.Next(10, 100),
                    velocity: random.Next(1, 5), 
                    direction: random.Next(0, 360), 
                    displaySize.Width, 
                    displaySize.Height);
            }
        }

        public void MoveBubbles()
        {
            foreach (var bubble in Bubbles)
            {
                bubble.Move();
            }
        }


        public Draw.RectangleShape Rectangle1 { get; set; } = new Draw.RectangleShape()
        {
            Rect = new RectangleF(100, 100, 101, 101),
        };

        public Draw.RectangleShape Rectangle2 { get; set; } = new Draw.RectangleShape()
        {
            Rect = new RectangleF(300, 200, 100, 100),
            PixelAlign = BitmapDisplay.DisplayPixelAlign.Centre,
        };

        public Draw.CrosshairShape CrossHairShape { get; set; } = new Draw.CrosshairShape()
        {
            Centre = new PointF(400, 400),
            Length = 40,
            CentreGap = 10,
        };

        public Draw.EllipseShape EllipseShape { get; set; } = new Draw.EllipseShape
        {
            Centre = new PointF(10, 5),
            MajorAxis = 20,
            MinorAxis = 10,
            MajorAxisAngleDegrees = 0,
        };

        public Draw.LineShape Line1 { get; set; } = new Draw.LineShape { Start = new PointF(0, 5), End = new PointF(20, 5) };
        public Draw.LineShape Line2 { get; set; } = new Draw.LineShape { Start = new PointF(10, 0), End = new PointF(10, 10) };

        public Draw.TextShape Text1 { get; set; } = new Draw.TextShape { Text = "Look at the ellipse at the top-left!", Location = new PointF(40, 30) };
        public Draw.TextShape Text2 { get; set; } = new Draw.TextShape { Text = "The two lines should (by default) cross the ellipse axis", Location = new PointF(60, 50) };

        public Draw.CircleShape Circle1 { get; set; } = new Draw.CircleShape { Centre = new PointF(500, 300), Radius = 50 };
        public Draw.CircleShape Circle2 { get; set; } = new Draw.CircleShape { Centre = new PointF(300, 500), Radius = 50 };

        public Draw.PolygonShape PolygonShape { get; set; } = new Draw.PolygonShape()
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
    }
}


