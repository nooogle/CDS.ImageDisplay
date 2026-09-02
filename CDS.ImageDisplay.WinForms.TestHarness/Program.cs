namespace CDS.ImageDisplay.WinForms.TestHarness;

/// <summary>
/// Entry point for a minimal host app used by UI-automation tests (see the UITests project).
/// </summary>
/// <remarks>
/// Not shipped — this exists purely so FlaUI can drive a real <see cref="ImageBrowsing.ImageListPanel"/>
/// in its own process. Command-line usage:
/// <c>CDS.ImageDisplay.WinForms.TestHarness.exe &lt;folder&gt; [width] [height] [thumbnailHeight] [maximize]</c>.
/// <c>folder</c> is required. <c>width</c>/<c>height</c> set the restored window size (ignored if
/// <c>maximize</c> is <c>true</c>); <c>thumbnailHeight</c> sets the panel's thumbnail size —
/// a maximized window with small thumbnails packs many more rows per page, which is more
/// representative of the field report than the small default window.
/// </remarks>
internal static class Program
{
    [STAThread]
    private static void Main(string[] args)
    {
        if (args.Length < 1)
        {
            throw new ArgumentException(
                "Usage: CDS.ImageDisplay.WinForms.TestHarness.exe <folder> [width] [height] [thumbnailHeight] [maximize]");
        }

        string folder = args[0];
        int width = args.Length > 1 ? int.Parse(args[1]) : 360;
        int height = args.Length > 2 ? int.Parse(args[2]) : 320;
        int thumbnailHeight = args.Length > 3 ? int.Parse(args[3]) : 48;
        bool maximize = args.Length > 4 && bool.Parse(args[4]);

        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        Application.Run(new MainForm(folder, width, height, thumbnailHeight, maximize));
    }
}
