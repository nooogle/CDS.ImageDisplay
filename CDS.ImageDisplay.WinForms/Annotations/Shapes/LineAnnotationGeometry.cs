using System;
using System.Drawing;
using System.Text.Json.Serialization;
using CDS.ImageDisplay.Annotations.Internal;
using CDS.ImageDisplay.BitmapDisplay;
using CDS.ImageDisplay.Overlays;
using CDS.ImageDisplay.Utils;

namespace CDS.ImageDisplay.Annotations.Shapes;

/// <summary>
/// Annotation geometry that represents a straight line segment.
/// </summary>
public sealed class LineAnnotationGeometry : AnnotationGeometry
{
    /// <summary>Handle index for the start endpoint.</summary>
    public const int HandleStart = 0;
    /// <summary>Handle index for the end endpoint.</summary>
    public const int HandleEnd = 1;

    /// <summary>
    /// The start point of the line in image pixel coordinates.
    /// </summary>
    public Point Start { get; set; }

    /// <summary>
    /// The end point of the line in image pixel coordinates.
    /// </summary>
    public Point End { get; set; }

    /// <summary>
    /// Initializes a new <see cref="LineAnnotationGeometry"/> with the given endpoints.
    /// </summary>
    [JsonConstructor]
    public LineAnnotationGeometry(Point start, Point end)
    {
        Start = start;
        End = end;
    }

    /// <inheritdoc/>
    public override RectangleF GetBoundingBox()
    {
        float left = Math.Min(Start.X, End.X);
        float top = Math.Min(Start.Y, End.Y);
        float right = Math.Max(Start.X, End.X);
        float bottom = Math.Max(Start.Y, End.Y);
        return RectangleF.FromLTRB(left, top, right, bottom);
    }

    /// <inheritdoc/>
    public override void Draw(BitmapDisplayPanel panel, Graphics graphics, bool isSelected)
    {
        Guard.ThrowIfNull(panel, nameof(panel));
        Guard.ThrowIfNull(graphics, nameof(graphics));

        if (!Drawing.Visible) { return; }

        PointF startDisplay = panel.MapImageToDisplay(new PointF(Start.X, Start.Y), DisplayPixelAlign.Centre);
        PointF endDisplay = panel.MapImageToDisplay(new PointF(End.X, End.Y), DisplayPixelAlign.Centre);
        Pen pen = DrawingToolsPool.GetPen(Drawing.Lines);
        Brush brush = DrawingToolsPool.GetBrush(Drawing.Fill);

        graphics.DrawLine(pen, startDisplay, endDisplay);

        if (isSelected)
        {
            AnnotationHandleHelper.DrawHandle(graphics, pen, brush, startDisplay);
            AnnotationHandleHelper.DrawHandle(graphics, pen, brush, endDisplay);
        }
    }

    /// <inheritdoc/>
    public override AnnotationHitInfo HitTest(BitmapDisplayPanel panel, Point displayPoint, int hitBorder)
    {
        Guard.ThrowIfNull(panel, nameof(panel));

        PointF startDisplay = panel.MapImageToDisplay(new PointF(Start.X, Start.Y), DisplayPixelAlign.Centre);
        PointF endDisplay = panel.MapImageToDisplay(new PointF(End.X, End.Y), DisplayPixelAlign.Centre);

        if (AnnotationHandleHelper.IsNearPoint(displayPoint, startDisplay, hitBorder))
        {
            return AnnotationHitInfo.Handle(HandleStart);
        }

        if (AnnotationHandleHelper.IsNearPoint(displayPoint, endDisplay, hitBorder))
        {
            return AnnotationHitInfo.Handle(HandleEnd);
        }

        float distance = AnnotationHandleHelper.DistanceFromPointToSegment(
            new PointF(displayPoint.X, displayPoint.Y), startDisplay, endDisplay);

        return distance <= hitBorder ? AnnotationHitInfo.Move : AnnotationHitInfo.Miss;
    }

    /// <inheritdoc/>
    public override void ApplyImageDelta(AnnotationHitInfo hit, Size imageDelta)
    {
        Guard.ThrowIfNull(hit, nameof(hit));

        switch (hit.Kind)
        {
            case AnnotationHitKind.MoveBody:
                Start = new Point(Start.X + imageDelta.Width, Start.Y + imageDelta.Height);
                End = new Point(End.X + imageDelta.Width, End.Y + imageDelta.Height);
                break;
            case AnnotationHitKind.Handle when hit.HandleIndex == HandleStart:
                Start = new Point(Start.X + imageDelta.Width, Start.Y + imageDelta.Height);
                break;
            case AnnotationHitKind.Handle when hit.HandleIndex == HandleEnd:
                End = new Point(End.X + imageDelta.Width, End.Y + imageDelta.Height);
                break;
        }
    }

    /// <inheritdoc/>
    public override AnnotationGeometry Clone()
    {
        var clone = new LineAnnotationGeometry(Start, End);
        CopyDrawingTo(clone);
        return clone;
    }
}
