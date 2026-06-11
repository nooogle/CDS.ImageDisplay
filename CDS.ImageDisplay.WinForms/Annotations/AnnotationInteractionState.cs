namespace CDS.ImageDisplay.WinForms.Annotations;

/// <summary>
/// Tracks the current mouse-interaction state of <see cref="AnnotationManager"/>.
/// </summary>
internal enum AnnotationInteractionState
{
    /// <summary>No interaction in progress.</summary>
    Idle,

    /// <summary>Left button held; freehand path is accumulating.</summary>
    Drawing,

    /// <summary>Drag gesture completed; waiting for the shape menu selection.</summary>
    MenuOpen,

    /// <summary>An annotation is selected and showing handles.</summary>
    Selected,

    /// <summary>A selected annotation or handle is being dragged.</summary>
    Dragging,
}
