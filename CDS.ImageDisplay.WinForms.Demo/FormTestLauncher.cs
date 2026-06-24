using System;
using System.Windows.Forms;
using CDS.ImageDisplay.WinForms.Utils;
using CDS.WinFormsMenus.Basic;

namespace CDS.ImageDisplay.WinForms.Demo;

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
        AddTextPanelDemoNodes();
        AddAnnotationsDemoNodes();
        AddImageBrowsingDemoNodes();

        menuTree.ExpandAllGroups();
    }

    private void AddTextPanelDemoNodes()
    {
        MenuGroup textPanel = menuTree.AddGroup("Text panel");

        textPanel.AddItem(
            name: "Standard",
            tooltip: "Demonstration of the text panel using built-in type and drawing specs",
            parent: this,
            createForm: () => new DemoForms.FormTextPanelStandard());

        textPanel.AddItem(
            name: "Custom",
            tooltip: "Demonstration of the text panel using custom type and drawing specs",
            parent: this,
            createForm: () => new DemoForms.FormTextPanelCustom());
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
            name: "Greyscale palette modes",
            tooltip: "Three panels showing the same 8-bit image with Standard, Inverted, and Highlight Saturated palette modes",
            parent: this,
            createForm: () => new DemoForms.FormGreyscalePalette());

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
            name: "ROI selection: simple",
            tooltip: "Basic demo of ROI selection",
            parent: this,
            createForm: () => new DemoForms.FormROISelectionSimple());

        other.AddItem(
            name: "ROI selection: detailed",
            tooltip: "Detailed demo of ROI selection",
            parent: this,
            createForm: () => new DemoForms.FormROISelectionDetailed());

        other.AddItem(
            name: "Line selection: simple",
            tooltip: "Basic demo of line selection",
            parent: this,
            createForm: () => new DemoForms.FormLineSelectionSimple());

        other.AddItem(
            name: "Line selection: detailed",
            tooltip: "Detailed demo of line selection",
            parent: this,
            createForm: () => new DemoForms.FormLineSelectionDetailed());

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

        other.AddItem(
            name: "Overlay scale factor",
            tooltip: "Two panels share the same overlay rectangle defined in full-size coordinates. " +
                     "The right panel shows a quarter-size image with MapImageToDisplayScaleFactor = 0.25 " +
                     "so the overlay aligns correctly despite the downscaled image.",
            parent: this,
            createForm: () => new DemoForms.FormScaleFactorDemo());
    }

    private void AddAnnotationsDemoNodes()
    {
        MenuGroup annotations = menuTree.AddGroup("Annotations");

        annotations.AddItem(
            name: "Annotations: simple",
            tooltip: "Basic demo of the annotation manager — draw shapes on an image by clicking or dragging",
            parent: this,
            createForm: () => new DemoForms.AnnotationsDemo.FormAnnotationsSimple());

        annotations.AddItem(
            name: "Annotations: detailed",
            tooltip: "Full labelling-app demo with annotation list, title/notes editing, and JSON save/load",
            parent: this,
            createForm: () => new DemoForms.AnnotationsDemo.FormAnnotationsDetailed());
    }

    private void AddImageBrowsingDemoNodes()
    {
        MenuGroup browsing = menuTree.AddGroup("Image browsing");

        browsing.AddItem(
            name: "Image list",
            tooltip: "Browse a folder of images with thumbnail previews; use ◀ ▶ or click to step through them",
            parent: this,
            createForm: () => new DemoForms.ImageBrowsingDemo.FormImageBrowsing());

        browsing.AddItem(
            name: "Status indicators",
            tooltip: "Demonstrates StatusProvider — each item gets a randomised colour badge; click Randomise to reshuffle",
            parent: this,
            createForm: () => new DemoForms.ImageBrowsingDemo.FormImageBrowsingStatus());
    }
}

