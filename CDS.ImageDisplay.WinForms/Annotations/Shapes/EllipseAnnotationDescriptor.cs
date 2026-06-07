using System;
using System.Collections.Generic;
using System.Drawing;
using CDS.ImageDisplay.Annotations.Internal;

namespace CDS.ImageDisplay.Annotations.Shapes;

/// <summary>
/// Descriptor for ellipse annotations. Fits a rotated ellipse to the freehand gesture
/// using PCA to determine the optimal orientation and axis lengths.
/// </summary>
public sealed class EllipseAnnotationDescriptor : IAnnotationShapeDescriptor
{
    /// <inheritdoc/>
    public string Name => "Ellipse";

    /// <inheritdoc/>
    public string Description => "Rotated ellipse fitted to the drawn gesture via PCA";

    /// <inheritdoc/>
    public float FitScore(FreehandPath path)
    {
        ArgumentNullException.ThrowIfNull(path, nameof(path));

        IReadOnlyList<PointF> points = path.Points;
        if (points.Count < 3) { return 0f; }

        RectangleF bbox = path.BoundingBox;
        float w = bbox.Width;
        float h = bbox.Height;
        if (w < 1f || h < 1f) { return 0f; }

        // Aspect ratio check: reject near-circles (handled by CircleAnnotationDescriptor).
        float aspect = MathF.Max(w, h) / MathF.Min(w, h);
        float aspectFactor = FreehandPathAnalyser.Clamp01((aspect - 1.3f) / 0.3f);
        if (aspectFactor <= 0f) { return 0f; }

        // Compute PCA ellipse and check how well points lie on its boundary.
        (PointF center, float semiMajor, float semiMinor, float angleDeg) = ComputePcaEllipse(points);
        float meanError = MeanPcaEllipseBoundaryError(points, center, semiMajor, semiMinor, angleDeg);
        float boundaryScore = FreehandPathAnalyser.Clamp01(1f - meanError / 0.3f);

        return aspectFactor * boundaryScore;
    }

    /// <inheritdoc/>
    public AnnotationGeometry CreateGeometry(FreehandPath path)
    {
        ArgumentNullException.ThrowIfNull(path, nameof(path));

        IReadOnlyList<PointF> points = path.Points;
        PointF center;
        float semiMajor, semiMinor, angleDegrees;

        if (points.Count < 2)
        {
            center = points.Count == 1 ? points[0] : PointF.Empty;
            semiMajor = 1f;
            semiMinor = 1f;
            angleDegrees = 0f;
        }
        else
        {
            (center, semiMajor, semiMinor, angleDegrees) = ComputePcaEllipse(points);
        }

        var geometry = new EllipseAnnotationGeometry(center, semiMajor, semiMinor, angleDegrees);
        geometry.Drawing.Lines.Color = Color.CornflowerBlue;
        geometry.Drawing.Lines.Width = 2f;
        geometry.Drawing.Fill.Color = Color.Transparent;
        return geometry;
    }

    // -----------------------------------------------------------------------
    // PCA helpers
    // -----------------------------------------------------------------------

    private static (PointF center, float semiMajor, float semiMinor, float angleDegrees)
        ComputePcaEllipse(IReadOnlyList<PointF> points)
    {
        // Centroid.
        float cx = 0f, cy = 0f;
        foreach (PointF p in points) { cx += p.X; cy += p.Y; }
        cx /= points.Count;
        cy /= points.Count;

        // Covariance matrix.
        float mxx = 0f, mxy = 0f, myy = 0f;
        foreach (PointF p in points)
        {
            float dx = p.X - cx, dy = p.Y - cy;
            mxx += dx * dx;
            mxy += dx * dy;
            myy += dy * dy;
        }
        mxx /= points.Count;
        mxy /= points.Count;
        myy /= points.Count;

        // Principal axis angle.
        float theta;
        if (MathF.Abs(mxy) < 1e-6f)
        {
            theta = mxx >= myy ? 0f : MathF.PI / 2f;
        }
        else
        {
            float trace = mxx + myy;
            float det = mxx * myy - mxy * mxy;
            float lambda1 = (trace + MathF.Sqrt(MathF.Max(0f, trace * trace - 4f * det))) / 2f;
            theta = MathF.Atan2(lambda1 - myy, mxy);
        }

        float cosT = MathF.Cos(theta);
        float sinT = MathF.Sin(theta);

        // Project points onto the two axes and take the maximum absolute projection
        // as the semi-axis length. This gives the bounding ellipse in the PCA orientation.
        float maxU = 0f, maxV = 0f;
        foreach (PointF p in points)
        {
            float dx = p.X - cx, dy = p.Y - cy;
            float u = MathF.Abs(dx * cosT + dy * sinT);
            float v = MathF.Abs(-dx * sinT + dy * cosT);
            if (u > maxU) { maxU = u; }
            if (v > maxV) { maxV = v; }
        }

        float semiMajor = MathF.Max(1f, maxU);
        float semiMinor = MathF.Max(1f, maxV);

        // Ensure semiMajor >= semiMinor by convention.
        if (semiMinor > semiMajor)
        {
            (semiMajor, semiMinor) = (semiMinor, semiMajor);
            theta += MathF.PI / 2f;
        }

        float angleDeg = theta * 180f / MathF.PI;

        return (new PointF(cx, cy), semiMajor, semiMinor, angleDeg);
    }

    private static float MeanPcaEllipseBoundaryError(
        IReadOnlyList<PointF> points, PointF center, float semiMajor, float semiMinor, float angleDeg)
    {
        if (points.Count == 0 || semiMajor < 0.001f || semiMinor < 0.001f) { return 1f; }

        float rad = angleDeg * MathF.PI / 180f;
        float cosA = MathF.Cos(rad);
        float sinA = MathF.Sin(rad);

        float total = 0f;
        foreach (PointF p in points)
        {
            float dx = p.X - center.X, dy = p.Y - center.Y;
            // Rotate into ellipse local frame.
            float u = dx * cosA + dy * sinA;
            float v = -dx * sinA + dy * cosA;
            float norm = (u / semiMajor) * (u / semiMajor) + (v / semiMinor) * (v / semiMinor);
            total += MathF.Abs(norm - 1f);
        }

        return total / points.Count;
    }
}
