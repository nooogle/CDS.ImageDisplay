using System;
using System.Windows.Forms;

namespace CDS.ImageDisplay.Demo.DemoForms;

internal sealed partial class FormActualSizeCentred : Form
{
    public FormActualSizeCentred()
    {
        InitializeComponent();
    }


    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);
        bitmapDisplayPanel1.SetImage(Properties.Resources.Thailand);
    }
}
