using System.ComponentModel;
using System.Drawing;

namespace CDS.Imaging.Demo.DemoForms.OverlaysDemo;


[TypeConverter(typeof(ExpandableObjectConverter))]
class DrawingSettings
{
    public WinForms.Draw.OverlayPainterSettings Rectangles { get; set; } = new WinForms.Draw.OverlayPainterSettings()
    {
        BrushColor = Color.FromArgb(64, Color.Red),
        PenColor = Color.Red,
        PenWidth = 2,
        PenDashStyle = System.Drawing.Drawing2D.DashStyle.Dot,
    };

    public WinForms.Draw.OverlayPainterSettings Lines { get; set; } = new WinForms.Draw.OverlayPainterSettings()
    {
        PenColor = Color.FromArgb(192, Color.Yellow),
        PenWidth = 2,
        PenDashStyle = System.Drawing.Drawing2D.DashStyle.Solid,
    };

    public WinForms.Draw.OverlayPainterSettings Circles { get; set; } = new WinForms.Draw.OverlayPainterSettings()
    {
        PenColor = Color.FromArgb(128, Color.Yellow),
        PenWidth = 0,
        BrushColor = Color.FromArgb(32, Color.Yellow),
    };

    public WinForms.Draw.OverlayPainterSettings Text { get; set; } = new WinForms.Draw.OverlayPainterSettings()
    {
        FontSize = 12,
        BrushColor = Color.WhiteSmoke,
    };

    public WinForms.Draw.OverlayPainterSettings Ellipses { get; set; } = new WinForms.Draw.OverlayPainterSettings()
    {
        BrushColor = Color.FromArgb(64, Color.Purple),
        PenColor = Color.Purple,
        PenWidth = 2,
        PenDashStyle = System.Drawing.Drawing2D.DashStyle.Solid,
    };


    public WinForms.Draw.OverlayPainterSettings Polygons { get; set; } = new WinForms.Draw.OverlayPainterSettings()
    {
        BrushColor = Color.FromArgb(64, Color.LightCyan),
        PenColor = Color.DarkCyan,
        PenWidth = 2,
        PenDashStyle = System.Drawing.Drawing2D.DashStyle.Dot,
    };


    public WinForms.Draw.OverlayPainterSettings CrossHair { get; set; } = new WinForms.Draw.OverlayPainterSettings()
    {
        PenColor = Color.FromArgb(128, Color.Sienna),
        PenWidth = 1,
        PenDashStyle = System.Drawing.Drawing2D.DashStyle.Solid,
    };
}
