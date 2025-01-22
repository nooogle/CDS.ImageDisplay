using CDS.Imaging.BitmapDisplay;
using System.ComponentModel;
using System.Drawing;

namespace CDS.Imaging.Draw;


/// <summary>
/// A polygon overlay combining polyon geometry and rendering properties
/// </summary>
[TypeConverter(typeof(ExpandableObjectConverter))]
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

        var pointsOnDisplay = new PointF[Points.Length];

        for (int i = 0; i < Points.Length; i++)
        {
            pointsOnDisplay[i] = sender.MapImageToDisplay(Points[i], pixelAdjust: PixelAlign);
        }

        graphics.FillPolygon(brush, pointsOnDisplay);
        graphics.DrawPolygon(pen, pointsOnDisplay);
    }
}

