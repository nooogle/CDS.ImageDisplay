using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Humanizer;

namespace CDS.ImageDisplay.WinForms.Demo.DemoForms;

/// <summary>
/// Form for demonstrating the single line selection manager.
/// </summary>
internal sealed partial class FormLineSelectionDetailed : Form
{
    private sealed class TestProperties
    {
        [Category("WinForms controls")]
        [DisplayName("Bitmap display")]
        public BitmapDisplay.BitmapDisplayPanel BitmapDisplayPanel { get; }

        [Category("WinForms controls")]
        [DisplayName("Line selection")]
        public LineSelection.SingleLineSelectionManager SingleLineSelectionManager { get; }

        /// <summary>
        /// Initialise the test properties.
        /// </summary>
        public TestProperties(
            BitmapDisplay.BitmapDisplayPanel bitmapDisplayPanel,
            LineSelection.SingleLineSelectionManager singleLineSelectionManager)
        {
            BitmapDisplayPanel = bitmapDisplayPanel;
            SingleLineSelectionManager = singleLineSelectionManager;
        }
    }

    private readonly TestProperties _testProperties;

    /// <summary>
    /// Initializes a new instance of the <see cref="FormLineSelectionDetailed"/> class.
    /// </summary>
    public FormLineSelectionDetailed()
    {
        InitializeComponent();
        _testProperties = new TestProperties(bitmapDisplayPanel, singleLineSelectionManager);
    }

    /// <summary>
    /// Setup after the form has loaded.
    /// </summary>
    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);
        using var generatedBitmap = BitmapGenerator.Make(new Size(800, 600));
        bitmapDisplayPanel.SetImage(generatedBitmap);
        UpdateLineLabels();

        propertyGrid.SelectedObject = _testProperties;
    }

    /// <summary>
    /// The form has been resized, so fit the bitmap display to the window.
    /// </summary>
    protected override void OnSizeChanged(EventArgs e)
    {
        base.OnSizeChanged(e);
        bitmapDisplayPanel.FitToWindowCentred();
    }

    private void UpdateLineLabels()
    {
        labelCommittedLine.Text = FormatLine(singleLineSelectionManager.CommittedLine);
        labelDraggingLine.Text = FormatLine(singleLineSelectionManager.LiveDraggingLine);
    }

    private static string FormatLine((Point Start, Point End)? line)
    {
        return line is null ? "<none>" : $"{line.Value.Start} -> {line.Value.End}";
    }

    /// <summary>
    /// User has clicked the Set Line button.
    /// </summary>
    private void btnSetLine_Click(object sender, EventArgs e)
    {
        singleLineSelectionManager.CommittedLine = (new Point(10, 20), new Point(200, 160));
    }

    /// <summary>
    /// User has clicked the Clear Line button.
    /// </summary>
    private void btnClearLine_Click(object sender, EventArgs e)
    {
        singleLineSelectionManager.CommittedLine = null;
        UpdateLineLabels();
    }

    /// <summary>
    /// Update the paint metrics periodically.
    /// </summary>
    private void timerUpdateMetrics_Tick(object sender, EventArgs e)
    {
        labelPaintForegroundMetrics.Text = bitmapDisplayPanel.TimingMetrics.ForegroundPaint.Humanize();
        labelPaintBackgroundMetrics.Text = bitmapDisplayPanel.TimingMetrics.BackgroundPaint.Humanize();
    }

    /// <summary>
    /// The line has been committed to the single line selection manager.
    /// </summary>
    private void singleLineSelectionManager_OnCommittedLineChanged(object sender, WinForms.LineSelection.CommittedLineChangedEventArgs e)
    {
        UpdateLineLabels();
    }

    /// <summary>
    /// The line is being dragged on the single line selection manager.
    /// </summary>
    private void singleLineSelectionManager_OnDraggingLineChanged(object sender, WinForms.LineSelection.DraggingLineChangedEventArgs e)
    {
        UpdateLineLabels();
    }
}
