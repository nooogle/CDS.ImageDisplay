using System.ComponentModel;
using System.Drawing;

namespace CDS.Imaging.Demo.DemoForms.OverlaysDemo;


[TypeConverter(typeof(ExpandableObjectConverter))]
class RenderingSettings
{
    public Draw.RenderingSpec Rectangles { get; set; } = new Draw.RenderingSpec()
    {
        Fill = new Draw.BrushSpec()
        {
            Color = Color.FromArgb(64, Color.Red),
        },

        Lines = new Draw.LineSpec()
        {
            Color = Color.Red,
            Width = 2,
            DashStyle = System.Drawing.Drawing2D.DashStyle.Dot,
        },
    };


    public Draw.RenderingSpec Lines { get; set; } = new Draw.RenderingSpec()
    {
        Lines = new Draw.LineSpec()
        {
            Color = Color.FromArgb(192, Color.Yellow),
            Width = 2,
            DashStyle = System.Drawing.Drawing2D.DashStyle.Solid,
        },
    };


    public Draw.RenderingSpec Circles { get; set; } = new Draw.RenderingSpec()
    {
        Fill = new Draw.BrushSpec()
        {
            Color = Color.FromArgb(32, Color.Yellow),
        },
        Lines = new Draw.LineSpec()
        {
            Color = Color.Yellow,
            Width = 2,
            DashStyle = System.Drawing.Drawing2D.DashStyle.Solid,
        },
    };


    public Draw.RenderingSpec Ellipses { get; set; } = new Draw.RenderingSpec()
    {
        Fill = new Draw.BrushSpec()
        {
            Color = Color.FromArgb(32, Color.Purple),
        },
        Lines = new Draw.LineSpec()
        {
            Color = Color.Purple,
            Width = 2,
            DashStyle = System.Drawing.Drawing2D.DashStyle.Solid,
        },
    };


    public Draw.RenderingSpec Text { get; set; } = new Draw.RenderingSpec()
    {
        Fill = new Draw.BrushSpec()
        {
            Color = Color.FromArgb(128, Color.WhiteSmoke),
        },
        Font = new Draw.FontSpec()
        {
            FontSize = 12,
        }
    };


    public Draw.RenderingSpec Polygons { get; set; } = new Draw.RenderingSpec()
    {
        Fill = new Draw.BrushSpec()
        {
            Color = Color.FromArgb(64, Color.LightCyan),
        },
        Lines = new Draw.LineSpec()
        {
            Color = Color.DarkCyan,
            Width = 2,
            DashStyle = System.Drawing.Drawing2D.DashStyle.Dot,
        },
    };


    public Draw.RenderingSpec CrossHair { get; set; } = new Draw.RenderingSpec()
    {
        Lines = new Draw.LineSpec()
        {
            Color = Color.FromArgb(128, Color.Sienna),
            Width = 1,
            DashStyle = System.Drawing.Drawing2D.DashStyle.Solid,
        },
    };
}
