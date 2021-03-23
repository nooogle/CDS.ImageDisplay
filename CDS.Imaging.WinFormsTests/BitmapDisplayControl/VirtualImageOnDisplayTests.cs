using FluentAssertions;
using System.Drawing;
using Xunit;

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
        public void MapImageToDisplay_Returns_DisplayRect(float imageX, float imageY, float displayX, float displayY)
        {
            var vid = new WinForms.VirtualImageOnDisplay();
            vid.Mode = WinForms.BitmapDisplayMode.ActualSizeCentred;
            vid.DisplaySize = new Size(1000, 1000);
            vid.ImageSize = new Size(400, 200);

            var imageLocation = new PointF(imageX, imageY);
            var expectedDisplayLocation = new PointF(displayX, displayY);

            var actualDisplayLocation = vid.MapImageToDisplay(imageLocation);

            actualDisplayLocation.Should().Be(expectedDisplayLocation);
        }


        [Theory]
        [InlineData(0, 0, 300, 400)]
        public void MapDisplayToImage_Returns_ImageRect(float imageX, float imageY, float displayX, float displayY)
        {
            var vid = new WinForms.VirtualImageOnDisplay();
            vid.Mode = WinForms.BitmapDisplayMode.ActualSizeCentred;
            vid.DisplaySize = new Size(1000, 1000);
            vid.ImageSize = new Size(400, 200);

            var expectedImageLocation = new PointF(imageX, imageY);
            var displayLocation = new PointF(displayX, displayY);

            var actualImageLocation = vid.MapDisplayToImage(displayLocation);

            actualImageLocation.Should().Be(expectedImageLocation);
        }
    }
}
