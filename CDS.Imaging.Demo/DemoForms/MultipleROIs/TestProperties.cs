using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;

namespace CDS.Imaging.Demo.DemoForms.MultipleROIs;


class TestProperties
{
    [Category("WinForms controls")]
    [DisplayName("Bitmap display")]
    public WinForms.BitmapDisplay.BitmapDisplayPanel BitmapDisplayPanel { get; }


    [Category("WinForms controls")]
    [DisplayName("Multiple ROI manager")]
    public WinForms.RegionOfInterest.MultipleROIManager MultipleROIManager { get; }


    [Category("Demo form")]
    [DisplayName("ROI descriptors")]
    public List<MyROIDescriptor> ROIDescriptors { get; }


    /// <summary>
    /// Initialise the test properties
    /// </summary>
    public TestProperties(
        WinForms.BitmapDisplay.BitmapDisplayPanel bitmapDisplayPanel,
        WinForms.RegionOfInterest.MultipleROIManager multipleROIManager)
    {
        BitmapDisplayPanel = bitmapDisplayPanel;
        MultipleROIManager = multipleROIManager;

        ROIDescriptors = CreateDefaultROIs();
    }


    /// <summary>
    /// Create some default ROIs
    /// </summary>
    private List<MyROIDescriptor> CreateDefaultROIs()
    {
        return new List<MyROIDescriptor>
            {
                new MyROIDescriptor()
                {
                    Name = "ROI 1",
                    ROI = new Rectangle(10, 10, 100, 20),
                    MinimumSize = new Size(100, 20),
                },

                new MyROIDescriptor()
                {
                    Name = "ROI 2",
                    ROI = new Rectangle(50, 50, 200, 100),
                    MinimumSize = new Size(100, 40),
                }
            };
    }
}
