using System;
using System.Drawing;
using CDS.ImageDisplay.WinForms.Annotations.Internal;

namespace CDS.ImageDisplay.WinForms.Annotations.Shapes;

/// <summary>
/// Descriptor for circle annotations.
/// </summary>
public sealed class CircleAnnotationDescriptor : IAnnotationShapeDescriptor
{
    /// <inheritdoc/>
    public string Name => "Circle";

    /// <inheritdoc/>
    public string Description => "Circle fitted to the bounding box of the drawn gesture";

    /// <inheritdoc/>
    public float FitScore(FreehandPath path)
    {
        if (path == null) { throw new ArgumentNullException(nameof(path)); }

        RectangleF bbox = path.BoundingBox;
        float w = bbox.Width;
        float h = bbox.Height;

        if (w < 1f || h < 1f) { return 0f; }

        // Bounding box must be approximately square.
        float aspect = MathF.Max(w, h) / MathF.Min(w, h);
        float aspectScore = FreehandPathAnalyser.Clamp01(1f - (aspect - 1f) / 0.4f);
        if (aspectScore <= 0f) { return 0f; }

        // All points should be roughly equidistant from the centroid.
        var (meanR, stdDev) = FreehandPathAnalyser.RadiusStats(path.Points, path.Centroid);
        float normStdDev = stdDev / (meanR + 1f);
        float varianceScore = FreehandPathAnalyser.Clamp01(1f - normStdDev / 0.15f);

        // The gesture should trace a full loop: path perimeter ≈ 2π × mean radius.
        float perimeterRatio = path.ApproximatePerimeter / (2f * MathF.PI * MathF.Max(meanR, 1f));
        float perimeterScore = FreehandPathAnalyser.Clamp01(1f - MathF.Abs(perimeterRatio - 1f) / 0.5f);

        // For 7+ points use the algebraic ellipse fit to confirm the shape is circular.
        // Only applied when the fit succeeds and has low residual error; otherwise neutral (1).
        float algebraicScore = 1f;
        if (path.Points.Count >= 7)
        {
            FittedEllipse? fit = FreehandPathAnalyser.FitEllipse(path.Points);
            if (fit.HasValue && fit.Value.MeanAlgebraicError < 0.3f)
            {
                float axisRatio = fit.Value.SemiMinor / fit.Value.SemiMajor;
                algebraicScore = FreehandPathAnalyser.Clamp01((axisRatio - 0.7f) / 0.3f);
            }
        }

        return aspectScore * varianceScore * perimeterScore * algebraicScore;
    }

    /// <inheritdoc/>
    public AnnotationGeometry CreateGeometry(FreehandPath path)
    {
        if (path == null) { throw new ArgumentNullException(nameof(path)); }

        RectangleF bbox = path.BoundingBox;
        var centre = new Point(
            (int)MathF.Round(bbox.Left + bbox.Width / 2f),
            (int)MathF.Round(bbox.Top + bbox.Height / 2f));
        int radius = Math.Max(1, (int)MathF.Round(Math.Min(bbox.Width, bbox.Height) / 2f));

        var geometry = new CircleAnnotationGeometry(centre, radius);
        geometry.Drawing.Lines.Color = Color.DodgerBlue;
        geometry.Drawing.Lines.Width = 2f;
        geometry.Drawing.Fill.Color = Color.Transparent;
        return geometry;
    }
}
