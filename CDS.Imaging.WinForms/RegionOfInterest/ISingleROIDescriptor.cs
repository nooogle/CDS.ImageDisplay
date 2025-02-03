using CDS.Imaging.BitmapDisplay;
using CDS.Imaging.Overlays;
using System.Drawing;

namespace CDS.Imaging.RegionOfInterest;

/// <summary>
/// Interface for a descriptor for a single ROI, The <see cref="MultipleROIManager"/> uses these
/// to manage multiple ROIs on a bitmap display.
/// </summary>
public interface ISingleROIDescriptor
{
    /// <summary>
    /// The region of interest.
    /// </summary>        
    public Rectangle ROI { get; set; }


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
    /// True if the ROI is visible.
    /// </summary>
    bool Visible { get; set; }


    /// <summary>
    /// Draws the ROI on the bitmap display.
    /// </summary>
    void Draw(BitmapDisplayPanel bitmapDisplayPanel, Graphics graphics);
}
