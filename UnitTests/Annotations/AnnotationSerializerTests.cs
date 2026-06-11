using System;
using System.Collections.Generic;
using System.Drawing;
using AwesomeAssertions;
using CDS.ImageDisplay.WinForms.Annotations;
using CDS.ImageDisplay.WinForms.Annotations.Shapes;
using CDS.ImageDisplay.WinForms.Overlays;

namespace UnitTests.Annotations;

/// <summary>
/// Tests for <see cref="AnnotationSerializer"/>.
/// Verifies round-trip JSON serialization for all geometry types and annotation metadata.
/// </summary>
[TestClass]
[TestCategory("Serialization")]
public sealed class AnnotationSerializerTests
{
    // ── Rect ────────────────────────────────────────────────────────────────

    /// <summary>
    /// Verifies that a rect annotation serializes and deserializes back to an equal geometry.
    /// </summary>
    [TestMethod]
    public void Serialize_RectAnnotation_RoundTripsGeometry()
    {
        var original = new Annotation(new RectAnnotationGeometry(new Rectangle(10, 20, 100, 50)));

        string json = AnnotationSerializer.Serialize(original);
        Annotation? restored = AnnotationSerializer.Deserialize(json);

        restored.Should().NotBeNull();
        restored!.Geometry.Should().BeOfType<RectAnnotationGeometry>();
        ((RectAnnotationGeometry)restored.Geometry).Bounds.Should().Be(new Rectangle(10, 20, 100, 50));
    }

    // ── Circle ──────────────────────────────────────────────────────────────

    /// <summary>
    /// Verifies that a circle annotation serializes and deserializes back to an equal geometry.
    /// </summary>
    [TestMethod]
    public void Serialize_CircleAnnotation_RoundTripsGeometry()
    {
        var original = new Annotation(new CircleAnnotationGeometry(new Point(50, 60), 30));

        string json = AnnotationSerializer.Serialize(original);
        Annotation? restored = AnnotationSerializer.Deserialize(json);

        restored.Should().NotBeNull();
        restored!.Geometry.Should().BeOfType<CircleAnnotationGeometry>();
        var circle = (CircleAnnotationGeometry)restored.Geometry;
        circle.Centre.Should().Be(new Point(50, 60));
        circle.Radius.Should().Be(30);
    }

    // ── Ellipse ─────────────────────────────────────────────────────────────

    /// <summary>
    /// Verifies that a rotated ellipse annotation serializes and deserializes back to an equal geometry.
    /// </summary>
    [TestMethod]
    public void Serialize_EllipseAnnotation_RoundTripsGeometry()
    {
        var original = new Annotation(new EllipseAnnotationGeometry(new PointF(45f, 30f), 40f, 20f, 30f));

        string json = AnnotationSerializer.Serialize(original);
        Annotation? restored = AnnotationSerializer.Deserialize(json);

        restored.Should().NotBeNull();
        restored!.Geometry.Should().BeOfType<EllipseAnnotationGeometry>();
        var ellipse = (EllipseAnnotationGeometry)restored.Geometry;
        ellipse.Center.Should().Be(new PointF(45f, 30f));
        ellipse.SemiMajor.Should().BeApproximately(40f, 0.001f);
        ellipse.SemiMinor.Should().BeApproximately(20f, 0.001f);
        ellipse.AngleDegrees.Should().BeApproximately(30f, 0.001f);
    }

    // ── Rotated Rectangle ───────────────────────────────────────────────────

    /// <summary>
    /// Verifies that a rotated rectangle annotation serializes and deserializes back to an equal geometry.
    /// </summary>
    [TestMethod]
    public void Serialize_RotatedRectAnnotation_RoundTripsGeometry()
    {
        var original = new Annotation(new RotatedRectAnnotationGeometry(new PointF(100f, 150f), 80f, 40f, 45f));

        string json = AnnotationSerializer.Serialize(original);
        Annotation? restored = AnnotationSerializer.Deserialize(json);

        restored.Should().NotBeNull();
        restored!.Geometry.Should().BeOfType<RotatedRectAnnotationGeometry>();
        var rr = (RotatedRectAnnotationGeometry)restored.Geometry;
        rr.Center.Should().Be(new PointF(100f, 150f));
        rr.Width.Should().BeApproximately(80f, 0.001f);
        rr.Height.Should().BeApproximately(40f, 0.001f);
        rr.AngleDegrees.Should().BeApproximately(45f, 0.001f);
    }

    // ── Line ─────────────────────────────────────────────────────────────────

    /// <summary>
    /// Verifies that a line annotation serializes and deserializes back to an equal geometry.
    /// </summary>
    [TestMethod]
    public void Serialize_LineAnnotation_RoundTripsGeometry()
    {
        var original = new Annotation(new LineAnnotationGeometry(new Point(0, 0), new Point(100, 200)));

        string json = AnnotationSerializer.Serialize(original);
        Annotation? restored = AnnotationSerializer.Deserialize(json);

        restored.Should().NotBeNull();
        restored!.Geometry.Should().BeOfType<LineAnnotationGeometry>();
        var line = (LineAnnotationGeometry)restored.Geometry;
        line.Start.Should().Be(new Point(0, 0));
        line.End.Should().Be(new Point(100, 200));
    }

    // ── Polygon ───────────────────────────────────────────────────────────────

    /// <summary>
    /// Verifies that a polygon annotation serializes and deserializes back with all vertices.
    /// </summary>
    [TestMethod]
    public void Serialize_PolygonAnnotation_RoundTripsVertices()
    {
        var vertices = new[] { new Point(0, 0), new Point(50, 0), new Point(25, 40) };
        var original = new Annotation(new PolygonAnnotationGeometry(vertices));

        string json = AnnotationSerializer.Serialize(original);
        Annotation? restored = AnnotationSerializer.Deserialize(json);

        restored.Should().NotBeNull();
        restored!.Geometry.Should().BeOfType<PolygonAnnotationGeometry>();
        var poly = (PolygonAnnotationGeometry)restored.Geometry;
        poly.Vertices.Should().HaveCount(3);
        poly.Vertices[0].Should().Be(new Point(0, 0));
        poly.Vertices[1].Should().Be(new Point(50, 0));
        poly.Vertices[2].Should().Be(new Point(25, 40));
    }

    // ── Crosshair ─────────────────────────────────────────────────────────────

    /// <summary>
    /// Verifies that a crosshair annotation serializes and deserializes back to an equal geometry.
    /// </summary>
    [TestMethod]
    public void Serialize_CrosshairAnnotation_RoundTripsGeometry()
    {
        var geo = new CrosshairAnnotationGeometry(new Point(100, 200)) { Length = 20f, CentreGap = 5f };
        var original = new Annotation(geo);

        string json = AnnotationSerializer.Serialize(original);
        Annotation? restored = AnnotationSerializer.Deserialize(json);

        restored.Should().NotBeNull();
        restored!.Geometry.Should().BeOfType<CrosshairAnnotationGeometry>();
        var crosshair = (CrosshairAnnotationGeometry)restored.Geometry;
        crosshair.Centre.Should().Be(new Point(100, 200));
        crosshair.Length.Should().BeApproximately(20f, 0.001f);
        crosshair.CentreGap.Should().BeApproximately(5f, 0.001f);
    }

    // ── Metadata ──────────────────────────────────────────────────────────────

    /// <summary>
    /// Verifies that Id, Title, Notes, and Label round-trip correctly.
    /// </summary>
    [TestMethod]
    public void Serialize_AnnotationMetadata_RoundTripsCorrectly()
    {
        var original = new Annotation(new CrosshairAnnotationGeometry(new Point(0, 0)))
        {
            Title = "Cat",
            Notes = "High confidence",
            Label = "cat",
        };

        string json = AnnotationSerializer.Serialize(original);
        Annotation? restored = AnnotationSerializer.Deserialize(json);

        restored.Should().NotBeNull();
        restored!.Id.Should().Be(original.Id);
        restored.Title.Should().Be("Cat");
        restored.Notes.Should().Be("High confidence");
        restored.Label.Should().Be("cat");
    }

    // ── DrawingSpec ────────────────────────────────────────────────────────────

    /// <summary>
    /// Verifies that pen colour in the Drawing spec round-trips through JSON.
    /// </summary>
    [TestMethod]
    public void Serialize_DrawingSpecPenColor_RoundTripsCorrectly()
    {
        var geo = new RectAnnotationGeometry(new Rectangle(0, 0, 10, 10))
        {
            Drawing = new DrawingSpec { Lines = new PenSpec { Color = Color.Tomato } },
        };
        var original = new Annotation(geo);

        string json = AnnotationSerializer.Serialize(original);
        Annotation? restored = AnnotationSerializer.Deserialize(json);

        restored.Should().NotBeNull();
        restored!.Geometry.Drawing.Lines.Color.Should().Be(Color.Tomato);
    }

    /// <summary>
    /// Verifies that visibility in the Drawing spec round-trips through JSON.
    /// </summary>
    [TestMethod]
    public void Serialize_DrawingSpecVisibility_RoundTripsCorrectly()
    {
        var geo = new CircleAnnotationGeometry(new Point(0, 0), 10)
        {
            Drawing = new DrawingSpec { Visible = false },
        };
        var original = new Annotation(geo);

        string json = AnnotationSerializer.Serialize(original);
        Annotation? restored = AnnotationSerializer.Deserialize(json);

        restored.Should().NotBeNull();
        restored!.Geometry.Drawing.Visible.Should().BeFalse();
    }

    // ── List ──────────────────────────────────────────────────────────────────

    /// <summary>
    /// Verifies that a mixed list of annotations serializes and deserializes to an equal list.
    /// </summary>
    [TestMethod]
    public void Serialize_AnnotationList_RoundTripsCorrectly()
    {
        var annotations = new List<Annotation>
        {
            new(new RectAnnotationGeometry(new Rectangle(0, 0, 50, 50))) { Title = "rect" },
            new(new CircleAnnotationGeometry(new Point(25, 25), 15)) { Title = "circle" },
            new(new CrosshairAnnotationGeometry(new Point(10, 10))) { Title = "crosshair" },
        };

        string json = AnnotationSerializer.Serialize(annotations);
        IReadOnlyList<Annotation> restored = AnnotationSerializer.DeserializeList(json);

        restored.Should().HaveCount(3);
        restored[0].Geometry.Should().BeOfType<RectAnnotationGeometry>();
        restored[0].Title.Should().Be("rect");
        restored[1].Geometry.Should().BeOfType<CircleAnnotationGeometry>();
        restored[1].Title.Should().Be("circle");
        restored[2].Geometry.Should().BeOfType<CrosshairAnnotationGeometry>();
        restored[2].Title.Should().Be("crosshair");
    }

    /// <summary>
    /// Verifies that deserializing a JSON null array returns an empty list.
    /// </summary>
    [TestMethod]
    public void DeserializeList_NullJson_ReturnsEmptyList()
    {
        IReadOnlyList<Annotation> result = AnnotationSerializer.DeserializeList("null");

        result.Should().BeEmpty();
    }

    // ── Type discriminator ────────────────────────────────────────────────────

    /// <summary>
    /// Verifies that the serialized JSON contains the $type discriminator for the geometry.
    /// </summary>
    [TestMethod]
    public void Serialize_RectAnnotation_JsonContainsTypeDiscriminator()
    {
        var original = new Annotation(new RectAnnotationGeometry(new Rectangle(0, 0, 10, 10)));

        string json = AnnotationSerializer.Serialize(original);

        json.Should().Contain("\"$type\"");
        json.Should().Contain("\"rect\"");
    }

    // ── CreateOptions ──────────────────────────────────────────────────────────

    /// <summary>
    /// Verifies that CreateOptions returns a new instance each time.
    /// </summary>
    [TestMethod]
    public void CreateOptions_CalledTwice_ReturnsDifferentInstances()
    {
        var options1 = AnnotationSerializer.CreateOptions();
        var options2 = AnnotationSerializer.CreateOptions();

        options1.Should().NotBeSameAs(options2);
    }
}
