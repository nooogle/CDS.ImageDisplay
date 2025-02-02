using CDS.Imaging.BitmapDisplay;
using CDS.Imaging.Utils;
using System;
using System.ComponentModel;
using System.Drawing;

namespace CDS.Imaging.Draw;

/// <summary>
/// A line overlay combining a line and rendering properties
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
    public void Draw(BitmapDisplayPanel sender, Graphics graphics, RenderingSpec rendering)
    {
        ArgumentNullException.ThrowIfNull(sender, nameof(sender));
        ArgumentNullException.ThrowIfNull(graphics, nameof(graphics));
        ArgumentNullException.ThrowIfNull(rendering, nameof(rendering));
        if (!rendering.Visible) { return; }

        var pen = RenderingToolsPool.GetPen(rendering.Lines);

        var startOnDisplay = 
            rendering.MappingMode == MappingMode.ImageToDisplay ? 
            sender.MapImageToDisplay(Start, pixelAdjust: PixelAlign) : 
            Start;
        
        var endOnDisplay = 
            rendering.MappingMode == MappingMode.ImageToDisplay ? 
            sender.MapImageToDisplay(End, pixelAdjust: PixelAlign) : 
            End;

        graphics.DrawLine(pen, startOnDisplay, endOnDisplay);
    }
}

