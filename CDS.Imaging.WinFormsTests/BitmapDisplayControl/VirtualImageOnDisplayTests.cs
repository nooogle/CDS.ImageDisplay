using System;
using System.Drawing;
using Xunit;
using FluentAssertions;
using System.Collections.Generic;
using System.Collections;

namespace CDS.Imaging.WinFormsTests
{
    public class VirtualImageOnDisplayTests
    {
        [Fact]
        public void FitToWindowMode_ImageSameSizeAsDisplay_FillsDisplayExactly()
        {
            var vid = new WinForms.VirtualImageOnDisplay();
            vid.DisplaySize = new Size(1000, 600);
            vid.ImageSize = vid.DisplaySize;
            vid.Mode = WinForms.BitmapDisplayMode.FitToWindowCentred;

            vid.PaintRect.Should().Be(new RectangleF(PointF.Empty, vid.DisplaySize));
        }


        [Fact]
        public void ActualSizeMode_ImageSameSizeAsDisplay_FillsDisplayExactly()
        {
            var vid = new WinForms.VirtualImageOnDisplay();
            vid.DisplaySize = new Size(1000, 600);
            vid.ImageSize = vid.DisplaySize;
            vid.Mode = WinForms.BitmapDisplayMode.ActualSizeCentred;

            vid.PaintRect.Should().Be(new RectangleF(PointF.Empty, vid.DisplaySize));
        }


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

            public IEnumerator<object[]> GetEnumerator()
            {
                yield return SquareImageInLargeWideDisplay;
                yield return SquareImageInLargeTallDisplay;
                yield return SquareImageInSmallWideDisplay;
                yield return SquareImageInSmallTallDisplay;
            }

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }


        [Theory]
        [ClassData(typeof(FitToWindowSampleData))]
        public void FitToWindow_Maximumise_DisplayArea(Size imageSize, Size displaySize, RectangleF paintRect)
        {
            var vid = new WinForms.VirtualImageOnDisplay();
            vid.DisplaySize = displaySize;
            vid.ImageSize = imageSize;
            vid.Mode = WinForms.BitmapDisplayMode.FitToWindowCentred;

            vid.PaintRect.Should().Be(paintRect);
        }


        [Fact]
        public void MovePaintRect_Changes_Location()
        {
            var vid = new WinForms.VirtualImageOnDisplay();
            vid.Mode = WinForms.BitmapDisplayMode.FitToWindowCentred;
            vid.DisplaySize = new Size(1000, 1000);
            vid.ImageSize = new Size(200, 200);

            vid.Mode = WinForms.BitmapDisplayMode.Free;
            var newLocation = new PointF(123, 456);
            vid.MovePaintRect(newLocation);
            vid.PaintRect.Location.Should().Be(newLocation);
        }


        [Fact]
        public void CentreNonCentredRect_MovesTo_Centre()
        {
            var vid = new WinForms.VirtualImageOnDisplay();
            vid.Mode = WinForms.BitmapDisplayMode.FitToWindowCentred;
            vid.DisplaySize = new Size(1000, 1000);
            vid.ImageSize = new Size(200, 200);
            var fitToWindowRect = vid.PaintRect;

            vid.Mode = WinForms.BitmapDisplayMode.Free;
            vid.MovePaintRect(new PointF(123, 456));
            vid.Centre();
            vid.PaintRect.Should().Be(fitToWindowRect);
        }


        [Theory]
        [InlineData(0, 0, 300, 400)]
        public void DisplayLocation_FromImageLocation_Correct(float imageX, float imageY, float displayX, float displayY)
        {
            var vid = new WinForms.VirtualImageOnDisplay();
            vid.Mode = WinForms.BitmapDisplayMode.ActualSizeCentred;
            vid.DisplaySize = new Size(1000, 1000);
            vid.ImageSize = new Size(400, 200);

            var imageLocation = new PointF(imageX, imageY);
            var expectedDisplayLocation = new PointF(displayX, displayY);

            var actualDisplayLocation = vid.DisplayLocationFromImageLocation(imageLocation);

            actualDisplayLocation.Should().Be(expectedDisplayLocation);
        }


        [Theory]
        [InlineData(0, 0, 300, 400)]
        public void ImageLocation_FromDisplayLocation_Correct(float imageX, float imageY, float displayX, float displayY)
        {
            var vid = new WinForms.VirtualImageOnDisplay();
            vid.Mode = WinForms.BitmapDisplayMode.ActualSizeCentred;
            vid.DisplaySize = new Size(1000, 1000);
            vid.ImageSize = new Size(400, 200);

            var expectedImageLocation = new PointF(imageX, imageY);
            var displayLocation = new PointF(displayX, displayY);

            var actualImageLocation = vid.ImageLocationFromDisplayLocation(displayLocation);

            actualImageLocation.Should().Be(expectedImageLocation);
        }
    }
}
