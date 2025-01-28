using CDS.Imaging.BitmapDisplay;
using System.ComponentModel;
using System.Drawing;

namespace CDS.Imaging.Draw;

/// <summary>
/// A rectangle overlay combining a rectangle and rendering properties
/// </summary>
[TypeConverter(typeof(SerializableExpandableObjectConverter))]
public class RectangleShape : IShape
{
    /// <summary>
    /// Simple representation of this instance
    /// </summary>
    public override string ToString() => $"Rect: {Rect}";


    /// <inheritdoc />
    public bool Visible { get; set; } = true;


    /// <inheritdoc/>
    public DisplayPixelAlign PixelAlign { get; set; } = DisplayPixelAlign.TopLeft;


    /// <summary>
    /// The rectangle to draw (in image coordinates)
    /// </summary>
    [TypeConverter(typeof(RectangleFConverter))]
    public RectangleF Rect { get; set; }


    /// <inheritdoc />
    public void Draw(BitmapDisplayPanel sender, Graphics graphics, RenderingSpec rendering)
    {
        if (!Visible || !rendering.Visible) { return; }

        var pen = RenderingToolsPool.GetPen(rendering.Lines);
        var brush = RenderingToolsPool.GetBrush(rendering.Fill);

        var rectangleOnDisplay = sender.MapImageToDisplay(Rect, pixelAdjust: PixelAlign);

        graphics.FillRectangle(brush, rectangleOnDisplay);
        graphics.DrawRectangle(pen, rectangleOnDisplay);
    }
}

