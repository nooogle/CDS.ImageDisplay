using CDS.Imaging.WinForms.BitmapDisplay;
using System.ComponentModel;
using System.Drawing;

namespace CDS.Imaging.Demo.DemoForms.OverlaysDemo
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    class Circle
    {
        [TypeConverter(typeof(WinForms.Draw.PointFConverter))]
        public PointF Centre { get; set; }


        public float Radius { get; set; }

        public DisplayPixelAlign CentreDisplayMode { get; set; } = DisplayPixelAlign.Centre;
    }
}
