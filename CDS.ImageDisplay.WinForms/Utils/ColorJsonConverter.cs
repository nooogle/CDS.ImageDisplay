using System;
using System.Drawing;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace CDS.ImageDisplay.Utils;

/// <summary>
/// Converts <see cref="Color"/> values to and from JSON-friendly string values.
/// </summary>
public sealed class ColorJsonConverter : JsonConverter<Color>
{
    /// <summary>
    /// Reads a <see cref="Color"/> value from JSON.
    /// </summary>
    /// <param name="reader">The JSON reader.</param>
    /// <param name="typeToConvert">The target type.</param>
    /// <param name="options">The serializer options.</param>
    /// <returns>The deserialized color.</returns>
    public override Color Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return reader.TokenType switch
        {
            JsonTokenType.String => ReadFromString(reader.GetString()),
            JsonTokenType.Number => Color.FromArgb(reader.GetInt32()),
            JsonTokenType.StartObject => ReadFromLegacyObject(ref reader),
            _ => throw new JsonException($"Unsupported token {reader.TokenType} when reading a Color value."),
        };
    }

    /// <summary>
    /// Writes a <see cref="Color"/> value as JSON.
    /// </summary>
    /// <param name="writer">The JSON writer.</param>
    /// <param name="value">The color to serialize.</param>
    /// <param name="options">The serializer options.</param>
    public override void Write(Utf8JsonWriter writer, Color value, JsonSerializerOptions options)
    {
        ArgumentNullException.ThrowIfNull(writer, nameof(writer));
        ArgumentNullException.ThrowIfNull(options, nameof(options));

        if (value.IsNamedColor || value.IsKnownColor || value.IsSystemColor)
        {
            writer.WriteStringValue(value.Name);
            return;
        }

        writer.WriteStringValue(FormattableString.Invariant($"#{value.A:X2}{value.R:X2}{value.G:X2}{value.B:X2}"));
    }

    private static Color ReadFromString(string? value)
    {
        return string.IsNullOrWhiteSpace(value)
            ? throw new JsonException("Color string value cannot be null or empty.")
            : string.Equals(value, nameof(Color.Empty), StringComparison.OrdinalIgnoreCase)
            ? Color.Empty
            : value.StartsWith('#')
            ? ReadFromHex(value)
            : Enum.TryParse<KnownColor>(value, ignoreCase: true, out KnownColor knownColor)
            ? Color.FromKnownColor(knownColor)
            : throw new JsonException($"'{value}' is not a valid Color value.");
    }

    private static Color ReadFromHex(string value)
    {
        string hexValue = value[1..];

        return hexValue.Length switch
        {
            6 => Color.FromArgb(
                255,
                ParseHexByte(hexValue.AsSpan(0, 2)),
                ParseHexByte(hexValue.AsSpan(2, 2)),
                ParseHexByte(hexValue.AsSpan(4, 2))),
            8 => Color.FromArgb(
                ParseHexByte(hexValue.AsSpan(0, 2)),
                ParseHexByte(hexValue.AsSpan(2, 2)),
                ParseHexByte(hexValue.AsSpan(4, 2)),
                ParseHexByte(hexValue.AsSpan(6, 2))),
            _ => throw new JsonException($"'{value}' is not a valid Color hex value."),
        };
    }

    private static int ParseHexByte(ReadOnlySpan<char> value)
    {
        return !int.TryParse(value, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out int result)
            ? throw new JsonException($"'{value}' is not a valid hexadecimal byte value.")
            : result;
    }

    private static Color ReadFromLegacyObject(ref Utf8JsonReader reader)
    {
        string? name = null;
        int alpha = 0;
        int red = 0;
        int green = 0;
        int blue = 0;
        bool hasArgb = false;

        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndObject)
            {
                if (!hasArgb)
                {
                    return !string.IsNullOrWhiteSpace(name)
                        ? ReadFromString(name)
                        : throw new JsonException("Color object did not contain a usable value.");
                }

                var argbColor = Color.FromArgb(alpha, red, green, blue);
                if (!string.IsNullOrWhiteSpace(name))
                {
                    if (string.Equals(name, nameof(Color.Empty), StringComparison.OrdinalIgnoreCase))
                    {
                        return Color.Empty;
                    }

                    var namedColor = Color.FromName(name);
                    if ((namedColor.IsNamedColor || namedColor.IsKnownColor || namedColor.IsSystemColor) && namedColor.ToArgb() == argbColor.ToArgb())
                    {
                        return namedColor;
                    }
                }

                return argbColor;
            }

            if (reader.TokenType != JsonTokenType.PropertyName)
            {
                continue;
            }

            string? propertyName = reader.GetString();
            reader.Read();

            switch (propertyName)
            {
                case nameof(Color.A):
                    alpha = reader.GetInt32();
                    hasArgb = true;
                    break;
                case nameof(Color.R):
                    red = reader.GetInt32();
                    hasArgb = true;
                    break;
                case nameof(Color.G):
                    green = reader.GetInt32();
                    hasArgb = true;
                    break;
                case nameof(Color.B):
                    blue = reader.GetInt32();
                    hasArgb = true;
                    break;
                case nameof(Color.Name):
                    name = reader.TokenType == JsonTokenType.String ? reader.GetString() : null;
                    break;
                default:
                    reader.Skip();
                    break;
            }
        }

        throw new JsonException("Unexpected end of JSON while reading a Color value.");
    }
}
