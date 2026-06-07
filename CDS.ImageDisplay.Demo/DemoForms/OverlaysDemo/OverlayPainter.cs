using System.Drawing;
using CDS.ImageDisplay.BitmapDisplay;

namespace CDS.ImageDisplay.Demo.DemoForms.OverlaysDemo;


internal static class OverlayPainter
{
    public static void Paint(BitmapDisplay.BitmapDisplayPanel bitmapDisplayPanel, Graphics graphics, OverlayShapes shapes, OverlayDrawingSpecs overlaySettings)
    {
        PaintShapes(bitmapDisplayPanel, graphics, shapes, overlaySettings);
        PaintFloatingBubbles(bitmapDisplayPanel, graphics, shapes.Bubbles, overlaySettings.Bubbles);
    }

    private static void PaintFloatingBubbles(BitmapDisplayPanel bitmapDisplayPanel, Graphics graphics, Bubble[] bubbles, Overlays.DrawingSpec drawingSpec)
    {
        foreach (Bubble bubble in bubbles)
        {
            bubble.Draw(bitmapDisplayPanel, graphics, drawingSpec);
        }
    }

    private static void PaintShapes(BitmapDisplayPanel bitmapDisplayPanel, Graphics graphics, OverlayShapes shapes, OverlayDrawingSpecs overlaySettings)
    {
        shapes.Rectangle1.Draw(bitmapDisplayPanel, graphics, overlaySettings.Rectangles);
        shapes.RotatedRectangle1.Draw(bitmapDisplayPanel, graphics, overlaySettings.Rectangles);

        shapes.CrossHairShape.Draw(bitmapDisplayPanel, graphics, overlaySettings.CrossHair);

        shapes.EllipseShape.Draw(bitmapDisplayPanel, graphics, overlaySettings.Ellipses);

        shapes.Line1.Draw(bitmapDisplayPanel, graphics, overlaySettings.Lines);
        shapes.Line2.Draw(bitmapDisplayPanel, graphics, overlaySettings.Lines);

        shapes.Text1.Draw(bitmapDisplayPanel, graphics, overlaySettings.Text);
        shapes.Text2.Draw(bitmapDisplayPanel, graphics, overlaySettings.Text);

        shapes.Circle1.Draw(bitmapDisplayPanel, graphics, overlaySettings.Circles);
        shapes.Circle2.Draw(bitmapDisplayPanel, graphics, overlaySettings.Circles);

        shapes.PolygonShape.Draw(bitmapDisplayPanel, graphics, overlaySettings.Polygons);
    }
}
