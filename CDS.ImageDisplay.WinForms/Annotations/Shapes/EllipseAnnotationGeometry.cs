using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text.Json.Serialization;
using CDS.ImageDisplay.WinForms.Annotations.Internal;
using CDS.ImageDisplay.WinForms.BitmapDisplay;
using CDS.ImageDisplay.WinForms.Overlays;
using CDS.ImageDisplay.WinForms.Utils;

namespace CDS.ImageDisplay.WinForms.Annotations.Shapes;

/// <summary>
/// Annotation geometry that represents a rotated ellipse defined by its centre, semi-axes,
/// and rotation angle.
/// </summary>
public sealed class EllipseAnnotationGeometry : AnnotationGeometry
{
    /// <summary>Handle index for the positive-major-axis endpoint.</summary>
    public const int HandleMajorPos = 0;
    /// <summary>Handle index for the negative-major-axis endpoint.</summary>
    public const int HandleMajorNeg = 1;
    /// <summary>Handle index for the positive-minor-axis endpoint.</summary>
    public const int HandleMinorPos = 2;
    /// <summary>Handle index for the negative-minor-axis endpoint.</summary>
    public const int HandleMinorNeg = 3;
    /// <summary>Handle index for the rotation handle (beyond the positive-major-axis tip).</summary>
    public const int HandleRotation = 4;

    private const float RotationHandleOffset = 15f;

    /// <summary>Centre of the ellipse in image pixel coordinates.</summary>
    public PointF Center { get; set; }

    /// <summary>Semi-major axis length in image pixels.</summary>
    public float SemiMajor { get; set; }

    /// <summary>Semi-minor axis length in image pixels.</summary>
    public float SemiMinor { get; set; }

    /// <summary>Clockwise rotation of the major axis in degrees.</summary>
    public float AngleDegrees { get; set; }

    /// <summary>
    /// Initializes a new <see cref="EllipseAnnotationGeometry"/>.
    /// </summary>
    [JsonConstructor]
    public EllipseAnnotationGeometry(PointF center, float semiMajor, float semiMinor, float angleDegrees)
    {
        Center = center;
        SemiMajor = MathF.Max(1f, semiMajor);
        SemiMinor = MathF.Max(1f, semiMinor);
        AngleDegrees = angleDegrees;
    }

    /// <inheritdoc/>
    public override RectangleF GetBoundingBox()
    {
        // AABB of a rotated ellipse with semi-axes a, b and rotation θ.
        float rad = AngleDegrees * MathF.PI / 180f;
        float cosA = MathF.Cos(rad);
        float sinA = MathF.Sin(rad);
        float hw = MathF.Sqrt(SemiMajor * SemiMajor * cosA * cosA + SemiMinor * SemiMinor * sinA * sinA);
        float hh = MathF.Sqrt(SemiMajor * SemiMajor * sinA * sinA + SemiMinor * SemiMinor * cosA * cosA);
        return new RectangleF(Center.X - hw, Center.Y - hh, 2f * hw, 2f * hh);
    }

    /// <inheritdoc/>
    public override void Draw(BitmapDisplayPanel panel, Graphics graphics, bool isSelected)
    {
        ArgumentNullException.ThrowIfNull(panel);
        ArgumentNullException.ThrowIfNull(graphics);

        if (!Drawing.Visible) { return; }

        Pen pen = DrawingToolsPool.GetPen(Drawing.Lines);
        Brush brush = DrawingToolsPool.GetBrush(Drawing.Fill);

        PointF centreDisplay = panel.MapImageToDisplay(Center, DisplayPixelAlign.Centre);
        float semiMajorDisplay = panel.MapImageToDisplay(SemiMajor);
        float semiMinorDisplay = panel.MapImageToDisplay(SemiMinor);

        GraphicsState state = graphics.Save();
        try
        {
            graphics.TranslateTransform(centreDisplay.X, centreDisplay.Y);
            graphics.RotateTransform(AngleDegrees);
            graphics.FillEllipse(brush, -semiMajorDisplay, -semiMinorDisplay, 2f * semiMajorDisplay, 2f * semiMinorDisplay);
            graphics.DrawEllipse(pen, -semiMajorDisplay, -semiMinorDisplay, 2f * semiMajorDisplay, 2f * semiMinorDisplay);
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
        ArgumentNullException.ThrowIfNull(panel);

        PointF[] handles = GetDisplayHandles(panel);

        for (int i = 0; i < handles.Length; i++)
        {
            if (AnnotationHandleHelper.IsNearPoint(displayPoint, handles[i], hitBorder))
            {
                return AnnotationHitInfo.Handle(i);
            }
        }

        // Body hit: transform display point into ellipse local coordinates and test unit ellipse.
        PointF centreDisplay = panel.MapImageToDisplay(Center, DisplayPixelAlign.Centre);
        float semiMajorDisplay = panel.MapImageToDisplay(SemiMajor);
        float semiMinorDisplay = panel.MapImageToDisplay(SemiMinor);

        float dx = displayPoint.X - centreDisplay.X;
        float dy = displayPoint.Y - centreDisplay.Y;
        PointF local = InverseRotate(dx, dy, AngleDegrees);

        float aMod = semiMajorDisplay + hitBorder;
        float bMod = semiMinorDisplay + hitBorder;
        float norm = (local.X / aMod) * (local.X / aMod) + (local.Y / bMod) * (local.Y / bMod);

        return norm <= 1f ? AnnotationHitInfo.Move : AnnotationHitInfo.Miss;
    }

    /// <inheritdoc/>
    public override void ApplyImageDelta(AnnotationHitInfo hit, Size imageDelta)
    {
        ArgumentNullException.ThrowIfNull(hit);

        if (hit.Kind == AnnotationHitKind.MoveBody)
        {
            Center = new PointF(Center.X + imageDelta.Width, Center.Y + imageDelta.Height);
            return;
        }

        if (hit.Kind != AnnotationHitKind.Handle) { return; }

        float rad = AngleDegrees * MathF.PI / 180f;
        float cosA = MathF.Cos(rad);
        float sinA = MathF.Sin(rad);

        switch (hit.HandleIndex)
        {
            case HandleMajorPos:
            {
                float proj = imageDelta.Width * cosA + imageDelta.Height * sinA;
                SemiMajor = MathF.Max(1f, SemiMajor + proj);
                break;
            }
            case HandleMajorNeg:
            {
                float proj = -(imageDelta.Width * cosA + imageDelta.Height * sinA);
                SemiMajor = MathF.Max(1f, SemiMajor + proj);
                break;
            }
            case HandleMinorPos:
            {
                // Minor axis perpendicular: (-sin, cos)
                float proj = imageDelta.Width * (-sinA) + imageDelta.Height * cosA;
                SemiMinor = MathF.Max(1f, SemiMinor + proj);
                break;
            }
            case HandleMinorNeg:
            {
                float proj = -(imageDelta.Width * (-sinA) + imageDelta.Height * cosA);
                SemiMinor = MathF.Max(1f, SemiMinor + proj);
                break;
            }
            case HandleRotation:
                ApplyRotationDelta(imageDelta);
                break;
        }
    }

    /// <inheritdoc/>
    public override AnnotationGeometry Clone()
    {
        var clone = new EllipseAnnotationGeometry(Center, SemiMajor, SemiMinor, AngleDegrees);
        CopyDrawingTo(clone);
        return clone;
    }

    // -----------------------------------------------------------------------
    // Helpers
    // -----------------------------------------------------------------------

    private void ApplyRotationDelta(Size imageDelta)
    {
        // Rotation handle is at Center + (SemiMajor + offset) along the major axis.
        float rad = AngleDegrees * MathF.PI / 180f;
        float cosA = MathF.Cos(rad);
        float sinA = MathF.Sin(rad);
        float handleDist = SemiMajor + RotationHandleOffset;

        float handleX = Center.X + handleDist * cosA;
        float handleY = Center.Y + handleDist * sinA;

        float newX = handleX + imageDelta.Width;
        float newY = handleY + imageDelta.Height;

        AngleDegrees = MathF.Atan2(newY - Center.Y, newX - Center.X) * 180f / MathF.PI;
    }

    private PointF[] GetDisplayHandles(BitmapDisplayPanel panel)
    {
        float rad = AngleDegrees * MathF.PI / 180f;
        float cosA = MathF.Cos(rad);
        float sinA = MathF.Sin(rad);

        // Major axis unit vector: (cos, sin); minor axis: (-sin, cos)
        PointF majorPos = new(Center.X + SemiMajor * cosA, Center.Y + SemiMajor * sinA);
        PointF majorNeg = new(Center.X - SemiMajor * cosA, Center.Y - SemiMajor * sinA);
        PointF minorPos = new(Center.X - SemiMinor * sinA, Center.Y + SemiMinor * cosA);
        PointF minorNeg = new(Center.X + SemiMinor * sinA, Center.Y - SemiMinor * cosA);
        float rotDist = SemiMajor + RotationHandleOffset;
        PointF rotHandle = new(Center.X + rotDist * cosA, Center.Y + rotDist * sinA);

        return
        [
            panel.MapImageToDisplay(majorPos, DisplayPixelAlign.Centre),
            panel.MapImageToDisplay(majorNeg, DisplayPixelAlign.Centre),
            panel.MapImageToDisplay(minorPos, DisplayPixelAlign.Centre),
            panel.MapImageToDisplay(minorNeg, DisplayPixelAlign.Centre),
            panel.MapImageToDisplay(rotHandle, DisplayPixelAlign.Centre),
        ];
    }

    private static PointF InverseRotate(float x, float y, float angleDegrees)
    {
        float rad = angleDegrees * MathF.PI / 180f;
        float cosA = MathF.Cos(rad);
        float sinA = MathF.Sin(rad);
        return new PointF(x * cosA + y * sinA, -x * sinA + y * cosA);
    }
}
