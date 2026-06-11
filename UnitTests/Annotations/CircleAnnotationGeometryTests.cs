using System.Drawing;
using AwesomeAssertions;
using CDS.ImageDisplay.WinForms.Annotations;
using CDS.ImageDisplay.WinForms.Annotations.Shapes;

namespace UnitTests.Annotations;

/// <summary>
/// Tests for <see cref="CircleAnnotationGeometry"/>.
/// </summary>
[TestClass]
public sealed class CircleAnnotationGeometryTests
{
    /// <summary>
    /// Verifies that GetBoundingBox returns a square whose side is the diameter, centred on the circle.
    /// </summary>
    [TestMethod]
    public void GetBoundingBox_ReturnsSquareAroundCentreAndRadius()
    {
        // Arrange
        var geometry = new CircleAnnotationGeometry(new Point(50, 40), radius: 10);

        // Act
        RectangleF result = geometry.GetBoundingBox();

        // Assert
        result.Should().Be(new RectangleF(40f, 30f, 20f, 20f));
    }

    /// <summary>
    /// Verifies that Clone returns a different instance with the same centre and radius.
    /// </summary>
    [TestMethod]
    public void Clone_ReturnsDifferentInstance_WithSameCentreAndRadius()
    {
        // Arrange
        var geometry = new CircleAnnotationGeometry(new Point(30, 60), radius: 15);

        // Act
        var clone = (CircleAnnotationGeometry)geometry.Clone();

        // Assert
        clone.Should().NotBeSameAs(geometry);
        clone.Centre.Should().Be(geometry.Centre);
        clone.Radius.Should().Be(geometry.Radius);
    }

    /// <summary>
    /// Verifies that a MoveBody delta shifts the centre point.
    /// </summary>
    [TestMethod]
    public void ApplyImageDelta_MoveBody_ShiftsCentre()
    {
        // Arrange
        var geometry = new CircleAnnotationGeometry(new Point(50, 50), radius: 10);

        // Act
        geometry.ApplyImageDelta(AnnotationHitInfo.Move, new Size(10, -5));

        // Assert
        geometry.Centre.Should().Be(new Point(60, 45));
        geometry.Radius.Should().Be(10);
    }

    /// <summary>
    /// Verifies that dragging the right handle increases the radius.
    /// </summary>
    [TestMethod]
    public void ApplyImageDelta_RightHandle_IncreasesRadius()
    {
        // Arrange
        var geometry = new CircleAnnotationGeometry(new Point(50, 50), radius: 10);

        // Act
        geometry.ApplyImageDelta(AnnotationHitInfo.Handle(CircleAnnotationGeometry.HandleRight), new Size(5, 0));

        // Assert
        geometry.Radius.Should().Be(15);
        geometry.Centre.Should().Be(new Point(50, 50));
    }

    /// <summary>
    /// Verifies that dragging the top handle upward increases the radius.
    /// </summary>
    [TestMethod]
    public void ApplyImageDelta_TopHandle_MovingUp_IncreasesRadius()
    {
        // Arrange
        var geometry = new CircleAnnotationGeometry(new Point(50, 50), radius: 10);

        // Act — negative Y = upward
        geometry.ApplyImageDelta(AnnotationHitInfo.Handle(CircleAnnotationGeometry.HandleTop), new Size(0, -8));

        // Assert
        geometry.Radius.Should().Be(18);
    }

    /// <summary>
    /// Verifies that the radius is clamped to a minimum of 1.
    /// </summary>
    [TestMethod]
    public void Radius_ClampedToMinimumOne()
    {
        // Arrange / Act
        var geometry = new CircleAnnotationGeometry(new Point(0, 0), radius: -5);

        // Assert
        geometry.Radius.Should().Be(1);
    }

    /// <summary>
    /// Verifies that Clone produces an independent copy — mutating centre on the clone does not affect the original.
    /// </summary>
    [TestMethod]
    public void Clone_MutatingClone_DoesNotAffectOriginal()
    {
        // Arrange
        var original = new CircleAnnotationGeometry(new Point(10, 10), radius: 5);
        var clone = (CircleAnnotationGeometry)original.Clone();

        // Act
        clone.ApplyImageDelta(AnnotationHitInfo.Move, new Size(100, 100));

        // Assert
        original.Centre.Should().Be(new Point(10, 10));
    }
}
