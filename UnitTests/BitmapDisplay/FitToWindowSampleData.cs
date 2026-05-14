using System.Collections;
using System.Drawing;

namespace CDS.ImageDisplay.WinFormsTests.BitmapDisplay;

/// <summary>
/// Provides sample data for fit-to-window layout scenarios.
/// </summary>
public class FitToWindowSampleData
{
    public static IEnumerable<object[]> Data
    {
        get
        {
            // Square image in large wide display.
            yield return
            [
                new Size(200, 200),
                new Size(1000, 500),
                new RectangleF(250, 0, 500, 500)
            ];

            // Square image in large tall display.
            yield return
            [
                new Size(200, 200),
                new Size(500, 1000),
                new RectangleF(0, 250, 500, 500)
            ];

            // Square image in small wide display.
            yield return
            [
                new Size(200, 200),
                new Size(100, 50),
                new RectangleF(25, 0, 50, 50)
            ];

            // Square image in small tall display.
            yield return
            [
                new Size(200, 200),
                new Size(50, 100),
                new RectangleF(0, 25, 50, 50)
            ];

            // Real-world case that previously produced a negative Y offset.
            yield return
            [
                new Size(5518, 3104),
                new Size(2560, 1417),
                new RectangleF(20.5f, 0, 2519, 1417)
            ];
        }
    }
}
