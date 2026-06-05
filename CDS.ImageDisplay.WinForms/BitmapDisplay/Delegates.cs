using System;
using System.Drawing;

namespace CDS.ImageDisplay.BitmapDisplay;

/// <summary>
/// Callback delegate raised when the paint rectangle changes on a <see cref="VirtualDisplay"/>.
/// </summary>
/// <param name="sender">The <see cref="VirtualDisplay"/> whose paint rectangle changed.</param>
/// <param name="paintRect">The new paint rectangle.</param>
public delegate void OnPaintRectChangedCallback(VirtualDisplay sender, RectangleF paintRect);

/// <summary>
/// Provides data for the <see cref="BitmapDisplayPanel.PaintOver"/> event.
/// </summary>
public sealed class PaintOverEventArgs : EventArgs
{
    /// <summary>Gets the <see cref="BitmapDisplayPanel"/> that raised the event.</summary>
    public BitmapDisplayPanel Sender { get; }

    /// <summary>Gets the <see cref="System.Drawing.Graphics"/> object to draw with.</summary>
    public Graphics Graphics { get; }

    /// <summary>Initialises a new instance of <see cref="PaintOverEventArgs"/>.</summary>
    /// <param name="sender">The <see cref="BitmapDisplayPanel"/> that raised the event.</param>
    /// <param name="graphics">The <see cref="System.Drawing.Graphics"/> object to draw with.</param>
    public PaintOverEventArgs(BitmapDisplayPanel sender, Graphics graphics)
    {
        Sender = sender;
        Graphics = graphics;
    }
}

/// <summary>
/// Provides data for the <see cref="BitmapDisplayPanel.PaintUnder"/> event.
/// </summary>
public sealed class PaintUnderEventArgs : EventArgs
{
    /// <summary>Gets the <see cref="BitmapDisplayPanel"/> that raised the event.</summary>
    public BitmapDisplayPanel Sender { get; }

    /// <summary>Gets the <see cref="System.Drawing.Graphics"/> object to draw with.</summary>
    public Graphics Graphics { get; }

    /// <summary>Initialises a new instance of <see cref="PaintUnderEventArgs"/>.</summary>
    /// <param name="sender">The <see cref="BitmapDisplayPanel"/> that raised the event.</param>
    /// <param name="graphics">The <see cref="System.Drawing.Graphics"/> object to draw with.</param>
    public PaintUnderEventArgs(BitmapDisplayPanel sender, Graphics graphics)
    {
        Sender = sender;
        Graphics = graphics;
    }
}

/// <summary>
/// Provides data for the <see cref="BitmapDisplayPanel.PaintRectChanged"/> event.
/// </summary>
public sealed class PaintRectChangedEventArgs : EventArgs
{
    /// <summary>Gets the <see cref="BitmapDisplayPanel"/> that raised the event.</summary>
    public BitmapDisplayPanel Sender { get; }

    /// <summary>Gets the new paint rectangle.</summary>
    public RectangleF PaintRect { get; }

    /// <summary>Initialises a new instance of <see cref="PaintRectChangedEventArgs"/>.</summary>
    /// <param name="sender">The <see cref="BitmapDisplayPanel"/> that raised the event.</param>
    /// <param name="paintRect">The new paint rectangle.</param>
    public PaintRectChangedEventArgs(BitmapDisplayPanel sender, RectangleF paintRect)
    {
        Sender = sender;
        PaintRect = paintRect;
    }
}

/// <summary>
/// Provides data for the <see cref="BitmapDisplayPanel.DisplayModeChanged"/> event.
/// </summary>
public sealed class DisplayModeChangedEventArgs : EventArgs
{
    /// <summary>Gets the new display mode.</summary>
    public BitmapDisplayMode NewMode { get; }

    /// <summary>Initialises a new instance of <see cref="DisplayModeChangedEventArgs"/>.</summary>
    /// <param name="newMode">The new display mode.</param>
    public DisplayModeChangedEventArgs(BitmapDisplayMode newMode)
    {
        NewMode = newMode;
    }
}

/// <summary>
/// Provides data for the <see cref="BitmapDisplayPanel.ImageSizeChanged"/> event.
/// </summary>
public sealed class ImageSizeChangedEventArgs : EventArgs
{
    /// <summary>Gets the image size before the change.</summary>
    public Size OldSize { get; }

    /// <summary>Gets the image size after the change.</summary>
    public Size NewSize { get; }

    /// <summary>Initialises a new instance of <see cref="ImageSizeChangedEventArgs"/>.</summary>
    /// <param name="oldSize">The image size before the change.</param>
    /// <param name="newSize">The image size after the change.</param>
    public ImageSizeChangedEventArgs(Size oldSize, Size newSize)
    {
        OldSize = oldSize;
        NewSize = newSize;
    }
}

/// <summary>
/// Provides data for the <see cref="BitmapDisplayPanel.ZoomChanged"/> event.
/// </summary>
public sealed class ZoomChangedEventArgs : EventArgs
{
    /// <summary>Gets the new zoom level.</summary>
    public float Zoom { get; }

    /// <summary>Initialises a new instance of <see cref="ZoomChangedEventArgs"/>.</summary>
    /// <param name="zoom">The new zoom level.</param>
    public ZoomChangedEventArgs(float zoom)
    {
        Zoom = zoom;
    }
}
