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
            AddDemo(
                parentNode: treeView.Nodes,
                name: "Simple fit to window",
                tooltip: "Form with single image configure to always resize to fit to the window constraints",
                runDemo: () => RunModalForm<FormSimpleFitToWindow>());
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
