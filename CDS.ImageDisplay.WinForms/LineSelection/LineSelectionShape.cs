using System;
using System.ComponentModel;
using System.Drawing;
using CDS.ImageDisplay.WinForms.BitmapDisplay;
using CDS.ImageDisplay.WinForms.Overlays;
using CDS.ImageDisplay.WinForms.Utils;

namespace CDS.ImageDisplay.WinForms.LineSelection;

/// <summary>
/// Paints a line on a graphics object. Supports endpoint handles.
/// </summary>
[TypeConverter(typeof(SerializableExpandableObjectConverter))]
public class LineSelectionShape
{
    private readonly LineShape _lineShape = new();
    private readonly Rectangle[] _endpointHandleRectangles = new Rectangle[2];
    private Point _lastStartOnDisplay;
    private Point _lastEndOnDisplay;
    private int _lastHandleDiameter;

    /// <summary>
    /// True if the line is visible.
    /// </summary>
    public bool Visible { get; set; } = true;

    /// <summary>
    /// True if the endpoint handles are visible.
    /// </summary>
    public bool HandlesVisible { get; set; } = true;

    /// <summary>
    /// The start point of the line in image coordinates.
    /// </summary>
    public Point Start { get; set; }

    /// <summary>
    /// The end point of the line in image coordinates.
    /// </summary>
    public Point End { get; set; }

    /// <summary>
    /// The diameter of the endpoint handles.
    /// </summary>
    public int HandleDiameter { get; set; } = 6;

    /// <summary>
    /// Controls how image points align to the display pixel grid.
    /// </summary>
    public DisplayPixelAlign PixelAlign
    {
        get => _lineShape.PixelAlign;
        set => _lineShape.PixelAlign = value;
    }

    /// <summary>
    /// Drawing specification for the line and handles.
    /// </summary>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
    public DrawingSpec Drawing { get; set; } = new DrawingSpec();

    /// <summary>
    /// Draws the line selection on the display.
    /// </summary>
    public void Draw(BitmapDisplayPanel bitmapDisplayPanel, Graphics graphics)
    {
        ArgumentNullException.ThrowIfNull(bitmapDisplayPanel);
        ArgumentNullException.ThrowIfNull(graphics);

        if (!Visible)
        {
            return;
        }

        _lineShape.Start = Start;
        _lineShape.End = End;
        _lineShape.Draw(bitmapDisplayPanel, graphics, Drawing);

        RecalculateHandleRectangles(bitmapDisplayPanel);
        DrawHandles(graphics);
    }

    private void DrawHandles(Graphics graphics)
    {
        if (!HandlesVisible)
        {
            return;
        }

        var pen = DrawingToolsPool.GetPen(Drawing.Lines);
        var brush = DrawingToolsPool.GetBrush(Drawing.Fill);

        foreach (var handleRect in _endpointHandleRectangles)
        {
            graphics.FillEllipse(brush, handleRect);
            graphics.DrawEllipse(pen, handleRect);
        }
    }

    private void RecalculateHandleRectangles(BitmapDisplayPanel bitmapDisplayPanel)
    {
        Point startOnDisplay = bitmapDisplayPanel.MapImageToDisplay(Start, PixelAlign);
        Point endOnDisplay = bitmapDisplayPanel.MapImageToDisplay(End, PixelAlign);

        if (_lastStartOnDisplay == startOnDisplay &&
            _lastEndOnDisplay == endOnDisplay &&
            _lastHandleDiameter == HandleDiameter)
        {
            return;
        }

        _lastStartOnDisplay = startOnDisplay;
        _lastEndOnDisplay = endOnDisplay;
        _lastHandleDiameter = HandleDiameter;

        _endpointHandleRectangles[0] = CreateHandleRect(startOnDisplay);
        _endpointHandleRectangles[1] = CreateHandleRect(endOnDisplay);
    }

    private Rectangle CreateHandleRect(Point location)
    {
        return new Rectangle(
            x: location.X - (HandleDiameter / 2),
            y: location.Y - (HandleDiameter / 2),
            width: HandleDiameter,
            height: HandleDiameter);
    }

    /// <summary>
    /// String representation of the object.
    /// </summary>
    public override string ToString() => $"Line: {Start} to {End}";
}
