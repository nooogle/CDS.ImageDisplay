using System.Drawing;
using System.Drawing.Drawing2D;
using AwesomeAssertions;
using CDS.ImageDisplay.Overlays;

namespace UnitTests.Overlays;

[TestClass]
public partial class SpecTests
{
    [TestMethod]
    public void PenSpec_Create_ReturnsConfiguredPen()
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

        // Bundle
        var expected = new
        {
            Color = Color.Blue,
            Width = 3.5f,
            DashStyle = DashStyle.DashDot,
            StartCap = LineCap.Round,
            EndCap = LineCap.ArrowAnchor,
        };

        // Verify
        result.Should().BeEquivalentTo(expected);
    }

    [TestMethod]
    public void PenSpec_Equals_WithEquivalentValues_ReturnsTrue()
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

        // Bundle
        var expected = new
        {
            Equals = true,
            HashCodesMatch = true,
        };

        // Verify
        result.Should().BeEquivalentTo(expected);
    }

    [TestMethod]
    public void BrushSpec_Create_ReturnsSolidBrushWithConfiguredColor()
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

        // Bundle
        var expected = new
        {
            BrushType = typeof(SolidBrush),
            Color = Color.FromArgb(10, 20, 30, 40),
        };

        // Verify
        result.Should().BeEquivalentTo(expected);
    }

    [TestMethod]
    public void FontSpec_Create_ReturnsConfiguredFont()
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

        // Bundle
        var expected = new
        {
            Name = "Arial",
            Size = 16.0f,
        };

        // Verify
        result.Should().BeEquivalentTo(expected);
    }

    [TestMethod]
    public void DrawingSpec_DefaultInitialization_UsesExpectedDefaults()
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

        // Bundle
        var expected = new
        {
            Visible = true,
            MappingMode = MappingMode.ImageToDisplay,
            LineColor = Color.Red,
            FillColor = Color.Transparent,
            FontName = "Arial",
            FontSize = 12,
        };

        // Verify
        result.Should().BeEquivalentTo(expected);
    }
}
