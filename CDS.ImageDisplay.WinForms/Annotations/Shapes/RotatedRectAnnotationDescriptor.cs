using System;
using System.Collections.Generic;
using System.Drawing;
using CDS.ImageDisplay.WinForms.Annotations.Internal;

namespace CDS.ImageDisplay.WinForms.Annotations.Shapes;

/// <summary>
/// Descriptor for rotated rectangle annotations. Uses the algebraic ellipse fit to determine
/// the primary axis direction, scores based on how closely the path tracks the four edges of
/// the oriented bounding box, and only competes with <see cref="RectAnnotationDescriptor"/>
/// when the gesture is meaningfully off-axis.
/// </summary>
public sealed class RotatedRectAnnotationDescriptor : IAnnotationShapeDescriptor
{
    /// <inheritdoc/>
    public string Name => "Rotated Rectangle";

    /// <inheritdoc/>
    public string Description => "Minimum-area rotated rectangle fitted to the drawn gesture";

    /// <inheritdoc/>
    public float FitScore(FreehandPath path)
    {
        if (path == null) { throw new ArgumentNullException(nameof(path)); }

        IReadOnlyList<PointF> points = path.Points;
        if (points.Count < 3) { return 0f; }

        // Use the ellipse fit for a robust primary-axis direction; fall back to PCA.
        float angleDeg;
        FittedEllipse? fit = FreehandPathAnalyser.FitEllipse(points);
        if (fit.HasValue)
        {
            angleDeg = fit.Value.AngleDegrees;
        }
        else
        {
            angleDeg = ComputePcaAngle(points);
        }

        // Only compete with RectAnnotationDescriptor when there is meaningful rotation.
        // angleDeviation = distance from the nearest axis (0° or 90°), range [0°, 45°].
        float angleFromAxis = angleDeg % 90f;
        if (angleFromAxis < 0f) { angleFromAxis += 90f; }
        float angleDeviation = MathF.Min(angleFromAxis, 90f - angleFromAxis);
        // Score is 0 below 10° off-axis, ramps to 1 at 25°.
        float rotationScore = FreehandPathAnalyser.Clamp01((angleDeviation - 10f) / 15f);
        if (rotationScore <= 0f) { return 0f; }

        // Project all points into the rotated frame and compute the oriented bounding box.
        float rad = angleDeg * MathF.PI / 180f;
        float cosT = MathF.Cos(rad);
        float sinT = MathF.Sin(rad);

        float cx = 0f, cy = 0f;
        foreach (PointF p in points) { cx += p.X; cy += p.Y; }
        cx /= points.Count;
        cy /= points.Count;

        var rotated = new PointF[points.Count];
        float minU = float.MaxValue, maxU = float.MinValue;
        float minV = float.MaxValue, maxV = float.MinValue;

        for (int i = 0; i < points.Count; i++)
        {
            float dx = points[i].X - cx, dy = points[i].Y - cy;
            float u = dx * cosT + dy * sinT;
            float v = -dx * sinT + dy * cosT;
            rotated[i] = new PointF(u, v);
            if (u < minU) { minU = u; }
            if (u > maxU) { maxU = u; }
            if (v < minV) { minV = v; }
            if (v > maxV) { maxV = v; }
        }

        float w = maxU - minU;
        float h = maxV - minV;
        if (w < 1f || h < 1f) { return 0f; }

        // Points should track the four edges of the oriented bounding box.
        var rotatedBbox = new RectangleF(minU, minV, w, h);
        float meanEdgeDist = FreehandPathAnalyser.MeanEdgeDistance(rotated, rotatedBbox);
        float diagonal = MathF.Sqrt(w * w + h * h);
        float edgeScore = FreehandPathAnalyser.Clamp01(1f - meanEdgeDist / (diagonal * 0.15f));

        // Penalise very elongated shapes — those are better recognised as lines or ellipses.
        float aspect = MathF.Max(w, h) / MathF.Min(w, h);
        float aspectScore = FreehandPathAnalyser.Clamp01(1f - (aspect - 5f) / 2f);

        return rotationScore * edgeScore * aspectScore;
    }

    /// <inheritdoc/>
    public AnnotationGeometry CreateGeometry(FreehandPath path)
    {
        if (path == null) { throw new ArgumentNullException(nameof(path)); }

        IReadOnlyList<PointF> points = path.Points;
        PointF center;
        float width, height, angleDegrees;

        if (points.Count < 2)
        {
            center = points.Count == 1 ? points[0] : PointF.Empty;
            width = 1f;
            height = 1f;
            angleDegrees = 0f;
        }
        else
        {
            (center, width, height, angleDegrees) = FitRotatedRect(points);
        }

        var geometry = new RotatedRectAnnotationGeometry(center, width, height, angleDegrees);
        geometry.Drawing.Lines.Color = Color.OrangeRed;
        geometry.Drawing.Lines.Width = 2f;
        geometry.Drawing.Fill.Color = Color.FromArgb(25, Color.OrangeRed);
        return geometry;
    }

    // -----------------------------------------------------------------------
    // Helpers
    // -----------------------------------------------------------------------

    private static (PointF center, float width, float height, float angleDegrees)
        FitRotatedRect(IReadOnlyList<PointF> points)
    {
        // The ellipse fit gives a more accurate primary-axis direction than raw PCA for
        // rectangle-perimeter paths where the point cloud has two dominant directions.
        float theta;
        FittedEllipse? fit = FreehandPathAnalyser.FitEllipse(points);
        if (fit.HasValue)
        {
            theta = fit.Value.AngleDegrees * MathF.PI / 180f;
        }
        else
        {
            theta = ComputePcaAngle(points) * MathF.PI / 180f;
        }

        float cosT = MathF.Cos(theta);
        float sinT = MathF.Sin(theta);

        float cx = 0f, cy = 0f;
        foreach (PointF p in points) { cx += p.X; cy += p.Y; }
        cx /= points.Count;
        cy /= points.Count;

        float minU = float.MaxValue, maxU = float.MinValue;
        float minV = float.MaxValue, maxV = float.MinValue;

        foreach (PointF p in points)
        {
            float dx = p.X - cx, dy = p.Y - cy;
            float u = dx * cosT + dy * sinT;
            float v = -dx * sinT + dy * cosT;
            if (u < minU) { minU = u; }
            if (u > maxU) { maxU = u; }
            if (v < minV) { minV = v; }
            if (v > maxV) { maxV = v; }
        }

        float w = MathF.Max(1f, maxU - minU);
        float h = MathF.Max(1f, maxV - minV);
        float centerU = (minU + maxU) / 2f;
        float centerV = (minV + maxV) / 2f;

        float centerX = cx + centerU * cosT - centerV * sinT;
        float centerY = cy + centerU * sinT + centerV * cosT;

        return (new PointF(centerX, centerY), w, h, theta * 180f / MathF.PI);
    }

    private static float ComputePcaAngle(IReadOnlyList<PointF> points)
        => FreehandPathAnalyser.ComputePca(points).AngleDegrees;
}
