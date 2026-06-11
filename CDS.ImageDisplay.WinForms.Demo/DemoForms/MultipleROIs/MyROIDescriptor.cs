using System.ComponentModel;
using System.Drawing;
using CDS.ImageDisplay.WinForms.BitmapDisplay;

namespace CDS.ImageDisplay.WinForms.Demo.DemoForms.MultipleROIs;


internal sealed class MyROIDescriptor : WinForms.RegionOfInterest.ISingleROIDescriptor, INotifyPropertyChanged
{
    private readonly string name;

    public int ChangeCount { get; set; }

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

    public string Name
    {
        get => $"{name}, {ChangeCount} changes";
        set { }
    }


    public Rectangle ROI { get => CoreShape.ROI; set => CoreShape.ROI = value; }


    public DisplayPixelAlign PixelAlign { get => CoreShape.PixelAlign; set => CoreShape.PixelAlign = value; }

    public Overlays.DrawingSpec CustomLabelDrawingSpec { get; } = new WinForms.Overlays.DrawingSpec()
    {
        Fill = new WinForms.Overlays.BrushSpec()
        {
            Color = Color.Yellow,
        },
    };

    public MyROIDescriptor(string name)
    {
        this.name = name;
        CoreShape.Drawing = new WinForms.Overlays.DrawingSpec();
        CoreShape.GrapplesVisible = false;
    }


    void WinForms.RegionOfInterest.ISingleROIDescriptor.Draw(WinForms.BitmapDisplay.BitmapDisplayPanel bitmapDisplay, Graphics graphics)
    {
        if (!Visible)
        { return; }

        CoreShape.Draw(bitmapDisplay, graphics);

        Point locationOnDisplay = bitmapDisplay.MapImageToDisplay(ROI.Location, WinForms.BitmapDisplay.DisplayPixelAlign.TopLeft);
        locationOnDisplay.Offset(0, -12);

        Font font = Overlays.DrawingToolsPool.GetFont(CustomLabelDrawingSpec.Font);
        Brush brush = Overlays.DrawingToolsPool.GetBrush(CustomLabelDrawingSpec.Fill);
        graphics.DrawString(Name, font, brush, locationOnDisplay);
    }


    public override string ToString() => CoreShape.ToString();


    public event PropertyChangedEventHandler? PropertyChanged;


    private void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}
