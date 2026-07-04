using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using CDS.ImageDisplay.WinForms.Annotations.Internal;

namespace CDS.ImageDisplay.WinForms.Annotations.Shapes;

/// <summary>
/// Descriptor for line annotations.
/// </summary>
public sealed class LineAnnotationDescriptor : IAnnotationShapeDescriptor
{
    /// <inheritdoc/>
    public string Name => "Line";

    /// <inheritdoc/>
    public string Description => "Straight line between the first and last points of the drawn gesture";

    /// <inheritdoc/>
    public float FitScore(FreehandPath path)
    {
        if (path == null) { throw new ArgumentNullException(nameof(path)); }

        if (path.Points.Count < 2) { return 0f; }

        (float lambda1, float lambda2, _) = FreehandPathAnalyser.ComputePca(path.Points);
        if (lambda1 < 1f) { return 0f; }

        // Shape must be strongly elongated along one axis. Variance ratio λ1/λ2 is the square
        // of the aspect ratio, so ratio=4 ≈ 2:1 and ratio=25 ≈ 5:1. Using PCA means diagonal
        // lines are correctly detected — bounding-box aspect fails for off-axis gestures.
        float varRatio = lambda1 / (lambda2 + 1f);
        float elongationScore = FreehandPathAnalyser.Clamp01((varRatio - 4f) / 21f);
        if (elongationScore <= 0f) { return 0f; }

        // Points must cluster tightly around the major axis. sqrt(λ2)/sqrt(λ1) is the ratio of
        // perpendicular std to parallel std; 0 for a perfect line, ~0.4 for an arc or fat stroke.
        float linearityScore = FreehandPathAnalyser.Clamp01(
            1f - MathF.Sqrt(lambda2) / (MathF.Sqrt(lambda1) * 0.4f));

        return elongationScore * linearityScore;
    }

    /// <inheritdoc/>
    public AnnotationGeometry CreateGeometry(FreehandPath path)
    {
        if (path == null) { throw new ArgumentNullException(nameof(path)); }

        Point start, end;

        if (path.Points.Count == 0)
        {
            start = Point.Empty;
            end = Point.Empty;
        }
        else if (path.Points.Count == 1)
        {
            start = Point.Round(path.Points[0]);
            end = start;
        }
        else
        {
            start = Point.Round(path.Points[0]);
            end = Point.Round(path.Points[path.Points.Count - 1]);
        }

        var geometry = new LineAnnotationGeometry(start, end);
        geometry.Drawing.Lines.Color = Color.Gold;
        geometry.Drawing.Lines.Width = 2f;
        geometry.Drawing.Lines.EndCap = LineCap.ArrowAnchor;
        geometry.Drawing.Fill.Color = Color.Transparent;
        return geometry;
    }
}
