using System.Collections;
using System.Collections.Generic;
using System.Drawing;

namespace CDS.Imaging.WinFormsTests
{
    /// <summary>
    /// A data provider for the <see cref="FitToWindow_Maximumise_DisplayArea(Size, Size, RectangleF)"/>
    /// test. We need this because inline params for an xUnit Theory must be constant and we 
    /// want to pass non-const Size and RectangleF data to the test
    /// </summary>
    public class FitToWindowSampleData : IEnumerable<object[]>
    {
        object[] SquareImageInLargeWideDisplay => new object[]
        {
                new Size(200, 200), // image size
                new Size(1000, 500), // display size
                new RectangleF(250, 0, 500, 500) // fit-to-window paint rect
        };

        object[] SquareImageInLargeTallDisplay => new object[]
        {
                new Size(200, 200), // image size
                new Size(500, 1000), // display size
                new RectangleF(0, 250, 500, 500) // fit-to-window paint rect
        };

        object[] SquareImageInSmallWideDisplay => new object[]
        {
                new Size(200, 200), // image size
                new Size(100, 50), // display size
                new RectangleF(25, 0, 50, 50) // fit-to-window paint rect
        };

        object[] SquareImageInSmallTallDisplay => new object[]
        {
                new Size(200, 200), // image size
                new Size(50, 100), // display size
                new RectangleF(0, 25, 50, 50) // fit-to-window paint rect
        };

        /// <summary>
        /// There was a bug with this image and display, y was going to -0.5
        /// </summary>
        object[] RealWorldWasMiscalculated => new object[]
        {
            new Size(5518, 3104), // image size
            new Size(2560, 1417), // display size
            new RectangleF(20.5f, 0, 2519, 1417) // fit-to-window paint rect
        };


        public IEnumerator<object[]> GetEnumerator()
        {
            yield return SquareImageInLargeWideDisplay;
            yield return SquareImageInLargeTallDisplay;
            yield return SquareImageInSmallWideDisplay;
            yield return SquareImageInSmallTallDisplay;
            yield return RealWorldWasMiscalculated;
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
