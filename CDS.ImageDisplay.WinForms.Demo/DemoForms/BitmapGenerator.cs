using System;
using System.Drawing;
using System.Runtime.InteropServices;

namespace CDS.ImageDisplay.WinForms.Demo.DemoForms;


/// <summary>
/// Utility to generate bitmaps for demo forms
/// </summary>
internal static class BitmapGenerator
{
    /// <summary>
    /// Creates an 8bpp greyscale image designed to showcase greyscale palette modes.
    /// Contains a smooth diagonal gradient background (0..220) plus two Gaussian hotspots
    /// that saturate to 255, making the HighlightSaturated palette mode clearly visible.
    /// </summary>
    public static Bitmap Make8bppSaturationDemo()
    {
        const int width = 480;
        const int height = 280;
        const float cx1 = 270f, cy1 = 110f, sigma1 = 52f, amp1 = 300f;
        const float cx2 = 390f, cy2 = 195f, sigma2 = 30f, amp2 = 250f;
        const float totalRange = width + height;

        var bitmap = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format8bppIndexed);

        var palette = bitmap.Palette;
        for (int i = 0; i < 256; i++)
        {
            palette.Entries[i] = Color.FromArgb(i, i, i);
        }
        bitmap.Palette = palette;

        var bitmapData = bitmap.LockBits(
            new Rectangle(0, 0, width, height),
            System.Drawing.Imaging.ImageLockMode.WriteOnly,
            System.Drawing.Imaging.PixelFormat.Format8bppIndexed);

        var buffer = new byte[bitmapData.Stride * height];

        for (int y = 0; y < height; y++)
        {
            int rowOffset = y * bitmapData.Stride;
            for (int x = 0; x < width; x++)
            {
                float background = 5f + (x + y) / totalRange * 210f;

                float dx1 = x - cx1, dy1 = y - cy1;
                float g1 = amp1 * (float)Math.Exp(-(dx1 * dx1 + dy1 * dy1) / (2f * sigma1 * sigma1));

                float dx2 = x - cx2, dy2 = y - cy2;
                float g2 = amp2 * (float)Math.Exp(-(dx2 * dx2 + dy2 * dy2) / (2f * sigma2 * sigma2));

                float value = background + g1 + g2;
                buffer[rowOffset + x] = (byte)Math.Max(0, Math.Min(255, (int)value));
            }
        }

        Marshal.Copy(buffer, 0, bitmapData.Scan0, buffer.Length);
        bitmap.UnlockBits(bitmapData);
        return bitmap;
    }


    public static Bitmap Make(Size imageSize)
    {
        var bitmap = new Bitmap(imageSize.Width, imageSize.Height, format: System.Drawing.Imaging.PixelFormat.Format32bppArgb);
        FillBackground(imageSize.Width, imageSize.Height, bitmap);
        DrawGrid(imageSize.Width, imageSize.Height, bitmap);
        return bitmap;
    }

    private static void DrawGrid(int width, int height, Bitmap bitmap)
    {
        // Draw a grid, like grid paper, with 10*10 squares, using LightGrey, and using DarkGrey for every 10th grid line
        using var g = Graphics.FromImage(bitmap);
        using var pen10 = new Pen(Color.FromArgb(64, 0, 64, 0), 1);
        using var pen100 = new Pen(Color.FromArgb(64, 0, 128, 0), 1);

        for (int x = 0; x < width; x += 10)
        {
            g.DrawLine((x % 100 == 0) ? pen100 : pen10, x, 0, x, height);
        }

        for (int y = 0; y < height; y += 10)
        {
            g.DrawLine((y % 100 == 0) ? pen100 : pen10, 0, y, width, y);
        }
    }

    private static void FillBackground(int width, int height, Bitmap bitmap)
    {
        //// fill with dark green
        //using (Graphics g = Graphics.FromImage(bitmap))
        //{
        //    g.Clear(Color.FromArgb(255, 0, 12, 0));
        //}

        // fill with drak green, and every alternate pixel light green
        using var g = Graphics.FromImage(bitmap);
        g.Clear(Color.FromArgb(255, 0, 12, 0));

        using Brush brush = new SolidBrush(Color.FromArgb(255, 0, 24, 0));

        for (int x = 0; x < width; x += 2)
        {
            for (int y = 0; y < height; y += 2)
            {
                g.FillRectangle(brush, x, y, 1, 1);
            }
        }
    }
}
