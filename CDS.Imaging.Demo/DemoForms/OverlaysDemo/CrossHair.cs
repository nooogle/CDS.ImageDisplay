using CDS.Imaging.WinForms.BitmapDisplay;
using System.ComponentModel;
using System.Drawing;

namespace CDS.Imaging.Demo.DemoForms.OverlaysDemo
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    class CrossHair
    {
        [TypeConverter(typeof(WinForms.Draw.PointFConverter))]
        public PointF Centre { get; set; }


        public float Length { get; set; } = 10;


        public float CentreGap { get; set; } = 2;


        public DisplayPixelAlign CentreDisplayMode { get; set; } = DisplayPixelAlign.Centre;
    }
}
