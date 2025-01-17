namespace System.Drawing
{
    /// <summary>
    /// Extension methods for drawing.
    /// </summary>
    public static class DrawingExtensionMethods
    {
        /// <summary>
        /// Returns true if the point is inside the rectangle or touching the rectangle.
        /// </summary>
        public static bool IsInOrTouching(this Point point, Rectangle rectangle)
        {
            return
                (point.X >= rectangle.Left) &&
                (point.X <= rectangle.Right) &&
                (point.Y >= rectangle.Top) &&
                (point.Y <= rectangle.Bottom);
        }
    }
}
