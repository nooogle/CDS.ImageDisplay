using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace CDS.ImageDisplay.WinForms.BitmapDisplay;

/// <summary>
/// Manages a .Net Bitmap. New images will be copied into the image when
/// the specification is the same; otherwise a new image is created and
/// a clone taken of the new image.
/// </summary>
internal sealed class ImageWrapper : IDisposable
{
    /// <summary>
    /// The image
    /// </summary>
    public Bitmap? Image { get; private set; }


    /// <summary>
    /// The size of the image, or Size.Empty if there isn't an image
    /// </summary>
    public Size ImageSize => (Image == null) ? Size.Empty : Image.Size;


    /// <summary>
    /// Clean up managed resources
    /// </summary>
    public void Dispose()
    {
        Image?.Dispose();
        Image = null;
    }


    /// <summary>
    /// Sets a new image, reusing the existing <see cref="Image"/> if it
    /// exists and is the same specification as the new image, otherwise 
    /// creating a clone of the new image.
    /// </summary>
    public void SetNewImage(IImageSource? imageSource)
    {
        if ((imageSource == null) || (imageSource.Size == Size.Empty))
        {
            DropImage();
        }
        else if (Image == null)
        {
            CreateImageFromOther(imageSource);
        }
        else
        {
            bool sameSpecification =
                (Image.PixelFormat == imageSource.PixelFormat) &&
                (Image.Size == imageSource.Size);

            if (sameSpecification)
            {
                CopyImageBitsIntoExisting(imageSource);
            }
            else
            {
                DropImage();
                CreateImageFromOther(imageSource);
            }
        }
    }


    /// <summary>
    /// Creates the display image from a full clone of another image
    /// and reconfigures the display
    /// </summary>
    private void CreateImageFromOther(IImageSource imageSource)
    {
        Image = new Bitmap(imageSource.Width, imageSource.Height, imageSource.PixelFormat);

        if (imageSource.PixelFormat == System.Drawing.Imaging.PixelFormat.Format8bppIndexed)
        {
            ColorPalette palette = Image.Palette;
            for (int index = 0; index < 256; index++)
            {
                palette.Entries[index] = Color.FromArgb(index, index, index);
            }

            Image.Palette = palette;
        }


        CopyImageBitsIntoExisting(imageSource);
    }


    /// <summary>
    /// Copy pixels from one image directly into another; the images
    /// must have the same specification (checked by the caller!)
    /// </summary>
    private void CopyImageBitsIntoExisting(IImageSource imageSource)
    {
        if (Image == null)
        { return; }

        var roi = new Rectangle(0, 0, Image.Width, Image.Height);
        BitmapData destBits = Image.LockBits(roi, System.Drawing.Imaging.ImageLockMode.WriteOnly, Image.PixelFormat);

        int sourceBytesToCopy = imageSource.Stride * imageSource.Height;
        int destBytesAvailable = destBits.Stride * destBits.Height;

        unsafe
        {
            System.Diagnostics.Debug.Assert(imageSource.Scan0 != destBits.Scan0);
            System.Diagnostics.Debug.Assert(sourceBytesToCopy > 0);
            System.Diagnostics.Debug.Assert(imageSource.Stride == destBits.Stride,
                "Source and destination strides differ; only the destination stride was used before this fix.");

            Buffer.MemoryCopy(
                source: (void*)imageSource.Scan0,
                destination: (void*)destBits.Scan0,
                destinationSizeInBytes: destBytesAvailable,
                sourceBytesToCopy: sourceBytesToCopy);
        }

        Image.UnlockBits(destBits);
    }


    /// <summary>
    /// Disposes the display image; should only be called on the UI thread!
    /// </summary>
    private void DropImage()
    {
        Image?.Dispose();
        Image = null;
    }
}
