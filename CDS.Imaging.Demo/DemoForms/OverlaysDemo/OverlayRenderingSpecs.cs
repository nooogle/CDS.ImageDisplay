using CDS.Imaging.Draw;
using System.ComponentModel;
using System.Drawing;

namespace CDS.Imaging.Demo.DemoForms.OverlaysDemo;


[TypeConverter(typeof(SerializableExpandableObjectConverter))]
class OverlayRenderingSpecs
{
    public RenderingSpec Rectangles { get; set; } = new RenderingSpec();
    public RenderingSpec Ellipses { get; set; } = new RenderingSpec();
    public RenderingSpec Lines { get; set; } = new RenderingSpec();
    public RenderingSpec Text { get; set; } = new RenderingSpec();
    public RenderingSpec Circles { get; set; } = new RenderingSpec();
    public RenderingSpec Polygons { get; set; } = new RenderingSpec();
    public RenderingSpec CrossHair { get; set; } = new RenderingSpec();
    public RenderingSpec Bubbles { get; set; } = new RenderingSpec();



    public OverlayRenderingSpecs()
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
