using CDS.Imaging.Utils;
using System.ComponentModel;

namespace CDS.Imaging.Demo.DemoForms.OverlaysDemo
{
    [TypeConverter(typeof(SerializableExpandableObjectConverter))]
    public class TestSettings
    {
        public OverlayDrawingSpecs Overlays { get; set; } = new OverlayDrawingSpecs();

        public OverlayShapes Shapes { get; set; } = new OverlayShapes();
    }
}
