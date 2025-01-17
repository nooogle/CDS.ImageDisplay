using System.ComponentModel;
using System.Drawing;

namespace CDS.Imaging.Demo.DemoForms.OverlaysDemo;


[TypeConverter(typeof(ExpandableObjectConverter))]
class Line
{
    [TypeConverter(typeof(WinForms.Draw.PointFConverter))]
    public PointF Start { get; set; }


    [TypeConverter(typeof(WinForms.Draw.PointFConverter))]
    public PointF End { get; set; }


    public WinForms.BitmapDisplay.DisplayPixelAlign LineEndDisplayMode { get; set; } = WinForms.BitmapDisplay.DisplayPixelAlign.Centre;
}
