namespace CDS.Imaging.WinForms.RegionOfInterest
{
    /// <summary>
    /// The different dragging modes for the ROI manager.
    /// </summary>
    public enum ROIDragMode
    {
        /// <summary>
        /// No dragging mode.
        /// </summary>
        None,

        /// <summary>
        /// Dragging the whole region of interest (ROI).
        /// </summary>
        WholeROI,

        /// <summary>
        /// Dragging the top-left corner of the ROI.
        /// </summary>
        TopLeftCorner,

        /// <summary>
        /// Dragging the top-right corner of the ROI.
        /// </summary>
        TopRightCorner,

        /// <summary>
        /// Dragging the bottom-left corner of the ROI.
        /// </summary>
        BottomLeftCorner,

        /// <summary>
        /// Dragging the bottom-right corner of the ROI.
        /// </summary>
        BottomRightCorner,

        /// <summary>
        /// Dragging the top edge of the ROI.
        /// </summary>
        TopEdge,

        /// <summary>
        /// Dragging the bottom edge of the ROI.
        /// </summary>
        BottomEdge,

        /// <summary>
        /// Dragging the left edge of the ROI.
        /// </summary>
        LeftEdge,

        /// <summary>
        /// Dragging the right edge of the ROI.
        /// </summary>
        RightEdge
    }
}
