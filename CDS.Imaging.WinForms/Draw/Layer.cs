namespace CDS.Imaging.WinForms.Draw;

using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;

[TypeConverter(typeof(ExpandableObjectConverter))]
public class Layer
{
    public override string ToString() => Name + (Visible ? "" : " (Hidden)");

    public string Name { get; set; } = "Layer";

    public bool Visible { get; set; } = true;

    public List<WinForms.Draw.Shapes.IShapeOverlay> Shapes { get; } = new List<WinForms.Draw.Shapes.IShapeOverlay>();

    // New: List of child layers
    public List<Layer> ChildLayers { get; } = new List<Layer>();

    /// <summary>
    /// Draws the shapes on this layer and all visible child layers recursively.
    /// </summary>
    /// <param name="bitmapDisplay">The bitmap display panel.</param>
    /// <param name="graphics">The graphics object to draw with.</param>
    public void Draw(WinForms.BitmapDisplay.BitmapDisplayPanel bitmapDisplay, Graphics graphics)
    {
        if (!Visible) return;

        // Draw shapes on this layer
        for (int i = 0; i < Shapes.Count; i++)
        {
            Shapes[i].Draw(bitmapDisplay, graphics);
        }

        // Recursively draw child layers
        for (int i = 0; i < ChildLayers.Count; i++)
        {
            ChildLayers[i].Draw(bitmapDisplay, graphics);
        }
    }
}
