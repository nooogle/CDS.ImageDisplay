using System;
using System.Drawing;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace CDS.ImageDisplay.WinForms.Annotations.Json;

/// <summary>
/// JSON converter for <see cref="Rectangle"/>. Serializes as <c>{"X":n,"Y":n,"Width":n,"Height":n}</c>.
/// </summary>
public sealed class RectangleConverter : JsonConverter<Rectangle>
{
    /// <inheritdoc/>
    public override Rectangle Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.StartObject) { throw new JsonException("Expected {"); }

        int x = 0, y = 0, width = 0, height = 0;

        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndObject) { break; }
            if (reader.TokenType != JsonTokenType.PropertyName) { continue; }

            string name = reader.GetString()!;
            reader.Read();

            switch (name)
            {
                case "X": x = reader.GetInt32(); break;
                case "Y": y = reader.GetInt32(); break;
                case "Width": width = reader.GetInt32(); break;
                case "Height": height = reader.GetInt32(); break;
            }
        }

        return new Rectangle(x, y, width, height);
    }

    /// <inheritdoc/>
    public override void Write(Utf8JsonWriter writer, Rectangle value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        writer.WriteNumber("X", value.X);
        writer.WriteNumber("Y", value.Y);
        writer.WriteNumber("Width", value.Width);
        writer.WriteNumber("Height", value.Height);
        writer.WriteEndObject();
    }
}
