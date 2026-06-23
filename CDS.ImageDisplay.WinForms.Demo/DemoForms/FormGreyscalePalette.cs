using System;
using System.Drawing;
using System.Windows.Forms;
using CDS.ImageDisplay.WinForms.BitmapDisplay;

namespace CDS.ImageDisplay.WinForms.Demo.DemoForms;

internal sealed partial class FormGreyscalePalette : Form
{
    private readonly Bitmap _demoBitmap;

    public FormGreyscalePalette()
    {
        _demoBitmap = BitmapGenerator.Make8bppSaturationDemo();
        InitializeComponent();
    }

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);
        _panelStandard.SetImage(_demoBitmap);
        _panelInverted.SetImage(_demoBitmap);
        _panelHighlightSaturated.SetImage(_demoBitmap);
    }
}
