using System;
using System.Drawing;
using System.Windows.Forms;

namespace CDS.ImageDisplay.Demo.DemoForms;

/// <summary>
/// Form for demonstrating the single line selection manager.
/// </summary>
internal sealed partial class FormLineSelectionSimple : Form
{
    private readonly Overlays.TextShape _committedLineText;
    private readonly Overlays.TextShape _draggingLineText;
    private readonly Overlays.DrawingSpec _textDrawingSpec;

    /// <summary>
    /// Initializes a new instance of the <see cref="FormLineSelectionSimple"/> class.
    /// </summary>
    public FormLineSelectionSimple()
    {
        InitializeComponent();

        _committedLineText = new Overlays.TextShape
        {
            Text = "Committed line",
            Location = new Point(10, 10),
        };

        _draggingLineText = new Overlays.TextShape
        {
            Text = "Dragging line",
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
    /// Setup after the form has loaded.
    /// </summary>
    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);
        using var generatedBitmap = BitmapGenerator.Make(new Size(800, 600));
        bitmapDisplayPanel.SetImage(generatedBitmap);
    }

    /// <summary>
    /// The form has been resized, so fit the bitmap display to the window.
    /// </summary>
    protected override void OnSizeChanged(EventArgs e)
    {
        base.OnSizeChanged(e);
        bitmapDisplayPanel.FitToWindowCentred();
    }

    private void singleLineSelectionManager_OnCommittedLineChanged(object sender, CDS.ImageDisplay.LineSelection.CommittedLineChangedEventArgs e)
    {
        UpdateLabels();
    }

    private void singleLineSelectionManager_OnDraggingLineChanged(object sender, CDS.ImageDisplay.LineSelection.DraggingLineChangedEventArgs e)
    {
        UpdateLabels();
    }

    private void UpdateLabels()
    {
        _committedLineText.Text = $"Committed line: {FormatLine(singleLineSelectionManager.CommittedLine)}";
        _draggingLineText.Text = $"Dragging line: {FormatLine(singleLineSelectionManager.LiveDraggingLine)}";
        bitmapDisplayPanel.Invalidate();
    }

    private static string FormatLine((Point Start, Point End)? line)
    {
        return line is null ? "<none>" : $"{line.Value.Start} -> {line.Value.End}";
    }

    private void bitmapDisplayPanel_OnPaintOver(object sender, CDS.ImageDisplay.BitmapDisplay.PaintOverEventArgs e)
    {
        ArgumentNullException.ThrowIfNull(e.Graphics);

        _committedLineText.Draw(e.Sender, e.Graphics, _textDrawingSpec);
        _draggingLineText.Draw(e.Sender, e.Graphics, _textDrawingSpec);
    }
}
