using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.CompilerServices;

namespace CDS.Imaging.Demo.DemoForms.MultipleROIs;


class TestProperties : INotifyPropertyChanged
{
    [Category("WinForms controls")]
    [DisplayName("Bitmap display")]
    public WinForms.BitmapDisplay.BitmapDisplayPanel BitmapDisplayPanel { get; }


    [Category("WinForms controls")]
    [DisplayName("Multiple ROI manager")]
    public WinForms.RegionOfInterest.MultipleROIManager MultipleROIManager { get; }


    [Category("Demo form")]
    [DisplayName("ROI descriptors")]
    public BindingList<MyROIDescriptor> ROIDescriptors { get; }


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
        ROIDescriptors.ListChanged += (s, e) =>
        {
            NotifyPropertyChanged(nameof(ROIDescriptors));
            //PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ROIDescriptors)));
        };
    }


    public event PropertyChangedEventHandler? PropertyChanged;


    private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
    {
        if (PropertyChanged != null)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }


    /// <summary>
    /// Create some default ROIs
    /// </summary>
    private BindingList<MyROIDescriptor> CreateDefaultROIs()
    {
        return new BindingList<MyROIDescriptor>
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
