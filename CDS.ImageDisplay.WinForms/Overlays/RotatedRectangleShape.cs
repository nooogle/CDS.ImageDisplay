using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using CDS.ImageDisplay.WinForms.BitmapDisplay;
using CDS.ImageDisplay.WinForms.Utils;

namespace CDS.ImageDisplay.WinForms.Overlays;

/// <summary>
/// A rotated rectangle overlay shape. Geometry matches the OpenCV <c>RotatedRect</c> convention:
/// <see cref="Centre"/> is the rectangle's centre in image coordinates, <see cref="Width"/> and
/// <see cref="Height"/> are the side lengths, and <see cref="Angle"/> is the clockwise rotation
/// in degrees.
/// </summary>
[TypeConverter(typeof(SerializableExpandableObjectConverter))]
public class RotatedRectangleShape
{
    /// <summary>Simple representation of this instance.</summary>
    public override string ToString() => $"RotatedRect: centre={Centre}, {Width}x{Height}, angle={Angle}°";

    /// <summary>
    /// Controls how the centre of the rectangle is aligned to the pixel grid.
    /// Only applicable when the mapping mode is <see cref="MappingMode.ImageToDisplay"/>.
    /// </summary>
    public DisplayPixelAlign PixelAlign { get; set; } = DisplayPixelAlign.Centre;

    /// <summary>The centre of the rectangle in image coordinates.</summary>
    [TypeConverter(typeof(PointFConverter))]
    public PointF Centre { get; set; }

    /// <summary>Width of the rectangle in image coordinates.</summary>
    public float Width { get; set; }

    /// <summary>Height of the rectangle in image coordinates.</summary>
    public float Height { get; set; }

    /// <summary>Clockwise rotation angle in degrees.</summary>
    public float Angle { get; set; }

    /// <summary>Draws the rotated rectangle on the display.</summary>
    public void Draw(ICoordinateMapper mapper, Graphics graphics, DrawingSpec drawing)
    {
        if (mapper == null) { throw new ArgumentNullException(nameof(mapper)); }
        if (graphics == null) { throw new ArgumentNullException(nameof(graphics)); }
        if (drawing == null) { throw new ArgumentNullException(nameof(drawing)); }

        if (!drawing.Visible) { return; }

        Pen pen = DrawingToolsPool.GetPen(drawing.Lines);
        Brush brush = DrawingToolsPool.GetBrush(drawing.Fill);

        PointF centreDisplay =
            drawing.MappingMode == MappingMode.ImageToDisplay
            ? mapper.MapPoint(Centre, PixelAlign)
            : Centre;

        float widthDisplay =
            drawing.MappingMode == MappingMode.ImageToDisplay
            ? mapper.MapDistance(Width)
            : Width;

        float heightDisplay =
            drawing.MappingMode == MappingMode.ImageToDisplay
            ? mapper.MapDistance(Height)
            : Height;

        GraphicsState state = graphics.Save();
        try
        {
            graphics.TranslateTransform(centreDisplay.X, centreDisplay.Y);
            graphics.RotateTransform(Angle);

            var rect = new RectangleF(-widthDisplay / 2f, -heightDisplay / 2f, widthDisplay, heightDisplay);
            graphics.FillRectangle(brush, rect);
            graphics.DrawRectangle(pen, rect.X, rect.Y, rect.Width, rect.Height);
        }
        finally
        {
            graphics.Restore(state);
        }
    }
}
