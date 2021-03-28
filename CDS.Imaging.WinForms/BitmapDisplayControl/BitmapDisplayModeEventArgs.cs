using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDS.Imaging.WinForms
{
    public class BitmapDisplayModeEventArgs : EventArgs
    {
        public BitmapDisplayMode NewMode { get; }


        public BitmapDisplayModeEventArgs(BitmapDisplayMode newMode)
        {
            NewMode = newMode;
        }
    }
}
