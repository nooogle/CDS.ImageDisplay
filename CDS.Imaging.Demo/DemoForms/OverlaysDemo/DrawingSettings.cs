using System.ComponentModel;
using System.Drawing;

namespace CDS.Imaging.Demo.DemoForms.OverlaysDemo;


[TypeConverter(typeof(ExpandableObjectConverter))]
class DrawingSettings
{
    public WinForms.Draw.OverlaySettings Rectangles { get; set; } = new WinForms.Draw.OverlaySettings()
    {
        BrushColor = Color.FromArgb(64, Color.Red),
        PenColor = Color.Red,
        PenWidth = 2,
        PenDashStyle = System.Drawing.Drawing2D.DashStyle.Dot,
    };

    public WinForms.Draw.OverlaySettings Lines { get; set; } = new WinForms.Draw.OverlaySettings()
    {
        PenColor = Color.FromArgb(192, Color.Yellow),
        PenWidth = 2,
        PenDashStyle = System.Drawing.Drawing2D.DashStyle.Solid,
    };

    public WinForms.Draw.OverlaySettings Circles { get; set; } = new WinForms.Draw.OverlaySettings()
    {
        PenColor = Color.FromArgb(128, Color.Yellow),
        PenWidth = 0,
        BrushColor = Color.FromArgb(32, Color.Yellow),
    };

    public WinForms.Draw.OverlaySettings Text { get; set; } = new WinForms.Draw.OverlaySettings()
    {
        FontSize = 12,
        BrushColor = Color.WhiteSmoke,
    };

    public WinForms.Draw.OverlaySettings Ellipses { get; set; } = new WinForms.Draw.OverlaySettings()
    {
        BrushColor = Color.FromArgb(64, Color.Purple),
        PenColor = Color.Purple,
        PenWidth = 2,
        PenDashStyle = System.Drawing.Drawing2D.DashStyle.Solid,
    };
}
