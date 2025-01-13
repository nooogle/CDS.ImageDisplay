using Humanizer;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace CDS.Imaging.Demo.DemoForms;


/// <summary>
/// Form for demonstrating the ROISelectionOnBitmapDisplay
/// </summary>
public partial class FormMultipleROIs : Form
{
    class TestProperties
    {
        [Category("WinForms controls")]
        [DisplayName("Bitmap display")]
        public WinForms.BitmapDisplay.BitmapDisplayPanel BitmapDisplayPanel { get; }


        [Category("WinForms controls")]
        [DisplayName("Multiple ROI manager")]
        public WinForms.RegionOfInterest.MultipleROIManagerOnBitmapDisplay MultipleROIManager { get; }


        [Category("Demo form")]
        [DisplayName("ROI descriptors")]
        public List<WinForms.RegionOfInterest.MultipleROIManagerOnBitmapDisplay.ROIDescriptor> ROIDescriptors { get; }


        /// <summary>
        /// Initialise the test properties
        /// </summary>
        public TestProperties(
            WinForms.BitmapDisplay.BitmapDisplayPanel bitmapDisplayPanel,
            WinForms.RegionOfInterest.MultipleROIManagerOnBitmapDisplay multipleROIManager)
        {
            BitmapDisplayPanel = bitmapDisplayPanel;
            MultipleROIManager = multipleROIManager;

            ROIDescriptors = CreateDefaultROIs();
        }


        /// <summary>
        /// Create some default ROIs
        /// </summary>
        private List<WinForms.RegionOfInterest.MultipleROIManagerOnBitmapDisplay.ROIDescriptor> CreateDefaultROIs()
        {
            return new List<WinForms.RegionOfInterest.MultipleROIManagerOnBitmapDisplay.ROIDescriptor>
            {
                new WinForms.RegionOfInterest.MultipleROIManagerOnBitmapDisplay.ROIDescriptor()
                {
                    Name = "ROI 1",
                    ROI = new Rectangle(10, 10, 100, 20),
                },

                new WinForms.RegionOfInterest.MultipleROIManagerOnBitmapDisplay.ROIDescriptor()
                {
                    Name = "ROI 2",
                    ROI = new Rectangle(50, 50, 200, 100),
                }
            };
        }
    }


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
