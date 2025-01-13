using System.Drawing;

namespace CDS.Imaging.WinForms.RegionOfInterest
{
    /// <summary>
    /// Event handler for when the ROI is committed to the ROISelectionOnBitmapDisplay
    /// </summary>
    public delegate void OnCommittedROIChangedEvent(ROISelectionOnBitmapDisplay sender, Rectangle roi);


    /// <summary>
    /// Event handler for when the ROI is being dragged on the ROISelectionOnBitmapDisplay
    /// </summary>
    public delegate void OnDraggingROIChangedEvent(ROISelectionOnBitmapDisplay sender, Rectangle roi);
}
