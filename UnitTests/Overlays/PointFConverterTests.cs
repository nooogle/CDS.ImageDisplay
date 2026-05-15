using System.Drawing;
using AwesomeAssertions;
using CDS.ImageDisplay.Overlays;

namespace UnitTests.Overlays;

/// <summary>
/// Point converter tests for <see cref="PointFConverter"/>.
/// </summary>
[TestClass]
public sealed class PointFConverterTests
{
    /// <summary>
    /// Verifies that converting a <see cref="PointF"/> to a string returns the expected formatted value.
    /// </summary>
    [TestMethod]
    public void ConvertToWithPointFReturnsFormattedString()
    {
        // Arrange
        var converter = new PointFConverter();
        var point = new PointF(12.5f, 34.75f);

        // Act
        object? result = converter.ConvertTo(value: point, destinationType: typeof(string));

        // Assert
        const string expected = "12.5, 34.75";
        result.Should().Be(expected);
    }

    /// <summary>
    /// Verifies that converting a valid string returns the expected <see cref="PointF"/>.
    /// </summary>
    [TestMethod]
    public void ConvertFromWithValidStringReturnsPointF()
    {
        // Arrange
        var converter = new PointFConverter();

        // Act
        object? result = converter.ConvertFrom("12.5, 34.75");

        // Assert
        var expected = new PointF(12.5f, 34.75f);
        result.Should().Be(expected);
    }

    /// <summary>
    /// Verifies that the converter can convert from a string.
    /// </summary>
    [TestMethod]
    public void CanConvertFromWithStringReturnsTrue()
    {
        // Arrange
        var converter = new PointFConverter();

        // Act
        bool result = converter.CanConvertFrom(typeof(string));

        // Assert
        const bool expected = true;
        result.Should().Be(expected);
    }

    /// <summary>
    /// Verifies that converting an invalid string throws <see cref="NotSupportedException"/>.
    /// </summary>
    [TestMethod]
    public void ConvertFromWithInvalidStringThrowsNotSupportedException()
    {
        // Arrange
        var converter = new PointFConverter();

        // Act
        Action action = () => converter.ConvertFrom("bad-value");

        // Assert
        Type expectedExceptionType = typeof(NotSupportedException);
        action.Should().Throw<NotSupportedException>()
            .Where(exception => exception.GetType() == expectedExceptionType);
    }

    /// <summary>
    /// Verifies that the converter can convert to a string.
    /// </summary>
    [TestMethod]
    public void CanConvertToWithStringReturnsTrue()
    {
        // Arrange
        var converter = new PointFConverter();

        // Act
        bool result = converter.CanConvertTo(typeof(string));

        // Assert
        const bool expected = true;
        result.Should().Be(expected);
    }
}
