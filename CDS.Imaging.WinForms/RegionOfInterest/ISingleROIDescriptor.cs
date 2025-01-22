using System.Drawing;

namespace CDS.Imaging.RegionOfInterest
{
    /// <summary>
    /// Interface for a descriptor for a single ROI, The <see cref="MultipleROIManager"/> uses these
    /// to manage multiple ROIs on a bitmap display.
    /// </summary>

    /* Unmerged change from project 'CDS.Imaging.WinForms (net48)'
    Before:
        public interface ISingleROIDescriptor : Draw.Shapes.IShape
        {
    After:
        public interface ISingleROIDescriptor : IShape
        {
    */
    public interface ISingleROIDescriptor : Draw.IShape
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
    }
}
