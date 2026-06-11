using System;
using System.Drawing;
using CDS.ImageDisplay.WinForms.BitmapDisplay;

namespace CDS.ImageDisplay.WinForms.Annotations.Internal;

/// <summary>
/// Shared drawing and hit-testing helpers used by annotation geometry classes.
/// </summary>
internal static class AnnotationHandleHelper
{
    /// <summary>Diameter of selection handles in display pixels.</summary>
    internal const int HandleDiameter = 8;

    /// <summary>
    /// Returns true when the mouse is within handle-capture distance of a display point.
    /// </summary>
    internal static bool IsNearPoint(Point mouseDisplay, PointF handleDisplay, int extraBorder = 0)
    {
        float radius = (HandleDiameter / 2f) + extraBorder;
        float dx = mouseDisplay.X - handleDisplay.X;
        float dy = mouseDisplay.Y - handleDisplay.Y;
        return (dx * dx) + (dy * dy) <= radius * radius;
    }

    /// <summary>
    /// Draws a circular handle centred at a display-coordinate point.
    /// </summary>
    internal static void DrawHandle(Graphics graphics, System.Drawing.Pen pen, System.Drawing.Brush brush, PointF centreDisplay)
    {
        float r = HandleDiameter / 2f;
        graphics.FillEllipse(brush, centreDisplay.X - r, centreDisplay.Y - r, HandleDiameter, HandleDiameter);
        graphics.DrawEllipse(pen, centreDisplay.X - r, centreDisplay.Y - r, HandleDiameter, HandleDiameter);
    }

    /// <summary>
    /// Converts a display-pixel delta into an image-pixel delta, accounting for the current zoom.
    /// </summary>
    internal static Size DisplayDeltaToImageDelta(BitmapDisplayPanel panel, Point displayDelta)
    {
        Guard.ThrowIfNull(panel, nameof(panel));

        PointF zero = panel.MapDisplayToImage(PointF.Empty);
        PointF moved = panel.MapDisplayToImage(new PointF(displayDelta.X, displayDelta.Y));

        return new Size(
            (int)Math.Round(moved.X - zero.X),
            (int)Math.Round(moved.Y - zero.Y));
    }

    /// <summary>
    /// Returns the distance from a point to a line segment.
    /// </summary>
    internal static float DistanceFromPointToSegment(PointF point, PointF segmentStart, PointF segmentEnd)
    {
        float segDx = segmentEnd.X - segmentStart.X;
        float segDy = segmentEnd.Y - segmentStart.Y;
        float lengthSquared = (segDx * segDx) + (segDy * segDy);

        if (lengthSquared == 0f)
        {
            float dx = point.X - segmentStart.X;
            float dy = point.Y - segmentStart.Y;
            return MathF.Sqrt((dx * dx) + (dy * dy));
        }

        float t = Math.Min(Math.Max(
            ((point.X - segmentStart.X) * segDx + (point.Y - segmentStart.Y) * segDy) / lengthSquared,
            0f), 1f);

        float nearX = segmentStart.X + t * segDx;
        float nearY = segmentStart.Y + t * segDy;
        float distX = point.X - nearX;
        float distY = point.Y - nearY;

        return MathF.Sqrt((distX * distX) + (distY * distY));
    }

    /// <summary>
    /// Returns true if the given point lies inside the polygon (even-odd rule).
    /// Requires at least 3 vertices.
    /// </summary>
    internal static bool IsPointInPolygon(PointF[] polygon, PointF point)
    {
        bool inside = false;
        int j = polygon.Length - 1;

        for (int i = 0; i < polygon.Length; i++)
        {
            if ((polygon[i].Y > point.Y) != (polygon[j].Y > point.Y) &&
                point.X < (polygon[j].X - polygon[i].X) * (point.Y - polygon[i].Y) / (polygon[j].Y - polygon[i].Y) + polygon[i].X)
            {
                inside = !inside;
            }

            j = i;
        }

        return inside;
    }
}
