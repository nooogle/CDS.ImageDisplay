using CDS.ImageDisplay.WinForms.ImageBrowsing;

namespace CDS.ImageDisplay.WinForms.TestHarness;

/// <summary>
/// Hosts a single <see cref="ImageListPanel"/> filling the client area, pointed at a folder
/// supplied on the command line. Deliberately bare — this is a UI-automation test fixture,
/// not a demo.
/// </summary>
internal sealed class MainForm : Form
{
    private readonly ImageListPanel _imageListPanel;

    /// <summary>Initialises the form and points the panel at <paramref name="folder"/>.</summary>
    /// <param name="folder">Folder whose images the panel should list.</param>
    /// <param name="width">Restored client area width, in pixels. Ignored if <paramref name="maximize"/> is <see langword="true"/>.</param>
    /// <param name="height">Restored client area height, in pixels. Ignored if <paramref name="maximize"/> is <see langword="true"/>.</param>
    /// <param name="thumbnailHeight">Thumbnail edge length, in pixels — see <see cref="ImageListPanel.ThumbnailHeight"/>.</param>
    /// <param name="maximize">When <see langword="true"/>, the window opens maximized instead of at <paramref name="width"/>×<paramref name="height"/>.</param>
    public MainForm(string folder, int width, int height, int thumbnailHeight, bool maximize)
    {
        Name = "MainForm";
        Text = "CDS.ImageDisplay.WinForms.TestHarness";
        ClientSize = new Size(width, height);
        StartPosition = FormStartPosition.Manual;
        Location = new Point(0, 0);

        _imageListPanel = new ImageListPanel
        {
            Name = "imageListPanel",
            Dock = DockStyle.Fill,
            ThumbnailHeight = thumbnailHeight,
        };
        Controls.Add(_imageListPanel);

        Load += (_, _) => _imageListPanel.SetFolder(folder);

        if (maximize)
        {
            WindowState = FormWindowState.Maximized;
        }
    }
}
