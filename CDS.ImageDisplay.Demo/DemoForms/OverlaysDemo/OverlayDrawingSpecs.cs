using System.ComponentModel;
using System.Drawing;
using CDS.ImageDisplay.Overlays;
using CDS.ImageDisplay.Utils;

namespace CDS.ImageDisplay.Demo.DemoForms.OverlaysDemo;


[TypeConverter(typeof(SerializableExpandableObjectConverter))]
public class OverlayDrawingSpecs
{
    public DrawingSpec Rectangles { get; set; } = new DrawingSpec();
    public DrawingSpec Ellipses { get; set; } = new DrawingSpec();
    public DrawingSpec Lines { get; set; } = new DrawingSpec();
    public DrawingSpec Text { get; set; } = new DrawingSpec();
    public DrawingSpec Circles { get; set; } = new DrawingSpec();
    public DrawingSpec Polygons { get; set; } = new DrawingSpec();
    public DrawingSpec CrossHair { get; set; } = new DrawingSpec();
    public DrawingSpec Bubbles { get; set; } = new DrawingSpec();



    public OverlayDrawingSpecs()
    {
        Rectangles.Fill.Color = Color.FromArgb(64, Color.Red);
        Rectangles.Lines.Color = Color.Red;
        Rectangles.Lines.Width = 2;
        Rectangles.Lines.DashStyle = System.Drawing.Drawing2D.DashStyle.Dot;

        Ellipses.Fill.Color = Color.FromArgb(64, Color.Purple);
        Ellipses.Lines.Color = Color.Purple;
        Ellipses.Lines.Width = 2;
        Ellipses.Lines.DashStyle = System.Drawing.Drawing2D.DashStyle.Solid;

        Lines.Lines.Color = Color.FromArgb(128, Color.Orange);
        Lines.Lines.Width = 3;
        Lines.Lines.DashStyle = System.Drawing.Drawing2D.DashStyle.Solid;

        Text.Fill.Color = Color.Yellow;
        Text.Font.FontSize = 12;

        Circles.Fill.Color = Color.FromArgb(64, Color.Orange);
        Circles.Lines.Color = Color.Orange;
        Circles.Lines.Width = 2;
        Circles.Lines.DashStyle = System.Drawing.Drawing2D.DashStyle.Solid;

        Polygons.Fill.Color = Color.FromArgb(64, Color.LightCyan);
        Polygons.Lines.Color = Color.DarkCyan;
        Polygons.Lines.Width = 2;
        Polygons.Lines.DashStyle = System.Drawing.Drawing2D.DashStyle.Dot;
        Polygons.Lines.Width = 2;

        CrossHair.Lines.Color = Color.FromArgb(128, Color.Sienna);
        CrossHair.Lines.Width = 1;
        CrossHair.Lines.DashStyle = System.Drawing.Drawing2D.DashStyle.Solid;

        Bubbles.Lines.Color = Color.FromArgb(128, Color.Navy);
        Bubbles.Fill.Color = Color.FromArgb(64, Color.Navy);
        Bubbles.MappingMode = MappingMode.DirectToDisplay;
    }
}
