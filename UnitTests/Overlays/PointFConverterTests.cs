using AwesomeAssertions;

using CDS.ImageDisplay.Overlays;

using System.ComponentModel;
using System.Drawing;

namespace CDS.ImageDisplay.WinFormsTests.Overlays;

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
        var result = converter.ConvertTo(value: point, destinationType: typeof(string));

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
        var result = converter.ConvertFrom("12.5, 34.75");

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
        var result = converter.CanConvertFrom(typeof(string));

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
        var expectedExceptionType = typeof(NotSupportedException);

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
        var result = converter.CanConvertTo(typeof(string));

        // Bundle
        const bool expected = true;

        // Verify
        result.Should().Be(expected);
    }
}
