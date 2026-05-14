using System;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;

namespace CDS.ImageDisplay.Overlays;


/// <summary>
/// Provides a type converter to convert <see cref="PointF"/> objects to and from a string representation.
/// This is useful for properties in the property grid.
/// </summary>
public class PointFConverter : ExpandableObjectConverter
{
    /// <summary>
    /// Determines whether this converter can convert the object to the specified type, using the specified context.
    /// </summary>
    /// <param name="context">An <see cref="ITypeDescriptorContext"/> that provides a format context.</param>
    /// <param name="destinationType">A <see cref="Type"/> that represents the type you want to convert to.</param>
    /// <returns><c>true</c> if this converter can perform the conversion; otherwise, <c>false</c>.</returns>
    public override bool CanConvertTo(ITypeDescriptorContext? context, Type? destinationType) => destinationType == typeof(string) || base.CanConvertTo(context, destinationType);

    /// <summary>
    /// Converts the given value object to the specified type, using the specified context and culture information.
    /// </summary>
    /// <param name="context">An <see cref="ITypeDescriptorContext"/> that provides a format context.</param>
    /// <param name="culture">A <see cref="CultureInfo"/>. If null, the current culture is assumed.</param>
    /// <param name="value">The <see cref="object"/> to convert.</param>
    /// <param name="destinationType">The <see cref="Type"/> to convert the <paramref name="value"/> parameter to.</param>
    /// <returns>An <see cref="object"/> that represents the converted value.</returns>
    /// <exception cref="NotSupportedException">The conversion cannot be performed.</exception>
    public override object? ConvertTo(ITypeDescriptorContext? context, CultureInfo? culture, object? value, Type destinationType)
    {
        return destinationType == typeof(string) && value is PointF point
            ? FormattableString.Invariant($"{point.X}, {point.Y}")
            : base.ConvertTo(context, culture, value, destinationType);
    }

    /// <summary>
    /// Determines whether this converter can convert an object of the given type to the type of this converter, using the specified context.
    /// </summary>
    /// <param name="context">An <see cref="ITypeDescriptorContext"/> that provides a format context.</param>
    /// <param name="sourceType">A <see cref="Type"/> that represents the type you want to convert from.</param>
    /// <returns><c>true</c> if this converter can perform the conversion; otherwise, <c>false</c>.</returns>
    public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType) => sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);

    /// <summary>
    /// Converts the given object to the type of this converter, using the specified context and culture information.
    /// </summary>
    /// <param name="context">An <see cref="ITypeDescriptorContext"/> that provides a format context.</param>
    /// <param name="culture">A <see cref="CultureInfo"/>. If null, the current culture is assumed.</param>
    /// <param name="value">The <see cref="object"/> to convert.</param>
    /// <returns>An <see cref="object"/> that represents the converted value.</returns>
    /// <exception cref="NotSupportedException">The conversion cannot be performed.</exception>
    public override object? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
    {
        if (value is string s)
        {
            string[] parts = s.Split(',');
            if (parts.Length == 2 &&
                float.TryParse(parts[0], System.Globalization.NumberStyles.Float, CultureInfo.InvariantCulture, out float x) &&
                float.TryParse(parts[1], System.Globalization.NumberStyles.Float, CultureInfo.InvariantCulture, out float y))
            {
                return new PointF(x, y);
            }
        }
        return base.ConvertFrom(context, culture, value);
    }
}
