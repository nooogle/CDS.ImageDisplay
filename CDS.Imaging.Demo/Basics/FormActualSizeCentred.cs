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

public partial class FormActualSizeCentred : Form
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
