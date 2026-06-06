using System;
using System.Drawing;
using CDS.ImageDisplay.Annotations.Internal;

namespace CDS.ImageDisplay.Annotations.Shapes;

/// <summary>
/// Descriptor for ellipse annotations.
/// </summary>
public sealed class EllipseAnnotationDescriptor : IAnnotationShapeDescriptor
{
    /// <inheritdoc/>
    public string Name => "Ellipse";

    /// <inheritdoc/>
    public string Description => "Axis-aligned ellipse fitted to the bounding box of the drawn gesture";

    /// <inheritdoc/>
    public float FitScore(FreehandPath path)
    {
        ArgumentNullException.ThrowIfNull(path, nameof(path));

        RectangleF bbox = path.BoundingBox;
        float w = bbox.Width;
        float h = bbox.Height;

        if (w < 1f || h < 1f) { return 0f; }

        // Aspect ratio must be sufficiently non-circular; the circle descriptor handles the square case.
        float aspect = MathF.Max(w, h) / MathF.Min(w, h);
        float aspectFactor = FreehandPathAnalyser.Clamp01((aspect - 1.3f) / 0.3f);
        if (aspectFactor <= 0f) { return 0f; }

        // Points should lie close to the ellipse boundary defined by the bounding box.
        float cx = bbox.Left + w / 2f;
        float cy = bbox.Top + h / 2f;
        float meanError = FreehandPathAnalyser.MeanEllipseBoundaryError(path.Points, cx, cy, w / 2f, h / 2f);
        float boundaryScore = FreehandPathAnalyser.Clamp01(1f - meanError / 0.3f);

        return aspectFactor * boundaryScore;
    }

    /// <inheritdoc/>
    public AnnotationGeometry CreateGeometry(FreehandPath path)
    {
        ArgumentNullException.ThrowIfNull(path, nameof(path));

        RectangleF bbox = path.BoundingBox;
        var bounds = Rectangle.FromLTRB(
            (int)MathF.Floor(bbox.Left),
            (int)MathF.Floor(bbox.Top),
            (int)MathF.Ceiling(bbox.Right),
            (int)MathF.Ceiling(bbox.Bottom));

        if (bounds.Width < 1) { bounds = new Rectangle(bounds.X, bounds.Y, 1, bounds.Height); }
        if (bounds.Height < 1) { bounds = new Rectangle(bounds.X, bounds.Y, bounds.Width, 1); }

        var geometry = new EllipseAnnotationGeometry(bounds);
        geometry.Drawing.Lines.Color = Color.CornflowerBlue;
        geometry.Drawing.Lines.Width = 2f;
        geometry.Drawing.Fill.Color = Color.Transparent;
        return geometry;
    }
}
