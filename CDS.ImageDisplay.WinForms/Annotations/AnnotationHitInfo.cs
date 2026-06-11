namespace CDS.ImageDisplay.WinForms.Annotations;

/// <summary>
/// Identifies what was hit during a hit-test on a selected annotation.
/// </summary>
public enum AnnotationHitKind
{
    /// <summary>No hit — the point is outside the annotation.</summary>
    None,

    /// <summary>The body of the annotation was hit (drag to move the whole shape).</summary>
    MoveBody,

    /// <summary>A resize or reposition handle was hit.</summary>
    Handle,
}

/// <summary>
/// Describes the result of a hit-test on a selected annotation.
/// </summary>
/// <param name="Kind">What was hit.</param>
/// <param name="HandleIndex">
/// Zero-based index of the handle when <see cref="Kind"/> is <see cref="AnnotationHitKind.Handle"/>; -1 otherwise.
/// </param>
public sealed record AnnotationHitInfo(AnnotationHitKind Kind, int HandleIndex)
{
    /// <summary>No hit — the point is outside the annotation.</summary>
    public static readonly AnnotationHitInfo Miss = new(AnnotationHitKind.None, -1);

    /// <summary>The body of the annotation was hit.</summary>
    public static readonly AnnotationHitInfo Move = new(AnnotationHitKind.MoveBody, -1);

    /// <summary>
    /// Creates a hit result for a specific resize handle.
    /// </summary>
    /// <param name="index">Zero-based handle index.</param>
    public static AnnotationHitInfo Handle(int index) => new(AnnotationHitKind.Handle, index);
}
