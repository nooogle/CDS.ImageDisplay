using System;
using System.ComponentModel;
using System.Drawing;
using CDS.ImageDisplay.WinForms.BitmapDisplay;
using CDS.ImageDisplay.WinForms.Utils;


namespace CDS.ImageDisplay.WinForms.Overlays;

/// <summary>
/// A line shape
/// </summary>
[TypeConverter(typeof(SerializableExpandableObjectConverter))]
public class LineShape
{
    /// <summary>
    /// Simple representation of this instance
    /// </summary>
    public override string ToString() => $"Line: form {Start} to {End}";


    /// <summary>
    /// Controls how the end points are aligned to the display pixels
    /// Only applicable when the mapping mode is set to <see cref="MappingMode.ImageToDisplay"/>.
    /// </summary>
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
    /// Draws the line on the display
    /// </summary>
    public void Draw(BitmapDisplayPanel sender, Graphics graphics, DrawingSpec drawingSpec)
    {
        ArgumentNullException.ThrowIfNull(sender);
        ArgumentNullException.ThrowIfNull(graphics);
        ArgumentNullException.ThrowIfNull(drawingSpec);
        if (!drawingSpec.Visible)
        { return; }

        Pen pen = DrawingToolsPool.GetPen(drawingSpec.Lines);

        PointF startOnDisplay =
            drawingSpec.MappingMode == MappingMode.ImageToDisplay ?
            sender.MapImageToDisplay(Start, pixelAdjust: PixelAlign) :
            Start;

        PointF endOnDisplay =
            drawingSpec.MappingMode == MappingMode.ImageToDisplay ?
            sender.MapImageToDisplay(End, pixelAdjust: PixelAlign) :
            End;

        graphics.DrawLine(pen, startOnDisplay, endOnDisplay);
    }
}
