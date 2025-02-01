using CDS.Imaging.BitmapDisplay;
using System.ComponentModel;
using System.Drawing;

namespace CDS.Imaging.Draw;


/// <summary>
/// A text overlay combining text information and geometry and rendering properties
/// </summary>
[TypeConverter(typeof(SerializableExpandableObjectConverter))]
public class TextShape
{
    /// <summary>
    /// Simple representation of this instance
    /// </summary>
    public override string ToString() => $"Text: location = {Location}, Text = {Text}";


    /// <summary>
    /// Controls how the text is aligned with the pixel grid. 
    /// Only applicable when the mapping mode is set to <see cref="MappingMode.ImageToDisplay"/>.
    /// </summary>
    public DisplayPixelAlign PixelAlign { get; set; } = DisplayPixelAlign.TopLeft;


    /// <summary>
    /// The text to display
    /// </summary>
    public string Text { get; set; } = "";


    /// <summary>
    /// The location of the text (in image coordinates)
    /// </summary>
    [TypeConverter(typeof(PointFConverter))]
    public PointF Location { get; set; }


    /// <summary>
    /// Draws the text on the display
    /// </summary>
    public void Draw(BitmapDisplayPanel sender, Graphics graphics, RenderingSpec rendering)
    {
        if (!rendering.Visible) { return; }

        var font = RenderingToolsPool.GetFont(rendering.Font);
        var brush = RenderingToolsPool.GetBrush(rendering.Fill);

        var pointOnDisplay =
            rendering.MappingMode == MappingMode.ImageToDisplay ?
            sender.MapImageToDisplay(Location, PixelAlign) :
            Location;

        graphics.DrawString(Text, font, brush, pointOnDisplay);
    }
}
