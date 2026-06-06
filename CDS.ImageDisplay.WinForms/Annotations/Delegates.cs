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
