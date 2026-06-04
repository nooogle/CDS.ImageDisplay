using System;
using System.Drawing;
using System.Drawing.Imaging;
using CDS.ImageDisplay.BitmapDisplay;

namespace CDS.ImageDisplay.Demo.OpenCVSharpExtras;

/// <summary>
/// Adapts an <see cref="OpenCvSharp.Mat"/> image so that it can be displayed on a
/// <see cref="BitmapDisplayPanel"/> control by implementing the <see cref="IImageSource"/>
/// interface.
/// </summary>
/// <remarks>
/// <para>
/// <see cref="BitmapDisplayPanel"/> accepts images through the <see cref="IImageSource"/>
/// interface rather than directly consuming framework-specific types such as
/// <see cref="OpenCvSharp.Mat"/>. <see cref="MatImageSource"/> bridges that gap by wrapping a
/// <see cref="OpenCvSharp.Mat"/> and exposing its raw pixel data through
/// <see cref="IImageSource"/> in a format the control can consume for supported image types,
/// avoiding additional format conversion in this adapter even though the control still copies
/// the image into its own backing bitmap.
/// </para>
/// <para>
/// For the most convenient way to display an OpenCV image, use the
/// <see cref="ExtensionMethods.CDSSetImage"/> extension method on
/// <see cref="BitmapDisplayPanel"/>, which constructs a <see cref="MatImageSource"/> and calls
/// <see cref="BitmapDisplayPanel.SetImage(IImageSource?)"/> in a single step.
/// </para>
/// <para>
/// Supported <see cref="OpenCvSharp.Mat"/> types are:
/// <list type="bullet">
///   <item><description><c>CV_8UC1</c> — 8-bit greyscale</description></item>
///   <item><description><c>CV_8UC3</c> — 8-bit BGR colour</description></item>
///   <item><description><c>CV_8UC4</c> — 8-bit BGRA colour with alpha</description></item>
/// </list>
/// Any other type will cause a <see cref="NotSupportedException"/> to be thrown in the
/// constructor.
/// </para>
/// </remarks>
internal sealed class MatImageSource : IImageSource
{
    private readonly OpenCvSharp.Mat? mat;
    private readonly PixelFormat pixelFormat = PixelFormat.Undefined;
    private readonly int width;

    bool IImageSource.IsImageAvailable => mat != null;

    int IImageSource.Stride => (mat == null) ? 0 : (int)mat.Step();

    int IImageSource.Width => width;

    int IImageSource.Height => (mat == null) ? 0 : mat.Height;

    Size IImageSource.Size => (mat == null) ? Size.Empty : new Size(width, mat.Height);

    IntPtr IImageSource.Scan0 => (mat == null) ? IntPtr.Zero : mat.Data;

    PixelFormat IImageSource.PixelFormat => pixelFormat;


    /// <summary>
    /// Initialises a new instance of <see cref="MatImageSource"/> wrapping the supplied
    /// <paramref name="mat"/>.
    /// </summary>
    /// <param name="mat">
    /// The <see cref="OpenCvSharp.Mat"/> to wrap, or <see langword="null"/> to represent no
    /// image.
    /// </param>
    /// <param name="logicalWidth">
    /// The logical image width in pixels to report through <see cref="IImageSource.Width"/>.
    /// When <see langword="null"/> (the default) <paramref name="mat"/>.<see cref="OpenCvSharp.Mat.Width"/>
    /// is used. Pass the original (unpadded) width when <paramref name="mat"/> was widened to
    /// achieve a 4-byte-aligned stride.
    /// </param>
    /// <exception cref="NotSupportedException">
    /// Thrown when <paramref name="mat"/> is not <see langword="null"/> and its type is not one
    /// of the supported 8-bit formats (<c>CV_8UC1</c>, <c>CV_8UC3</c>, <c>CV_8UC4</c>).
    /// </exception>
    public MatImageSource(OpenCvSharp.Mat? mat, int? logicalWidth = null)
    {
        this.mat = mat;
        width = logicalWidth ?? mat?.Width ?? 0;

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
