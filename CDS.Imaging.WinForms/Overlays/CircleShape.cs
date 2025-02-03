using CDS.Imaging.BitmapDisplay;
using CDS.Imaging.Utils;
using System;
using System.ComponentModel;
using System.Drawing;

namespace CDS.Imaging.Overlays;


/// <summary>
/// A circle shape.
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


    /// <summary>
    /// Draws the circle.
    /// </summary>
    public void Draw(BitmapDisplayPanel sender, Graphics graphics, DrawingSpec drawing)
    {
        ArgumentNullException.ThrowIfNull(sender, nameof(sender));
        ArgumentNullException.ThrowIfNull(graphics, nameof(graphics));
        ArgumentNullException.ThrowIfNull(drawing, nameof(drawing));
        if (!drawing.Visible) { return; }
        if (Radius <= 0) { return; }

        var pen = DrawingToolsPool.GetPen(drawing.Lines);
        var brush = DrawingToolsPool.GetBrush(drawing.Fill);

        var squareAroundCircle = new RectangleF(Centre.X - Radius, Centre.Y - Radius, 2 * Radius, 2 * Radius);

        if (drawing.MappingMode == MappingMode.ImageToDisplay)
        {
            squareAroundCircle = sender.MapImageToDisplay(squareAroundCircle, pixelAdjust: PixelAlign);
        }

        graphics.FillEllipse(brush, squareAroundCircle);
        graphics.DrawEllipse(pen, squareAroundCircle);
    }
}
