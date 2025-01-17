using CDS.Imaging.WinForms.BitmapDisplay;
using System;
using System.Drawing;

namespace CDS.Imaging.WinForms.RegionOfInterest
{
    /// <summary>
    /// A descriptor for a single ROI, The <see cref="MultipleROIManager"/> uses these
    /// to manage multiple ROIs on a bitmap display.
    /// </summary>
    public class SingleROIDescriptor : IDisposable, ISingleROIDescriptor
    {
        /// <summary>
        /// This instance in string form.
        /// </summary>
        /// <returns></returns>
        public override string ToString() => Name;


        /// <inheritdoc/>
        public bool Visible { get; set; } = true;


        /// <inheritdoc/>
        public bool Locked { get; set; } = false;


        /// <inheritdoc/>
        public string Name { get; set; } = "";


        /// <inheritdoc/>
        public Rectangle ROI { get; set; }


        /// <inheritdoc/>
        public Size MinimumSize { get; set; } = new Size(1, 1);


        /// <inheritdoc/>
        public Size MaximumSize { get; set; } = new Size(1000000, 1000000);


        /// <inheritdoc/>
        public RectangleRenderer Renderer { get; } = new RectangleRenderer()
        {
            GrapplesMode = RectangleRenderer.GrapplesRenderingMode.Hide,
        };


        /// <inheritdoc/>
        public void Dispose()
        {
            Renderer.Dispose();
        }


        /// <inheritdoc/>
        public void Draw(Graphics graphics, BitmapDisplayPanel bitmapDisplay, Rectangle roiOnImage)
        {
            if(!Visible) { return; }

            var displayRect = bitmapDisplay.MapImageToDisplay(roiOnImage, pixelAdjust: DisplayPixelAlign.TopLeft);
            Renderer.Draw(graphics, displayRect);
        }
    }
}
