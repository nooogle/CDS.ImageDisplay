using System;
using System.Drawing;

namespace CDS.Imaging.WinForms.BitmapDisplay
{
    public sealed class BitmapOnDisplay : IDisposable
    {
        private Bitmap displayBitmap;
        private ImageDisplayMode mode = ImageDisplayMode.FitToWindowCentred;
        private readonly Action InvalidateAll;
        private readonly Action<Rectangle> InvalidateRect;
        private Func<Size> GetClientSize;


        public Image Image => displayBitmap;
        public RectangleF DisplayRect { get; private set; }

        public bool AnythingToDisplay => !DisplayRect.IsEmpty;


        public ImageDisplayMode Mode
        {
            get => mode;

            set
            {
                if (mode != value)
                {
                    mode = value;
                    RecalculateRenderRect(displayMode: this.mode);
                    InvalidateAll();
                }
            }
        }


        public BitmapOnDisplay(
            Action invalidateAll,
            Action<Rectangle> invalidateRect,
            Func<Size> getClientSize)
        {
            InvalidateAll = invalidateAll;
            InvalidateRect = invalidateRect;
            GetClientSize = getClientSize;
        }


        public void Dispose()
        {
            DropBitmap();
        }

        private void DropBitmap()
        {
            displayBitmap?.Dispose();
            displayBitmap = null;
            DisplayRect = Rectangle.Empty;
        }


        public void SetImage(Bitmap image)
        {
            if (image == null)
            {
                SetNullImage();
            }
            else
            {
                SetNonNullBitmap(image);
            }
        }

        private void SetNonNullBitmap(Bitmap newBitmap)
        {
            bool invalidateAll = true;

            DropBitmapIfFormatOrSizeChanged(newBitmap);

            var createNewBitmap = (displayBitmap == null);

            if(createNewBitmap)
            {
                CreateNewBitmapFromBitmap(newBitmap);
            }
            else
            {
                CopyBitmapToExistingBitmap(newBitmap);
                invalidateAll = false;
            }

            RecalculateRenderRect(displayMode: this.mode);

            if (invalidateAll)
            {
                InvalidateAll();
            }
            else
            {
                InvalidateRect(Rectangle.Round(DisplayRect));
            }
        }


        private void CopyBitmapToExistingBitmap(Bitmap image)
        {
            Rectangle rect = new Rectangle(0, 0, displayBitmap.Width, displayBitmap.Height);

            var existingBitmapData = displayBitmap.LockBits(rect, System.Drawing.Imaging.ImageLockMode.WriteOnly, image.PixelFormat);
            var newBitmapData = image.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadOnly, image.PixelFormat);

            try
            {
                var bytesToCopy = newBitmapData.Stride * newBitmapData.Height;

                unsafe
                {
                    Buffer.MemoryCopy(
                        source: existingBitmapData.Scan0.ToPointer(),
                        destination: newBitmapData.Scan0.ToPointer(),
                        destinationSizeInBytes: bytesToCopy,
                        sourceBytesToCopy: bytesToCopy);
                }
            }
            finally
            {
                image.UnlockBits(newBitmapData);
                displayBitmap.UnlockBits(existingBitmapData);
            }
        }


        private void CreateNewBitmapFromBitmap(Bitmap newBitmap)
        {
            displayBitmap = (Bitmap)newBitmap.Clone();
        }


        private void SetNullImage()
        {
            DropBitmap();
            InvalidateAll();
        }


        private void DropBitmapIfFormatOrSizeChanged(Bitmap image)
        {
            if (displayBitmap == null) { return; }

            var doesExistingBitmapFormatMatchNewBitmapFormat =
                (displayBitmap.PixelFormat == image.PixelFormat) &&
                (displayBitmap.Size == image.Size);

            if (!doesExistingBitmapFormatMatchNewBitmapFormat)
            {
                DropBitmap();
            }
        }


        private void RecalculateRenderRect(ImageDisplayMode displayMode)
        {
            if (displayBitmap != null)
            {
                DisplayRect = ImageDisplayMaths.CalcRenderRect(
                    displayMode,
                    imageSize: displayBitmap.Size,
                    displaySize: GetClientSize(),
                    existingRenderRect: DisplayRect);
            }
            else
            {
                DisplayRect = Rectangle.Empty;
            }
        }



        public void SetNewRenderRect(RectangleF newRenderRect)
        {
            if ((displayBitmap == null) || (DisplayRect == newRenderRect)) { return; }

            DisplayRect = newRenderRect;
            InvalidateAll();
        }


        public PointF? DisplayLocationFromImageLocation(PointF imageLocation)
        {
            if (!AnythingToDisplay) { return null; }

            var drawingLocation = ImageDisplayMaths.DisplayLocationFromImageLocation(
                imageLocation: imageLocation,
                imageSize: displayBitmap.Size,
                renderRect: DisplayRect);

            return drawingLocation;
        }


        public PointF? ImageLocationFromDisplayLocation(PointF displayLocation)
        {
            if (!AnythingToDisplay) { return null; }

            var imageLocation = ImageDisplayMaths.DisplayLocationFromImageLocation(
                imageLocation: displayLocation,
                imageSize: displayBitmap.Size,
                renderRect: DisplayRect);

            return imageLocation;
        }


        /// <summary>
        /// Centre the image and set to 1:1 zoom. Only applies if the 
        /// mode is <see cref="ImageDisplayMode.Free"/>, otherwise does 
        /// nothing.
        /// </summary>
        public void ActualSizeCentred()
        {
            if (mode != ImageDisplayMode.Free) { return; }

            var originalDisplayRect = DisplayRect;

            RecalculateRenderRect(displayMode: ImageDisplayMode.ActualSizeCentred);

            if (originalDisplayRect != DisplayRect)
            {
                InvalidateAll();
            }
        }

        public void Centre()
        {
            if (mode != ImageDisplayMode.Free) { return; }

            var originalDisplayRect = DisplayRect;

            if (displayBitmap != null)
            {
                DisplayRect = ImageDisplayMaths.CalcCentredRect(
                    displaySize: GetClientSize(),
                    existingRect: DisplayRect,
                    imageSize: displayBitmap.Size);
            }
            else
            {
                DisplayRect = Rectangle.Empty;
            }

            if (originalDisplayRect != DisplayRect)
            {
                InvalidateAll();
            }
        }


        public void FitToWindow()
        {
            if(mode != ImageDisplayMode.Free) { return; }

            var originalDisplayRect = DisplayRect;

            RecalculateRenderRect(displayMode: ImageDisplayMode.FitToWindowCentred);

            if (originalDisplayRect != DisplayRect)
            {
                InvalidateAll();
            }
        }

        public void OnClientSizeChanged()
        {
            RecalculateRenderRect(displayMode: this.mode);
            InvalidateAll();
        }
    }
}
