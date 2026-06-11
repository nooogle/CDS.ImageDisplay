using System;
using System.Collections.Immutable;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using CDS.ImageDisplay.WinForms.Utils;

namespace CDS.ImageDisplay.WinForms.RegionOfInterest;

/// <summary>
/// Manages a region of interest (ROI) on a <see cref="BitmapDisplay.BitmapDisplayPanel"/>
/// </summary>
public partial class SingleROIManager : Component
{
    private const string categoryCDS = "CDS";

    private ImmutableDictionary<ROIDragMode, Cursor>? mouseCursors;
    private ROIDragMode draggingMode = ROIDragMode.None;
    private Size? imageSize;
    private Rectangle committedROI;
    private Rectangle originalDraggingROI;
    private Rectangle liveDraggingROI;
    private Point mouseDownLocationOnDisplay;


    /// <summary>
    /// How to draw the committed ROI.
    /// </summary>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
    public Overlays.DrawingSpec CommittedROIDrawingSpec => CommittedROIShape.Drawing;



    /// <summary>
    /// How to draw the live dragging ROI.
    /// </summary>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
    public Overlays.DrawingSpec LiveDraggingROIDrawingSpec => LiveDraggingROIShape.Drawing;


    /// <summary>
    /// The border inside and outside the ROI that will test positive
    /// for a mouse hit test. (E.g. this allows the mouse to drag a corner
    /// or edge even if it's not exactly over the edge.)
    /// </summary>
    [DefaultValue(10)]
    public int DragBorder
    {
        get;
        set => field = Math.Max(0, Math.Min(value, 100));
    } = 10;


    /// <summary>
    /// Fired when the committed ROI changes.
    /// </summary>
    public event EventHandler<CommittedROIChangedEventArgs>? CommittedROIChanged;


    /// <summary>
    /// Fired when the ROI is being dragged. The reported rectangle may extend outside the image
    /// bounds — only <see cref="CommittedROI"/> is clamped (on mouse-up).
    /// </summary>
    public event EventHandler<DraggingROIChangedEventArgs>? DraggingROIChanged;


    /// <summary>
    /// The shape for the committed ROI.
    /// </summary>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
    public ROIWithGrapplesShape CommittedROIShape { get; } = new ROIWithGrapplesShape();


    /// <summary>
    /// The shape for the ROI that is being dragged.
    /// </summary>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
    public ROIWithGrapplesShape LiveDraggingROIShape { get; } = new ROIWithGrapplesShape();


    /// <summary>
    /// When true the ROI is drawn even when it is the same (full) size as the image.
    /// Normally the ROI is not drawn in this case.
    /// </summary>
    [DefaultValue(false)]
    public bool DrawCommittedROIWhenFullSize { get; set; } = false;


    /// <summary>
    /// True if the ROI is currently being dragged.
    /// </summary>
    public bool IsDragging => draggingMode != ROIDragMode.None;


    /// <summary>
    /// The bitmap display panel that the ROI is drawn on.
    /// </summary>
    [Category(categoryCDS)]
    [DefaultValue(null)]
    public BitmapDisplay.BitmapDisplayPanel? BitmapDisplayPanel
    {
        get; set
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
                    field.KeyDown -= BitmapDisplayPanel_KeyDown;
                }

                field = value;

                if (field != null)
                {
                    field.MouseDown += BitmapDisplayPanel_MouseDown;
                    field.MouseMove += BitmapDisplayPanel_MouseMove;
                    field.MouseUp += BitmapDisplayPanel_MouseUp;
                    field.PaintOver += BitmapDisplayPanel_OnPaintOver;
                    field.ImageSizeChanged += BitmapDisplayPanel_OnImageSizeChanged;
                    field.KeyDown += BitmapDisplayPanel_KeyDown;

                    // Seed imageSize from the panel's current image so that ROIs can
                    // be set immediately, without waiting for the next OnImageSizeChanged event.
                    Size existingSize = field.ImageSize;
                    imageSize = existingSize == Size.Empty ? null : existingSize;
                }
                else
                {
                    imageSize = null;
                }
            }
        }
    }

    private void BitmapDisplayPanel_KeyDown(object? sender, KeyEventArgs e)
    {
        if (draggingMode != ROIDragMode.None)
        { return; }
        if (!Visible)
        { return; }

        int change = 1;
        if (e.Control)
        { change = 10; }

        if (e.KeyCode == Keys.Left)
        {
            CommittedROI = e.Shift
                ? new Rectangle(CommittedROI.Left, CommittedROI.Top, CommittedROI.Width - change, CommittedROI.Height)
                : new Rectangle(CommittedROI.Left - change, CommittedROI.Top, CommittedROI.Width, CommittedROI.Height);
        }
        else if (e.KeyCode == Keys.Right)
        {
            CommittedROI = e.Shift
                ? new Rectangle(CommittedROI.Left, CommittedROI.Top, CommittedROI.Width + change, CommittedROI.Height)
                : new Rectangle(CommittedROI.Left + change, CommittedROI.Top, CommittedROI.Width, CommittedROI.Height);
        }
        else if (e.KeyCode == Keys.Up)
        {
            CommittedROI = e.Shift
                ? new Rectangle(CommittedROI.Left, CommittedROI.Top, CommittedROI.Width, CommittedROI.Height - change)
                : new Rectangle(CommittedROI.Left, CommittedROI.Top - change, CommittedROI.Width, CommittedROI.Height);
        }
        else if (e.KeyCode == Keys.Down)
        {
            CommittedROI = e.Shift
                ? new Rectangle(CommittedROI.Left, CommittedROI.Top, CommittedROI.Width, CommittedROI.Height + change)
                : new Rectangle(CommittedROI.Left, CommittedROI.Top + change, CommittedROI.Width, CommittedROI.Height);
        }
    }




    /// <summary>
    /// Handle a change to the image size
    /// </summary>
    private void BitmapDisplayPanel_OnImageSizeChanged(object? sender, BitmapDisplay.ImageSizeChangedEventArgs e)
        => imageSize = e.NewSize;


    /// <summary>
    /// Controls whether the ROI is visible.
    /// </summary>
    [DefaultValue(true)]
    public bool Visible
    {
        get; set
        {
            if (field != value)
            {
                field = value;
                BitmapDisplayPanel?.Invalidate();
            }
        }
    } = true;


    /// <summary>
    /// Controls whether the ROI can changed. 
    /// </summary>
    [DefaultValue(true)]
    public bool CanEditCommitted
    {
        get; set
        {
            if (field != value)
            {
                field = value;
                BitmapDisplayPanel?.Invalidate();
            }
        }
    } = true;


    /// <summary>
    /// Controls whether a new ROI can be created.
    /// </summary>
    [DefaultValue(true)]
    public bool CanCreateNew
    {
        get; set
        {
            if (field != value)
            {
                field = value;
                BitmapDisplayPanel?.Invalidate();
            }
        }
    } = true;



    /// <summary>
    /// Initializes a new instance of the <see cref="SingleROIManager"/> class.
    /// </summary>
    public SingleROIManager()
    {
        InitializeComponent();
        CompleteInitialisation();
    }


    /// <summary>
    /// Initializes a new instance of the <see cref="SingleROIManager"/> class.
    /// </summary>
    public SingleROIManager(IContainer container)
    {
        ArgumentNullException.ThrowIfNull(container);

        container.Add(this);
        InitializeComponent();
        CompleteInitialisation();
    }


    /// <summary>
    /// Completes the initialisation of the ROI manager.
    /// </summary>
    private void CompleteInitialisation()
    {
        mouseCursors = CreateMouseCursorsDict();

        CommittedROIShape.Drawing.Lines.Color = Color.FromArgb(128, Color.Green);
        CommittedROIShape.Drawing.Lines.Width = 2;
        CommittedROIShape.Drawing.Fill.Color = Color.Transparent;

        LiveDraggingROIShape.Drawing.Lines.Color = Color.FromArgb(128, Color.Orange);
        LiveDraggingROIShape.Drawing.Lines.Width = 2;
        LiveDraggingROIShape.Drawing.Fill.Color = Color.Transparent;
    }


    /// <summary>
    /// Gets/sets the current (committed) ROI. An empty rectangle indicates no ROI is set.
    /// </summary>
    [DefaultValue(typeof(Rectangle), "0, 0, 0, 0")]
    public Rectangle CommittedROI
    {
        get => committedROI;

        set
        {
            Rectangle originalCommittedROI = committedROI;

            if (!imageSize.HasValue)
            {
                committedROI = Rectangle.Empty;
            }
            else
            {
                Rectangle newCommittedROI = value;
                newCommittedROI.Intersect(new Rectangle(Point.Empty, imageSize!.Value));
                if (newCommittedROI.Size.IsEmpty)
                { newCommittedROI = Rectangle.Empty; }

                if (newCommittedROI != committedROI)
                {
                    committedROI = newCommittedROI;
                    CommittedROIChanged?.Invoke(this, new CommittedROIChangedEventArgs(committedROI));
                    BitmapDisplayPanel?.Invalidate();
                }
            }
        }
    }


    /// <summary>
    /// Gets the ROI that is currently being dragged, or returns an empty rectangle if no drag is in progress.
    /// The rectangle is in image coordinates and may extend outside the image bounds; clamping is applied
    /// only when the drag is committed (see <see cref="CommittedROI"/>).
    /// </summary>
    public Rectangle LiveDraggingROI
    {
        get => liveDraggingROI;

        private set
        {
            if (liveDraggingROI != value)
            {
                liveDraggingROI = value;
                DraggingROIChanged?.Invoke(this, new DraggingROIChangedEventArgs(liveDraggingROI));
                BitmapDisplayPanel?.Invalidate();
            }
        }
    }


    /// <summary>
    /// Creates a dictionary of cursors for each dragging mode.
    /// </summary>
    private static ImmutableDictionary<ROIDragMode, Cursor> CreateMouseCursorsDict()
    {
        ImmutableDictionary<ROIDragMode, Cursor>.Builder builder = ImmutableDictionary.CreateBuilder<ROIDragMode, Cursor>();

        builder.Add(ROIDragMode.None, Cursors.Default);
        builder.Add(ROIDragMode.WholeROI, Cursors.Hand);
        builder.Add(ROIDragMode.TopLeftCorner, Cursors.SizeNWSE);
        builder.Add(ROIDragMode.TopRightCorner, Cursors.SizeNESW);
        builder.Add(ROIDragMode.BottomLeftCorner, Cursors.SizeNESW);
        builder.Add(ROIDragMode.BottomRightCorner, Cursors.SizeNWSE);
        builder.Add(ROIDragMode.TopEdge, Cursors.SizeNS);
        builder.Add(ROIDragMode.BottomEdge, Cursors.SizeNS);
        builder.Add(ROIDragMode.LeftEdge, Cursors.SizeWE);
        builder.Add(ROIDragMode.RightEdge, Cursors.SizeWE);

        return builder.ToImmutable();
    }


    /// <summary>
    /// True if there's an image that the ROI can be applied to and that 
    /// </summary>
    private bool CanWorkWithROI => (BitmapDisplayPanel != null) && imageSize.HasValue;



    /// <summary>
    /// True if the spacebar is pressed.
    /// </summary>
    private static bool IsSpacebarPressed()
    {
        return (Win32Imports.GetKeyState(Win32Imports.VK_SPACE) & 0x8000) != 0;
    }


    /// <summary>
    /// Handles the mouse down event to begin defining an ROI.
    /// </summary>
    private void BitmapDisplayPanel_MouseDown(object? sender, MouseEventArgs e)
    {
        if (!CanWorkWithROI)
        { return; }

        if (!IsSpacebarPressed() && (e.Button == MouseButtons.Left))
        {
            OnLeftMouseButtonDown(e);
        }
    }


    /// <summary>
    /// Handles the mouse down event to begin defining an ROI.
    /// </summary>
    private void OnLeftMouseButtonDown(MouseEventArgs e)
    {
        mouseDownLocationOnDisplay = e.Location;

        var imagePoint = Point.Round(BitmapDisplayPanel!.MapDisplayToImage(e.Location));
        ROIDragMode newDragMode = DetermineDragModeFromMouseLocation(mouseLocationOnDisplay: e.Location);

        if (newDragMode != ROIDragMode.None)
        {
            StartDraggingCommittedROI(newDragMode);
        }
        else
        {
            StartDraggingNewROI(imagePoint);
        }

        BitmapDisplayPanel?.Invalidate();
    }


    private ROIDragMode DetermineDragModeFromMouseLocation(Point mouseLocationOnDisplay)
    {
        if (!CanEditCommitted)
        { return ROIDragMode.None; }
        if (committedROI.IsEmpty)
        { return ROIDragMode.None; }

        var mouseLocationOnImage = Point.Round(BitmapDisplayPanel!.MapDisplayToImage(mouseLocationOnDisplay));

        Rectangle committedROIInDisplayCoordinates = BitmapDisplayPanel!.MapImageToDisplay(committedROI, BitmapDisplay.DisplayPixelAlign.TopLeft);

        Rectangle hitTestROI = committedROIInDisplayCoordinates;
        hitTestROI.Inflate(DragBorder, DragBorder);

        if (!hitTestROI.Contains(mouseLocationOnDisplay))
        { return ROIDragMode.None; }

        return mouseLocationOnDisplay.X < committedROIInDisplayCoordinates.Left + DragBorder
            ? mouseLocationOnDisplay.Y < committedROIInDisplayCoordinates.Top + DragBorder
                ? ROIDragMode.TopLeftCorner
                : mouseLocationOnDisplay.Y > committedROIInDisplayCoordinates.Bottom - DragBorder
                    ? ROIDragMode.BottomLeftCorner
                    : ROIDragMode.LeftEdge
            : mouseLocationOnDisplay.X > committedROIInDisplayCoordinates.Right - DragBorder
                ? mouseLocationOnDisplay.Y < committedROIInDisplayCoordinates.Top + DragBorder
                            ? ROIDragMode.TopRightCorner
                            : mouseLocationOnDisplay.Y > committedROIInDisplayCoordinates.Bottom - DragBorder
                                ? ROIDragMode.BottomRightCorner
                                : ROIDragMode.RightEdge
                : mouseLocationOnDisplay.Y < committedROIInDisplayCoordinates.Top + DragBorder
                            ? ROIDragMode.TopEdge
                            : mouseLocationOnDisplay.Y > committedROIInDisplayCoordinates.Bottom - DragBorder
                                        ? ROIDragMode.BottomEdge
                                        : ROIDragMode.WholeROI;
    }


    private void StartDraggingNewROI(Point imagePoint)
    {
        if (!CanCreateNew)
        { return; }

        originalDraggingROI = new Rectangle(imagePoint, Size.Empty);
        LiveDraggingROI = originalDraggingROI;
        draggingMode = ROIDragMode.BottomRightCorner;
        BitmapDisplayPanel!.SuppressDragging();
    }

    private void StartDraggingCommittedROI(ROIDragMode newDragMode)
    {
        if (!CanEditCommitted)
        { return; }

        originalDraggingROI = committedROI;
        LiveDraggingROI = originalDraggingROI;
        draggingMode = newDragMode;
        BitmapDisplayPanel!.SuppressDragging();
    }


    /// <summary>
    /// Handles the mouse move event to update the ROI dimensions.
    /// </summary>
    private void BitmapDisplayPanel_MouseMove(object? sender, MouseEventArgs e)
    {
        if (!CanWorkWithROI)
        { return; }

        if (draggingMode != ROIDragMode.None)
        {
            UpdateDraggingROI(mouseLocationOnDisplay: e.Location);
        }
        else if (!committedROI.IsEmpty)
        {
            ROIDragMode dragModeIfMouseClicked = DetermineDragModeFromMouseLocation(mouseLocationOnDisplay: e.Location);

            BitmapDisplayPanel!.Cursor = mouseCursors![dragModeIfMouseClicked];
        }
    }

    private void UpdateDraggingROI(Point mouseLocationOnDisplay)
    {
        if ((BitmapDisplayPanel == null) || mouseCursors == null)
        { return; }

        switch (draggingMode)
        {
            case ROIDragMode.WholeROI:
                UpdateWholeROIDragging(mouseLocationOnDisplay: mouseLocationOnDisplay);
                break;
            case ROIDragMode.TopLeftCorner:
                UpdateTopLeftCornerDragging(mouseLocationOnDisplay: mouseLocationOnDisplay);
                break;
            case ROIDragMode.TopRightCorner:
                UpdateTopRightCornerDragging(mouseLocationOnDisplay: mouseLocationOnDisplay);
                break;
            case ROIDragMode.BottomLeftCorner:
                UpdateBottomLeftCornerDragging(mouseLocationOnDisplay: mouseLocationOnDisplay);
                break;
            case ROIDragMode.BottomRightCorner:
                UpdateBottomRightCornerDragging(mouseLocationOnDisplay: mouseLocationOnDisplay);
                break;
            case ROIDragMode.TopEdge:
                UpdateTopEdgeDragging(mouseLocationOnDisplay: mouseLocationOnDisplay);
                break;
            case ROIDragMode.BottomEdge:
                UpdateBottomEdgeDragging(mouseLocationOnDisplay: mouseLocationOnDisplay);
                break;
            case ROIDragMode.LeftEdge:
                UpdateLeftEdgeDragging(mouseLocationOnDisplay: mouseLocationOnDisplay);
                break;
            case ROIDragMode.RightEdge:
                UpdateRightEdgeDragging(mouseLocationOnDisplay: mouseLocationOnDisplay);
                break;
            default:
                break;
        }

        BitmapDisplayPanel.Cursor = mouseCursors[draggingMode];
        BitmapDisplayPanel?.Invalidate();
    }


    private void UpdateRightEdgeDragging(Point mouseLocationOnDisplay)
    {
        var currentMouseLocationOverImage = Point.Round(BitmapDisplayPanel!.MapDisplayToImage(mouseLocationOnDisplay));

        bool isMouseLeftOfLeftEdgeOfOriginalROI = currentMouseLocationOverImage.X < originalDraggingROI.Left;

        LiveDraggingROI = isMouseLeftOfLeftEdgeOfOriginalROI
            ? Rectangle.FromLTRB(
                currentMouseLocationOverImage.X,
                originalDraggingROI.Top,
                originalDraggingROI.Left,
                originalDraggingROI.Bottom)
            : Rectangle.FromLTRB(
                originalDraggingROI.Left,
                originalDraggingROI.Top,
                currentMouseLocationOverImage.X,
                originalDraggingROI.Bottom);
    }

    private void UpdateLeftEdgeDragging(Point mouseLocationOnDisplay)
    {
        var currentMouseLocationOverImage = Point.Round(BitmapDisplayPanel!.MapDisplayToImage(mouseLocationOnDisplay));
        bool isMouseRightOfRightEdgeOfOriginalROI = currentMouseLocationOverImage.X > originalDraggingROI.Right;

        LiveDraggingROI = isMouseRightOfRightEdgeOfOriginalROI
            ? Rectangle.FromLTRB(
                originalDraggingROI.Right,
                originalDraggingROI.Top,
                currentMouseLocationOverImage.X,
                originalDraggingROI.Bottom)
            : Rectangle.FromLTRB(
                currentMouseLocationOverImage.X,
                originalDraggingROI.Top,
                originalDraggingROI.Right,
                originalDraggingROI.Bottom);
    }

    private void UpdateTopEdgeDragging(Point mouseLocationOnDisplay)
    {
        var currentMouseLocationOverImage = Point.Round(BitmapDisplayPanel!.MapDisplayToImage(mouseLocationOnDisplay));

        bool isMouseBelowBottomOfOriginalROI = currentMouseLocationOverImage.Y > originalDraggingROI.Bottom;

        LiveDraggingROI = isMouseBelowBottomOfOriginalROI
            ? Rectangle.FromLTRB(
                originalDraggingROI.Left,
                originalDraggingROI.Bottom,
                originalDraggingROI.Right,
                currentMouseLocationOverImage.Y)
            : Rectangle.FromLTRB(
                originalDraggingROI.Left,
                currentMouseLocationOverImage.Y,
                originalDraggingROI.Right,
                originalDraggingROI.Bottom);
    }

    private void UpdateBottomEdgeDragging(Point mouseLocationOnDisplay)
    {
        var currentMouseLocationOverImage = Point.Round(BitmapDisplayPanel!.MapDisplayToImage(mouseLocationOnDisplay));
        bool isMouseAboveTopOfOriginalROI = currentMouseLocationOverImage.Y < originalDraggingROI.Top;

        LiveDraggingROI = isMouseAboveTopOfOriginalROI
            ? Rectangle.FromLTRB(
                originalDraggingROI.Left,
                currentMouseLocationOverImage.Y,
                originalDraggingROI.Right,
                originalDraggingROI.Top)
            : Rectangle.FromLTRB(
                originalDraggingROI.Left,
                originalDraggingROI.Top,
                originalDraggingROI.Right,
                currentMouseLocationOverImage.Y);
    }

    private static void Swap(ref int left, ref int right) => (right, left) = (left, right);


    private void UpdateTopLeftCornerDragging(Point mouseLocationOnDisplay)
    {
        var currentMouseLocationOverImage = Point.Round(BitmapDisplayPanel!.MapDisplayToImage(mouseLocationOnDisplay));

        int left = currentMouseLocationOverImage.X;
        int right = originalDraggingROI.Right;
        int top = currentMouseLocationOverImage.Y;
        int bottom = originalDraggingROI.Bottom;

        if (left > right)
        { Swap(ref left, ref right); }
        if (top > bottom)
        { Swap(ref top, ref bottom); }

        LiveDraggingROI = Rectangle.FromLTRB(left, top, right, bottom);
    }


    private void UpdateBottomRightCornerDragging(Point mouseLocationOnDisplay)
    {
        var currentMouseLocationOverImage = Point.Round(BitmapDisplayPanel!.MapDisplayToImage(mouseLocationOnDisplay));

        int left = originalDraggingROI.Left;
        int right = currentMouseLocationOverImage.X;
        int top = originalDraggingROI.Top;
        int bottom = currentMouseLocationOverImage.Y;

        if (left > right)
        { Swap(ref left, ref right); }
        if (top > bottom)
        { Swap(ref top, ref bottom); }

        LiveDraggingROI = Rectangle.FromLTRB(left, top, right, bottom);
    }


    private void UpdateTopRightCornerDragging(Point mouseLocationOnDisplay)
    {
        var currentMouseLocationOverImage = Point.Round(BitmapDisplayPanel!.MapDisplayToImage(mouseLocationOnDisplay));
        int left = originalDraggingROI.Left;
        int right = currentMouseLocationOverImage.X;
        int top = currentMouseLocationOverImage.Y;
        int bottom = originalDraggingROI.Bottom;
        if (left > right)
        { Swap(ref left, ref right); }
        if (top > bottom)
        { Swap(ref top, ref bottom); }
        LiveDraggingROI = Rectangle.FromLTRB(left, top, right, bottom);
    }

    private void UpdateBottomLeftCornerDragging(Point mouseLocationOnDisplay)
    {
        var currentMouseLocationOverImage = Point.Round(BitmapDisplayPanel!.MapDisplayToImage(mouseLocationOnDisplay));
        int left = currentMouseLocationOverImage.X;
        int right = originalDraggingROI.Right;
        int top = originalDraggingROI.Top;
        int bottom = currentMouseLocationOverImage.Y;
        if (left > right)
        { Swap(ref left, ref right); }
        if (top > bottom)
        { Swap(ref top, ref bottom); }
        LiveDraggingROI = Rectangle.FromLTRB(left, top, right, bottom);
    }


    private void UpdateWholeROIDragging(Point mouseLocationOnDisplay)
    {
        var currentMouseLocationOverImage = Point.Round(BitmapDisplayPanel!.MapDisplayToImage(mouseLocationOnDisplay));
        var mouseDownLocationOverImage = Point.Round(BitmapDisplayPanel!.MapDisplayToImage(mouseDownLocationOnDisplay));

        int deltaX = currentMouseLocationOverImage.X - mouseDownLocationOverImage.X;
        int deltaY = currentMouseLocationOverImage.Y - mouseDownLocationOverImage.Y;

        LiveDraggingROI = Rectangle.FromLTRB(
            originalDraggingROI.Left + deltaX,
            originalDraggingROI.Top + deltaY,
            originalDraggingROI.Right + deltaX,
            originalDraggingROI.Bottom + deltaY);
    }


    /// <summary>
    /// Handles the mouse up event to finalize the ROI.
    /// </summary>
    private void BitmapDisplayPanel_MouseUp(object? sender, MouseEventArgs e)
    {
        if (!CanWorkWithROI)
        { return; }
        if (draggingMode == ROIDragMode.None)
        { return; }

        CommittedROI = LiveDraggingROI;
        LiveDraggingROI = Rectangle.Empty;
        draggingMode = ROIDragMode.None;
        BitmapDisplayPanel!.UnsuppressDragging();
        BitmapDisplayPanel!.Cursor = Cursors.Default;

        BitmapDisplayPanel?.Invalidate();
    }


    /// <summary>
    /// Draws the current ROI.
    /// </summary>
    private void BitmapDisplayPanel_OnPaintOver(object? sender, BitmapDisplay.PaintOverEventArgs e)
    {
        if (!CanWorkWithROI)
        {
            return;
        }

        if (!Visible)
        {
            return;
        }

        var bitmapDisplayPanel = e.Sender;

        if (!committedROI.IsEmpty && (DrawCommittedROIWhenFullSize || (committedROI.Size != imageSize)))
        {
            CommittedROIShape.ROI = committedROI;
            CommittedROIShape.Draw(bitmapDisplayPanel, e.Graphics);
        }

        if (draggingMode != ROIDragMode.None)
        {
            LiveDraggingROIShape.ROI = LiveDraggingROI;
            LiveDraggingROIShape.Draw(bitmapDisplayPanel, e.Graphics);
        }
    }
}
