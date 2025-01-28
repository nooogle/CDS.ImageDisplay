using System.ComponentModel;

namespace CDS.Imaging.Demo.DemoForms.OverlaysDemo
{
    [TypeConverter(typeof(SerializableExpandableObjectConverter))]
    internal class TestSettings
    {
        public OverlaysSettings Overlay { get; set; } = new OverlaysSettings();
    }
}
