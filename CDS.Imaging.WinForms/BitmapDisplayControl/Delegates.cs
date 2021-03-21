using System.Drawing;

namespace CDS.Imaging.WinForms
{
    public delegate void PaintOverEvent(BitmapDisplay sender, Graphics graphics, Size imageSize, RectangleF renderRect);
}
