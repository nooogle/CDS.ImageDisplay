using System.ComponentModel;
using System.Drawing;
using System.Runtime.CompilerServices;

namespace CDS.ImageDisplay.Demo.DemoForms.MultipleROIs;


internal sealed class TestProperties : INotifyPropertyChanged
{
    [Category("WinForms controls")]
    [DisplayName("Bitmap display")]
    public BitmapDisplay.BitmapDisplayPanel BitmapDisplayPanel { get; }


    [Category("WinForms controls")]
    [DisplayName("Multiple ROI manager")]
    public RegionOfInterest.MultipleROIManager MultipleROIManager { get; }


    [Category("Demo form")]
    [DisplayName("ROI descriptors")]
    public BindingList<MyROIDescriptor> ROIDescriptors { get; }


    /// <summary>
    /// Initialise the test properties
    /// </summary>
    public TestProperties(
        BitmapDisplay.BitmapDisplayPanel bitmapDisplayPanel,
        RegionOfInterest.MultipleROIManager multipleROIManager)
    {
        BitmapDisplayPanel = bitmapDisplayPanel;
        MultipleROIManager = multipleROIManager;

        ROIDescriptors = CreateDefaultROIs();
        ROIDescriptors.ListChanged += (s, e) => NotifyPropertyChanged(nameof(ROIDescriptors));
    }


    public event PropertyChangedEventHandler? PropertyChanged;


    private void NotifyPropertyChanged([CallerMemberName] string propertyName = "") => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));


    /// <summary>
    /// Create some default ROIs
    /// </summary>
    private static BindingList<MyROIDescriptor> CreateDefaultROIs()
    {
        var list = new BindingList<MyROIDescriptor>
        {
            new("ROI 1")
            {
                ROI = new Rectangle(10, 10, 100, 20),
                MinimumSize = new Size(100, 20),
            },

            new("ROI 2")
            {
                ROI = new Rectangle(50, 50, 200, 100),
                MinimumSize = new Size(100, 40),
            }
        };

        list[0].CoreShape.Drawing.Fill.Color = Color.FromArgb(64, Color.AliceBlue);
        list[1].CoreShape.Drawing.Fill.Color = Color.FromArgb(64, Color.Linen);

        return list;
    }
}
