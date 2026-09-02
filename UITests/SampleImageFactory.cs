using System.Drawing.Imaging;

namespace UITests;

/// <summary>
/// Generates a folder of solid-colour PNGs for UI-automation tests. Colours are spread around the
/// hue wheel at the golden angle so consecutive images are visually distinct from each other and,
/// deliberately, from the placeholder tile's <see cref="Color.LightGray"/>.
/// </summary>
/// <remarks>
/// Content is trivial (a flat fill) so pixel-sampling stays cheap and unambiguous, but
/// <paramref name="imageSize"/> defaults large enough to force realistic decode latency —
/// <see cref="ImageBrowsing.ImageListPanel"/> loads thumbnails one at a time
/// (<c>LoadRangeAsync</c> awaits each <c>Task.Run</c> sequentially), so with small/fast synthetic
/// images a whole page loads before a scroll-triggered debounce cycle has any real chance to be
/// interrupted mid-flight by another one — the opposite of a folder of full-size photos.
/// </remarks>
internal static class SampleImageFactory
{
    private const double GoldenAngleDegrees = 137.508;

    /// <summary>Creates <paramref name="count"/> images in a fresh temp folder and returns its path.</summary>
    public static string CreateFolder(int count, int imageSize = 4000)
    {
        string folder = Path.Combine(Path.GetTempPath(), "CDS.ImageDisplay.UITests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(folder);

        for (int i = 0; i < count; i++)
        {
            using var bmp = new Bitmap(imageSize, imageSize, PixelFormat.Format32bppArgb);
            using (var g = Graphics.FromImage(bmp))
            {
                g.Clear(ColorFromIndex(i));
            }

            bmp.Save(Path.Combine(folder, $"img_{i:D4}.png"), ImageFormat.Png);
        }

        return folder;
    }

    private static Color ColorFromIndex(int index)
    {
        double hue = index * GoldenAngleDegrees % 360;
        return ColorFromHsv(hue, saturation: 0.85, value: 0.85);
    }

    private static Color ColorFromHsv(double hue, double saturation, double value)
    {
        int hueSector = (int)(hue / 60) % 6;
        double fraction = hue / 60 - Math.Floor(hue / 60);

        int v = (int)(value * 255);
        int p = (int)(value * (1 - saturation) * 255);
        int q = (int)(value * (1 - fraction * saturation) * 255);
        int t = (int)(value * (1 - (1 - fraction) * saturation) * 255);

        return hueSector switch
        {
            0 => Color.FromArgb(v, t, p),
            1 => Color.FromArgb(q, v, p),
            2 => Color.FromArgb(p, v, t),
            3 => Color.FromArgb(p, q, v),
            4 => Color.FromArgb(t, p, v),
            _ => Color.FromArgb(v, p, q),
        };
    }
}
