using System;
using System.Drawing;

namespace CDS.ImageDisplay.Annotations.Shapes;

/// <summary>
/// Descriptor for crosshair annotations. Used for the click (point) creation path;
/// crosshair does not participate in gesture auto-recognition.
/// </summary>
public sealed class CrosshairAnnotationDescriptor : IAnnotationShapeDescriptor
{
    /// <inheritdoc/>
    public string Name => "Crosshair";

    /// <inheritdoc/>
    public string Description => "Point marker at the click location";

    /// <summary>
    /// Always returns 0 — crosshair is created via the click path, not gesture recognition.
    /// </summary>
    public float FitScore(FreehandPath path) => 0f;

    /// <inheritdoc/>
    public AnnotationGeometry CreateGeometry(FreehandPath path)
    {
        ArgumentNullException.ThrowIfNull(path, nameof(path));

        Point centre = path.Points.Count > 0
            ? Point.Round(path.Centroid)
            : Point.Empty;

        var geometry = new CrosshairAnnotationGeometry(centre);
        geometry.Drawing.Lines.Color = Color.Tomato;
        geometry.Drawing.Lines.Width = 2f;
        geometry.Drawing.Fill.Color = Color.Transparent;
        return geometry;
    }
}
