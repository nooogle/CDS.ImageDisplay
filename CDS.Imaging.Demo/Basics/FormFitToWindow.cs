using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CDS.Imaging.Demo.Basics;

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
