using System.Drawing;

namespace CDS.Imaging.WinForms.BitmapDisplay
{
    public delegate void PaintOverEvent(BitmapDisplay sender, Graphics graphics, Size imageSize, RectangleF renderRect);
}
