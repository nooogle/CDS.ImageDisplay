using System;

namespace CDS.ImageDisplay.RegionOfInterest;

/// <summary>
/// Event arguments for when a ROI has been committed.
/// </summary>
public class CommittedROIDescriptorChangedEventArgs : EventArgs
{
    /// <summary>
    /// The ROI that has been committed.
    /// </summary>
    public ISingleROIDescriptor ROIDescriptor { get; }


    /// <summary>
    /// Constructor
    /// </summary>
    public CommittedROIDescriptorChangedEventArgs(ISingleROIDescriptor roiDescriptor)
    {
        ROIDescriptor = roiDescriptor;
    }
}
