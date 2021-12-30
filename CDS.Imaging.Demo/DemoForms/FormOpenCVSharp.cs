using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenCvSharp.Extensions;
using CDS.Imaging.Demo.OpenCVSharpExtras;

namespace CDS.Imaging.Demo.DemoForms
{
    public partial class FormOpenCVSharp : Form
    {
        OpenCvSharp.Mat? cvImageGrey;
        OpenCvSharp.Mat? cvImageBlurred;

        public FormOpenCVSharp()
        {
            InitializeComponent();
        }


        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            bitmapPanel1.CDS.SetImage(Properties.Resources.Thailand);

            var cvImageColor = Properties.Resources.Thailand.ToMat();

            bitmapPanel2.SetImage(cvImageColor);

            cvImageGrey = new OpenCvSharp.Mat(
                rows: cvImageColor.Rows,
                cols: cvImageColor.Cols,
                type: OpenCvSharp.MatType.CV_8UC1);

            OpenCvSharp.Cv2.CvtColor(
                src: cvImageColor,
                dst: cvImageGrey,
                code: OpenCvSharp.ColorConversionCodes.RGB2GRAY);

            bitmapPanel3.CDS.SetImage(cvImageGrey);

            cvImageBlurred = new OpenCvSharp.Mat(
                rows: cvImageColor.Rows,
                cols: cvImageColor.Cols,
                type: OpenCvSharp.MatType.CV_8UC1);

            ProcessBlurring();
        }

        private void ProcessBlurring()
        {
            if ((cvImageBlurred == null) || (cvImageGrey == null)) { return; }

            var ksize = (trackBarGaussianSize.Value * 2) + 1;

            OpenCvSharp.Cv2.GaussianBlur(
                src: cvImageGrey,
                dst: cvImageBlurred,
                ksize: new OpenCvSharp.Size(ksize, ksize),
                sigmaX: trackGaussianSigma.Value);

            bitmapPanel4.CDS.SetImage(cvImageBlurred);
        }

        private void trackBarGaussianSize_Scroll(object sender, EventArgs e)
        {
            ProcessBlurring();
        }

        private void trackGaussianSigma_Scroll(object sender, EventArgs e)
        {
            ProcessBlurring();
        }
    }
}
