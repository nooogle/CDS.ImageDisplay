using System;
using System.Windows.Forms;

namespace CDS.ImageDisplay.Demo.DemoForms;

public partial class FormFree : Form
{
    public FormFree()
    {
        InitializeComponent();
    }


    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);
        bitmapDisplayPanel.SetImage(Properties.Resources.Thailand);
    }
}
