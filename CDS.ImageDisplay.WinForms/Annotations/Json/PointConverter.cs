using System;
using System.Drawing;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace CDS.ImageDisplay.Annotations.Json;

/// <summary>
/// JSON converter for <see cref="Point"/>. Serializes as <c>{"X":n,"Y":n}</c>.
/// </summary>
public sealed class PointConverter : JsonConverter<Point>
{
    /// <inheritdoc/>
    public override Point Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.StartObject) { throw new JsonException("Expected {"); }

        int x = 0, y = 0;

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
            }
        }

        return new Point(x, y);
    }

    /// <inheritdoc/>
    public override void Write(Utf8JsonWriter writer, Point value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        writer.WriteNumber("X", value.X);
        writer.WriteNumber("Y", value.Y);
        writer.WriteEndObject();
    }
}
