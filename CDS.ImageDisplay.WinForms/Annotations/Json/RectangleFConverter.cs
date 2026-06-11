using System;
using System.Drawing;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace CDS.ImageDisplay.WinForms.Annotations.Json;

/// <summary>
/// JSON converter for <see cref="RectangleF"/>. Serializes as <c>{"X":n,"Y":n,"Width":n,"Height":n}</c>.
/// </summary>
public sealed class RectangleFConverter : JsonConverter<RectangleF>
{
    /// <inheritdoc/>
    public override RectangleF Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.StartObject) { throw new JsonException("Expected {"); }

        float x = 0f, y = 0f, width = 0f, height = 0f;

        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndObject) { break; }
            if (reader.TokenType != JsonTokenType.PropertyName) { continue; }

            string name = reader.GetString()!;
            reader.Read();

            switch (name)
            {
                case "X": x = reader.GetSingle(); break;
                case "Y": y = reader.GetSingle(); break;
                case "Width": width = reader.GetSingle(); break;
                case "Height": height = reader.GetSingle(); break;
            }
        }

        return new RectangleF(x, y, width, height);
    }

    /// <inheritdoc/>
    public override void Write(Utf8JsonWriter writer, RectangleF value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        writer.WriteNumber("X", value.X);
        writer.WriteNumber("Y", value.Y);
        writer.WriteNumber("Width", value.Width);
        writer.WriteNumber("Height", value.Height);
        writer.WriteEndObject();
    }
}
