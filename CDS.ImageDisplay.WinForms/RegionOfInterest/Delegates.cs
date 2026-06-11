using System;
using System.Drawing;

namespace CDS.ImageDisplay.WinForms.RegionOfInterest;

/// <summary>
/// Provides data for the <see cref="SingleROIManager.CommittedROIChanged"/> event.
/// </summary>
public sealed class CommittedROIChangedEventArgs : EventArgs
{
    /// <summary>Gets the new committed region of interest.</summary>
    public Rectangle ROI { get; }

    /// <summary>Initialises a new instance of <see cref="CommittedROIChangedEventArgs"/>.</summary>
    /// <param name="roi">The new committed region of interest.</param>
    public CommittedROIChangedEventArgs(Rectangle roi)
    {
        ROI = roi;
    }
}

/// <summary>
/// Provides data for the <see cref="SingleROIManager.DraggingROIChanged"/> event.
/// </summary>
public sealed class DraggingROIChangedEventArgs : EventArgs
{
    /// <summary>Gets the current live-dragging region of interest.</summary>
    public Rectangle ROI { get; }

    /// <summary>Initialises a new instance of <see cref="DraggingROIChangedEventArgs"/>.</summary>
    /// <param name="roi">The current live-dragging region of interest.</param>
    public DraggingROIChangedEventArgs(Rectangle roi)
    {
        ROI = roi;
    }
}
