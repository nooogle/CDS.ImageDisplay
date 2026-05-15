using System.ComponentModel;
using CDS.ImageDisplay.Utils;

namespace CDS.ImageDisplay.Demo.DemoForms.OverlaysDemo;

[TypeConverter(typeof(SerializableExpandableObjectConverter))]
internal sealed class TestSettings
{
    public OverlayDrawingSpecs Overlays { get; set; } = new OverlayDrawingSpecs();

    public OverlayShapes Shapes { get; set; } = new OverlayShapes();
}
