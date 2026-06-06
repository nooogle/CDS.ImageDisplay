using System;
using System.Drawing;
using System.Windows.Forms;
using CDS.ImageDisplay.Annotations;

namespace CDS.ImageDisplay.Demo.DemoForms.AnnotationsDemo;

/// <summary>
/// Minimal demo of the annotation manager — draw shapes on an image by clicking or dragging.
/// </summary>
internal sealed partial class FormAnnotationsSimple : Form
{
    private Annotation? _lastCreated;

    /// <summary>
    /// Initializes a new instance of <see cref="FormAnnotationsSimple"/>.
    /// </summary>
    public FormAnnotationsSimple()
    {
        InitializeComponent();
        UpdateStatus();
    }

    /// <summary>Setup after the form has loaded.</summary>
    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);
        using var bitmap = BitmapGenerator.Make(new Size(800, 600));
        bitmapDisplayPanel.SetImage(bitmap);
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
        return name.EndsWith(suffix, StringComparison.Ordinal) ? name[..^suffix.Length] : name;
    }
}
