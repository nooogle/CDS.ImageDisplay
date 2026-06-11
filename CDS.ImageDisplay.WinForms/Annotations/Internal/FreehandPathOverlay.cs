using System;
using System.Collections.Generic;
using System.Drawing;
using CDS.ImageDisplay.WinForms.BitmapDisplay;
using CDS.ImageDisplay.WinForms.Overlays;
using CDS.ImageDisplay.WinForms.Utils;

namespace CDS.ImageDisplay.WinForms.Annotations.Internal;

/// <summary>
/// Draws the in-progress freehand gesture as a thin translucent polyline during the Drawing state.
/// </summary>
internal sealed class FreehandPathOverlay
{
    private static readonly PenSpec s_pathPen = new()
    {
        Color = Color.FromArgb(180, Color.White),
        Width = 1.5f,
    };

    private readonly List<PointF> _points = [];

    internal bool HasPoints => _points.Count > 0;

    internal void Clear() => _points.Clear();

    internal void AddPoint(PointF imagePoint) => _points.Add(imagePoint);

    /// <summary>Builds a <see cref="FreehandPath"/> from the accumulated points.</summary>
    internal FreehandPath ToFreehandPath() => FreehandPath.From(_points);

    internal void Draw(Graphics graphics, BitmapDisplayPanel panel)
    {
        if (_points.Count < 2) { return; }

        var displayPoints = new PointF[_points.Count];
        for (int i = 0; i < _points.Count; i++)
        {
            displayPoints[i] = panel.MapImageToDisplay(_points[i], DisplayPixelAlign.Centre);
        }

        Pen pen = DrawingToolsPool.GetPen(s_pathPen);
        graphics.DrawLines(pen, displayPoints);
    }
}
