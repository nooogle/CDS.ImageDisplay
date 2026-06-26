using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using CDS.ImageDisplay.WinForms.Annotations;

namespace CDS.ImageDisplay.WinForms.Demo.DemoForms.AnnotationsDemo;

/// <summary>
/// Demo that lets the user draw annotations on the Thailand image then save the result as a PNG.
/// </summary>
internal sealed partial class FormAnnotationsToBitmap : Form
{
    private Annotation? _lastCreated;

    /// <summary>
    /// Initializes a new instance of <see cref="FormAnnotationsToBitmap"/>.
    /// </summary>
    public FormAnnotationsToBitmap()
    {
        InitializeComponent();
        UpdateStatus();
    }

    /// <summary>Setup after the form has loaded.</summary>
    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);
        bitmapDisplayPanel.SetImage(Properties.Resources.Thailand);
        bitmapDisplayPanel.FitToWindowCentred();
    }

    /// <summary>The form has been resized.</summary>
    protected override void OnSizeChanged(EventArgs e)
    {
        base.OnSizeChanged(e);
        bitmapDisplayPanel.FitToWindowCentred();
    }

    private void annotationManager_AnnotationCreated(object sender, AnnotationCreatedEventArgs e)
    {
        _lastCreated = e.Annotation;
        UpdateStatus();
    }

    private void annotationManager_AnnotationDeleted(object sender, AnnotationDeletedEventArgs e)
    {
        if (_lastCreated == e.Annotation) { _lastCreated = null; }
        UpdateStatus();
    }

    private void btnSavePng_Click(object sender, EventArgs e)
    {
        using var dlg = new SaveFileDialog
        {
            Title = "Save annotated image",
            Filter = "PNG files (*.png)|*.png|All files (*.*)|*.*",
            DefaultExt = "png",
        };

        if (dlg.ShowDialog(this) != DialogResult.OK) { return; }

        using var source = Properties.Resources.Thailand;
        using var clone = (Bitmap)source.Clone();
        using (var g = Graphics.FromImage(clone))
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;
            annotationManager.DrawAnnotationsToBitmap(g);
        }

        clone.Save(dlg.FileName, System.Drawing.Imaging.ImageFormat.Png);
        labelStatus.Text = $"Saved to {System.IO.Path.GetFileName(dlg.FileName)}";
    }

    private void UpdateStatus()
    {
        int count = annotationManager.Annotations.Count;
        string last = _lastCreated is null
            ? "<none>"
            : $"{GeometryTypeName(_lastCreated)} \"{_lastCreated.Title}\"";
        labelStatus.Text = $"Count: {count} | Last: {last}";
    }

    private static string GeometryTypeName(Annotation annotation)
    {
        string name = annotation.Geometry.GetType().Name;
        const string suffix = "AnnotationGeometry";
        return name.EndsWith(suffix, StringComparison.Ordinal) ? name.Substring(0, name.Length - suffix.Length) : name;
    }
}
