using System.Drawing;

namespace CDS.Imaging.WinForms.BitmapDisplay
{
    /// <summary>
    /// Behaviour for a bitmap display control
    /// </summary>
    public interface IBitmapDisplay
    {
        /// <summary>
        /// Gets the paint rectangle. This is based on the current image size,
        /// display size, zoom, target image centre and target display centre.
        /// </summary>
        RectangleF PaintRect { get; }


        /// <summary>
        /// Gets the image currently being rendered. This is owned by the display 
        /// control and should not be modified by clients.
        /// </summary>
        Image? Image { get; }


        /// <summary>
        /// True if there's anything to display
        /// </summary>
        bool AnythingToDisplay { get; }


        /// <summary>
        /// The image display mode
        /// </summary>
        BitmapDisplayMode Mode { get; set; }


        /// <summary>
        /// Timing metrics
        /// </summary>
        BitmapDisplayMetrics TimingMetrics { get; }


        /// <summary>
        /// Called when the image has been rendered; gives a client an opportunity
        /// to paint graphics on top of the image. This will be flicker-free as long
        /// as the control uses double buffering
        /// </summary>
        event PaintOverEvent? PaintOver;


        /// <summary>
        /// Called afer the background has been painted and before the image been painted;
        /// gives a client an opportunity to paint graphics under the image. 
        /// This will be flicker-free as long as the control uses double buffering
        /// </summary>
        event PaintUnderEvent? PaintUnder;


        /// <summary>
        /// Called when the display mode is changed.
        /// </summary>
        event ModeChangedEvent? DisplayModeChanged;



        /// <summary>
        /// Returns the display location where a pixel at <paramref name="imageLocation"/> would be drawn.
        /// </summary>
        /// <remarks>
        /// This is useful when you want to overlay graphics on the image with respect to the 
        /// image regardless of the current zoom and display settings.
        /// 
        /// When the zoom is large each image pixel will occupy several display pixels. The 
        /// returned coordinate is at the top-left of this region. The <see cref="SizeOfHalfDisplayPixel"/>
        /// property can be used if the drawing should use the centre of the rendered pixels.
        /// </remarks>
        /// <param name="imageLocation">A rectangle on the image</param>
        /// <returns>A rectangle on the display or an empty rectangle if there's nothing to display</returns>
        PointF MapImageToDisplay(PointF imageLocation);



        /// <summary>
        /// Returns the display location where a rectangle at <paramref name="imageLocation"/> would be drawn.
        /// </summary>
        /// <remarks>
        /// This is useful when you want to overlay graphics on the image with respect to the 
        /// image regardless of the current zoom and display settings.
        /// 
        /// When the zoom is large each image pixel will occupy several display pixels. The location
        /// of the returned rectangle is at the top-left of this region. The 
        /// location is at the top-left of this region. The <see cref="SizeOfHalfDisplayPixel"/>
        /// property can be used if the drawing should use the centre of the rendered pixels.
        /// </remarks>
        /// <param name="imageLocation">A rectangle on the image</param>
        /// <returns>A rectangle on the display or an empty rectangle if there's nothing to display</returns>
        RectangleF MapImageToDisplay(RectangleF imageLocation);


        /// <summary>
        /// Returns the image location where a pixel at <paramref name="displayLocation"/> would 
        /// have been drawn.
        /// </summary>
        /// <remarks>
        /// This is useful when you have used the mouse to select a region of interest
        /// (rectangle) over the image and want to deteremine the ROI with respect to 
        /// the image.
        /// </remarks>
        /// <param name="displayLocation">A region on the image</param>
        /// <returns>A region on the display or an empty rectangle if there's nothing to display</returns>
        PointF MapDisplayToImage(PointF displayLocation);


        /// <summary>
        /// Returns the image location where a rectangle at <paramref name="displayLocation"/> would 
        /// have been drawn.
        /// </summary>
        /// <remarks>
        /// This is useful when you have used the mouse to select a region of interest
        /// (rectangle) over the image and want to deteremine the ROI with respect to 
        /// the image.
        /// </remarks>
        /// <param name="displayLocation">A region on the image</param>
        /// <returns>A region on the display or an empty rectangle if there's nothing to display</returns>
        RectangleF MapDisplayToImage(RectangleF displayLocation);
        
        
        /// <summary>
        /// Set the image to display. A copy is taken and the reference is not retained.
        /// Passing null delegates to <see cref="ClearImage"/>. The geometry is not changed
        /// if an image of the same size is already displayed. Otherwise the current 
        /// <see cref="Mode"/> is respected but <see cref="TargetImageCentre"/> is
        /// reset.
        /// </summary>
        /// <param name="image"></param>
        void SetImage(Bitmap image);


        /// <summary>
        /// Removes the displayed image
        /// </summary>
        void ClearImage();


        /// <summary>
        /// Centres the image on the display, retaining the existing zoom level.
        /// Only applied if the display mode is <see cref="BitmapDisplayMode.Free"/>,
        /// no-op otherwise.
        /// </summary>
        void Centre();


        /// <summary>
        /// Centres the image on the display and adjusts the zoom so that the 
        /// image fills the display as much as possible.
        /// Only applied if the display mode is <see cref="BitmapDisplayMode.Free"/>,
        /// no-op otherwise.
        /// </summary>
        void FitToWindowCentred();


        /// <summary>
        /// Centres the image on the display and sets the zoom to 1.
        /// Only applied if the display mode is <see cref="BitmapDisplayMode.Free"/>,
        /// no-op otherwise.
        /// </summary>
        void ActualSizeCentred();


        /// <summary>
        /// Reset the zoom to 1:1
        /// </summary>
        void ResetZoom();


        /// <summary>
        /// Zoom in
        /// </summary>
        void ZoomIn();


        /// <summary>
        /// Zoom out
        /// </summary>
        void ZoomOut();


        /// <summary>
        /// Set/get the zoom level. The limits in the <see cref="Consts"/> class are
        /// used.
        /// </summary>
        float Zoom { get; set; } 


        /// <summary>
        /// The location in the image that should be rendered at 
        /// <see cref="TargetDisplayCentre"/>.
        /// </summary>
        PointF TargetImageCentre { get; set; }


        /// <summary>
        /// The location on the display that should render the pixel in the image
        /// at location <see cref="TargetImageCentre"/>.
        /// </summary>
        PointF TargetDisplayCentre { get; set; }


        /// <summary>
        /// The size of half a displayed pixel
        /// </summary>
        /// <remarks>
        /// Use this as an offset when drawing with a large zoom and where the 
        /// drawing locations should be in the middle of an image pixel. E.g. with
        /// a zoom of 11, each image pixel will take 11*11 pixels on the screen. The
        /// half pixel size will be 5.5. Calling <see cref="MapImageToDisplay(PointF)"/>
        /// will return the location of the top-left of this 11*11 block for a particular
        /// image pixel; adding this offset of 5.5 will allow drawing to start in the middle
        /// of this 11*11 block.
        /// </remarks>
        SizeF SizeOfHalfDisplayPixel { get; } 
    }
}
