using System.ComponentModel;

namespace CDS.Imaging.Draw
{
    /// <summary>
    /// Represents the specification of how shapes or information should be drawn.
    /// </summary>
    [TypeConverter(typeof(SerializableExpandableObjectConverter))]
    public class RenderingSpec
    {
        /// <summary>
        /// Simple representation of the rendering specification.
        /// </summary>
        public override string ToString() => "";


        /// <summary>
        /// True if the shape should be visible.
        /// </summary>
        public bool Visible { get; set; } = true;


        /// <summary>
        /// Line specification.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public LineSpec Lines { get; set; } = new LineSpec();


        /// <summary>
        /// Fill specification.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public BrushSpec Fill { get; set; } = new BrushSpec();


        /// <summary>
        /// Font specification.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public FontSpec Font { get; set; } = new FontSpec();
    }
}
