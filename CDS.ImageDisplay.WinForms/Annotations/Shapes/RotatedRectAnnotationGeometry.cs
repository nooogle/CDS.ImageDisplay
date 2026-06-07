using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text.Json.Serialization;
using CDS.ImageDisplay.Annotations.Internal;
using CDS.ImageDisplay.BitmapDisplay;
using CDS.ImageDisplay.Overlays;
using CDS.ImageDisplay.Utils;

namespace CDS.ImageDisplay.Annotations.Shapes;

/// <summary>
/// Annotation geometry that represents a rotated rectangle. Geometry follows the OpenCV
/// <c>RotatedRect</c> convention: <see cref="Center"/> is the rectangle's centre in image
/// coordinates, <see cref="Width"/> and <see cref="Height"/> are the side lengths, and
/// <see cref="AngleDegrees"/> is the clockwise rotation.
/// </summary>
public sealed class RotatedRectAnnotationGeometry : AnnotationGeometry
{
    /// <summary>Handle index for the top-left corner (local frame).</summary>
    public const int HandleTopLeft = 0;
    /// <summary>Handle index for the top-right corner (local frame).</summary>
    public const int HandleTopRight = 1;
    /// <summary>Handle index for the bottom-right corner (local frame).</summary>
    public const int HandleBottomRight = 2;
    /// <summary>Handle index for the bottom-left corner (local frame).</summary>
    public const int HandleBottomLeft = 3;
    /// <summary>Handle index for the rotation handle above the top-centre edge.</summary>
    public const int HandleRotation = 4;

    private const float RotationHandleOffset = 20f;

    /// <summary>Centre of the rectangle in image pixel coordinates.</summary>
    public PointF Center { get; set; }

    /// <summary>Width of the rectangle in image pixels.</summary>
    public float Width { get; set; }

    /// <summary>Height of the rectangle in image pixels.</summary>
    public float Height { get; set; }

    /// <summary>Clockwise rotation angle in degrees.</summary>
    public float AngleDegrees { get; set; }

    /// <summary>
    /// Initializes a new <see cref="RotatedRectAnnotationGeometry"/>.
    /// </summary>
    [JsonConstructor]
    public RotatedRectAnnotationGeometry(PointF center, float width, float height, float angleDegrees)
    {
        Center = center;
        Width = MathF.Max(1f, width);
        Height = MathF.Max(1f, height);
        AngleDegrees = angleDegrees;
    }

    /// <inheritdoc/>
    public override RectangleF GetBoundingBox()
    {
        PointF[] corners = GetImageCorners();
        float minX = corners[0].X, maxX = corners[0].X;
        float minY = corners[0].Y, maxY = corners[0].Y;

        for (int i = 1; i < corners.Length; i++)
        {
            if (corners[i].X < minX) { minX = corners[i].X; }
            if (corners[i].X > maxX) { maxX = corners[i].X; }
            if (corners[i].Y < minY) { minY = corners[i].Y; }
            if (corners[i].Y > maxY) { maxY = corners[i].Y; }
        }

        return RectangleF.FromLTRB(minX, minY, maxX, maxY);
    }

    /// <inheritdoc/>
    public override void Draw(BitmapDisplayPanel panel, Graphics graphics, bool isSelected)
    {
        ArgumentNullException.ThrowIfNull(panel, nameof(panel));
        ArgumentNullException.ThrowIfNull(graphics, nameof(graphics));

        if (!Drawing.Visible) { return; }

        Pen pen = DrawingToolsPool.GetPen(Drawing.Lines);
        Brush brush = DrawingToolsPool.GetBrush(Drawing.Fill);

        PointF centreDisplay = panel.MapImageToDisplay(Center, DisplayPixelAlign.Centre);
        float widthDisplay = panel.MapImageToDisplay(Width);
        float heightDisplay = panel.MapImageToDisplay(Height);

        GraphicsState state = graphics.Save();
        try
        {
            graphics.TranslateTransform(centreDisplay.X, centreDisplay.Y);
            graphics.RotateTransform(AngleDegrees);

            var rect = new RectangleF(-widthDisplay / 2f, -heightDisplay / 2f, widthDisplay, heightDisplay);
            graphics.FillRectangle(brush, rect);
            graphics.DrawRectangle(pen, rect.X, rect.Y, rect.Width, rect.Height);
        }
        finally
        {
            graphics.Restore(state);
        }

        if (isSelected)
        {
            PointF[] handles = GetDisplayHandles(panel);
            foreach (PointF h in handles)
            {
                AnnotationHandleHelper.DrawHandle(graphics, pen, brush, h);
            }
        }
    }

    /// <inheritdoc/>
    public override AnnotationHitInfo HitTest(BitmapDisplayPanel panel, Point displayPoint, int hitBorder)
    {
        ArgumentNullException.ThrowIfNull(panel, nameof(panel));

        PointF[] handles = GetDisplayHandles(panel);

        for (int i = 0; i < handles.Length; i++)
        {
            if (AnnotationHandleHelper.IsNearPoint(displayPoint, handles[i], hitBorder))
            {
                return AnnotationHitInfo.Handle(i);
            }
        }

        // Body hit: transform display point into the rotated rect's local frame.
        PointF centreDisplay = panel.MapImageToDisplay(Center, DisplayPixelAlign.Centre);
        float widthDisplay = panel.MapImageToDisplay(Width);
        float heightDisplay = panel.MapImageToDisplay(Height);

        float dx = displayPoint.X - centreDisplay.X;
        float dy = displayPoint.Y - centreDisplay.Y;
        PointF local = InverseRotate(dx, dy, AngleDegrees);

        if (MathF.Abs(local.X) <= widthDisplay / 2f + hitBorder
            && MathF.Abs(local.Y) <= heightDisplay / 2f + hitBorder)
        {
            return AnnotationHitInfo.Move;
        }

        return AnnotationHitInfo.Miss;
    }

    /// <inheritdoc/>
    public override void ApplyImageDelta(AnnotationHitInfo hit, Size imageDelta)
    {
        ArgumentNullException.ThrowIfNull(hit, nameof(hit));

        if (hit.Kind == AnnotationHitKind.MoveBody)
        {
            Center = new PointF(Center.X + imageDelta.Width, Center.Y + imageDelta.Height);
            return;
        }

        if (hit.Kind != AnnotationHitKind.Handle) { return; }

        // Project the delta into the local frame.
        PointF localDelta = InverseRotate(imageDelta.Width, imageDelta.Height, AngleDegrees);

        switch (hit.HandleIndex)
        {
            case HandleTopLeft:
                Width = MathF.Max(1f, Width - 2f * localDelta.X);
                Height = MathF.Max(1f, Height - 2f * localDelta.Y);
                break;

            case HandleTopRight:
                Width = MathF.Max(1f, Width + 2f * localDelta.X);
                Height = MathF.Max(1f, Height - 2f * localDelta.Y);
                break;

            case HandleBottomRight:
                Width = MathF.Max(1f, Width + 2f * localDelta.X);
                Height = MathF.Max(1f, Height + 2f * localDelta.Y);
                break;

            case HandleBottomLeft:
                Width = MathF.Max(1f, Width - 2f * localDelta.X);
                Height = MathF.Max(1f, Height + 2f * localDelta.Y);
                break;

            case HandleRotation:
                ApplyRotationDelta(imageDelta);
                break;
        }
    }

    /// <inheritdoc/>
    public override AnnotationGeometry Clone()
    {
        var clone = new RotatedRectAnnotationGeometry(Center, Width, Height, AngleDegrees);
        CopyDrawingTo(clone);
        return clone;
    }

    // -----------------------------------------------------------------------
    // Helpers
    // -----------------------------------------------------------------------

    private void ApplyRotationDelta(Size imageDelta)
    {
        // Current rotation handle position (above top-centre in local frame).
        float rad = AngleDegrees * MathF.PI / 180f;
        float cosA = MathF.Cos(rad);
        float sinA = MathF.Sin(rad);
        float handleDist = Height / 2f + RotationHandleOffset;

        // Local (0, -handleDist) rotated to image space.
        float handleX = Center.X + handleDist * sinA;
        float handleY = Center.Y - handleDist * cosA;

        float newX = handleX + imageDelta.Width;
        float newY = handleY + imageDelta.Height;

        float dX = newX - Center.X;
        float dY = newY - Center.Y;

        // Recover angle: handle at (H/2+offset)*sin(θ), -(H/2+offset)*cos(θ) relative to center.
        AngleDegrees = MathF.Atan2(dX, -dY) * 180f / MathF.PI;
    }

    /// <summary>
    /// Returns the four corners in image coordinates, in the order:
    /// top-left, top-right, bottom-right, bottom-left (local frame).
    /// </summary>
    private PointF[] GetImageCorners()
    {
        float hw = Width / 2f;
        float hh = Height / 2f;

        return
        [
            Center + Rotate(-hw, -hh, AngleDegrees),
            Center + Rotate(+hw, -hh, AngleDegrees),
            Center + Rotate(+hw, +hh, AngleDegrees),
            Center + Rotate(-hw, +hh, AngleDegrees),
        ];
    }

    private PointF[] GetDisplayHandles(BitmapDisplayPanel panel)
    {
        PointF[] imageCorners = GetImageCorners();

        // Rotation handle above the top-centre in local frame.
        float rad = AngleDegrees * MathF.PI / 180f;
        float handleDist = Height / 2f + RotationHandleOffset;
        PointF rotHandleImage = new(
            Center.X + handleDist * MathF.Sin(rad),
            Center.Y - handleDist * MathF.Cos(rad));

        return
        [
            panel.MapImageToDisplay(imageCorners[0], DisplayPixelAlign.Centre),
            panel.MapImageToDisplay(imageCorners[1], DisplayPixelAlign.Centre),
            panel.MapImageToDisplay(imageCorners[2], DisplayPixelAlign.Centre),
            panel.MapImageToDisplay(imageCorners[3], DisplayPixelAlign.Centre),
            panel.MapImageToDisplay(rotHandleImage, DisplayPixelAlign.Centre),
        ];
    }

    /// <summary>
    /// Rotates point (x, y) clockwise by <paramref name="angleDegrees"/> (GDI+ screen convention).
    /// </summary>
    private static SizeF Rotate(float x, float y, float angleDegrees)
    {
        float rad = angleDegrees * MathF.PI / 180f;
        float cosA = MathF.Cos(rad);
        float sinA = MathF.Sin(rad);
        return new SizeF(x * cosA - y * sinA, x * sinA + y * cosA);
    }

    /// <summary>
    /// Inverse-rotates (i.e. rotates by -<paramref name="angleDegrees"/>).
    /// </summary>
    private static PointF InverseRotate(float x, float y, float angleDegrees)
    {
        float rad = angleDegrees * MathF.PI / 180f;
        float cosA = MathF.Cos(rad);
        float sinA = MathF.Sin(rad);
        return new PointF(x * cosA + y * sinA, -x * sinA + y * cosA);
    }
}
