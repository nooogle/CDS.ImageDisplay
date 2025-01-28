using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace CDS.Imaging.Draw
{
    /// <summary>
    /// A tree view for displaying layers.
    /// </summary>
    public partial class LayerTreeView : UserControl
    {
        /// <summary>
        /// Fired when a layer node is double clicked.
        /// </summary>
        [Category(CategoryStrings.CDS)]
        public event EventHandler<LayerTreeNodeEventArgs>? LayerTreeNodeDoubleClicked;


        /// <summary>
        /// Fired when a layer node is clicked.
        /// </summary>
        [Category(CategoryStrings.CDS)]
        public event EventHandler<LayerTreeNodeEventArgs>? LayerTreeNodeClicked;


        /// <summary>
        /// Fired when a layer node is selected
        /// </summary>
        [Category(CategoryStrings.CDS)]
        public event EventHandler<LayerTreeNodeEventArgs>? LayerTreeNodeSelected;


        /// <summary>
        /// Constructor
        /// </summary>
        public LayerTreeView()
        {
            InitializeComponent();
        }


        /// <summary>
        /// Set the root layer for the tree view.
        /// </summary>
        public void SetRootLayer(Layer rootLayer)
        {
            treeView.Nodes.Clear();

            // create a root note on the tree view
            TreeNode rootNode = treeView.Nodes.Add(rootLayer.ToString());
            ConfigureTreeNode(rootNode, rootLayer);

            // Recursively add child nodes
            AddChildNodes(rootNode, rootLayer);

            // Optionally expand the root node
            rootNode.Expand();
        }


        private void ConfigureTreeNode(TreeNode node, Layer layer)
        {
            node.Tag = layer;
        }

        void AddChildNodes(TreeNode parentNode, Layer parentLayer)
        {
            foreach (var childLayer in parentLayer.ChildLayers)
            {
                // Create a node for each child
                TreeNode childNode = new TreeNode(childLayer.Name);
                ConfigureTreeNode(childNode, childLayer);

                // Add this node to the parent
                parentNode.Nodes.Add(childNode);

                // Recurse to add grandchildren
                AddChildNodes(childNode, childLayer);
            }
        }


        /// <summary>
        /// User has double clicked a node in the tree view.
        /// </summary>
        private void treeView_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            var layer = e?.Node?.Tag as Layer;
            if (layer == null) { return; }

            LayerTreeNodeDoubleClicked?.Invoke(this, new LayerTreeNodeEventArgs(layer));
        }


        /// <summary>
        /// User has clicked a node in the tree view.
        /// </summary>
        private void treeView_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            var layer = e?.Node?.Tag as Layer;
            if (layer == null) { return; }

            LayerTreeNodeClicked?.Invoke(this, new LayerTreeNodeEventArgs(layer));
        }


        /// <summary>
        /// User has selected a node in the tree view.
        /// </summary>
        private void treeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            var layer = e?.Node?.Tag as Layer;
            if (layer == null) { return; }

            LayerTreeNodeSelected?.Invoke(this, new LayerTreeNodeEventArgs(layer));
        }
    }
}

