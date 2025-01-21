using CDS.Imaging.WinForms.BitmapDisplay;
using System.ComponentModel;
using System.Drawing;

namespace CDS.Imaging.WinForms.Draw.Shapes;

/// <summary>
/// A rectangle overlay combining a rectangle and rendering properties
/// </summary>
[TypeConverter(typeof(ExpandableObjectConverter))]
public class RectangleOverlay : IShapeOverlay
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


    /// <summary>
    /// The rendering properties of the rectangle
    /// </summary>
    public RenderingSpec Rendering { get; set; } = new RenderingSpec();


    /// <inheritdoc />
    public void Draw(BitmapDisplayPanel sender, Graphics graphics)
    {
        if (!Visible || !Rendering.Visible) { return; }

        var pen = RenderingToolsPool.GetPen(Rendering.Lines);
        var brush = RenderingToolsPool.GetBrush(Rendering.Fill);

        var rectangleOnDisplay = sender.MapImageToDisplay(Rect, pixelAdjust: PixelAlign);

        graphics.FillRectangle(brush, rectangleOnDisplay);
        graphics.DrawRectangle(pen, rectangleOnDisplay);
    }
}

