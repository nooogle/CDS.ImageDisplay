using System;
using System.ComponentModel;
using System.Drawing;
using System.Text.Json.Serialization;
using CDS.ImageDisplay.WinForms.BitmapDisplay;
using CDS.ImageDisplay.WinForms.Utils;


namespace CDS.ImageDisplay.WinForms.Overlays;


/// <summary>
/// A polygon shape
/// </summary>
[TypeConverter(typeof(SerializableExpandableObjectConverter))]
public class PolygonShape
{
    [JsonPropertyName("Points")]
    private PointF[] _points = [];


    /// <summary>
    /// Simple representation of this instance
    /// </summary>
    public override string ToString() => $"Polygon: {_points.Length} points";



    /// <summary>
    /// Controls how the polygon is aligned to the display pixels
    /// Only applicable when the mapping mode is set to <see cref="MappingMode.ImageToDisplay"/>.
    /// </summary>
    public DisplayPixelAlign PixelAlign { get; set; } = DisplayPixelAlign.Centre;



    /// <summary>
    /// Takes a copy of the provided points to use as the points of the polygon
    /// </summary>
    public void SetPoints(PointF[] points)
    {
        if (points == null) { throw new ArgumentNullException(nameof(points)); }
        _points = [.. points];
    }


    /// <summary>
    /// Draws the polygon on the display
    /// </summary>
    public void Draw(ICoordinateMapper mapper, Graphics graphics, DrawingSpec drawing)
    {
        if (mapper == null) { throw new ArgumentNullException(nameof(mapper)); }
        if (graphics == null) { throw new ArgumentNullException(nameof(graphics)); }
        if (drawing == null) { throw new ArgumentNullException(nameof(drawing)); }
        if ((_points.Length < 3) || !drawing.Visible)
        {
            return;
        }

        Pen pen = DrawingToolsPool.GetPen(drawing.Lines);
        Brush brush = DrawingToolsPool.GetBrush(drawing.Fill);

        PointF[] pointsOnDisplay;

        if (drawing.MappingMode == MappingMode.DirectToDisplay)
        {
            pointsOnDisplay = _points;
        }
        else
        {
            pointsOnDisplay = new PointF[_points.Length];
            for (int i = 0; i < _points.Length; i++)
            {
                pointsOnDisplay[i] = mapper.MapPoint(_points[i], PixelAlign);
            }
        }

        graphics.FillPolygon(brush, pointsOnDisplay);
        graphics.DrawPolygon(pen, pointsOnDisplay);
    }
}
