using System;
using System.Drawing;
using System.Text.Json.Serialization;
using CDS.ImageDisplay.WinForms.Annotations.Internal;
using CDS.ImageDisplay.WinForms.BitmapDisplay;
using CDS.ImageDisplay.WinForms.Overlays;
using CDS.ImageDisplay.WinForms.Utils;

namespace CDS.ImageDisplay.WinForms.Annotations.Shapes;

/// <summary>
/// Annotation geometry that represents a circle defined by a centre point and radius.
/// </summary>
public sealed class CircleAnnotationGeometry : AnnotationGeometry
{
    /// <summary>Handle index for the top cardinal point.</summary>
    public const int HandleTop = 0;
    /// <summary>Handle index for the right cardinal point.</summary>
    public const int HandleRight = 1;
    /// <summary>Handle index for the bottom cardinal point.</summary>
    public const int HandleBottom = 2;
    /// <summary>Handle index for the left cardinal point.</summary>
    public const int HandleLeft = 3;

    /// <summary>
    /// The centre of the circle in image pixel coordinates.
    /// </summary>
    public Point Centre { get; set; }

    /// <summary>
    /// The radius of the circle in image pixels. Always at least 1.
    /// </summary>
    public int Radius
    {
        get;
        set => field = Math.Max(1, value);
    } = 1;

    /// <summary>
    /// Initializes a new <see cref="CircleAnnotationGeometry"/> with the given centre and radius.
    /// </summary>
    [JsonConstructor]
    public CircleAnnotationGeometry(Point centre, int radius)
    {
        Centre = centre;
        Radius = radius;
    }

    /// <inheritdoc/>
    public override RectangleF GetBoundingBox() =>
        new(Centre.X - Radius, Centre.Y - Radius, Radius * 2f, Radius * 2f);

    /// <inheritdoc/>
    public override void Draw(BitmapDisplayPanel panel, Graphics graphics, bool isSelected)
    {
        ArgumentNullException.ThrowIfNull(panel);
        ArgumentNullException.ThrowIfNull(graphics);

        if (!Drawing.Visible) { return; }

        PointF centreDisplay = panel.MapImageToDisplay(new PointF(Centre.X, Centre.Y), DisplayPixelAlign.Centre);
        float radiusDisplay = panel.MapImageToDisplay((float)Radius);
        var boundingRect = new RectangleF(
            centreDisplay.X - radiusDisplay,
            centreDisplay.Y - radiusDisplay,
            radiusDisplay * 2f,
            radiusDisplay * 2f);

        Pen pen = DrawingToolsPool.GetPen(Drawing.Lines);
        Brush brush = DrawingToolsPool.GetBrush(Drawing.Fill);

        graphics.FillEllipse(brush, boundingRect);
        graphics.DrawEllipse(pen, boundingRect);

        if (isSelected)
        {
            foreach (PointF h in GetHandleDisplayPoints(centreDisplay, radiusDisplay))
            {
                AnnotationHandleHelper.DrawHandle(graphics, pen, brush, h);
            }
        }
    }

    /// <inheritdoc/>
    public override AnnotationHitInfo HitTest(BitmapDisplayPanel panel, Point displayPoint, int hitBorder)
    {
        ArgumentNullException.ThrowIfNull(panel);

        PointF centreDisplay = panel.MapImageToDisplay(new PointF(Centre.X, Centre.Y), DisplayPixelAlign.Centre);
        float radiusDisplay = panel.MapImageToDisplay((float)Radius);
        PointF[] handles = GetHandleDisplayPoints(centreDisplay, radiusDisplay);

        for (int i = 0; i < handles.Length; i++)
        {
            if (AnnotationHandleHelper.IsNearPoint(displayPoint, handles[i], hitBorder))
            {
                return AnnotationHitInfo.Handle(i);
            }
        }

        float dx = displayPoint.X - centreDisplay.X;
        float dy = displayPoint.Y - centreDisplay.Y;
        float distance = MathF.Sqrt((dx * dx) + (dy * dy));

        return distance <= radiusDisplay + hitBorder ? AnnotationHitInfo.Move : AnnotationHitInfo.Miss;
    }

    /// <inheritdoc/>
    public override void ApplyImageDelta(AnnotationHitInfo hit, Size imageDelta)
    {
        ArgumentNullException.ThrowIfNull(hit);

        if (hit.Kind == AnnotationHitKind.MoveBody)
        {
            Centre = new Point(Centre.X + imageDelta.Width, Centre.Y + imageDelta.Height);
            return;
        }

        if (hit.Kind != AnnotationHitKind.Handle) { return; }

        int radiusChange = hit.HandleIndex switch
        {
            HandleTop => -imageDelta.Height,
            HandleRight => imageDelta.Width,
            HandleBottom => imageDelta.Height,
            HandleLeft => -imageDelta.Width,
            _ => 0,
        };

        Radius += radiusChange;
    }

    /// <inheritdoc/>
    public override AnnotationGeometry Clone()
    {
        var clone = new CircleAnnotationGeometry(Centre, Radius);
        CopyDrawingTo(clone);
        return clone;
    }

    private static PointF[] GetHandleDisplayPoints(PointF centreDisplay, float radiusDisplay) =>
    [
        new(centreDisplay.X, centreDisplay.Y - radiusDisplay),
        new(centreDisplay.X + radiusDisplay, centreDisplay.Y),
        new(centreDisplay.X, centreDisplay.Y + radiusDisplay),
        new(centreDisplay.X - radiusDisplay, centreDisplay.Y),
    ];
}
