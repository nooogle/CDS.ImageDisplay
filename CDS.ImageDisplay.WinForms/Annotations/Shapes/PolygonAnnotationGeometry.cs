using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text.Json.Serialization;
using CDS.ImageDisplay.Annotations.Internal;
using CDS.ImageDisplay.BitmapDisplay;
using CDS.ImageDisplay.Overlays;
using CDS.ImageDisplay.Utils;

namespace CDS.ImageDisplay.Annotations.Shapes;

/// <summary>
/// Annotation geometry that represents an arbitrary polygon defined by a list of vertices.
/// </summary>
public sealed class PolygonAnnotationGeometry : AnnotationGeometry
{
    private List<Point> _vertices = [];

    /// <summary>
    /// The vertices of the polygon in image pixel coordinates.
    /// </summary>
    public IReadOnlyList<Point> Vertices
    {
        get => _vertices;
        init => _vertices = new List<Point>(value ?? []);
    }

    /// <summary>
    /// Parameterless constructor used by the JSON deserializer.
    /// </summary>
    [JsonConstructor]
    private PolygonAnnotationGeometry() { }

    /// <summary>
    /// Initializes a new <see cref="PolygonAnnotationGeometry"/> with the given vertices.
    /// </summary>
    public PolygonAnnotationGeometry(IEnumerable<Point> vertices)
    {
        Guard.ThrowIfNull(vertices, nameof(vertices));
        _vertices = new List<Point>(vertices);
    }

    /// <inheritdoc/>
    public override RectangleF GetBoundingBox()
    {
        if (_vertices.Count == 0) { return RectangleF.Empty; }

        int minX = _vertices[0].X, maxX = _vertices[0].X;
        int minY = _vertices[0].Y, maxY = _vertices[0].Y;

        for (int i = 1; i < _vertices.Count; i++)
        {
            Point v = _vertices[i];
            if (v.X < minX) { minX = v.X; }
            if (v.X > maxX) { maxX = v.X; }
            if (v.Y < minY) { minY = v.Y; }
            if (v.Y > maxY) { maxY = v.Y; }
        }

        return RectangleF.FromLTRB(minX, minY, maxX, maxY);
    }

    /// <inheritdoc/>
    public override void Draw(BitmapDisplayPanel panel, Graphics graphics, bool isSelected)
    {
        Guard.ThrowIfNull(panel, nameof(panel));
        Guard.ThrowIfNull(graphics, nameof(graphics));

        if (!Drawing.Visible || _vertices.Count < 3) { return; }

        PointF[] displayPoints = GetDisplayPoints(panel);
        Pen pen = DrawingToolsPool.GetPen(Drawing.Lines);
        Brush brush = DrawingToolsPool.GetBrush(Drawing.Fill);

        graphics.FillPolygon(brush, displayPoints);
        graphics.DrawPolygon(pen, displayPoints);

        if (isSelected)
        {
            foreach (PointF h in displayPoints)
            {
                AnnotationHandleHelper.DrawHandle(graphics, pen, brush, h);
            }
        }
    }

    /// <inheritdoc/>
    public override AnnotationHitInfo HitTest(BitmapDisplayPanel panel, Point displayPoint, int hitBorder)
    {
        Guard.ThrowIfNull(panel, nameof(panel));

        if (_vertices.Count == 0) { return AnnotationHitInfo.Miss; }

        PointF[] displayPoints = GetDisplayPoints(panel);

        for (int i = 0; i < displayPoints.Length; i++)
        {
            if (AnnotationHandleHelper.IsNearPoint(displayPoint, displayPoints[i], hitBorder))
            {
                return AnnotationHitInfo.Handle(i);
            }
        }

        if (_vertices.Count >= 3 &&
            AnnotationHandleHelper.IsPointInPolygon(displayPoints, new PointF(displayPoint.X, displayPoint.Y)))
        {
            return AnnotationHitInfo.Move;
        }

        return AnnotationHitInfo.Miss;
    }

    /// <inheritdoc/>
    public override void ApplyImageDelta(AnnotationHitInfo hit, Size imageDelta)
    {
        Guard.ThrowIfNull(hit, nameof(hit));

        if (hit.Kind == AnnotationHitKind.MoveBody)
        {
            for (int i = 0; i < _vertices.Count; i++)
            {
                _vertices[i] = new Point(_vertices[i].X + imageDelta.Width, _vertices[i].Y + imageDelta.Height);
            }
            return;
        }

        if (hit.Kind == AnnotationHitKind.Handle && hit.HandleIndex >= 0 && hit.HandleIndex < _vertices.Count)
        {
            _vertices[hit.HandleIndex] = new Point(
                _vertices[hit.HandleIndex].X + imageDelta.Width,
                _vertices[hit.HandleIndex].Y + imageDelta.Height);
        }
    }

    /// <inheritdoc/>
    public override AnnotationGeometry Clone()
    {
        var clone = new PolygonAnnotationGeometry(_vertices);
        CopyDrawingTo(clone);
        return clone;
    }

    private PointF[] GetDisplayPoints(BitmapDisplayPanel panel)
    {
        var points = new PointF[_vertices.Count];

        for (int i = 0; i < _vertices.Count; i++)
        {
            points[i] = panel.MapImageToDisplay(new PointF(_vertices[i].X, _vertices[i].Y), DisplayPixelAlign.Centre);
        }

        return points;
    }
}
