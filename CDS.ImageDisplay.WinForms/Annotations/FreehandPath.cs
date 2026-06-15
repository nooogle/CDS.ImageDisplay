using System;
using System.Collections.Generic;
using System.Drawing;

namespace CDS.ImageDisplay.WinForms.Annotations;

/// <summary>
/// An immutable record of points captured during a freehand drawing gesture, in image coordinates.
/// </summary>
public sealed class FreehandPath
{
    /// <summary>
    /// The points captured during the gesture, in image coordinates.
    /// </summary>
    public IReadOnlyList<PointF> Points { get; }

    /// <summary>
    /// The axis-aligned bounding box of all points, in image coordinates.
    /// Returns <see cref="RectangleF.Empty"/> when the sequence is empty.
    /// A single-point path returns a zero-sized rectangle at that point.
    /// </summary>
    public RectangleF BoundingBox { get; }

    /// <summary>
    /// The geometric centroid of all points, in image coordinates.
    /// Returns <see cref="PointF.Empty"/> when the sequence is empty.
    /// </summary>
    public PointF Centroid { get; }

    /// <summary>
    /// The sum of distances between consecutive points, in image pixels.
    /// Returns 0 when there are fewer than two points.
    /// </summary>
    public float ApproximatePerimeter { get; }

    private FreehandPath(List<PointF> points)
    {
        Points = points.AsReadOnly();
        BoundingBox = ComputeBoundingBox(points);
        Centroid = ComputeCentroid(points);
        ApproximatePerimeter = ComputePerimeter(points);
    }

    /// <summary>
    /// Creates a <see cref="FreehandPath"/> from a sequence of image-coordinate points.
    /// </summary>
    /// <param name="points">The points in image coordinates.</param>
    public static FreehandPath From(IEnumerable<PointF> points)
    {
        if(points == null) {  throw  new ArgumentNullException(nameof(points)); }
        return new FreehandPath(new List<PointF>(points));
    }

    private static RectangleF ComputeBoundingBox(List<PointF> points)
    {
        if (points.Count == 0) { return RectangleF.Empty; }

        float minX = points[0].X, maxX = points[0].X;
        float minY = points[0].Y, maxY = points[0].Y;

        for (int i = 1; i < points.Count; i++)
        {
            PointF p = points[i];
            if (p.X < minX) { minX = p.X; }
            if (p.X > maxX) { maxX = p.X; }
            if (p.Y < minY) { minY = p.Y; }
            if (p.Y > maxY) { maxY = p.Y; }
        }

        return RectangleF.FromLTRB(minX, minY, maxX, maxY);
    }

    private static PointF ComputeCentroid(List<PointF> points)
    {
        if (points.Count == 0) { return PointF.Empty; }

        float sumX = 0f, sumY = 0f;

        foreach (PointF p in points)
        {
            sumX += p.X;
            sumY += p.Y;
        }

        return new PointF(sumX / points.Count, sumY / points.Count);
    }

    private static float ComputePerimeter(List<PointF> points)
    {
        if (points.Count < 2) { return 0f; }

        float total = 0f;

        for (int i = 1; i < points.Count; i++)
        {
            float dx = points[i].X - points[i - 1].X;
            float dy = points[i].Y - points[i - 1].Y;
            total += MathF.Sqrt((dx * dx) + (dy * dy));
        }

        return total;
    }
}
