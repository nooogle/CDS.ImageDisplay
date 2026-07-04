using System;
using System.Drawing;
using CDS.ImageDisplay.WinForms.Annotations.Internal;

namespace CDS.ImageDisplay.WinForms.Annotations.Shapes;

/// <summary>
/// Descriptor for circle annotations. Uses the algebraic ellipse fit when enough points are
/// available (5+), falling back to bounding-box aspect ratio for shorter paths.
/// </summary>
public sealed class CircleAnnotationDescriptor : IAnnotationShapeDescriptor
{
    /// <inheritdoc/>
    public string Name => "Circle";

    /// <inheritdoc/>
    public string Description => "Circle fitted to the drawn gesture";

    /// <inheritdoc/>
    public float FitScore(FreehandPath path)
    {
        if (path == null) { throw new ArgumentNullException(nameof(path)); }

        FittedEllipse? fit = FreehandPathAnalyser.FitEllipse(path.Points);
        if (!fit.HasValue) { return FallbackScore(path); }

        FittedEllipse e = fit.Value;

        // Circle: the two fitted axes must be nearly equal.
        float ratio = e.SemiMajor / MathF.Max(e.SemiMinor, 0.01f);
        float aspectScore = FreehandPathAnalyser.Clamp01(1f - (ratio - 1f) / 0.35f);
        if (aspectScore <= 0f) { return 0f; }

        // How well the path points lie on the fitted ellipse boundary.
        float meanError = FreehandPathAnalyser.MeanRotatedEllipseBoundaryError(
            path.Points, e.Center, e.SemiMajor, e.SemiMinor, e.AngleDegrees);
        float boundaryScore = FreehandPathAnalyser.Clamp01(1f - meanError / 0.4f);

        return aspectScore * boundaryScore;
    }

    /// <inheritdoc/>
    public AnnotationGeometry CreateGeometry(FreehandPath path)
    {
        if (path == null) { throw new ArgumentNullException(nameof(path)); }

        Point centre;
        int radius;

        FittedEllipse? fit = FreehandPathAnalyser.FitEllipse(path.Points);
        if (fit.HasValue)
        {
            centre = new Point(
                (int)MathF.Round(fit.Value.Center.X),
                (int)MathF.Round(fit.Value.Center.Y));
            radius = Math.Max(1, (int)MathF.Round((fit.Value.SemiMajor + fit.Value.SemiMinor) / 2f));
        }
        else
        {
            RectangleF bbox = path.BoundingBox;
            centre = new Point(
                (int)MathF.Round(bbox.Left + bbox.Width / 2f),
                (int)MathF.Round(bbox.Top + bbox.Height / 2f));
            radius = Math.Max(1, (int)MathF.Round(Math.Min(bbox.Width, bbox.Height) / 2f));
        }

        var geometry = new CircleAnnotationGeometry(centre, radius);
        geometry.Drawing.Lines.Color = Color.DodgerBlue;
        geometry.Drawing.Lines.Width = 2f;
        geometry.Drawing.Fill.Color = Color.Transparent;
        return geometry;
    }

    // Bounding-box aspect check for the rare case of fewer than 5 path points.
    private static float FallbackScore(FreehandPath path)
    {
        RectangleF bbox = path.BoundingBox;
        float w = bbox.Width, h = bbox.Height;
        if (w < 1f || h < 1f) { return 0f; }
        float aspect = MathF.Max(w, h) / MathF.Min(w, h);
        return FreehandPathAnalyser.Clamp01(1f - (aspect - 1f) / 0.4f);
    }
}
