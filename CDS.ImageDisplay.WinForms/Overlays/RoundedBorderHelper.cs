using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace CDS.ImageDisplay.WinForms.Overlays;

/// <summary>
/// Utility to help draw rounded borders around a rectangle.
/// </summary>
public class RoundedBorderHelper : IDisposable
{
    /// <summary>
    /// Fired when the border needs to be redrawn.
    /// </summary>
    public event EventHandler? RedrawNeeded;


    /// <summary>Default coner radius.</summary>
    public const int defCornerRadius = 6;


    /// <summary>Corner radius.</summary>
    private int _cornerRadius = defCornerRadius;


    /// <summary>
    /// Cached graphics path used to define the border of the button.
    /// </summary>
    private GraphicsPath? _borderGraphicsPath = new GraphicsPath();


    /// <summary>
    /// Defines which corners should be rounded. The others will be straight.
    /// </summary>
    private RoundedPanelCorners _corners = RoundedPanelCorners.All;


    /// <summary>
    /// The square-edged rectangle that defines the border as it would be drawn
    /// by Graphics.DrawRect()
    /// </summary>
    private Rectangle _rect = Rectangle.Empty;


    /// <summary>The path the describes the border.</summary>
    public GraphicsPath? BorderGraphicsPath => _borderGraphicsPath;


    /// <summary>
    /// The square-edged rectangle that defines the border as it would be drawn
    /// by Graphics.DrawRect()
    /// </summary>
    public Rectangle Rect
    {
        get => _rect;

        set
        {
            if (_rect != value)
            {
                _rect = value;
                GenerateBorderGraphicsPath();
            }
        }
    }


    /// <summary>Corner radius.</summary>
    public int CornerRadius
    {
        get => _cornerRadius;

        set
        {
            if ((value < 0) || (value > 100))
            {
                throw new ArgumentException("The value must be between 0 and 100 (inclusive).");
            }

            if (_cornerRadius != value)
            {
                _cornerRadius = value;
                GenerateBorderGraphicsPath();
            }
        }
    }


    /// <summary>
    /// Defines which corners should be rounded. The others will be straight.
    /// </summary>
    public RoundedPanelCorners Corners
    {
        get => _corners;

        set
        {
            if (_corners != value)
            {
                _corners = value;
                GenerateBorderGraphicsPath();
            }
        }
    }


    /// <summary>
    /// Initialise.
    /// </summary>
    public RoundedBorderHelper()
    {
        _rect = new Rectangle(0, 0, 32, 32);
        GenerateBorderGraphicsPath();
    }


    /// <summary>
    /// Cleanup managed resources.
    /// </summary>
    public void Dispose()
    {
        if (_borderGraphicsPath != null)
        {
            _borderGraphicsPath.Dispose();
            _borderGraphicsPath = null;
        }
    }




    /// <summary>
    /// Generates a graphic path that defines the border of the control. This will
    /// be filled and outlined.
    /// </summary>
    private void GenerateBorderGraphicsPath()
    {
        if (_borderGraphicsPath == null) { return; }

        // Drop existing path
        _borderGraphicsPath.Reset();

        int left = _rect.X;
        int top = _rect.Y;
        int right = _rect.Right - 1;
        int bottom = _rect.Bottom - 1;

        bool roundTopLeft = ((_corners & RoundedPanelCorners.TopLeft) == RoundedPanelCorners.TopLeft);
        bool roundTopRight = ((_corners & RoundedPanelCorners.TopRight) == RoundedPanelCorners.TopRight);
        bool roundBottomRight = ((_corners & RoundedPanelCorners.BottomRight) == RoundedPanelCorners.BottomRight);
        bool roundBottomLeft = ((_corners & RoundedPanelCorners.BottomLeft) == RoundedPanelCorners.BottomLeft);

        int topLeftRadius = roundTopLeft ? _cornerRadius : 0;
        int topRightRadius = roundTopRight ? _cornerRadius : 0;
        int bottomRightRadius = roundBottomRight ? _cornerRadius : 0;
        int bottomLeftRadius = roundBottomLeft ? _cornerRadius : 0;

        int topLeftDiameter = topLeftRadius * 2;
        int topRightDiameter = topRightRadius * 2;
        int bottomRightDiameter = bottomRightRadius * 2;
        int bottomLeftDiameter = bottomLeftRadius * 2;

        _borderGraphicsPath.StartFigure();

        // Top-left corner
        if (topLeftDiameter > 0)
        {
            _borderGraphicsPath.AddArc(left, top, topLeftDiameter, topLeftDiameter, 180, 90);
        }

        // Top horiz line
        _borderGraphicsPath.AddLine(left + topLeftRadius, top, right - topRightRadius, top);

        // Top right corner
        if (topRightDiameter > 0)
        {
            _borderGraphicsPath.AddArc(right - topRightDiameter, top, topRightDiameter, topRightDiameter, 270, 90);
        }

        // Right vertical line
        _borderGraphicsPath.AddLine(right, top + topRightRadius, right, bottom - bottomRightRadius);

        // Bottom right corner
        if (bottomRightDiameter > 0)
        {
            _borderGraphicsPath.AddArc(right - bottomRightDiameter, bottom - bottomRightDiameter, bottomRightDiameter, bottomRightDiameter, 0, 90);
        }

        // Bottom horiz line
        _borderGraphicsPath.AddLine(right - bottomRightRadius, bottom, left + bottomLeftRadius, bottom);

        // Bottom left corner
        if (bottomLeftDiameter > 0)
        {
            _borderGraphicsPath.AddArc(left, bottom - bottomLeftDiameter, bottomLeftDiameter, bottomLeftDiameter, 90, 90);
        }

        // All done!
        _borderGraphicsPath.CloseFigure();

        // Let clients know we need to redraw
        RedrawNeeded?.Invoke(this, EventArgs.Empty);
    }
}
