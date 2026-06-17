using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using CDS.ImageDisplay.WinForms.ImageBrowsing;

namespace CDS.ImageDisplay.WinForms.Demo.DemoForms.ImageBrowsingDemo;

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
        _sampleFolder = SampleImages.CreateSampleFolder();
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
        };
#if NET6_0_OR_GREATER
        dlg.UseDescriptionForTitle = true;
#endif

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

    /// <summary>
    /// Update the time display every 100ms to show that the UI remains responsive while loading images.
    /// </summary>
    private void timerTime_Tick(object sender, EventArgs e)
    {
        labelTime.Text = DateTime.Now.ToString("HH:mm:ss.fff");
    }
}
