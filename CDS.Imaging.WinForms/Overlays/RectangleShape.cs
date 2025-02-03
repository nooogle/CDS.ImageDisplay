using CDS.Imaging.BitmapDisplay;
using CDS.Imaging.Utils;
using System;
using System.ComponentModel;
using System.Drawing;

namespace CDS.Imaging.Overlays;

/// <summary>
/// A rectangle shape
/// </summary>
[TypeConverter(typeof(SerializableExpandableObjectConverter))]
public class RectangleShape
{
    /// <summary>
    /// Simple representation of this instance
    /// </summary>
    public override string ToString() => $"Rect: {Rect}";


    /// <summary>
    /// Controls how the corners of the rectangle are aligned to the display pixels
    /// Only applicable when the mapping mode is set to <see cref="MappingMode.ImageToDisplay"/>.
    /// </summary>
    public DisplayPixelAlign PixelAlign { get; set; } = DisplayPixelAlign.TopLeft;


    /// <summary>
    /// The rectangle to draw (in image coordinates)
    /// </summary>
    [TypeConverter(typeof(RectangleFConverter))]
    public RectangleF Rect { get; set; }


    /// <summary>
    /// Draws the rectangle on the display
    /// </summary>
    public void Draw(BitmapDisplayPanel sender, Graphics graphics, DrawingSpec drawing)
    {
        ArgumentNullException.ThrowIfNull(sender, nameof(sender));
        ArgumentNullException.ThrowIfNull(graphics, nameof(graphics));
        ArgumentNullException.ThrowIfNull(drawing, nameof(drawing));
        if (!drawing.Visible) { return; }

        var pen = DrawingToolsPool.GetPen(drawing.Lines);
        var brush = DrawingToolsPool.GetBrush(drawing.Fill);

        Rectangle rectangleOnDisplay = 
            drawing.MappingMode == MappingMode.ImageToDisplay ?
            sender.MapImageToDisplay(Rect, pixelAdjust: PixelAlign) :
            Rectangle.Truncate(Rect);

        graphics.FillRectangle(brush, rectangleOnDisplay);
        graphics.DrawRectangle(pen, rectangleOnDisplay);
    }
}

