using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CDS.Imaging.Demo.Simple
{
    public partial class FormLocked : Form
    {
        public FormLocked()
        {
            InitializeComponent();
        }

        private void FormLocked_Load(object sender, EventArgs e)
        {
            bitmapDisplayPanel.CDS.Mode = WinForms.BitmapDisplay.BitmapDisplayMode.Free;

            bitmapDisplayPanel.CDS.TargetDisplayCentre = new PointF(
                12 + (bitmapDisplayPanel.CDS.Image.Width / 2),
                12 + (bitmapDisplayPanel.CDS.Image.Height / 2));
            
            bitmapDisplayPanel.CDS.Mode = WinForms.BitmapDisplay.BitmapDisplayMode.Locked;
        }
    }
}
