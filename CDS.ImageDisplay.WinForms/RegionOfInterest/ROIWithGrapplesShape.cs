using System.ComponentModel;
using System.Drawing;
using CDS.ImageDisplay.BitmapDisplay;
using CDS.ImageDisplay.Overlays;
using CDS.ImageDisplay.Utils;


namespace CDS.ImageDisplay.RegionOfInterest;

/// <summary>
/// Paints a rectangle on a graphics object. Supports grapples.
/// </summary>
[TypeConverter(typeof(SerializableExpandableObjectConverter))]
public class ROIWithGrapplesShape : ISingleROIDescriptor
{
    /// <summary>
    /// Last known region of interest (in display coordinates).
    /// </summary>
    private Rectangle lastROIOnDisplay = Rectangle.Empty;

    /// <summary>
    /// GrappleDiameter value used to compute the current cached grapple rectangles.
    /// </summary>
    private int lastGrappleDiameter;


    /// <summary>
    /// Cached grapple rectangles.
    /// </summary>
    private readonly Rectangle[] grappleRectangles = new Rectangle[8];


    /// <inheritdoc/>
    public bool Visible { get; set; } = true;


    /// <summary>
    /// True if the grapples are visible.
    /// </summary>
    public bool GrapplesVisible { get; set; } = true;


    /// <summary>
    /// True if the rectangle is locked and cannot be moved.
    /// </summary>
    public bool Locked { get; set; }


    /// <inheritdoc/>
    public string Name { get; set; } = "";


    /// <inheritdoc/>
    public Rectangle ROI { get; set; }


    /// <inheritdoc/>
    public Size MinimumSize { get; set; } = new Size(1, 1);


    /// <inheritdoc/>
    public Size MaximumSize { get; set; } = new Size(1000000, 1000000);


    /// <summary>
    /// The diameter of the grapple points.
    /// </summary>
    public int GrappleDiameter { get; set; } = 6;


    /// <inheritdoc/>
    public DisplayPixelAlign PixelAlign { get; set; } = DisplayPixelAlign.TopLeft;


    /// <inheritdoc/>
    public DrawingSpec Drawing { get; set; } = new DrawingSpec();


    /// <summary>
    /// Default constructor.
    /// </summary>
    public ROIWithGrapplesShape()
    {
    }


    /// <inheritdoc/>
    public void Draw(BitmapDisplayPanel bitmapDisplayPanel, Graphics graphics)
    {
        if (!Visible || ROI.IsEmpty || bitmapDisplayPanel == null || graphics == null)
        {
            return;
        }

        Rectangle roiOnDisplay = bitmapDisplayPanel.MapImageToDisplay(ROI, PixelAlign);
        RecalculateGrapplesRectangles(roiOnDisplay);

        Pen pen = DrawingToolsPool.GetPen(Drawing.Lines);
        Brush brush = DrawingToolsPool.GetBrush(Drawing.Fill);

        graphics.FillRectangle(brush, roiOnDisplay);
        graphics.DrawRectangle(pen, roiOnDisplay);

        DrawGrapples(graphics, pen, brush);
    }


    private void DrawGrapples(Graphics graphics, Pen pen, Brush brush)
    {
        if (!GrapplesVisible)
        { return; }

        for (int grappleIndex = 0; grappleIndex < grappleRectangles.Length; grappleIndex++)
        {
            Rectangle grappleRect = grappleRectangles[grappleIndex];
            graphics.FillEllipse(brush, grappleRect);
            graphics.DrawEllipse(pen, grappleRect);
        }
    }


    /// <summary>
    /// Recalculates the rectangles for the grapple points.
    /// </summary>
    private void RecalculateGrapplesRectangles(Rectangle roiOnDisplay)
    {
        if (lastROIOnDisplay == roiOnDisplay && lastGrappleDiameter == GrappleDiameter)
        { return; }

        lastROIOnDisplay = roiOnDisplay;
        lastGrappleDiameter = GrappleDiameter;

        grappleRectangles[0] = CreateGrappleRect(location: roiOnDisplay.Location);
        grappleRectangles[1] = CreateGrappleRect(location: new Point(roiOnDisplay.Right, roiOnDisplay.Top));
        grappleRectangles[2] = CreateGrappleRect(location: new Point(roiOnDisplay.Right, roiOnDisplay.Bottom));
        grappleRectangles[3] = CreateGrappleRect(location: new Point(roiOnDisplay.Left, roiOnDisplay.Bottom));
        grappleRectangles[4] = CreateGrappleRect(location: new Point(roiOnDisplay.Left + (roiOnDisplay.Width / 2), roiOnDisplay.Top));
        grappleRectangles[5] = CreateGrappleRect(location: new Point(roiOnDisplay.Right, roiOnDisplay.Top + (roiOnDisplay.Height / 2)));
        grappleRectangles[6] = CreateGrappleRect(location: new Point(roiOnDisplay.Left + (roiOnDisplay.Width / 2), roiOnDisplay.Bottom));
        grappleRectangles[7] = CreateGrappleRect(location: new Point(roiOnDisplay.Left, roiOnDisplay.Top + (roiOnDisplay.Height / 2)));
    }


    /// <summary>
    /// Creates a rectangle for a grapple point.
    /// </summary>
    private Rectangle CreateGrappleRect(Point location)
    {
        return new Rectangle(
            x: location.X - (GrappleDiameter / 2),
            y: location.Y - (GrappleDiameter / 2),
            width: GrappleDiameter,
            height: GrappleDiameter);
    }


    /// <summary>
    /// String representation of the object.
    /// </summary>
    public override string ToString() => $"{Name}: {ROI}";
}
