using System.ComponentModel;

namespace CDS.Imaging.Demo.DemoForms.OverlaysDemo
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    internal class TestSettings
    {
        public Shapes Shapes { get; set; } = new Shapes();
        public RenderingSettings Rendering { get; set; } = new RenderingSettings();
    }
}
