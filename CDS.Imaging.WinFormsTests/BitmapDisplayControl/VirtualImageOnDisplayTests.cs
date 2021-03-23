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

            vid.PaintRect.X.Should().BeApproximately(paintRect.X, 0.01f);
            vid.PaintRect.Y.Should().BeApproximately(paintRect.Y, 0.01f);
            vid.PaintRect.Width.Should().BeApproximately(paintRect.Width, 0.01f);
            vid.PaintRect.Height.Should().BeApproximately(paintRect.Height, 0.01f);
        }


        [Fact]
        public void ChangeTargetImageCentre_MovePaintRect()
        {
            var vid = new WinForms.VirtualImageOnDisplay();
            vid.Mode = WinForms.BitmapDisplayMode.FitToWindowCentred;
            vid.DisplaySize = new Size(1000, 1000);
            vid.ImageSize = new Size(200, 200);

            vid.Mode = WinForms.BitmapDisplayMode.Free;
            vid.TargetImageCentre = Point.Empty;
            vid.PaintRect.Location.Should().Be(new PointF(500, 500));
        }


        [Fact]
        public void DefaultInitialisation_ModeIs_FitToWindow()
        {
            var vid = new WinForms.VirtualImageOnDisplay();
            vid.Mode.Should().Be(WinForms.BitmapDisplayMode.FitToWindowCentred);
        }


        [Fact]
        public void DefaultInitialisation_FitsToDisplay()
        {
            var vid = new WinForms.VirtualImageOnDisplay();
            vid.DisplaySize = new Size(1000, 1000);
            vid.ImageSize = new Size(100, 100);

            vid.PaintRect.Should().Be(new RectangleF(0, 0, 1000, 1000));
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
