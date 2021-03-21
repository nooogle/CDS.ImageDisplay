using System.Drawing;

namespace CDS.Imaging.WinForms
{
    public interface IBitmapDisplay
    {
        Image Image { get; }
        bool IsDisplayingImage { get; }
        BitmapDisplayMode Mode { get; set; }
        BitmapDisplayMetrics TimingMetrics { get; }

        event PaintOverEvent PaintOver;

        PointF? DisplayLocationFromImageLocation(PointF imageLocation);
        PointF? ImageLocationFromDisplayLocation(PointF displayLocation);
        void SetImage(Bitmap image);
        void FitToWindowCentred();
        void ActualSizeCentred();
        void Centre();
    }
}