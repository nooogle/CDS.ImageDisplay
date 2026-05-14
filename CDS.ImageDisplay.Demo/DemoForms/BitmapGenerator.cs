using System.Drawing;
using System.Drawing.Drawing2D;

namespace CDS.ImageDisplay.Demo.DemoForms;


/// <summary>
/// Utility to generate a bitmap for the overlay demo
/// </summary>
static class BitmapGenerator
{
    public static Bitmap Make(Size imageSize)
    {
        Bitmap bitmap = new Bitmap(imageSize.Width, imageSize.Height, format: System.Drawing.Imaging.PixelFormat.Format32bppArgb);
        FillBackground(imageSize.Width, imageSize.Height, bitmap);
        DrawGrid(imageSize.Width, imageSize.Height, bitmap);
        return bitmap;
    }

    private static void DrawGrid(int width, int height, Bitmap bitmap)
    {
        // Draw a grid, like grid paper, with 10*10 squares, using LightGrey, and using DarkGrey for every 10th grid line
        using (Graphics g = Graphics.FromImage(bitmap))
        {
            using Pen pen10 = new Pen(Color.FromArgb(64, 0, 64, 0), 1);
            using Pen pen100 = new Pen(Color.FromArgb(64, 0, 128, 0), 1);

            for (int x = 0; x < width; x += 10)
            {
                g.DrawLine((x % 100 == 0) ? pen100 : pen10, x, 0, x, height);
            }

            for (int y = 0; y < height; y += 10)
            {
                g.DrawLine((y % 100 == 0) ? pen100 : pen10, 0, y, width, y);
            }
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
        using (Graphics g = Graphics.FromImage(bitmap))
        {
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
}
