using CDS.Imaging.BitmapDisplay;
using System.ComponentModel;
using System.Drawing;

namespace CDS.Imaging.Draw;


/// <summary>
/// A circle overlay combining circle geometry and rendering properties
/// </summary>
[TypeConverter(typeof(SerializableExpandableObjectConverter))]
public class CircleShape : IShape
{
    /// <summary>
    /// Simple representation of this instance
    /// </summary>
    public override string ToString() => $"Circle: centre = {Centre}, radius = {Radius}";


    /// <inheritdoc />
    public bool Visible { get; set; } = true;


    /// <inheritdoc/>
    public DisplayPixelAlign PixelAlign { get; set; } = DisplayPixelAlign.Centre;


    /// <summary>
    /// The centre of the circle (in image coordinates)
    /// </summary>
    [TypeConverter(typeof(PointFConverter))]
    public PointF Centre { get; set; }


    /// <summary>
    /// The radius of the circle (in image coordinates)
    /// </summary>
    public float Radius { get; set; }


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

        var squareAroundCircle = new RectangleF(Centre.X - Radius, Centre.Y - Radius, 2 * Radius, 2 * Radius);
        var circleOnDisplay = sender.MapImageToDisplay(squareAroundCircle, pixelAdjust: PixelAlign);

        graphics.FillEllipse(brush, circleOnDisplay);
        graphics.DrawEllipse(pen, circleOnDisplay);
    }
}
