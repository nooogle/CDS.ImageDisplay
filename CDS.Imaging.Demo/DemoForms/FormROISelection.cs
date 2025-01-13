using Humanizer;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace CDS.Imaging.Demo.DemoForms;


/// <summary>
/// Form for demonstrating the ROISelectionOnBitmapDisplay
/// </summary>
public partial class FormROISelection : Form
{
    class TestProperties
    {
        [Category("WinForms controls")]
        [DisplayName("Bitmap display")]
        public WinForms.BitmapDisplay.BitmapDisplayPanel BitmapDisplayPanel { get; }


        [Category("WinForms controls")]
        [DisplayName("ROI selection")]
        public WinForms.RegionOfInterest.ROISelectionOnBitmapDisplay ROISelectionOnBitmapDisplay { get; }


        [Category("Demo form")]
        [DisplayName("Committed ROI size limit")]
        public Size CommittedROISizeLimit { get; set; } = new Size(1000000, 1000000);


        /// <summary>
        /// Initialise the test properties
        /// </summary>
        public TestProperties(
            WinForms.BitmapDisplay.BitmapDisplayPanel bitmapDisplayPanel,
            WinForms.RegionOfInterest.ROISelectionOnBitmapDisplay roiSelectionOnBitmapDisplay)
        {
            BitmapDisplayPanel = bitmapDisplayPanel;
            ROISelectionOnBitmapDisplay = roiSelectionOnBitmapDisplay;
        }
    }


    /// <summary>
    /// Information for the properties grid
    /// </summary>
    private TestProperties testProperties;


    /// <summary>
    /// Constructor
    /// </summary>
    public FormROISelection()
    {
        InitializeComponent();

        testProperties = new TestProperties(bitmapDisplayPanel, roiSelectionOnBitmapDisplay);
    }


    /// <summary>
    /// Setup after the form has loaded
    /// </summary>
    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);
        bitmapDisplayPanel.SetImage(Properties.Resources.Thailand);
        UpdateROILabels();

        propertyGrid.SelectedObject = testProperties;
    }


    /// <summary>
    /// Update the ROI labels on the form
    /// </summary>
    private void UpdateROILabels()
    {
        labelCommittedROI.Text = $"Committed ROI: {roiSelectionOnBitmapDisplay.CommittedROI}";
        labelDraggingROI.Text = $"Dragging ROI: {roiSelectionOnBitmapDisplay.LiveDraggingROI}";
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
    /// User has clicked the Set ROI button
    /// </summary>
    private void btnSetROI_Click(object sender, EventArgs e)
    {
        roiSelectionOnBitmapDisplay.CommittedROI = new Rectangle(10, 20, 100, 200);
    }


    /// <summary>
    /// User has clicked the Clear ROI button
    /// </summary>
    private void btnClearROI_Click(object sender, EventArgs e)
    {
        roiSelectionOnBitmapDisplay.CommittedROI = Rectangle.Empty;
    }


    /// <summary>
    /// Update the paint metrics every second
    /// </summary>
    private void timerUpdateMetrics_Tick(object sender, EventArgs e)
    {
        labelPaintForegroundMetrics.Text = bitmapDisplayPanel.TimingMetrics.ForegroundPaint.Humanize();
        labelPaintBackgroundMetrics.Text = bitmapDisplayPanel.TimingMetrics.BackgroundPaint.Humanize();
    }


    /// <summary>
    /// The ROI has been committed to the ROISelectionOnBitmapDisplay
    /// </summary>
    private void roiSelectionOnBitmapDisplay_OnCommittedROIChanged(CDS.Imaging.WinForms.RegionOfInterest.ROISelectionOnBitmapDisplay sender, Rectangle roi)
    {
        var sizeLimitedROI = new Rectangle(
            roi.Location.X,
            roi.Location.Y,
            Math.Min(roi.Width, testProperties.CommittedROISizeLimit.Width),
            Math.Min(roi.Height, testProperties.CommittedROISizeLimit.Height));

        if (roi != sizeLimitedROI)
        {
            roiSelectionOnBitmapDisplay.CommittedROI = sizeLimitedROI;
        }

        UpdateROILabels();
    }


    /// <summary>
    /// The ROI is being dragged on the ROISelectionOnBitmapDisplay
    /// </summary>
    private void roiSelectionOnBitmapDisplay_OnDraggingROIChanged(CDS.Imaging.WinForms.RegionOfInterest.ROISelectionOnBitmapDisplay sender, Rectangle roi)
    {
        UpdateROILabels();
    }
}
