using CDS.Imaging.BitmapDisplay;
using System.ComponentModel;
using System.Drawing;

namespace CDS.Imaging.Demo.DemoForms.MultipleROIs;


class MyROIDescriptor : RegionOfInterest.ISingleROIDescriptor, INotifyPropertyChanged
{
    private string name;

    public int ChangeCount {  get; set; }

    public RegionOfInterest.ROIWithGrapplesShape CoreShape { get; } = new RegionOfInterest.ROIWithGrapplesShape();

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
    
    public string Name
    {
        get => $"{name}, {ChangeCount} changes";
        set { }
    }
    
    public Rectangle ROI { get => CoreShape.ROI; set => CoreShape.ROI = value; }
    public DisplayPixelAlign PixelAlign { get => CoreShape.PixelAlign; set => CoreShape.PixelAlign = value; }


    public MyROIDescriptor(string name)
    {
        this.name = name;
        CoreShape.Rendering = new Draw.RenderingSpec();
        CoreShape.GrapplesVisible = false;
    }

    void Draw.IShape.Draw(BitmapDisplay.BitmapDisplayPanel bitmapDisplay, Graphics graphics)
    {
        if (!Visible) { return; }

        CoreShape.Draw(bitmapDisplay, graphics);

        var locationOnDisplay = bitmapDisplay.MapImageToDisplay(ROI.Location, BitmapDisplay.DisplayPixelAlign.TopLeft);
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
