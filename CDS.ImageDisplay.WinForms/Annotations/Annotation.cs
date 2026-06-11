using System;
using System.Text.Json.Serialization;

namespace CDS.ImageDisplay.WinForms.Annotations;

/// <summary>
/// An annotation on an image, combining a geometric shape with descriptive metadata.
/// Subclass this to attach domain-specific metadata such as label categories or confidence scores.
/// </summary>
[JsonPolymorphic(TypeDiscriminatorPropertyName = "$type")]
[JsonDerivedType(typeof(Annotation), "annotation")]
public class Annotation
{
    /// <summary>
    /// Unique identifier for this annotation. Assigned automatically at construction
    /// and preserved by <see cref="Clone"/>.
    /// </summary>
    public Guid Id { get; init; } = Guid.NewGuid();

    /// <summary>
    /// Display title for this annotation.
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Free-text notes for this annotation.
    /// </summary>
    public string Notes { get; set; } = string.Empty;

    /// <summary>
    /// Machine-readable class label for this annotation (e.g. <c>"car"</c>, <c>"pedestrian"</c>).
    /// Used by consuming apps when exporting to YOLO, COCO, Pascal VOC, or any other format
    /// that requires a category name. Distinct from <see cref="Title"/>, which is free-form
    /// display text.
    /// </summary>
    public string Label { get; set; } = string.Empty;

    /// <summary>
    /// The geometry of this annotation.
    /// </summary>
    public AnnotationGeometry Geometry
    {
        get;
        set
        {
            ArgumentNullException.ThrowIfNull(value);
            field = value;
        }
    }

    /// <summary>
    /// Initializes a new <see cref="Annotation"/> with the given geometry and a new unique identifier.
    /// </summary>
    /// <param name="geometry">The annotation geometry.</param>
    [JsonConstructor]
    public Annotation(AnnotationGeometry geometry)
    {
        ArgumentNullException.ThrowIfNull(geometry);
        Geometry = geometry;
    }

    /// <summary>
    /// Returns a deep copy of this annotation: the geometry is cloned, and
    /// <see cref="Id"/>, <see cref="Title"/>, and <see cref="Notes"/> are copied.
    /// </summary>
    public virtual Annotation Clone()
    {
        return new Annotation(Geometry.Clone())
        {
            Id = Id,
            Title = Title,
            Notes = Notes,
            Label = Label,
        };
    }
}
