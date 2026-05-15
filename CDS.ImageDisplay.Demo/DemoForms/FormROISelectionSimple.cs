using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace CDS.ImageDisplay.Demo.DemoForms;


/// <summary>
/// Form for demonstrating the singleROIManager
/// </summary>
internal sealed partial class FormROISelectionSimple : Form
{
    private readonly Overlays.TextShape _committedROIText;
    private readonly Overlays.TextShape _draggingROIText;
    private readonly Overlays.DrawingSpec _textDrawingSpec;

    /// <summary>
    /// Constructor
    /// </summary>
    public FormROISelectionSimple()
    {
        InitializeComponent();

        _committedROIText = new Overlays.TextShape
        {
            Text = "Committed ROI",
            Location = new Point(10, 10),
        };

        _draggingROIText = new Overlays.TextShape
        {
            Text = "Dragging ROI",
            Location = new Point(10, 30),
        };

        _textDrawingSpec = new Overlays.DrawingSpec
        {
            Fill =
            {
                Color = Color.Yellow,
            },
            MappingMode = Overlays.MappingMode.DirectToDisplay,
        };

        UpdateLabels();
    }

    /// <summary>
    /// Setup after the form has loaded
    /// </summary>
    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);
        using var generatedBitmap = BitmapGenerator.Make(new Size(800, 600));
        bitmapDisplayPanel.SetImage(generatedBitmap);
    }

    /// <summary>
    /// The form has been resized, so fit the bitmap display to the window
    /// </summary>
    protected override void OnSizeChanged(EventArgs e)
    {
        base.OnSizeChanged(e);
        bitmapDisplayPanel.FitToWindowCentred();
    }

    /// <summary>
    /// The ROI has been committed to the singleROIManager
    /// </summary>
    private void singleROIManager_OnCommittedROIChanged(object sender, CDS.ImageDisplay.RegionOfInterest.CommittedROIChangedEventArgs e)
    {
        UpdateLabels();
    }

    /// <summary>
    /// The ROI is being dragged on the singleROIManager
    /// </summary>
    private void singleROIManager_OnDraggingROIChanged(object sender, CDS.ImageDisplay.RegionOfInterest.DraggingROIChangedEventArgs e)
    {
        UpdateLabels();
    }

    private void UpdateLabels()
    {
        _committedROIText.Text = $"Committed ROI: {singleROIManager.CommittedROI}";
        _draggingROIText.Text = $"Dragging ROI: {singleROIManager.LiveDraggingROI}";
        bitmapDisplayPanel.Invalidate();
    }

    private void bitmapDisplayPanel_OnPaintOver(object sender, CDS.ImageDisplay.BitmapDisplay.PaintOverEventArgs e)
    {
        ArgumentNullException.ThrowIfNull(e.Graphics);
        ArgumentNullException.ThrowIfNull(sender);

        if (sender is not CDS.ImageDisplay.BitmapDisplay.BitmapDisplayPanel bitmapDisplayPanel)
        {
            throw new ArgumentException("Sender must be a BitmapDisplayPanel.", nameof(sender));
        }

        _committedROIText.Draw(bitmapDisplayPanel, e.Graphics, _textDrawingSpec);
        _draggingROIText.Draw(bitmapDisplayPanel, e.Graphics, _textDrawingSpec);
    }
}
