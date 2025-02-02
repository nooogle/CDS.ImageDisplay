using CDS.Imaging.Utils;
using System.ComponentModel;
using System.Drawing;

namespace CDS.Imaging.Draw
{
    /// <summary>
    /// Represents a brush specification.
    /// </summary>
    [TypeConverter(typeof(SerializableExpandableObjectConverter))]
    public class BrushSpec
    {
        /// <summary>
        /// The color of the brush.
        /// </summary>
        public Color Color { get; set; } = Color.Transparent;


        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        public override int GetHashCode()
        {
            return Color.GetHashCode();
        }


        /// <summary>
        /// Determines whether the specified object is equal to the current object.
        /// </summary>
        public override bool Equals(object? obj)
        {
            if (obj is not BrushSpec other) return false;
            return Color == other.Color;
        }
    }
}
