using CDS.Imaging.WinForms.BitmapDisplay;
using FluentAssertions;
using System.Drawing;

namespace CDS.Imaging.WinFormsTests.BitmapDisplay
{
    [TestClass]
    public class BitmapDisplayPaintRectTests
    {
        [TestMethod]
        public void FitToWindowMode_ImageSameSizeAsDisplay_FillsDisplayExactly()
        {
            var vid = new VirtualDisplay(onPaintRectChanged: (_, _) => { });
            vid.DisplaySize = new Size(1000, 600);
            vid.ImageSize = vid.DisplaySize;
            vid.Mode = BitmapDisplayMode.FitToWindowCentred;

            vid.PaintRect.Should().Be(new RectangleF(PointF.Empty, vid.DisplaySize));
        }


        [TestMethod]
        public void ActualSizeMode_ImageSameSizeAsDisplay_FillsDisplayExactly()
        {
            var vid = new VirtualDisplay(onPaintRectChanged: (_, _) => { });
            vid.DisplaySize = new Size(1000, 600);
            vid.ImageSize = vid.DisplaySize;
            vid.Mode = BitmapDisplayMode.ActualSizeCentred;

            vid.PaintRect.Should().Be(new RectangleF(PointF.Empty, vid.DisplaySize));
        }



        [TestMethod]
        [DynamicData(nameof(FitToWindowSampleData.Data), typeof(FitToWindowSampleData))]
        public void FitToWindow_Maximumise_DisplayArea(Size imageSize, Size displaySize, RectangleF paintRect)
        {
            var vid = new VirtualDisplay(onPaintRectChanged: (_, _) => { });
            vid.DisplaySize = displaySize;
            vid.ImageSize = imageSize;

            vid.PaintRect.X.Should().BeApproximately(paintRect.X, 0.01f);
            vid.PaintRect.Y.Should().BeApproximately(paintRect.Y, 0.01f);
            vid.PaintRect.Width.Should().BeApproximately(paintRect.Width, 0.01f);
            vid.PaintRect.Height.Should().BeApproximately(paintRect.Height, 0.01f);
        }


        [TestMethod]
        public void ChangeTargetImageCentre_MovePaintRect()
        {
            var vid = new VirtualDisplay(onPaintRectChanged: (_, _) => { });
            vid.Mode = BitmapDisplayMode.FitToWindowCentred;
            vid.DisplaySize = new Size(1000, 1000);
            vid.ImageSize = new Size(200, 200);

            vid.Mode = BitmapDisplayMode.Free;
            vid.TargetImageCentre = Point.Empty;
            vid.PaintRect.Location.Should().Be(new PointF(500, 500));
        }


        [TestMethod]
        public void DefaultInitialisation_ModeIs_FitToWindow()
        {
            var vid = new VirtualDisplay(onPaintRectChanged: (_, _) => { });
            vid.Mode.Should().Be(BitmapDisplayMode.FitToWindowCentred);
        }


        [TestMethod]
        public void DefaultInitialisation_FitsToDisplay()
        {
            var vid = new VirtualDisplay(onPaintRectChanged: (_, _) => { });
            vid.DisplaySize = new Size(1000, 1000);
            vid.ImageSize = new Size(100, 100);

            vid.PaintRect.Should().Be(new RectangleF(0, 0, 1000, 1000));
        }


        [TestMethod]
        [DataRow(0, 0, 300, 400)]
        public void MapImageToDisplay_Returns_DisplayRect(
            float imageX,
            float imageY,
            float expectedDisplayX,
            float expectedDisplayY)
        {
            var vid = new VirtualDisplay(onPaintRectChanged: (_, _) => { });
            vid.Mode = BitmapDisplayMode.ActualSizeCentred;
            vid.DisplaySize = new Size(1000, 1000);
            vid.ImageSize = new Size(400, 200);

            var imageLocation = new PointF(imageX, imageY);
            var expectedDisplayLocation = new PointF(expectedDisplayX, expectedDisplayY);

            var actualDisplayLocation = vid.MapImageToDisplay(imageLocation);

            actualDisplayLocation.Should().Be(expectedDisplayLocation);
        }


        [TestMethod]
        [DataRow(0, 0, 300, 400)]
        public void MapDisplayToImage_Returns_ImageRect(float imageX, float imageY, float displayX, float displayY)
        {
            var vid = new VirtualDisplay(onPaintRectChanged: (_, _) => { });
            vid.Mode = BitmapDisplayMode.ActualSizeCentred;;
            vid.DisplaySize = new Size(1000, 1000);
            vid.ImageSize = new Size(400, 200);

            var expectedImageLocation = new PointF(imageX, imageY);
            var displayLocation = new PointF(displayX, displayY);

            var actualImageLocation = vid.MapDisplayToImage(displayLocation);

            actualImageLocation.Should().Be(expectedImageLocation);
        }


        [TestMethod]
        public void ChangeImageSizeInFitToWindowMode_Resizes_ToFitWindow()
        {
            var vid = new VirtualDisplay(onPaintRectChanged: (_, _) => { });
            vid.Mode = BitmapDisplayMode.FitToWindowCentred;
            vid.DisplaySize = new Size(2000, 1000);
            vid.ImageSize = new Size(200, 200);

            vid.ImageSize = new Size(100, 1000);
            vid.PaintRect.X.Should().BeApproximately(950, 0.01f);
            vid.PaintRect.Y.Should().BeApproximately(0, 0.01f);
            vid.PaintRect.Width.Should().BeApproximately(100, 0.01f);
            vid.PaintRect.Height.Should().BeApproximately(1000, 0.01f);
        }



        [TestMethod]
        public void FreshImage_InFreeMode_HasZoom1()
        {
            var vid = new VirtualDisplay(onPaintRectChanged: (_, _) => { });
            vid.Mode = BitmapDisplayMode.Free;
            vid.DisplaySize = new Size(2000, 1000);
            vid.ImageSize = new Size(200, 200);

            vid.Zoom.Should().BeApproximately(1, 0.00001f);
        }


        [TestMethod]
        public void FreshImage_InFreeMode_IsCentred()
        {
            var vid = new VirtualDisplay(onPaintRectChanged: (_, _) => { });
            vid.Mode = BitmapDisplayMode.Free;
            vid.DisplaySize = new Size(2000, 1000);
            vid.ImageSize = new Size(200, 200);

            var expectedPaintRect = new RectangleF(900, 400, 200, 200);
            CheckRectagleFIsExpected(vid.PaintRect, expectedPaintRect, 0.0001f);
        }


        [TestMethod]
        public void ChangeImageSizeInFreeMode_Leaves_ZoomUnchanged()
        {
            var vid = new VirtualDisplay(onPaintRectChanged: (_, _) => { });
            vid.Mode = BitmapDisplayMode.Free;
            vid.DisplaySize = new Size(2000, 1000);
            vid.ImageSize = new Size(200, 200);

            vid.ImageSize = new Size(300, 100);
            vid.Zoom.Should().BeApproximately(1, 0.0001f);
        }


        [TestMethod]
        public void ChangeImageSizeInFreeMode_Leaves_CentresUnchanged()
        {
            var vid = new VirtualDisplay(onPaintRectChanged: (_, _) => { });
            vid.Mode = BitmapDisplayMode.FitToWindowCentred;
            vid.DisplaySize = new Size(2000, 1000);
            vid.ImageSize = new Size(200, 200);

            vid.ImageSize = new Size(100, 1000);
            var expectedPaintRect = new RectangleF(950, 0, 100, 1000);
            CheckRectagleFIsExpected(vid.PaintRect, expectedPaintRect, 0.0001f);
        }


        /// <summary>
        /// The target image centre can only be changed when free mode is active
        /// </summary>
        [TestMethod]
        public void ChangeTargetImageCentreNotInFreeMode_Leaves_CentreUnchanged()
        {
            // Setup
            var vid = new VirtualDisplay(onPaintRectChanged: (_, _) => { });
            vid.DisplaySize = new Size(2000, 1000);
            vid.ImageSize = new Size(200, 200);

            // Fit to window
            vid.Mode = BitmapDisplayMode.FitToWindowCentred;
            var originalPaintRect = vid.PaintRect;
            vid.TargetImageCentre = new PointF(0, 0);
            CheckRectagleFIsExpected(vid.PaintRect, originalPaintRect, 0.0001f);

            // Actual size
            vid.Mode = BitmapDisplayMode.ActualSizeCentred;
            originalPaintRect = vid.PaintRect;
            vid.TargetImageCentre = new PointF(0, 0);
            CheckRectagleFIsExpected(vid.PaintRect, originalPaintRect, 0.0001f);
        }


        /// <summary>
        /// The target image centre can only be changed when free mode is active
        /// </summary>
        [TestMethod]
        public void ChangeTargetImageCentreInFreeMode_Changes_Centre()
        {
            // Setup
            var vid = new VirtualDisplay(onPaintRectChanged: (_, _) => { });
            vid.DisplaySize = new Size(2000, 1000);
            vid.ImageSize = new Size(200, 200);

            // Free
            vid.Mode = BitmapDisplayMode.Free;
            var originalPaintRect = vid.PaintRect;
            vid.TargetImageCentre = new PointF(0, 0);
            var leftDifference = vid.PaintRect.X - originalPaintRect.X;
            leftDifference.Should().BeGreaterThan(1);
        }

        /// <summary>
        /// The target display centre can only be changed when free mode is active
        /// </summary>
        [TestMethod]
        public void ChangeTargetDisplayCentreNotInFreeMode_Leaves_CentreUnchanged()
        {
            // Setup
            var vid = new VirtualDisplay(onPaintRectChanged: (_, _) => { });
            vid.DisplaySize = new Size(2000, 1000);
            vid.ImageSize = new Size(200, 200);

            // Fit to window
            vid.Mode = BitmapDisplayMode.FitToWindowCentred;
            var originalPaintRect = vid.PaintRect;
            vid.TargetDisplayCentre = new PointF(0, 0);
            CheckRectagleFIsExpected(vid.PaintRect, originalPaintRect, 0.0001f);

            // Actual size
            vid.Mode = BitmapDisplayMode.ActualSizeCentred;
            originalPaintRect = vid.PaintRect;
            vid.TargetDisplayCentre = new PointF(0, 0);
            CheckRectagleFIsExpected(vid.PaintRect, originalPaintRect, 0.0001f);
        }


        /// <summary>
        /// The target display centre can only be changed when free mode is active
        /// </summary>
        [TestMethod]
        public void ChangeTargetDisplayCentreInFreeMode_Changes_Centre()
        {
            // Setup
            var vid = new VirtualDisplay(onPaintRectChanged: (_, _) => { });
            vid.DisplaySize = new Size(2000, 1000);
            vid.ImageSize = new Size(200, 200);

            // Free
            vid.Mode = BitmapDisplayMode.Free;
            var originalPaintRect = vid.PaintRect;
            vid.TargetDisplayCentre = new PointF(0, 0);
            var leftDifference = Math.Abs(vid.PaintRect.X - originalPaintRect.X);
            leftDifference.Should().BeGreaterThan(1);
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
