using System;
using System.ComponentModel;
using System.Drawing;
using CDS.ImageDisplay.BitmapDisplay;
using CDS.ImageDisplay.Utils;


namespace CDS.ImageDisplay.Overlays;


/// <summary>
/// A text shape
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
    public void Draw(BitmapDisplayPanel sender, Graphics graphics, DrawingSpec drawing)
    {
        ArgumentNullException.ThrowIfNull(sender, nameof(sender));
        ArgumentNullException.ThrowIfNull(graphics, nameof(graphics));
        ArgumentNullException.ThrowIfNull(drawing, nameof(drawing));
        if (!drawing.Visible) { return; }
        if(string.IsNullOrWhiteSpace(Text)) { return; }

        var font = DrawingToolsPool.GetFont(drawing.Font);
        var brush = DrawingToolsPool.GetBrush(drawing.Fill);

        var pointOnDisplay =
            drawing.MappingMode == MappingMode.ImageToDisplay ?
            sender.MapImageToDisplay(Location, PixelAlign) :
            Location;

        graphics.DrawString(Text, font, brush, pointOnDisplay);
    }
}
