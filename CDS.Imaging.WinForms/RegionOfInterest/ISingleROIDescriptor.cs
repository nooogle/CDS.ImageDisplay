using System;
using System.Drawing;

namespace CDS.Imaging.WinForms.RegionOfInterest
{
    /// <summary>
    /// Interface for a descriptor for a single ROI, The <see cref="MultipleROIManager"/> uses these
    /// to manage multiple ROIs on a bitmap display.
    /// </summary>
    public interface ISingleROIDescriptor : IDisposable
    {
        /// <summary>
        /// True if the ROI is visible.
        /// </summary>
        bool Visible { get; set; }


        /// <summary>
        /// True if the ROI is locked and should be editable.
        /// </summary>
        bool Locked { get; set; } 


        /// <summary>
        /// Maximum size of the ROI.
        /// </summary>
        Size MaximumSize { get; set; }


        /// <summary>
        /// Minimum size of the ROI.
        /// </summary>
        Size MinimumSize { get; set; }


        /// <summary>
        /// Name of the ROI.
        /// </summary>
        string Name { get; set; }


        /// <summary>
        /// Renderer for the ROI.
        /// </summary>
        RectangleRenderer Renderer { get; }


        /// <summary>
        /// The region of interest (ROI).
        /// </summary>
        Rectangle ROI { get; set; }


        /// <summary>
        /// Draws the ROI on the bitmap display.
        /// </summary>
        /// <param name="graphics">A graphics instance</param>
        /// <param name="bitmapDisplay">A bitmap display control</param>
        /// <param name="roiOnImage">The ROI, in image (not display) coordinates</param>
        void Draw(Graphics graphics, BitmapDisplay.BitmapDisplayPanel bitmapDisplay, Rectangle roiOnImage);
    }
}
