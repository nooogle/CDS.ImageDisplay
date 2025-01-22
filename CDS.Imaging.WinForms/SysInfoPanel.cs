using System;
using System.Windows.Forms;

namespace CDS.Imaging
{
    /// <summary>
    /// Panel to display system information
    /// </summary>
    public partial class SysInfoPanel : UserControl
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public SysInfoPanel()
        {
            InitializeComponent();
        }


        /// <summary>
        /// Load the system information
        /// </summary>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            labelSystemInfo.Text = SystemInfo.Get();
        }
    }
}
