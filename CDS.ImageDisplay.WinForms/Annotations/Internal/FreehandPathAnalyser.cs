using System;
using System.Collections.Generic;
using System.Drawing;

namespace CDS.ImageDisplay.WinForms.Annotations.Internal;

/// <summary>
/// Geometric analysis helpers used by shape descriptors to score freehand gesture paths.
/// </summary>
internal static class FreehandPathAnalyser
{
    /// <summary>Clamps <paramref name="value"/> to [0, 1].</summary>
    internal static float Clamp01(float value) => MathF.Max(0f, MathF.Min(1f, value));

    /// <summary>
    /// Returns the mean perpendicular distance of all points from the line through
    /// <paramref name="lineStart"/> and <paramref name="lineEnd"/>.
    /// Returns 0 when the line has zero length or the list is empty.
    /// </summary>
    internal static float MeanPerpDeviation(IReadOnlyList<PointF> points, PointF lineStart, PointF lineEnd)
    {
        if (points.Count == 0) { return 0f; }

        float dx = lineEnd.X - lineStart.X;
        float dy = lineEnd.Y - lineStart.Y;
        float lineLength = MathF.Sqrt(dx * dx + dy * dy);

        if (lineLength < 0.001f) { return 0f; }

        float total = 0f;
        foreach (PointF p in points)
        {
            float cross = MathF.Abs((p.X - lineStart.X) * dy - (p.Y - lineStart.Y) * dx);
            total += cross / lineLength;
        }

        return total / points.Count;
    }

    /// <summary>
    /// Returns the mean distance of each point from the nearest of the four bounding-box edges.
    /// </summary>
    internal static float MeanEdgeDistance(IReadOnlyList<PointF> points, RectangleF bbox)
    {
        if (points.Count == 0) { return 0f; }

        float total = 0f;
        foreach (PointF p in points)
        {
            float toLeft = MathF.Abs(p.X - bbox.Left);
            float toRight = MathF.Abs(p.X - bbox.Right);
            float toTop = MathF.Abs(p.Y - bbox.Top);
            float toBottom = MathF.Abs(p.Y - bbox.Bottom);
            total += MathF.Min(MathF.Min(toLeft, toRight), MathF.Min(toTop, toBottom));
        }

        return total / points.Count;
    }

    /// <summary>
    /// Computes the mean and standard deviation of point distances from <paramref name="centre"/>.
    /// </summary>
    internal static (float Mean, float StdDev) RadiusStats(IReadOnlyList<PointF> points, PointF centre)
    {
        if (points.Count == 0) { return (0f, 0f); }

        float sum = 0f;
        foreach (PointF p in points)
        {
            float dx = p.X - centre.X;
            float dy = p.Y - centre.Y;
            sum += MathF.Sqrt(dx * dx + dy * dy);
        }

        float mean = sum / points.Count;

        float varSum = 0f;
        foreach (PointF p in points)
        {
            float dx = p.X - centre.X;
            float dy = p.Y - centre.Y;
            float r = MathF.Sqrt(dx * dx + dy * dy);
            float dev = r - mean;
            varSum += dev * dev;
        }

        return (mean, MathF.Sqrt(varSum / points.Count));
    }

    /// <summary>
    /// Computes the mean absolute deviation of points from the ellipse boundary
    /// <c>((x − cx) / a)² + ((y − cy) / b)² = 1</c>.
    /// Returns 1 when the ellipse is degenerate or the list is empty.
    /// </summary>
    internal static float MeanEllipseBoundaryError(
        IReadOnlyList<PointF> points, float cx, float cy, float a, float b)
    {
        if (points.Count == 0 || a < 0.001f || b < 0.001f) { return 1f; }

        float total = 0f;
        foreach (PointF p in points)
        {
            float nx = (p.X - cx) / a;
            float ny = (p.Y - cy) / b;
            total += MathF.Abs(nx * nx + ny * ny - 1f);
        }

        return total / points.Count;
    }
}
