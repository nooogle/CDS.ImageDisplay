using System;
using System.Drawing;
using CDS.ImageDisplay.WinForms.Annotations.Internal;

namespace CDS.ImageDisplay.WinForms.Annotations.Shapes;

/// <summary>
/// Descriptor for rectangle annotations.
/// </summary>
public sealed class RectAnnotationDescriptor : IAnnotationShapeDescriptor
{
    /// <inheritdoc/>
    public string Name => "Rectangle";

    /// <inheritdoc/>
    public string Description => "Axis-aligned bounding rectangle";

    /// <inheritdoc/>
    public float FitScore(FreehandPath path)
    {
        Guard.ThrowIfNull(path, nameof(path));

        RectangleF bbox = path.BoundingBox;
        float w = bbox.Width;
        float h = bbox.Height;

        if (w < 1f || h < 1f) { return 0f; }

        // Penalise very elongated shapes — those are better matched as lines.
        float aspect = MathF.Max(w, h) / MathF.Min(w, h);
        if (aspect > 6f) { return 0f; }
        float aspectPenalty = FreehandPathAnalyser.Clamp01(1f - (aspect - 4f) / 2f);

        // Perimeter should be close to 2*(W+H) for a rectangular trace.
        float fillRatio = path.ApproximatePerimeter / (2f * (w + h));
        float fillScore = FreehandPathAnalyser.Clamp01(1f - MathF.Abs(fillRatio - 1f) / 0.3f);

        // Points should sit close to the four edges of the bounding box.
        float meanEdgeDist = FreehandPathAnalyser.MeanEdgeDistance(path.Points, bbox);
        float diagonal = MathF.Sqrt(w * w + h * h);
        float edgeScore = FreehandPathAnalyser.Clamp01(1f - meanEdgeDist / (diagonal * 0.15f));

        return aspectPenalty * fillScore * edgeScore;
    }

    /// <inheritdoc/>
    public AnnotationGeometry CreateGeometry(FreehandPath path)
    {
        Guard.ThrowIfNull(path, nameof(path));

        RectangleF bbox = path.BoundingBox;
        var bounds = Rectangle.FromLTRB(
            (int)MathF.Floor(bbox.Left),
            (int)MathF.Floor(bbox.Top),
            (int)MathF.Ceiling(bbox.Right),
            (int)MathF.Ceiling(bbox.Bottom));

        if (bounds.Width < 1) { bounds = new Rectangle(bounds.X, bounds.Y, 1, bounds.Height); }
        if (bounds.Height < 1) { bounds = new Rectangle(bounds.X, bounds.Y, bounds.Width, 1); }

        var geometry = new RectAnnotationGeometry(bounds);
        geometry.Drawing.Lines.Color = Color.LimeGreen;
        geometry.Drawing.Lines.Width = 2f;
        geometry.Drawing.Fill.Color = Color.FromArgb(25, Color.LimeGreen);
        return geometry;
    }
}
