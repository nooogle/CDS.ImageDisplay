using System.Drawing;


namespace CDS.ImageDisplay.WinForms.BitmapDisplay;


/// <summary>
/// An <see cref="ICoordinateMapper"/> that returns all coordinates unchanged.
/// Use this when rendering shapes directly onto a <see cref="System.Drawing.Bitmap"/>
/// where image coordinates map 1:1 to bitmap pixels with no zoom or pan.
/// </summary>
public sealed class IdentityCoordinateMapper : ICoordinateMapper
{
    /// <summary>The shared singleton instance.</summary>
    public static readonly IdentityCoordinateMapper Instance = new();

    private IdentityCoordinateMapper() { }

    /// <inheritdoc/>
    public PointF MapPoint(PointF point, DisplayPixelAlign pixelAdjust) => point;

    /// <inheritdoc/>
    public RectangleF MapRect(RectangleF rect, DisplayPixelAlign pixelAdjust) => rect;

    /// <inheritdoc/>
    public float MapDistance(float distance) => distance;
}
