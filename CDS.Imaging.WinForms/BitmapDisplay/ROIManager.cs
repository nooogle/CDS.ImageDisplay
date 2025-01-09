using System;
using System.Collections.Immutable;
using System.Drawing;
using System.Windows.Forms;

namespace CDS.Imaging.WinForms.BitmapDisplay
{


    public class ROIManager : IDisposable
    {
        private bool isDisposed = false;
        private readonly Func<Point, Point> mapImagePointToDisplayPoint;
        private readonly Func<Rectangle, Rectangle> mapImageRectangleToDisplayRectangle;
        private readonly Func<Point, Point> mapDisplayPointToImagePoint;
        private readonly Action invalidateDisplay;
        private readonly Action<Cursor> setMouseCursor;

        private readonly ImmutableDictionary<DraggingMode, Cursor> mouseCursors;

        private DraggingMode draggingMode = DraggingMode.None;
        private Size? imageSize;
        private Rectangle committedROI;
        private Rectangle originalDraggingROI;
        private Rectangle liveDraggingROI;
        private Point mouseDownLocationOnDisplay;

        private RectangleRenderer committedROIRenderer;
        private RectangleRenderer liveDraggingROIRenderer;

        public ROIManager(
            Func<Point, Point> mapImagePointToDisplayPoint,
            Func<Rectangle, Rectangle> mapImageRectangleToDisplayRectangle,
            Func<Point, Point> mapDisplayPointToImagePoint,
            Action invalidateDisplay,
            Action<Cursor> setMouseCursor)
        {
            this.imageSize = null;

            this.mapImagePointToDisplayPoint = mapImagePointToDisplayPoint;
            this.mapImageRectangleToDisplayRectangle = mapImageRectangleToDisplayRectangle;
            this.mapDisplayPointToImagePoint = mapDisplayPointToImagePoint;
            this.invalidateDisplay = invalidateDisplay;

            this.mouseCursors = CreateMouseCursorsDict();
            this.setMouseCursor = setMouseCursor;

            committedROIRenderer = new RectangleRenderer();
            committedROIRenderer.ShowGrapples = true;
            committedROIRenderer.OutlinePen.Color = Color.FromArgb(128, Color.Red);
            committedROIRenderer.OutlinePen.Width = 2;

            liveDraggingROIRenderer = new RectangleRenderer();
            liveDraggingROIRenderer.ShowGrapples = true;
            liveDraggingROIRenderer.OutlinePen.Color = Color.FromArgb(128, Color.Red);
            liveDraggingROIRenderer.OutlinePen.Width = 4;
            liveDraggingROIRenderer.FillBrush.Color = Color.FromArgb(32, Color.Red);
            liveDraggingROIRenderer.GrapplePen.Color = Color.FromArgb(128, Color.Cyan);
            liveDraggingROIRenderer.GrapplePen.Width = 2;
            liveDraggingROIRenderer.GrappleBrush.Color = Color.FromArgb(128, Color.Navy);
        }


        public void Dispose()
        {
            if (isDisposed) { return; }
         
            committedROIRenderer.Dispose();
            liveDraggingROIRenderer.Dispose();
            
            isDisposed = true;
        }


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

            committedROI = roi;
            committedROI.Intersect(new Rectangle(Point.Empty, imageSize.Value));
            invalidateDisplay();
        }


        public void SetImageSize(Size? imageSize)
        {
            this.imageSize = imageSize;
        }


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

        private void OnLeftMouseButtonDown(MouseEventArgs e)
        {
            mouseDownLocationOnDisplay = e.Location;
            var imagePoint = mapDisplayPointToImagePoint(e.Location);

            var newDragMode = DetermineDragModeFromMouseLocation(imagePoint);

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


        private DraggingMode DetermineDragModeFromMouseLocation(Point mouseLocationOverImage)
        {
            if (committedROI.IsEmpty) { return DraggingMode.None; }
            if (!committedROI.Contains(mouseLocationOverImage)) { return DraggingMode.None; }

            if (mouseLocationOverImage.X < committedROI.Left + 10)
            {
                if (mouseLocationOverImage.Y < committedROI.Top + 10)
                {
                    return DraggingMode.TopLeftCorner;
                }
                else if (mouseLocationOverImage.Y > committedROI.Bottom - 10)
                {
                    return DraggingMode.BottomLeftCorner;
                }
                else
                {
                    return DraggingMode.LeftEdge;
                }
            }
            else if (mouseLocationOverImage.X > committedROI.Right - 10)
            {
                if (mouseLocationOverImage.Y < committedROI.Top + 10)
                {
                    return DraggingMode.TopRightCorner;
                }
                else if (mouseLocationOverImage.Y > committedROI.Bottom - 10)
                {
                    return DraggingMode.BottomRightCorner;
                }
                else
                {
                    return DraggingMode.RightEdge;
                }
            }
            else if (mouseLocationOverImage.Y < committedROI.Top + 10)
            {
                return DraggingMode.TopEdge;
            }
            else if (mouseLocationOverImage.Y > committedROI.Bottom - 10)
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
            originalDraggingROI = new Rectangle(imagePoint, Size.Empty);
            liveDraggingROI = originalDraggingROI;
            draggingMode = DraggingMode.BottomRightCorner;
        }

        private void StartDraggingCommittedROI(DraggingMode newDragMode)
        {
            originalDraggingROI = committedROI;
            liveDraggingROI = originalDraggingROI;
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
                var mouseLocationOverImage = mapDisplayPointToImagePoint(e.Location);
                var dragModeIfMouseClicked = DetermineDragModeFromMouseLocation(mouseLocationOverImage);

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
                liveDraggingROI = Rectangle.FromLTRB(
                    currentMouseLocationOverImage.X,
                    originalDraggingROI.Top,
                    originalDraggingROI.Left,
                    originalDraggingROI.Bottom);
            }
            else
            {
                liveDraggingROI = Rectangle.FromLTRB(
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
                liveDraggingROI = Rectangle.FromLTRB(
                    originalDraggingROI.Right,
                    originalDraggingROI.Top,
                    currentMouseLocationOverImage.X,
                    originalDraggingROI.Bottom);
            }
            else
            {
                liveDraggingROI = Rectangle.FromLTRB(
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
                liveDraggingROI = Rectangle.FromLTRB(
                    originalDraggingROI.Left,
                    originalDraggingROI.Bottom,
                    originalDraggingROI.Right,
                    currentMouseLocationOverImage.Y);
            }
            else
            {
                liveDraggingROI = Rectangle.FromLTRB(
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
                liveDraggingROI = Rectangle.FromLTRB(
                    originalDraggingROI.Left,
                    currentMouseLocationOverImage.Y,
                    originalDraggingROI.Right,
                    originalDraggingROI.Top);
            }
            else
            {
                liveDraggingROI = Rectangle.FromLTRB(
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

            liveDraggingROI = Rectangle.FromLTRB(left, top, right, bottom);
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

            liveDraggingROI = Rectangle.FromLTRB(left, top, right, bottom);

            System.Diagnostics.Debug.WriteLine(liveDraggingROI);
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
            liveDraggingROI = Rectangle.FromLTRB(left, top, right, bottom);
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
            liveDraggingROI = Rectangle.FromLTRB(left, top, right, bottom);
        }


        private void UpdateWholeROIDragging(Point mouseLocationOnDisplay)
        {
            var currentMouseLocationOverImage = mapDisplayPointToImagePoint(mouseLocationOnDisplay);
            var mouseDownLocationOverImage = mapDisplayPointToImagePoint(mouseDownLocationOnDisplay);

            var deltaX = currentMouseLocationOverImage.X - mouseDownLocationOverImage.X;
            var deltaY = currentMouseLocationOverImage.Y - mouseDownLocationOverImage.Y;

            liveDraggingROI = Rectangle.FromLTRB(
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

            committedROI = liveDraggingROI;
            draggingMode = DraggingMode.None;

            committedROI.Intersect(new Rectangle(Point.Empty, imageSize!.Value));
            if(committedROI.IsEmpty) { committedROI = Rectangle.Empty; }

            invalidateDisplay();
        }


        /// <summary>
        /// Draws the current ROI.
        /// </summary>
        public void Draw(Graphics g)
        {
            if (!committedROI.IsEmpty)
            {
                var displayRect = mapImageRectangleToDisplayRectangle(committedROI);
                committedROIRenderer.Draw(g, displayRect);
            }

            if (draggingMode != DraggingMode.None)
            {
                var displayRect = mapImageRectangleToDisplayRectangle(liveDraggingROI);
                liveDraggingROIRenderer.Draw(g, displayRect);
            }
        }
    }
}
