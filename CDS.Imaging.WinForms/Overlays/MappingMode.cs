namespace CDS.Imaging.Overlays;


/// <summary>
/// Specifies how the image should be mapped to the display.
/// </summary>
public enum MappingMode
{
    /// <summary>
    /// Pixel coordinadates are mapped to the display. For a 1:1 zoom factor, these are mapping
    /// without scaling. For other zoom factors, the coordinates are scaled so that they remain
    /// proportional to the original image size.
    /// </summary>
    ImageToDisplay,


    /// <summary>
    /// Pixel coordinates are mapped directly to the display. This means that display zoom
    /// level does not affect the size of the coordinates.
    /// </summary>
    DirectToDisplay,
}
