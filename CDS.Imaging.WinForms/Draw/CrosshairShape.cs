using CDS.Imaging.BitmapDisplay;
using System.ComponentModel;
using System.Drawing;

namespace CDS.Imaging.Draw;


/// <summary>
/// A crosshair overlay combining crosshair geometry and rendering properties
/// </summary>
[TypeConverter(typeof(SerializableExpandableObjectConverter))]
public class CrosshairShape : IShape
{
    /// <summary>
    /// Simple representation of this instance
    /// </summary>
    public override string ToString() => $"Crosshair: centre = {Centre}";


    /// <inheritdoc />
    public bool Visible { get; set; } = true;


    /// <inheritdoc/>
    public DisplayPixelAlign PixelAlign { get; set; } = DisplayPixelAlign.TopLeft;


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


    /// <inheritdoc />
    public void Draw(BitmapDisplayPanel sender, Graphics graphics, RenderingSpec rendering)
    {
        if (!Visible || !rendering.Visible) { return; }

        var pen = RenderingToolsPool.GetPen(rendering.Lines);

        var centreOnDisplay = sender.MapImageToDisplay(Centre, PixelAlign);

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
