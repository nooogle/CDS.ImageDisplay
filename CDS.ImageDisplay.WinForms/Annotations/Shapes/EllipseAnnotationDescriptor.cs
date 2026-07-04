using System;
using System.Collections.Generic;
using System.Drawing;
using CDS.ImageDisplay.WinForms.Annotations.Internal;

namespace CDS.ImageDisplay.WinForms.Annotations.Shapes;

/// <summary>
/// Descriptor for ellipse annotations. Uses a direct algebraic fit when enough points are
/// available (5+), falling back to PCA only inside <see cref="CreateGeometry"/> for shorter paths.
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

        FittedEllipse? fit = FreehandPathAnalyser.FitEllipse(points);
        if (!fit.HasValue) { return 0f; }

        FittedEllipse e = fit.Value;

        // Must be noticeably elongated so the ellipse descriptor beats the circle descriptor.
        float ratio = e.SemiMajor / MathF.Max(e.SemiMinor, 0.01f);
        float aspectFactor = FreehandPathAnalyser.Clamp01((ratio - 1.3f) / 0.3f);
        if (aspectFactor <= 0f) { return 0f; }

        // How well the path points lie on the fitted ellipse boundary.
        float meanError = FreehandPathAnalyser.MeanRotatedEllipseBoundaryError(
            points, e.Center, e.SemiMajor, e.SemiMinor, e.AngleDegrees);
        float boundaryScore = FreehandPathAnalyser.Clamp01(1f - meanError / 0.4f);

        return aspectFactor * boundaryScore;
    }

    /// <inheritdoc/>
    public AnnotationGeometry CreateGeometry(FreehandPath path)
    {
        if (path == null) { throw new ArgumentNullException(nameof(path)); }

        IReadOnlyList<PointF> points = path.Points;
        PointF center;
        float semiMajor, semiMinor, angleDegrees;

        FittedEllipse? fit = FreehandPathAnalyser.FitEllipse(points);
        if (fit.HasValue)
        {
            center = fit.Value.Center;
            semiMajor = fit.Value.SemiMajor;
            semiMinor = fit.Value.SemiMinor;
            angleDegrees = fit.Value.AngleDegrees;
        }
        else if (points.Count >= 2)
        {
            (center, semiMajor, semiMinor, angleDegrees) = ComputePcaEllipse(points);
        }
        else
        {
            center = points.Count == 1 ? points[0] : PointF.Empty;
            semiMajor = 1f;
            semiMinor = 1f;
            angleDegrees = 0f;
        }

        var geometry = new EllipseAnnotationGeometry(center, semiMajor, semiMinor, angleDegrees);
        geometry.Drawing.Lines.Color = Color.CornflowerBlue;
        geometry.Drawing.Lines.Width = 2f;
        geometry.Drawing.Fill.Color = Color.Transparent;
        return geometry;
    }

    // -----------------------------------------------------------------------
    // PCA fallback — used only by CreateGeometry when the algebraic fit cannot run.
    // -----------------------------------------------------------------------

    private static (PointF center, float semiMajor, float semiMinor, float angleDegrees)
        ComputePcaEllipse(IReadOnlyList<PointF> points)
    {
        float cx = 0f, cy = 0f;
        foreach (PointF p in points) { cx += p.X; cy += p.Y; }
        cx /= points.Count;
        cy /= points.Count;

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

        float sqrtDisc = MathF.Sqrt((mxx - myy) * (mxx - myy) + 4f * mxy * mxy);
        float lambda1 = (mxx + myy + sqrtDisc) / 2f;
        float lambda2 = (mxx + myy - sqrtDisc) / 2f;

        float theta;
        if (MathF.Abs(mxy) < 1e-6f)
        {
            theta = mxx >= myy ? 0f : MathF.PI / 2f;
        }
        else
        {
            theta = MathF.Atan2(lambda1 - myy, mxy);
        }

        // semiAxis = sqrt(2λ): the correct relationship for boundary-sampled points.
        float semiMajor = MathF.Max(1f, MathF.Sqrt(2f * MathF.Max(0f, lambda1)));
        float semiMinor = MathF.Max(1f, MathF.Sqrt(2f * MathF.Max(0f, lambda2)));

        if (semiMinor > semiMajor)
        {
            (semiMajor, semiMinor) = (semiMinor, semiMajor);
            theta += MathF.PI / 2f;
        }

        return (new PointF(cx, cy), semiMajor, semiMinor, theta * 180f / MathF.PI);
    }
}
