using System.ComponentModel;

namespace CDS.Imaging.Draw
{
    /// <summary>
    /// Represents a font specification.
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class FontSpec
    {
        /// <summary>
        /// Gets or sets the font size.
        /// </summary>
        /// <value>The size of the font.</value>
        public int FontSize { get; set; } = 12;


        /// <summary>
        /// Gets or sets the font name.
        /// </summary>
        /// <value>The name of the font.</value>
        public string FontName { get; set; } = "Arial";


        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString() => $"Font {FontName}, Size {FontSize}";


        /// <summary>
        /// Determines whether the specified object is equal to the current object.
        /// </summary>
        override public bool Equals(object? obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }
            var other = (FontSpec)obj;
            return FontSize == other.FontSize && FontName == other.FontName;
        }


        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        override public int GetHashCode()
        {
            return FontSize.GetHashCode() ^ FontName.GetHashCode();
        }
    }
}
