using System.ComponentModel;
using System.Drawing;

namespace CDS.Imaging.Demo.DemoForms.OverlaysDemo;


[TypeConverter(typeof(ExpandableObjectConverter))]
class RenderingSettings
{
    public WinForms.Draw.RenderingSpec Rectangles { get; set; } = new WinForms.Draw.RenderingSpec()
    {
        Fill = new WinForms.Draw.BrushSpec()
        {
            Color = Color.FromArgb(64, Color.Red),
        },

        Lines = new WinForms.Draw.LineSpec()
        {
            Color = Color.Red,
            Width = 2,
            DashStyle = System.Drawing.Drawing2D.DashStyle.Dot,
        },
    };


    public WinForms.Draw.RenderingSpec Lines { get; set; } = new WinForms.Draw.RenderingSpec()
    {
        Lines = new WinForms.Draw.LineSpec()
        {
            Color = Color.FromArgb(192, Color.Yellow),
            Width = 2,
            DashStyle = System.Drawing.Drawing2D.DashStyle.Solid,
        },
    };


    public WinForms.Draw.RenderingSpec Circles { get; set; } = new WinForms.Draw.RenderingSpec()
    {
        Fill = new WinForms.Draw.BrushSpec()
        {
            Color = Color.FromArgb(32, Color.Yellow),
        },
        Lines = new WinForms.Draw.LineSpec()
        {
            Color = Color.Yellow,
            Width = 2,
            DashStyle = System.Drawing.Drawing2D.DashStyle.Solid,
        },
    };


    public WinForms.Draw.RenderingSpec Ellipses { get; set; } = new WinForms.Draw.RenderingSpec()
    {
        Fill = new WinForms.Draw.BrushSpec()
        {
            Color = Color.FromArgb(32, Color.Purple),
        },
        Lines = new WinForms.Draw.LineSpec()
        {
            Color = Color.Purple,
            Width = 2,
            DashStyle = System.Drawing.Drawing2D.DashStyle.Solid,
        },
    };


    public WinForms.Draw.RenderingSpec Text { get; set; } = new WinForms.Draw.RenderingSpec()
    {
        Fill = new WinForms.Draw.BrushSpec()
        {
            Color = Color.FromArgb(128, Color.WhiteSmoke),
        },
        Font = new WinForms.Draw.FontSpec()
        {
            FontSize = 12,
        }
    };


    public WinForms.Draw.RenderingSpec Polygons { get; set; } = new WinForms.Draw.RenderingSpec()
    {
        Fill = new WinForms.Draw.BrushSpec()
        {
            Color = Color.FromArgb(64, Color.LightCyan),
        },
        Lines = new WinForms.Draw.LineSpec()
        {
            Color = Color.DarkCyan,
            Width = 2,
            DashStyle = System.Drawing.Drawing2D.DashStyle.Dot,
        },
    };


    public WinForms.Draw.RenderingSpec CrossHair { get; set; } = new WinForms.Draw.RenderingSpec()
    {
        Lines = new WinForms.Draw.LineSpec()
        {
            Color = Color.FromArgb(128, Color.Sienna),
            Width = 1,
            DashStyle = System.Drawing.Drawing2D.DashStyle.Solid,
        },
    };
}
