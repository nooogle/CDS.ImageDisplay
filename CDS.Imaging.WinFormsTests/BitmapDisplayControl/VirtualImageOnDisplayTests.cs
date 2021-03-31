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
            var vid = new WinForms.VirtualImageOnDisplay(onPaintRectChanged: (_, _) => { });
            vid.DisplaySize = new Size(1000, 600);
            vid.ImageSize = vid.DisplaySize;
            vid.Mode = WinForms.BitmapDisplayMode.FitToWindowCentred;

            vid.PaintRect.Should().Be(new RectangleF(PointF.Empty, vid.DisplaySize));
        }


        [Fact]
        public void ActualSizeMode_ImageSameSizeAsDisplay_FillsDisplayExactly()
        {
            var vid = new WinForms.VirtualImageOnDisplay(onPaintRectChanged: (_, _) => { });
            vid.DisplaySize = new Size(1000, 600);
            vid.ImageSize = vid.DisplaySize;
            vid.Mode = WinForms.BitmapDisplayMode.ActualSizeCentred;

            vid.PaintRect.Should().Be(new RectangleF(PointF.Empty, vid.DisplaySize));
        }



        [Theory]
        [ClassData(typeof(FitToWindowSampleData))]
        public void FitToWindow_Maximumise_DisplayArea(Size imageSize, Size displaySize, RectangleF paintRect)
        {
            var vid = new WinForms.VirtualImageOnDisplay(onPaintRectChanged: (_, _) => { });
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
            var vid = new WinForms.VirtualImageOnDisplay(onPaintRectChanged: (_, _) => { });
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
            var vid = new WinForms.VirtualImageOnDisplay(onPaintRectChanged: (_, _) => { });
            vid.Mode.Should().Be(WinForms.BitmapDisplayMode.FitToWindowCentred);
        }


        [Fact]
        public void DefaultInitialisation_FitsToDisplay()
        {
            var vid = new WinForms.VirtualImageOnDisplay(onPaintRectChanged: (_, _) => { });
            vid.DisplaySize = new Size(1000, 1000);
            vid.ImageSize = new Size(100, 100);

            vid.PaintRect.Should().Be(new RectangleF(0, 0, 1000, 1000));
        }


        [Theory]
        [InlineData(0, 0, 300, 400)]
        public void MapImageToDisplay_Returns_DisplayRect(float imageX, float imageY, float displayX, float displayY)
        {
            var vid = new WinForms.VirtualImageOnDisplay(onPaintRectChanged: (_, _) => { });
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
            var vid = new WinForms.VirtualImageOnDisplay(onPaintRectChanged: (_, _) => { });
            vid.Mode = WinForms.BitmapDisplayMode.ActualSizeCentred;
            vid.DisplaySize = new Size(1000, 1000);
            vid.ImageSize = new Size(400, 200);

            var expectedImageLocation = new PointF(imageX, imageY);
            var displayLocation = new PointF(displayX, displayY);

            var actualImageLocation = vid.MapDisplayToImage(displayLocation);

            actualImageLocation.Should().Be(expectedImageLocation);
        }


        [Fact]
        public void ChangeImageSizeInFitToWindowMode_Resizes_ToFitWindow()
        {
            var vid = new WinForms.VirtualImageOnDisplay(onPaintRectChanged: (_, _) => { });
            vid.Mode = WinForms.BitmapDisplayMode.FitToWindowCentred;
            vid.DisplaySize = new Size(2000, 1000);
            vid.ImageSize = new Size(200, 200);

            vid.ImageSize = new Size(100, 1000);
            vid.PaintRect.X.Should().BeApproximately(950, 0.01f);
            vid.PaintRect.Y.Should().BeApproximately(0, 0.01f);
            vid.PaintRect.Width.Should().BeApproximately(100, 0.01f);
            vid.PaintRect.Height.Should().BeApproximately(1000, 0.01f);
        }



        [Fact]
        public void FreshImage_InFreeMode_HasZoom1()
        {
            var vid = new WinForms.VirtualImageOnDisplay(onPaintRectChanged: (_, _) => { });
            vid.Mode = WinForms.BitmapDisplayMode.Free;
            vid.DisplaySize = new Size(2000, 1000);
            vid.ImageSize = new Size(200, 200);

            vid.Zoom.Should().BeApproximately(1, 0.00001f);
        }


        [Fact]
        public void FreshImage_InFreeMode_IsCentred()
        {
            var vid = new WinForms.VirtualImageOnDisplay(onPaintRectChanged: (_, _) => { });
            vid.Mode = WinForms.BitmapDisplayMode.Free;
            vid.DisplaySize = new Size(2000, 1000);
            vid.ImageSize = new Size(200, 200);

            var expectedPaintRect = new RectangleF(900, 400, 200, 200);
            CheckRectagleFIsExpected(vid.PaintRect, expectedPaintRect, 0.0001f);
        }


        [Fact]
        public void ChangeImageSizeInFreeMode_Leaves_ZoomUnchanged()
        {
            var vid = new WinForms.VirtualImageOnDisplay(onPaintRectChanged: (_, _) => { });
            vid.Mode = WinForms.BitmapDisplayMode.Free;
            vid.DisplaySize = new Size(2000, 1000);
            vid.ImageSize = new Size(200, 200);

            vid.ImageSize = new Size(300, 100);
            vid.Zoom.Should().BeApproximately(1, 0.0001f);
        }


        [Fact]
        public void ChangeImageSizeInFreeMode_Leaves_CentresUnchanged()
        {
            var vid = new WinForms.VirtualImageOnDisplay(onPaintRectChanged: (_, _) => { });
            vid.Mode = WinForms.BitmapDisplayMode.FitToWindowCentred;
            vid.DisplaySize = new Size(2000, 1000);
            vid.ImageSize = new Size(200, 200);

            vid.ImageSize = new Size(100, 1000);
            var expectedPaintRect = new RectangleF(950, 0, 100, 1000);
            CheckRectagleFIsExpected(vid.PaintRect, expectedPaintRect, 0.0001f);
        }


        private static void CheckRectagleFIsExpected(RectangleF actual, RectangleF expected, float precision)
        {
            actual.X.Should().BeApproximately(expected.X, precision);
            actual.Y.Should().BeApproximately(expected.Y, precision);
            actual.Width.Should().BeApproximately(expected.Width, precision);
            actual.Height.Should().BeApproximately(expected.Height, precision);
        }
    }
}
