using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using CDS.ImageDisplay.WinForms.ImageBrowsing;

namespace CDS.ImageDisplay.WinForms.Demo.DemoForms.ImageBrowsingDemo;

/// <summary>
/// Demonstrates <see cref="ImageListPanel.StatusProvider"/> with randomised per-item statuses.
/// Click "Randomise" to shuffle which items are Approved, Rejected, Under review, or unsettled.
/// </summary>
internal sealed partial class FormImageBrowsingStatus : Form
{
    // The three named status states — colour, badge, and human label.
    private static readonly (Color Color, string Badge, string Label)[] s_statuses =
    [
        (Color.LightGreen,  "OK",  "Approved"),
        (Color.LightCoral,  "ERR", "Rejected"),
        (Color.LightYellow, "REV", "Review"),
    ];

    private readonly Dictionary<string, ImageItemStatus?> _statusMap = [];
    private readonly Random _random = new();
    private string _sampleFolder = string.Empty;

    /// <summary>Initializes a new instance of <see cref="FormImageBrowsingStatus"/>.</summary>
    public FormImageBrowsingStatus()
    {
        InitializeComponent();
    }

    /// <summary>Generate sample images, wire up the status provider, and populate the list on startup.</summary>
    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);
        nudThumbnailSize.Value = imageListPanel.ThumbnailHeight;
        BuildLegend();
        _sampleFolder = SampleImages.CreateSampleFolder();
        txtFolder.Text = _sampleFolder;
        imageListPanel.StatusProvider = GetStatus;
        imageListPanel.SetFolder(_sampleFolder);
        RandomiseAndRefresh();
    }

    /// <summary>Keep the display panel fitted to its current size.</summary>
    protected override void OnSizeChanged(EventArgs e)
    {
        base.OnSizeChanged(e);
        bitmapDisplayPanel.FitToWindowCentred();
    }

    private ImageItemStatus? GetStatus(string filePath) =>
        _statusMap.TryGetValue(filePath, out var status) ? status : null;

    private void BuildLegend()
    {
        panelLegend.SuspendLayout();

        AddLegendLabel("Legend:", SystemColors.Control, false);

        foreach (var (color, badge, label) in s_statuses)
        {
            AddLegendLabel($" [{badge}] {label} ", color, true);
            AddLegendLabel("  ", SystemColors.Control, false);
        }

        AddLegendLabel("no colour = no status (25 % chance)", SystemColors.Control, false);

        panelLegend.ResumeLayout();
    }

    private void AddLegendLabel(string text, Color backColor, bool border)
    {
        panelLegend.Controls.Add(new Label
        {
            Text = text,
            AutoSize = true,
            BackColor = backColor,
            BorderStyle = border ? BorderStyle.FixedSingle : BorderStyle.None,
            Margin = new Padding(border ? 4 : 0, 3, 0, 0),
        });
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
        _statusMap.Clear();
        imageListPanel.SetFolder(dlg.SelectedPath);
        bitmapDisplayPanel.SetImage((Bitmap?)null);
        labelStatus.Text = "Select an image from the list";
        RandomiseAndRefresh();
    }

    private void btnRandomise_Click(object sender, EventArgs e) => RandomiseAndRefresh();

    private void btnPrevious_Click(object sender, EventArgs e) => imageListPanel.MoveToPrevious();

    private void btnNext_Click(object sender, EventArgs e) => imageListPanel.MoveToNext();

    private void nudThumbnailSize_ValueChanged(object sender, EventArgs e) =>
        imageListPanel.ThumbnailHeight = (int)nudThumbnailSize.Value;

    private void imageListPanel_SelectionChanged(object sender, ImageFileEventArgs e) => LoadImage(e.FilePath);

    private void RandomiseAndRefresh()
    {
        _statusMap.Clear();

        foreach (string file in imageListPanel.GetAllFilePaths())
        {
            // One in four items gets no status; the other three pick uniformly from s_statuses.
            int pick = _random.Next(4);
            if (pick > 0)
            {
                var (color, badge, _) = s_statuses[pick - 1];
                _statusMap[file] = new ImageItemStatus(color, badge);
            }
        }

        imageListPanel.RefreshStatuses();
    }

    private void LoadImage(string filePath)
    {
        try
        {
            using var bmp = new Bitmap(filePath);
            bitmapDisplayPanel.SetImage(bmp);
            bitmapDisplayPanel.FitToWindowCentred();

            string statusDesc = _statusMap.TryGetValue(filePath, out var status) && status is not null
                ? $"{Path.GetFileName(filePath)}  [{status.BadgeText}]"
                : Path.GetFileName(filePath);

            labelStatus.Text = statusDesc;
        }
        catch (Exception ex)
        {
            labelStatus.Text = $"Error loading image: {ex.Message}";
        }
    }

    private void timerTime_Tick(object sender, EventArgs e)
    {
        labelTime.Text = DateTime.Now.ToString("HH:mm:ss.fff");
    }
}
