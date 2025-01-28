using CDS.Imaging.BitmapDisplay;
using System.ComponentModel;
using System.Drawing;

namespace CDS.Imaging.Draw;

/// <summary>
/// A line overlay combining a line and rendering properties
/// </summary>
[TypeConverter(typeof(SerializableExpandableObjectConverter))]
public class LineShape : IShape
{
    /// <summary>
    /// Simple representation of this instance
    /// </summary>
    public override string ToString() => $"Line: form {Start} to {End}";


    /// <inheritdoc />
    public bool Visible { get; set; } = true;


    /// <inheritdoc/>
    public DisplayPixelAlign PixelAlign { get; set; } = DisplayPixelAlign.Centre;


    /// <summary>
    /// The start point of the line (in image coordinates)
    /// </summary>
    [TypeConverter(typeof(PointFConverter))]
    public PointF Start { get; set; }


    /// <summary>
    /// The end point of the line (in image coordinates)
    /// </summary>
    [TypeConverter(typeof(PointFConverter))]
    public PointF End { get; set; }


    /// <inheritdoc />
    public void Draw(BitmapDisplayPanel sender, Graphics graphics, RenderingSpec rendering)
    {
        if (!Visible || !rendering.Visible) { return; }

        var pen = RenderingToolsPool.GetPen(rendering.Lines);

        var startOnDisplay = sender.MapImageToDisplay(Start, pixelAdjust: PixelAlign);
        var endOnDisplay = sender.MapImageToDisplay(End, pixelAdjust: PixelAlign);

        graphics.DrawLine(pen, startOnDisplay, endOnDisplay);
    }
}

