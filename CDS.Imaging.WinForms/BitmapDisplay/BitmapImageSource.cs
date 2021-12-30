using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDS.Imaging.WinForms.BitmapDisplay
{
    /// <summary>
    /// Provides read-only access to the properties and pixels for a .Net Bitmap
    /// </summary>
    public class BitmapImageSource : IImageSource, IDisposable
    {
        private Bitmap? bitmap;
        private BitmapData? imageData;


        bool IImageSource.IsImageAvailable => (bitmap != null);

        int IImageSource.Stride => (imageData == null) ? 0 : imageData.Stride;

        Size IImageSource.Size => (imageData == null) ? Size.Empty : new Size(imageData.Width, imageData.Height);

        int IImageSource.Width => ((IImageSource)this).Size.Width;

        int IImageSource.Height => ((IImageSource)this).Size.Height;

        IntPtr IImageSource.Scan0 => (imageData == null) ? IntPtr.Zero : imageData.Scan0;

        PixelFormat IImageSource.PixelFormat => (imageData == null) ? PixelFormat.Undefined : imageData.PixelFormat;


        /// <summary>
        /// Locks the bitap for reading and provide access via the properties
        /// to the bitmap properties and pixels. 
        /// </summary>
        public BitmapImageSource(Bitmap? bitmap)
        {
            this.bitmap = bitmap;

            if (bitmap != null)
            {
                var roi = new Rectangle(0, 0, bitmap.Width, bitmap.Height);

                imageData = bitmap.LockBits(
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
            if ((bitmap != null) && (imageData != null))
            {
                bitmap.UnlockBits(imageData);
            }
        }
    }
}
