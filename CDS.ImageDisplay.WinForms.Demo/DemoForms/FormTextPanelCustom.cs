using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace CDS.ImageDisplay.WinForms.Demo.DemoForms;


/// <summary>
/// Demonstration that shows what the bitmap display panel looks like when an image has not been assigned.
/// </summary>
internal sealed partial class FormTextPanelCustom : Form
{
    enum CustomLogLevel
    {
        Trace,
        Debug,
        Info,
        Warning,
        Error,
        Critical
    }

    private Overlays.TextPanel<CustomLogLevel> _customLogLevelTextPanel;
    private Overlays.DrawingSpec _customDrawingSpecPanel;
    private Dictionary<CustomLogLevel, Overlays.DrawingSpec> _customDrawingSpecsMessages;


    /// <summary>
    /// Initializes a new instance of the FormTextPanelCustom class.
    /// </summary>
    public FormTextPanelCustom()
    {
        InitializeComponent();

        _customLogLevelTextPanel = new Overlays.TextPanel<CustomLogLevel>();
        _customLogLevelTextPanel.AddMessage(CustomLogLevel.Trace, "This is a trace message.");
        _customLogLevelTextPanel.AddMessage(CustomLogLevel.Debug, "This is a debug message.");
        _customLogLevelTextPanel.AddMessage(CustomLogLevel.Info, "This is an informational message.");
        _customLogLevelTextPanel.AddMessage(CustomLogLevel.Warning, "This is a warning message.");
        _customLogLevelTextPanel.AddMessage(CustomLogLevel.Error, "This is an error message.");
        _customLogLevelTextPanel.AddMessage(CustomLogLevel.Critical, "This is a critical message.");

        _customDrawingSpecPanel = new Overlays.DrawingSpec();
        _customDrawingSpecPanel.Lines.Color = Color.FromArgb(240, 0, 0, 64);
        _customDrawingSpecPanel.Lines.Width = 5;
        _customDrawingSpecPanel.Lines.DashStyle = System.Drawing.Drawing2D.DashStyle.Dot;
        _customDrawingSpecPanel.Fill.Color = Color.FromArgb(180, 0, 0, 64);

        _customDrawingSpecsMessages = new Dictionary<CustomLogLevel, Overlays.DrawingSpec>();
        int counter = 0;
        foreach (CustomLogLevel logLevel in Enum.GetValues(typeof(CustomLogLevel)))
        {
            _customDrawingSpecsMessages[logLevel] = new Overlays.DrawingSpec();
            _customDrawingSpecsMessages[logLevel].Font.FontSize = 12 + (counter * 2);
            _customDrawingSpecsMessages[logLevel].Fill.Color = Color.FromArgb(counter * 50, 255 - (counter * 50), 0);

            counter++;
        }
    }

    private void bitmapDisplayPanelCustom_OnPaintOver(object sender, WinForms.BitmapDisplay.PaintOverEventArgs e)
    {
        _customLogLevelTextPanel.Draw(
            e.Sender,
            e.Graphics,
            _customDrawingSpecPanel,
            getDrawingSpecForMessageType: logLevel => _customDrawingSpecsMessages[logLevel]);
    }
}

            
