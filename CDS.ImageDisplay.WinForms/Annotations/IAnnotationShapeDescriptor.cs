namespace CDS.ImageDisplay.WinForms.Annotations;

/// <summary>
/// Describes a shape type that can be created from a freehand drawing gesture.
/// Implement this interface to register custom shape types with an annotation manager.
/// </summary>
public interface IAnnotationShapeDescriptor
{
    /// <summary>
    /// The display name shown in the shape-selection popup menu.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// A short description shown as a tooltip in the shape-selection popup menu.
    /// </summary>
    string Description { get; }

    /// <summary>
    /// Returns a confidence score in the range 0..1 indicating how well the given freehand path
    /// matches this shape type. A score of 0 means impossible; 1 means a perfect match.
    /// </summary>
    /// <param name="path">The freehand path to evaluate.</param>
    float FitScore(FreehandPath path);

    /// <summary>
    /// Creates an <see cref="AnnotationGeometry"/> that best fits the given freehand path.
    /// </summary>
    /// <param name="path">The freehand path to fit.</param>
    AnnotationGeometry CreateGeometry(FreehandPath path);
}
