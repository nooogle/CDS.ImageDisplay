using CDS.Imaging.BitmapDisplay;
using System.ComponentModel;
using System.Drawing;

namespace CDS.Imaging.Draw;


/// <summary>
/// A text overlay combining text information and geometry and rendering properties
/// </summary>
[TypeConverter(typeof(SerializableExpandableObjectConverter))]
public class TextShape : IShape
{
    /// <summary>
    /// Simple representation of this instance
    /// </summary>
    public override string ToString() => $"Text: location = {Location}, Text = {Text}";


    /// <inheritdoc />
    public bool Visible { get; set; } = true;


    /// <inheritdoc/>
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
    /// The rendering properties of the text
    /// </summary>
    public RenderingSpec Rendering { get; set; } = new RenderingSpec();


    /// <inheritdoc />
    public void Draw(BitmapDisplayPanel sender, Graphics graphics)
    {
        if (!Visible || !Rendering.Visible) { return; }

        var font = RenderingToolsPool.GetFont(Rendering.Font);
        var brush = RenderingToolsPool.GetBrush(Rendering.Fill);

        var pointOnDisplay = sender.MapImageToDisplay(Location, PixelAlign);

        graphics.DrawString(Text, font, brush, pointOnDisplay);
    }
}
