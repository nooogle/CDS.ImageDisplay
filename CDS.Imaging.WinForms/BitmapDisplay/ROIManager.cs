using System;
using System.Collections.Immutable;
using System.Drawing;
using System.Windows.Forms;

namespace CDS.Imaging.WinForms.BitmapDisplay
{
    /// <summary>
    /// Manages a region of interest (ROI) on an image.
    /// </summary>
    public class ROIManager : IDisposable
    {
        private bool isDisposed = false;
        private readonly Func<Point, Point> mapImagePointToDisplayPoint;
        private readonly Func<Rectangle, Rectangle> mapImageRectangleToDisplayRectangle;
        private readonly Func<Point, Point> mapDisplayPointToImagePoint;
        private readonly Action invalidateDisplay;
        private readonly Action<Cursor> setMouseCursor;
        private readonly Action onCommittedROIChange;
        private readonly Action onDraggingROIChange;

        private readonly ImmutableDictionary<DraggingMode, Cursor> mouseCursors;

        private DraggingMode draggingMode = DraggingMode.None;
        private Size? imageSize;
        private Rectangle committedROI;
        private Rectangle originalDraggingROI;
        private Rectangle liveDraggingROI;
        private Point mouseDownLocationOnDisplay;

        private RectangleRenderer committedROIRenderer;
        private RectangleRenderer liveDraggingROIRenderer;

        private bool visible = true;
        private bool canEditCommitted = false;
        private bool canCreateNew = false;


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
                    invalidateDisplay();
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
                    invalidateDisplay();
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
                    invalidateDisplay();
                }
            }
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="ROIManager"/> class.
        /// </summary>
        public ROIManager(
            Func<Point, Point> mapImagePointToDisplayPoint,
            Func<Rectangle, Rectangle> mapImageRectangleToDisplayRectangle,
            Func<Point, Point> mapDisplayPointToImagePoint,
            Action invalidateDisplay,
            Action<Cursor> setMouseCursor,
            Action onCommittedROIChange,
            Action onDraggingROIChange)
        {
            this.imageSize = null;

            this.mapImagePointToDisplayPoint = mapImagePointToDisplayPoint;
            this.mapImageRectangleToDisplayRectangle = mapImageRectangleToDisplayRectangle;
            this.mapDisplayPointToImagePoint = mapDisplayPointToImagePoint;
            this.invalidateDisplay = invalidateDisplay;

            this.mouseCursors = CreateMouseCursorsDict();
            this.setMouseCursor = setMouseCursor;

            committedROIRenderer = new RectangleRenderer();
            committedROIRenderer.GrapplesMode = RectangleRenderer.GrapplesRenderingMode.ShowEnabled;
            committedROIRenderer.OutlinePen.Color = Color.FromArgb(128, Color.Red);
            committedROIRenderer.OutlinePen.Width = 2;

            liveDraggingROIRenderer = new RectangleRenderer();
            liveDraggingROIRenderer.GrapplesMode = RectangleRenderer.GrapplesRenderingMode.ShowEnabled;
            liveDraggingROIRenderer.OutlinePen.Color = Color.FromArgb(128, Color.Red);
            liveDraggingROIRenderer.OutlinePen.Width = 4;
            liveDraggingROIRenderer.FillBrush.Color = Color.FromArgb(32, Color.Red);
            liveDraggingROIRenderer.EnabledGrapplePen.Color = Color.FromArgb(128, Color.Cyan);
            liveDraggingROIRenderer.EnabledGrapplePen.Width = 2;
            liveDraggingROIRenderer.EnabledGrappleBrush.Color = Color.FromArgb(128, Color.Navy);

            liveDraggingROIRenderer.DisabledGrapplePen.Width = 2;

            this.onCommittedROIChange = onCommittedROIChange;
            this.onDraggingROIChange = onDraggingROIChange;
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
                    committedROI = value;
                    committedROI.Intersect(new Rectangle(Point.Empty, imageSize!.Value));
                    if (committedROI.Size.IsEmpty) { committedROI = Rectangle.Empty; }


                    if (committedROI != originalCommittedROI)
                    {
                        onCommittedROIChange();
                        invalidateDisplay();
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
                    onDraggingROIChange();
                    invalidateDisplay();
                }
            }
        }


        /// <summary>
        /// Disposes of the resources used by the ROI manager.
        /// </summary>
        public void Dispose()
        {
            if (isDisposed) { return; }
         
            committedROIRenderer.Dispose();
            liveDraggingROIRenderer.Dispose();
            
            isDisposed = true;
        }


        /// <summary>
        /// Creates a dictionary of cursors for each dragging mode.
        /// </summary>
        private static ImmutableDictionary<DraggingMode, Cursor> CreateMouseCursorsDict()
        {
            var builder = ImmutableDictionary.CreateBuilder<DraggingMode, Cursor>();

            builder.Add(DraggingMode.None, Cursors.Default);
            builder.Add(DraggingMode.WholeROI, Cursors.Hand);
            builder.Add(DraggingMode.TopLeftCorner, Cursors.SizeNWSE);
            builder.Add(DraggingMode.TopRightCorner, Cursors.SizeNESW);
            builder.Add(DraggingMode.BottomLeftCorner, Cursors.SizeNESW);
            builder.Add(DraggingMode.BottomRightCorner, Cursors.SizeNWSE);
            builder.Add(DraggingMode.TopEdge, Cursors.SizeNS);
            builder.Add(DraggingMode.BottomEdge, Cursors.SizeNS);
            builder.Add(DraggingMode.LeftEdge, Cursors.SizeWE);
            builder.Add(DraggingMode.RightEdge, Cursors.SizeWE);

            return builder.ToImmutable();
        }


        /// <summary>
        /// Gets or sets the current ROI.
        /// </summary>
        public void SetROI(Rectangle roi)
        {
            if(!imageSize.HasValue) { return; }

            var newCommittedROI = roi;
            newCommittedROI.Intersect(new Rectangle(Point.Empty, imageSize.Value));
            CommittedROI = newCommittedROI;

            invalidateDisplay();
        }


        /// <summary>
        /// Sets the current image size (or uses null to indiate an image is not loaded).
        /// </summary>
        /// <param name="imageSize"></param>
        public void SetImageSize(Size? imageSize)
        {
            this.imageSize = imageSize;
        }


        /// <summary>
        /// True if there's an image that the ROI can be applied to.
        /// </summary>
        private bool CanWorkWithROI => imageSize.HasValue;



        /// <summary>
        /// Handles the mouse down event to begin defining an ROI.
        /// </summary>
        public void OnMouseDown(MouseEventArgs e)
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

            var imagePoint = mapDisplayPointToImagePoint(e.Location);
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

            invalidateDisplay();
        }


        private DraggingMode DetermineDragModeFromMouseLocation(Point mouseLocationOnDisplay)
        {
            if (committedROI.IsEmpty) { return DraggingMode.None; }

            var mouseLocationOnImage = mapDisplayPointToImagePoint(mouseLocationOnDisplay);

            if (!committedROI.Contains(mouseLocationOnImage)) { return DraggingMode.None; }

            var committedROIInDisplayCoordinates = mapImageRectangleToDisplayRectangle(committedROI);

            if (mouseLocationOnDisplay.X < committedROIInDisplayCoordinates.Left + 10)
            {
                if (mouseLocationOnDisplay.Y < committedROIInDisplayCoordinates.Top + 10)
                {
                    return DraggingMode.TopLeftCorner;
                }
                else if (mouseLocationOnDisplay.Y > committedROIInDisplayCoordinates.Bottom - 10)
                {
                    return DraggingMode.BottomLeftCorner;
                }
                else
                {
                    return DraggingMode.LeftEdge;
                }
            }
            else if (mouseLocationOnDisplay.X > committedROIInDisplayCoordinates.Right - 10)
            {
                if (mouseLocationOnDisplay.Y < committedROIInDisplayCoordinates.Top + 10)
                {
                    return DraggingMode.TopRightCorner;
                }
                else if (mouseLocationOnDisplay.Y > committedROIInDisplayCoordinates.Bottom - 10)
                {
                    return DraggingMode.BottomRightCorner;
                }
                else
                {
                    return DraggingMode.RightEdge;
                }
            }
            else if (mouseLocationOnDisplay.Y < committedROIInDisplayCoordinates.Top + 10)
            {
                return DraggingMode.TopEdge;
            }
            else if (mouseLocationOnDisplay.Y > committedROIInDisplayCoordinates.Bottom - 10)
            {
                return DraggingMode.BottomEdge;
            }
            else
            {
                return DraggingMode.WholeROI;
            }
        }


        private void StartDraggingNewROI(Point imagePoint)
        {
            if(!canCreateNew) { return; }

            originalDraggingROI = new Rectangle(imagePoint, Size.Empty);
            LiveDraggingROI = originalDraggingROI;
            draggingMode = DraggingMode.BottomRightCorner;
        }

        private void StartDraggingCommittedROI(DraggingMode newDragMode)
        {
            if (!canEditCommitted) { return; }

            originalDraggingROI = committedROI;
            LiveDraggingROI = originalDraggingROI;
            draggingMode = newDragMode;
        }


        /// <summary>
        /// Handles the mouse move event to update the ROI dimensions.
        /// </summary>
        public void OnMouseMove(MouseEventArgs e)
        {
            if (draggingMode != DraggingMode.None)
            {
                UpdateDraggingROI(mouseLocationOnDisplay: e.Location);
            }
            else if (!committedROI.IsEmpty)
            {
                var dragModeIfMouseClicked = DetermineDragModeFromMouseLocation(mouseLocationOnDisplay: e.Location);

                setMouseCursor(mouseCursors[dragModeIfMouseClicked]);
            }
        }

        private void UpdateDraggingROI(Point mouseLocationOnDisplay)
        {
            switch (draggingMode)
            {
                case DraggingMode.WholeROI:
                    UpdateWholeROIDragging(mouseLocationOnDisplay: mouseLocationOnDisplay);
                    break;
                case DraggingMode.TopLeftCorner:
                    UpdateTopLeftCornerDragging(mouseLocationOnDisplay: mouseLocationOnDisplay);
                    break;
                case DraggingMode.TopRightCorner:
                    UpdateTopRightCornerDragging(mouseLocationOnDisplay: mouseLocationOnDisplay);
                    break;
                case DraggingMode.BottomLeftCorner:
                    UpdateBottomLeftCornerDragging(mouseLocationOnDisplay: mouseLocationOnDisplay);
                    break;
                case DraggingMode.BottomRightCorner:
                    UpdateBottomRightCornerDragging(mouseLocationOnDisplay: mouseLocationOnDisplay);
                    break;
                case DraggingMode.TopEdge:
                    UpdateTopEdgeDragging(mouseLocationOnDisplay: mouseLocationOnDisplay);
                    break;
                case DraggingMode.BottomEdge:
                    UpdateBottomEdgeDragging(mouseLocationOnDisplay: mouseLocationOnDisplay);
                    break;
                case DraggingMode.LeftEdge:
                    UpdateLeftEdgeDragging(mouseLocationOnDisplay: mouseLocationOnDisplay);
                    break;
                case DraggingMode.RightEdge:
                    UpdateRightEdgeDragging(mouseLocationOnDisplay: mouseLocationOnDisplay);
                    break;
                default:
                    break;
            }

            setMouseCursor(mouseCursors[draggingMode]);

            invalidateDisplay();
        }

        private void UpdateRightEdgeDragging(Point mouseLocationOnDisplay)
        {
            var currentMouseLocationOverImage = mapDisplayPointToImagePoint(mouseLocationOnDisplay);

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
            var currentMouseLocationOverImage = mapDisplayPointToImagePoint(mouseLocationOnDisplay);
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
            var currentMouseLocationOverImage = mapDisplayPointToImagePoint(mouseLocationOnDisplay);

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
            var currentMouseLocationOverImage = mapDisplayPointToImagePoint(mouseLocationOnDisplay);
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
            var currentMouseLocationOverImage = mapDisplayPointToImagePoint(mouseLocationOnDisplay);

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
            var currentMouseLocationOverImage = mapDisplayPointToImagePoint(mouseLocationOnDisplay);

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
            var currentMouseLocationOverImage = mapDisplayPointToImagePoint(mouseLocationOnDisplay);
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
            var currentMouseLocationOverImage = mapDisplayPointToImagePoint(mouseLocationOnDisplay);
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
            var currentMouseLocationOverImage = mapDisplayPointToImagePoint(mouseLocationOnDisplay);
            var mouseDownLocationOverImage = mapDisplayPointToImagePoint(mouseDownLocationOnDisplay);

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
        public void OnMouseUp(MouseEventArgs e)
        {
            if(draggingMode == DraggingMode.None) { return; }

            CommittedROI = LiveDraggingROI;
            LiveDraggingROI = Rectangle.Empty;
            draggingMode = DraggingMode.None;

            invalidateDisplay();
        }


        /// <summary>
        /// Draws the current ROI.
        /// </summary>
        public void Draw(Graphics g)
        {
            if (!visible) { return; }

            if (!committedROI.IsEmpty)
            {
                var displayRect = mapImageRectangleToDisplayRectangle(committedROI);
                committedROIRenderer.Draw(g, displayRect);
            }

            if (draggingMode != DraggingMode.None)
            {
                var displayRect = mapImageRectangleToDisplayRectangle(LiveDraggingROI);
                liveDraggingROIRenderer.Draw(g, displayRect);
            }
        }
    }
}
