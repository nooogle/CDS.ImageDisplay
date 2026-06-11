using System.Drawing;
using System.Drawing.Drawing2D;
using AwesomeAssertions;
using CDS.ImageDisplay.WinForms.Overlays;

namespace UnitTests.Overlays;
/// <summary>
/// Tests for specification types in <see cref="CDS.ImageDisplay.Overlays"/>.
/// </summary>
[TestClass]
public sealed class SpecTests
{
    /// <summary>
    /// Verifies that <see cref="PenSpec.Create"/> returns a configured <see cref="Pen"/>.
    /// </summary>
    [TestMethod]
    public void PenSpecCreateReturnsConfiguredPen()
    {
        // Arrange
        var spec = new PenSpec
        {
            Color = Color.Blue,
            Width = 3.5f,
            DashStyle = DashStyle.DashDot,
            StartCap = LineCap.Round,
            EndCap = LineCap.ArrowAnchor,
        };

        // Act
        using Pen pen = spec.Create();
        var result = new
        {
            pen.Color,
            pen.Width,
            pen.DashStyle,
            pen.StartCap,
            pen.EndCap,
        };

        // Assert
        var expected = new
        {
            Color = Color.Blue,
            Width = 3.5f,
            DashStyle = DashStyle.DashDot,
            StartCap = LineCap.Round,
            EndCap = LineCap.ArrowAnchor,
        };

        result.Should().BeEquivalentTo(expected);
    }

    /// <summary>
    /// Verifies that equivalent <see cref="PenSpec"/> instances are equal and produce the same hash code.
    /// </summary>
    [TestMethod]
    public void PenSpecEqualsWithEquivalentValuesReturnsTrue()
    {
        // Arrange
        var first = new PenSpec
        {
            Color = Color.Green,
            Width = 2.0f,
            DashStyle = DashStyle.Dot,
            StartCap = LineCap.Flat,
            EndCap = LineCap.Square,
        };
        var second = new PenSpec
        {
            Color = Color.Green,
            Width = 2.0f,
            DashStyle = DashStyle.Dot,
            StartCap = LineCap.Flat,
            EndCap = LineCap.Square,
        };

        // Act
        var result = new
        {
            Equals = first.Equals(second),
            HashCodesMatch = first.GetHashCode() == second.GetHashCode(),
        };

        // Assert
        var expected = new
        {
            Equals = true,
            HashCodesMatch = true,
        };

        result.Should().BeEquivalentTo(expected);
    }

    /// <summary>
    /// Verifies that <see cref="BrushSpec.Create"/> returns a <see cref="SolidBrush"/> with the configured color.
    /// </summary>
    [TestMethod]
    public void BrushSpecCreateReturnsSolidBrushWithConfiguredColor()
    {
        // Arrange
        var spec = new BrushSpec
        {
            Color = Color.FromArgb(10, 20, 30, 40),
        };

        // Act
        using Brush brush = spec.Create();
        var solidBrush = brush as SolidBrush;
        var result = new
        {
            BrushType = brush.GetType(),
            solidBrush!.Color,
        };

        // Assert
        var expected = new
        {
            BrushType = typeof(SolidBrush),
            Color = Color.FromArgb(10, 20, 30, 40),
        };

        result.Should().BeEquivalentTo(expected);
    }

    /// <summary>
    /// Verifies that <see cref="FontSpec.Create"/> returns a configured <see cref="Font"/>.
    /// </summary>
    [TestMethod]
    public void FontSpecCreateReturnsConfiguredFont()
    {
        // Arrange
        var spec = new FontSpec
        {
            FontName = "Arial",
            FontSize = 16,
        };

        // Act
        using Font font = spec.Create();
        var result = new
        {
            font.Name,
            font.Size,
        };

        // Assert
        var expected = new
        {
            Name = "Arial",
            Size = 16.0f,
        };

        result.Should().BeEquivalentTo(expected);
    }

    /// <summary>
    /// Verifies that <see cref="DrawingSpec"/> initializes with the expected default values.
    /// </summary>
    [TestMethod]
    public void DrawingSpecDefaultInitializationUsesExpectedDefaults()
    {
        // Arrange
        var spec = new DrawingSpec();

        // Act
        var result = new
        {
            spec.Visible,
            spec.MappingMode,
            LineColor = spec.Lines.Color,
            FillColor = spec.Fill.Color,
            spec.Font.FontName,
            spec.Font.FontSize,
        };

        // Assert
        var expected = new
        {
            Visible = true,
            MappingMode = MappingMode.ImageToDisplay,
            LineColor = Color.Red,
            FillColor = Color.Transparent,
            FontName = "Arial",
            FontSize = 12,
        };

        result.Should().BeEquivalentTo(expected);
    }
}
