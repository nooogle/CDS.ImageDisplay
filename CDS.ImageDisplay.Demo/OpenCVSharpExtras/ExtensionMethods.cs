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
    /// This method wraps the <paramref name="mat"/> in a <see cref="MatImageSource"/> adapter and
    /// passes it to <see cref="BitmapDisplayPanel.SetImage(IImageSource?)"/>, allowing an OpenCV image to be
    /// rendered without conversion; note that the control itself will retain a copy of the image data.
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
        var matImageSource = new MatImageSource(mat);
        bitmapDisplay.SetImage(matImageSource);
    }
}
