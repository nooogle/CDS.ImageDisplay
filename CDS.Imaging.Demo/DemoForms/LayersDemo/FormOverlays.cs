using CDS.Imaging.WinForms.BitmapDisplay;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;

namespace CDS.Imaging.Demo.DemoForms.LayersDemo;


/// <summary>
/// Form for demonstrating the ROISelectionOnBitmapDisplay
/// </summary>
public partial class FormOverlays : Form
{
    private Bitmap bitmap;
    private WinForms.Draw.Layer shapes;

    /// <summary>
    /// Constructor
    /// </summary>
    public FormOverlays()
    {
        InitializeComponent();
        var imageSize = new Size(800, 600);
        bitmap = BitmapGenerator.Make(imageSize);
        shapes = ShapesFactory.Create(imageSize);
    }


    /// <summary>
    /// Setup after the form has loaded
    /// </summary>
    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        propertyGrid.SelectedObject = shapes;
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

        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        shapes.Draw(sender, graphics);
        stopwatch.Stop();
    }


    /// <summary>
    /// A property has changed, so repaint the image
    /// </summary>
    private void propertyGrid_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
    {
        bitmapDisplayPanel.Invalidate();
    }


    /// <summary>
    /// Redraw the image (just in case any properties have changed that didn't
    /// trigger a repaint)
    /// </summary>
    private void timerRefresh_Tick(object sender, EventArgs e)
    {
        bitmapDisplayPanel.Invalidate();
    }
}
