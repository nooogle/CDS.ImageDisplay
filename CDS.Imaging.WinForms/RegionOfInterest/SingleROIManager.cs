using CDS.Imaging.Utils;
using System;
using System.Collections.Immutable;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace CDS.Imaging.RegionOfInterest
{
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

        private ROIWithGrapplesShape committedROIShape = new ROIWithGrapplesShape();
        private ROIWithGrapplesShape liveDraggingROIShape = new ROIWithGrapplesShape();

        private bool visible = true;
        private bool canEditCommitted = true;
        private bool canCreateNew = true;

        private BitmapDisplay.BitmapDisplayPanel? bitmapDisplayPanel;


        /// <summary>
        /// The border inside and outside the ROI that will test positive
        /// for a mouse hit test. (E.g. this allows the mouse to drag a corner
        /// or edge even if it's not exactly over the edge.)
        /// </summary>
        private int dragBorder = 10;


        /// <summary>
        /// How to draw the committed ROI.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public Overlays.DrawingSpec CommittedROIDrawingSpec => committedROIShape.Drawing;



        /// <summary>
        /// How to draw the live dragging ROI.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public Overlays.DrawingSpec LiveDraggingROIDrawingSpec => liveDraggingROIShape.Drawing;
        

        /// <summary>
        /// The border inside and outside the ROI that will test positive
        /// for a mouse hit test. (E.g. this allows the mouse to drag a corner
        /// or edge even if it's not exactly over the edge.)
        /// </summary>
        public int DragBorder
        {
            get => dragBorder;
            set => dragBorder = Math.Max(0, Math.Min(value, 100));
        }


        /// <summary>
        /// Fired when the committed ROI changes.
        /// </summary>
        public event OnCommittedROIChangedEvent? OnCommittedROIChanged;


        /// <summary>
        /// Fired when the ROI is being dragged.
        /// </summary>
        public event OnDraggingROIChangedEvent? OnDraggingROIChanged;


        /// <summary>
        /// The shape for the committed ROI.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public ROIWithGrapplesShape CommittedROIShape
        {
            get => committedROIShape;
        }


        /// <summary>
        /// The shape for the ROI that is being dragged.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public ROIWithGrapplesShape LiveDraggingROIShape
        {
            get => liveDraggingROIShape;
        }


        /// <summary>
        /// When true the ROI is drawn even when it is the same (full) size as the image.
        /// Normally the ROI is not drawn in this case.
        /// </summary>
        public bool DrawCommittedROIWhenFullSize { get; set; } = false;


        /// <summary>
        /// True if the ROI is currently being dragged.
        /// </summary>
        public bool IsDragging => (draggingMode != ROIDragMode.None);


        /// <summary>
        /// The bitmap display panel that the ROI is drawn on.
        /// </summary>
        [Category(categoryCDS)]
        public BitmapDisplay.BitmapDisplayPanel? BitmapDisplayPanel
        {
            get => bitmapDisplayPanel;

            set
            {
                if (bitmapDisplayPanel != value)
                {
                    if (bitmapDisplayPanel != null)
                    {
                        bitmapDisplayPanel.MouseDown -= BitmapDisplayPanel_MouseDown;
                        bitmapDisplayPanel.MouseMove -= BitmapDisplayPanel_MouseMove;
                        bitmapDisplayPanel.MouseUp -= BitmapDisplayPanel_MouseUp;
                        bitmapDisplayPanel.OnPaintOver -= BitmapDisplayPanel_OnPaintOver;
                        bitmapDisplayPanel.OnImageSizeChanged -= BitmapDisplayPanel_OnImageSizeChanged;
                        bitmapDisplayPanel.KeyPress -= BitmapDisplayPanel_KeyPress;
                        bitmapDisplayPanel.KeyDown -= BitmapDisplayPanel_KeyDown;
                    }

                    bitmapDisplayPanel = value;

                    if (bitmapDisplayPanel != null)
                    {
                        bitmapDisplayPanel.MouseDown += BitmapDisplayPanel_MouseDown;
                        bitmapDisplayPanel.MouseMove += BitmapDisplayPanel_MouseMove;
                        bitmapDisplayPanel.MouseUp += BitmapDisplayPanel_MouseUp;
                        bitmapDisplayPanel.OnPaintOver += BitmapDisplayPanel_OnPaintOver;
                        bitmapDisplayPanel.OnImageSizeChanged += BitmapDisplayPanel_OnImageSizeChanged;
                        bitmapDisplayPanel.KeyPress += BitmapDisplayPanel_KeyPress;
                        bitmapDisplayPanel.KeyDown += BitmapDisplayPanel_KeyDown;
                    }
                }
            }
        }

        private void BitmapDisplayPanel_KeyDown(object? sender, KeyEventArgs e)
        {
            if (draggingMode != ROIDragMode.None) { return; }
            if (!Visible) { return; }

            int change = 1;
            if (e.Control) { change = 10; }

            if (e.KeyCode == Keys.Left)
            {
                if (e.Shift)
                {
                    CommittedROI = new Rectangle(CommittedROI.Left, CommittedROI.Top, CommittedROI.Width - change, CommittedROI.Height);
                }
                else
                {
                    CommittedROI = new Rectangle(CommittedROI.Left - change, CommittedROI.Top, CommittedROI.Width, CommittedROI.Height);
                }
            }
            else if (e.KeyCode == Keys.Right)
            {
                if (e.Shift)
                {
                    CommittedROI = new Rectangle(CommittedROI.Left, CommittedROI.Top, CommittedROI.Width + change, CommittedROI.Height);
                }
                else
                {
                    CommittedROI = new Rectangle(CommittedROI.Left + change, CommittedROI.Top, CommittedROI.Width, CommittedROI.Height);
                }
            }
            else if (e.KeyCode == Keys.Up)
            {
                if (e.Shift)
                {
                    CommittedROI = new Rectangle(CommittedROI.Left, CommittedROI.Top, CommittedROI.Width, CommittedROI.Height - change);
                }
                else
                {
                    CommittedROI = new Rectangle(CommittedROI.Left, CommittedROI.Top - change, CommittedROI.Width, CommittedROI.Height);
                }
            }
            else if (e.KeyCode == Keys.Down)
            {
                if (e.Shift)
                {
                    CommittedROI = new Rectangle(CommittedROI.Left, CommittedROI.Top, CommittedROI.Width, CommittedROI.Height + change);
                }
                else
                {
                    CommittedROI = new Rectangle(CommittedROI.Left, CommittedROI.Top + change, CommittedROI.Width, CommittedROI.Height);
                }
            }
        }

        private void BitmapDisplayPanel_KeyPress(object? sender, KeyPressEventArgs e)
        {
            if (draggingMode != ROIDragMode.None) { return; }
            if (!Visible) { return; }

        }


        /// <summary>
        /// Handle a change to the image size
        /// </summary>
        private void BitmapDisplayPanel_OnImageSizeChanged(BitmapDisplay.BitmapDisplayPanel sender, Size oldSize, Size newSize)
        {
            imageSize = newSize;
        }


        /// <summary>
        /// Controls whether the ROI is visible.
        /// </summary>
        public bool Visible
        {
            get => visible;

            set
            {
                if (visible != value)
                {
                    visible = value;
                    bitmapDisplayPanel?.Invalidate();
                }
            }
        }


        /// <summary>
        /// Controls whether the ROI can changed. 
        /// </summary>
        public bool CanEditCommitted
        {
            get => canEditCommitted;

            set
            {
                if (canEditCommitted != value)
                {
                    canEditCommitted = value;
                    bitmapDisplayPanel?.Invalidate();
                }
            }
        }


        /// <summary>
        /// Controls whether a new ROI can be created.
        /// </summary>
        public bool CanCreateNew
        {
            get => canCreateNew;

            set
            {
                if (canCreateNew != value)
                {
                    canCreateNew = value;
                    bitmapDisplayPanel?.Invalidate();
                }
            }
        }



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

            committedROIShape.Drawing.Lines.Color = Color.FromArgb(128, Color.Green);
            committedROIShape.Drawing.Lines.Width = 2;
            committedROIShape.Drawing.Fill.Color = Color.Transparent;

            liveDraggingROIShape.Drawing.Lines.Color = Color.FromArgb(128, Color.Orange);
            liveDraggingROIShape.Drawing.Lines.Width = 2;
            liveDraggingROIShape.Drawing.Fill.Color = Color.Transparent;
        }


        /// <summary>
        /// Gets/sets the current (committed) ROI. An empty rectangle indicates no ROI is set.
        /// </summary>
        public Rectangle CommittedROI
        {
            get => committedROI;

            set
            {
                var originalCommittedROI = committedROI;

                if (!imageSize.HasValue)
                {
                    committedROI = Rectangle.Empty;
                }
                else
                {
                    var newCommittedROI = value;

                    newCommittedROI = value;
                    newCommittedROI.Intersect(new Rectangle(Point.Empty, imageSize!.Value));
                    if (newCommittedROI.Size.IsEmpty) { newCommittedROI = Rectangle.Empty; }

                    if (newCommittedROI != committedROI)
                    {
                        committedROI = newCommittedROI;
                        OnCommittedROIChanged?.Invoke(this, committedROI);
                        bitmapDisplayPanel?.Invalidate();
                    }
                }
            }
        }


        /// <summary>
        /// Gets the ROI that is currently being dragged, or returns an empty rectangle if no ROI is being dragged.
        /// </summary>
        public Rectangle LiveDraggingROI
        {
            get => liveDraggingROI;

            private set
            {
                if (liveDraggingROI != value)
                {
                    liveDraggingROI = value;
                    OnDraggingROIChanged?.Invoke(this, liveDraggingROI);
                    bitmapDisplayPanel?.Invalidate();
                }
            }
        }


        /// <summary>
        /// Creates a dictionary of cursors for each dragging mode.
        /// </summary>
        private static ImmutableDictionary<ROIDragMode, Cursor> CreateMouseCursorsDict()
        {
            var builder = ImmutableDictionary.CreateBuilder<ROIDragMode, Cursor>();

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
        /// Gets or sets the current ROI.
        /// </summary>
        public void SetROI(Rectangle roi)
        {
            if (!imageSize.HasValue) { return; }

            var newCommittedROI = roi;
            newCommittedROI.Intersect(new Rectangle(Point.Empty, imageSize.Value));
            CommittedROI = newCommittedROI;

            bitmapDisplayPanel?.Invalidate();
        }


        /// <summary>
        /// True if there's an image that the ROI can be applied to and that 
        /// </summary>
        private bool CanWorkWithROI => (bitmapDisplayPanel != null) && imageSize.HasValue;



        /// <summary>
        /// True if the spacebar is pressed.
        /// </summary>
        private bool IsSpacebarPressed()
        {
            return (Win32.GetKeyState(Win32.VK_SPACE) & 0x8000) != 0;
        }


        /// <summary>
        /// Handles the mouse down event to begin defining an ROI.
        /// </summary>
        private void BitmapDisplayPanel_MouseDown(object? sender, MouseEventArgs e)
        {
            if (!CanWorkWithROI) { return; }

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

            var imagePoint = Point.Round(bitmapDisplayPanel!.MapDisplayToImage(e.Location));
            var newDragMode = DetermineDragModeFromMouseLocation(mouseLocationOnDisplay: e.Location);

            if(newDragMode != ROIDragMode.None)
            {
                StartDraggingCommittedROI(newDragMode);
            }
            else
            {
                StartDraggingNewROI(imagePoint);
            }

            bitmapDisplayPanel?.Invalidate();
        }


        private ROIDragMode DetermineDragModeFromMouseLocation(Point mouseLocationOnDisplay)
        {
            if (!canEditCommitted) { return ROIDragMode.None; }
            if (committedROI.IsEmpty) { return ROIDragMode.None; }

            var mouseLocationOnImage = Point.Round(bitmapDisplayPanel!.MapDisplayToImage(mouseLocationOnDisplay));

            var committedROIInDisplayCoordinates = bitmapDisplayPanel!.MapImageToDisplay(committedROI, BitmapDisplay.DisplayPixelAlign.TopLeft);

            var hitTestROI = committedROIInDisplayCoordinates;
            hitTestROI.Inflate(DragBorder, DragBorder);

            if (!hitTestROI.Contains(mouseLocationOnDisplay)) { return ROIDragMode.None; }

            if (mouseLocationOnDisplay.X < committedROIInDisplayCoordinates.Left + DragBorder)
            {
                if (mouseLocationOnDisplay.Y < committedROIInDisplayCoordinates.Top + DragBorder)
                {
                    return ROIDragMode.TopLeftCorner;
                }
                else if (mouseLocationOnDisplay.Y > committedROIInDisplayCoordinates.Bottom - DragBorder)
                {
                    return ROIDragMode.BottomLeftCorner;
                }
                else
                {
                    return ROIDragMode.LeftEdge;
                }
            }
            else if (mouseLocationOnDisplay.X > committedROIInDisplayCoordinates.Right - DragBorder)
            {
                if (mouseLocationOnDisplay.Y < committedROIInDisplayCoordinates.Top + DragBorder)
                {
                    return ROIDragMode.TopRightCorner;
                }
                else if (mouseLocationOnDisplay.Y > committedROIInDisplayCoordinates.Bottom - DragBorder)
                {
                    return ROIDragMode.BottomRightCorner;
                }
                else
                {
                    return ROIDragMode.RightEdge;
                }
            }
            else if (mouseLocationOnDisplay.Y < committedROIInDisplayCoordinates.Top + DragBorder)
            {
                return ROIDragMode.TopEdge;
            }
            else if (mouseLocationOnDisplay.Y > committedROIInDisplayCoordinates.Bottom - DragBorder)
            {
                return ROIDragMode.BottomEdge;
            }
            else
            {
                return ROIDragMode.WholeROI;
            }
        }


        private void StartDraggingNewROI(Point imagePoint)
        {
            if (!canCreateNew) { return; }

            originalDraggingROI = new Rectangle(imagePoint, Size.Empty);
            LiveDraggingROI = originalDraggingROI;
            draggingMode = ROIDragMode.BottomRightCorner;
            bitmapDisplayPanel!.SuppressDragging();
        }

        private void StartDraggingCommittedROI(ROIDragMode newDragMode)
        {
            if (!canEditCommitted) { return; }

            originalDraggingROI = committedROI;
            LiveDraggingROI = originalDraggingROI;
            draggingMode = newDragMode;
            bitmapDisplayPanel!.SuppressDragging();
        }


        /// <summary>
        /// Handles the mouse move event to update the ROI dimensions.
        /// </summary>
        private void BitmapDisplayPanel_MouseMove(object? sender, MouseEventArgs e)
        {
            if (!CanWorkWithROI) { return; }

            if (draggingMode != ROIDragMode.None)
            {
                UpdateDraggingROI(mouseLocationOnDisplay: e.Location);
            }
            else if (!committedROI.IsEmpty)
            {
                var dragModeIfMouseClicked = DetermineDragModeFromMouseLocation(mouseLocationOnDisplay: e.Location);

                bitmapDisplayPanel!.Cursor = mouseCursors![dragModeIfMouseClicked];
            }
        }

        private void UpdateDraggingROI(Point mouseLocationOnDisplay)
        {
            if ((bitmapDisplayPanel == null) || mouseCursors == null) { return; }

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

            bitmapDisplayPanel.Cursor = mouseCursors[draggingMode];
            bitmapDisplayPanel?.Invalidate();
        }


        private void UpdateRightEdgeDragging(Point mouseLocationOnDisplay)
        {
            var currentMouseLocationOverImage = Point.Round(bitmapDisplayPanel!.MapDisplayToImage(mouseLocationOnDisplay));

            var isMouseLeftOfLeftEdgeOfOriginalROI = currentMouseLocationOverImage.X < originalDraggingROI.Left;

            if (isMouseLeftOfLeftEdgeOfOriginalROI)
            {
                LiveDraggingROI = Rectangle.FromLTRB(
                    currentMouseLocationOverImage.X,
                    originalDraggingROI.Top,
                    originalDraggingROI.Left,
                    originalDraggingROI.Bottom);
            }
            else
            {
                LiveDraggingROI = Rectangle.FromLTRB(
                    originalDraggingROI.Left,
                    originalDraggingROI.Top,
                    currentMouseLocationOverImage.X,
                    originalDraggingROI.Bottom);
            }
        }

        private void UpdateLeftEdgeDragging(Point mouseLocationOnDisplay)
        {
            var currentMouseLocationOverImage = Point.Round(bitmapDisplayPanel!.MapDisplayToImage(mouseLocationOnDisplay));
            var isMouseRightOfRightEdgeOfOriginalROI = currentMouseLocationOverImage.X > originalDraggingROI.Right;

            if (isMouseRightOfRightEdgeOfOriginalROI)
            {
                LiveDraggingROI = Rectangle.FromLTRB(
                    originalDraggingROI.Right,
                    originalDraggingROI.Top,
                    currentMouseLocationOverImage.X,
                    originalDraggingROI.Bottom);
            }
            else
            {
                LiveDraggingROI = Rectangle.FromLTRB(
                    currentMouseLocationOverImage.X,
                    originalDraggingROI.Top,
                    originalDraggingROI.Right,
                    originalDraggingROI.Bottom);
            }
        }

        private void UpdateTopEdgeDragging(Point mouseLocationOnDisplay)
        {
            var currentMouseLocationOverImage = Point.Round(bitmapDisplayPanel!.MapDisplayToImage(mouseLocationOnDisplay));

            var isMouseBelowBottomOfOriginalROI = currentMouseLocationOverImage.Y > originalDraggingROI.Bottom;

            if (isMouseBelowBottomOfOriginalROI)
            {
                LiveDraggingROI = Rectangle.FromLTRB(
                    originalDraggingROI.Left,
                    originalDraggingROI.Bottom,
                    originalDraggingROI.Right,
                    currentMouseLocationOverImage.Y);
            }
            else
            {
                LiveDraggingROI = Rectangle.FromLTRB(
                    originalDraggingROI.Left,
                    currentMouseLocationOverImage.Y,
                    originalDraggingROI.Right,
                    originalDraggingROI.Bottom);
            }
        }

        private void UpdateBottomEdgeDragging(Point mouseLocationOnDisplay)
        {
            var currentMouseLocationOverImage = Point.Round(bitmapDisplayPanel!.MapDisplayToImage(mouseLocationOnDisplay));
            var isMouseAboveTopOfOriginalROI = currentMouseLocationOverImage.Y < originalDraggingROI.Top;

            if (isMouseAboveTopOfOriginalROI)
            {
                LiveDraggingROI = Rectangle.FromLTRB(
                    originalDraggingROI.Left,
                    currentMouseLocationOverImage.Y,
                    originalDraggingROI.Right,
                    originalDraggingROI.Top);
            }
            else
            {
                LiveDraggingROI = Rectangle.FromLTRB(
                    originalDraggingROI.Left,
                    originalDraggingROI.Top,
                    originalDraggingROI.Right,
                    currentMouseLocationOverImage.Y);
            }
        }

        private void Swap(ref int left, ref int right)
        {
            int temp = left;
            left = right;
            right = temp;
        }


        private void UpdateTopLeftCornerDragging(Point mouseLocationOnDisplay)
        {
            var currentMouseLocationOverImage = Point.Round(bitmapDisplayPanel!.MapDisplayToImage(mouseLocationOnDisplay));

            var left = currentMouseLocationOverImage.X;
            var right = originalDraggingROI.Right;
            var top = currentMouseLocationOverImage.Y;
            var bottom = originalDraggingROI.Bottom;

            if (left > right) { Swap(ref left, ref right); }
            if (top > bottom) { Swap(ref top, ref bottom); }

            LiveDraggingROI = Rectangle.FromLTRB(left, top, right, bottom);
        }


        private void UpdateBottomRightCornerDragging(Point mouseLocationOnDisplay)
        {
            var currentMouseLocationOverImage = Point.Round(bitmapDisplayPanel!.MapDisplayToImage(mouseLocationOnDisplay));

            var left = originalDraggingROI.Left;
            var right = currentMouseLocationOverImage.X;
            var top = originalDraggingROI.Top;
            var bottom = currentMouseLocationOverImage.Y;

            if (left > right) { Swap(ref left, ref right); }
            if (top > bottom) { Swap(ref top, ref bottom); }

            LiveDraggingROI = Rectangle.FromLTRB(left, top, right, bottom);
        }


        private void UpdateTopRightCornerDragging(Point mouseLocationOnDisplay)
        {
            var currentMouseLocationOverImage = Point.Round(bitmapDisplayPanel!.MapDisplayToImage(mouseLocationOnDisplay));
            var left = originalDraggingROI.Left;
            var right = currentMouseLocationOverImage.X;
            var top = currentMouseLocationOverImage.Y;
            var bottom = originalDraggingROI.Bottom;
            if (left > right) { Swap(ref left, ref right); }
            if (top > bottom) { Swap(ref top, ref bottom); }
            LiveDraggingROI = Rectangle.FromLTRB(left, top, right, bottom);
        }

        private void UpdateBottomLeftCornerDragging(Point mouseLocationOnDisplay)
        {
            var currentMouseLocationOverImage = Point.Round(bitmapDisplayPanel!.MapDisplayToImage(mouseLocationOnDisplay));
            var left = currentMouseLocationOverImage.X;
            var right = originalDraggingROI.Right;
            var top = originalDraggingROI.Top;
            var bottom = currentMouseLocationOverImage.Y;
            if (left > right) { Swap(ref left, ref right); }
            if (top > bottom) { Swap(ref top, ref bottom); }
            LiveDraggingROI = Rectangle.FromLTRB(left, top, right, bottom);
        }


        private void UpdateWholeROIDragging(Point mouseLocationOnDisplay)
        {
            var currentMouseLocationOverImage = Point.Round(bitmapDisplayPanel!.MapDisplayToImage(mouseLocationOnDisplay));
            var mouseDownLocationOverImage = Point.Round(bitmapDisplayPanel!.MapDisplayToImage(mouseDownLocationOnDisplay));

            var deltaX = currentMouseLocationOverImage.X - mouseDownLocationOverImage.X;
            var deltaY = currentMouseLocationOverImage.Y - mouseDownLocationOverImage.Y;

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
            if (!CanWorkWithROI) { return; }
            if (draggingMode == ROIDragMode.None) { return; }

            CommittedROI = LiveDraggingROI;
            LiveDraggingROI = Rectangle.Empty;
            draggingMode = ROIDragMode.None;
            bitmapDisplayPanel!.UnsuppressDragging();

            bitmapDisplayPanel?.Invalidate();
        }


        /// <summary>
        /// Draws the current ROI.
        /// </summary>
        private void BitmapDisplayPanel_OnPaintOver(BitmapDisplay.BitmapDisplayPanel sender, Graphics graphics)
        {
            if (!CanWorkWithROI) { return; }
            if (!visible) { return; }

            if (!committedROI.IsEmpty && (DrawCommittedROIWhenFullSize || (committedROI.Size != imageSize)))
            {
                committedROIShape.ROI = committedROI;
                committedROIShape.Draw(sender, graphics);
            }

            if (draggingMode != ROIDragMode.None)
            {
                liveDraggingROIShape.ROI = LiveDraggingROI;
                liveDraggingROIShape.Draw(sender, graphics);
            }
        }
    }
}

