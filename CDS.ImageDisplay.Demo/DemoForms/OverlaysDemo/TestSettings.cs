using CDS.ImageDisplay.Utils;
using System.ComponentModel;

namespace CDS.ImageDisplay.Demo.DemoForms.OverlaysDemo
{
    [TypeConverter(typeof(SerializableExpandableObjectConverter))]
    public class TestSettings
    {
        public OverlayDrawingSpecs Overlays { get; set; } = new OverlayDrawingSpecs();

        public OverlayShapes Shapes { get; set; } = new OverlayShapes();
    }
}
