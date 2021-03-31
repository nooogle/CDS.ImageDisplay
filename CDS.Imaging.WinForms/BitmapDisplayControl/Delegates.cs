using System.Drawing;

namespace CDS.Imaging.WinForms
{
    public delegate void OnPaintRectChangedCallback(VirtualImageOnDisplay sender, RectangleF paintRect);
    public delegate void PaintOverEvent(BitmapDisplay sender, Graphics graphics, Size imageSize, RectangleF renderRect);
    public delegate void PaintUnderEvent(BitmapDisplay sender, Graphics graphics, Size imageSize, RectangleF renderRect);

    public delegate void ModeEventHandler(BitmapDisplay sender, BitmapDisplayModeEventArgs modeChangedArgs);
}
