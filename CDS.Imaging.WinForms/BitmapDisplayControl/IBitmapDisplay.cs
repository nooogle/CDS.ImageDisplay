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

        PointF? DisplayCoordsFromImage(PointF imageLocation);
        PointF? ImageCoordsFromDisplay(PointF displayLocation);
        void SetImage(Bitmap image);
        void FitToWindowCentred();
        void ActualSizeCentred();
        void Centre();
    }
}