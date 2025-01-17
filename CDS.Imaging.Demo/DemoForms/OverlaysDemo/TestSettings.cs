using System.ComponentModel;

namespace CDS.Imaging.Demo.DemoForms.OverlaysDemo
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    internal class TestSettings
    {
        public DrawingShapes Shapes { get; set; } = new DrawingShapes();
        public DrawingSettings Settings { get; set; } = new DrawingSettings();
    }
}
