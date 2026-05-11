using AwesomeAssertions;

using CDS.Imaging.BitmapDisplay;

using System.Drawing;

namespace CDS.Imaging.WinFormsTests.BitmapDisplay;

[TestClass]
public partial class VirtualDisplayTests
{
    [TestMethod]
    public void Constructor_DefaultState_UsesFitToWindowMode()
    {
        // Arrange
        var virtualDisplay = CreateSubject();

        // Act
        var result = new
        {
            virtualDisplay.Mode,
            virtualDisplay.Zoom,
            virtualDisplay.AnythingToDisplay,
            virtualDisplay.PaintRect,
            virtualDisplay.TargetDisplayCentre,
            virtualDisplay.TargetImageCentre,
        };

        // Bundle
        var expected = new
        {
            Mode = BitmapDisplayMode.FitToWindowCentred,
            Zoom = 1.0f,
            AnythingToDisplay = false,
            PaintRect = RectangleF.Empty,
            TargetDisplayCentre = PointF.Empty,
            TargetImageCentre = PointF.Empty,
        };

        // Verify
        result.Should().BeEquivalentTo(expected);
    }

    [TestMethod]
    public void FitToWindowMode_ImageSameSizeAsDisplay_FillsDisplayExactly()
    {
        // Arrange
        var virtualDisplay = CreateSubject();
        virtualDisplay.DisplaySize = new Size(1000, 600);
        virtualDisplay.ImageSize = virtualDisplay.DisplaySize;
        virtualDisplay.Mode = BitmapDisplayMode.FitToWindowCentred;

        // Act
        var result = CaptureDisplayState(virtualDisplay);

        // Bundle
        var expected = new
        {
            Zoom = 1.0f,
            PaintRect = new RectangleF(0, 0, 1000, 600),
            TargetDisplayCentre = new PointF(500, 300),
            TargetImageCentre = new PointF(500, 300),
            SizeOfHalfDisplayPixel = new SizeF(0, 0),
            AnythingToDisplay = true,
        };

        // Verify
        result.Should().BeEquivalentTo(expected);
    }

    [TestMethod]
    public void ActualSizeMode_ImageSameSizeAsDisplay_FillsDisplayExactly()
    {
        // Arrange
        var virtualDisplay = CreateSubject();
        virtualDisplay.DisplaySize = new Size(1000, 600);
        virtualDisplay.ImageSize = virtualDisplay.DisplaySize;
        virtualDisplay.Mode = BitmapDisplayMode.ActualSizeCentred;

        // Act
        var result = CaptureDisplayState(virtualDisplay);

        // Bundle
        var expected = new
        {
            Zoom = 1.0f,
            PaintRect = new RectangleF(0, 0, 1000, 600),
            TargetDisplayCentre = new PointF(500, 300),
            TargetImageCentre = new PointF(500, 300),
            SizeOfHalfDisplayPixel = new SizeF(0, 0),
            AnythingToDisplay = true,
        };

        // Verify
        result.Should().BeEquivalentTo(expected);
    }

    [TestMethod]
    [DynamicData(nameof(FitToWindowSampleData.Data), typeof(FitToWindowSampleData))]
    public void FitToWindow_Maximizes_DisplayArea(Size imageSize, Size displaySize, RectangleF expectedPaintRect)
    {
        // Arrange
        var virtualDisplay = CreateSubject();
        virtualDisplay.DisplaySize = displaySize;
        virtualDisplay.ImageSize = imageSize;

        // Act
        var actualPaintRect = virtualDisplay.PaintRect;
        var actualZoom = virtualDisplay.Zoom;

        // Bundle
        var expectedZoom = expectedPaintRect.Width / imageSize.Width;

        // Verify
        actualPaintRect.X.Should().BeApproximately(expectedPaintRect.X, 0.01f);
        actualPaintRect.Y.Should().BeApproximately(expectedPaintRect.Y, 0.01f);
        actualPaintRect.Width.Should().BeApproximately(expectedPaintRect.Width, 0.02f);
        actualPaintRect.Height.Should().BeApproximately(expectedPaintRect.Height, 0.02f);
        actualZoom.Should().BeApproximately(expectedZoom, 0.00001f);
    }

    [TestMethod]
    public void ChangeTargetImageCentre_InFreeMode_MovesPaintRect()
    {
        // Arrange
        var virtualDisplay = CreateConfiguredFreeDisplay();

        // Act
        virtualDisplay.TargetImageCentre = PointF.Empty;
        var result = CaptureDisplayState(virtualDisplay);

        // Bundle
        var expected = new
        {
            Zoom = 5.0f,
            PaintRect = new RectangleF(500, 500, 1000, 1000),
            TargetDisplayCentre = new PointF(500, 500),
            TargetImageCentre = PointF.Empty,
        };

        // Verify
        result.Should().BeEquivalentTo(expected);
    }

    [TestMethod]
    public void DefaultInitialization_WithDisplayAndImage_FitsToDisplay()
    {
        // Arrange
        var virtualDisplay = CreateSubject();
        virtualDisplay.DisplaySize = new Size(1000, 1000);
        virtualDisplay.ImageSize = new Size(100, 100);

        // Act
        var result = CaptureDisplayState(virtualDisplay);

        // Bundle
        var expected = new
        {
            Zoom = 10.0f,
            PaintRect = new RectangleF(0, 0, 1000, 1000),
            TargetDisplayCentre = new PointF(500, 500),
            TargetImageCentre = new PointF(50, 50),
        };

        // Verify
        result.Should().BeEquivalentTo(expected);
    }

    [TestMethod]
    [DataRow(0, 0, 300, 400)]
    [DataRow(200, 100, 500, 500)]
    public void MapImageToDisplay_ReturnsExpectedDisplayLocation(
        float imageX,
        float imageY,
        float expectedDisplayX,
        float expectedDisplayY)
    {
        // Arrange
        var virtualDisplay = CreateActualSizeCentredDisplay();
        var imageLocation = new PointF(imageX, imageY);

        // Act
        var actualDisplayLocation = virtualDisplay.MapImageToDisplay(imageLocation);

        // Bundle
        var result = new
        {
            actualDisplayLocation.X,
            actualDisplayLocation.Y,
        };
        var expected = new
        {
            X = expectedDisplayX,
            Y = expectedDisplayY,
        };

        // Verify
        result.Should().BeEquivalentTo(expected);
    }

    [TestMethod]
    [DataRow(300, 400, 0, 0)]
    [DataRow(500, 500, 200, 100)]
    public void MapDisplayToImage_ReturnsExpectedImageLocation(
        float displayX,
        float displayY,
        float expectedImageX,
        float expectedImageY)
    {
        // Arrange
        var virtualDisplay = CreateActualSizeCentredDisplay();
        var displayLocation = new PointF(displayX, displayY);

        // Act
        var actualImageLocation = virtualDisplay.MapDisplayToImage(displayLocation);

        // Bundle
        var result = new
        {
            actualImageLocation.X,
            actualImageLocation.Y,
        };
        var expected = new
        {
            X = expectedImageX,
            Y = expectedImageY,
        };

        // Verify
        result.Should().BeEquivalentTo(expected);
    }

    [TestMethod]
    public void MapDisplayRectToImage_ReturnsScaledRectangle()
    {
        // Arrange
        var virtualDisplay = CreateActualSizeCentredDisplay();
        var displayRect = new RectangleF(300, 400, 200, 100);

        // Act
        var actualImageRect = virtualDisplay.MapDisplayToImage(displayRect);

        // Bundle
        var expected = new RectangleF(0, 0, 200, 100);

        // Verify
        actualImageRect.Should().Be(expected);
    }

    [TestMethod]
    public void MapImageRectToDisplay_ReturnsScaledRectangle()
    {
        // Arrange
        var virtualDisplay = CreateActualSizeCentredDisplay();
        var imageRect = new RectangleF(0, 0, 200, 100);

        // Act
        var actualDisplayRect = virtualDisplay.MapImageToDisplay(imageRect);

        // Bundle
        var expected = new RectangleF(300, 400, 200, 100);

        // Verify
        actualDisplayRect.Should().Be(expected);
    }

    [TestMethod]
    public void ChangeImageSizeInFitToWindowMode_ResizesToFitWindow()
    {
        // Arrange
        var virtualDisplay = CreateSubject();
        virtualDisplay.Mode = BitmapDisplayMode.FitToWindowCentred;
        virtualDisplay.DisplaySize = new Size(2000, 1000);
        virtualDisplay.ImageSize = new Size(200, 200);

        // Act
        virtualDisplay.ImageSize = new Size(100, 1000);
        var result = CaptureDisplayState(virtualDisplay);

        // Bundle
        var expected = new
        {
            Zoom = 1.0f,
            PaintRect = new RectangleF(950, 0, 100, 1000),
            TargetDisplayCentre = new PointF(1000, 500),
            TargetImageCentre = new PointF(50, 500),
        };

        // Verify
        result.Should().BeEquivalentTo(expected);
    }

    [TestMethod]
    public void FreshImage_InFreeMode_HasZoomOneAndIsCentred()
    {
        // Arrange
        var virtualDisplay = CreateSubject();
        virtualDisplay.Mode = BitmapDisplayMode.Free;
        virtualDisplay.DisplaySize = new Size(2000, 1000);
        virtualDisplay.ImageSize = new Size(200, 200);

        // Act
        var result = CaptureDisplayState(virtualDisplay);

        // Bundle
        var expected = new
        {
            Zoom = 1.0f,
            PaintRect = new RectangleF(900, 400, 200, 200),
            TargetDisplayCentre = new PointF(1000, 500),
            TargetImageCentre = new PointF(100, 100),
        };

        // Verify
        result.Should().BeEquivalentTo(expected);
    }

    [TestMethod]
    public void ChangeImageSizeInFreeMode_LeavesZoomUnchanged()
    {
        // Arrange
        var virtualDisplay = CreateSubject();
        virtualDisplay.Mode = BitmapDisplayMode.Free;
        virtualDisplay.DisplaySize = new Size(2000, 1000);
        virtualDisplay.ImageSize = new Size(200, 200);
        virtualDisplay.Zoom = 2.5f;

        // Act
        virtualDisplay.ImageSize = new Size(300, 100);
        var result = CaptureDisplayState(virtualDisplay);

        // Bundle
        var expected = new
        {
            Zoom = 2.5f,
            PaintRect = new RectangleF(625, 375, 750, 250),
            TargetDisplayCentre = new PointF(1000, 500),
            TargetImageCentre = new PointF(150, 50),
            SizeOfHalfDisplayPixel = new SizeF(1.25f, 1.25f),
        };

        // Verify
        result.Should().BeEquivalentTo(expected);
    }

    [TestMethod]
    public void ChangeImageSizeInAutomaticMode_RecalculatesImageCentre()
    {
        // Arrange
        var virtualDisplay = CreateSubject();
        virtualDisplay.Mode = BitmapDisplayMode.FitToWindowCentred;
        virtualDisplay.DisplaySize = new Size(2000, 1000);
        virtualDisplay.ImageSize = new Size(200, 200);

        // Act
        virtualDisplay.ImageSize = new Size(100, 1000);
        var result = new
        {
            virtualDisplay.TargetImageCentre,
            virtualDisplay.TargetDisplayCentre,
        };

        // Bundle
        var expected = new
        {
            TargetImageCentre = new PointF(50, 500),
            TargetDisplayCentre = new PointF(1000, 500),
        };

        // Verify
        result.Should().BeEquivalentTo(expected);
    }

    [TestMethod]
    public void ChangeTargetImageCentre_NotInFreeMode_LeavesCentreUnchanged()
    {
        // Arrange
        var virtualDisplay = CreateSubject();
        virtualDisplay.DisplaySize = new Size(2000, 1000);
        virtualDisplay.ImageSize = new Size(200, 200);

        // Act
        virtualDisplay.Mode = BitmapDisplayMode.FitToWindowCentred;
        var fitToWindowOriginal = virtualDisplay.TargetImageCentre;
        virtualDisplay.TargetImageCentre = PointF.Empty;
        var fitToWindowResult = virtualDisplay.TargetImageCentre;

        virtualDisplay.Mode = BitmapDisplayMode.ActualSizeCentred;
        var actualSizeOriginal = virtualDisplay.TargetImageCentre;
        virtualDisplay.TargetImageCentre = PointF.Empty;
        var actualSizeResult = virtualDisplay.TargetImageCentre;

        var result = new
        {
            FitToWindowOriginal = fitToWindowOriginal,
            FitToWindowResult = fitToWindowResult,
            ActualSizeOriginal = actualSizeOriginal,
            ActualSizeResult = actualSizeResult,
        };

        // Bundle
        var expected = new
        {
            FitToWindowOriginal = new PointF(100, 100),
            FitToWindowResult = new PointF(100, 100),
            ActualSizeOriginal = new PointF(100, 100),
            ActualSizeResult = new PointF(100, 100),
        };

        // Verify
        result.Should().BeEquivalentTo(expected);
    }

    [TestMethod]
    public void ChangeTargetImageCentre_InFreeMode_ChangesCentre()
    {
        // Arrange
        var virtualDisplay = CreateSubject();
        virtualDisplay.DisplaySize = new Size(2000, 1000);
        virtualDisplay.ImageSize = new Size(200, 200);
        virtualDisplay.Mode = BitmapDisplayMode.Free;

        // Act
        virtualDisplay.TargetImageCentre = PointF.Empty;
        var result = new
        {
            virtualDisplay.TargetImageCentre,
            virtualDisplay.PaintRect,
        };

        // Bundle
        var expected = new
        {
            TargetImageCentre = PointF.Empty,
            PaintRect = new RectangleF(1000, 500, 1000, 1000),
        };

        // Verify
        result.Should().BeEquivalentTo(expected);
    }

    [TestMethod]
    public void ChangeTargetDisplayCentre_NotInFreeMode_LeavesCentreUnchanged()
    {
        // Arrange
        var virtualDisplay = CreateSubject();
        virtualDisplay.DisplaySize = new Size(2000, 1000);
        virtualDisplay.ImageSize = new Size(200, 200);

        // Act
        virtualDisplay.Mode = BitmapDisplayMode.FitToWindowCentred;
        var fitToWindowOriginal = virtualDisplay.TargetDisplayCentre;
        virtualDisplay.TargetDisplayCentre = PointF.Empty;
        var fitToWindowResult = virtualDisplay.TargetDisplayCentre;

        virtualDisplay.Mode = BitmapDisplayMode.ActualSizeCentred;
        var actualSizeOriginal = virtualDisplay.TargetDisplayCentre;
        virtualDisplay.TargetDisplayCentre = PointF.Empty;
        var actualSizeResult = virtualDisplay.TargetDisplayCentre;

        var result = new
        {
            FitToWindowOriginal = fitToWindowOriginal,
            FitToWindowResult = fitToWindowResult,
            ActualSizeOriginal = actualSizeOriginal,
            ActualSizeResult = actualSizeResult,
        };

        // Bundle
        var expected = new
        {
            FitToWindowOriginal = new PointF(1000, 500),
            FitToWindowResult = new PointF(1000, 500),
            ActualSizeOriginal = new PointF(1000, 500),
            ActualSizeResult = new PointF(1000, 500),
        };

        // Verify
        result.Should().BeEquivalentTo(expected);
    }

    [TestMethod]
    public void ChangeTargetDisplayCentre_InFreeMode_ChangesCentre()
    {
        // Arrange
        var virtualDisplay = CreateSubject();
        virtualDisplay.DisplaySize = new Size(2000, 1000);
        virtualDisplay.ImageSize = new Size(200, 200);
        virtualDisplay.Mode = BitmapDisplayMode.Free;

        // Act
        virtualDisplay.TargetDisplayCentre = PointF.Empty;
        var result = new
        {
            virtualDisplay.TargetDisplayCentre,
            virtualDisplay.PaintRect,
        };

        // Bundle
        var expected = new
        {
            TargetDisplayCentre = PointF.Empty,
            PaintRect = new RectangleF(-500, -500, 1000, 1000),
        };

        // Verify
        result.Should().BeEquivalentTo(expected);
    }

    [TestMethod]
    public void Zoom_WhenValueIsNearOne_SnapsToOne()
    {
        // Arrange
        var virtualDisplay = CreateConfiguredFreeDisplay();

        // Act
        virtualDisplay.Zoom = 1.005f;
        var result = new
        {
            virtualDisplay.Zoom,
            virtualDisplay.SizeOfHalfDisplayPixel,
            virtualDisplay.PaintRect,
        };

        // Bundle
        var expected = new
        {
            Zoom = 1.0f,
            SizeOfHalfDisplayPixel = new SizeF(0.5f, 0.5f),
            PaintRect = new RectangleF(400, 400, 200, 200),
        };

        // Verify
        result.Should().BeEquivalentTo(expected);
    }

    [TestMethod]
    public void Zoom_WhenValueIsBelowMinimum_ClipsToMinimum()
    {
        // Arrange
        var virtualDisplay = CreateConfiguredFreeDisplay();

        // Act
        virtualDisplay.Zoom = 0.001f;
        var result = new
        {
            virtualDisplay.Zoom,
            virtualDisplay.SizeOfHalfDisplayPixel,
            virtualDisplay.PaintRect,
        };

        // Bundle
        var expected = new
        {
            Zoom = Consts.MinZoom,
            SizeOfHalfDisplayPixel = new SizeF(Consts.MinZoom / 2, Consts.MinZoom / 2),
            PaintRect = new RectangleF(499, 499, 2, 2),
        };

        // Verify
        result.Should().BeEquivalentTo(expected);
    }

    [TestMethod]
    public void Zoom_WhenValueIsAboveMaximum_ClipsToMaximum()
    {
        // Arrange
        var virtualDisplay = CreateConfiguredFreeDisplay();

        // Act
        virtualDisplay.Zoom = 500.0f;
        var result = new
        {
            virtualDisplay.Zoom,
            virtualDisplay.SizeOfHalfDisplayPixel,
            virtualDisplay.PaintRect,
        };

        // Bundle
        var expected = new
        {
            Zoom = Consts.MaxZoom,
            SizeOfHalfDisplayPixel = new SizeF(Consts.MaxZoom / 2, Consts.MaxZoom / 2),
            PaintRect = new RectangleF(-19500, -19500, 40000, 40000),
        };

        // Verify
        result.Should().BeEquivalentTo(expected);
    }

    [TestMethod]
    public void MapMethods_WhenNothingToDisplay_ReturnEmptyValues()
    {
        // Arrange
        var virtualDisplay = CreateSubject();

        // Act
        var result = new
        {
            ImagePoint = virtualDisplay.MapDisplayToImage(new PointF(10, 20)),
            DisplayPoint = virtualDisplay.MapImageToDisplay(new PointF(10, 20)),
            ImageRect = virtualDisplay.MapDisplayToImage(new RectangleF(1, 2, 3, 4)),
            DisplayRect = virtualDisplay.MapImageToDisplay(new RectangleF(1, 2, 3, 4)),
            DisplayDistance = virtualDisplay.MapImageToDisplay(123),
        };

        // Bundle
        var expected = new
        {
            ImagePoint = PointF.Empty,
            DisplayPoint = PointF.Empty,
            ImageRect = RectangleF.Empty,
            DisplayRect = RectangleF.Empty,
            DisplayDistance = 0.0f,
        };

        // Verify
        result.Should().BeEquivalentTo(expected);
    }

    [TestMethod]
    public void MapImageDistanceToDisplay_ReturnsZoomScaledDistance()
    {
        // Arrange
        var virtualDisplay = CreateConfiguredFreeDisplay();

        // Act
        var actualDistance = virtualDisplay.MapImageToDisplay(12.5f);

        // Bundle
        const float expectedDistance = 62.5f;

        // Verify
        actualDistance.Should().Be(expectedDistance);
    }

    private static VirtualDisplay CreateSubject()
    {
        return new VirtualDisplay(onPaintRectChanged: (_, _) => { });
    }

    private static VirtualDisplay CreateActualSizeCentredDisplay()
    {
        var virtualDisplay = CreateSubject();
        virtualDisplay.Mode = BitmapDisplayMode.ActualSizeCentred;
        virtualDisplay.DisplaySize = new Size(1000, 1000);
        virtualDisplay.ImageSize = new Size(400, 200);
        return virtualDisplay;
    }

    private static VirtualDisplay CreateConfiguredFreeDisplay()
    {
        var virtualDisplay = CreateSubject();
        virtualDisplay.DisplaySize = new Size(1000, 1000);
        virtualDisplay.ImageSize = new Size(200, 200);
        virtualDisplay.Mode = BitmapDisplayMode.Free;
        virtualDisplay.Zoom = 5.0f;
        return virtualDisplay;
    }

    private static object CaptureDisplayState(VirtualDisplay virtualDisplay)
    {
        return new
        {
            virtualDisplay.Zoom,
            virtualDisplay.PaintRect,
            virtualDisplay.TargetDisplayCentre,
            virtualDisplay.TargetImageCentre,
            virtualDisplay.SizeOfHalfDisplayPixel,
            virtualDisplay.AnythingToDisplay,
        };
    }
}
