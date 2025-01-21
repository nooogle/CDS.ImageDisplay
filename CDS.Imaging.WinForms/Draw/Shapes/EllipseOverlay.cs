using CDS.Imaging.WinForms.BitmapDisplay;
using System.ComponentModel;
using System.Drawing;

namespace CDS.Imaging.WinForms.Draw.Shapes;


/// <summary>
/// An ellipse overlay combining ellipse geometry and rendering properties
/// </summary>
[TypeConverter(typeof(ExpandableObjectConverter))]
public class EllipseOverlay : IShapeOverlay
{
    /// <summary>
    /// Simple representation of this instance
    /// </summary>
    public override string ToString() => $"Ellipse: centre = {Centre}, major = {MajorAxis}, minor = {MinorAxis}, angle = {MajorAxisAngleDegrees}";


    /// <inheritdoc />
    public bool Visible { get; set; } = true;


    /// <inheritdoc/>
    public DisplayPixelAlign PixelAlign { get; set; } = DisplayPixelAlign.Centre;


    /// <summary>
    /// The centre of the ellipse (in image coordinates)
    /// </summary>
    [TypeConverter(typeof(WinForms.Draw.PointFConverter))]
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
    /// The rendering properties of the rectangle
    /// </summary>
    public RenderingSpec Rendering { get; set; } = new RenderingSpec();


    /// <inheritdoc />
    public void Draw(BitmapDisplayPanel sender, Graphics graphics)
    {
        if (!Visible || !Rendering.Visible) { return; }

        var pen = RenderingToolsPool.GetPen(Rendering.Lines);
        var brush = RenderingToolsPool.GetBrush(Rendering.Fill);

        // Save the current state of the Graphics object
        var state = graphics.Save();

        try
        {
            // Translate the ellipse from image to display coordinates
            var centreOnDisplay = sender.MapImageToDisplay(Centre, PixelAlign);
            var majorAxisOnDisplay = sender.MapImageToDisplay(MajorAxis);
            var minorAxisOnDisplay = sender.MapImageToDisplay(MinorAxis);

            // Translate to the center of the ellipse
            graphics.TranslateTransform(centreOnDisplay.X, centreOnDisplay.Y);

            // Rotate by the specified angle
            graphics.RotateTransform(MajorAxisAngleDegrees);

            // Draw the ellipse (centered at the origin after translation)
            graphics.FillEllipse(brush, (-majorAxisOnDisplay / 2), (-minorAxisOnDisplay / 2), majorAxisOnDisplay, minorAxisOnDisplay);
            graphics.DrawEllipse(pen, (-majorAxisOnDisplay / 2), (-minorAxisOnDisplay) / 2, majorAxisOnDisplay, minorAxisOnDisplay);
        }
        finally
        {
            // Restore the original state
            graphics.Restore(state);
        }
    }
}
