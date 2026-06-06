using System.Drawing;
using AwesomeAssertions;
using CDS.ImageDisplay.Annotations;
using CDS.ImageDisplay.Annotations.Shapes;

namespace UnitTests.Annotations;

/// <summary>
/// Tests for <see cref="RectAnnotationGeometry"/>.
/// </summary>
[TestClass]
public sealed class RectAnnotationGeometryTests
{
    /// <summary>
    /// Verifies that GetBoundingBox returns the bounds as a RectangleF.
    /// </summary>
    [TestMethod]
    public void GetBoundingBox_ReturnsRectangle_AsRectangleF()
    {
        // Arrange
        var geometry = new RectAnnotationGeometry(new Rectangle(10, 20, 50, 30));

        // Act
        RectangleF result = geometry.GetBoundingBox();

        // Assert
        result.Should().Be(new RectangleF(10f, 20f, 50f, 30f));
    }

    /// <summary>
    /// Verifies that Clone returns a different instance with the same bounds.
    /// </summary>
    [TestMethod]
    public void Clone_ReturnsDifferentInstance_WithSameBounds()
    {
        // Arrange
        var geometry = new RectAnnotationGeometry(new Rectangle(5, 10, 40, 25));

        // Act
        var clone = (RectAnnotationGeometry)geometry.Clone();

        // Assert
        clone.Should().NotBeSameAs(geometry);
        clone.Bounds.Should().Be(geometry.Bounds);
    }

    /// <summary>
    /// Verifies that applying a MoveBody delta shifts the rectangle position.
    /// </summary>
    [TestMethod]
    public void ApplyImageDelta_MoveBody_ShiftsRectangle()
    {
        // Arrange
        var geometry = new RectAnnotationGeometry(new Rectangle(10, 20, 50, 30));

        // Act
        geometry.ApplyImageDelta(AnnotationHitInfo.Move, new Size(5, -3));

        // Assert
        geometry.Bounds.Should().Be(new Rectangle(15, 17, 50, 30));
    }

    /// <summary>
    /// Verifies that dragging the top-left handle adjusts the top-left corner while keeping the bottom-right fixed.
    /// </summary>
    [TestMethod]
    public void ApplyImageDelta_TopLeftHandle_AdjustsTopLeftKeepsBottomRight()
    {
        // Arrange
        var geometry = new RectAnnotationGeometry(new Rectangle(10, 20, 50, 40));

        // Act — move top-left handle by (+5, +5)
        geometry.ApplyImageDelta(AnnotationHitInfo.Handle(RectAnnotationGeometry.HandleTopLeft), new Size(5, 5));

        // Assert: left and top increase; right and bottom unchanged → width and height shrink
        geometry.Bounds.Should().Be(Rectangle.FromLTRB(15, 25, 60, 60));
    }

    /// <summary>
    /// Verifies that dragging the top-edge handle only adjusts the height (top moves, left/right/bottom fixed).
    /// </summary>
    [TestMethod]
    public void ApplyImageDelta_TopEdgeHandle_ResizesHeightOnly()
    {
        // Arrange
        var geometry = new RectAnnotationGeometry(new Rectangle(10, 20, 50, 40));

        // Act
        geometry.ApplyImageDelta(AnnotationHitInfo.Handle(RectAnnotationGeometry.HandleTopMid), new Size(99, 10));

        // Assert: only top moves; width/left/right/bottom unchanged
        geometry.Bounds.Should().Be(Rectangle.FromLTRB(10, 30, 60, 60));
    }

    /// <summary>
    /// Verifies that dragging the right-edge handle only adjusts the width.
    /// </summary>
    [TestMethod]
    public void ApplyImageDelta_RightEdgeHandle_ResizesWidthOnly()
    {
        // Arrange
        var geometry = new RectAnnotationGeometry(new Rectangle(10, 20, 50, 40));

        // Act
        geometry.ApplyImageDelta(AnnotationHitInfo.Handle(RectAnnotationGeometry.HandleRightMid), new Size(15, 99));

        // Assert: only right moves
        geometry.Bounds.Should().Be(Rectangle.FromLTRB(10, 20, 75, 60));
    }

    /// <summary>
    /// Verifies that the width is clamped to a minimum of 1 when handles would invert the rectangle.
    /// </summary>
    [TestMethod]
    public void ApplyImageDelta_TopRightHandle_ClampsToMinimumSize()
    {
        // Arrange
        var geometry = new RectAnnotationGeometry(new Rectangle(10, 20, 5, 5));

        // Act — try to drag right edge past the left
        geometry.ApplyImageDelta(AnnotationHitInfo.Handle(RectAnnotationGeometry.HandleTopRight), new Size(-100, 0));

        // Assert: width clamped to 1
        geometry.Bounds.Width.Should().Be(1);
    }

    /// <summary>
    /// Verifies that Clone produces an independent copy — mutating one does not affect the other.
    /// </summary>
    [TestMethod]
    public void Clone_MutatingCloneBounds_DoesNotAffectOriginal()
    {
        // Arrange
        var original = new RectAnnotationGeometry(new Rectangle(0, 0, 100, 100));
        var clone = (RectAnnotationGeometry)original.Clone();

        // Act
        clone.ApplyImageDelta(AnnotationHitInfo.Move, new Size(50, 50));

        // Assert
        original.Bounds.Should().Be(new Rectangle(0, 0, 100, 100));
    }
}
