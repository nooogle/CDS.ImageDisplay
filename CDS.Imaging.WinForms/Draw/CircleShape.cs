using CDS.Imaging.BitmapDisplay;
using System.ComponentModel;
using System.Drawing;

namespace CDS.Imaging.Draw;


/// <summary>
/// A circle overlay combining circle geometry and rendering properties
/// </summary>
[TypeConverter(typeof(SerializableExpandableObjectConverter))]
public class CircleShape
{
    /// <summary>
    /// Simple representation of this instance
    /// </summary>
    public override string ToString() => $"Circle: centre = {Centre}, radius = {Radius}";


    /// <summary>
    /// Controls how the circle centre is aligned to the pixel grid
    /// Only applicable when the mapping mode is set to <see cref="MappingMode.ImageToDisplay"/>.
    /// </summary>
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


    /// <inheritdoc />
    public void Draw(BitmapDisplayPanel sender, Graphics graphics, RenderingSpec rendering)
    {
        if (!rendering.Visible) { return; }

        var pen = RenderingToolsPool.GetPen(rendering.Lines);
        var brush = RenderingToolsPool.GetBrush(rendering.Fill);

        var squareAroundCircle = new RectangleF(Centre.X - Radius, Centre.Y - Radius, 2 * Radius, 2 * Radius);

        if (rendering.MappingMode == MappingMode.ImageToDisplay)
        {
            squareAroundCircle = sender.MapImageToDisplay(squareAroundCircle, pixelAdjust: PixelAlign);
        }

        graphics.FillEllipse(brush, squareAroundCircle);
        graphics.DrawEllipse(pen, squareAroundCircle);
    }
}
