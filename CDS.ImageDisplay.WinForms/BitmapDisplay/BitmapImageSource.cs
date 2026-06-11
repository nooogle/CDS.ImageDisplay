using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace CDS.ImageDisplay.WinForms.BitmapDisplay;

/// <summary>
/// Provides read-only access to the properties and pixels for a .Net Bitmap
/// </summary>
public sealed class BitmapImageSource : IImageSource, IDisposable
{
    private readonly Bitmap? _bitmap;
    private readonly BitmapData? _imageData;


    bool IImageSource.IsImageAvailable => _bitmap != null;

    int IImageSource.Stride => (_imageData == null) ? 0 : _imageData.Stride;

    Size IImageSource.Size => (_imageData == null) ? Size.Empty : new Size(_imageData.Width, _imageData.Height);

    int IImageSource.Width => ((IImageSource)this).Size.Width;

    int IImageSource.Height => ((IImageSource)this).Size.Height;

    IntPtr IImageSource.Scan0 => (_imageData == null) ? IntPtr.Zero : _imageData.Scan0;

    PixelFormat IImageSource.PixelFormat => (_imageData == null) ? PixelFormat.Undefined : _imageData.PixelFormat;


    /// <summary>
    /// Locks the bitap for reading and provide access via the properties
    /// to the bitmap properties and pixels. 
    /// </summary>
    public BitmapImageSource(Bitmap? bitmap)
    {
        this._bitmap = bitmap;

        if (bitmap != null)
        {
            var roi = new Rectangle(0, 0, bitmap.Width, bitmap.Height);

            _imageData = bitmap.LockBits(
                rect: roi,
                flags: ImageLockMode.ReadOnly,
                format: bitmap.PixelFormat);
        }
    }


    /// <summary>
    /// Unlocks the bitmap
    /// </summary>
    public void Dispose()
    {
        if ((_bitmap != null) && (_imageData != null))
        {
            _bitmap.UnlockBits(_imageData);
        }
    }
}
