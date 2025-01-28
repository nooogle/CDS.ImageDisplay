using CDS.Imaging.BitmapDisplay;
using System.ComponentModel;
using System.Drawing;

namespace CDS.Imaging.Draw;


/// <summary>
/// A polygon overlay combining polyon geometry and rendering properties
/// </summary>
[TypeConverter(typeof(SerializableExpandableObjectConverter))]
public class PolygonShape : IShape
{
    /// <summary>
    /// Simple representation of this instance
    /// </summary>
    public override string ToString() => $"Polygon: {Points.Length} points";


    /// <inheritdoc />
    public bool Visible { get; set; } = true;


    /// <inheritdoc/>
    public DisplayPixelAlign PixelAlign { get; set; } = DisplayPixelAlign.Centre;


    /// <summary>
    /// The points of the polygon
    /// </summary>
    [TypeConverter(typeof(PointFConverter))]
    public PointF[] Points { get; set; } = new PointF[0];


    /// <inheritdoc />
    public void Draw(BitmapDisplayPanel sender, Graphics graphics, RenderingSpec rendering)
    {
        if (!Visible || !rendering.Visible) { return; }

        var pen = RenderingToolsPool.GetPen(rendering.Lines);
        var brush = RenderingToolsPool.GetBrush(rendering.Fill);

        var pointsOnDisplay = new PointF[Points.Length];

        for (int i = 0; i < Points.Length; i++)
        {
            pointsOnDisplay[i] = sender.MapImageToDisplay(Points[i], pixelAdjust: PixelAlign);
        }

        graphics.FillPolygon(brush, pointsOnDisplay);
        graphics.DrawPolygon(pen, pointsOnDisplay);
    }
}

