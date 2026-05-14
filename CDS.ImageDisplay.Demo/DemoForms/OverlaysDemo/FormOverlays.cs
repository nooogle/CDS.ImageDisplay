using Humanizer;
using System;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace CDS.ImageDisplay.Demo.DemoForms.OverlaysDemo;


/// <summary>
/// Form for demonstrating the ROISelectionOnBitmapDisplay
/// </summary>
public partial class FormOverlays : Form
{
    private TestSettings? testSettings;
    private Bitmap bitmap;
    private OverlayPainter overlayPainter = new OverlayPainter();

    /// <summary>
    /// Constructor
    /// </summary>
    public FormOverlays(TestSettings testSettings)
    {
        InitializeComponent();
        this.testSettings = testSettings;
        bitmap = BitmapGenerator.Make(new Size(800, 600));
    }


    /// <summary>
    /// Setup after the form has loaded
    /// </summary>
    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        bitmapDisplayPanel.SetImage(bitmap);
        propertyGrid.SelectedObject = testSettings;
    }

    protected override void OnClientSizeChanged(EventArgs e)
    {
        base.OnClientSizeChanged(e);

        if (testSettings == null) { return; }

        testSettings.Shapes.RecreateBubbles(bitmapDisplayPanel.Size);
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
    private void bitmapDisplayPanel_OnPaintOver(CDS.ImageDisplay.BitmapDisplay.BitmapDisplayPanel sender, System.Drawing.Graphics graphics)
    {
        if (testSettings == null) { return; }
        if (bitmapDisplayPanel == null) { return; }
        if (bitmapDisplayPanel.GetDisplayImage() == null) { return; }

        PaintMetrics(sender, graphics);
        overlayPainter.Paint(bitmapDisplayPanel, graphics, testSettings.Shapes, testSettings.Overlays);
    }


    private void PaintMetrics(BitmapDisplay.BitmapDisplayPanel sender, Graphics graphics)
    {
        var info = new StringBuilder();
        info.Append($"Display mode      {sender.DisplayMode.Humanize()}\n");
        info.Append($"Display size      {sender.ClientSize}\n");
        
        if (!sender.AnythingToDisplay)
        {
            info.Append($"Image not loaded\n");
        }
        else
        {
            var r = sender.PaintRect;
            info.Append($"Bitmap size       {sender.GetDisplayImage()?.Size}\n");
            info.Append($"Paint zoom        {sender.Zoom:0.000}\n");
            info.Append($"Paint rect        {r.X:0.0}, {r.Y:0.0}, {r.Width:0.0}, {r.Height:0:0}\n");
            info.Append($"Format            {sender.GetDisplayImage()?.PixelFormat.Humanize()}\n");
        }

        info.Append($"Paint foreground  {sender.TimingMetrics.ForegroundPaint.Humanize()}\n");
        info.Append($"Paint background  {sender.TimingMetrics.BackgroundPaint.Humanize()}\n");
        
        var textTopleft = new PointF(12, 12);

        var font = Overlays.DrawingToolsPool.GetFont(new Overlays.FontSpec() { FontName = "Courier New", FontSize = 10 });

        graphics.DrawString(info.ToString(), font, Brushes.Navy, textTopleft);
    }


    /// <summary>
    /// A property has changed, so repaint the image
    /// </summary>
    private void propertyGrid_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
    {
        bitmapDisplayPanel.Invalidate();
    }


    /// <summary>
    /// Bubble animation timer
    /// </summary>
    private void timerBubbles_Tick(object sender, EventArgs e)
    {
        if (testSettings == null) { return; }

        testSettings.Shapes.MoveBubbles();
        bitmapDisplayPanel.Invalidate();
    }
}
