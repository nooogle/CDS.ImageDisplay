using System.ComponentModel;
using System.Drawing;

namespace CDS.Imaging.Demo.DemoForms.OverlaysDemo
{
    [TypeConverter(typeof(RectangleFEditableConverter))]
    public class RectangleFEditable
    {
        private RectangleF rectangleF { get; set; }

        public WinForms.BitmapDisplay.DisplayPixelAlign CornerDisplayMode { get; set; } = WinForms.BitmapDisplay.DisplayPixelAlign.TopLeft;

        public float X
        {
            get => rectangleF.X;
            set => rectangleF = new RectangleF(value, Y, Width, Height);
        }


        public float Y
        {
            get => rectangleF.Y;
            set => rectangleF = new RectangleF(X, value, Width, Height);
        }

        public float Width
        {
            get => rectangleF.Width;
            set => rectangleF = new RectangleF(X, Y, value, Height);
        }

        public float Height
        {
            get => rectangleF.Height;
            set => rectangleF = new RectangleF(X, Y, Width, value);
        }



        public RectangleFEditable(RectangleF rectangleF)
        {
            this.rectangleF = rectangleF;
        }


        public RectangleFEditable(float x, float y, float width, float height)
        {
            rectangleF = new RectangleF(x, y, width, height);
        }


        /// <summary>
        /// Returns this as a <see cref="RectangleF"/>
        /// </summary>
        public static implicit operator RectangleF(RectangleFEditable rectangleFEditable)
        {
            return rectangleFEditable.rectangleF;
        }
    }
}
