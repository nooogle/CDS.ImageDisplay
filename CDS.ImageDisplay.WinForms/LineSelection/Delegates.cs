using System;
using System.Drawing;

namespace CDS.ImageDisplay.LineSelection;

/// <summary>
/// Provides data for the <see cref="SingleLineSelectionManager.OnCommittedLineChanged"/> event.
/// </summary>
public sealed class CommittedLineChangedEventArgs : EventArgs
{
    /// <summary>
    /// Gets the new committed line.
    /// </summary>
    public (Point Start, Point End) Line { get; }

    /// <summary>
    /// Initialises a new instance of <see cref="CommittedLineChangedEventArgs"/>.
    /// </summary>
    /// <param name="line">The new committed line.</param>
    public CommittedLineChangedEventArgs((Point Start, Point End) line)
    {
        Line = line;
    }
}

/// <summary>
/// Provides data for the <see cref="SingleLineSelectionManager.OnDraggingLineChanged"/> event.
/// </summary>
public sealed class DraggingLineChangedEventArgs : EventArgs
{
    /// <summary>
    /// Gets the current live-dragging line.
    /// </summary>
    public (Point Start, Point End) Line { get; }

    /// <summary>
    /// Initialises a new instance of <see cref="DraggingLineChangedEventArgs"/>.
    /// </summary>
    /// <param name="line">The current live-dragging line.</param>
    public DraggingLineChangedEventArgs((Point Start, Point End) line)
    {
        Line = line;
    }
}
