using System;
using System.Drawing;
using System.Windows.Forms;
using CDS.ImageDisplay.WinForms.Overlays;

namespace CDS.ImageDisplay.WinForms.Demo.DemoForms;

/// <summary>
/// Demonstrates <see cref="CDS.ImageDisplay.WinForms.BitmapDisplay.BitmapDisplayPanel.MapImageToDisplayScaleFactor"/>.
/// Both panels share the same overlay rectangle defined in full-size image coordinates.
/// The right panel displays a quarter-size copy of the image and has
/// MapImageToDisplayScaleFactor = 0.25, so the overlay maps correctly onto it.
/// </summary>
internal sealed partial class FormScaleFactorDemo : Form
{
    private const float c_scaleFactor = 0.25f;

    private static readonly RectangleShape s_overlayRect = new()
    {
        Rect = new RectangleF(160, 130, 220, 250),
    };

    private static readonly DrawingSpec s_overlaySpec = new()
    {
        Lines = new PenSpec { Color = Color.Yellow, Width = 3 },
        Fill = new BrushSpec { Color = Color.Transparent },
        MappingMode = MappingMode.ImageToDisplay,
    };

    private Bitmap? _fullImage;
    private Bitmap? _quarterImage;

    /// <summary>
    /// Initialises a new instance of <see cref="FormScaleFactorDemo"/>.
    /// </summary>
    public FormScaleFactorDemo()
    {
        InitializeComponent();
    }

    /// <inheritdoc/>
    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);
        _fullImage = BitmapGenerator.MakeBlobDetectionDemo(new Size(640, 480));
        _quarterImage = BitmapGenerator.ScaleDown(_fullImage, c_scaleFactor);
        _panelFull.SetImage(_fullImage);
        _panelQuarter.MapImageToDisplayScaleFactor = c_scaleFactor;
        _panelQuarter.SetImage(_quarterImage);
    }

    /// <inheritdoc/>
    protected override void OnShown(EventArgs e)
    {
        base.OnShown(e);
        _panelFull.FitToWindowCentred();
        _panelQuarter.FitToWindowCentred();
    }

    /// <inheritdoc/>
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _fullImage?.Dispose();
            _quarterImage?.Dispose();
            components?.Dispose();
        }
        base.Dispose(disposing);
    }

    private void PanelFull_PaintOver(object sender, BitmapDisplay.PaintOverEventArgs e)
        => s_overlayRect.Draw(e.Sender, e.Graphics, s_overlaySpec);

    private void PanelQuarter_PaintOver(object sender, BitmapDisplay.PaintOverEventArgs e)
        => s_overlayRect.Draw(e.Sender, e.Graphics, s_overlaySpec);
}
