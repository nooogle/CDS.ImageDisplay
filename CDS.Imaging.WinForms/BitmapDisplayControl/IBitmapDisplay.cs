using System.Drawing;

namespace CDS.Imaging.WinForms
{
    public interface IBitmapDisplay
    {
        RectangleF PaintRect { get; }
        Image? Image { get; }
        bool AnythingToDisplay { get; }
        BitmapDisplayMode Mode { get; set; }
        BitmapDisplayMetrics TimingMetrics { get; }

        event PaintOverEvent? PaintOver;
        event PaintUnderEvent? PaintUnder;
        event ModeEventHandler? DisplayModeChanged;

        PointF MapImageToDisplay(PointF imageLocation);
        RectangleF MapImageToDisplay(RectangleF imageLocation);
        PointF MapDisplayToImage(PointF displayLocation);
        RectangleF MapDisplayToImage(RectangleF displayLocation);
        void SetImage(Bitmap image);


        /// <summary>
        /// Centres the image on the display, retaining the existing zoom level.
        /// Only applied if the display mode is <see cref="BitmapDisplayMode.Free"/>,
        /// no-op otherwise.
        /// </summary>
        void Centre();


        /// <summary>
        /// Reset the zoom to 1:1
        /// </summary>
        void ResetZoom();


        /// <summary>
        /// Zoom in
        /// </summary>
        void ZoomIn();


        /// <summary>
        /// Zoom out
        /// </summary>
        void ZoomOut();


        PointF ImageDisplayCentre { get; set; } 
    }
}
