using System.ComponentModel;
using System.Drawing;
using System.Text.Json.Serialization;
using CDS.ImageDisplay.WinForms.Utils;


namespace CDS.ImageDisplay.WinForms.Overlays;

/// <summary>
/// Represents a brush specification.
/// </summary>
[TypeConverter(typeof(SerializableExpandableObjectConverter))]
public class BrushSpec
{
    /// <summary>
    /// Simple string representation of this instance.
    /// </summary>
    public override string ToString() => Color.ToString();


    /// <summary>
    /// The color of the brush.
    /// </summary>
    [JsonConverter(typeof(ColorJsonConverter))]
    public Color Color { get; set; } = Color.Transparent;


    /// <summary>
    /// Returns a hash code for this instance.
    /// </summary>
    public override int GetHashCode() => Color.GetHashCode();


    /// <summary>
    /// Determines whether the specified object is equal to the current object.
    /// </summary>
    public override bool Equals(object? obj) => obj is BrushSpec other && Color == other.Color;


    /// <summary>
    /// Creates a brush from this specification.
    /// </summary>
    public Brush Create() => new SolidBrush(Color);

    /// <summary>
    /// Returns a shallow copy suitable for use as a stable dictionary key.
    /// </summary>
    internal BrushSpec Clone() => (BrushSpec)MemberwiseClone();
}
