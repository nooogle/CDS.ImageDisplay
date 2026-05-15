using System;
using System.Drawing;

namespace CDS.ImageDisplay.BitmapDisplay;

/// <summary>
/// Callback delegate for getting an image.
/// </summary>
/// <param name="sender">Sender of the callback.</param>
/// <returns>An image, or <see langword="null"/> if none is available.</returns>
public delegate Image GetImageCallback(BitmapDisplayPanel sender);

/// <summary>
/// Callback delegate raised when the paint rectangle changes on a <see cref="VirtualDisplay"/>.
/// </summary>
/// <param name="sender">The <see cref="VirtualDisplay"/> whose paint rectangle changed.</param>
/// <param name="paintRect">The new paint rectangle.</param>
public delegate void OnPaintRectChangedCallback(VirtualDisplay sender, RectangleF paintRect);

/// <summary>
/// Provides data for the <see cref="BitmapDisplayPanel.OnPaintOver"/> event.
/// </summary>
public sealed class PaintOverEventArgs : EventArgs
{
    /// <summary>Gets the <see cref="System.Drawing.Graphics"/> object to draw with.</summary>
    public Graphics Graphics { get; }

    /// <summary>Initialises a new instance of <see cref="PaintOverEventArgs"/>.</summary>
    /// <param name="graphics">The <see cref="System.Drawing.Graphics"/> object to draw with.</param>
    public PaintOverEventArgs(Graphics graphics)
    {
        Graphics = graphics;
    }
}

/// <summary>
/// Provides data for the <see cref="BitmapDisplayPanel.OnPaintUnder"/> event.
/// </summary>
public sealed class PaintUnderEventArgs : EventArgs
{
    /// <summary>Gets the <see cref="System.Drawing.Graphics"/> object to draw with.</summary>
    public Graphics Graphics { get; }

    /// <summary>Initialises a new instance of <see cref="PaintUnderEventArgs"/>.</summary>
    /// <param name="graphics">The <see cref="System.Drawing.Graphics"/> object to draw with.</param>
    public PaintUnderEventArgs(Graphics graphics)
    {
        Graphics = graphics;
    }
}

/// <summary>
/// Provides data for the <see cref="BitmapDisplayPanel.OnImageSizeChanged"/> event.
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
