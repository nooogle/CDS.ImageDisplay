using CDS.Imaging.WinForms.BitmapDisplay;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;

namespace CDS.Imaging.WinForms.Draw
{
    /// <summary>
    /// Provides functionality to draw overlays on a bitmap display panel.
    /// </summary>
    public partial class OverlayPainter : Component
    {
        /// <summary>
        /// Cache of fonts for different sizes.
        /// </summary>
        private static Dictionary<(string fontName, int fontSize), Font> fonts = new Dictionary<(string fontName, int fontSize), Font>();

        /// <summary>
        /// Initializes a new instance of the <see cref="OverlayPainter"/> class.
        /// </summary>
        public OverlayPainter()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OverlayPainter"/> class with the specified container.
        /// </summary>
        /// <param name="container">The container to add the component to.</param>
        public OverlayPainter(IContainer container)
        {
            container.Add(this);
            InitializeComponent();
        }

        /// <summary>
        /// Applies the settings and performs the drawing.
        /// </summary>
        /// <param name="settings">The overlay settings to apply.</param>
        /// <param name="drawingAction">The drawing action to perform.</param>
        public void Draw(OverlayPainterSettings settings, Action drawingAction)
        {
            if (!settings.Enabled) { return; }

            pen.Color = settings.PenColor;
            pen.Width = settings.PenWidth;
            pen.StartCap = settings.PenStartCap;
            pen.EndCap = settings.PenEndCap;
            pen.DashStyle = settings.PenDashStyle;
            brush.Color = settings.BrushColor;
            CreateFont(settings);

            drawingAction();
        }

        /// <summary>
        /// Draws a line with the specified settings.
        /// </summary>
        /// <param name="settings">The overlay settings to apply.</param>
        /// <param name="p1">The start point of the line.</param>
        /// <param name="p2">The end point of the line.</param>
        /// <param name="sender">The bitmap display panel.</param>
        /// <param name="graphics">The graphics object to draw on.</param>
        /// <param name="lineEndsAlign">The pixel adjust mode for the line.</param>
        public void DrawLine(OverlayPainterSettings settings, Point p1, Point p2, BitmapDisplayPanel sender, Graphics graphics, DisplayPixelAlign lineEndsAlign)
        {
            PointF p1F = new PointF(p1.X, p1.Y);
            PointF p2F = new PointF(p2.X, p2.Y);
            DrawLine(settings, p1F, p2F, sender, graphics, lineEndsAlign);
        }

        /// <summary>
        /// Draws a line with the specified settings.
        /// </summary>
        /// <param name="settings">The overlay settings to apply.</param>
        /// <param name="p1">The start point of the line.</param>
        /// <param name="p2">The end point of the line.</param>
        /// <param name="sender">The bitmap display panel.</param>
        /// <param name="graphics">The graphics object to draw on.</param>
        /// <param name="displayPixelAlign">The pixel adjust mode for the line.</param>
        public void DrawLine(OverlayPainterSettings settings, PointF p1, PointF p2, BitmapDisplayPanel sender, Graphics graphics, DisplayPixelAlign displayPixelAlign)
        {
            Draw(settings, () =>
            {
                var p1OnDisplay = sender.MapImageToDisplay(p1, displayPixelAlign);
                var p2OnDisplay = sender.MapImageToDisplay(p2, displayPixelAlign);
                graphics.DrawLine(pen, p1OnDisplay, p2OnDisplay);
            });
        }

        /// <summary>
        /// Draws a rectangle with the specified settings.
        /// </summary>
        /// <param name="settings">The overlay settings to apply.</param>
        /// <param name="rectangle">The rectangle to draw.</param>
        /// <param name="sender">The bitmap display panel.</param>
        /// <param name="graphics">The graphics object to draw on.</param>
        /// <param name="cornerMode">The pixel adjust mode for the corners of the rectangle.</param>
        public void DrawRectangle(OverlayPainterSettings settings, Rectangle rectangle, BitmapDisplayPanel sender, Graphics graphics, DisplayPixelAlign cornerMode)
        {
            RectangleF rectangleF = new RectangleF(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height);
            DrawRectangle(settings, rectangleF, sender, graphics, cornerMode);
        }

        /// <summary>
        /// Draws a rectangle with the specified settings.
        /// </summary>
        /// <param name="settings">The overlay settings to apply.</param>
        /// <param name="rectangle">The rectangle to draw.</param>
        /// <param name="sender">The bitmap display panel.</param>
        /// <param name="graphics">The graphics object to draw on.</param>
        /// <param name="cornerMode">The pixel adjust mode for the corners of the rectangle.</param>
        public void DrawRectangle(OverlayPainterSettings settings, RectangleF rectangle, BitmapDisplayPanel sender, Graphics graphics, DisplayPixelAlign cornerMode)
        {
            Draw(settings, () =>
            {
                var rectangleOnDisplay = sender.MapImageToDisplay(rectangle, pixelAdjust: cornerMode);
                graphics.FillRectangle(brush, rectangleOnDisplay);
                graphics.DrawRectangle(pen, rectangleOnDisplay);
            });
        }

        /// <summary>
        /// Draws a circle with the specified settings.
        /// </summary>
        /// <param name="settings">The overlay settings to apply.</param>
        /// <param name="rectangle">The bounding rectangle of the circle.</param>
        /// <param name="sender">The bitmap display panel.</param>
        /// <param name="graphics">The graphics object to draw on.</param>
        /// <param name="centreMode">The pixel adjust mode for the centre of the circle.</param>
        public void DrawCircle(OverlayPainterSettings settings, Rectangle rectangle, BitmapDisplayPanel sender, Graphics graphics, DisplayPixelAlign centreMode)
        {
            RectangleF rectangleF = new RectangleF(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height);
            DrawCircle(settings, rectangleF, sender, graphics, centreMode);
        }

        /// <summary>
        /// Draws a circle with the specified settings.
        /// </summary>
        /// <param name="settings">The overlay settings to apply.</param>
        /// <param name="rectangle">The bounding rectangle of the circle.</param>
        /// <param name="sender">The bitmap display panel.</param>
        /// <param name="graphics">The graphics object to draw on.</param>
        /// <param name="centreMode">The pixel adjust mode for the centre of the circle.</param>
        public void DrawCircle(OverlayPainterSettings settings, RectangleF rectangle, BitmapDisplayPanel sender, Graphics graphics, DisplayPixelAlign centreMode)
        {
            Draw(settings, () =>
            {
                var rectangleOnDisplay = sender.MapImageToDisplay(rectangle, pixelAdjust: centreMode);
                graphics.FillEllipse(brush, rectangleOnDisplay);
                graphics.DrawEllipse(pen, rectangleOnDisplay);
            });
        }

        /// <summary>
        /// Draws an ellipse with the specified settings.
        /// </summary>
        /// <param name="settings">The overlay settings to apply.</param>
        /// <param name="centre">The center of the ellipse.</param>
        /// <param name="majorAxis">The length of the major axis.</param>
        /// <param name="minorAxis">The length of the minor axis.</param>
        /// <param name="majorAxisAngleDegrees">The angle of the major axis in degrees.</param>
        /// <param name="sender">The bitmap display panel.</param>
        /// <param name="graphics">The graphics object to draw on.</param>
        /// <param name="originMode">The pixel adjust mode for the origin of the ellipse.</param>
        public void DrawEllipse(
            OverlayPainterSettings settings,
            PointF centre,
            float majorAxis,
            float minorAxis,
            float majorAxisAngleDegrees,
            BitmapDisplayPanel sender,
            Graphics graphics,
            DisplayPixelAlign originMode)
        {
            Draw(settings, () =>
            {
                // Save the current state of the Graphics object
                var state = graphics.Save();

                try
                {
                    // Translate the ellipse from image to display coordinates
                    var centreOnDisplay = sender.MapImageToDisplay(centre, originMode);
                    var majorAxisOnDisplay = sender.MapImageToDisplay(majorAxis);
                    var minorAxisOnDisplay = sender.MapImageToDisplay(minorAxis);

                    // Translate to the center of the ellipse
                    graphics.TranslateTransform(centreOnDisplay.X, centreOnDisplay.Y);

                    // Rotate by the specified angle
                    graphics.RotateTransform(majorAxisAngleDegrees);

                    // Draw the ellipse (centered at the origin after translation)
                    graphics.FillEllipse(brush, (-majorAxisOnDisplay / 2), (-minorAxisOnDisplay / 2), majorAxisOnDisplay, minorAxisOnDisplay);
                    graphics.DrawEllipse(pen, (-majorAxisOnDisplay / 2), (-minorAxisOnDisplay) / 2, majorAxisOnDisplay, minorAxisOnDisplay);
                }
                finally
                {
                    // Restore the original state
                    graphics.Restore(state);
                }
            });
        }


        /// <summary>
        /// Draws a polygon with the specified settings.
        /// </summary>
        /// <param name="graphics">The graphics object to draw on.</param>
        /// <param name="pointAlign">The pixel adjust mode for the points of the polygon.</param>
        /// <param name="points">The points of the polygon.</param>
        /// <param name="sender">The bitmap display panel.</param>
        /// <param name="settings">The overlay settings to apply.</param>
        public void DrawPolygon(
            OverlayPainterSettings settings, 
            PointF[] points, 
            BitmapDisplayPanel sender, 
            Graphics graphics, 
            DisplayPixelAlign pointAlign)
        {
            Draw(settings, () =>
            {
                PointF[] pointsOnDisplay = new PointF[points.Length];
                
                for (int i = 0; i < points.Length; i++)
                {
                    pointsOnDisplay[i] = sender.MapImageToDisplay(points[i], pointAlign);
                }

                graphics.FillPolygon(brush, pointsOnDisplay);
                graphics.DrawPolygon(pen, pointsOnDisplay);
            });
        }


        /// <summary>
        /// Draws a crosshair with the specified settings.
        /// </summary>
        /// <param name="centre">The center of the crosshair.</param>
        /// <param name="centreGap">The gap between the center and the crosshair lines.</param>
        /// <param name="pixelAlignment">The pixel adjust mode for the center of the crosshair.</param>
        /// <param name="graphics">The graphics object to draw on.</param>
        /// <param name="lineLength">The length of the crosshair lines.</param>
        /// <param name="sender">The bitmap display panel.</param>
        /// <param name="settings">The overlay settings to apply.</param>
        public void DrawCrossHair(
            OverlayPainterSettings settings, 
            PointF centre, 
            float lineLength, 
            float centreGap,
            BitmapDisplayPanel sender, 
            Graphics graphics, 
            DisplayPixelAlign pixelAlignment)
        {
            Draw(settings, () =>
            {
                var centreOnDisplay = sender.MapImageToDisplay(centre, pixelAlignment);
                var lineLengthOnDisplay = sender.MapImageToDisplay(lineLength);
                var centreGapOnDisplay = sender.MapImageToDisplay(centreGap);

                // top line
                graphics.DrawLine(
                    pen, 
                    centreOnDisplay.X, 
                    centreOnDisplay.Y - lineLengthOnDisplay - centreGapOnDisplay, 
                    centreOnDisplay.X, 
                    centreOnDisplay.Y - centreGapOnDisplay);

                // bottom line
                graphics.DrawLine(
                    pen,
                    centreOnDisplay.X,
                    centreOnDisplay.Y + centreGapOnDisplay,
                    centreOnDisplay.X,
                    centreOnDisplay.Y + lineLengthOnDisplay + centreGapOnDisplay);

                // left line
                graphics.DrawLine(
                    pen,
                    centreOnDisplay.X - lineLengthOnDisplay - centreGapOnDisplay,
                    centreOnDisplay.Y,
                    centreOnDisplay.X - centreGapOnDisplay,
                    centreOnDisplay.Y);

                // right line
                graphics.DrawLine(
                    pen,
                    centreOnDisplay.X + centreGapOnDisplay,
                    centreOnDisplay.Y,
                    centreOnDisplay.X + lineLengthOnDisplay + centreGapOnDisplay,
                    centreOnDisplay.Y);
            });
        }


        /// <summary>
        /// Creates and caches a font for the specified size if it does not exist.
        /// </summary>
        /// <param name="settings">The overlay settings containing the font name and size.</param>
        private void CreateFont(OverlayPainterSettings settings)
        {
            if (fonts.ContainsKey((fontName: settings.FontName, fontSize: settings.FontSize))) { return; }

            Font newFont = SystemFonts.DefaultFont;

            try
            {
                newFont = new Font(settings.FontName, settings.FontSize);
            }
            catch
            {
                newFont = SystemFonts.DefaultFont;
            }

            fonts[(fontName: settings.FontName, fontSize: settings.FontSize)] = newFont;
        }

        /// <summary>
        /// Draws text with the specified settings.
        /// </summary>
        /// <param name="settings">The overlay settings to apply.</param>
        /// <param name="text">The text to draw.</param>
        /// <param name="point">The location to draw the text.</param>
        /// <param name="sender">The bitmap display panel.</param>
        /// <param name="graphics">The graphics object to draw on.</param>
        public void DrawText(OverlayPainterSettings settings, string text, Point point, BitmapDisplayPanel sender, Graphics graphics)
        {
            PointF pointF = new PointF(point.X, point.Y);
            DrawText(settings, text, pointF, sender, graphics);
        }

        /// <summary>
        /// Draws text with the specified settings.
        /// </summary>
        /// <param name="settings">The overlay settings to apply.</param>
        /// <param name="text">The text to draw.</param>
        /// <param name="point">The location to draw the text.</param>
        /// <param name="sender">The bitmap display panel.</param>
        /// <param name="graphics">The graphics object to draw on.</param>
        public void DrawText(OverlayPainterSettings settings, string text, PointF point, BitmapDisplayPanel sender, Graphics graphics)
        {
            Draw(settings, () =>
            {
                var pointOnDisplay = sender.MapImageToDisplay(point, DisplayPixelAlign.TopLeft);
                graphics.DrawString(text, fonts[(fontName: settings.FontName, fontSize: settings.FontSize)], brush, pointOnDisplay);
            });
        }
    }
}

