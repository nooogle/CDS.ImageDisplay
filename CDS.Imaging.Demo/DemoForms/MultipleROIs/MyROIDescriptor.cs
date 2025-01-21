using CDS.Imaging.WinForms.BitmapDisplay;
using System.ComponentModel;
using System.Drawing;

namespace CDS.Imaging.Demo.DemoForms.MultipleROIs;


class MyROIDescriptor : WinForms.RegionOfInterest.ISingleROIDescriptor, INotifyPropertyChanged
{
    public WinForms.RegionOfInterest.ROIWithGrapplesShape CoreShape { get; } = new WinForms.RegionOfInterest.ROIWithGrapplesShape();

    public bool Locked 
    { 
        get => CoreShape.Locked;
       
        set
        {
            CoreShape.Locked = value;
            OnPropertyChanged(nameof(Locked));
        }
    }
    public bool Visible
    {
        get => CoreShape.Visible;
    
        set
        {
            CoreShape.Visible = value;
            OnPropertyChanged(nameof(Visible));
        }
    }

    public Size MaximumSize { get => CoreShape.MaximumSize; set => CoreShape.MaximumSize = value; }
    public Size MinimumSize { get => CoreShape.MinimumSize; set => CoreShape.MinimumSize = value; }
    public string Name { get => CoreShape.Name; set => CoreShape.Name = value; }
    public Rectangle ROI { get => CoreShape.ROI; set => CoreShape.ROI = value; }
    public DisplayPixelAlign PixelAlign { get => CoreShape.PixelAlign; set => CoreShape.PixelAlign = value; }


    public MyROIDescriptor()
    {
        CoreShape.Rendering = new WinForms.Draw.RenderingSpec();
    }

    void WinForms.Draw.Shapes.IShapeOverlay.Draw(WinForms.BitmapDisplay.BitmapDisplayPanel bitmapDisplay, Graphics graphics)
    {
        if (!Visible) { return; }

        CoreShape.Draw(bitmapDisplay, graphics);

        var locationOnDisplay = bitmapDisplay.MapImageToDisplay(ROI.Location, WinForms.BitmapDisplay.DisplayPixelAlign.TopLeft);
        locationOnDisplay.Offset(0, -12);
        graphics.DrawString(Name, SystemFonts.DefaultFont, Brushes.Yellow, locationOnDisplay);
    }

    public override string ToString() => CoreShape.ToString();

    
    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
