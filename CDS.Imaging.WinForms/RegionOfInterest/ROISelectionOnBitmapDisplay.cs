using System.Collections.Immutable;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace CDS.Imaging.WinForms.RegionOfInterest
{
    /// <summary>
    /// Manages a region of interest (ROI) on a <see cref="BitmapDisplay.BitmapDisplayPanel"/>
    /// </summary>
    public partial class ROISelectionOnBitmapDisplay : Component
    {
        private const string categoryCDS = "CDS";

        private ImmutableDictionary<ROIDragMode, Cursor>? mouseCursors;
        private ROIDragMode draggingMode = ROIDragMode.None;
        private Size? imageSize;
        private Rectangle committedROI;
        private Rectangle originalDraggingROI;
        private Rectangle liveDraggingROI;
        private Point mouseDownLocationOnDisplay;

        private RectangleRenderer committedROIRenderer = new RectangleRenderer();
        private RectangleRenderer liveDraggingROIRenderer = new RectangleRenderer();

        private bool visible = true;
        private bool canEditCommitted = false;
        private bool canCreateNew = false;

        private BitmapDisplay.BitmapDisplayPanel? bitmapDisplayPanel;


        /// <summary>
        /// Fired when the committed ROI changes.
        /// </summary>
        public event OnCommittedROIChangedEvent? OnCommittedROIChanged;


        /// <summary>
        /// Fired when the ROI is being dragged.
        /// </summary>
        public event OnDraggingROIChangedEvent? OnDraggingROIChanged;


        /// <summary>
        /// The renderer for the committed ROI.
        /// </summary>
        public RectangleRenderer CommittedROIRenderer => committedROIRenderer;


        /// <summary>
        /// The renderer for the ROI that is being dragged.
        /// </summary>
        public RectangleRenderer LiveDraggingROIRenderer => liveDraggingROIRenderer;


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
                    }

                    bitmapDisplayPanel = value;

                    if (bitmapDisplayPanel != null)
                    {
                        bitmapDisplayPanel.MouseDown += BitmapDisplayPanel_MouseDown;
                        bitmapDisplayPanel.MouseMove += BitmapDisplayPanel_MouseMove;
                        bitmapDisplayPanel.MouseUp += BitmapDisplayPanel_MouseUp;
                        bitmapDisplayPanel.OnPaintOver += BitmapDisplayPanel_OnPaintOver;
                        bitmapDisplayPanel.OnImageSizeChanged += BitmapDisplayPanel_OnImageSizeChanged;
                    }
                }
            }
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
        /// Initializes a new instance of the <see cref="ROISelectionOnBitmapDisplay"/> class.
        /// </summary>
        public ROISelectionOnBitmapDisplay()
        {
            InitializeComponent();
            CompleteInitialisation();
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="ROISelectionOnBitmapDisplay"/> class.
        /// </summary>
        public ROISelectionOnBitmapDisplay(IContainer container)
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
            components.Add(committedROIRenderer);
            components.Add(liveDraggingROIRenderer);

            committedROIRenderer.GrapplesMode = RectangleRenderer.GrapplesRenderingMode.ShowEnabled;
            committedROIRenderer.OutlinePen.Color = Color.FromArgb(128, Color.Red);
            committedROIRenderer.OutlinePen.Width = 2;

            liveDraggingROIRenderer.GrapplesMode = RectangleRenderer.GrapplesRenderingMode.ShowEnabled;
            liveDraggingROIRenderer.OutlinePen.Color = Color.FromArgb(128, Color.Red);
            liveDraggingROIRenderer.OutlinePen.Width = 4;
            liveDraggingROIRenderer.FillBrush.Color = Color.FromArgb(32, Color.Red);
            liveDraggingROIRenderer.EnabledGrapplePen.Color = Color.FromArgb(128, Color.Cyan);
            liveDraggingROIRenderer.EnabledGrapplePen.Width = 2;
            liveDraggingROIRenderer.EnabledGrappleBrush.Color = Color.FromArgb(128, Color.Navy);

            liveDraggingROIRenderer.DisabledGrapplePen.Width = 2;

            mouseCursors = CreateMouseCursorsDict();
        }


        ///// <summary>
        ///// Initializes a new instance of the <see cref="ROIManager"/> class.
        ///// </summary>
        //public ROIManager(
        //    Func<Point, Point> mapImagePointToDisplayPoint,
        //    Func<Rectangle, Rectangle> mapImageRectangleToDisplayRectangle,
        //    Func<Point, Point> mapDisplayPointToImagePoint,
        //    Action invalidateDisplay,
        //    Action<Cursor> setMouseCursor,
        //    Action onCommittedROIChange,
        //    Action onDraggingROIChange)
        //{
        //    imageSize = null;

        //    this.mapImagePointToDisplayPoint = mapImagePointToDisplayPoint;
        //    this.mapImageRectangleToDisplayRectangle = mapImageRectangleToDisplayRectangle;
        //    this.mapDisplayPointToImagePoint = mapDisplayPointToImagePoint;
        //    this.invalidateDisplay = invalidateDisplay;

        //    this.setMouseCursor = setMouseCursor;


        //    this.onCommittedROIChange = onCommittedROIChange;
        //    this.onDraggingROIChange = onDraggingROIChange;
        //}


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
        /// True if there's an image that the ROI can be applied to.
        /// </summary>
        private bool CanWorkWithROI => (bitmapDisplayPanel != null) && imageSize.HasValue;


        /// <summary>
        /// Handles the mouse down event to begin defining an ROI.
        /// </summary>
        private void BitmapDisplayPanel_MouseDown(object? sender, MouseEventArgs e)
        {
            if (!CanWorkWithROI) { return; }

            if (e.Button == MouseButtons.Left)
            {
                OnLeftMouseButtonDown(e);
            }
        }


        /// <summary>
        /// Handles the mouse down event to begin defining an ROI.
        /// </summary>
        /// <param name="e"></param>
        private void OnLeftMouseButtonDown(MouseEventArgs e)
        {
            mouseDownLocationOnDisplay = e.Location;

            var imagePoint = bitmapDisplayPanel!.MapDisplayPointToImagePoint(e.Location);
            var newDragMode = DetermineDragModeFromMouseLocation(mouseLocationOnDisplay: e.Location);

            var isMouseDownOverCommittedROI = !committedROI.IsEmpty && committedROI.Contains(imagePoint);

            if (isMouseDownOverCommittedROI)
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
            if (committedROI.IsEmpty) { return ROIDragMode.None; }

            var mouseLocationOnImage = bitmapDisplayPanel!.MapDisplayPointToImagePoint(mouseLocationOnDisplay);

            if (!committedROI.Contains(mouseLocationOnImage)) { return ROIDragMode.None; }

            var committedROIInDisplayCoordinates = bitmapDisplayPanel!.MapImageRectangleToDisplayRectangle(committedROI);

            if (mouseLocationOnDisplay.X < committedROIInDisplayCoordinates.Left + 10)
            {
                if (mouseLocationOnDisplay.Y < committedROIInDisplayCoordinates.Top + 10)
                {
                    return ROIDragMode.TopLeftCorner;
                }
                else if (mouseLocationOnDisplay.Y > committedROIInDisplayCoordinates.Bottom - 10)
                {
                    return ROIDragMode.BottomLeftCorner;
                }
                else
                {
                    return ROIDragMode.LeftEdge;
                }
            }
            else if (mouseLocationOnDisplay.X > committedROIInDisplayCoordinates.Right - 10)
            {
                if (mouseLocationOnDisplay.Y < committedROIInDisplayCoordinates.Top + 10)
                {
                    return ROIDragMode.TopRightCorner;
                }
                else if (mouseLocationOnDisplay.Y > committedROIInDisplayCoordinates.Bottom - 10)
                {
                    return ROIDragMode.BottomRightCorner;
                }
                else
                {
                    return ROIDragMode.RightEdge;
                }
            }
            else if (mouseLocationOnDisplay.Y < committedROIInDisplayCoordinates.Top + 10)
            {
                return ROIDragMode.TopEdge;
            }
            else if (mouseLocationOnDisplay.Y > committedROIInDisplayCoordinates.Bottom - 10)
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
            if((bitmapDisplayPanel == null) || mouseCursors == null) { return; }

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
            var currentMouseLocationOverImage = bitmapDisplayPanel!.MapDisplayPointToImagePoint(mouseLocationOnDisplay);

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
            var currentMouseLocationOverImage = bitmapDisplayPanel!.MapDisplayPointToImagePoint(mouseLocationOnDisplay);
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
            var currentMouseLocationOverImage = bitmapDisplayPanel!.MapDisplayPointToImagePoint(mouseLocationOnDisplay);

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
            var currentMouseLocationOverImage = bitmapDisplayPanel!.MapDisplayPointToImagePoint(mouseLocationOnDisplay);
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
            var currentMouseLocationOverImage = bitmapDisplayPanel!.MapDisplayPointToImagePoint(mouseLocationOnDisplay);

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
            var currentMouseLocationOverImage = bitmapDisplayPanel!.MapDisplayPointToImagePoint(mouseLocationOnDisplay);

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
            var currentMouseLocationOverImage = bitmapDisplayPanel!.MapDisplayPointToImagePoint(mouseLocationOnDisplay);
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
            var currentMouseLocationOverImage = bitmapDisplayPanel!.MapDisplayPointToImagePoint(mouseLocationOnDisplay);
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
            var currentMouseLocationOverImage = bitmapDisplayPanel!.MapDisplayPointToImagePoint(mouseLocationOnDisplay);
            var mouseDownLocationOverImage = bitmapDisplayPanel!.MapDisplayPointToImagePoint(mouseDownLocationOnDisplay);

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
                var displayRect = bitmapDisplayPanel!.MapImageRectangleToDisplayRectangle(committedROI);
                committedROIRenderer.Draw(graphics, displayRect);
            }

            if (draggingMode != ROIDragMode.None)
            {
                var displayRect = bitmapDisplayPanel!.MapImageRectangleToDisplayRectangle(LiveDraggingROI);
                liveDraggingROIRenderer.Draw(graphics, displayRect);
            }
        }
    }
}
