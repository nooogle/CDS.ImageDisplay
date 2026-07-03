using System;
using System.Collections.Generic;
using System.Drawing;
using CDS.ImageDisplay.WinForms.Annotations.Internal;

namespace CDS.ImageDisplay.WinForms.Annotations.Shapes;

/// <summary>
/// Descriptor for ellipse annotations. Uses a direct algebraic fit when enough points are
/// available (7+), falling back to PCA for shorter paths.
/// </summary>
public sealed class EllipseAnnotationDescriptor : IAnnotationShapeDescriptor
{
    /// <inheritdoc/>
    public string Name => "Ellipse";

    /// <inheritdoc/>
    public string Description => "Rotated ellipse fitted to the drawn gesture";

    /// <inheritdoc/>
    public float FitScore(FreehandPath path)
    {
        if (path == null) { throw new ArgumentNullException(nameof(path)); }

        IReadOnlyList<PointF> points = path.Points;
        if (points.Count < 3) { return 0f; }

        RectangleF bbox = path.BoundingBox;
        float w = bbox.Width;
        float h = bbox.Height;
        if (w < 1f || h < 1f) { return 0f; }

        // Reject near-circles — those are handled by CircleAnnotationDescriptor.
        float aspect = MathF.Max(w, h) / MathF.Min(w, h);
        float aspectFactor = FreehandPathAnalyser.Clamp01((aspect - 1.3f) / 0.3f);
        if (aspectFactor <= 0f) { return 0f; }

        // For 7+ points use the algebraic direct fit; fall back to PCA for shorter paths.
        float boundaryScore = points.Count >= 7
            ? AlgebraicBoundaryScore(points)
            : PcaBoundaryScore(points);

        return aspectFactor * boundaryScore;
    }

    /// <inheritdoc/>
    public AnnotationGeometry CreateGeometry(FreehandPath path)
    {
        if (path == null) { throw new ArgumentNullException(nameof(path)); }

        IReadOnlyList<PointF> points = path.Points;
        PointF center;
        float semiMajor, semiMinor, angleDegrees;

        // Prefer the algebraic fit (needs 5+ points); fall back to PCA.
        FittedEllipse? fit = points.Count >= 5 ? FreehandPathAnalyser.FitEllipse(points) : null;

        if (fit.HasValue)
        {
            center = fit.Value.Center;
            semiMajor = fit.Value.SemiMajor;
            semiMinor = fit.Value.SemiMinor;
            angleDegrees = fit.Value.AngleDegrees;
        }
        else if (points.Count < 2)
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
    // Scoring helpers
    // -----------------------------------------------------------------------

    private static float AlgebraicBoundaryScore(IReadOnlyList<PointF> points)
    {
        FittedEllipse? fit = FreehandPathAnalyser.FitEllipse(points);
        if (!fit.HasValue) { return PcaBoundaryScore(points); }
        return FreehandPathAnalyser.Clamp01(1f - fit.Value.MeanAlgebraicError / 0.3f);
    }

    private static float PcaBoundaryScore(IReadOnlyList<PointF> points)
    {
        (PointF center, float semiMajor, float semiMinor, float angleDeg) = ComputePcaEllipse(points);
        float meanError = MeanPcaEllipseBoundaryError(points, center, semiMajor, semiMinor, angleDeg);
        return FreehandPathAnalyser.Clamp01(1f - meanError / 0.3f);
    }

    // -----------------------------------------------------------------------
    // PCA ellipse helpers
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

        // Eigenvalues of the covariance matrix: λ = ((mxx+myy) ± sqrt((mxx-myy)²+4·mxy²)) / 2
        float sqrtDisc = MathF.Sqrt((mxx - myy) * (mxx - myy) + 4f * mxy * mxy);
        float lambda1 = (mxx + myy + sqrtDisc) / 2f; // larger → major axis
        float lambda2 = (mxx + myy - sqrtDisc) / 2f; // smaller → minor axis

        // Principal axis angle (of the major axis).
        float theta;
        if (MathF.Abs(mxy) < 1e-6f)
        {
            theta = mxx >= myy ? 0f : MathF.PI / 2f;
        }
        else
        {
            theta = MathF.Atan2(lambda1 - myy, mxy);
        }

        // For points uniformly sampled by angle on an ellipse, variance λ = semiAxis²/2,
        // so semiAxis = sqrt(2λ). This is more robust than using the maximum projection.
        float semiMajor = MathF.Max(1f, MathF.Sqrt(2f * MathF.Max(0f, lambda1)));
        float semiMinor = MathF.Max(1f, MathF.Sqrt(2f * MathF.Max(0f, lambda2)));

        if (semiMinor > semiMajor)
        {
            (semiMajor, semiMinor) = (semiMinor, semiMajor);
            theta += MathF.PI / 2f;
        }

        return (new PointF(cx, cy), semiMajor, semiMinor, theta * 180f / MathF.PI);
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
            float u = dx * cosA + dy * sinA;
            float v = -dx * sinA + dy * cosA;
            float norm = (u / semiMajor) * (u / semiMajor) + (v / semiMinor) * (v / semiMinor);
            total += MathF.Abs(norm - 1f);
        }

        return total / points.Count;
    }
}
