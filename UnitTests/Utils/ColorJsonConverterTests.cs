using System.Drawing;
using System.Text.Json;
using AwesomeAssertions;
using CDS.ImageDisplay.WinForms.Overlays;
using CDS.ImageDisplay.WinForms.Utils;

namespace UnitTests.Utils;

/// <summary>
/// Tests for <see cref="ColorJsonConverter"/> to ensure that <see cref="Color"/> values are correctly serialized and deserialized in JSON, including both named colors and ARGB hex formats.
/// </summary>
[TestClass]
public sealed class ColorJsonConverterTests
{
    private static readonly JsonSerializerOptions s_jsonSerializerOptions = CreateJsonSerializerOptions();

    /// <summary>
    /// Verifies that named colors are serialized using the color name.
    /// </summary>
    [TestMethod]
    public void SerializeWhenColorIsNamedWritesColorName()
    {
        // Arrange
        var spec = new PenSpec
        {
            Color = Color.Red,
        };

        // Act
        string json = JsonSerializer.Serialize(spec, s_jsonSerializerOptions);

        // Assert
        string expectedFragment = "\"Color\":\"Red\"";
        json.Should().Contain(expectedFragment);
    }

    /// <summary>
    /// Verifies that colors with alpha are serialized using ARGB hex.
    /// </summary>
    [TestMethod]
    public void SerializeWhenColorHasAlphaWritesArgbHex()
    {
        // Arrange
        var spec = new BrushSpec
        {
            Color = Color.FromArgb(64, Color.Navy),
        };

        // Act
        string json = JsonSerializer.Serialize(spec, s_jsonSerializerOptions);

        // Assert
        string expectedFragment = "\"Color\":\"#40000080\"";
        json.Should().Contain(expectedFragment);
    }

    /// <summary>
    /// Verifies that a named color payload restores the original named color.
    /// </summary>
    [TestMethod]
    public void DeserializeWhenColorIsNamedRestoresNamedColor()
    {
        // Arrange
        const string json = "{\"Color\":\"Orange\",\"Width\":1,\"DashStyle\":0,\"StartCap\":0,\"EndCap\":0}";

        // Act
        PenSpec? spec = JsonSerializer.Deserialize<PenSpec>(json, s_jsonSerializerOptions);

        // Assert
        var result = new
        {
            spec!.Color.Name,
            Argb = spec.Color.ToArgb(),
            spec.Color.IsNamedColor,
        };

        var expected = new
        {
            Color.Orange.Name,
            Argb = Color.Orange.ToArgb(),
            IsNamedColor = true,
        };
        result.Should().BeEquivalentTo(expected);
    }

    /// <summary>
    /// Verifies that an ARGB hex payload restores the original color value.
    /// </summary>
    [TestMethod]
    public void DeserializeWhenColorIsArgbHexRestoresArgbColor()
    {
        // Arrange
        const string json = "{\"Color\":\"#40FF0000\"}";

        // Act
        BrushSpec? spec = JsonSerializer.Deserialize<BrushSpec>(json, s_jsonSerializerOptions);

        // Assert
        var expected = Color.FromArgb(64, Color.Red);
        spec.Should().NotBeNull();
        spec!.Color.ToArgb().Should().Be(expected.ToArgb());
    }

    /// <summary>
    /// Verifies that the legacy object payload format still restores the original color value.
    /// </summary>
    [TestMethod]
    public void DeserializeWhenUsingLegacyObjectPayloadRestoresArgbColor()
    {
        // Arrange
        const string json = "{\"Color\":{\"R\":255,\"G\":0,\"B\":0,\"A\":64,\"IsKnownColor\":false,\"IsEmpty\":false,\"IsNamedColor\":false,\"IsSystemColor\":false,\"Name\":\"40ff0000\"}}";

        // Act
        BrushSpec? spec = JsonSerializer.Deserialize<BrushSpec>(json, s_jsonSerializerOptions);

        // Assert
        var expected = Color.FromArgb(64, Color.Red);
        spec.Should().NotBeNull();
        spec!.Color.ToArgb().Should().Be(expected.ToArgb());
    }

    /// <summary>
    /// Verifies that invalid color values produce a <see cref="JsonException"/>.
    /// </summary>
    [TestMethod]
    public void DeserializeWhenColorIsInvalidThrowsJsonException()
    {
        // Arrange
        const string json = "{\"Color\":\"not-a-color\"}";

        // Act
        Action action = () => JsonSerializer.Deserialize<BrushSpec>(json, s_jsonSerializerOptions);

        // Assert
        string expectedMessageFragment = "not a valid Color value";
        action.Should().Throw<JsonException>()
            .WithMessage($"*{expectedMessageFragment}*");
    }

    /// <summary>
    /// Creates serializer options with the color converter registered.
    /// </summary>
    private static JsonSerializerOptions CreateJsonSerializerOptions()
    {
        var jsonSerializerOptions = new JsonSerializerOptions();
        jsonSerializerOptions.Converters.Add(new ColorJsonConverter());
        return jsonSerializerOptions;
    }
}
