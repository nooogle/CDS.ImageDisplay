using CDS.Imaging.Utils;
using System.ComponentModel;

namespace CDS.Imaging.Overlays;

/// <summary>
/// Bundles together the settings for lines, fills, and fonts.
/// </summary>
[TypeConverter(typeof(SerializableExpandableObjectConverter))]
public class DrawingSpec
{
    /// <summary>
    /// Simple representation of the specification.
    /// </summary>
    public override string ToString() => $"Visible={Visible}, Mode={MappingMode}";


    /// <summary>
    /// True if the shape should be visible.
    /// </summary>
    public bool Visible { get; set; } = true;


    /// <summary>
    /// Line specification.
    /// </summary>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
    public PenSpec Lines { get; set; } = new PenSpec();


    /// <summary>
    /// Fill specification.
    /// </summary>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
    public BrushSpec Fill { get; set; } = new BrushSpec();


    /// <summary>
    /// Font specification.
    /// </summary>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
    public FontSpec Font { get; set; } = new FontSpec();


    /// <summary>
    /// How to map coordinates onto the display. Use <see cref="MappingMode.ImageToDisplay"/> when
    /// you want the graphics to shift and scale with the image. Use <see cref="MappingMode.DirectToDisplay"/>
    /// when you want the graphics to remain fixed on the display, regardless of the image position and zoom.
    /// </summary>
    [Description(
        "How to map coordinates onto the display. Use ImageToDisplay when you want the " +
        "graphics to shift and scale with the image. Use DisplayToImage when you " +
        "want the graphics to remain fixed on the display, " +
        "regardless of the image position and zoom.")]
    public MappingMode MappingMode { get; set; } = MappingMode.ImageToDisplay;
}
