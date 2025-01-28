using System;

namespace CDS.Imaging.Draw
{
    /// <summary>
    /// Event arguments for when a layer node is double clicked.
    /// </summary>
    public class LayerTreeNodeEventArgs : EventArgs
    {
        /// <summary>
        /// The layer that was double clicked.
        /// </summary>
        public Layer? Layer { get; }


        /// <summary>
        /// Constructor
        /// </summary>
        public LayerTreeNodeEventArgs(Layer? layer)
        {
            Layer = layer;
        }
    }
}

