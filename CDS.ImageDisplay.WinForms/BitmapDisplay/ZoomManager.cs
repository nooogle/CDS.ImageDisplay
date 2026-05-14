using System;
using System.Drawing;
using System.Windows.Forms;

namespace CDS.ImageDisplay.BitmapDisplay;

/// <summary>
/// Handles mouse-wheel zoom gestures for a <see cref="BitmapDisplayPanel"/>.
/// </summary>
internal class ZoomManager
{
    private Action<float, PointF, PointF> _setNewZoom;


    /// <summary>
    /// Constructor
    /// </summary>
    public ZoomManager(
        Action<float, PointF, PointF> setNewZoom)
    {
        _setNewZoom = setNewZoom;
    }


    public void OnMouseWheel(
        BitmapDisplayMode imageDisplayMode,
        float currentZoom,
        PointF mouseLocationInDisplayUnits,
        PointF mouseLocationInImageUnits,
        MouseEventArgs mouseEventArgs)
    {
        if ((mouseEventArgs.Delta == 0) || (imageDisplayMode != BitmapDisplayMode.Free)) { return; }

        var change = mouseEventArgs.Delta;

        if (change > 0)
        {
            var changeFactor = 1.0f + (change / 500.0f);
            _setNewZoom(currentZoom * changeFactor, mouseLocationInDisplayUnits, mouseLocationInImageUnits);
        }
        else if (change < 0)
        {
            var changeFactor = 1.0f + (-change / 500.0f);
            _setNewZoom(currentZoom / changeFactor, mouseLocationInDisplayUnits, mouseLocationInImageUnits);
        }
    }
}
