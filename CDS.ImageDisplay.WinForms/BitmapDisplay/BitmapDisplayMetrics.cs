using System;

namespace CDS.Imaging.BitmapDisplay;

/// <summary>
/// Timing metrics
/// </summary>
public class BitmapDisplayMetrics
{
    /// <summary>
    /// Time taken to paint the foreground, including any overlays
    /// </summary>
    public TimeSpan ForegroundPaint { get; set; }


    /// <summary>
    /// Time taken to paint the background
    /// </summary>
    public TimeSpan BackgroundPaint { get; set; }
}
