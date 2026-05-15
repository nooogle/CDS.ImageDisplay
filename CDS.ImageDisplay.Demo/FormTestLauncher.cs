using System;
using System.Windows.Forms;
using CDS.WinFormsMenus.Basic;

namespace CDS.ImageDisplay.Demo;

internal sealed partial class FormTestLauncher : Form
{
    private readonly JSONSettingsManager<AppSettings> _settingsManager;

    public FormTestLauncher()
    {
        InitializeComponent();
        _settingsManager = new JSONSettingsManager<AppSettings>();
    }

    protected override void OnFormClosing(FormClosingEventArgs e)
    {
        base.OnFormClosing(e);
        _settingsManager.Save();
    }

    private void FormTestLauncher_Load(object sender, EventArgs e) => AddDemos();

    private void AddDemos()
    {
        AddBasicsDemoNodes();
        AddOtherDemoNodes();
        AddOpenCVSharpDemoNodes();

        menuTree.ExpandAllGroups();
    }

    private void AddBasicsDemoNodes()
    {
        MenuGroup basics = menuTree.AddGroup("Basics");

        basics.AddItem(
            name: "No image",
            tooltip: "Form that shows what the bitmap display panel looks like when an image has not been assigned",
            parent: this,
            createForm: () => new DemoForms.FormNoImage());

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
        MenuGroup other = menuTree.AddGroup("Other");

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
            createForm: () => new DemoForms.OverlaysDemo.FormOverlays(_settingsManager.Settings.DemoForms.OverlaysDemo));
    }

    private void AddOpenCVSharpDemoNodes()
    {
        MenuGroup openCv = menuTree.AddGroup("OpenCV Sharp");

        openCv.AddItem(
            name: "Blurring",
            tooltip: "",
            parent: this,
            createForm: () => new DemoForms.FormOpenCVSharp());
    }
}

