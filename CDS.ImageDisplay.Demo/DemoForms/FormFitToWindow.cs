using System;
using System.Windows.Forms;

namespace CDS.ImageDisplay.Demo.DemoForms;

public partial class FormFitToWindow : Form
{
    public FormFitToWindow()
    {
        InitializeComponent();
    }


    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);
        bitmapDisplayPanel.SetImage(Properties.Resources.Thailand);
    }
}
