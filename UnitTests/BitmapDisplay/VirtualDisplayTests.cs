using CDS.Imaging.WinForms.BitmapDisplay;
using System.Drawing;
using System.Threading.Tasks;

namespace CDS.Imaging.WinFormsTests.BitmapDisplay
{
    [TestClass]
    public partial class VirtualDisplayTests
    {
        [TestMethod]
        public Task FitToWindowMode_ImageSameSizeAsDisplay_FillsDisplayExactly()
        {
            var vid = new VirtualDisplay(onPaintRectChanged: (_, _) => { });
            vid.DisplaySize = new Size(1000, 600);
            vid.ImageSize = vid.DisplaySize;
            vid.Mode = BitmapDisplayMode.FitToWindowCentred;

            var reviewData = new
            {
                vid
            };

            return Verify(reviewData);
        }

        [TestMethod]
        public Task ActualSizeMode_ImageSameSizeAsDisplay_FillsDisplayExactly()
        {
            var vid = new VirtualDisplay(onPaintRectChanged: (_, _) => { });
            vid.DisplaySize = new Size(1000, 600);
            vid.ImageSize = vid.DisplaySize;
            vid.Mode = BitmapDisplayMode.ActualSizeCentred;

            var reviewData = new
            {
                vid
            };

            return Verify(reviewData);
        }

        [TestMethod]
        [DynamicData(nameof(FitToWindowSampleData.Data), typeof(FitToWindowSampleData))]
        public Task FitToWindow_Maximumise_DisplayArea(Size imageSize, Size displaySize, RectangleF paintRect)
        {
            var vid = new VirtualDisplay(onPaintRectChanged: (_, _) => { });
            vid.DisplaySize = displaySize;
            vid.ImageSize = imageSize;

            var reviewData = new
            {
                vid
            };

            return Verify(reviewData);
        }

        [TestMethod]
        public Task ChangeTargetImageCentre_MovePaintRect()
        {
            var vid = new VirtualDisplay(onPaintRectChanged: (_, _) => { });
            vid.Mode = BitmapDisplayMode.FitToWindowCentred;
            vid.DisplaySize = new Size(1000, 1000);
            vid.ImageSize = new Size(200, 200);

            vid.Mode = BitmapDisplayMode.Free;
            vid.TargetImageCentre = Point.Empty;

            var reviewData = new
            {
                vid
            };

            return Verify(reviewData);
        }

        [TestMethod]
        public Task DefaultInitialisation_ModeIs_FitToWindow()
        {
            var vid = new VirtualDisplay(onPaintRectChanged: (_, _) => { });

            var reviewData = new
            {
                vid
            };

            return Verify(reviewData);
        }

        [TestMethod]
        public Task DefaultInitialisation_FitsToDisplay()
        {
            var vid = new VirtualDisplay(onPaintRectChanged: (_, _) => { });
            vid.DisplaySize = new Size(1000, 1000);
            vid.ImageSize = new Size(100, 100);

            var reviewData = new
            {
                vid
            };

            return Verify(reviewData);
        }

        [TestMethod]
        [DataRow(0, 0, 300, 400)]
        public Task MapImageToDisplay_Returns_DisplayRect(
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
            var actualDisplayLocation = vid.MapImageToDisplay(imageLocation);

            var reviewData = new
            {
                vid,
                actualDisplayLocation
            };

            return Verify(reviewData);
        }

        [TestMethod]
        [DataRow(0, 0, 300, 400)]
        public Task MapDisplayToImage_Returns_ImageRect(float imageX, float imageY, float displayX, float displayY)
        {
            var vid = new VirtualDisplay(onPaintRectChanged: (_, _) => { });
            vid.Mode = BitmapDisplayMode.ActualSizeCentred;
            vid.DisplaySize = new Size(1000, 1000);
            vid.ImageSize = new Size(400, 200);

            var displayLocation = new PointF(displayX, displayY);
            var actualImageLocation = vid.MapDisplayToImage(displayLocation);

            var reviewData = new
            {
                vid,
                actualImageLocation
            };

            return Verify(reviewData);
        }

        [TestMethod]
        public Task ChangeImageSizeInFitToWindowMode_Resizes_ToFitWindow()
        {
            var vid = new VirtualDisplay(onPaintRectChanged: (_, _) => { });
            vid.Mode = BitmapDisplayMode.FitToWindowCentred;
            vid.DisplaySize = new Size(2000, 1000);
            vid.ImageSize = new Size(200, 200);

            vid.ImageSize = new Size(100, 1000);

            var reviewData = new
            {
                vid
            };

            return Verify(reviewData);
        }

        [TestMethod]
        public Task FreshImage_InFreeMode_HasZoom1()
        {
            var vid = new VirtualDisplay(onPaintRectChanged: (_, _) => { });
            vid.Mode = BitmapDisplayMode.Free;
            vid.DisplaySize = new Size(2000, 1000);
            vid.ImageSize = new Size(200, 200);

            var reviewData = new
            {
                vid
            };

            return Verify(reviewData);
        }

        [TestMethod]
        public Task FreshImage_InFreeMode_IsCentred()
        {
            var vid = new VirtualDisplay(onPaintRectChanged: (_, _) => { });
            vid.Mode = BitmapDisplayMode.Free;
            vid.DisplaySize = new Size(2000, 1000);
            vid.ImageSize = new Size(200, 200);

            var reviewData = new
            {
                vid
            };

            return Verify(reviewData);
        }

        [TestMethod]
        public Task ChangeImageSizeInFreeMode_Leaves_ZoomUnchanged()
        {
            var vid = new VirtualDisplay(onPaintRectChanged: (_, _) => { });
            vid.Mode = BitmapDisplayMode.Free;
            vid.DisplaySize = new Size(2000, 1000);
            vid.ImageSize = new Size(200, 200);

            vid.ImageSize = new Size(300, 100);

            var reviewData = new
            {
                vid
            };

            return Verify(reviewData);
        }

        [TestMethod]
        public Task ChangeImageSizeInFreeMode_Leaves_CentresUnchanged()
        {
            var vid = new VirtualDisplay(onPaintRectChanged: (_, _) => { });
            vid.Mode = BitmapDisplayMode.FitToWindowCentred;
            vid.DisplaySize = new Size(2000, 1000);
            vid.ImageSize = new Size(200, 200);

            vid.ImageSize = new Size(100, 1000);

            var reviewData = new
            {
                vid
            };

            return Verify(reviewData);
        }

        [TestMethod]
        public Task ChangeTargetImageCentreNotInFreeMode_Leaves_CentreUnchanged()
        {
            var vid = new VirtualDisplay(onPaintRectChanged: (_, _) => { });
            vid.DisplaySize = new Size(2000, 1000);
            vid.ImageSize = new Size(200, 200);

            // Fit to window
            vid.Mode = BitmapDisplayMode.FitToWindowCentred;
            var originalPaintRect = vid.PaintRect;
            vid.TargetImageCentre = new PointF(0, 0);
            var fitToWindowTargetImageCentre = vid.TargetImageCentre;

            // Actual size
            vid.Mode = BitmapDisplayMode.ActualSizeCentred;
            originalPaintRect = vid.PaintRect;
            vid.TargetImageCentre = new PointF(0, 0);
            var actualSizeTargetImageCentre = vid.TargetImageCentre;

            var reviewData = new
            {
                fitToWindowTargetImageCentre,
                actualSizeTargetImageCentre
            };

            return Verify(reviewData);
        }

        [TestMethod]
        public Task ChangeTargetImageCentreInFreeMode_Changes_Centre()
        {
            var vid = new VirtualDisplay(onPaintRectChanged: (_, _) => { });
            vid.DisplaySize = new Size(2000, 1000);
            vid.ImageSize = new Size(200, 200);

            // Free
            vid.Mode = BitmapDisplayMode.Free;
            var originalPaintRect = vid.PaintRect;
            vid.TargetImageCentre = new PointF(0, 0);

            var reviewData = new
            {
                vid
            };

            return Verify(reviewData);
        }

        [TestMethod]
        public Task ChangeTargetDisplayCentreNotInFreeMode_Leaves_CentreUnchanged()
        {
            var vid = new VirtualDisplay(onPaintRectChanged: (_, _) => { });
            vid.DisplaySize = new Size(2000, 1000);
            vid.ImageSize = new Size(200, 200);

            // Fit to window
            vid.Mode = BitmapDisplayMode.FitToWindowCentred;
            var originalPaintRect = vid.PaintRect;
            vid.TargetDisplayCentre = new PointF(0, 0);
            var fitToWindowPaintRect = vid.PaintRect;
            var fitToWindowTargetDisplayCentre = vid.TargetDisplayCentre;

            // Actual size
            vid.Mode = BitmapDisplayMode.ActualSizeCentred;
            originalPaintRect = vid.PaintRect;
            vid.TargetDisplayCentre = new PointF(0, 0);
            var actualSizePaintRect = vid.PaintRect;
            var actualSizeTargetDisplayCentre = vid.TargetDisplayCentre;

            var reviewData = new
            {
                fitToWindowTargetDisplayCentre,
                actualSizeTargetDisplayCentre,
            };

            return Verify(reviewData);
        }

        [TestMethod]
        public Task ChangeTargetDisplayCentreInFreeMode_Changes_Centre()
        {
            var vid = new VirtualDisplay(onPaintRectChanged: (_, _) => { });
            vid.DisplaySize = new Size(2000, 1000);
            vid.ImageSize = new Size(200, 200);

            // Free
            vid.Mode = BitmapDisplayMode.Free;
            var originalPaintRect = vid.PaintRect;
            vid.TargetDisplayCentre = new PointF(0, 0);

            var reviewData = new
            {
                vid
            };

            return Verify(reviewData);
        }
    }
}
