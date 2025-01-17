using System.ComponentModel;
using System.Drawing;

namespace CDS.Imaging.Demo.DemoForms.OverlaysDemo
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    class TextMessage
    {
        public string Text { get; set; } = "";


        [TypeConverter(typeof(WinForms.Draw.PointFConverter))]
        public PointF Location { get; set; }
    }
}
