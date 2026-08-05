using System;
using System.Drawing;
using System.Windows.Forms;

namespace CDS.ImageDisplay.WinForms.Demo.DemoForms.OverlaysDemo;


/// <summary>
/// Form for demonstrating the ROISelectionOnBitmapDisplay
/// </summary>
internal sealed partial class FormOverlays : Form
{
    private readonly TestSettings? testSettings;
    private readonly Bitmap bitmap;

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

        if (testSettings == null)
        { return; }

        testSettings.Shapes.RecreateBubbles(bitmapDisplayPanel.Size);
        bitmapDisplayPanel.FitToWindowCentred();
    }

    protected override void OnFormClosed(FormClosedEventArgs e)
    {
        base.OnFormClosed(e);
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
    private void bitmapDisplayPanel_OnPaintOver(object sender, CDS.ImageDisplay.WinForms.BitmapDisplay.PaintOverEventArgs e)
    {
        if (testSettings == null)
        { return; }
        if (bitmapDisplayPanel == null)
        { return; }
        if (bitmapDisplayPanel.DisplayImage == null)
        { return; }

        OverlayPainter.Paint(bitmapDisplayPanel, e.Graphics, testSettings.Shapes, testSettings.Overlays);
    }


    /// <summary>
    /// A property has changed, so repaint the image
    /// </summary>
    private void propertyGrid_PropertyValueChanged(object s, PropertyValueChangedEventArgs e) => bitmapDisplayPanel.Invalidate();


    /// <summary>
    /// Bubble animation timer
    /// </summary>
    private void timerBubbles_Tick(object sender, EventArgs e)
    {
        if (testSettings == null)
        { return; }

        testSettings.Shapes.MoveBubbles();
        bitmapDisplayPanel.Invalidate();
    }
}
