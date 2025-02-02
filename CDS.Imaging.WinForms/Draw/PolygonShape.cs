using CDS.Imaging.BitmapDisplay;
using CDS.Imaging.Utils;
using System;
using System.ComponentModel;
using System.Drawing;

namespace CDS.Imaging.Draw;


/// <summary>
/// A polygon overlay combining polyon geometry and rendering properties
/// </summary>
[TypeConverter(typeof(SerializableExpandableObjectConverter))]
public class PolygonShape
{
    /// <summary>
    /// Simple representation of this instance
    /// </summary>
    public override string ToString() => $"Polygon: {Points.Length} points";



    /// <summary>
    /// Controls how the polygon is aligned to the display pixels
    /// Only applicable when the mapping mode is set to <see cref="MappingMode.ImageToDisplay"/>.
    /// </summary>
    public DisplayPixelAlign PixelAlign { get; set; } = DisplayPixelAlign.Centre;


    /// <summary>
    /// The points of the polygon
    /// </summary>
    [TypeConverter(typeof(PointFConverter))]
    public PointF[] Points { get; set; } = new PointF[0];


    /// <summary>
    /// Draws the polygon on the display
    /// </summary>
    public void Draw(BitmapDisplayPanel sender, Graphics graphics, RenderingSpec rendering)
    {
        ArgumentNullException.ThrowIfNull(sender, nameof(sender));
        ArgumentNullException.ThrowIfNull(graphics, nameof(graphics));
        ArgumentNullException.ThrowIfNull(rendering, nameof(rendering));
        if(Points == null || Points.Length < 3) { return; }
        if (!rendering.Visible) { return; }

        var pen = RenderingToolsPool.GetPen(rendering.Lines);
        var brush = RenderingToolsPool.GetBrush(rendering.Fill);

        PointF[] pointsOnDisplay;

        if (rendering.MappingMode == MappingMode.DirectToDisplay)
        {
            pointsOnDisplay = Points;
        }
        else
        {
            pointsOnDisplay = new PointF[Points.Length];
            for (int i = 0; i < Points.Length; i++)
            {
                pointsOnDisplay[i] = sender.MapImageToDisplay(Points[i], pixelAdjust: PixelAlign);
            }
        }

        graphics.FillPolygon(brush, pointsOnDisplay);
        graphics.DrawPolygon(pen, pointsOnDisplay);
    }
}

