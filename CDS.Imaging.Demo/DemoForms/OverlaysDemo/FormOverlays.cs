using CDS.Imaging.WinForms.BitmapDisplay;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace CDS.Imaging.Demo.DemoForms.OverlaysDemo;


/// <summary>
/// Form for demonstrating the ROISelectionOnBitmapDisplay
/// </summary>
public partial class FormOverlays : Form
{
    private TestSettings testSettings = new TestSettings();
    private Bitmap bitmap;


    /// <summary>
    /// Constructor
    /// </summary>
    public FormOverlays()
    {
        InitializeComponent();
        bitmap = BitmapGenerator.Make(800, 600);
    }


    /// <summary>
    /// Setup after the form has loaded
    /// </summary>
    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        propertyGrid.SelectedObject = testSettings;
        bitmapDisplayPanel.SetImage(bitmap);
    }

    protected override void OnClientSizeChanged(EventArgs e)
    {
        base.OnClientSizeChanged(e);
        bitmapDisplayPanel.FitToWindowCentred();
    }

    protected override void OnFormClosed(FormClosedEventArgs e)
    {
        base.OnFormClosed(e);
        bitmap.Dispose();
    }


    /// <summary>
    /// The form has been resized, so fit the bitmap display to the window
    /// </summary>
    protected override void OnSizeChanged(EventArgs e)
    {
        base.OnSizeChanged(e);
        bitmapDisplayPanel.FitToWindowCentred();
    }


    /// <summary>
    /// Paint allOverlaySettings over the image
    /// </summary>
    private void bitmapDisplayPanel_OnPaintOver(CDS.Imaging.WinForms.BitmapDisplay.BitmapDisplayPanel sender, System.Drawing.Graphics graphics)
    {
        if (bitmapDisplayPanel == null) { return; }
        if (bitmapDisplayPanel.GetDisplayImage() == null) { return; }

        var imageSize = bitmapDisplayPanel.GetDisplayImage()!.Size;

        OverlayRectangles(sender, graphics, imageSize);
        OverlayLines(sender, graphics, imageSize);
        OverlayBubbles(sender, graphics, imageSize);
        OverlayText(sender, graphics);
        OverlayEllipses(sender, graphics, imageSize);
    }

    private void OverlayEllipses(BitmapDisplayPanel sender, Graphics graphics, Size imageSize)
    {
        if (!testSettings.Settings.Ellipses.Enabled) { return; };

        foreach (var ellipse in testSettings.Shapes.Ellipses)
        {
            overlayPainter.DrawEllipse(
                settings: testSettings.Settings.Ellipses,
                centre: ellipse.Centre,
                majorAxis: ellipse.MajorAxis,
                minorAxis: ellipse.MinorAxis,
                majorAxisAngleDegrees: ellipse.MajorAxisAngleDegrees,
                sender: sender,
                graphics: graphics,
                originMode: ellipse.OriginOnDisplayMode);
        }
    }

    private void OverlayText(BitmapDisplayPanel sender, Graphics graphics)
    {
        foreach (var textMessage in testSettings.Shapes.TextMessages)
        {
            overlayPainter.DrawText(
                testSettings.Settings.Text,
                textMessage.Text,
                textMessage.Location,
                sender,
                graphics);
        }
    }

    private void OverlayBubbles(BitmapDisplayPanel sender, Graphics graphics, Size imageSize)
    {
        if (testSettings.Settings.Circles.Enabled)
        {
            foreach (var circle in testSettings.Shapes.Circles)
            {
                overlayPainter.DrawCircle(
                        testSettings.Settings.Circles,
                        new RectangleF(
                            circle.Centre.X - circle.Radius,
                            circle.Centre.Y - circle.Radius,
                            2 * circle.Radius,
                            2 * circle.Radius),
                        sender,
                        graphics,
                        centreMode: circle.CentreDisplayMode);
            }
        }
    }

    private void OverlayLines(BitmapDisplayPanel sender, Graphics graphics, Size imageSize)
    {
        foreach (var line in testSettings.Shapes.Lines)
        {
            overlayPainter.DrawLine(
                testSettings.Settings.Lines,
                line.Start,
                line.End,
                sender,
                graphics,
                displayPixelAlign: line.LineEndDisplayMode);
        }
    }

    private void OverlayRectangles(BitmapDisplayPanel sender, Graphics graphics, Size imageSize)
    {
        foreach (var rectangle in testSettings.Shapes.Rectangles)
        {
            overlayPainter.DrawRectangle(
                testSettings.Settings.Rectangles,
                rectangle,
                sender,
                graphics,
                cornerMode: rectangle.CornerDisplayMode);
        }
    }


    /// <summary>
    /// A property has changed, so repaint the image
    /// </summary>
    private void propertyGrid_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
    {
        bitmapDisplayPanel.Invalidate();
    }
}
