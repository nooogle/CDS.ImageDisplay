using System;
using CDS.ImageDisplay.BitmapDisplay;

namespace CDS.ImageDisplay.Demo.OpenCVSharpExtras;

/// <summary>
/// Extension methods that simplify displaying OpenCV images on a <see cref="BitmapDisplayPanel"/>.
/// </summary>
internal static class ExtensionMethods
{
    /// <summary>
    /// Displays an <see cref="OpenCvSharp.Mat"/> image on the <see cref="BitmapDisplayPanel"/>.
    /// </summary>
    /// <remarks>
    /// <para>
    /// When the <paramref name="mat"/> stride is already a multiple of 4 (as required by GDI) the
    /// Mat is wrapped directly in a <see cref="MatImageSource"/> adapter with no data copy.
    /// When the stride is <em>not</em> a multiple of 4 (for example, a 3-channel image whose width
    /// is not divisible by 4) a temporary <see cref="OpenCvSharp.Mat"/> is created with the
    /// required row padding so that GDI can consume the pixel data correctly.
    /// </para>
    /// <para>
    /// In either case the control itself retains a copy of the image data, so the original
    /// <paramref name="mat"/> (and any temporary Mat) may be safely disposed after this call
    /// returns.
    /// </para>
    /// </remarks>
    /// <param name="bitmapDisplay">The <see cref="BitmapDisplayPanel"/> on which to display the image.</param>
    /// <param name="mat">The <see cref="OpenCvSharp.Mat"/> image to display.</param>
    /// <exception cref="System.NotSupportedException">
    /// Thrown when <paramref name="mat"/> has an unsupported type. See <see cref="MatImageSource"/>
    /// for the list of supported types.
    /// </exception>
    public static void CDSSetImage(this BitmapDisplayPanel bitmapDisplay, OpenCvSharp.Mat mat)
    {
        ArgumentNullException.ThrowIfNull(bitmapDisplay);

        if (IsStrideAligned(mat))
        {
            var matImageSource = new MatImageSource(mat);
            bitmapDisplay.SetImage(matImageSource);
        }
        else
        {
            using var paddedMat = CreateStridePaddedMat(mat);
            var matImageSource = new MatImageSource(paddedMat, logicalWidth: mat.Width);
            bitmapDisplay.SetImage(matImageSource);
        }
    }

    /// <summary>
    /// Returns <see langword="true"/> when the <paramref name="mat"/> row stride is already a
    /// multiple of 4, which is required for GDI-based rendering.
    /// </summary>
    private static bool IsStrideAligned(OpenCvSharp.Mat mat)
        => mat.Step() % 4 == 0;

    /// <summary>
    /// Creates a new <see cref="OpenCvSharp.Mat"/> that contains the same pixels as
    /// <paramref name="source"/> but whose row stride is padded to the next multiple of 4.
    /// The caller is responsible for disposing the returned Mat.
    /// </summary>
    private static OpenCvSharp.Mat CreateStridePaddedMat(OpenCvSharp.Mat source)
    {
        int channels = source.Channels();
        int width = source.Width;
        int height = source.Height;

        // Calculate how many extra columns of pixels are needed so that
        // width * bytesPerPixel is a multiple of 4.  Each "column" is one
        // pixel wide, so we work in bytes-per-row.
        int bytesPerPixel = channels; // all supported types are CV_8U
        int unpaddedRowBytes = width * bytesPerPixel;
        int paddedRowBytes = (unpaddedRowBytes + 3) & ~3;
        int extraPixels = (paddedRowBytes - unpaddedRowBytes) / bytesPerPixel;

        // copyMakeBorder adds the padding columns on the right; the image
        // content is unchanged and the resulting Mat has the required stride.
        var padded = new OpenCvSharp.Mat();
        OpenCvSharp.Cv2.CopyMakeBorder(
            source,
            padded,
            top: 0,
            bottom: 0,
            left: 0,
            right: extraPixels,
            borderType: OpenCvSharp.BorderTypes.Constant,
            value: OpenCvSharp.Scalar.All(0));

        return padded;
    }
}
