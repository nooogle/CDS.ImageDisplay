using System;
using System.ComponentModel;
using System.Drawing;
using CDS.ImageDisplay.BitmapDisplay;
using CDS.ImageDisplay.Utils;


namespace CDS.ImageDisplay.Overlays;


/// <summary>
/// An ellipse overlay combining ellipse geometry and drawing properties
/// </summary>
[TypeConverter(typeof(SerializableExpandableObjectConverter))]
public class EllipseShape
{
    /// <summary>
    /// Simple representation of this instance
    /// </summary>
    public override string ToString() => $"Ellipse: centre = {Centre}, major = {MajorAxis}, minor = {MinorAxis}, angle = {MajorAxisAngleDegrees}";


    /// <summary>
    /// Controls how the centre of the ellipse is aligned to the pixel grid
    /// Only applicable when the mapping mode is set to <see cref="MappingMode.ImageToDisplay"/>.
    /// </summary>
    public DisplayPixelAlign PixelAlign { get; set; } = DisplayPixelAlign.Centre;


    /// <summary>
    /// The centre of the ellipse (in image coordinates)
    /// </summary>
    [TypeConverter(typeof(PointFConverter))]
    public PointF Centre { get; set; }


    /// <summary>
    /// The major axis of the ellipse (in image coordinates)
    /// </summary>
    public float MajorAxis { get; set; }


    /// <summary>
    /// The minor axis of the ellipse (in image coordinates)
    /// </summary>
    public float MinorAxis { get; set; }


    /// <summary>
    /// The angle of the major axis (in degrees)
    /// </summary>
    public float MajorAxisAngleDegrees { get; set; }


    /// <summary>
    /// Draws the shape
    /// </summary>
    public void Draw(BitmapDisplayPanel sender, Graphics graphics, DrawingSpec drawing)
    {
        ArgumentNullException.ThrowIfNull(sender, nameof(sender));
        ArgumentNullException.ThrowIfNull(graphics, nameof(graphics));
        ArgumentNullException.ThrowIfNull(drawing, nameof(drawing));
        if (!drawing.Visible) { return; }

        var pen = DrawingToolsPool.GetPen(drawing.Lines);
        var brush = DrawingToolsPool.GetBrush(drawing.Fill);

        // Save the current state of the Graphics object
        var state = graphics.Save();

        try
        {
            var centreOnDisplay =
                drawing.MappingMode == MappingMode.ImageToDisplay ?
                sender.MapImageToDisplay(Centre, PixelAlign) :
                Centre;
            
            var majorAxisOnDisplay =
                drawing.MappingMode == MappingMode.ImageToDisplay ?
                sender.MapImageToDisplay(MajorAxis) :
                MajorAxis;

            var minorAxisOnDisplay = 
                drawing.MappingMode == MappingMode.ImageToDisplay ? 
                sender.MapImageToDisplay(MinorAxis) : 
                MinorAxis;

            // Translate to the center of the ellipse
            graphics.TranslateTransform(centreOnDisplay.X, centreOnDisplay.Y);

            // Rotate by the specified angle
            graphics.RotateTransform(MajorAxisAngleDegrees);

            // Draw the ellipse (centered at the origin after translation)
            graphics.FillEllipse(brush, -majorAxisOnDisplay / 2, -minorAxisOnDisplay / 2, majorAxisOnDisplay, minorAxisOnDisplay);
            graphics.DrawEllipse(pen, -majorAxisOnDisplay / 2, -minorAxisOnDisplay / 2, majorAxisOnDisplay, minorAxisOnDisplay);
        }
        finally
        {
            // Restore the original state
            graphics.Restore(state);
        }
    }
}
