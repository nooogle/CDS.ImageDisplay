using System;
using System.Windows.Forms;

namespace CDS.Imaging.Demo.DemoForms.MultipleROIs;


/// <summary>
/// Form for demonstrating the ROISelectionOnBitmapDisplay
/// </summary>
public partial class FormMultipleROIs : Form
{
    /// <summary>
    /// Information for the properties grid
    /// </summary>
    private TestProperties testProperties;


    /// <summary>
    /// Constructor
    /// </summary>
    public FormMultipleROIs()
    {
        InitializeComponent();

        testProperties = new TestProperties(bitmapDisplayPanel, multipleROIManagerOnBitmapDisplay);
    }


    /// <summary>
    /// Setup after the form has loaded
    /// </summary>
    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);
        bitmapDisplayPanel.SetImage(Properties.Resources.Thailand);

        propertyGrid.SelectedObject = testProperties;
        multipleROIManagerOnBitmapDisplay.GetROIDescriptors = () => testProperties.ROIDescriptors;
    }


    /// <summary>
    /// The form has been resized, so fit the bitmap display to the window
    /// </summary>
    protected override void OnSizeChanged(EventArgs e)
    {
        base.OnSizeChanged(e);
        bitmapDisplayPanel.FitToWindowCentred();
    }
}
