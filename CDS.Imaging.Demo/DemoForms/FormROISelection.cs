using System;
using System.Drawing;
using System.Windows.Forms;

namespace CDS.Imaging.Demo.DemoForms;

public partial class FormROISelection : Form
{
    public FormROISelection()
    {
        InitializeComponent();
    }


    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);
        bitmapDisplayPanel.CDSSetImage(Properties.Resources.Thailand);

        //bitmapDisplayPanel.CDSSetROI(new Rectangle(100, 100, 100, 50));

        bitmapDisplayPanel.CDSMouseMode = WinForms.BitmapDisplay.MouseMode.ROISelection;
    }
}
