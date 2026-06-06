using System;
using System.Collections.Generic;
using System.Drawing;

namespace CDS.ImageDisplay.Annotations.Shapes;

/// <summary>
/// Descriptor for polygon annotations. Acts as the catch-all shape when no other descriptor
/// achieves sufficient confidence during auto-recognition.
/// </summary>
public sealed class PolygonAnnotationDescriptor : IAnnotationShapeDescriptor
{
    private const int MaxVertices = 20;

    /// <inheritdoc/>
    public string Name => "Polygon";

    /// <inheritdoc/>
    public string Description => "Arbitrary polygon fitted to the drawn gesture";

    /// <summary>
    /// Always returns 0.3 — polygon is the catch-all when no other shape is confident enough.
    /// </summary>
    public float FitScore(FreehandPath path) => 0.3f;

    /// <inheritdoc/>
    public AnnotationGeometry CreateGeometry(FreehandPath path)
    {
        ArgumentNullException.ThrowIfNull(path, nameof(path));

        IReadOnlyList<System.Drawing.PointF> points = path.Points;
        var vertices = new List<Point>(Math.Min(points.Count, MaxVertices));

        if (points.Count <= MaxVertices)
        {
            foreach (PointF p in points)
            {
                vertices.Add(Point.Round(p));
            }
        }
        else
        {
            // Sample uniformly so we keep no more than MaxVertices points.
            float step = (points.Count - 1f) / (MaxVertices - 1f);

            for (int i = 0; i < MaxVertices; i++)
            {
                int index = (int)MathF.Round(i * step);
                vertices.Add(Point.Round(points[index]));
            }
        }

        var geometry = new PolygonAnnotationGeometry(vertices);
        geometry.Drawing.Lines.Color = Color.Orange;
        geometry.Drawing.Lines.Width = 2f;
        geometry.Drawing.Fill.Color = Color.FromArgb(25, Color.Orange);
        return geometry;
    }
}
