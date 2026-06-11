using System;
using System.Drawing;
using System.Windows.Forms;

namespace CDS.ImageDisplay.WinForms.BitmapDisplay;

/// <summary>
/// Handles mouse-drag panning for a <see cref="BitmapDisplayPanel"/>.
/// </summary>
internal sealed class DragManager
{
    private Point _dragStartLocation;
    private PointF _initialTargetDisplayCentre;
    private readonly Action<PointF> _setTargetDisplayCentre;


    public bool IsDragging { get; private set; }


    /// <summary>
    /// Constructor
    /// </summary>
    public DragManager(Action<PointF> setTargetDisplayCentre)
    {
        _setTargetDisplayCentre = setTargetDisplayCentre;
    }

    /// <summary>
    /// Call on mouse-down to potentially start a drag.
    /// </summary>
    public void OnMouseDown(BitmapDisplayMode imageDisplayMode, MouseEventArgs mouseEventArgs, PointF currentTargetDisplayCentre)
    {
        if (!IsDragging && (imageDisplayMode == BitmapDisplayMode.Free) && (mouseEventArgs.Button == MouseButtons.Left))
        {
            _dragStartLocation = mouseEventArgs.Location;
            IsDragging = true;
            _initialTargetDisplayCentre = currentTargetDisplayCentre;
        }
    }


    /// <summary>
    /// Call on mouse-up to end a drag.
    /// </summary>
    public void OnMouseUp(MouseEventArgs mouseEventArgs)
    {
        if (IsDragging)
        {
            IsDragging = false;
        }
    }

    /// <summary>
    /// Call on mouse-move to update the display centre while dragging.
    /// </summary>
    public void OnMouseMove(MouseEventArgs mouseEventArgs)
    {
        if (!IsDragging)
        { return; }

        int xDrag = mouseEventArgs.Location.X - _dragStartLocation.X;
        int yDrag = mouseEventArgs.Location.Y - _dragStartLocation.Y;

        var newTargetDisplayCentre = new PointF(
            x: _initialTargetDisplayCentre.X + xDrag,
            y: _initialTargetDisplayCentre.Y + yDrag);

        _setTargetDisplayCentre(newTargetDisplayCentre);
    }
}
