using System;
using System.ComponentModel;
using System.Drawing;
using CDS.ImageDisplay.Utils;


namespace CDS.ImageDisplay.Overlays;

/// <summary>
/// Represents a font specification.
/// </summary>
[TypeConverter(typeof(SerializableExpandableObjectConverter))]
public class FontSpec
{
    /// <summary>
    /// Simple string representation of this instance.
    /// </summary>
    public override string ToString() => $"Font {FontName}, Size {FontSize}, Style {FontStyle}";


    /// <summary>
    /// Gets or sets the font size.
    /// </summary>
    /// <value>The size of the font.</value>
    public int FontSize { get; set; } = 12;


    /// <summary>
    /// Gets or sets the font name.
    /// </summary>
    /// <value>The name of the font.</value>
    public string FontName { get; set; } = "Arial";


    /// <summary>
    /// Gets or sets the font style.
    /// </summary>
    public FontStyle FontStyle { get; set; } = FontStyle.Regular;


    /// <summary>
    /// Determines whether the specified object is equal to the current object.
    /// </summary>
    public override bool Equals(object? obj)
    {
        if (obj is not FontSpec other) return false;

        return FontSize == other.FontSize &&
               FontName == other.FontName &&
               FontStyle == other.FontStyle;
    }


    /// <summary>
    /// Returns a hash code for this instance.
    /// </summary>
    public override int GetHashCode() => HashCode.Combine(FontSize, FontName, FontStyle);


    /// <summary>
    /// Creates a font from this specification.
    /// </summary>
    public Font Create() => new Font(FontName, FontSize, FontStyle);


    /// <summary>
    /// Returns a shallow copy suitable for use as a stable dictionary key.
    /// FontName is a string (immutable), so a shallow copy is a complete independent copy.
    /// </summary>
    internal FontSpec Clone() => (FontSpec)MemberwiseClone();
}
