using CDS.Imaging.BitmapDisplay;
using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace CDS.Imaging.Demo.OpenCVSharpExtras
{
    public class MatImageSource : IImageSource
    {
        private OpenCvSharp.Mat? mat;
        private PixelFormat pixelFormat = PixelFormat.Undefined;

        bool IImageSource.IsImageAvailable => (mat != null);

        int IImageSource.Stride => (mat == null) ? 0 : (int)mat.Step();

        int IImageSource.Width => (mat == null) ? 0 : mat.Width;

        int IImageSource.Height => (mat == null) ? 0 : mat.Height;

        Size IImageSource.Size => (mat == null) ? Size.Empty : new Size(mat.Width, mat.Height);

        IntPtr IImageSource.Scan0 => (mat == null) ? IntPtr.Zero : mat.Data;

        PixelFormat IImageSource.PixelFormat => pixelFormat;


        public MatImageSource(OpenCvSharp.Mat? mat)
        {
            this.mat = mat;

            if (mat != null)
            {
                if (mat.Type() == OpenCvSharp.MatType.CV_8UC1)
                {
                    pixelFormat = PixelFormat.Format8bppIndexed;
                }
                else if (mat.Type() == OpenCvSharp.MatType.CV_8UC3)
                {
                    pixelFormat = PixelFormat.Format24bppRgb;
                }
                else if (mat.Type() == OpenCvSharp.MatType.CV_8UC4)
                {
                    pixelFormat = PixelFormat.Format32bppArgb;
                }

                if (pixelFormat == PixelFormat.Undefined)
                {
                    throw new NotSupportedException($"Only 8-bit images with 1, 3 or 4 channels are supported");
                }
            }
        }
    }

    public static class ExtensionMethods
    {
        public static void CDSSetImage(this BitmapDisplayPanel bitmapDisplay, OpenCvSharp.Mat mat)
        {
            var matImageSource = new MatImageSource(mat);
            bitmapDisplay.SetImage(matImageSource);
        }
    }
}
