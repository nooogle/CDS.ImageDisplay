using System.Drawing;

namespace CDS.Imaging.WinForms.BitmapDisplay
{
    /// <summary>
    /// Callback delegate for getting an image
    /// </summary>
    /// <param name="sender">Sender of the callback</param>
    /// <returns>An image or null if none is available</returns>
    public delegate Image GetImageCallback(IBitmapDisplay sender);


    /// <summary>
    /// Callback delegate
    /// </summary>
    /// <param name="sender">Callback sender</param>
    /// <param name="paintRect">New paint rectangle</param>
    public delegate void OnPaintRectChangedCallback(VirtualDisplay sender, RectangleF paintRect);


    /// <summary>
    /// Event delegate
    /// </summary>
    /// <param name="sender">Event sender</param>
    /// <param name="graphics">For drawing</param>
    public delegate void PaintOverEvent(BitmapDisplayPanel sender, Graphics graphics);


    /// <summary>
    /// Event delegate
    /// </summary>
    /// <param name="sender">Event sender</param>
    /// <param name="graphics">For drawing</param>
    public delegate void PaintUnderEvent(BitmapDisplayPanel sender, Graphics graphics);


    /// <summary>
    /// Event delegate
    /// </summary>
    /// <param name="sender">Event sender</param>
    public delegate void ModeChangedEvent(BitmapDisplayPanel sender);
}
