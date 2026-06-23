using System.Drawing;
using AwesomeAssertions;
using CDS.ImageDisplay.WinForms.BitmapDisplay;

namespace UnitTests.BitmapDisplay;

/// <summary>
/// Tests for <see cref="VirtualDisplay"/>.
/// </summary>
[TestClass]
public sealed class VirtualDisplayTests
{
    /// <summary>
    /// Verifies that the default state uses fit-to-window mode.
    /// </summary>
    [TestMethod]
    public void ConstructorDefaultStateUsesFitToWindowMode()
    {
        // Arrange
        VirtualDisplay virtualDisplay = CreateSubject();

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

        // Assert
        var expected = new
        {
            Mode = BitmapDisplayMode.FitToWindowCentred,
            Zoom = 1.0f,
            AnythingToDisplay = false,
            PaintRect = RectangleF.Empty,
            TargetDisplayCentre = PointF.Empty,
            TargetImageCentre = PointF.Empty,
        };

        result.Should().BeEquivalentTo(expected);
    }

    /// <summary>
    /// Verifies that fit-to-window mode fills the display when the image and display sizes match.
    /// </summary>
    [TestMethod]
    public void FitToWindowModeImageSameSizeAsDisplayFillsDisplayExactly()
    {
        // Arrange
        VirtualDisplay virtualDisplay = CreateSubject();
        virtualDisplay.DisplaySize = new Size(1000, 600);
        virtualDisplay.ImageSize = virtualDisplay.DisplaySize;
        virtualDisplay.Mode = BitmapDisplayMode.FitToWindowCentred;

        // Act
        object result = CaptureDisplayState(virtualDisplay);

        // Assert
        var expected = new
        {
            Zoom = 1.0f,
            PaintRect = new RectangleF(0, 0, 1000, 600),
            TargetDisplayCentre = new PointF(500, 300),
            TargetImageCentre = new PointF(500, 300),
            SizeOfHalfDisplayPixel = new SizeF(0.5f, 0.5f),
            AnythingToDisplay = true,
        };

        result.Should().BeEquivalentTo(expected);
    }

    /// <summary>
    /// Verifies that actual-size mode fills the display when the image and display sizes match.
    /// </summary>
    [TestMethod]
    public void ActualSizeModeImageSameSizeAsDisplayFillsDisplayExactly()
    {
        // Arrange
        VirtualDisplay virtualDisplay = CreateSubject();
        virtualDisplay.DisplaySize = new Size(1000, 600);
        virtualDisplay.ImageSize = virtualDisplay.DisplaySize;
        virtualDisplay.Mode = BitmapDisplayMode.ActualSizeCentred;

        // Act
        object result = CaptureDisplayState(virtualDisplay);

        // Assert
        var expected = new
        {
            Zoom = 1.0f,
            PaintRect = new RectangleF(0, 0, 1000, 600),
            TargetDisplayCentre = new PointF(500, 300),
            TargetImageCentre = new PointF(500, 300),
            SizeOfHalfDisplayPixel = new SizeF(0.5f, 0.5f),
            AnythingToDisplay = true,
        };

        result.Should().BeEquivalentTo(expected);
    }

    /// <summary>
    /// Verifies that fit-to-window mode maximizes the display area for each sample.
    /// </summary>
    [TestMethod]
    [DynamicData(nameof(FitToWindowSampleData.Data), typeof(FitToWindowSampleData))]
    public void FitToWindowMaximizesDisplayArea(Size imageSize, Size displaySize, RectangleF expectedPaintRect)
    {
        // Arrange
        VirtualDisplay virtualDisplay = CreateSubject();
        virtualDisplay.DisplaySize = displaySize;
        virtualDisplay.ImageSize = imageSize;

        // Act
        RectangleF actualPaintRect = virtualDisplay.PaintRect;
        float actualZoom = virtualDisplay.Zoom;

        // Assert
        float expectedZoom = expectedPaintRect.Width / imageSize.Width;

        actualPaintRect.X.Should().BeApproximately(expectedPaintRect.X, 0.01f);
        actualPaintRect.Y.Should().BeApproximately(expectedPaintRect.Y, 0.01f);
        actualPaintRect.Width.Should().BeApproximately(expectedPaintRect.Width, 0.02f);
        actualPaintRect.Height.Should().BeApproximately(expectedPaintRect.Height, 0.02f);
        actualZoom.Should().BeApproximately(expectedZoom, 0.00001f);
    }

    /// <summary>
    /// Verifies that changing the target image centre in free mode moves the paint rectangle.
    /// </summary>
    [TestMethod]
    public void ChangeTargetImageCentreInFreeModeMovesPaintRect()
    {
        // Arrange
        VirtualDisplay virtualDisplay = CreateConfiguredFreeDisplay();

        // Act
        virtualDisplay.TargetImageCentre = PointF.Empty;
        object result = CaptureDisplayState(virtualDisplay);

        // Assert
        var expected = new
        {
            Zoom = 5.0f,
            PaintRect = new RectangleF(500, 500, 1000, 1000),
            TargetDisplayCentre = new PointF(500, 500),
            TargetImageCentre = PointF.Empty,
        };

        result.Should().BeEquivalentTo(expected);
    }

    /// <summary>
    /// Verifies that the default initialization fits the image to the display.
    /// </summary>
    [TestMethod]
    public void DefaultInitializationWithDisplayAndImageFitsToDisplay()
    {
        // Arrange
        VirtualDisplay virtualDisplay = CreateSubject();
        virtualDisplay.DisplaySize = new Size(1000, 1000);
        virtualDisplay.ImageSize = new Size(100, 100);

        // Act
        object result = CaptureDisplayState(virtualDisplay);

        // Assert
        var expected = new
        {
            Zoom = 10.0f,
            PaintRect = new RectangleF(0, 0, 1000, 1000),
            TargetDisplayCentre = new PointF(500, 500),
            TargetImageCentre = new PointF(50, 50),
        };

        result.Should().BeEquivalentTo(expected);
    }

    /// <summary>
    /// Verifies that mapping image coordinates to display coordinates returns the expected location.
    /// </summary>
    [TestMethod]
    [DataRow(0, 0, 300, 400)]
    [DataRow(200, 100, 500, 500)]
    public void MapImageToDisplayReturnsExpectedDisplayLocation(
        float imageX,
        float imageY,
        float expectedDisplayX,
        float expectedDisplayY)
    {
        // Arrange
        VirtualDisplay virtualDisplay = CreateActualSizeCentredDisplay();
        var imageLocation = new PointF(imageX, imageY);

        // Act
        PointF actualDisplayLocation = virtualDisplay.MapImageToDisplay(imageLocation);

        // Assert
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

        result.Should().BeEquivalentTo(expected);
    }

    /// <summary>
    /// Verifies that mapping display coordinates to image coordinates returns the expected location.
    /// </summary>
    [TestMethod]
    [DataRow(300, 400, 0, 0)]
    [DataRow(500, 500, 200, 100)]
    public void MapDisplayToImageReturnsExpectedImageLocation(
        float displayX,
        float displayY,
        float expectedImageX,
        float expectedImageY)
    {
        // Arrange
        VirtualDisplay virtualDisplay = CreateActualSizeCentredDisplay();
        var displayLocation = new PointF(displayX, displayY);

        // Act
        PointF actualImageLocation = virtualDisplay.MapDisplayToImage(displayLocation);

        // Assert
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

        result.Should().BeEquivalentTo(expected);
    }

    /// <summary>
    /// Verifies that mapping a display rectangle to image coordinates returns the expected rectangle.
    /// </summary>
    [TestMethod]
    public void MapDisplayRectToImageReturnsScaledRectangle()
    {
        // Arrange
        VirtualDisplay virtualDisplay = CreateActualSizeCentredDisplay();
        var displayRect = new RectangleF(300, 400, 200, 100);

        // Act
        RectangleF actualImageRect = virtualDisplay.MapDisplayToImage(displayRect);

        // Assert
        var expected = new RectangleF(0, 0, 200, 100);

        actualImageRect.Should().Be(expected);
    }

    /// <summary>
    /// Verifies that mapping an image rectangle to display coordinates returns the expected rectangle.
    /// </summary>
    [TestMethod]
    public void MapImageRectToDisplayReturnsScaledRectangle()
    {
        // Arrange
        VirtualDisplay virtualDisplay = CreateActualSizeCentredDisplay();
        var imageRect = new RectangleF(0, 0, 200, 100);

        // Act
        RectangleF actualDisplayRect = virtualDisplay.MapImageToDisplay(imageRect);

        // Assert
        var expected = new RectangleF(300, 400, 200, 100);

        actualDisplayRect.Should().Be(expected);
    }

    /// <summary>
    /// Verifies that changing the image size in fit-to-window mode resizes the image to fit the window.
    /// </summary>
    [TestMethod]
    public void ChangeImageSizeInFitToWindowModeResizesToFitWindow()
    {
        // Arrange
        VirtualDisplay virtualDisplay = CreateSubject();
        virtualDisplay.Mode = BitmapDisplayMode.FitToWindowCentred;
        virtualDisplay.DisplaySize = new Size(2000, 1000);
        virtualDisplay.ImageSize = new Size(200, 200);

        // Act
        virtualDisplay.ImageSize = new Size(100, 1000);
        object result = CaptureDisplayState(virtualDisplay);

        // Assert
        var expected = new
        {
            Zoom = 1.0f,
            PaintRect = new RectangleF(950, 0, 100, 1000),
            TargetDisplayCentre = new PointF(1000, 500),
            TargetImageCentre = new PointF(50, 500),
        };

        result.Should().BeEquivalentTo(expected);
    }

    /// <summary>
    /// Verifies that a fresh image in free mode has zoom one and is centered.
    /// </summary>
    [TestMethod]
    public void FreshImageInFreeModeHasZoomOneAndIsCentred()
    {
        // Arrange
        VirtualDisplay virtualDisplay = CreateSubject();
        virtualDisplay.Mode = BitmapDisplayMode.Free;
        virtualDisplay.DisplaySize = new Size(2000, 1000);
        virtualDisplay.ImageSize = new Size(200, 200);

        // Act
        object result = CaptureDisplayState(virtualDisplay);

        // Assert
        var expected = new
        {
            Zoom = 1.0f,
            PaintRect = new RectangleF(900, 400, 200, 200),
            TargetDisplayCentre = new PointF(1000, 500),
            TargetImageCentre = new PointF(100, 100),
        };

        result.Should().BeEquivalentTo(expected);
    }

    /// <summary>
    /// Verifies that changing the image size in free mode leaves the zoom unchanged.
    /// </summary>
    [TestMethod]
    public void ChangeImageSizeInFreeModeLeavesZoomUnchanged()
    {
        // Arrange
        VirtualDisplay virtualDisplay = CreateSubject();
        virtualDisplay.Mode = BitmapDisplayMode.Free;
        virtualDisplay.DisplaySize = new Size(2000, 1000);
        virtualDisplay.ImageSize = new Size(200, 200);
        virtualDisplay.Zoom = 2.5f;

        // Act
        virtualDisplay.ImageSize = new Size(300, 100);
        object result = CaptureDisplayState(virtualDisplay);

        // Assert
        var expected = new
        {
            Zoom = 2.5f,
            PaintRect = new RectangleF(625, 375, 750, 250),
            TargetDisplayCentre = new PointF(1000, 500),
            TargetImageCentre = new PointF(150, 50),
            SizeOfHalfDisplayPixel = new SizeF(1.25f, 1.25f),
        };

        result.Should().BeEquivalentTo(expected);
    }

    /// <summary>
    /// Verifies that changing the image size in automatic mode recalculates the image centre.
    /// </summary>
    [TestMethod]
    public void ChangeImageSizeInAutomaticModeRecalculatesImageCentre()
    {
        // Arrange
        VirtualDisplay virtualDisplay = CreateSubject();
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

        // Assert
        var expected = new
        {
            TargetImageCentre = new PointF(50, 500),
            TargetDisplayCentre = new PointF(1000, 500),
        };

        result.Should().BeEquivalentTo(expected);
    }

    /// <summary>
    /// Verifies that changing the target image centre outside free mode leaves the centre unchanged.
    /// </summary>
    [TestMethod]
    public void ChangeTargetImageCentreNotInFreeModeLeavesCentreUnchanged()
    {
        // Arrange
        VirtualDisplay virtualDisplay = CreateSubject();
        virtualDisplay.DisplaySize = new Size(2000, 1000);
        virtualDisplay.ImageSize = new Size(200, 200);

        // Act
        virtualDisplay.Mode = BitmapDisplayMode.FitToWindowCentred;
        PointF fitToWindowOriginal = virtualDisplay.TargetImageCentre;
        virtualDisplay.TargetImageCentre = PointF.Empty;
        PointF fitToWindowResult = virtualDisplay.TargetImageCentre;

        virtualDisplay.Mode = BitmapDisplayMode.ActualSizeCentred;
        PointF actualSizeOriginal = virtualDisplay.TargetImageCentre;
        virtualDisplay.TargetImageCentre = PointF.Empty;
        PointF actualSizeResult = virtualDisplay.TargetImageCentre;

        var result = new
        {
            FitToWindowOriginal = fitToWindowOriginal,
            FitToWindowResult = fitToWindowResult,
            ActualSizeOriginal = actualSizeOriginal,
            ActualSizeResult = actualSizeResult,
        };

        // Assert
        var expected = new
        {
            FitToWindowOriginal = new PointF(100, 100),
            FitToWindowResult = new PointF(100, 100),
            ActualSizeOriginal = new PointF(100, 100),
            ActualSizeResult = new PointF(100, 100),
        };

        result.Should().BeEquivalentTo(expected);
    }

    /// <summary>
    /// Verifies that changing the target image centre in free mode changes the centre.
    /// </summary>
    [TestMethod]
    public void ChangeTargetImageCentreInFreeModeChangesCentre()
    {
        // Arrange
        VirtualDisplay virtualDisplay = CreateSubject();
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

        // Assert
        var expected = new
        {
            TargetImageCentre = PointF.Empty,
            PaintRect = new RectangleF(1000, 500, 1000, 1000),
        };

        result.Should().BeEquivalentTo(expected);
    }

    /// <summary>
    /// Verifies that changing the target display centre outside free mode leaves the centre unchanged.
    /// </summary>
    [TestMethod]
    public void ChangeTargetDisplayCentreNotInFreeModeLeavesCentreUnchanged()
    {
        // Arrange
        VirtualDisplay virtualDisplay = CreateSubject();
        virtualDisplay.DisplaySize = new Size(2000, 1000);
        virtualDisplay.ImageSize = new Size(200, 200);

        // Act
        virtualDisplay.Mode = BitmapDisplayMode.FitToWindowCentred;
        PointF fitToWindowOriginal = virtualDisplay.TargetDisplayCentre;
        virtualDisplay.TargetDisplayCentre = PointF.Empty;
        PointF fitToWindowResult = virtualDisplay.TargetDisplayCentre;

        virtualDisplay.Mode = BitmapDisplayMode.ActualSizeCentred;
        PointF actualSizeOriginal = virtualDisplay.TargetDisplayCentre;
        virtualDisplay.TargetDisplayCentre = PointF.Empty;
        PointF actualSizeResult = virtualDisplay.TargetDisplayCentre;

        var result = new
        {
            FitToWindowOriginal = fitToWindowOriginal,
            FitToWindowResult = fitToWindowResult,
            ActualSizeOriginal = actualSizeOriginal,
            ActualSizeResult = actualSizeResult,
        };

        // Assert
        var expected = new
        {
            FitToWindowOriginal = new PointF(1000, 500),
            FitToWindowResult = new PointF(1000, 500),
            ActualSizeOriginal = new PointF(1000, 500),
            ActualSizeResult = new PointF(1000, 500),
        };

        result.Should().BeEquivalentTo(expected);
    }

    /// <summary>
    /// Verifies that changing the target display centre in free mode changes the centre.
    /// </summary>
    [TestMethod]
    public void ChangeTargetDisplayCentreInFreeModeChangesCentre()
    {
        // Arrange
        VirtualDisplay virtualDisplay = CreateSubject();
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

        // Assert
        var expected = new
        {
            TargetDisplayCentre = PointF.Empty,
            PaintRect = new RectangleF(-500, -500, 1000, 1000),
        };

        result.Should().BeEquivalentTo(expected);
    }

    /// <summary>
    /// Verifies that zoom values near one snap to one.
    /// </summary>
    [TestMethod]
    public void ZoomWhenValueIsNearOneSnapsToOne()
    {
        // Arrange
        VirtualDisplay virtualDisplay = CreateConfiguredFreeDisplay();

        // Act
        virtualDisplay.Zoom = 1.005f;
        var result = new
        {
            virtualDisplay.Zoom,
            virtualDisplay.SizeOfHalfDisplayPixel,
            virtualDisplay.PaintRect,
        };

        // Assert
        var expected = new
        {
            Zoom = 1.0f,
            SizeOfHalfDisplayPixel = new SizeF(0.5f, 0.5f),
            PaintRect = new RectangleF(400, 400, 200, 200),
        };

        result.Should().BeEquivalentTo(expected);
    }

    /// <summary>
    /// Verifies that zoom values below the minimum are clipped to the minimum.
    /// </summary>
    [TestMethod]
    public void ZoomWhenValueIsBelowMinimumClipsToMinimum()
    {
        // Arrange
        VirtualDisplay virtualDisplay = CreateConfiguredFreeDisplay();

        // Act
        virtualDisplay.Zoom = 0.001f;
        var result = new
        {
            virtualDisplay.Zoom,
            virtualDisplay.SizeOfHalfDisplayPixel,
            virtualDisplay.PaintRect,
        };

        // Assert
        var expected = new
        {
            Zoom = Consts.MinZoom,
            SizeOfHalfDisplayPixel = new SizeF(Consts.MinZoom / 2, Consts.MinZoom / 2),
            PaintRect = new RectangleF(499, 499, 2, 2),
        };

        result.Should().BeEquivalentTo(expected);
    }

    /// <summary>
    /// Verifies that zoom values above the maximum are clipped to the maximum.
    /// </summary>
    [TestMethod]
    public void ZoomWhenValueIsAboveMaximumClipsToMaximum()
    {
        // Arrange
        VirtualDisplay virtualDisplay = CreateConfiguredFreeDisplay();

        // Act
        virtualDisplay.Zoom = 500.0f;
        var result = new
        {
            virtualDisplay.Zoom,
            virtualDisplay.SizeOfHalfDisplayPixel,
            virtualDisplay.PaintRect,
        };

        // Assert
        var expected = new
        {
            Zoom = Consts.MaxZoom,
            SizeOfHalfDisplayPixel = new SizeF(Consts.MaxZoom / 2, Consts.MaxZoom / 2),
            PaintRect = new RectangleF(-19500, -19500, 40000, 40000),
        };

        result.Should().BeEquivalentTo(expected);
    }

    /// <summary>
    /// Verifies that mapping methods return empty values when there is nothing to display.
    /// </summary>
    [TestMethod]
    public void MapMethodsWhenNothingToDisplayReturnEmptyValues()
    {
        // Arrange
        VirtualDisplay virtualDisplay = CreateSubject();

        // Act
        var result = new
        {
            ImagePoint = virtualDisplay.MapDisplayToImage(new PointF(10, 20)),
            DisplayPoint = virtualDisplay.MapImageToDisplay(new PointF(10, 20)),
            ImageRect = virtualDisplay.MapDisplayToImage(new RectangleF(1, 2, 3, 4)),
            DisplayRect = virtualDisplay.MapImageToDisplay(new RectangleF(1, 2, 3, 4)),
            DisplayDistance = virtualDisplay.MapImageToDisplay(123),
        };

        // Assert
        var expected = new
        {
            ImagePoint = PointF.Empty,
            DisplayPoint = PointF.Empty,
            ImageRect = RectangleF.Empty,
            DisplayRect = RectangleF.Empty,
            DisplayDistance = 0.0f,
        };

        result.Should().BeEquivalentTo(expected);
    }

    /// <summary>
    /// Verifies that mapping image distance to display distance scales by the zoom factor.
    /// </summary>
    [TestMethod]
    public void MapImageDistanceToDisplayReturnsZoomScaledDistance()
    {
        // Arrange
        VirtualDisplay virtualDisplay = CreateConfiguredFreeDisplay();

        // Act
        float actualDistance = virtualDisplay.MapImageToDisplay(12.5f);

        // Assert
        const float expectedDistance = 62.5f;

        actualDistance.Should().Be(expectedDistance);
    }

    /// <summary>
    /// Verifies that MapImageToDisplayScaleFactor defaults to 1.0 and has no effect at that value.
    /// </summary>
    [TestMethod]
    public void MapImageToDisplayScaleFactorDefaultIsOne()
    {
        // Arrange
        VirtualDisplay virtualDisplay = CreateActualSizeCentredDisplay();

        // Act / Assert
        virtualDisplay.MapImageToDisplayScaleFactor.Should().Be(1.0f);
    }

    /// <summary>
    /// Verifies that MapImageToDisplayScaleFactor is clamped at 0.01.
    /// </summary>
    [TestMethod]
    public void MapImageToDisplayScaleFactorClampsToMinimum()
    {
        // Arrange
        VirtualDisplay virtualDisplay = CreateSubject();

        // Act
        virtualDisplay.MapImageToDisplayScaleFactor = 0.0f;

        // Assert
        virtualDisplay.MapImageToDisplayScaleFactor.Should().Be(0.01f);
    }

    /// <summary>
    /// Verifies that MapImageToDisplayScaleFactor is clamped at 100.
    /// </summary>
    [TestMethod]
    public void MapImageToDisplayScaleFactorClampsToMaximum()
    {
        // Arrange
        VirtualDisplay virtualDisplay = CreateSubject();

        // Act
        virtualDisplay.MapImageToDisplayScaleFactor = 200.0f;

        // Assert
        virtualDisplay.MapImageToDisplayScaleFactor.Should().Be(100.0f);
    }

    /// <summary>
    /// Verifies that a scale factor of 0.5 halves image coordinates before mapping to display.
    /// </summary>
    [TestMethod]
    public void MapImageToDisplayWithHalfScaleFactorHalvesImageCoordinate()
    {
        // Arrange — actual-size centred, 400×200 image in 1000×1000 display.
        // PaintRect = (300, 400, 400, 200). At scale 1.0, image (200, 100) → display (500, 500).
        // At scale 0.5, image (200, 100) is treated as (100, 50) → display (400, 450).
        VirtualDisplay virtualDisplay = CreateActualSizeCentredDisplay();
        virtualDisplay.MapImageToDisplayScaleFactor = 0.5f;

        // Act
        PointF result = virtualDisplay.MapImageToDisplay(new PointF(200, 100));

        // Assert
        result.Should().BeEquivalentTo(new PointF(400, 450));
    }

    /// <summary>
    /// Verifies that MapDisplayToImage with a scale factor of 0.5 doubles the image coordinate returned.
    /// </summary>
    [TestMethod]
    public void MapDisplayToImageWithHalfScaleFactorDoublesImageCoordinate()
    {
        // Arrange — at scale 1.0, display (400, 450) → image (100, 50).
        // With scale 0.5, we divide by 0.5, so the result is (200, 100).
        VirtualDisplay virtualDisplay = CreateActualSizeCentredDisplay();
        virtualDisplay.MapImageToDisplayScaleFactor = 0.5f;

        // Act
        PointF result = virtualDisplay.MapDisplayToImage(new PointF(400, 450));

        // Assert
        result.Should().BeEquivalentTo(new PointF(200, 100));
    }

    /// <summary>
    /// Verifies that MapDisplayToImage with ignoreScaleFactor=true returns raw image coords
    /// regardless of the scale factor set.
    /// </summary>
    [TestMethod]
    public void MapDisplayToImageIgnoringScaleFactorReturnsRawImageCoordinate()
    {
        // Arrange — same setup as above. Raw result at display (400, 450) is image (100, 50).
        VirtualDisplay virtualDisplay = CreateActualSizeCentredDisplay();
        virtualDisplay.MapImageToDisplayScaleFactor = 0.5f;

        // Act
        PointF result = virtualDisplay.MapDisplayToImage(new PointF(400, 450), ignoreScaleFactor: true);

        // Assert
        result.Should().BeEquivalentTo(new PointF(100, 50));
    }

    /// <summary>
    /// Verifies that MapImageToDisplay(float) scales the distance by both the scale factor and zoom.
    /// </summary>
    [TestMethod]
    public void MapImageDistanceToDisplayWithScaleFactorAppliesScaleAndZoom()
    {
        // Arrange — zoom = 5, scale factor = 0.5; result = 12.5 * 0.5 * 5 = 31.25
        VirtualDisplay virtualDisplay = CreateConfiguredFreeDisplay();
        virtualDisplay.MapImageToDisplayScaleFactor = 0.5f;

        // Act
        float result = virtualDisplay.MapImageToDisplay(12.5f);

        // Assert
        result.Should().Be(31.25f);
    }

    private static VirtualDisplay CreateSubject() => new(onPaintRectChanged: (_, _) => { });

    private static VirtualDisplay CreateActualSizeCentredDisplay()
    {
        VirtualDisplay virtualDisplay = CreateSubject();
        virtualDisplay.Mode = BitmapDisplayMode.ActualSizeCentred;
        virtualDisplay.DisplaySize = new Size(1000, 1000);
        virtualDisplay.ImageSize = new Size(400, 200);
        return virtualDisplay;
    }

    private static VirtualDisplay CreateConfiguredFreeDisplay()
    {
        VirtualDisplay virtualDisplay = CreateSubject();
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
