using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace CDS.ImageDisplay.BitmapDisplay;

/// <summary>
/// Provides access to an image's properties and pixel data
/// </summary>
public interface IImageSource
{
    /// <summary>
    /// True if an image is available
    /// </summary>
    bool IsImageAvailable { get; } 


    /// <summary>
    /// Stride of the image
    /// </summary>
    int Stride { get; }


    /// <summary>
    /// Width
    /// </summary>
    int Width { get; }


    /// <summary>
    /// Height
    /// </summary>
    int Height { get; }


    /// <summary>
    /// Size
    /// </summary>
    Size Size { get; } 


    /// <summary>
    /// Point to first pixel
    /// </summary>
    IntPtr Scan0 { get; }


    /// <summary>
    /// Image format
    /// </summary>
    PixelFormat PixelFormat { get; }
}
