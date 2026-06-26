using System.Drawing;


namespace CDS.ImageDisplay.WinForms.BitmapDisplay;


/// <summary>
/// Maps coordinates from image space to a target rendering surface.
/// </summary>
/// <remarks>
/// Implementations apply zoom, pan, and pixel-alignment for on-screen rendering
/// (<see cref="BitmapDisplayPanel"/>), or pass coordinates through unchanged for
/// off-screen rendering such as saving to a <see cref="System.Drawing.Bitmap"/>.
/// </remarks>
public interface ICoordinateMapper
{
    /// <summary>
    /// Maps a point from image coordinates to target surface coordinates.
    /// </summary>
    PointF MapPoint(PointF point, DisplayPixelAlign pixelAdjust);

    /// <summary>
    /// Maps a rectangle from image coordinates to target surface coordinates.
    /// </summary>
    RectangleF MapRect(RectangleF rect, DisplayPixelAlign pixelAdjust);

    /// <summary>
    /// Maps a scalar distance from image units to target surface units.
    /// </summary>
    float MapDistance(float distance);
}
