using System;
using System.Collections.Immutable;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using CDS.ImageDisplay.BitmapDisplay;
using CDS.ImageDisplay.Overlays;
using CDS.ImageDisplay.Utils;

namespace CDS.ImageDisplay.LineSelection;

/// <summary>
/// Manages a single interactive line selection on a <see cref="BitmapDisplayPanel"/>.
/// </summary>
public partial class SingleLineSelectionManager : Component
{
    private const string s_categoryCDS = "CDS";

    private ImmutableDictionary<LineDragMode, Cursor>? _mouseCursors;
    private LineDragMode _draggingMode;
    private Size? _imageSize;
    private bool _hasCommittedLine;
    private Point _committedLineStart;
    private Point _committedLineEnd;
    private bool _hasLiveDraggingLine;
    private Point _liveDraggingLineStart;
    private Point _liveDraggingLineEnd;
    private Point _originalDraggingLineStart;
    private Point _originalDraggingLineEnd;
    private Point _mouseDownLocationOnDisplay;

    /// <summary>
    /// How to draw the committed line.
    /// </summary>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
    public DrawingSpec CommittedLineDrawingSpec => CommittedLineShape.Drawing;

    /// <summary>
    /// How to draw the live dragging line.
    /// </summary>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
    public DrawingSpec LiveDraggingLineDrawingSpec => LiveDraggingLineShape.Drawing;

    /// <summary>
    /// The border around the line endpoints and segment that will test positive for a mouse hit.
    /// </summary>
    [DefaultValue(10)]
    public int DragBorder
    {
        get;
        set => field = Math.Max(0, Math.Min(value, 100));
    } = 10;

    /// <summary>
    /// Fired when the committed line changes.
    /// </summary>
    public event EventHandler<CommittedLineChangedEventArgs>? CommittedLineChanged;

    /// <summary>
    /// Fired when the line is being dragged.
    /// </summary>
    public event EventHandler<DraggingLineChangedEventArgs>? DraggingLineChanged;

    /// <summary>
    /// The shape for the committed line.
    /// </summary>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
    public LineSelectionShape CommittedLineShape { get; } = new();

    /// <summary>
    /// The shape for the line that is being dragged.
    /// </summary>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
    public LineSelectionShape LiveDraggingLineShape { get; } = new();

    /// <summary>
    /// True if the line is currently being dragged.
    /// </summary>
    public bool IsDragging => _draggingMode != LineDragMode.None;

    /// <summary>
    /// The bitmap display panel that the line is drawn on.
    /// </summary>
    [Category(s_categoryCDS)]
    [DefaultValue(null)]
    public BitmapDisplayPanel? BitmapDisplayPanel
    {
        get;
        set
        {
            if (field != value)
            {
                if (field != null)
                {
                    field.MouseDown -= BitmapDisplayPanel_MouseDown;
                    field.MouseMove -= BitmapDisplayPanel_MouseMove;
                    field.MouseUp -= BitmapDisplayPanel_MouseUp;
                    field.PaintOver -= BitmapDisplayPanel_OnPaintOver;
                    field.ImageSizeChanged -= BitmapDisplayPanel_OnImageSizeChanged;
                }

                field = value;

                if (field != null)
                {
                    field.MouseDown += BitmapDisplayPanel_MouseDown;
                    field.MouseMove += BitmapDisplayPanel_MouseMove;
                    field.MouseUp += BitmapDisplayPanel_MouseUp;
                    field.PaintOver += BitmapDisplayPanel_OnPaintOver;
                    field.ImageSizeChanged += BitmapDisplayPanel_OnImageSizeChanged;

                    Size existingSize = field.ImageSize;
                    _imageSize = existingSize == Size.Empty ? null : existingSize;
                }
                else
                {
                    _imageSize = null;
                }
            }
        }
    }

    /// <summary>
    /// Controls whether the line is visible.
    /// </summary>
    [DefaultValue(true)]
    public bool Visible
    {
        get;
        set
        {
            if (field != value)
            {
                field = value;
                BitmapDisplayPanel?.Invalidate();
            }
        }
    } = true;

    /// <summary>
    /// Controls whether the committed line can be edited.
    /// </summary>
    [DefaultValue(true)]
    public bool CanEditCommitted
    {
        get;
        set
        {
            if (field != value)
            {
                field = value;
                BitmapDisplayPanel?.Invalidate();
            }
        }
    } = true;

    /// <summary>
    /// Controls whether a new line can be created.
    /// </summary>
    [DefaultValue(true)]
    public bool CanCreateNew
    {
        get;
        set
        {
            if (field != value)
            {
                field = value;
                BitmapDisplayPanel?.Invalidate();
            }
        }
    } = true;

    /// <summary>
    /// True when a committed line exists.
    /// </summary>
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool HasCommittedLine => _hasCommittedLine;

    /// <summary>
    /// Gets or sets the committed line in image coordinates. Set <see langword="null"/> to clear it.
    /// </summary>
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public (Point Start, Point End)? CommittedLine
    {
        get => _hasCommittedLine ? (_committedLineStart, _committedLineEnd) : null;
        set
        {
            (Point Start, Point End)? oldValue = _hasCommittedLine ? (_committedLineStart, _committedLineEnd) : null;

            if (!imageSizeHasValue())
            {
                ClearCommittedLine();
                return;
            }

            if (value is null)
            {
                ClearCommittedLine();
                return;
            }

            Point clampedStart = ClampToImageBounds(value.Value.Start);
            Point clampedEnd = ClampToImageBounds(value.Value.End);

            if (!_hasCommittedLine || clampedStart != _committedLineStart || clampedEnd != _committedLineEnd)
            {
                _committedLineStart = clampedStart;
                _committedLineEnd = clampedEnd;
                _hasCommittedLine = true;
                CommittedLineChanged?.Invoke(this, new CommittedLineChangedEventArgs((_committedLineStart, _committedLineEnd)));
                BitmapDisplayPanel?.Invalidate();
            }

            bool imageSizeHasValue() => _imageSize.HasValue && _imageSize.Value.Width > 0 && _imageSize.Value.Height > 0;
        }
    }

    /// <summary>
    /// Gets the line that is currently being dragged, or <see langword="null"/> if no drag is in progress.
    /// </summary>
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public (Point Start, Point End)? LiveDraggingLine => _hasLiveDraggingLine ? (_liveDraggingLineStart, _liveDraggingLineEnd) : null;

    /// <summary>
    /// Initializes a new instance of the <see cref="SingleLineSelectionManager"/> class.
    /// </summary>
    public SingleLineSelectionManager()
    {
        InitializeComponent();
        CompleteInitialisation();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SingleLineSelectionManager"/> class.
    /// </summary>
    /// <param name="container">The component container.</param>
    public SingleLineSelectionManager(IContainer container)
    {
        ArgumentNullException.ThrowIfNull(container, nameof(container));

        container.Add(this);
        InitializeComponent();
        CompleteInitialisation();
    }

    private void CompleteInitialisation()
    {
        _mouseCursors = CreateMouseCursorsDict();

        CommittedLineShape.Drawing.Lines.Color = Color.FromArgb(128, Color.Green);
        CommittedLineShape.Drawing.Lines.Width = 2;
        CommittedLineShape.Drawing.Fill.Color = Color.Transparent;

        LiveDraggingLineShape.Drawing.Lines.Color = Color.FromArgb(128, Color.Orange);
        LiveDraggingLineShape.Drawing.Lines.Width = 2;
        LiveDraggingLineShape.Drawing.Fill.Color = Color.Transparent;
    }

    private static ImmutableDictionary<LineDragMode, Cursor> CreateMouseCursorsDict()
    {
        ImmutableDictionary<LineDragMode, Cursor>.Builder builder = ImmutableDictionary.CreateBuilder<LineDragMode, Cursor>();

        builder.Add(LineDragMode.None, Cursors.Default);
        builder.Add(LineDragMode.WholeLine, Cursors.SizeAll);
        builder.Add(LineDragMode.StartPoint, Cursors.Cross);
        builder.Add(LineDragMode.EndPoint, Cursors.Cross);

        return builder.ToImmutable();
    }

    private bool CanWorkWithLine => (BitmapDisplayPanel != null) && _imageSize.HasValue && _imageSize.Value.Width > 0 && _imageSize.Value.Height > 0;

    private static bool IsSpacebarPressed()
    {
        return (Win32Imports.GetKeyState(Win32Imports.VK_SPACE) & 0x8000) != 0;
    }

    private void BitmapDisplayPanel_OnImageSizeChanged(object? sender, ImageSizeChangedEventArgs e)
    {
        _imageSize = e.NewSize == Size.Empty ? null : e.NewSize;

        if (_hasCommittedLine && _imageSize.HasValue)
        {
            CommittedLine = (_committedLineStart, _committedLineEnd);
        }
        else if (!_imageSize.HasValue)
        {
            ClearCommittedLine();
        }
    }

    private void BitmapDisplayPanel_MouseDown(object? sender, MouseEventArgs e)
    {
        if (!CanWorkWithLine)
        {
            return;
        }

        if (!IsSpacebarPressed() && e.Button == MouseButtons.Left)
        {
            OnLeftMouseButtonDown(e);
        }
    }

    private void OnLeftMouseButtonDown(MouseEventArgs e)
    {
        _mouseDownLocationOnDisplay = e.Location;

        Point imagePoint = Point.Round(BitmapDisplayPanel!.MapDisplayToImage(e.Location));
        LineDragMode newDragMode = DetermineDragModeFromMouseLocation(e.Location);

        if (newDragMode != LineDragMode.None)
        {
            StartDraggingCommittedLine(newDragMode);
        }
        else
        {
            StartDraggingNewLine(imagePoint);
        }

        BitmapDisplayPanel.Invalidate();
    }

    private LineDragMode DetermineDragModeFromMouseLocation(Point mouseLocationOnDisplay)
    {
        if (!CanEditCommitted || !_hasCommittedLine || BitmapDisplayPanel == null)
        {
            return LineDragMode.None;
        }

        Point committedStartOnDisplay = BitmapDisplayPanel.MapImageToDisplay(_committedLineStart, DisplayPixelAlign.Centre);
        Point committedEndOnDisplay = BitmapDisplayPanel.MapImageToDisplay(_committedLineEnd, DisplayPixelAlign.Centre);

        if (CreateHandleHitArea(committedStartOnDisplay).Contains(mouseLocationOnDisplay))
        {
            return LineDragMode.StartPoint;
        }

        if (CreateHandleHitArea(committedEndOnDisplay).Contains(mouseLocationOnDisplay))
        {
            return LineDragMode.EndPoint;
        }

        return IsMouseNearCommittedLine(mouseLocationOnDisplay, committedStartOnDisplay, committedEndOnDisplay)
            ? LineDragMode.WholeLine
            : LineDragMode.None;
    }

    private Rectangle CreateHandleHitArea(Point point)
    {
        return new Rectangle(
            x: point.X - DragBorder,
            y: point.Y - DragBorder,
            width: DragBorder * 2,
            height: DragBorder * 2);
    }

    private bool IsMouseNearCommittedLine(Point mouseLocationOnDisplay, Point startOnDisplay, Point endOnDisplay)
    {
        Rectangle lineBounds = Rectangle.FromLTRB(
            Math.Min(startOnDisplay.X, endOnDisplay.X),
            Math.Min(startOnDisplay.Y, endOnDisplay.Y),
            Math.Max(startOnDisplay.X, endOnDisplay.X),
            Math.Max(startOnDisplay.Y, endOnDisplay.Y));

        lineBounds.Inflate(DragBorder, DragBorder);
        if (!lineBounds.Contains(mouseLocationOnDisplay))
        {
            return false;
        }

        return DistanceFromPointToSegment(mouseLocationOnDisplay, startOnDisplay, endOnDisplay) <= DragBorder;
    }

    private void StartDraggingNewLine(Point imagePoint)
    {
        if (!CanCreateNew || BitmapDisplayPanel == null)
        {
            return;
        }

        _originalDraggingLineStart = imagePoint;
        _originalDraggingLineEnd = imagePoint;
        SetLiveDraggingLine(imagePoint, imagePoint);
        _draggingMode = LineDragMode.EndPoint;
        BitmapDisplayPanel.SuppressDragging();
    }

    private void StartDraggingCommittedLine(LineDragMode newDragMode)
    {
        if (!CanEditCommitted || !_hasCommittedLine || BitmapDisplayPanel == null)
        {
            return;
        }

        _originalDraggingLineStart = _committedLineStart;
        _originalDraggingLineEnd = _committedLineEnd;
        SetLiveDraggingLine(_originalDraggingLineStart, _originalDraggingLineEnd);
        _draggingMode = newDragMode;
        BitmapDisplayPanel.SuppressDragging();
    }

    private void BitmapDisplayPanel_MouseMove(object? sender, MouseEventArgs e)
    {
        if (!CanWorkWithLine)
        {
            return;
        }

        if (_draggingMode != LineDragMode.None)
        {
            UpdateDraggingLine(e.Location);
        }
        else if (_hasCommittedLine)
        {
            LineDragMode dragModeIfMouseClicked = DetermineDragModeFromMouseLocation(e.Location);
            BitmapDisplayPanel!.Cursor = _mouseCursors![dragModeIfMouseClicked];
        }
    }

    private void UpdateDraggingLine(Point mouseLocationOnDisplay)
    {
        if (BitmapDisplayPanel == null || _mouseCursors == null)
        {
            return;
        }

        Point currentMouseLocationOnImage = Point.Round(BitmapDisplayPanel.MapDisplayToImage(mouseLocationOnDisplay));

        switch (_draggingMode)
        {
            case LineDragMode.StartPoint:
                SetLiveDraggingLine(currentMouseLocationOnImage, _originalDraggingLineEnd);
                break;
            case LineDragMode.EndPoint:
                SetLiveDraggingLine(_originalDraggingLineStart, currentMouseLocationOnImage);
                break;
            case LineDragMode.WholeLine:
                UpdateWholeLineDragging(currentMouseLocationOnImage);
                break;
            default:
                break;
        }

        BitmapDisplayPanel.Cursor = _mouseCursors[_draggingMode];
        BitmapDisplayPanel.Invalidate();
    }

    private void UpdateWholeLineDragging(Point currentMouseLocationOnImage)
    {
        if (BitmapDisplayPanel == null)
        {
            return;
        }

        Point mouseDownLocationOnImage = Point.Round(BitmapDisplayPanel.MapDisplayToImage(_mouseDownLocationOnDisplay));
        int deltaX = currentMouseLocationOnImage.X - mouseDownLocationOnImage.X;
        int deltaY = currentMouseLocationOnImage.Y - mouseDownLocationOnImage.Y;

        SetLiveDraggingLine(
            new Point(_originalDraggingLineStart.X + deltaX, _originalDraggingLineStart.Y + deltaY),
            new Point(_originalDraggingLineEnd.X + deltaX, _originalDraggingLineEnd.Y + deltaY));
    }

    private void BitmapDisplayPanel_MouseUp(object? sender, MouseEventArgs e)
    {
        if (!CanWorkWithLine || _draggingMode == LineDragMode.None || BitmapDisplayPanel == null)
        {
            return;
        }

        CommittedLine = (_liveDraggingLineStart, _liveDraggingLineEnd);
        ClearLiveDraggingLine();
        _draggingMode = LineDragMode.None;
        BitmapDisplayPanel.UnsuppressDragging();
        BitmapDisplayPanel.Cursor = Cursors.Default;
        BitmapDisplayPanel.Invalidate();
    }

    private void BitmapDisplayPanel_OnPaintOver(object? sender, PaintOverEventArgs e)
    {
        if (!CanWorkWithLine || !Visible)
        {
            return;
        }

        var bitmapDisplayPanel = e.Sender;

        if (_hasCommittedLine)
        {
            CommittedLineShape.Start = _committedLineStart;
            CommittedLineShape.End = _committedLineEnd;
            CommittedLineShape.Draw(bitmapDisplayPanel, e.Graphics);
        }

        if (_draggingMode != LineDragMode.None && _hasLiveDraggingLine)
        {
            LiveDraggingLineShape.Start = _liveDraggingLineStart;
            LiveDraggingLineShape.End = _liveDraggingLineEnd;
            LiveDraggingLineShape.Draw(bitmapDisplayPanel, e.Graphics);
        }
    }

    private void SetLiveDraggingLine(Point start, Point end)
    {
        if (!_hasLiveDraggingLine || start != _liveDraggingLineStart || end != _liveDraggingLineEnd)
        {
            _liveDraggingLineStart = start;
            _liveDraggingLineEnd = end;
            _hasLiveDraggingLine = true;
            DraggingLineChanged?.Invoke(this, new DraggingLineChangedEventArgs((start, end)));
            BitmapDisplayPanel?.Invalidate();
        }
    }

    private void ClearLiveDraggingLine()
    {
        _hasLiveDraggingLine = false;
        _liveDraggingLineStart = Point.Empty;
        _liveDraggingLineEnd = Point.Empty;
    }

    private void ClearCommittedLine()
    {
        if (_hasCommittedLine)
        {
            _hasCommittedLine = false;
            _committedLineStart = Point.Empty;
            _committedLineEnd = Point.Empty;
            BitmapDisplayPanel?.Invalidate();
        }
    }

    private Point ClampToImageBounds(Point point)
    {
        if (!_imageSize.HasValue || _imageSize.Value.Width <= 0 || _imageSize.Value.Height <= 0)
        {
            return Point.Empty;
        }

        return new Point(
            x: Math.Clamp(point.X, 0, _imageSize.Value.Width - 1),
            y: Math.Clamp(point.Y, 0, _imageSize.Value.Height - 1));
    }

    private static float DistanceFromPointToSegment(Point point, Point segmentStart, Point segmentEnd)
    {
        if (segmentStart == segmentEnd)
        {
            int dx = point.X - segmentStart.X;
            int dy = point.Y - segmentStart.Y;
            return MathF.Sqrt((dx * dx) + (dy * dy));
        }

        float segmentDeltaX = segmentEnd.X - segmentStart.X;
        float segmentDeltaY = segmentEnd.Y - segmentStart.Y;
        float pointDeltaX = point.X - segmentStart.X;
        float pointDeltaY = point.Y - segmentStart.Y;
        float segmentLengthSquared = (segmentDeltaX * segmentDeltaX) + (segmentDeltaY * segmentDeltaY);
        float projection = ((pointDeltaX * segmentDeltaX) + (pointDeltaY * segmentDeltaY)) / segmentLengthSquared;
        float clampedProjection = Math.Clamp(projection, 0f, 1f);
        float nearestX = segmentStart.X + (clampedProjection * segmentDeltaX);
        float nearestY = segmentStart.Y + (clampedProjection * segmentDeltaY);
        float distanceX = point.X - nearestX;
        float distanceY = point.Y - nearestY;

        return MathF.Sqrt((distanceX * distanceX) + (distanceY * distanceY));
    }

    private enum LineDragMode
    {
        None,
        WholeLine,
        StartPoint,
        EndPoint,
    }
}
