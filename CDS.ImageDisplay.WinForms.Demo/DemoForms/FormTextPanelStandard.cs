using System.Windows.Forms;

namespace CDS.ImageDisplay.WinForms.Demo.DemoForms;


/// <summary>
/// Demonstration that shows what the bitmap display panel looks like when an image has not been assigned.
/// </summary>
internal sealed partial class FormTextPanelStandard : Form
{
    private WinForms.Overlays.TextPanelStd _textPanel = new();


    /// <summary>
    /// Initializes a new instance of the FormTextPanelStandard class.
    /// </summary>
    public FormTextPanelStandard()
    {
        InitializeComponent();

        _textPanel.AddMessage(Overlays.TextPanelStdMsgTypes.Info, "This is an informational message.");
        _textPanel.AddMessage(Overlays.TextPanelStdMsgTypes.Warning, "This is a warning message.");
        _textPanel.AddMessage(Overlays.TextPanelStdMsgTypes.Error, "This is an error message.");
    }

    private void bitmapDisplayPanelStandard_OnPaintOver(object sender, WinForms.BitmapDisplay.PaintOverEventArgs e)
    {
        _textPanel.Draw(e.Sender, e.Graphics);
    }
}
