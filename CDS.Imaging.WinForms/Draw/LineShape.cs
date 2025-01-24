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


    /// <summary>
    /// The rendering properties of the rectangle
    /// </summary>
    public RenderingSpec Rendering { get; set; } = new RenderingSpec();


    /// <inheritdoc />
    public void Draw(BitmapDisplayPanel sender, Graphics graphics)
    {
        if (!Visible || !Rendering.Visible) { return; }

        var pen = RenderingToolsPool.GetPen(Rendering.Lines);

        var startOnDisplay = sender.MapImageToDisplay(Start, pixelAdjust: PixelAlign);
        var endOnDisplay = sender.MapImageToDisplay(End, pixelAdjust: PixelAlign);

        graphics.DrawLine(pen, startOnDisplay, endOnDisplay);
    }
}

