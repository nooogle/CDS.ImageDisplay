using System;
using System.Collections.Generic;
using System.Drawing;
using CDS.ImageDisplay.WinForms.Annotations.Internal;

namespace CDS.ImageDisplay.WinForms.Annotations.Shapes;

/// <summary>
/// Descriptor for rotated rectangle annotations. Fits a minimum-area bounding rectangle
/// to the freehand path using PCA to determine the optimal orientation.
/// </summary>
public sealed class RotatedRectAnnotationDescriptor : IAnnotationShapeDescriptor
{
    /// <inheritdoc/>
    public string Name => "Rotated Rectangle";

    /// <inheritdoc/>
    public string Description => "Minimum-area rotated rectangle fitted to the drawn gesture via PCA";

    /// <inheritdoc/>
    public float FitScore(FreehandPath path)
    {
        ArgumentNullException.ThrowIfNull(path);

        // Delegate to RectAnnotationDescriptor's score with a slight discount so the
        // axis-aligned rect wins when the gesture itself is axis-aligned.
        return new RectAnnotationDescriptor().FitScore(path) * 0.85f;
    }

    /// <inheritdoc/>
    public AnnotationGeometry CreateGeometry(FreehandPath path)
    {
        ArgumentNullException.ThrowIfNull(path);

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
    // PCA fitting
    // -----------------------------------------------------------------------

    private static (PointF center, float width, float height, float angleDegrees)
        FitRotatedRect(IReadOnlyList<PointF> points)
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

        // Angle of the first eigenvector (axis with largest variance).
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

        // Project all points onto the two principal axes and find the extent.
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

        // Centre of the bounding rect, back in image space.
        float centerX = cx + centerU * cosT - centerV * sinT;
        float centerY = cy + centerU * sinT + centerV * cosT;

        float angleDeg = theta * 180f / MathF.PI;

        return (new PointF(centerX, centerY), w, h, angleDeg);
    }
}
