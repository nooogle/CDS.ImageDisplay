using System;
using System.ComponentModel;
using System.Drawing;
using CDS.ImageDisplay.WinForms.BitmapDisplay;
using CDS.ImageDisplay.WinForms.Utils;


namespace CDS.ImageDisplay.WinForms.Overlays;

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
    public void Draw(ICoordinateMapper mapper, Graphics graphics, DrawingSpec drawing)
    {
        if (mapper == null) { throw new ArgumentNullException(nameof(mapper)); }
        if (graphics == null) { throw new ArgumentNullException(nameof(graphics)); }
        if (drawing == null) { throw new ArgumentNullException(nameof(drawing)); }
        if (!drawing.Visible)
        { return; }

        Pen pen = DrawingToolsPool.GetPen(drawing.Lines);
        Brush brush = DrawingToolsPool.GetBrush(drawing.Fill);

        RectangleF rectangleOnDisplay =
            drawing.MappingMode == MappingMode.ImageToDisplay ?
            mapper.MapRect(Rect, PixelAlign) :
            Rect;

        graphics.FillRectangle(brush, rectangleOnDisplay);
        graphics.DrawRectangle(pen, rectangleOnDisplay.X, rectangleOnDisplay.Y, rectangleOnDisplay.Width, rectangleOnDisplay.Height);
    }
}
