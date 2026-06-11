using System;
using System.Collections.Generic;
using System.Drawing;
using AwesomeAssertions;
using CDS.ImageDisplay.WinForms.Annotations;
using CDS.ImageDisplay.WinForms.Annotations.Shapes;

namespace UnitTests.Annotations;

/// <summary>
/// Tests for <see cref="AnnotationShapeRecognizer"/> and the shape descriptor
/// <see cref="IAnnotationShapeDescriptor.FitScore"/> heuristics.
/// </summary>
[TestClass]
public sealed class AnnotationShapeRecognizerTests
{
    // -----------------------------------------------------------------------
    // Synthetic path builders
    // -----------------------------------------------------------------------

    private static FreehandPath MakeLinePath()
    {
        var pts = new List<PointF>();
        for (int x = 0; x <= 200; x += 10) { pts.Add(new PointF(x, 50)); }
        return FreehandPath.From(pts);
    }

    private static FreehandPath MakeRectPath()
    {
        var pts = new List<PointF>();
        for (int x = 0; x <= 160; x += 10) { pts.Add(new PointF(x, 0)); }
        for (int y = 0; y <= 120; y += 10) { pts.Add(new PointF(160, y)); }
        for (int x = 160; x >= 0; x -= 10) { pts.Add(new PointF(x, 120)); }
        for (int y = 120; y >= 0; y -= 10) { pts.Add(new PointF(0, y)); }
        return FreehandPath.From(pts);
    }

    private static FreehandPath MakeCirclePath()
    {
        float cx = 80, cy = 80, r = 80;
        var pts = new List<PointF>();
        for (int i = 0; i < 24; i++)
        {
            float theta = 2f * MathF.PI * i / 24f;
            pts.Add(new PointF(cx + r * MathF.Cos(theta), cy + r * MathF.Sin(theta)));
        }
        return FreehandPath.From(pts);
    }

    private static FreehandPath MakeEllipsePath()
    {
        float cx = 160, cy = 60, a = 160, b = 60;
        var pts = new List<PointF>();
        for (int i = 0; i < 24; i++)
        {
            float theta = 2f * MathF.PI * i / 24f;
            pts.Add(new PointF(cx + a * MathF.Cos(theta), cy + b * MathF.Sin(theta)));
        }
        return FreehandPath.From(pts);
    }

    // -----------------------------------------------------------------------
    // Line descriptor
    // -----------------------------------------------------------------------

    /// <summary>
    /// Line descriptor should score above the confidence threshold for a horizontal line gesture.
    /// </summary>
    [TestMethod]
    public void LineDescriptor_FitScore_ScoresHighForLinePath()
    {
        float score = new LineAnnotationDescriptor().FitScore(MakeLinePath());
        score.Should().BeGreaterThan(AnnotationShapeRecognizer.ConfidenceThreshold);
    }

    /// <summary>
    /// Line descriptor should score zero for a circular gesture (not elongated).
    /// </summary>
    [TestMethod]
    public void LineDescriptor_FitScore_ScoresZeroForCirclePath()
    {
        float score = new LineAnnotationDescriptor().FitScore(MakeCirclePath());
        score.Should().Be(0f);
    }

    /// <summary>
    /// Line descriptor should score zero for a rectangular gesture (aspect ratio too square).
    /// </summary>
    [TestMethod]
    public void LineDescriptor_FitScore_ScoresZeroForRectPath()
    {
        float score = new LineAnnotationDescriptor().FitScore(MakeRectPath());
        score.Should().Be(0f);
    }

    // -----------------------------------------------------------------------
    // Rectangle descriptor
    // -----------------------------------------------------------------------

    /// <summary>
    /// Rectangle descriptor should score above the confidence threshold for a rectangular trace.
    /// </summary>
    [TestMethod]
    public void RectDescriptor_FitScore_ScoresHighForRectPath()
    {
        float score = new RectAnnotationDescriptor().FitScore(MakeRectPath());
        score.Should().BeGreaterThan(AnnotationShapeRecognizer.ConfidenceThreshold);
    }

    /// <summary>
    /// Rectangle descriptor should score zero for a line gesture (one dimension is zero).
    /// </summary>
    [TestMethod]
    public void RectDescriptor_FitScore_ScoresZeroForLinePath()
    {
        float score = new RectAnnotationDescriptor().FitScore(MakeLinePath());
        score.Should().Be(0f);
    }

    /// <summary>
    /// Rectangle descriptor should score below the threshold for a circular gesture.
    /// </summary>
    [TestMethod]
    public void RectDescriptor_FitScore_ScoresBelowThresholdForCirclePath()
    {
        float score = new RectAnnotationDescriptor().FitScore(MakeCirclePath());
        score.Should().BeLessThan(AnnotationShapeRecognizer.ConfidenceThreshold);
    }

    // -----------------------------------------------------------------------
    // Circle descriptor
    // -----------------------------------------------------------------------

    /// <summary>
    /// Circle descriptor should score above the confidence threshold for a circular gesture.
    /// </summary>
    [TestMethod]
    public void CircleDescriptor_FitScore_ScoresHighForCirclePath()
    {
        float score = new CircleAnnotationDescriptor().FitScore(MakeCirclePath());
        score.Should().BeGreaterThan(AnnotationShapeRecognizer.ConfidenceThreshold);
    }

    /// <summary>
    /// Circle descriptor should score zero for a line gesture (aspect ratio too large).
    /// </summary>
    [TestMethod]
    public void CircleDescriptor_FitScore_ScoresZeroForLinePath()
    {
        float score = new CircleAnnotationDescriptor().FitScore(MakeLinePath());
        score.Should().Be(0f);
    }

    /// <summary>
    /// Circle descriptor should score zero for an ellipse gesture (aspect ratio too high).
    /// </summary>
    [TestMethod]
    public void CircleDescriptor_FitScore_ScoresZeroForEllipsePath()
    {
        float score = new CircleAnnotationDescriptor().FitScore(MakeEllipsePath());
        score.Should().Be(0f);
    }

    // -----------------------------------------------------------------------
    // Ellipse descriptor
    // -----------------------------------------------------------------------

    /// <summary>
    /// Ellipse descriptor should score above the confidence threshold for an elliptical gesture.
    /// </summary>
    [TestMethod]
    public void EllipseDescriptor_FitScore_ScoresHighForEllipsePath()
    {
        float score = new EllipseAnnotationDescriptor().FitScore(MakeEllipsePath());
        score.Should().BeGreaterThan(AnnotationShapeRecognizer.ConfidenceThreshold);
    }

    /// <summary>
    /// Ellipse descriptor should score zero for a circular gesture (aspect ratio too close to 1).
    /// </summary>
    [TestMethod]
    public void EllipseDescriptor_FitScore_ScoresZeroForCirclePath()
    {
        float score = new EllipseAnnotationDescriptor().FitScore(MakeCirclePath());
        score.Should().Be(0f);
    }

    /// <summary>
    /// Ellipse descriptor should score below the threshold for a rectangular gesture.
    /// </summary>
    [TestMethod]
    public void EllipseDescriptor_FitScore_ScoresBelowThresholdForRectPath()
    {
        float score = new EllipseAnnotationDescriptor().FitScore(MakeRectPath());
        score.Should().BeLessThan(AnnotationShapeRecognizer.ConfidenceThreshold);
    }

    // -----------------------------------------------------------------------
    // Polygon descriptor
    // -----------------------------------------------------------------------

    /// <summary>
    /// Polygon descriptor always returns 0.3 regardless of path content.
    /// </summary>
    [TestMethod]
    public void PolygonDescriptor_FitScore_AlwaysReturns03()
    {
        var d = new PolygonAnnotationDescriptor();
        d.FitScore(MakeLinePath()).Should().Be(0.3f);
        d.FitScore(MakeCirclePath()).Should().Be(0.3f);
        d.FitScore(MakeRectPath()).Should().Be(0.3f);
        d.FitScore(MakeEllipsePath()).Should().Be(0.3f);
    }

    // -----------------------------------------------------------------------
    // AnnotationShapeRecognizer.Rank
    // -----------------------------------------------------------------------

    /// <summary>
    /// Rank results should be sorted by confidence descending.
    /// </summary>
    [TestMethod]
    public void Rank_ResultsAreSortedDescending()
    {
        IAnnotationShapeDescriptor[] descriptors =
        [
            new LineAnnotationDescriptor(),
            new RectAnnotationDescriptor(),
            new CircleAnnotationDescriptor(),
            new EllipseAnnotationDescriptor(),
            new PolygonAnnotationDescriptor(),
        ];

        var ranked = AnnotationShapeRecognizer.Rank(MakeCirclePath(), descriptors);

        for (int i = 1; i < ranked.Count; i++)
        {
            ranked[i].Confidence.Should().BeLessThanOrEqualTo(ranked[i - 1].Confidence);
        }
    }

    /// <summary>
    /// Circle should be the top-ranked descriptor for a circular gesture.
    /// </summary>
    [TestMethod]
    public void Rank_CirclePath_TopRankedIsCircle()
    {
        IAnnotationShapeDescriptor[] descriptors =
        [
            new LineAnnotationDescriptor(),
            new RectAnnotationDescriptor(),
            new CircleAnnotationDescriptor(),
            new EllipseAnnotationDescriptor(),
            new PolygonAnnotationDescriptor(),
        ];

        var ranked = AnnotationShapeRecognizer.Rank(MakeCirclePath(), descriptors);

        ranked[0].Descriptor.Should().BeOfType<CircleAnnotationDescriptor>();
    }

    /// <summary>
    /// Line should be the top-ranked descriptor for a linear gesture.
    /// </summary>
    [TestMethod]
    public void Rank_LinePath_TopRankedIsLine()
    {
        IAnnotationShapeDescriptor[] descriptors =
        [
            new LineAnnotationDescriptor(),
            new RectAnnotationDescriptor(),
            new CircleAnnotationDescriptor(),
            new EllipseAnnotationDescriptor(),
            new PolygonAnnotationDescriptor(),
        ];

        var ranked = AnnotationShapeRecognizer.Rank(MakeLinePath(), descriptors);

        ranked[0].Descriptor.Should().BeOfType<LineAnnotationDescriptor>();
    }

    /// <summary>
    /// Rank should return an empty list when given an empty descriptor collection.
    /// </summary>
    [TestMethod]
    public void Rank_EmptyDescriptors_ReturnsEmpty()
    {
        var ranked = AnnotationShapeRecognizer.Rank(MakeLinePath(), []);
        ranked.Should().BeEmpty();
    }

    /// <summary>
    /// Polygon should be the top-ranked descriptor when it is the only entry and everything
    /// else would score below the confidence threshold (polygon always returns 0.3).
    /// </summary>
    [TestMethod]
    public void Rank_PolygonAlone_IsTopRanked()
    {
        var ranked = AnnotationShapeRecognizer.Rank(MakeLinePath(), [new PolygonAnnotationDescriptor()]);
        ranked[0].Descriptor.Should().BeOfType<PolygonAnnotationDescriptor>();
        ranked[0].Confidence.Should().Be(0.3f);
    }

    /// <summary>
    /// Rank should throw ArgumentNullException when path is null.
    /// </summary>
    [TestMethod]
    public void Rank_NullPath_ThrowsArgumentNullException()
    {
        Action act = () => AnnotationShapeRecognizer.Rank(null!, []);
        act.Should().Throw<ArgumentNullException>();
    }
}
