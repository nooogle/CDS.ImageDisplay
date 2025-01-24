using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;


namespace CDS.Imaging.Draw
{
    /// <summary>
    /// Represents the specification of a pen.
    /// </summary>
    [TypeConverter(typeof(SerializableExpandableObjectConverter))]
    public class LineSpec
    {
        /// <summary>
        /// The color of the pen.
        /// </summary>
        public Color Color { get; set; } = Color.Red;


        /// <summary>
        /// The width of the pen.
        /// </summary>
        public float Width { get; set; } = 1;


        /// <summary>
        /// The dash style of the pen.
        /// </summary>
        public DashStyle DashStyle { get; set; } = DashStyle.Solid;


        /// <summary>
        /// The start cap of the pen.
        /// </summary>
        public LineCap StartCap { get; set; } = LineCap.Flat;


        /// <summary>
        /// The end cap of the pen.
        /// </summary>
        public LineCap EndCap { get; set; } = LineCap.Flat;


        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        public override int GetHashCode()
        {
#if NET8_0_OR_GREATER
            return HashCode.Combine(Color, Width, DashStyle, StartCap, EndCap);
#else
            unchecked
            {
                int hash = 17;
                hash = hash * 23 + Color.GetHashCode();
                hash = hash * 23 + Width.GetHashCode();
                hash = hash * 23 + DashStyle.GetHashCode();
                hash = hash * 23 + StartCap.GetHashCode();
                hash = hash * 23 + EndCap.GetHashCode();
                return hash;
            }
#endif
        }


        /// <summary>
        /// Determines whether the specified object is equal to the current object.
        /// </summary>
        public override bool Equals(object? obj)
        {
            if (obj is not LineSpec other) return false;
            return Color == other.Color &&
                   Width == other.Width &&
                   DashStyle == other.DashStyle &&
                   StartCap == other.StartCap &&
                   EndCap == other.EndCap;
        }
    }
}
