using System;
using System.Windows.Forms;
using CDS.WinFormsMenus.Basic;

namespace CDS.Imaging.Demo;

public partial class FormTestLauncher : Form
{
    private JSONSettingsManager<AppSettings> settingsManager;

    public FormTestLauncher()
    {
        InitializeComponent();
        settingsManager = new JSONSettingsManager<AppSettings>();
    }

    protected override void OnFormClosing(FormClosingEventArgs e)
    {
        base.OnFormClosing(e);
        settingsManager.Save();
    }

    private void FormTestLauncher_Load(object sender, EventArgs e)
    {
        AddDemos();
    }

    private void AddDemos()
    {
        AddBasicsDemoNodes();
        AddOtherDemoNodes();
        AddOpenCVSharpDemoNodes();

        menuTree.ExpandAllGroups();
    }

    private void AddBasicsDemoNodes()
    {
        var basics = menuTree.AddGroup("Basics");

        basics.AddItem(
            name: "Fit to window",
            tooltip: "Form with single image configured to always resize to fit to the window constraints",
            parent: this,
            createForm: () => new DemoForms.FormFitToWindow());

        basics.AddItem(
            name: "Actual size, centered",
            tooltip: "Form with single image configured to use 1:1 zoom and remain centered",
            parent: this,
            createForm: () => new DemoForms.FormActualSizeCentred());

        basics.AddItem(
            name: "Free",
            tooltip: "Form with single image configured to allow the mouse to " +
            "drag (left-button) and zoom in and out (mouse wheel) of the image",
            parent: this,
            createForm: () => new DemoForms.FormFree());
    }

    private void AddOtherDemoNodes()
    {
        var other = menuTree.AddGroup("Other");

        other.AddItem(
            name: "Paint over and under",
            tooltip: "",
            parent: this,
            createForm: () => new DemoForms.FormPaintOverAndUnder());

        other.AddItem(
            name: "ROI selection",
            tooltip: "",
            parent: this,
            createForm: () => new DemoForms.FormROISelection());

        other.AddItem(
            name: "Multiple ROIs",
            tooltip: "",
            parent: this,
            createForm: () => new DemoForms.MultipleROIs.FormMultipleROIs());

        other.AddItem(
            name: "Overlays",
            tooltip: "Demonstrates how to use the overlays tools for drawing on top of an image using image-coordinates regardless of the current pan and zoom",
            parent: this,
            createForm: () => new DemoForms.OverlaysDemo.FormOverlays(settingsManager.Settings.DemoForms.OverlaysDemo));
    }

    private void AddOpenCVSharpDemoNodes()
    {
        var openCv = menuTree.AddGroup("OpenCV Sharp");

        openCv.AddItem(
            name: "Blurring",
            tooltip: "",
            parent: this,
            createForm: () => new DemoForms.FormOpenCVSharp());
    }
}
