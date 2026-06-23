using System;
using System.Collections.Generic;
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
    private GreyscalePaletteMode _greyscalePaletteMode = GreyscalePaletteMode.Standard;

    private static readonly Dictionary<GreyscalePaletteMode, Color[]> s_paletteCache = [];


    /// <summary>
    /// The image
    /// </summary>
    public Bitmap? Image { get; private set; }


    /// <summary>
    /// The size of the image, or Size.Empty if there isn't an image
    /// </summary>
    public Size ImageSize => (Image == null) ? Size.Empty : Image.Size;


    /// <summary>
    /// Palette mode applied to 8bpp indexed images. Changing this re-applies
    /// the palette to the current bitmap immediately.
    /// </summary>
    public GreyscalePaletteMode GreyscalePaletteMode
    {
        get => _greyscalePaletteMode;

        set
        {
            if (_greyscalePaletteMode != value)
            {
                _greyscalePaletteMode = value;
                ApplyGreyscalePaletteIf8bpp();
            }
        }
    }


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
                if (Image.PixelFormat == PixelFormat.Format8bppIndexed)
                {
                    ApplyGreyscalePaletteIf8bpp();
                }

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

        if (imageSource.PixelFormat == PixelFormat.Format8bppIndexed)
        {
            ApplyGreyscalePaletteIf8bpp();
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
        BitmapData destBits = Image.LockBits(roi, ImageLockMode.WriteOnly, Image.PixelFormat);

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


    /// <summary>
    /// Applies the current <see cref="GreyscalePaletteMode"/> to <see cref="Image"/>
    /// if it is an 8bpp indexed bitmap. No-op otherwise.
    /// </summary>
    private void ApplyGreyscalePaletteIf8bpp()
    {
        if (Image == null || Image.PixelFormat != PixelFormat.Format8bppIndexed)
        {
            return;
        }

        Color[] colors = GetPaletteForMode(_greyscalePaletteMode);
        ColorPalette palette = Image.Palette;

        for (int i = 0; i < 256; i++)
        {
            palette.Entries[i] = colors[i];
        }

        Image.Palette = palette;
    }


    /// <summary>
    /// Returns a cached 256-entry colour array for the given mode, building it on first access.
    /// </summary>
    private static Color[] GetPaletteForMode(GreyscalePaletteMode mode)
    {
        if (s_paletteCache.TryGetValue(mode, out Color[]? cached))
        {
            return cached;
        }

        Color[] palette = mode switch
        {
            GreyscalePaletteMode.Standard => BuildPalette([0, 255], [Color.Black, Color.White]),
            GreyscalePaletteMode.Inverted => BuildPalette([0, 255], [Color.White, Color.Black]),
            GreyscalePaletteMode.HighlightSaturated => BuildPalette(
                [0, 254, 255],
                [Color.Black, Color.FromArgb(254, 254, 254), Color.Red]),
            _ => BuildPalette([0, 255], [Color.Black, Color.White]),
        };

        s_paletteCache[mode] = palette;
        return palette;
    }


    /// <summary>
    /// Builds a 256-entry palette by linearly interpolating between the supplied colour stops.
    /// <paramref name="indices"/> and <paramref name="stops"/> must be the same length,
    /// with the first index being 0 and the last being 255.
    /// </summary>
    private static Color[] BuildPalette(int[] indices, Color[] stops)
    {
        var palette = new Color[256];

        for (int segment = 0; segment < indices.Length - 1; segment++)
        {
            int startIdx = indices[segment];
            int endIdx = indices[segment + 1];
            double range = endIdx - startIdx;

            double startR = stops[segment].R;
            double startG = stops[segment].G;
            double startB = stops[segment].B;

            double deltaR = stops[segment + 1].R - startR;
            double deltaG = stops[segment + 1].G - startG;
            double deltaB = stops[segment + 1].B - startB;

            for (int i = startIdx; i <= endIdx; i++)
            {
                double t = (i - startIdx) / range;
                palette[i] = Color.FromArgb(
                    (int)(startR + t * deltaR),
                    (int)(startG + t * deltaG),
                    (int)(startB + t * deltaB));
            }
        }

        return palette;
    }
}
