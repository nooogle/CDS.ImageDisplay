using System.Drawing;

namespace CDS.Imaging.WinForms.BitmapDisplay
{
    public interface IBitmapDisplay
    {
        Image Image { get; }
        bool IsDisplayingImage { get; }
        ImageDisplayMode Mode { get; set; }
        TimingMetrics TimingMetrics { get; }

        event PaintOverEvent PaintOver;

        PointF DisplayLocationFrom(PointF imageLocation);
        PointF ImageLocationFrom(PointF displayLocation);
        void SetImage(Bitmap image);
        void FitToWindowCentred();
        void ActualSizeCentred();
        void Centre();
    }
}