using System.ComponentModel;
using System.Drawing;

namespace CDS.Imaging.Demo.DemoForms.OverlaysDemo
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    class Ellipse
    {
        [TypeConverter(typeof(WinForms.Draw.PointFConverter))]
        public PointF Centre { get; set; }


        public float MajorAxis { get; set; }


        public float MinorAxis { get; set; }


        public float MajorAxisAngleDegrees { get; set; }


        public WinForms.BitmapDisplay.DisplayPixelAlign OriginOnDisplayMode { get; set; } = WinForms.BitmapDisplay.DisplayPixelAlign.Centre;
    }
}
