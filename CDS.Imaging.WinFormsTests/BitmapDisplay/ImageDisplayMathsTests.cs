using System;
using System.Drawing;
using Xunit;
using FluentAssertions;

namespace CDS.Imaging.WinFormsTests
{
    public class ImageDisplayMathsTests
    {
        /// <summary>
        /// An image drawn at 1:1 with its centre 'c' located at the 
        /// same location 'c' on the display will have its loction at
        /// (0, 0).
        /// </summary>
        [Fact]
        public void CalcDrawRect_ImageAtTopLeftZoom1_IsLocatedAtTopleft()
        {
            var imageSize = new Size(200, 100);
            var imageCentre = new Point(imageSize.Width / 2, imageSize.Height / 2);

            var drawRect = WinForms.BitmapDisplay.ImageDisplayMaths.CalcDrawRect(
                imageSize: imageSize,
                imageZoom: 1,
                targetDisplayCentre: imageCentre,
                targetImageCentre: imageCentre);

            drawRect.
                Location.
                Should().
                Be(new PointF(0, 0));
        }


        /// <summary>
        /// The rendering rectangle for an image drawn at 1:1 will have the
        /// same size as the image
        /// </summary>
        [Fact]
        public void CalcDrawRect_ImageWithZoom1_RenderedWithImageSize()
        {
            var imageSize = new Size(200, 100);

            var drawRect = WinForms.BitmapDisplay.ImageDisplayMaths.CalcDrawRect(
                imageSize: imageSize,
                imageZoom: 1,
                targetDisplayCentre: Point.Empty,
                targetImageCentre: Point.Empty);

            drawRect.
                Size.
                Should().
                Be(new SizeF(imageSize.Width, imageSize.Height));
        }


        /// <summary>
        /// Drawing an image using actual size / centred on the display, should have
        /// its centre of drawing in the centre of the display
        /// </summary>
        [Fact]
        public void CalcActualSizeCenteredRect_Has_CentreLocationOnDisplay()
        {
            var imageSize = new Size(200, 100);
            var displaySize = new Size(1024, 768);

            var drawRect = WinForms.BitmapDisplay.ImageDisplayMaths.CalcActualSizeCentredRect(
                imageSize: imageSize,
                displaySize: displaySize);


            var displayCentre = new PointF(
                x: displaySize.Width / 2,
                y: displaySize.Height / 2);

            var renderCentre = new PointF(
                x: drawRect.X + (drawRect.Width / 2),
                y: drawRect.Y + (drawRect.Height / 2));

            displayCentre.Should().Be(renderCentre);
        }



        /// <summary>
        /// Rendering rectangle for an image drawn actual size in the centre should
        /// be the same size as the image
        /// </summary>
        [Fact]
        public void CalcActualSizeCenteredRect_Has_ImageSize()
        {
            var imageSize = new Size(200, 100);
            var displaySize = new Size(1024, 768);

            var drawRect = WinForms.BitmapDisplay.ImageDisplayMaths.CalcActualSizeCentredRect(
                imageSize: imageSize,
                displaySize: displaySize);

            drawRect.Size.Should().Be(new SizeF(imageSize.Width, imageSize.Height));
        }


        [Theory]
        [InlineData(100, 100, 1)]
        [InlineData(100, 200, 2)]
        [InlineData(200, 100, 0.5)]
        public void CalcImageZoom_GiveCorrect_Result(int imageWidth, float renderWidth, double expectedZoom)
        {
            var zoom = WinForms.BitmapDisplay.ImageDisplayMaths.CalcZoom(imageWidth, renderWidth);

            zoom.Should().BeApproximately(expectedZoom, precision: 0.001);
        }


        [Fact]
        public void Centre_ZoomedOutImage_ShouldCentreOnDisplay()
        {
            var imageSize = new Size(400, 200);
            var zoom = 0.5;

            var existingDisplayRect = new RectangleF(
                x: 0,
                y: 0, 
                width: (float)(imageSize.Width * zoom),
                height: (float)(imageSize.Height * zoom));

            var displaySize = new Size(1000, 800);

            var newRect = WinForms.BitmapDisplay.ImageDisplayMaths.CalcCentredRect(
                displaySize: displaySize,
                existingRect: existingDisplayRect,
                imageSize: imageSize);

            var newRectCentre = new PointF(
                x: newRect.X + (newRect.Width / 2),
                y: newRect.Y + (newRect.Height / 2));

            var displayCentre = new PointF(
                x: displaySize.Width / 2,
                y: displaySize.Height / 2);

            newRectCentre.Should().Be(displayCentre);
        }
    }
}
