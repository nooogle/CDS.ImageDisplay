using System;
using System.Drawing;
using System.Text.Json.Serialization;
using CDS.ImageDisplay.Annotations.Internal;
using CDS.ImageDisplay.BitmapDisplay;
using CDS.ImageDisplay.Overlays;
using CDS.ImageDisplay.Utils;

namespace CDS.ImageDisplay.Annotations.Shapes;

/// <summary>
/// Annotation geometry that represents a crosshair at a single image point.
/// The arm length and centre gap are expressed in display pixels so the crosshair
/// remains constant-size regardless of zoom level.
/// </summary>
public sealed class CrosshairAnnotationGeometry : AnnotationGeometry
{
    /// <summary>
    /// The centre of the crosshair in image pixel coordinates.
    /// </summary>
    public Point Centre { get; set; }

    /// <summary>
    /// Length of each arm in display pixels.
    /// </summary>
    public float Length { get; set; } = 12f;

    /// <summary>
    /// Gap between the centre point and the start of each arm in display pixels.
    /// </summary>
    public float CentreGap { get; set; } = 3f;

    /// <summary>
    /// Initializes a new <see cref="CrosshairAnnotationGeometry"/> at the given image point.
    /// </summary>
    [JsonConstructor]
    public CrosshairAnnotationGeometry(Point centre)
    {
        Centre = centre;
    }

    /// <inheritdoc/>
    /// <remarks>Returns <see cref="RectangleF.Empty"/> — crosshairs are point shapes with no spatial extent.</remarks>
    public override RectangleF GetBoundingBox() => RectangleF.Empty;

    /// <inheritdoc/>
    public override void Draw(BitmapDisplayPanel panel, Graphics graphics, bool isSelected)
    {
        ArgumentNullException.ThrowIfNull(panel, nameof(panel));
        ArgumentNullException.ThrowIfNull(graphics, nameof(graphics));

        if (!Drawing.Visible) { return; }

        PointF c = panel.MapImageToDisplay(new PointF(Centre.X, Centre.Y), DisplayPixelAlign.Centre);
        Pen pen = DrawingToolsPool.GetPen(Drawing.Lines);
        Brush brush = DrawingToolsPool.GetBrush(Drawing.Fill);

        graphics.DrawLine(pen, c.X, c.Y - Length - CentreGap, c.X, c.Y - CentreGap);
        graphics.DrawLine(pen, c.X, c.Y + CentreGap, c.X, c.Y + Length + CentreGap);
        graphics.DrawLine(pen, c.X - Length - CentreGap, c.Y, c.X - CentreGap, c.Y);
        graphics.DrawLine(pen, c.X + CentreGap, c.Y, c.X + Length + CentreGap, c.Y);

        if (isSelected)
        {
            AnnotationHandleHelper.DrawHandle(graphics, pen, brush, c);
        }
    }

    /// <inheritdoc/>
    public override AnnotationHitInfo HitTest(BitmapDisplayPanel panel, Point displayPoint, int hitBorder)
    {
        ArgumentNullException.ThrowIfNull(panel, nameof(panel));

        PointF centreDisplay = panel.MapImageToDisplay(new PointF(Centre.X, Centre.Y), DisplayPixelAlign.Centre);
        return AnnotationHandleHelper.IsNearPoint(displayPoint, centreDisplay, hitBorder)
            ? AnnotationHitInfo.Move
            : AnnotationHitInfo.Miss;
    }

    /// <inheritdoc/>
    public override void ApplyImageDelta(AnnotationHitInfo hit, Size imageDelta)
    {
        ArgumentNullException.ThrowIfNull(hit, nameof(hit));

        if (hit.Kind == AnnotationHitKind.MoveBody)
        {
            Centre = new Point(Centre.X + imageDelta.Width, Centre.Y + imageDelta.Height);
        }
    }

    /// <inheritdoc/>
    public override AnnotationGeometry Clone()
    {
        var clone = new CrosshairAnnotationGeometry(Centre) { Length = Length, CentreGap = CentreGap };
        CopyDrawingTo(clone);
        return clone;
    }
}
