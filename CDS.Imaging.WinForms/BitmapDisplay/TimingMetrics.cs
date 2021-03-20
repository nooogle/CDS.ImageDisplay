using System;

namespace CDS.Imaging.WinForms.BitmapDisplay
{
    public class TimingMetrics
    {
        public TimeSpan SetImage { get; set; }
        public TimeSpan ForegroundPaint { get; set; }
        public TimeSpan BackgroundPaint { get; set; }
    }
}
