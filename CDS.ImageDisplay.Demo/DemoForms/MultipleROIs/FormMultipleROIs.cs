using System;
using System.Windows.Forms;

namespace CDS.ImageDisplay.Demo.DemoForms.MultipleROIs;


/// <summary>
/// Form for demonstrating the ROISelectionOnBitmapDisplay
/// </summary>
public partial class FormMultipleROIs : Form
{
    /// <summary>
    /// Information for the properties grid
    /// </summary>
    private readonly TestProperties testProperties;


    /// <summary>
    /// Constructor
    /// </summary>
    public FormMultipleROIs()
    {
        InitializeComponent();

        testProperties = new TestProperties(bitmapDisplayPanel, multipleROIManager);
        testProperties.PropertyChanged += (s, e) =>
            {
                multipleROIManager.RefreshSelection();
                propertyGrid.Refresh();
            };
    }


    /// <summary>
    /// Setup after the form has loaded
    /// </summary>
    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);
        bitmapDisplayPanel.SetImage(Properties.Resources.Thailand);

        propertyGrid.SelectedObject = testProperties;
        multipleROIManager.ROIDescriptors = testProperties.ROIDescriptors;
    }


    /// <summary>
    /// The form has been resized, so fit the bitmap display to the window
    /// </summary>
    protected override void OnSizeChanged(EventArgs e)
    {
        base.OnSizeChanged(e);
        bitmapDisplayPanel.FitToWindowCentred();
    }

    private void propertyGrid_PropertyValueChanged(object s, PropertyValueChangedEventArgs e) => multipleROIManager.RefreshSelection();

    private void btnLoadImage_Click(object sender, EventArgs e)
    {
        if (openFileDialog.ShowDialog(this) != DialogResult.OK)
        { return; }

        try
        {
            var newImage = System.Drawing.Image.FromFile(openFileDialog.FileName);
            bitmapDisplayPanel.SetImage((System.Drawing.Bitmap)newImage);
        }
        catch (Exception exception)
        {
            MessageBox.Show(
                exception.Message,
                "Error loading image",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }
    }

    private void multipleROIManager_OnCommittedROIChanged(object sender, CDS.ImageDisplay.RegionOfInterest.CommittedROIDescriptorChangedEventArgs e)
    {
        if (e.ROIDescriptor is not MyROIDescriptor myDesciptor)
        { throw new InvalidOperationException("ROI descriptor is not of the expected type"); }

        myDesciptor.ChangeCount++;
        bitmapDisplayPanel.Invalidate();
    }
}
