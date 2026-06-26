using System;
using System.Drawing;
using System.Text.Json.Serialization;
using CDS.ImageDisplay.WinForms.Annotations.Internal;
using CDS.ImageDisplay.WinForms.BitmapDisplay;
using CDS.ImageDisplay.WinForms.Overlays;
using CDS.ImageDisplay.WinForms.Utils;

namespace CDS.ImageDisplay.WinForms.Annotations.Shapes;

/// <summary>
/// Annotation geometry that represents an axis-aligned rectangle.
/// </summary>
public sealed class RectAnnotationGeometry : AnnotationGeometry
{
    // Handle indices — shared with EllipseAnnotationGeometry
    /// <summary>Handle index for the top-left corner.</summary>
    public const int HandleTopLeft = 0;
    /// <summary>Handle index for the top-right corner.</summary>
    public const int HandleTopRight = 1;
    /// <summary>Handle index for the bottom-right corner.</summary>
    public const int HandleBottomRight = 2;
    /// <summary>Handle index for the bottom-left corner.</summary>
    public const int HandleBottomLeft = 3;
    /// <summary>Handle index for the top edge midpoint.</summary>
    public const int HandleTopMid = 4;
    /// <summary>Handle index for the right edge midpoint.</summary>
    public const int HandleRightMid = 5;
    /// <summary>Handle index for the bottom edge midpoint.</summary>
    public const int HandleBottomMid = 6;
    /// <summary>Handle index for the left edge midpoint.</summary>
    public const int HandleLeftMid = 7;

    /// <summary>
    /// The annotation rectangle in image pixel coordinates.
    /// </summary>
    public Rectangle Bounds { get; set; }

    /// <summary>
    /// Initializes a new <see cref="RectAnnotationGeometry"/> with the given bounds.
    /// </summary>
    [JsonConstructor]
    public RectAnnotationGeometry(Rectangle bounds)
    {
        Bounds = bounds;
    }

    /// <inheritdoc/>
    public override RectangleF GetBoundingBox() =>
        new(Bounds.X, Bounds.Y, Bounds.Width, Bounds.Height);

    /// <inheritdoc/>
    public override void Draw(ICoordinateMapper mapper, Graphics graphics, bool isSelected)
    {
        if (mapper == null) { throw new ArgumentNullException(nameof(mapper)); }
        if (graphics == null) { throw new ArgumentNullException(nameof(graphics)); }

        if (!Drawing.Visible || Bounds.IsEmpty) { return; }

        RectangleF displayRect = mapper.MapRect((RectangleF)Bounds, DisplayPixelAlign.TopLeft);
        Pen pen = DrawingToolsPool.GetPen(Drawing.Lines);
        Brush brush = DrawingToolsPool.GetBrush(Drawing.Fill);

        graphics.FillRectangle(brush, displayRect);
        graphics.DrawRectangle(pen, displayRect.X, displayRect.Y, displayRect.Width, displayRect.Height);

        if (isSelected)
        {
            foreach (PointF h in GetHandleDisplayPoints(displayRect))
            {
                AnnotationHandleHelper.DrawHandle(graphics, pen, brush, h);
            }
        }
    }

    /// <inheritdoc/>
    public override AnnotationHitInfo HitTest(BitmapDisplayPanel panel, Point displayPoint, int hitBorder)
    {
        if (panel == null) { throw new ArgumentNullException(nameof(panel)); }

        Rectangle displayRect = panel.MapImageToDisplay((RectangleF)Bounds, DisplayPixelAlign.TopLeft);
        PointF[] handles = GetHandleDisplayPoints(displayRect);

        for (int i = 0; i < handles.Length; i++)
        {
            if (AnnotationHandleHelper.IsNearPoint(displayPoint, handles[i], hitBorder))
            {
                return AnnotationHitInfo.Handle(i);
            }
        }

        Rectangle inflated = displayRect;
        inflated.Inflate(hitBorder, hitBorder);

        return inflated.Contains(displayPoint) ? AnnotationHitInfo.Move : AnnotationHitInfo.Miss;
    }

    /// <inheritdoc/>
    public override void ApplyImageDelta(AnnotationHitInfo hit, Size imageDelta)
    {
        if (hit == null) { throw new ArgumentNullException(nameof(hit)); }

        if (hit.Kind == AnnotationHitKind.MoveBody)
        {
            Bounds = new Rectangle(
                Bounds.X + imageDelta.Width,
                Bounds.Y + imageDelta.Height,
                Bounds.Width,
                Bounds.Height);
            return;
        }

        if (hit.Kind != AnnotationHitKind.Handle) { return; }

        int left = Bounds.Left;
        int top = Bounds.Top;
        int right = Bounds.Right;
        int bottom = Bounds.Bottom;

        switch (hit.HandleIndex)
        {
            case HandleTopLeft:
                left += imageDelta.Width;
                top += imageDelta.Height;
                break;
            case HandleTopRight:
                right += imageDelta.Width;
                top += imageDelta.Height;
                break;
            case HandleBottomRight:
                right += imageDelta.Width;
                bottom += imageDelta.Height;
                break;
            case HandleBottomLeft:
                left += imageDelta.Width;
                bottom += imageDelta.Height;
                break;
            case HandleTopMid:
                top += imageDelta.Height;
                break;
            case HandleRightMid:
                right += imageDelta.Width;
                break;
            case HandleBottomMid:
                bottom += imageDelta.Height;
                break;
            case HandleLeftMid:
                left += imageDelta.Width;
                break;
        }

        // Ensure minimum 1x1 size.
        if (right <= left) { right = left + 1; }
        if (bottom <= top) { bottom = top + 1; }

        Bounds = Rectangle.FromLTRB(left, top, right, bottom);
    }

    /// <inheritdoc/>
    public override AnnotationGeometry Clone()
    {
        var clone = new RectAnnotationGeometry(Bounds);
        CopyDrawingTo(clone);
        return clone;
    }

    private static PointF[] GetHandleDisplayPoints(RectangleF displayRect) =>
    [
        new(displayRect.Left, displayRect.Top),
        new(displayRect.Right, displayRect.Top),
        new(displayRect.Right, displayRect.Bottom),
        new(displayRect.Left, displayRect.Bottom),
        new(displayRect.Left + displayRect.Width / 2f, displayRect.Top),
        new(displayRect.Right, displayRect.Top + displayRect.Height / 2f),
        new(displayRect.Left + displayRect.Width / 2f, displayRect.Bottom),
        new(displayRect.Left, displayRect.Top + displayRect.Height / 2f),
    ];
}
