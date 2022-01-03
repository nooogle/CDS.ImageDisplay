namespace CDS.Imaging.WinForms.BitmapDisplay
{
    /// <summary>
    /// Bitmap display modes
    /// </summary>
    public enum BitmapDisplayMode
    {
        /// <summary>
        /// Centre the image and make as large as possible within the bounds of the display
        /// </summary>
        FitToWindowCentred,


        /// <summary>
        /// Centre the image and set to the zoom to 1
        /// </summary>
        ActualSizeCentred,


        /// <summary>
        /// Allow the image zoom and location to change, either programmatically or
        /// via the mouse. The <see cref="BitmapDisplayPanel.CDSTargetImageCentre"/> is reset
        /// if the image size changes.
        /// </summary>
        Free,
    }
}
