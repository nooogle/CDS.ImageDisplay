using System;
using System.Windows.Forms;

namespace CDS.ImageDisplay.WinForms.Demo.DemoForms.MultipleROIs;


/// <summary>
/// Form for demonstrating the ROISelectionOnBitmapDisplay
/// </summary>
internal sealed partial class FormMultipleROIs : Form
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
#pragma warning disable CA1031 // Broad catch is intentional: any image-load failure should be shown to the user
        catch (Exception exception)
        {
            MessageBox.Show(
                exception.Message,
                "Error loading image",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }
#pragma warning restore CA1031
    }

    private void multipleROIManager_OnCommittedROIChanged(object sender, CDS.ImageDisplay.WinForms.RegionOfInterest.CommittedROIDescriptorChangedEventArgs e)
    {
        if (e.ROIDescriptor is not MyROIDescriptor myDesciptor)
        { throw new InvalidOperationException("ROI descriptor is not of the expected type"); }

        myDesciptor.ChangeCount++;
        bitmapDisplayPanel.Invalidate();
    }
}
