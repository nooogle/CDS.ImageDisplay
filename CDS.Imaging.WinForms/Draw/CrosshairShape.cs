using CDS.Imaging.BitmapDisplay;
using System.ComponentModel;
using System.Drawing;

namespace CDS.Imaging.Draw;


/// <summary>
/// A crosshair overlay combining crosshair geometry and rendering properties
/// </summary>
[TypeConverter(typeof(ExpandableObjectConverter))]
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


    /// <summary>
    /// The rendering properties of the rectangle
    /// </summary>
    public RenderingSpec Rendering { get; set; } = new RenderingSpec();


    /// <inheritdoc />
    public void Draw(BitmapDisplayPanel sender, Graphics graphics)
    {
        if (!Visible || !Rendering.Visible) { return; }

        var pen = RenderingToolsPool.GetPen(Rendering.Lines);

        var centreOnDisplay = sender.MapImageToDisplay(Centre, PixelAlign);
        var lineLengthOnDisplay = sender.MapImageToDisplay(Length);
        var centreGapOnDisplay = sender.MapImageToDisplay(CentreGap);

        // top line
        graphics.DrawLine(
            pen,
            centreOnDisplay.X,
            centreOnDisplay.Y - lineLengthOnDisplay - centreGapOnDisplay,
            centreOnDisplay.X,
            centreOnDisplay.Y - centreGapOnDisplay);

        // bottom line
        graphics.DrawLine(
            pen,
            centreOnDisplay.X,
            centreOnDisplay.Y + centreGapOnDisplay,
            centreOnDisplay.X,
            centreOnDisplay.Y + lineLengthOnDisplay + centreGapOnDisplay);

        // left line
        graphics.DrawLine(
            pen,
            centreOnDisplay.X - lineLengthOnDisplay - centreGapOnDisplay,
            centreOnDisplay.Y,
            centreOnDisplay.X - centreGapOnDisplay,
            centreOnDisplay.Y);

        // right line
        graphics.DrawLine(
            pen,
            centreOnDisplay.X + centreGapOnDisplay,
            centreOnDisplay.Y,
            centreOnDisplay.X + lineLengthOnDisplay + centreGapOnDisplay,
            centreOnDisplay.Y);
    }
}
