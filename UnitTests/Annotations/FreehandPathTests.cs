using System.Drawing;
using AwesomeAssertions;
using CDS.ImageDisplay.WinForms.Annotations;

namespace UnitTests.Annotations;

/// <summary>
/// Tests for <see cref="FreehandPath"/>.
/// </summary>
[TestClass]
public sealed class FreehandPathTests
{
    /// <summary>
    /// Verifies that an empty point sequence produces an empty bounding box.
    /// </summary>
    [TestMethod]
    public void From_EmptySequence_ReturnsBoundingBoxEmpty()
    {
        // Arrange / Act
        FreehandPath path = FreehandPath.From([]);

        // Assert
        path.BoundingBox.Should().Be(RectangleF.Empty);
    }

    /// <summary>
    /// Verifies that a single-point sequence produces a zero-sized bounding box at that point.
    /// </summary>
    [TestMethod]
    public void From_SinglePoint_BoundingBoxIsZeroSized()
    {
        // Arrange / Act
        FreehandPath path = FreehandPath.From([new PointF(10f, 20f)]);

        // Assert
        path.BoundingBox.Should().Be(new RectangleF(10f, 20f, 0f, 0f));
    }

    /// <summary>
    /// Verifies that a horizontal path produces a bounding box with zero height.
    /// </summary>
    [TestMethod]
    public void From_HorizontalLine_BoundingBoxHasZeroHeight()
    {
        // Arrange / Act
        FreehandPath path = FreehandPath.From(
        [
            new PointF(0f, 5f),
            new PointF(10f, 5f),
            new PointF(20f, 5f),
        ]);

        // Assert
        path.BoundingBox.Height.Should().Be(0f);
        path.BoundingBox.Width.Should().Be(20f);
    }

    /// <summary>
    /// Verifies that the bounding box spans the full extents of a scattered path.
    /// </summary>
    [TestMethod]
    public void BoundingBox_DiagonalPath_MatchesExtents()
    {
        // Arrange / Act
        FreehandPath path = FreehandPath.From(
        [
            new PointF(1f, 4f),
            new PointF(5f, 2f),
            new PointF(3f, 9f),
        ]);

        // Assert
        path.BoundingBox.Should().Be(RectangleF.FromLTRB(1f, 2f, 5f, 9f));
    }

    /// <summary>
    /// Verifies that the centroid of four symmetric corner points is at the geometric centre.
    /// </summary>
    [TestMethod]
    public void Centroid_SymmetricPoints_ReturnsGeometricCentre()
    {
        // Arrange / Act
        FreehandPath path = FreehandPath.From(
        [
            new PointF(0f, 0f),
            new PointF(4f, 0f),
            new PointF(4f, 4f),
            new PointF(0f, 4f),
        ]);

        // Assert
        path.Centroid.Should().Be(new PointF(2f, 2f));
    }

    /// <summary>
    /// Verifies that the approximate perimeter of a 3-4-5 right-triangle segment equals 5.
    /// </summary>
    [TestMethod]
    public void ApproximatePerimeter_SingleSegment_EqualsLength()
    {
        // Arrange / Act
        FreehandPath path = FreehandPath.From([new PointF(0f, 0f), new PointF(3f, 4f)]);

        // Assert
        path.ApproximatePerimeter.Should().Be(5f);
    }

    /// <summary>
    /// Verifies that an empty sequence produces a centroid of PointF.Empty.
    /// </summary>
    [TestMethod]
    public void From_EmptySequence_CentroidIsEmpty()
    {
        // Arrange / Act
        FreehandPath path = FreehandPath.From([]);

        // Assert
        path.Centroid.Should().Be(PointF.Empty);
    }

    /// <summary>
    /// Verifies that a single-point sequence produces zero perimeter.
    /// </summary>
    [TestMethod]
    public void From_SinglePoint_PerimeterIsZero()
    {
        // Arrange / Act
        FreehandPath path = FreehandPath.From([new PointF(5f, 5f)]);

        // Assert
        path.ApproximatePerimeter.Should().Be(0f);
    }

    /// <summary>
    /// Verifies that the Points collection matches the input sequence in order.
    /// </summary>
    [TestMethod]
    public void From_ThreePoints_PointsMatchInputOrder()
    {
        // Arrange
        PointF[] input = [new PointF(1f, 2f), new PointF(3f, 4f), new PointF(5f, 6f)];

        // Act
        FreehandPath path = FreehandPath.From(input);

        // Assert
        path.Points.Should().BeEquivalentTo(input, o => o.WithStrictOrdering());
    }

    /// <summary>
    /// Verifies that passing null throws ArgumentNullException.
    /// </summary>
    [TestMethod]
    public void From_NullSequence_ThrowsArgumentNullException()
    {
        // Arrange / Act / Assert
        Action act = () => FreehandPath.From(null!);
        act.Should().Throw<ArgumentNullException>();
    }

    /// <summary>
    /// Verifies that the approximate perimeter sums multiple consecutive segments correctly.
    /// </summary>
    [TestMethod]
    public void ApproximatePerimeter_MultipleSegments_SumsSegmentLengths()
    {
        // Arrange / Act
        // Two 3-4-5 segments: (0,0)→(3,4) = 5, (3,4)→(6,0) = 5
        FreehandPath path = FreehandPath.From(
        [
            new PointF(0f, 0f),
            new PointF(3f, 4f),
            new PointF(6f, 0f),
        ]);

        // Assert
        path.ApproximatePerimeter.Should().Be(10f);
    }
}
