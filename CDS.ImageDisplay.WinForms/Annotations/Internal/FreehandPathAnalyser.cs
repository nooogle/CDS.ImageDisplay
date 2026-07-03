using System;
using System.Collections.Generic;
using System.Drawing;

namespace CDS.ImageDisplay.WinForms.Annotations.Internal;

/// <summary>
/// Geometric analysis helpers used by shape descriptors to score freehand gesture paths.
/// </summary>
internal static class FreehandPathAnalyser
{
    /// <summary>
    /// Fits an ellipse to the given points using direct algebraic least squares with an
    /// A+C=1 normalization constraint. Requires at least 5 points; 7 or more
    /// are recommended for a stable result.
    /// </summary>
    /// <remarks>
    /// The algorithm solves a 5×5 reduced normal-equation system that arises from substituting
    /// C = 1−A into the general conic design matrix, then extracts geometric parameters from
    /// the conic coefficients. Points are translated and scaled internally for numerical stability.
    /// </remarks>
    /// <returns>
    /// The fitted ellipse, or <see langword="null"/> when: fewer than 5 points are supplied,
    /// the normal equations are singular, or the best-fit conic is not an ellipse.
    /// </returns>
    internal static FittedEllipse? FitEllipse(IReadOnlyList<PointF> points)
    {
        if (points.Count < 5) { return null; }

        // --- 1. Normalize for numerical stability: translate to centroid, scale to unit RMS distance.
        float cx = 0f, cy = 0f;
        foreach (PointF p in points) { cx += p.X; cy += p.Y; }
        cx /= points.Count;
        cy /= points.Count;

        float rmsScale = 0f;
        foreach (PointF p in points)
        {
            float dx = p.X - cx, dy = p.Y - cy;
            rmsScale += dx * dx + dy * dy;
        }
        rmsScale = MathF.Sqrt(rmsScale / points.Count);
        if (rmsScale < 0.001f) { return null; }

        // --- 2. Build AtA (5×5) and Atb (5×1) for the A+C=1 reduced system.
        // With C = 1-A, each design-matrix row becomes [u²-v², uv, u, v, 1] and RHS = -v².
        double[] ata = new double[25];
        double[] atb = new double[5];

        foreach (PointF p in points)
        {
            double u = (p.X - cx) / rmsScale;
            double v = (p.Y - cy) / rmsScale;
            double u2 = u * u, v2 = v * v;

            double[] row = [u2 - v2, u * v, u, v, 1.0];
            double rhs = -v2;

            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 5; j++) { ata[i * 5 + j] += row[i] * row[j]; }
                atb[i] += row[i] * rhs;
            }
        }

        // --- 3. Solve the 5×5 normal equations.
        double[]? sol = SolveLinear5(ata, atb);
        if (sol == null) { return null; }

        double A = sol[0], B = sol[1], C = 1.0 - A, D = sol[2], E = sol[3], F = sol[4];

        // --- 4. Reject if not an ellipse: requires B²-4AC < 0 (implies A,C same sign).
        double disc = B * B - 4.0 * A * C;
        if (disc >= 0.0) { return null; }

        // --- 5. Solve [[2A, B],[B, 2C]] * [h, k] = [-D, -E] for the centre.
        double det2 = 4.0 * A * C - B * B; // > 0 since disc < 0
        double h = (B * E - 2.0 * C * D) / det2;
        double k = (B * D - 2.0 * A * E) / det2;

        // --- 6. Value of the conic at the centre; must be negative for a real ellipse.
        double f0 = A * h * h + B * h * k + C * k * k + D * h + E * k + F;
        if (f0 >= 0.0) { return null; }

        // --- 7. Eigenvalues of [[A, B/2],[B/2, C]]; both positive since A+C=1 and disc<0.
        // λ = ((A+C) ± sqrt((A-C)² + B²)) / 2
        double delta = Math.Sqrt((A - C) * (A - C) + B * B);
        double lam1 = (A + C + delta) / 2.0; // larger → semi-minor
        double lam2 = (A + C - delta) / 2.0; // smaller → semi-major

        if (lam2 <= 0.0) { return null; }

        // --- 8. Semi-axes: a² = -f0/lam (major uses lam2, minor uses lam1).
        double semiMajorNorm = Math.Sqrt(-f0 / lam2);
        double semiMinorNorm = Math.Sqrt(-f0 / lam1);

        // --- 9. Angle of the major axis (eigenvector of the smaller eigenvalue lam2).
        // Direction vector of that eigenvector: [B/2, lam2-A].
        // In screen coords (y-down) this atan2 gives the clockwise angle from +X.
        double ex = B / 2.0;
        double ey = lam2 - A;
        double angleDeg;
        if (ex * ex + ey * ey < 1e-20)
        {
            angleDeg = 0.0; // circle or numerically degenerate — angle is arbitrary
        }
        else
        {
            angleDeg = Math.Atan2(ey, ex) * 180.0 / Math.PI;
            // Normalize to (-90°, 90°] — an axis has 180° symmetry.
            while (angleDeg > 90.0) { angleDeg -= 180.0; }
            while (angleDeg <= -90.0) { angleDeg += 180.0; }
        }

        // --- 10. Denormalize back to original coordinate space.
        float centreX = cx + (float)h * rmsScale;
        float centreY = cy + (float)k * rmsScale;
        float semiMajor = (float)semiMajorNorm * rmsScale;
        float semiMinor = (float)semiMinorNorm * rmsScale;

        if (semiMajor < semiMinor)
        {
            (semiMajor, semiMinor) = (semiMinor, semiMajor);
            angleDeg = angleDeg > 0.0 ? angleDeg - 90.0 : angleDeg + 90.0;
        }

        // --- 11. Mean algebraic residual on normalized points (lower = better fit).
        float error = AlgebraicResidual(points, cx, cy, rmsScale, A, B, C, D, E, F);

        return new FittedEllipse
        {
            Center = new PointF(centreX, centreY),
            SemiMajor = MathF.Max(1f, semiMajor),
            SemiMinor = MathF.Max(1f, semiMinor),
            AngleDegrees = (float)angleDeg,
            MeanAlgebraicError = error,
        };
    }

    /// <summary>
    /// Computes the mean absolute value of Ax²+Bxy+Cy²+Dx+Ey+F for the given points,
    /// expressed in the normalized coordinate frame.
    /// </summary>
    private static float AlgebraicResidual(
        IReadOnlyList<PointF> points, float cx, float cy, float scale,
        double A, double B, double C, double D, double E, double F)
    {
        float total = 0f;
        foreach (PointF p in points)
        {
            double u = (p.X - cx) / scale;
            double v = (p.Y - cy) / scale;
            double val = A * u * u + B * u * v + C * v * v + D * u + E * v + F;
            total += MathF.Abs((float)val);
        }
        return total / points.Count;
    }

    /// <summary>
    /// Solves the 5×5 linear system <c>a·x = b</c> using Gaussian elimination with partial
    /// pivoting. Returns <see langword="null"/> if the matrix is singular.
    /// </summary>
    private static double[]? SolveLinear5(double[] a, double[] b)
    {
        const int n = 5;

        // Augmented matrix [a | b], row-major.
        double[] aug = new double[n * (n + 1)];
        for (int i = 0; i < n; i++)
        {
            for (int j = 0; j < n; j++) { aug[i * (n + 1) + j] = a[i * n + j]; }
            aug[i * (n + 1) + n] = b[i];
        }

        for (int col = 0; col < n; col++)
        {
            // Partial pivot.
            int maxRow = col;
            double maxAbs = Math.Abs(aug[col * (n + 1) + col]);
            for (int row = col + 1; row < n; row++)
            {
                double abs = Math.Abs(aug[row * (n + 1) + col]);
                if (abs > maxAbs) { maxAbs = abs; maxRow = row; }
            }

            if (maxAbs < 1e-12) { return null; }

            if (maxRow != col)
            {
                for (int j = col; j <= n; j++)
                {
                    (aug[col * (n + 1) + j], aug[maxRow * (n + 1) + j]) =
                        (aug[maxRow * (n + 1) + j], aug[col * (n + 1) + j]);
                }
            }

            // Eliminate below.
            double pivot = aug[col * (n + 1) + col];
            for (int row = col + 1; row < n; row++)
            {
                double factor = aug[row * (n + 1) + col] / pivot;
                for (int j = col; j <= n; j++)
                {
                    aug[row * (n + 1) + j] -= factor * aug[col * (n + 1) + j];
                }
            }
        }

        // Back substitution.
        double[] x = new double[n];
        for (int i = n - 1; i >= 0; i--)
        {
            x[i] = aug[i * (n + 1) + n];
            for (int j = i + 1; j < n; j++) { x[i] -= aug[i * (n + 1) + j] * x[j]; }
            x[i] /= aug[i * (n + 1) + i];
        }

        return x;
    }


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
