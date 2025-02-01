using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace CDS.Imaging.Demo
{
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

            treeView.ExpandAll();
        }

        private void AddBasicsDemoNodes()
        {
            var basicsNode = treeView.Nodes.Add("Basics");

            AddDemo(
                parentNode: basicsNode.Nodes,
                name: "Fit to window",
                tooltip: "Form with single image configured to always resize to fit to the window constraints",
                runDemo: () => RunModalForm(new DemoForms.FormFitToWindow()));

            AddDemo(
                parentNode: basicsNode.Nodes,
                name: "Actual size, centered",
                tooltip: "Form with single image configured to use 1:1 zoom and remain centered",
                runDemo: () => RunModalForm(new DemoForms.FormActualSizeCentred()));

            AddDemo(
                parentNode: basicsNode.Nodes,
                name: "Free",
                tooltip: "Form with single image configured to allow the mouse to " +
                "drag (left-button) and zoom in and out (mouse wheel) of the image",
                runDemo: () => RunModalForm(new DemoForms.FormFree()));
        }

        private void AddOtherDemoNodes()
        {
            var otherNode = treeView.Nodes.Add("Other");

            AddDemo(
                parentNode: otherNode.Nodes,
                name: "Paint over and under",
                tooltip: "",
                runDemo: () => RunModalForm(new DemoForms.FormPaintOverAndUnder()));

            AddDemo(
                parentNode: otherNode.Nodes,
                name: "ROI selection",
                tooltip: "",
                runDemo: () => RunModalForm(new DemoForms.FormROISelection()));

            AddDemo(
                parentNode: otherNode.Nodes,
                name: "Multiple ROIs",
                tooltip: "",
                runDemo: () => RunModalForm(new DemoForms.MultipleROIs.FormMultipleROIs()));

            AddDemo(
                parentNode: otherNode.Nodes,
                name: "Overlays",
                tooltip: "Demonstrates how to use the overlays tools for drawing on top of an image using image-coordinates regarldess of the current pan and zoom",
                runDemo: () => RunModalForm(new DemoForms.OverlaysDemo.FormOverlays(settingsManager.Settings.DemoForms.OverlaysDemo)));
        }

        private void AddOpenCVSharpDemoNodes()
        {
            var node = treeView.Nodes.Add("OpenCV Sharp");

            AddDemo(
                parentNode: node.Nodes,
                name: "Blurring",
                tooltip: "",
                runDemo: () => RunModalForm(new DemoForms.FormOpenCVSharp()));
        }

        private void AddDemo(
            TreeNodeCollection parentNode,
            string name,
            string tooltip,
            Action runDemo)
        {
            var node = parentNode.Add(name);
            node.ToolTipText = tooltip;
            node.Tag = runDemo;
        }

        private void RunModalForm(Form form)
        {
            form.ShowDialog(this);
            form.Dispose();
        }

        private void treeView_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            var action = e.Node.Tag as Action;
            action?.Invoke();
        }

        private void treeView_KeyPress(object sender, KeyPressEventArgs e)
        {
            var isEnterKey = (e.KeyChar == '\r');
            if(!isEnterKey) { return; }

            var action = treeView.SelectedNode?.Tag as Action;
            action?.Invoke();
        }
    }
}
