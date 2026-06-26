using System;
using System.Drawing;
using AwesomeAssertions;
using CDS.ImageDisplay.WinForms.Annotations;
using CDS.ImageDisplay.WinForms.BitmapDisplay;

namespace UnitTests.Annotations;

/// <summary>
/// Tests for <see cref="Annotation"/>.
/// </summary>
[TestClass]
public sealed class AnnotationTests
{
    /// <summary>
    /// Verifies that construction assigns a non-empty Guid as the annotation's Id.
    /// </summary>
    [TestMethod]
    public void Constructor_AssignsNewGuid_IdIsNotEmpty()
    {
        // Arrange / Act
        var annotation = new Annotation(new StubGeometry());

        // Assert
        annotation.Id.Should().NotBe(Guid.Empty);
    }

    /// <summary>
    /// Verifies that Title, Notes, and Label are empty strings by default.
    /// </summary>
    [TestMethod]
    public void Constructor_DefaultsEmptyTitleNotesAndLabel()
    {
        // Arrange / Act
        var annotation = new Annotation(new StubGeometry());

        // Assert
        annotation.Title.Should().BeEmpty();
        annotation.Notes.Should().BeEmpty();
        annotation.Label.Should().BeEmpty();
    }

    /// <summary>
    /// Verifies that two separately constructed annotations receive different Ids.
    /// </summary>
    [TestMethod]
    public void Constructor_TwoInstances_HaveDifferentIds()
    {
        // Arrange / Act
        var first = new Annotation(new StubGeometry());
        var second = new Annotation(new StubGeometry());

        // Assert
        first.Id.Should().NotBe(second.Id);
    }

    /// <summary>
    /// Verifies that Clone returns a different instance but with the same Id, Title, Notes, and Label.
    /// </summary>
    [TestMethod]
    public void Clone_ReturnsDifferentInstance_WithSameIdTitleNotesAndLabel()
    {
        // Arrange
        var original = new Annotation(new StubGeometry())
        {
            Title = "Feature A",
            Notes = "Some notes",
            Label = "car",
        };

        // Act
        Annotation clone = original.Clone();

        // Assert
        clone.Should().NotBeSameAs(original);
        clone.Id.Should().Be(original.Id);
        clone.Title.Should().Be(original.Title);
        clone.Notes.Should().Be(original.Notes);
        clone.Label.Should().Be(original.Label);
    }

    /// <summary>
    /// Verifies that mutating the clone's Title does not affect the original.
    /// </summary>
    [TestMethod]
    public void Clone_MutatingCloneTitle_DoesNotAffectOriginal()
    {
        // Arrange
        var original = new Annotation(new StubGeometry()) { Title = "Original" };
        Annotation clone = original.Clone();

        // Act
        clone.Title = "Changed";

        // Assert
        original.Title.Should().Be("Original");
    }

    /// <summary>
    /// Verifies that the cloned Geometry is a separate instance.
    /// </summary>
    [TestMethod]
    public void Clone_GeometryIsIndependentInstance()
    {
        // Arrange
        var original = new Annotation(new StubGeometry());

        // Act
        Annotation clone = original.Clone();

        // Assert
        clone.Geometry.Should().NotBeSameAs(original.Geometry);
    }

    /// <summary>
    /// Verifies that passing null geometry throws ArgumentNullException.
    /// </summary>
    [TestMethod]
    public void Constructor_NullGeometry_ThrowsArgumentNullException()
    {
        // Arrange / Act / Assert
        Action act = () => _ = new Annotation(null!);
        act.Should().Throw<ArgumentNullException>();
    }

    /// <summary>
    /// Minimal concrete geometry used only for exercising <see cref="Annotation"/>.
    /// </summary>
    private sealed class StubGeometry : AnnotationGeometry
    {
        public override RectangleF GetBoundingBox() => RectangleF.Empty;

        public override void Draw(ICoordinateMapper mapper, Graphics graphics, bool isSelected) { }

        public override AnnotationHitInfo HitTest(BitmapDisplayPanel panel, Point displayPoint, int hitBorder)
            => AnnotationHitInfo.Miss;

        public override void ApplyImageDelta(AnnotationHitInfo hit, Size imageDelta) { }

        public override AnnotationGeometry Clone()
        {
            var clone = new StubGeometry();
            CopyDrawingTo(clone);
            return clone;
        }
    }
}
