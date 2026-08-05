using System.Drawing;
using CDS.ImageDisplay.WinForms.BitmapDisplay;
using Humanizer;

namespace CDS.ImageDisplay.WinForms.Demo.DemoForms.OverlaysDemo;


internal static class OverlayPainter
{
    public static void Paint(WinForms.BitmapDisplay.BitmapDisplayPanel bitmapDisplayPanel, Graphics graphics, OverlayShapes shapes, OverlayDrawingSpecs overlaySettings)
    {
        PaintMetrics(bitmapDisplayPanel, graphics, shapes.Metrics, overlaySettings);
        PaintShapes(bitmapDisplayPanel, graphics, shapes, overlaySettings);
        PaintFloatingBubbles(bitmapDisplayPanel, graphics, shapes.Bubbles, overlaySettings.Bubbles);
    }

    private static void PaintMetrics(
        WinForms.BitmapDisplay.BitmapDisplayPanel bitmapDisplayPanel,
        Graphics graphics,
        WinForms.Overlays.TextPanel<MetricsMessageType> metricsPanel,
        OverlayDrawingSpecs overlaySettings)
    {
        metricsPanel.Clear();

        metricsPanel.AddMessage(MetricsMessageType.Info, $"Display mode      {bitmapDisplayPanel.DisplayMode.Humanize()}");
        metricsPanel.AddMessage(MetricsMessageType.Info, $"Display size      {bitmapDisplayPanel.ClientSize}");

        if (!bitmapDisplayPanel.AnythingToDisplay)
        {
            metricsPanel.AddMessage(MetricsMessageType.Info, "Image not loaded");
        }
        else
        {
            RectangleF r = bitmapDisplayPanel.PaintRect;
            metricsPanel.AddMessage(MetricsMessageType.Info, $"Bitmap size       {bitmapDisplayPanel.DisplayImage?.Size}");
            metricsPanel.AddMessage(MetricsMessageType.Info, $"Paint zoom        {bitmapDisplayPanel.Zoom:0.000}");
            metricsPanel.AddMessage(MetricsMessageType.Info, $"Paint rect        {r.X:0.0}, {r.Y:0.0}, {r.Width:0.0}, {r.Height:0:0}");
            metricsPanel.AddMessage(MetricsMessageType.Info, $"Format            {bitmapDisplayPanel.DisplayImage?.PixelFormat.Humanize()}");
        }

        metricsPanel.AddMessage(MetricsMessageType.Info, $"Paint foreground  {bitmapDisplayPanel.TimingMetrics.ForegroundPaint.Humanize()}");
        metricsPanel.AddMessage(MetricsMessageType.Info, $"Paint background  {bitmapDisplayPanel.TimingMetrics.BackgroundPaint.Humanize()}");

        metricsPanel.Draw(bitmapDisplayPanel, graphics, overlaySettings.MetricsPanel, _ => overlaySettings.MetricsText);
    }

    private static void PaintFloatingBubbles(WinForms.BitmapDisplay.BitmapDisplayPanel bitmapDisplayPanel, Graphics graphics, Bubble[] bubbles, WinForms.Overlays.DrawingSpec drawingSpec)
    {
        foreach (Bubble bubble in bubbles)
        {
            bubble.Draw(bitmapDisplayPanel, graphics, drawingSpec);
        }
    }

    private static void PaintShapes(WinForms.BitmapDisplay.BitmapDisplayPanel bitmapDisplayPanel, Graphics graphics, OverlayShapes shapes, OverlayDrawingSpecs overlaySettings)
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

        shapes.DonutRing.Draw(bitmapDisplayPanel, graphics, overlaySettings.Donuts);
        shapes.DonutSlice.Draw(bitmapDisplayPanel, graphics, overlaySettings.Donuts);
    }
}
