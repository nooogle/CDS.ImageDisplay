using System;
using System.Windows.Forms;
using CDS.ImageDisplay.Demo.OpenCVSharpExtras;
using OpenCvSharp.Extensions;

namespace CDS.ImageDisplay.Demo.DemoForms;

public partial class FormOpenCVSharp : Form
{
    private bool _changingPaintRectProgramatically;
    private OpenCvSharp.Mat? _cvImageGrey;
    private OpenCvSharp.Mat? _cvImageBlurred;


    public FormOpenCVSharp()
    {
        InitializeComponent();
    }


    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        bitmapPanel1.SetImage(Properties.Resources.Thailand);

        var cvImageColor = Properties.Resources.Thailand.ToMat();

        bitmapPanel2.CDSSetImage(cvImageColor);

        _cvImageGrey = new OpenCvSharp.Mat(
            rows: cvImageColor.Rows,
            cols: cvImageColor.Cols,
            type: OpenCvSharp.MatType.CV_8UC1);

        OpenCvSharp.Cv2.CvtColor(
            src: cvImageColor,
            dst: _cvImageGrey,
            code: OpenCvSharp.ColorConversionCodes.RGB2GRAY);

        bitmapPanel3.CDSSetImage(_cvImageGrey);

        _cvImageBlurred = new OpenCvSharp.Mat(
            rows: cvImageColor.Rows,
            cols: cvImageColor.Cols,
            type: OpenCvSharp.MatType.CV_8UC1);

        ProcessBlurring();
    }

    private void ProcessBlurring()
    {
        if ((_cvImageBlurred == null) || (_cvImageGrey == null))
        { return; }

        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        int ksize = (trackBarGaussianSize.Value * 2) + 1;

        OpenCvSharp.Cv2.GaussianBlur(
            src: _cvImageGrey,
            dst: _cvImageBlurred,
            ksize: new OpenCvSharp.Size(ksize, ksize),
            sigmaX: trackGaussianSigma.Value);

        stopwatch.Stop();
        labelProcessTimeMS.Text = $"Processing time: {stopwatch.ElapsedMilliseconds:0.0} ms";

        bitmapPanel4.CDSSetImage(_cvImageBlurred);
    }

    private void trackBarGaussianSize_Scroll(object sender, EventArgs e) => ProcessBlurring();

    private void trackGaussianSigma_Scroll(object sender, EventArgs e) => ProcessBlurring();


    protected override void OnClientSizeChanged(EventArgs e)
    {
        base.OnClientSizeChanged(e);
        bitmapPanel4.FitToWindowCentred();
    }


    private void bitmapPanel_CDSPaintRectChanged(BitmapDisplay.BitmapDisplayPanel sender) => SyncPaintRects(sender);

    private void SyncPaintRects(BitmapDisplay.BitmapDisplayPanel sender)
    {
        if (_changingPaintRectProgramatically)
        { return; }
        _changingPaintRectProgramatically = true;

        if (sender != bitmapPanel1)
        { bitmapPanel1.SyncPaintRectFromOther(sender); }
        if (sender != bitmapPanel2)
        { bitmapPanel2.SyncPaintRectFromOther(sender); }
        if (sender != bitmapPanel3)
        { bitmapPanel3.SyncPaintRectFromOther(sender); }
        if (sender != bitmapPanel4)
        { bitmapPanel4.SyncPaintRectFromOther(sender); }

        _changingPaintRectProgramatically = false;
    }
}
