using System.ComponentModel;
using System.Drawing;

namespace CDS.Imaging.Demo.DemoForms.MultipleROIs;


class MyROIDescriptor : WinForms.RegionOfInterest.ISingleROIDescriptor, INotifyPropertyChanged
{
    private WinForms.RegionOfInterest.SingleROIDescriptor coreDescriptor = new WinForms.RegionOfInterest.SingleROIDescriptor();


    public void Dispose()
    {
        coreDescriptor.Dispose();
    }

    public bool Locked 
    { 
        get => coreDescriptor.Locked;
       
        set
        {
            coreDescriptor.Locked = value;
            OnPropertyChanged(nameof(Locked));
        }
    }
    public bool Visible
    {
        get => coreDescriptor.Visible;
    
        set
        {
            coreDescriptor.Visible = value;
            OnPropertyChanged(nameof(Visible));
        }
    }

    public Size MaximumSize { get => coreDescriptor.MaximumSize; set => coreDescriptor.MaximumSize = value; }
    public Size MinimumSize { get => coreDescriptor.MinimumSize; set => coreDescriptor.MinimumSize = value; }
    public string Name { get => coreDescriptor.Name; set => coreDescriptor.Name = value; }

    public WinForms.RegionOfInterest.RectangleRenderer Renderer => coreDescriptor.Renderer;

    public Rectangle ROI { get => coreDescriptor.ROI; set => coreDescriptor.ROI = value; }


    void WinForms.RegionOfInterest.ISingleROIDescriptor.Draw(Graphics graphics, WinForms.BitmapDisplay.BitmapDisplayPanel bitmapDisplay, Rectangle roiOnImage)
    {
        if (!Visible) { return; } 

        coreDescriptor.Draw(graphics, bitmapDisplay, roiOnImage);

        var locationOnDisplay = bitmapDisplay.MapImagePointToDisplayPoint(roiOnImage.Location);
        locationOnDisplay.Offset(0, -12);
        graphics.DrawString(Name, SystemFonts.DefaultFont, Brushes.Yellow, locationOnDisplay);
    }

    public override string ToString() => coreDescriptor.ToString();

    
    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
