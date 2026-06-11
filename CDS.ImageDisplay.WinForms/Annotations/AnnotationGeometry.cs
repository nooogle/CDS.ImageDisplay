using System;
using System.Drawing;
using System.Text.Json.Serialization;
using CDS.ImageDisplay.WinForms.Annotations.Internal;
using CDS.ImageDisplay.WinForms.Annotations.Shapes;
using CDS.ImageDisplay.WinForms.BitmapDisplay;
using CDS.ImageDisplay.WinForms.Overlays;

namespace CDS.ImageDisplay.WinForms.Annotations;

/// <summary>
/// Base class for the geometry of an annotation. Owns the shape coordinates and visual style,
/// and is responsible for drawing, hit-testing, and applying drag operations.
/// </summary>
[JsonPolymorphic(TypeDiscriminatorPropertyName = "$type")]
[JsonDerivedType(typeof(RectAnnotationGeometry), "rect")]
[JsonDerivedType(typeof(CircleAnnotationGeometry), "circle")]
[JsonDerivedType(typeof(EllipseAnnotationGeometry), "ellipse")]
[JsonDerivedType(typeof(LineAnnotationGeometry), "line")]
[JsonDerivedType(typeof(PolygonAnnotationGeometry), "polygon")]
[JsonDerivedType(typeof(CrosshairAnnotationGeometry), "crosshair")]
[JsonDerivedType(typeof(RotatedRectAnnotationGeometry), "rotated-rect")]
public abstract class AnnotationGeometry
{
    /// <summary>
    /// Controls how the annotation is drawn.
    /// </summary>
    public DrawingSpec Drawing { get; init; } = new();

    /// <summary>
    /// Returns the axis-aligned bounding box of this shape in image coordinates.
    /// Returns <see cref="RectangleF.Empty"/> for point shapes such as crosshairs that have no spatial extent.
    /// </summary>
    public abstract RectangleF GetBoundingBox();

    /// <summary>
    /// Draws the annotation on the panel.
    /// </summary>
    /// <param name="panel">The panel being painted.</param>
    /// <param name="graphics">The graphics context.</param>
    /// <param name="isSelected">When <see langword="true"/>, also draws resize and move handles.</param>
    public abstract void Draw(BitmapDisplayPanel panel, Graphics graphics, bool isSelected);

    /// <summary>
    /// Tests whether the given display-coordinate point is over this annotation.
    /// </summary>
    /// <param name="panel">The panel, used for coordinate conversion.</param>
    /// <param name="displayPoint">Mouse location in display (control client) coordinates.</param>
    /// <param name="hitBorder">Tolerance in display pixels around the shape boundary.</param>
    public abstract AnnotationHitInfo HitTest(BitmapDisplayPanel panel, Point displayPoint, int hitBorder);

    /// <summary>
    /// Applies a drag delta to the geometry, moving the whole shape or adjusting a specific handle.
    /// Converts the display-pixel delta to image-pixel coordinates then delegates to <see cref="ApplyImageDelta"/>.
    /// </summary>
    /// <param name="panel">The panel, used for coordinate conversion.</param>
    /// <param name="hit">The hit result that initiated the drag.</param>
    /// <param name="dragDeltaDisplay">Movement since the drag started, in display pixels.</param>
    public void ApplyDrag(BitmapDisplayPanel panel, AnnotationHitInfo hit, Point dragDeltaDisplay)
    {
        Guard.ThrowIfNull(panel, nameof(panel));
        Guard.ThrowIfNull(hit, nameof(hit));
        ApplyImageDelta(hit, AnnotationHandleHelper.DisplayDeltaToImageDelta(panel, dragDeltaDisplay));
    }

    /// <summary>
    /// Applies a delta expressed in image-pixel coordinates to this geometry.
    /// Override this in subclasses to implement move and resize logic.
    /// </summary>
    /// <param name="hit">The hit result that initiated the drag, identifying the handle or body.</param>
    /// <param name="imageDelta">Movement in image pixels.</param>
    public abstract void ApplyImageDelta(AnnotationHitInfo hit, Size imageDelta);

    /// <summary>
    /// Returns a deep copy of this geometry, including a copy of the <see cref="Drawing"/> spec.
    /// </summary>
    public abstract AnnotationGeometry Clone();

    /// <summary>
    /// Copies the <see cref="Drawing"/> spec from this instance into the given target geometry.
    /// Call this from <see cref="Clone"/> implementations after constructing the new instance.
    /// </summary>
    /// <param name="target">The newly constructed clone to copy the drawing spec into.</param>
    protected void CopyDrawingTo(AnnotationGeometry target)
    {
        Guard.ThrowIfNull(target, nameof(target));

        target.Drawing.Visible = Drawing.Visible;
        target.Drawing.MappingMode = Drawing.MappingMode;
        target.Drawing.Lines = Drawing.Lines.Clone();
        target.Drawing.Fill = Drawing.Fill.Clone();
        target.Drawing.Font = Drawing.Font.Clone();
    }
}
