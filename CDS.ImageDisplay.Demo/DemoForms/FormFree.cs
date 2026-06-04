using CDS.ImageDisplay.Demo.OpenCVSharpExtras;
using System;
using System.IO;
using System.Windows.Forms;

namespace CDS.ImageDisplay.Demo.DemoForms;

internal sealed partial class FormFree : Form
{
    public FormFree()
    {
        InitializeComponent();
    }


    protected override void OnLoad(EventArgs e)
    {   
        base.OnLoad(e);


        string folder = @"D:\Dropbox\CDS\Dev\IVMS\Conamec\Self Test NOK";
        var img = OpenCvSharp.Cv2.ImRead(Path.Combine(folder, @"Image0002.png"), OpenCvSharp.ImreadModes.Grayscale);
        bitmapDisplayPanel.CDSSetImage(img);


        //bitmapDisplayPanel.SetImage(Properties.Resources.Thailand);
    }
}
