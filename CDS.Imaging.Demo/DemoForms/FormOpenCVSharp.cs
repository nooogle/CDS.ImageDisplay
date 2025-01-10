using CDS.Imaging.Demo.OpenCVSharpExtras;
using OpenCvSharp.Extensions;
using System;
using System.Windows.Forms;

namespace CDS.Imaging.Demo.DemoForms
{
    public partial class FormOpenCVSharp : Form
    {
        bool changingPaintRectProgramatically;
        OpenCvSharp.Mat? cvImageGrey;
        OpenCvSharp.Mat? cvImageBlurred;


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

            cvImageGrey = new OpenCvSharp.Mat(
                rows: cvImageColor.Rows,
                cols: cvImageColor.Cols,
                type: OpenCvSharp.MatType.CV_8UC1);

            OpenCvSharp.Cv2.CvtColor(
                src: cvImageColor,
                dst: cvImageGrey,
                code: OpenCvSharp.ColorConversionCodes.RGB2GRAY);

            bitmapPanel3.CDSSetImage(cvImageGrey);

            cvImageBlurred = new OpenCvSharp.Mat(
                rows: cvImageColor.Rows,
                cols: cvImageColor.Cols,
                type: OpenCvSharp.MatType.CV_8UC1);

            ProcessBlurring();
        }

        private void ProcessBlurring()
        {
            if ((cvImageBlurred == null) || (cvImageGrey == null)) { return; }

            var stopwatch = System.Diagnostics.Stopwatch.StartNew();

            var ksize = (trackBarGaussianSize.Value * 2) + 1;

            OpenCvSharp.Cv2.GaussianBlur(
                src: cvImageGrey,
                dst: cvImageBlurred,
                ksize: new OpenCvSharp.Size(ksize, ksize),
                sigmaX: trackGaussianSigma.Value);

            stopwatch.Stop();
            labelProcessTimeMS.Text = $"Processing time: {stopwatch.ElapsedMilliseconds:0.0} ms";

            bitmapPanel4.CDSSetImage(cvImageBlurred);
        }

        private void trackBarGaussianSize_Scroll(object sender, EventArgs e)
        {
            ProcessBlurring();
        }

        private void trackGaussianSigma_Scroll(object sender, EventArgs e)
        {
            ProcessBlurring();
        }


        protected override void OnClientSizeChanged(EventArgs e)
        {
            base.OnClientSizeChanged(e);
            bitmapPanel4.FitToWindowCentred();
        }


        private void bitmapPanel_CDSPaintRectChanged(WinForms.BitmapDisplay.BitmapDisplayPanel sender)
        {
            SyncPaintRects(sender);
        }

        private void SyncPaintRects(WinForms.BitmapDisplay.BitmapDisplayPanel sender)
        {
            if (changingPaintRectProgramatically) { return; }
            changingPaintRectProgramatically = true;

            if (sender != bitmapPanel1) { bitmapPanel1.SyncPaintRectFromOther(sender); }
            if (sender != bitmapPanel2) { bitmapPanel2.SyncPaintRectFromOther(sender); }
            if (sender != bitmapPanel3) { bitmapPanel3.SyncPaintRectFromOther(sender); }
            if (sender != bitmapPanel4) { bitmapPanel4.SyncPaintRectFromOther(sender); }

            changingPaintRectProgramatically = false;
        }
    }
}
