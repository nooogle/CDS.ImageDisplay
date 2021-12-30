using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDS.Imaging.WinForms.BitmapDisplay
{
    /// <summary>
    /// Manages a .Net Bitmap. New images will be copied into the image when
    /// the specification is the same; otherwise a new image is created and 
    /// a clone taken of the new image.
    /// </summary>
    public class ImageWrapper : IDisposable
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
        public void SetNewImage(Bitmap? newImage)
        {
            if (newImage == null)
            {
                DropImage();
            }
            else if (Image == null)
            {
                CreateImageFromOther(newImage: newImage);
            }
            else
            {
                var sameSpecification =
                    (Image.PixelFormat == newImage.PixelFormat) &&
                    (Image.Size == newImage.Size);

                if (sameSpecification)
                {
                    CopyImageBitsIntoExisting(source: newImage);
                }
                else
                {
                    DropImage();
                    CreateImageFromOther(newImage: newImage);
                }
            }
        }


        /// <summary>
        /// Creates the display image from a full clone of another image
        /// and reconfigures the display
        /// </summary>
        private void CreateImageFromOther(Bitmap newImage)
        {
            Image = (Bitmap)newImage.Clone();
        }


        /// <summary>
        /// Copy pixels from one image directly into another; the images
        /// must have the same specification (checked by the caller!)
        /// </summary>
        private void CopyImageBitsIntoExisting(Bitmap source)
        {
            if(Image == null) { return; }

            var roi = new Rectangle(0, 0, source.Width, source.Height);

            var sourceBits = source.LockBits(roi, System.Drawing.Imaging.ImageLockMode.ReadOnly, source.PixelFormat);
            var destBits = Image.LockBits(roi, System.Drawing.Imaging.ImageLockMode.WriteOnly, Image.PixelFormat);

            var bytesToCopy = sourceBits.Stride * source.Height;

            unsafe
            {
                Buffer.MemoryCopy(
                    source: (void*)sourceBits.Scan0,
                    destination: (void*)destBits.Scan0,
                    destinationSizeInBytes: bytesToCopy,
                    sourceBytesToCopy: bytesToCopy);
            }

            Image.UnlockBits(destBits);
            source.UnlockBits(sourceBits);
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
}
