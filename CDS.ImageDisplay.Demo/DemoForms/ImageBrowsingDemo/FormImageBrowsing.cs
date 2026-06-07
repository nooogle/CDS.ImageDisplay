using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;
using CDS.ImageDisplay.ImageBrowsing;

namespace CDS.ImageDisplay.Demo.DemoForms.ImageBrowsingDemo;

/// <summary>
/// Demo of <see cref="ImageListPanel"/> — browse a folder of images with thumbnail previews.
/// On load, sample images are generated in a temporary folder so the demo works out of the box.
/// </summary>
internal sealed partial class FormImageBrowsing : Form
{
    private string _sampleFolder = string.Empty;

    /// <summary>Initializes a new instance of <see cref="FormImageBrowsing"/>.</summary>
    public FormImageBrowsing()
    {
        InitializeComponent();
    }

    /// <summary>Generate sample images and populate the list on startup.</summary>
    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);
        nudThumbnailSize.Value = imageListPanel.ThumbnailHeight;
        _sampleFolder = CreateSampleImages();
        txtFolder.Text = _sampleFolder;
        imageListPanel.SetFolder(_sampleFolder);
    }

    /// <summary>Keep the display panel fitted to its current size.</summary>
    protected override void OnSizeChanged(EventArgs e)
    {
        base.OnSizeChanged(e);
        bitmapDisplayPanel.FitToWindowCentred();
    }

    private void btnBrowse_Click(object sender, EventArgs e)
    {
        using var dlg = new FolderBrowserDialog
        {
            Description = "Select a folder containing images",
            SelectedPath = txtFolder.Text,
            UseDescriptionForTitle = true,
        };

        if (dlg.ShowDialog(this) != DialogResult.OK) { return; }

        txtFolder.Text = dlg.SelectedPath;
        imageListPanel.SetFolder(dlg.SelectedPath);
        bitmapDisplayPanel.SetImage((Bitmap?)null);
        labelStatus.Text = "Select an image from the list";
    }

    private void btnPrevious_Click(object sender, EventArgs e) => imageListPanel.MoveToPrevious();

    private void btnNext_Click(object sender, EventArgs e) => imageListPanel.MoveToNext();

    private void nudThumbnailSize_ValueChanged(object sender, EventArgs e) =>
        imageListPanel.ThumbnailHeight = (int)nudThumbnailSize.Value;

    private void imageListPanel_SelectionChanged(object sender, ImageFileEventArgs e) => LoadImage(e.FilePath);

    private void LoadImage(string filePath)
    {
        try
        {
            using var bmp = new Bitmap(filePath);
            bitmapDisplayPanel.SetImage(bmp);
            bitmapDisplayPanel.FitToWindowCentred();
            labelStatus.Text = Path.GetFileName(filePath);
        }
        catch (Exception ex)
        {
            labelStatus.Text = $"Error loading image: {ex.Message}";
        }
    }

    private static string CreateSampleImages()
    {
        string folder = Path.Combine(Path.GetTempPath(), "CDS.ImageDisplay.Demo.ImageBrowsing");
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
            using var bmp = MakeSampleBitmap(name, size, background);
            bmp.Save(path, ImageFormat.Png);
        }

        return folder;
    }

    private static Bitmap MakeSampleBitmap(string label, Size size, Color background)
    {
        var bmp = new Bitmap(size.Width, size.Height, PixelFormat.Format32bppArgb);
        using var g = Graphics.FromImage(bmp);
        g.Clear(background);

        // Grid lines
        using var gridPen = new Pen(Color.FromArgb(40, 255, 255, 255), 1);
        for (int x = 0; x < size.Width; x += 50) { g.DrawLine(gridPen, x, 0, x, size.Height); }
        for (int y = 0; y < size.Height; y += 50) { g.DrawLine(gridPen, 0, y, size.Width, y); }

        // Border
        using var borderPen = new Pen(Color.FromArgb(80, 255, 255, 255), 3);
        g.DrawRectangle(borderPen, 4, 4, size.Width - 8, size.Height - 8);

        // Centred label
        using var font = new Font("Segoe UI", Math.Max(12, Math.Min(size.Width, size.Height) / 8f), FontStyle.Bold);
        using var brush = new SolidBrush(Color.FromArgb(200, 255, 255, 255));
        var textSize = g.MeasureString(label, font);
        g.DrawString(label, font, brush,
            (size.Width - textSize.Width) / 2f,
            (size.Height - textSize.Height) / 2f);

        // Dimension label at bottom-right
        using var smallFont = new Font("Segoe UI", 9);
        using var dimBrush = new SolidBrush(Color.FromArgb(160, 255, 255, 255));
        string dims = $"{size.Width}×{size.Height}";
        var dimsSize = g.MeasureString(dims, smallFont);
        g.DrawString(dims, smallFont, dimBrush,
            size.Width - dimsSize.Width - 6,
            size.Height - dimsSize.Height - 4);

        return bmp;
    }


    /// <summary>
    /// Update the time display every 100ms to show that the UI remains responsive while loading images.
    /// </summary>
    private void timerTime_Tick(object sender, EventArgs e)
    {
        labelTime.Text = DateTime.Now.ToString("HH:mm:ss.fff");
    }
}
