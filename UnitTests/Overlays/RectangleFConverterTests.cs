using System.Drawing;
using AwesomeAssertions;
using CDS.ImageDisplay.Overlays;

namespace UnitTests.Overlays;

[TestClass]
public partial class RectangleFConverterTests
{
    [TestMethod]
    public void ConvertTo_WithRectangleF_ReturnsFormattedString()
    {
        // Arrange
        var converter = new RectangleFConverter();
        var rectangle = new RectangleF(1.5f, 2.5f, 3.5f, 4.5f);

        // Act
        object? result = converter.ConvertTo(value: rectangle, destinationType: typeof(string));

        // Bundle
        const string expected = "1.5, 2.5, 3.5, 4.5";

        // Verify
        result.Should().Be(expected);
    }

    [TestMethod]
    public void ConvertFrom_WithValidString_ReturnsRectangleF()
    {
        // Arrange
        var converter = new RectangleFConverter();

        // Act
        object? result = converter.ConvertFrom("1.5, 2.5, 3.5, 4.5");

        // Bundle
        var expected = new RectangleF(1.5f, 2.5f, 3.5f, 4.5f);

        // Verify
        result.Should().Be(expected);
    }

    [TestMethod]
    public void CanConvertTo_WithString_ReturnsTrue()
    {
        // Arrange
        var converter = new RectangleFConverter();

        // Act
        bool result = converter.CanConvertTo(typeof(string));

        // Bundle
        const bool expected = true;

        // Verify
        result.Should().Be(expected);
    }

    [TestMethod]
    public void ConvertFrom_WithInvalidString_ThrowsNotSupportedException()
    {
        // Arrange
        var converter = new RectangleFConverter();

        // Act
        Action action = () => converter.ConvertFrom("1, 2, 3");

        // Bundle
        Type expectedExceptionType = typeof(NotSupportedException);

        // Verify
        action.Should().Throw<NotSupportedException>()
            .Where(exception => exception.GetType() == expectedExceptionType);
    }
}
