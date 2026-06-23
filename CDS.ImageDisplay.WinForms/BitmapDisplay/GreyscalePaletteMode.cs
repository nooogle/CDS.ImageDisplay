namespace CDS.ImageDisplay.WinForms.BitmapDisplay;

/// <summary>
/// Palette modes for 8bpp indexed greyscale images.
/// </summary>
public enum GreyscalePaletteMode
{
    /// <summary>
    /// Standard greyscale: 0 maps to black, 255 maps to white.
    /// </summary>
    Standard,

    /// <summary>
    /// Inverted greyscale: 0 maps to white, 255 maps to black.
    /// </summary>
    Inverted,

    /// <summary>
    /// Greyscale with saturated pixels (value 255) highlighted in red.
    /// </summary>
    HighlightSaturated,
}
