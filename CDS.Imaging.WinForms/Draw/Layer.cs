namespace CDS.Imaging.Draw;

using CDS.Imaging.BitmapDisplay;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;


/// <summary>
/// A drawing layer containing shapes and child layers.
/// </summary>
[TypeConverter(typeof(ExpandableObjectConverter))]
public class Layer
{
    /// <summary>
    /// Simple representation of this instance
    /// </summary>
    public override string ToString() => Name + (Visible ? "" : " (Hidden)");


    /// <summary>
    /// The name of the layer
    /// </summary>
    public string Name { get; set; } = "Layer";


    /// <summary>
    /// Whether the layer is visible. This affects the visibility of all shapes and child layers, recursively.
    /// </summary>
    public bool Visible { get; set; } = true;


    /// <summary>
    /// The shapes on this layer.
    /// </summary>

/* Unmerged change from project 'CDS.Imaging.WinForms (net48)'
Before:
    public List<Shapes.IShape> Shapes { get; } = new List<Shapes.IShape>();
After:
    public List<IShape> Shapes { get; } = new List<IShape>();
*/
    public List<Draw.IShape> Shapes { get; } = new List<Draw.IShape>();


    /// <summary>
    /// The child layers of this layer.
    /// </summary>
    public List<Layer> ChildLayers { get; } = new List<Layer>();


    /// <summary>
    /// Draws the shapes on this layer and all visible child layers recursively.
    /// </summary>
    /// <param name="bitmapDisplay">The bitmap display panel.</param>
    /// <param name="graphics">The graphics object to draw with.</param>
    public void Draw(BitmapDisplay.BitmapDisplayPanel bitmapDisplay, Graphics graphics)
    {
        if (!Visible) { return; }

        DrawShapes(bitmapDisplay, graphics);
        DrawChildLayers(bitmapDisplay, graphics);
    }


    /// <summary>
    /// Draws the child layers of this layer.
    /// </summary>
    private void DrawChildLayers(BitmapDisplayPanel bitmapDisplay, Graphics graphics)
    {
        for (int i = 0; i < ChildLayers.Count; i++)
        {
            ChildLayers[i].Draw(bitmapDisplay, graphics);
        }
    }


    /// <summary>
    /// Draws the shapes on this layer.
    /// </summary>
    private void DrawShapes(BitmapDisplayPanel bitmapDisplay, Graphics graphics)
    {
        for (int i = 0; i < Shapes.Count; i++)
        {
            Shapes[i].Draw(bitmapDisplay, graphics);
        }
    }
}

