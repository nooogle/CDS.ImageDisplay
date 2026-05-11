using AwesomeAssertions;

using CDS.Imaging.Overlays;
using CDS.Imaging.Utils;

using System.Drawing;
using System.Text.Json;

namespace CDS.Imaging.WinFormsTests.Utils;

[TestClass]
public partial class ColorJsonConverterTests
{
    private static readonly JsonSerializerOptions JsonSerializerOptions = CreateJsonSerializerOptions();

    [TestMethod]
    public void Serialize_WhenColorIsNamed_WritesColorName()
    {
        // Arrange
        var spec = new PenSpec
        {
            Color = Color.Red,
        };

        // Act
        var json = JsonSerializer.Serialize(spec, JsonSerializerOptions);

        // Bundle
        var expectedFragment = "\"Color\":\"Red\"";

        // Verify
        json.Should().Contain(expectedFragment);
    }

    [TestMethod]
    public void Serialize_WhenColorHasAlpha_WritesArgbHex()
    {
        // Arrange
        var spec = new BrushSpec
        {
            Color = Color.FromArgb(64, Color.Navy),
        };

        // Act
        var json = JsonSerializer.Serialize(spec, JsonSerializerOptions);

        // Bundle
        var expectedFragment = "\"Color\":\"#40000080\"";

        // Verify
        json.Should().Contain(expectedFragment);
    }

    [TestMethod]
    public void Deserialize_WhenColorIsNamed_RestoresNamedColor()
    {
        // Arrange
        const string json = "{\"Color\":\"Orange\",\"Width\":1,\"DashStyle\":0,\"StartCap\":0,\"EndCap\":0}";

        // Act
        var spec = JsonSerializer.Deserialize<PenSpec>(json, JsonSerializerOptions);

        // Bundle
        var result = new
        {
            spec!.Color.Name,
            Argb = spec.Color.ToArgb(),
            spec.Color.IsNamedColor,
        };

        var expected = new
        {
            Name = Color.Orange.Name,
            Argb = Color.Orange.ToArgb(),
            IsNamedColor = true,
        };

        // Verify
        result.Should().BeEquivalentTo(expected);
    }

    [TestMethod]
    public void Deserialize_WhenColorIsArgbHex_RestoresArgbColor()
    {
        // Arrange
        const string json = "{\"Color\":\"#40FF0000\"}";

        // Act
        var spec = JsonSerializer.Deserialize<BrushSpec>(json, JsonSerializerOptions);

        // Bundle
        var expected = Color.FromArgb(64, Color.Red);

        // Verify
        spec.Should().NotBeNull();
        spec!.Color.ToArgb().Should().Be(expected.ToArgb());
    }

    [TestMethod]
    public void Deserialize_WhenUsingLegacyObjectPayload_RestoresArgbColor()
    {
        // Arrange
        const string json = "{\"Color\":{\"R\":255,\"G\":0,\"B\":0,\"A\":64,\"IsKnownColor\":false,\"IsEmpty\":false,\"IsNamedColor\":false,\"IsSystemColor\":false,\"Name\":\"40ff0000\"}}";

        // Act
        var spec = JsonSerializer.Deserialize<BrushSpec>(json, JsonSerializerOptions);

        // Bundle
        var expected = Color.FromArgb(64, Color.Red);

        // Verify
        spec.Should().NotBeNull();
        spec!.Color.ToArgb().Should().Be(expected.ToArgb());
    }

    [TestMethod]
    public void Deserialize_WhenColorIsInvalid_ThrowsJsonException()
    {
        // Arrange
        const string json = "{\"Color\":\"not-a-color\"}";

        // Act
        Action action = () => JsonSerializer.Deserialize<BrushSpec>(json, JsonSerializerOptions);

        // Bundle
        var expectedMessageFragment = "not a valid Color value";

        // Verify
        action.Should().Throw<JsonException>()
            .WithMessage($"*{expectedMessageFragment}*");
    }

    private static JsonSerializerOptions CreateJsonSerializerOptions()
    {
        var jsonSerializerOptions = new JsonSerializerOptions();
        jsonSerializerOptions.Converters.Add(new ColorJsonConverter());
        return jsonSerializerOptions;
    }
}
