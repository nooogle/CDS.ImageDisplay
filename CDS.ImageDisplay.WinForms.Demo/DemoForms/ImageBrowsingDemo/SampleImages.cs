using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace CDS.ImageDisplay.WinForms.Demo.DemoForms.ImageBrowsingDemo;

/// <summary>
/// Generates the shared set of sample images used by image-browsing demos.
/// Writing to a fixed temp folder means both demos share the same files
/// and the folder is created on first call, regenerated each run.
/// </summary>
internal static class SampleImages
{
    private const string FolderName = "CDS.ImageDisplay.Demo.ImageBrowsing";

    internal static string CreateSampleFolder()
    {
        string folder = Path.Combine(Path.GetTempPath(), FolderName);
        Directory.CreateDirectory(folder);

        (string name, Size size, Color background)[] samples =
        [
            ("Landscape",  new Size(800, 500), Color.SteelBlue),
            ("Portrait",   new Size(400, 600), Color.SeaGreen),
            ("Square",     new Size(500, 500), Color.Goldenrod),
            ("Wide",       new Size(1200, 300), Color.IndianRed),
            ("Tall",       new Size(300, 900), Color.MediumPurple),
            ("Small",      new Size(200, 150), Color.DarkSlateGray),
        ];

        foreach (var (name, size, background) in samples)
        {
            string path = Path.Combine(folder, $"{name}.png");
            using var bmp = MakeBitmap(name, size, background);
            bmp.Save(path, ImageFormat.Png);
        }

        return folder;
    }

    private static Bitmap MakeBitmap(string label, Size size, Color background)
    {
        var bmp = new Bitmap(size.Width, size.Height, PixelFormat.Format32bppArgb);
        using var g = Graphics.FromImage(bmp);
        g.Clear(background);

        using var gridPen = new Pen(Color.FromArgb(40, 255, 255, 255), 1);
        for (int x = 0; x < size.Width; x += 50) { g.DrawLine(gridPen, x, 0, x, size.Height); }
        for (int y = 0; y < size.Height; y += 50) { g.DrawLine(gridPen, 0, y, size.Width, y); }

        using var borderPen = new Pen(Color.FromArgb(80, 255, 255, 255), 3);
        g.DrawRectangle(borderPen, 4, 4, size.Width - 8, size.Height - 8);

        using var font = new Font("Segoe UI", Math.Max(12, Math.Min(size.Width, size.Height) / 8f), FontStyle.Bold);
        using var brush = new SolidBrush(Color.FromArgb(200, 255, 255, 255));
        var textSize = g.MeasureString(label, font);
        g.DrawString(label, font, brush,
            (size.Width - textSize.Width) / 2f,
            (size.Height - textSize.Height) / 2f);

        using var smallFont = new Font("Segoe UI", 9);
        using var dimBrush = new SolidBrush(Color.FromArgb(160, 255, 255, 255));
        string dims = $"{size.Width}×{size.Height}";
        var dimsSize = g.MeasureString(dims, smallFont);
        g.DrawString(dims, smallFont, dimBrush,
            size.Width - dimsSize.Width - 6,
            size.Height - dimsSize.Height - 4);

        return bmp;
    }
}
