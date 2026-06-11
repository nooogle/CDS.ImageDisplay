using System.Drawing;
using AwesomeAssertions;
using CDS.ImageDisplay.WinForms.Annotations;
using CDS.ImageDisplay.WinForms.Annotations.Shapes;

namespace UnitTests.Annotations;

/// <summary>
/// Tests for <see cref="CrosshairAnnotationGeometry"/>.
/// </summary>
[TestClass]
public sealed class CrosshairAnnotationGeometryTests
{
    /// <summary>
    /// Verifies that GetBoundingBox returns Empty because a crosshair is a point shape.
    /// </summary>
    [TestMethod]
    public void GetBoundingBox_ReturnsEmpty()
    {
        // Arrange
        var geometry = new CrosshairAnnotationGeometry(new Point(100, 200));

        // Act / Assert
        geometry.GetBoundingBox().Should().Be(RectangleF.Empty);
    }

    /// <summary>
    /// Verifies that Clone returns a different instance with the same centre, Length, and CentreGap.
    /// </summary>
    [TestMethod]
    public void Clone_ReturnsDifferentInstance_WithSameProperties()
    {
        // Arrange
        var geometry = new CrosshairAnnotationGeometry(new Point(40, 80))
        {
            Length = 20f,
            CentreGap = 5f,
        };

        // Act
        var clone = (CrosshairAnnotationGeometry)geometry.Clone();

        // Assert
        clone.Should().NotBeSameAs(geometry);
        clone.Centre.Should().Be(geometry.Centre);
        clone.Length.Should().Be(geometry.Length);
        clone.CentreGap.Should().Be(geometry.CentreGap);
    }

    /// <summary>
    /// Verifies that a MoveBody delta shifts the centre point.
    /// </summary>
    [TestMethod]
    public void ApplyImageDelta_MoveBody_ShiftsCentre()
    {
        // Arrange
        var geometry = new CrosshairAnnotationGeometry(new Point(50, 50));

        // Act
        geometry.ApplyImageDelta(AnnotationHitInfo.Move, new Size(10, -20));

        // Assert
        geometry.Centre.Should().Be(new Point(60, 30));
    }

    /// <summary>
    /// Verifies that a Handle hit has no effect (crosshair has no resize handles).
    /// </summary>
    [TestMethod]
    public void ApplyImageDelta_HandleHit_HasNoEffect()
    {
        // Arrange
        var geometry = new CrosshairAnnotationGeometry(new Point(50, 50));

        // Act
        geometry.ApplyImageDelta(AnnotationHitInfo.Handle(0), new Size(99, 99));

        // Assert
        geometry.Centre.Should().Be(new Point(50, 50));
    }

    /// <summary>
    /// Verifies that Clone produces an independent copy — mutating the clone does not affect the original.
    /// </summary>
    [TestMethod]
    public void Clone_MutatingClone_DoesNotAffectOriginal()
    {
        // Arrange
        var original = new CrosshairAnnotationGeometry(new Point(10, 10));
        var clone = (CrosshairAnnotationGeometry)original.Clone();

        // Act
        clone.ApplyImageDelta(AnnotationHitInfo.Move, new Size(100, 100));

        // Assert
        original.Centre.Should().Be(new Point(10, 10));
    }
}
