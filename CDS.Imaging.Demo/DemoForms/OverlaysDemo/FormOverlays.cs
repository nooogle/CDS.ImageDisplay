using CDS.Imaging.BitmapDisplay;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
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
        bitmap = BitmapGenerator.Make(new Size(800, 600));
    }


    /// <summary>
    /// Setup after the form has loaded
    /// </summary>
    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        testSettings.Shapes.PostLoadConfigure(testSettings.Rendering);

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
    private void bitmapDisplayPanel_OnPaintOver(CDS.Imaging.BitmapDisplay.BitmapDisplayPanel sender, System.Drawing.Graphics graphics)
    {
        if (bitmapDisplayPanel == null) { return; }
        if (bitmapDisplayPanel.GetDisplayImage() == null) { return; }

        testSettings.Shapes.Layer1.Draw(sender, graphics);
    }


    /// <summary>
    /// A property has changed, so repaint the image
    /// </summary>
    private void propertyGrid_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
    {
        bitmapDisplayPanel.Invalidate();
    }
}
