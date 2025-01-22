using CDS.Imaging.BitmapDisplay;
using System.Drawing;

namespace CDS.Imaging.Draw;


/// <summary>
/// Interface for a shape
/// </summary>
public interface IShape
{
    /// <summary>
    /// Gets or sets the visibility of the shape
    /// </summary>
    bool Visible { get; set; }


    /// <summary>
    /// Controls where line end points are rendered. Only has an effect when
    /// the image is zoomed in such that more than one display pixel corresponds to a single image pixel.
    /// </summary>
    DisplayPixelAlign PixelAlign { get; set; }


    /// <summary>
    /// Draws the shape on the display
    /// </summary>
    void Draw(BitmapDisplayPanel sender, Graphics graphics);
}
