using System;
using System.Drawing;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace CDS.ImageDisplay.Annotations.Json;

/// <summary>
/// JSON converter for <see cref="PointF"/>. Serializes as <c>{"X":n,"Y":n}</c>.
/// </summary>
public sealed class PointFConverter : JsonConverter<PointF>
{
    /// <inheritdoc/>
    public override PointF Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.StartObject) { throw new JsonException("Expected {"); }

        float x = 0f, y = 0f;

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
            }
        }

        return new PointF(x, y);
    }

    /// <inheritdoc/>
    public override void Write(Utf8JsonWriter writer, PointF value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        writer.WriteNumber("X", value.X);
        writer.WriteNumber("Y", value.Y);
        writer.WriteEndObject();
    }
}
