using System;
using System.Drawing;
using Xunit;
using FluentAssertions;

namespace CDS.Imaging.WinFormsTests
{
    public class BitmapOnDisplayTests
    {
        /// <summary>
        /// Forcing a redraw of an image at actual size and centred will
        /// centre the image at 1:1.
        /// </summary>
        [Fact]
        public void ActualSizeCentred_Should_CentreImageActualSize()
        {
            Size displaySize = new Size(1000, 600);
            using Bitmap bitmap = new Bitmap(500, 300, System.Drawing.Imaging.PixelFormat.Format8bppIndexed);

            using var bitmapOnDisplay = new WinForms.BitmapDisplay.BitmapOnDisplay(
                () => { },
                (r) => { },
                getClientSize: () => displaySize);

            bitmapOnDisplay.Mode = WinForms.BitmapDisplay.ImageDisplayMode.Free;
            bitmapOnDisplay.SetImage(bitmap);
            bitmapOnDisplay.ActualSizeCentred();
            bitmapOnDisplay.DisplayRect.Should().Be(new RectangleF(250, 150, 500, 300));
        }


        ///// <summary>
        ///// Bug check: image drawn at 1:2 and then told to centre did not
        ///// centre correctly.
        ///// </summary>
        //[Fact]
        //public void ZoomedOutAndCentred_Should_CentreOnDisplay()
        //{
        //    Size displaySize = new Size(1000, 600);
        //    using Bitmap bitmap = new Bitmap(500, 300, System.Drawing.Imaging.PixelFormat.Format8bppIndexed);

        //    using var bitmapOnDisplay = new WinForms.BitmapDisplay.BitmapOnDisplay(
        //        () => { },
        //        (r) => { },
        //        getClientSize: () => displaySize);

        //    bitmapOnDisplay.Mode = WinForms.BitmapDisplay.ImageDisplayMode.Free;
        //    bitmapOnDisplay.SetImage(bitmap);

        //    bitmapOnDisplay.MoveImage(
        //        imageCentre: new PointF(bitmap.Width / 2, bitmap.Height / 2),
        //        displayCentre: new PointF(displaySize.Width / 2, displaySize.Height / 2),
        //        zoom: 0.5);

        //    bitmapOnDisplay.DisplayRect.Should().Be(new RectangleF(
        //        x: (displaySize.Width / 2) - (bitmap.Width * zoom),
        //}
    }
}

