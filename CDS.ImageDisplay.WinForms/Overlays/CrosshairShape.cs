using System;
using System.ComponentModel;
using System.Drawing;
using CDS.ImageDisplay.WinForms.BitmapDisplay;
using CDS.ImageDisplay.WinForms.Utils;


namespace CDS.ImageDisplay.WinForms.Overlays;


/// <summary>
/// A crosshair overlay combining crosshair geometry and drawing properties
/// </summary>
[TypeConverter(typeof(SerializableExpandableObjectConverter))]
public class CrosshairShape
{
    /// <summary>
    /// Simple representation of this instance
    /// </summary>
    public override string ToString() => $"Crosshair: centre = {Centre}";


    /// <summary>
    /// Pixel alignment of the crosshair
    /// Only applicable when the mapping mode is set to <see cref="MappingMode.ImageToDisplay"/>.
    /// </summary>
    public DisplayPixelAlign PixelAlign { get; set; } = DisplayPixelAlign.Centre;


    /// <summary>
    /// Centre of the crosshair
    /// </summary>
    [TypeConverter(typeof(PointFConverter))]
    public PointF Centre { get; set; }


    /// <summary>
    /// Length of the crosshair lines
    /// </summary>
    public float Length { get; set; } = 10;


    /// <summary>
    /// Gap between the centre and the crosshair lines
    /// </summary>
    public float CentreGap { get; set; } = 2;


    /// <summary>
    /// Draw the crosshair on the display
    /// </summary>
    public void Draw(BitmapDisplayPanel sender, Graphics graphics, DrawingSpec drawing)
    {
        ArgumentNullException.ThrowIfNull(sender);
        ArgumentNullException.ThrowIfNull(graphics);
        ArgumentNullException.ThrowIfNull(drawing);
        if (!drawing.Visible)
        { return; }

        Pen pen = DrawingToolsPool.GetPen(drawing.Lines);

        PointF centreOnDisplay =
            drawing.MappingMode == MappingMode.ImageToDisplay ?
            sender.MapImageToDisplay(Centre, PixelAlign) :
            Centre;

        // top line
        graphics.DrawLine(
            pen,
            centreOnDisplay.X,
            centreOnDisplay.Y - Length - CentreGap,
            centreOnDisplay.X,
            centreOnDisplay.Y - CentreGap);

        // bottom line
        graphics.DrawLine(
            pen,
            centreOnDisplay.X,
            centreOnDisplay.Y + CentreGap,
            centreOnDisplay.X,
            centreOnDisplay.Y + Length + CentreGap);

        // left line
        graphics.DrawLine(
            pen,
            centreOnDisplay.X - Length - CentreGap,
            centreOnDisplay.Y,
            centreOnDisplay.X - CentreGap,
            centreOnDisplay.Y);

        // right line
        graphics.DrawLine(
            pen,
            centreOnDisplay.X + CentreGap,
            centreOnDisplay.Y,
            centreOnDisplay.X + Length + CentreGap,
            centreOnDisplay.Y);
    }
}
