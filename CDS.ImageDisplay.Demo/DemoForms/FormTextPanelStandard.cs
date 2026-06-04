using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace CDS.ImageDisplay.Demo.DemoForms;


/// <summary>
/// Demonstration that shows what the bitmap display panel looks like when an image has not been assigned.
/// </summary>
internal sealed partial class FormTextPanelStandard : Form
{
    private Overlays.TextPanel<Overlays.TextPanelStdMsgTypes> _textPanel;
    private Overlays.TextPanelStdMsgDrawingSpecs _standardDrawingSpecs = new Overlays.TextPanelStdMsgDrawingSpecs();


    /// <summary>
    /// Initializes a new instance of the FormTextPanelStandard class.
    /// </summary>
    public FormTextPanelStandard()
    {
        InitializeComponent();

        _textPanel = new Overlays.TextPanel<Overlays.TextPanelStdMsgTypes>();

        _textPanel.AddMessage(Overlays.TextPanelStdMsgTypes.Info, "This is an informational message.");
        _textPanel.AddMessage(Overlays.TextPanelStdMsgTypes.Warning, "This is a warning message.");
        _textPanel.AddMessage(Overlays.TextPanelStdMsgTypes.Error, "This is an error message.");
    }

    private void bitmapDisplayPanelStandard_OnPaintOver(object sender, CDS.ImageDisplay.BitmapDisplay.PaintOverEventArgs e)
    {
        var bitmapDisplayPanel = sender as BitmapDisplay.BitmapDisplayPanel;

        _textPanel.Draw(
            bitmapDisplayPanel!,
            e.Graphics,
            _standardDrawingSpecs.Panel,
            getDrawingSpecForMessageType: _standardDrawingSpecs.GetDrawingSpecForMessageType);
    }
}
