using System.Drawing;

namespace CDS.ImageDisplay.RegionOfInterest
{
    /// <summary>
    /// Event handler for when a ROI is committed
    /// </summary>
    public delegate void OnCommittedROIChangedEvent(SingleROIManager sender, Rectangle roi);


    /// <summary>
    /// Event handler for when a ROI is being dragged
    /// </summary>
    public delegate void OnDraggingROIChangedEvent(SingleROIManager sender, Rectangle roi);
}
