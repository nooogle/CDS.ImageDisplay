using System.Drawing;

namespace CDS.Imaging.Demo.DemoForms.MultipleROIs;


class MyROIDescriptor : WinForms.RegionOfInterest.ISingleROIDescriptor
{
    private WinForms.RegionOfInterest.SingleROIDescriptor coreDescriptor = new WinForms.RegionOfInterest.SingleROIDescriptor();


    public void Dispose()
    {
        coreDescriptor.Dispose();
    }

    public Size MaximumSize { get => coreDescriptor.MaximumSize; set => coreDescriptor.MaximumSize = value; }
    public Size MinimumSize { get => coreDescriptor.MinimumSize; set => coreDescriptor.MinimumSize = value; }
    public string Name { get => coreDescriptor.Name; set => coreDescriptor.Name = value; }

    public WinForms.RegionOfInterest.RectangleRenderer Renderer => coreDescriptor.Renderer;

    public Rectangle ROI { get => coreDescriptor.ROI; set => coreDescriptor.ROI = value; }


    void WinForms.RegionOfInterest.ISingleROIDescriptor.Draw(Graphics graphics, WinForms.BitmapDisplay.BitmapDisplayPanel bitmapDisplay, Rectangle roiOnImage)
    {
        coreDescriptor.Draw(graphics, bitmapDisplay, roiOnImage);

        var locationOnDisplay = bitmapDisplay.MapImagePointToDisplayPoint(roiOnImage.Location);
        locationOnDisplay.Offset(0, -12);
        graphics.DrawString(Name, SystemFonts.DefaultFont, Brushes.Yellow, locationOnDisplay);
    }

    public override string ToString() => coreDescriptor.ToString();
}
