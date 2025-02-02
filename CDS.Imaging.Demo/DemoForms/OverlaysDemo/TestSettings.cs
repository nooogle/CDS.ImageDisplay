using CDS.Imaging.Utils;
using System.ComponentModel;

namespace CDS.Imaging.Demo.DemoForms.OverlaysDemo
{
    [TypeConverter(typeof(SerializableExpandableObjectConverter))]
    public class TestSettings
    {
        public OverlayRenderingSpecs Overlays { get; set; } = new OverlayRenderingSpecs();

        public OverlayShapes Shapes { get; set; } = new OverlayShapes();
    }
}
