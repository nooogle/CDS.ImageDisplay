using System.Drawing;
using AwesomeAssertions;
using CDS.ImageDisplay.WinForms.Overlays;

namespace UnitTests.Overlays;

/// <summary>
/// Tests for <see cref="RectangleFConverter"/>.
/// </summary>
[TestClass]
public sealed class RectangleFConverterTests
{
    /// <summary>
    /// Verifies that converting a <see cref="RectangleF"/> to a string returns the expected formatted value.
    /// </summary>
    [TestMethod]
    public void ConvertToWithRectangleFReturnsFormattedString()
    {
        // Arrange
        var converter = new RectangleFConverter();
        var rectangle = new RectangleF(1.5f, 2.5f, 3.5f, 4.5f);

        // Act
        object? result = converter.ConvertTo(value: rectangle, destinationType: typeof(string));

        // Assert
        const string expected = "1.5, 2.5, 3.5, 4.5";
        result.Should().Be(expected);
    }

    /// <summary>
    /// Verifies that converting a valid string returns the expected <see cref="RectangleF"/>.
    /// </summary>
    [TestMethod]
    public void ConvertFromWithValidStringReturnsRectangleF()
    {
        // Arrange
        var converter = new RectangleFConverter();

        // Act
        object? result = converter.ConvertFrom("1.5, 2.5, 3.5, 4.5");

        // Assert
        var expected = new RectangleF(1.5f, 2.5f, 3.5f, 4.5f);
        result.Should().Be(expected);
    }

    /// <summary>
    /// Verifies that the converter can convert to a string.
    /// </summary>
    [TestMethod]
    public void CanConvertToWithStringReturnsTrue()
    {
        // Arrange
        var converter = new RectangleFConverter();

        // Act
        bool result = converter.CanConvertTo(typeof(string));

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
        var converter = new RectangleFConverter();

        // Act
        Action action = () => converter.ConvertFrom("1, 2, 3");

        // Assert
        var expectedExceptionType = typeof(NotSupportedException);
        action.Should().Throw<NotSupportedException>()
            .Where(exception => exception.GetType() == expectedExceptionType);
    }
}

