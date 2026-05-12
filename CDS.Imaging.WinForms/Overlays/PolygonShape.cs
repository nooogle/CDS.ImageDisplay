using System;
using System.ComponentModel;
using System.Drawing;
using CDS.Imaging.BitmapDisplay;
using CDS.Imaging.Utils;


namespace CDS.Imaging.Overlays;


/// <summary>
/// A polygon shape
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
    public PointF[] Points { get; set; } = [];


    /// <summary>
    /// Draws the polygon on the display
    /// </summary>
    public void Draw(BitmapDisplayPanel sender, Graphics graphics, DrawingSpec drawing)
    {
        ArgumentNullException.ThrowIfNull(sender, nameof(sender));
        ArgumentNullException.ThrowIfNull(graphics, nameof(graphics));
        ArgumentNullException.ThrowIfNull(drawing, nameof(drawing));
        if (Points.Length < 3) { return; }
        if (!drawing.Visible) { return; }

        var pen = DrawingToolsPool.GetPen(drawing.Lines);
        var brush = DrawingToolsPool.GetBrush(drawing.Fill);

        PointF[] pointsOnDisplay;

        if (drawing.MappingMode == MappingMode.DirectToDisplay)
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
