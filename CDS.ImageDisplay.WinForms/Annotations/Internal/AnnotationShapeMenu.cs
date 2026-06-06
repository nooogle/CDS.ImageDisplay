using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using CDS.ImageDisplay.Annotations.Shapes;

namespace CDS.ImageDisplay.Annotations.Internal;

/// <summary>
/// Creates and shows the shape-selection <see cref="ContextMenuStrip"/> after a drag gesture.
/// </summary>
internal static class AnnotationShapeMenu
{
    /// <summary>
    /// Shows the menu below-right of <paramref name="clientPoint"/> (relative to <paramref name="parent"/>).
    /// <paramref name="callback"/> is invoked with the chosen descriptor, or <see langword="null"/> on cancel.
    /// </summary>
    internal static void Show(
        Control parent,
        Point clientPoint,
        IReadOnlyList<(IAnnotationShapeDescriptor Descriptor, float Confidence)> ranked,
        IAnnotationShapeDescriptor crosshairDescriptor,
        Action<IAnnotationShapeDescriptor?> callback)
    {
        var menu = new ContextMenuStrip();
        bool callbackInvoked = false;

        void invoke(IAnnotationShapeDescriptor? d)
        {
            if (!callbackInvoked)
            {
                callbackInvoked = true;
                callback(d);
            }
        }

        // Auto pick: highest confidence above threshold, falling back to polygon.
        IAnnotationShapeDescriptor autoDescriptor = SelectAutoDescriptor(ranked);

        Font boldFont = new(menu.Font, FontStyle.Bold);
        var autoItem = new ToolStripMenuItem($"Auto ({autoDescriptor.Name})") { Font = boldFont };
        autoItem.Click += (_, _) => invoke(autoDescriptor);
        menu.Items.Add(autoItem);
        menu.Items.Add(new ToolStripSeparator());

        // Non-polygon shapes in ranked order, then polygon at the bottom.
        foreach (var (d, _) in ranked.Where(r => r.Descriptor is not PolygonAnnotationDescriptor))
        {
            AddItem(menu, d, invoke);
        }

        var polygonEntry = ranked.FirstOrDefault(r => r.Descriptor is PolygonAnnotationDescriptor);
        if (polygonEntry.Descriptor != null) { AddItem(menu, polygonEntry.Descriptor, invoke); }

        menu.Items.Add(new ToolStripSeparator());
        AddItem(menu, crosshairDescriptor, invoke);

        menu.Closed += (_, args) =>
        {
            if (args.CloseReason != ToolStripDropDownCloseReason.ItemClicked) { invoke(null); }
            // Defer disposal: Closed fires inside SetVisibleCore, so disposing here would
            // free native resources while WinForms is still calling SetWindowLong on the handle.
            parent.BeginInvoke(() =>
            {
                boldFont.Dispose();
                menu.Dispose();
            });
        };

        menu.Show(parent, clientPoint);
    }

    private static IAnnotationShapeDescriptor SelectAutoDescriptor(
        IReadOnlyList<(IAnnotationShapeDescriptor Descriptor, float Confidence)> ranked)
    {
        if (ranked.Count > 0 && ranked[0].Confidence >= AnnotationShapeRecognizer.ConfidenceThreshold)
        {
            return ranked[0].Descriptor;
        }

        // Fall back to the polygon catch-all if present.
        var polygon = ranked.FirstOrDefault(r => r.Descriptor is PolygonAnnotationDescriptor);
        if (polygon.Descriptor != null) { return polygon.Descriptor; }

        return ranked.Count > 0 ? ranked[^1].Descriptor : ranked[0].Descriptor;
    }

    private static void AddItem(
        ContextMenuStrip menu,
        IAnnotationShapeDescriptor descriptor,
        Action<IAnnotationShapeDescriptor?> invoke)
    {
        var item = new ToolStripMenuItem(descriptor.Name) { ToolTipText = descriptor.Description };
        item.Click += (_, _) => invoke(descriptor);
        menu.Items.Add(item);
    }
}
