using System;

namespace CDS.ImageDisplay.Annotations;

/// <summary>Event args for <see cref="AnnotationManager.AnnotationCreated"/>.</summary>
public sealed class AnnotationCreatedEventArgs(Annotation annotation) : EventArgs
{
    /// <summary>The annotation that was created.</summary>
    public Annotation Annotation { get; } = annotation;
}

/// <summary>Event args for <see cref="AnnotationManager.AnnotationModified"/>.</summary>
public sealed class AnnotationModifiedEventArgs(Annotation annotation) : EventArgs
{
    /// <summary>The annotation that was modified.</summary>
    public Annotation Annotation { get; } = annotation;
}

/// <summary>Event args for <see cref="AnnotationManager.AnnotationDeleted"/>.</summary>
public sealed class AnnotationDeletedEventArgs(Annotation annotation) : EventArgs
{
    /// <summary>The annotation that was deleted.</summary>
    public Annotation Annotation { get; } = annotation;
}

/// <summary>Event args for <see cref="AnnotationManager.AnnotationSelected"/>.</summary>
public sealed class AnnotationSelectedEventArgs(Annotation annotation) : EventArgs
{
    /// <summary>The annotation that was selected.</summary>
    public Annotation Annotation { get; } = annotation;
}

/// <summary>
/// Event args for <see cref="AnnotationManager.DragStarting"/>.
/// Carries a deep copy of the annotation geometry at the moment dragging begins,
/// suitable for use as an undo snapshot.
/// </summary>
public sealed class AnnotationDragStartingEventArgs : EventArgs
{
    /// <summary>The annotation whose geometry is about to be dragged.</summary>
    public Annotation Annotation { get; }

    /// <summary>
    /// A clone of the annotation's geometry captured immediately before the drag begins.
    /// </summary>
    public AnnotationGeometry GeometrySnapshot { get; }

    /// <summary>Initializes a new instance of <see cref="AnnotationDragStartingEventArgs"/>.</summary>
    public AnnotationDragStartingEventArgs(Annotation annotation, AnnotationGeometry geometrySnapshot)
    {
        Guard.ThrowIfNull(annotation, nameof(annotation));
        Guard.ThrowIfNull(geometrySnapshot, nameof(geometrySnapshot));
        Annotation = annotation;
        GeometrySnapshot = geometrySnapshot;
    }
}
