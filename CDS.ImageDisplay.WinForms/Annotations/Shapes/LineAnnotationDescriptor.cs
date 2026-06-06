using System;
using System.Drawing;
using CDS.ImageDisplay.Annotations.Internal;

namespace CDS.ImageDisplay.Annotations.Shapes;

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
        ArgumentNullException.ThrowIfNull(path, nameof(path));

        if (path.Points.Count < 2) { return 0f; }

        float w = path.BoundingBox.Width;
        float h = path.BoundingBox.Height;
        float longer = MathF.Max(w, h);
        float shorter = MathF.Min(w, h);

        if (longer < 1f) { return 0f; }

        // Must be elongated — a square-ish bbox is not a line.
        float elongation = longer / (shorter + 1f);
        if (elongation < 2f) { return 0f; }

        float elongationScore = FreehandPathAnalyser.Clamp01((elongation - 2f) / 3f);

        // Points should lie close to the straight line through the endpoints.
        float meanDev = FreehandPathAnalyser.MeanPerpDeviation(path.Points, path.Points[0], path.Points[^1]);
        float deviationScore = FreehandPathAnalyser.Clamp01(1f - meanDev / (longer * 0.2f));

        return elongationScore * deviationScore;
    }

    /// <inheritdoc/>
    public AnnotationGeometry CreateGeometry(FreehandPath path)
    {
        ArgumentNullException.ThrowIfNull(path, nameof(path));

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
            end = Point.Round(path.Points[^1]);
        }

        var geometry = new LineAnnotationGeometry(start, end);
        geometry.Drawing.Lines.Color = Color.Gold;
        geometry.Drawing.Lines.Width = 2f;
        geometry.Drawing.Fill.Color = Color.Transparent;
        return geometry;
    }
}
