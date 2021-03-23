using System.Drawing;

namespace CDS.Imaging.WinForms
{
    public interface IBitmapDisplay
    {
        RectangleF PaintRect { get; } 
        Image Image { get; }
        bool IsDisplayingImage { get; }
        BitmapDisplayMode Mode { get; set; }
        BitmapDisplayMetrics TimingMetrics { get; }

        event PaintOverEvent PaintOver;

        PointF MapImageToDisplay(PointF imageLocation);
        RectangleF MapImageToDisplay(RectangleF imageLocation);
        PointF MapDisplayToImage(PointF displayLocation);
        RectangleF MapDisplayToImage(RectangleF displayLocation);
        void SetImage(Bitmap image);
        void FitToWindowCentred();
        void ActualSizeCentred();
        void Centre();
    }
}