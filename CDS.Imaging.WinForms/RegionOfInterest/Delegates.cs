using System.Drawing;

namespace CDS.Imaging.WinForms.RegionOfInterest
{
    /// <summary>
    /// Event handler for when the ROI is committed to the ROISelectionOnBitmapDisplay
    /// </summary>
    public delegate void OnCommittedROIChangedEvent(SingleROIManager sender, Rectangle roi);


    /// <summary>
    /// Event handler for when the ROI is being dragged on the ROISelectionOnBitmapDisplay
    /// </summary>
    public delegate void OnDraggingROIChangedEvent(SingleROIManager sender, Rectangle roi);


    /// <summary>
    /// Event handler for when the ROI on the multiple ROI manager is clicked
    /// </summary>
    public delegate void OnROIClickedEvent(MultipleROIManager sender, SingleROIDescriptor roiDescriptor);
}
