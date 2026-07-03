using System.Drawing;

namespace CDS.ImageDisplay.WinForms.Annotations.Internal;

/// <summary>
/// Result of a direct algebraic ellipse fit to a set of points.
/// </summary>
internal readonly struct FittedEllipse
{
    /// <summary>Centre of the ellipse in the same coordinate space as the fitted points.</summary>
    internal PointF Center { get; init; }

    /// <summary>Semi-major axis length (always &gt;= <see cref="SemiMinor"/>).</summary>
    internal float SemiMajor { get; init; }

    /// <summary>Semi-minor axis length.</summary>
    internal float SemiMinor { get; init; }

    /// <summary>
    /// Clockwise angle of the major axis in degrees from the positive X axis.
    /// Matches the <c>AngleDegrees</c> convention of
    /// <see cref="CDS.ImageDisplay.WinForms.Annotations.Shapes.EllipseAnnotationGeometry"/>.
    /// </summary>
    internal float AngleDegrees { get; init; }

    /// <summary>
    /// Mean absolute algebraic residual across the fitted points (Bookstein-normalised conic).
    /// Zero indicates a perfect fit; values above ~0.3 suggest a poor match.
    /// </summary>
    internal float MeanAlgebraicError { get; init; }
}
