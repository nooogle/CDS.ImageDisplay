using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using CDS.ImageDisplay.Annotations;

namespace CDS.ImageDisplay.Demo.DemoForms.AnnotationsDemo;

/// <summary>
/// Full labelling-app demo with annotation list, title/notes editing, and JSON save/load.
/// </summary>
internal sealed partial class FormAnnotationsDetailed : Form
{
    private bool _updatingFields;

    /// <summary>
    /// Initializes a new instance of <see cref="FormAnnotationsDetailed"/>.
    /// </summary>
    public FormAnnotationsDetailed()
    {
        InitializeComponent();
    }

    /// <summary>Setup after the form has loaded.</summary>
    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);
        using var bitmap = BitmapGenerator.Make(new Size(800, 600));
        bitmapDisplayPanel.SetImage(bitmap);
        UpdateFieldsFromSelection();
    }

    /// <summary>The form has been resized.</summary>
    protected override void OnSizeChanged(EventArgs e)
    {
        base.OnSizeChanged(e);
        bitmapDisplayPanel.FitToWindowCentred();
    }

    // -----------------------------------------------------------------------
    // AnnotationManager events
    // -----------------------------------------------------------------------

    private void annotationManager_AnnotationCreated(object sender, AnnotationCreatedEventArgs e)
    {
        AddListViewItem(e.Annotation);
        SetStatus($"Created: {GeometryTypeName(e.Annotation)}");
    }

    private void annotationManager_AnnotationModified(object sender, AnnotationModifiedEventArgs e)
    {
        RefreshListViewItem(e.Annotation);
        SetStatus($"Modified: {GeometryTypeName(e.Annotation)} \"{e.Annotation.Title}\"");
    }

    private void annotationManager_AnnotationDeleted(object sender, AnnotationDeletedEventArgs e)
    {
        RemoveListViewItem(e.Annotation);
        UpdateFieldsFromSelection();
        SetStatus($"Deleted: {GeometryTypeName(e.Annotation)}");
    }

    private void annotationManager_AnnotationSelected(object sender, AnnotationSelectedEventArgs e)
    {
        SelectListViewItem(e.Annotation);
        UpdateFieldsFromSelection();
        SetStatus($"Selected: {GeometryTypeName(e.Annotation)} \"{e.Annotation.Title}\"");
    }

    private void annotationManager_AnnotationDeselected(object sender, EventArgs e)
    {
        listView.SelectedItems.Clear();
        UpdateFieldsFromSelection();
        SetStatus("Deselected");
    }

    // -----------------------------------------------------------------------
    // ListView management
    // -----------------------------------------------------------------------

    private void AddListViewItem(Annotation annotation)
    {
        var item = new ListViewItem(GeometryTypeName(annotation));
        item.SubItems.Add(annotation.Title);
        item.Tag = annotation;
        listView.Items.Add(item);
    }

    private void RefreshListViewItem(Annotation annotation)
    {
        foreach (ListViewItem item in listView.Items)
        {
            if (item.Tag != annotation) { continue; }
            item.Text = GeometryTypeName(annotation);
            item.SubItems[1].Text = annotation.Title;
            return;
        }
    }

    private void RemoveListViewItem(Annotation annotation)
    {
        foreach (ListViewItem item in listView.Items)
        {
            if (item.Tag != annotation) { continue; }
            listView.Items.Remove(item);
            return;
        }
    }

    private void SelectListViewItem(Annotation annotation)
    {
        foreach (ListViewItem item in listView.Items)
        {
            if (item.Tag != annotation) { continue; }
            item.Selected = true;
            item.EnsureVisible();
            return;
        }
    }

    // -----------------------------------------------------------------------
    // Title / Notes editors
    // -----------------------------------------------------------------------

    private void UpdateFieldsFromSelection()
    {
        _updatingFields = true;
        var ann = annotationManager.SelectedAnnotation;
        txtTitle.Text = ann?.Title ?? string.Empty;
        txtNotes.Text = ann?.Notes ?? string.Empty;
        txtTitle.Enabled = ann != null;
        txtNotes.Enabled = ann != null;
        _updatingFields = false;
    }

    private void txtTitle_TextChanged(object sender, EventArgs e)
    {
        if (_updatingFields) { return; }
        var ann = annotationManager.SelectedAnnotation;
        if (ann == null) { return; }
        ann.Title = txtTitle.Text;
        RefreshListViewItem(ann);
    }

    private void txtNotes_TextChanged(object sender, EventArgs e)
    {
        if (_updatingFields) { return; }
        var ann = annotationManager.SelectedAnnotation;
        if (ann == null) { return; }
        ann.Notes = txtNotes.Text;
    }

    // -----------------------------------------------------------------------
    // Buttons
    // -----------------------------------------------------------------------

    private void btnClearAll_Click(object sender, EventArgs e)
    {
        annotationManager.ClearAnnotations();
        listView.Items.Clear();
        UpdateFieldsFromSelection();
        SetStatus("Cleared all annotations");
    }

    private void btnSaveJson_Click(object sender, EventArgs e)
    {
        using var dlg = new SaveFileDialog
        {
            Title = "Save annotations",
            Filter = "JSON files (*.json)|*.json|All files (*.*)|*.*",
            DefaultExt = "json",
        };

        if (dlg.ShowDialog(this) != DialogResult.OK) { return; }

        string json = AnnotationSerializer.Serialize(annotationManager.Annotations);
        File.WriteAllText(dlg.FileName, json);
        SetStatus($"Saved {annotationManager.Annotations.Count} annotation(s) to {Path.GetFileName(dlg.FileName)}");
    }

    private void btnLoadJson_Click(object sender, EventArgs e)
    {
        using var dlg = new OpenFileDialog
        {
            Title = "Load annotations",
            Filter = "JSON files (*.json)|*.json|All files (*.*)|*.*",
        };

        if (dlg.ShowDialog(this) != DialogResult.OK) { return; }

        string json = File.ReadAllText(dlg.FileName);
        var loaded = AnnotationSerializer.DeserializeList(json);

        annotationManager.ClearAnnotations();
        listView.Items.Clear();

        foreach (var ann in loaded)
        {
            annotationManager.AddAnnotation(ann);
            AddListViewItem(ann);
        }

        SetStatus($"Loaded {loaded.Count} annotation(s) from {Path.GetFileName(dlg.FileName)}");
    }

    // -----------------------------------------------------------------------
    // Helpers
    // -----------------------------------------------------------------------

    private void SetStatus(string message) => statusLabel.Text = message;

    private static string GeometryTypeName(Annotation annotation)
    {
        string name = annotation.Geometry.GetType().Name;
        const string suffix = "AnnotationGeometry";
        return name.EndsWith(suffix, StringComparison.Ordinal) ? name[..^suffix.Length] : name;
    }
}
