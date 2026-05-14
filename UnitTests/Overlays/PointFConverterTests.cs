using System.Drawing;
using AwesomeAssertions;
using CDS.ImageDisplay.Overlays;

namespace UnitTests.Overlays;

[TestClass]
public partial class PointFConverterTests
{
    [TestMethod]
    public void ConvertTo_WithPointF_ReturnsFormattedString()
    {
        // Arrange
        var converter = new PointFConverter();
        var point = new PointF(12.5f, 34.75f);

        // Act
        object? result = converter.ConvertTo(value: point, destinationType: typeof(string));

        // Bundle
        const string expected = "12.5, 34.75";

        // Verify
        result.Should().Be(expected);
    }

    [TestMethod]
    public void ConvertFrom_WithValidString_ReturnsPointF()
    {
        // Arrange
        var converter = new PointFConverter();

        // Act
        object? result = converter.ConvertFrom("12.5, 34.75");

        // Bundle
        var expected = new PointF(12.5f, 34.75f);

        // Verify
        result.Should().Be(expected);
    }

    [TestMethod]
    public void CanConvertFrom_WithString_ReturnsTrue()
    {
        // Arrange
        var converter = new PointFConverter();

        // Act
        bool result = converter.CanConvertFrom(typeof(string));

        // Bundle
        const bool expected = true;

        // Verify
        result.Should().Be(expected);
    }

    [TestMethod]
    public void ConvertFrom_WithInvalidString_ThrowsNotSupportedException()
    {
        // Arrange
        var converter = new PointFConverter();

        // Act
        Action action = () => converter.ConvertFrom("bad-value");

        // Bundle
        Type expectedExceptionType = typeof(NotSupportedException);

        // Verify
        action.Should().Throw<NotSupportedException>()
            .Where(exception => exception.GetType() == expectedExceptionType);
    }

    [TestMethod]
    public void CanConvertTo_WithString_ReturnsTrue()
    {
        // Arrange
        var converter = new PointFConverter();

        // Act
        bool result = converter.CanConvertTo(typeof(string));

        // Bundle
        const bool expected = true;

        // Verify
        result.Should().Be(expected);
    }
}
