using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CDS.Imaging.Demo
{
    public partial class FormTestLauncher : Form
    {
        public FormTestLauncher()
        {
            InitializeComponent();
        }

        private void FormTestLauncher_Load(object sender, EventArgs e)
        {
            SetCaption();
            AddDemos();
        }

        private void AddDemos()
        {
            var simpleNode = treeView.Nodes.Add("Simple");

            AddDemo(
                parentNode: simpleNode.Nodes,
                name: "Fit to window",
                tooltip: "Form with single image configured to always resize to fit to the window constraints",
                runDemo: () => RunModalForm<FormSimpleFitToWindow>());

            AddDemo(
                parentNode: simpleNode.Nodes,
                name: "Actual size, centered",
                tooltip: "Form with single image configured to use 1:1 zoom and remain centered",
                runDemo: () => RunModalForm<FormSimpleActualSizeCentred>());

            treeView.ExpandAll();
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

        private void RunModalForm<T>() where T : Form, new()
        {
            using T form = new();
            form.ShowDialog(this);
        }

        private void SetCaption()
        {
            Text = $"{Application.ProductName} [{Application.ProductVersion}] Test launcher";
        }

        private void treeView_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            var action = e.Node.Tag as Action;
            action?.Invoke();
        }
    }
}
