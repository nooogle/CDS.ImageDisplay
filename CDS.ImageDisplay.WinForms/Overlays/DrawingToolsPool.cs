using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

namespace CDS.ImageDisplay.Overlays;


/// <summary>
/// Represents a pool of drawing tools that can be reused.
/// </summary>
/// <remarks>
/// Each resource type (Pen, Brush, Font) is cached up to <see cref="MaxCacheSize"/> entries.
/// When the limit is reached the oldest entry is disposed and evicted.
/// All methods must be called from the UI thread.
/// </remarks>
public class DrawingToolsPool : IDisposable
{
    /// <summary>
    /// Maximum number of entries cached per resource type (Pen, Brush, Font).
    /// When this limit is reached the oldest entry is disposed and evicted.
    /// </summary>
    public const int MaxCacheSize = 100;

    private static readonly Lazy<DrawingToolsPool> instance =
        new(() => new DrawingToolsPool(), LazyThreadSafetyMode.ExecutionAndPublication);

    private readonly Dictionary<PenSpec, Pen> penCache = [];
    private readonly LinkedList<PenSpec> penOrder = new();

    private readonly Dictionary<BrushSpec, Brush> brushCache = [];
    private readonly LinkedList<BrushSpec> brushOrder = new();

    private readonly Dictionary<FontSpec, Font> fontCache = [];
    private readonly LinkedList<FontSpec> fontOrder = new();

    private DrawingToolsPool() { }

    /// <summary>
    /// Ensures the current thread is the UI thread.
    /// Throws an exception if called from a non-UI thread.
    /// </summary>
    public static void EnsureOnUIThread()
    {
        if (SynchronizationContext.Current is not WindowsFormsSynchronizationContext)
        {
            throw new InvalidOperationException("This method must be called from the UI thread.");
        }
    }

    /// <summary>
    /// Retrieves or creates a Pen based on the provided description.
    /// </summary>
    public static Pen GetPen(PenSpec description)
    {
        EnsureOnUIThread();

        if (!instance.Value.penCache.TryGetValue(description, out Pen? pen))
        {
            PenSpec key = description.Clone();
            pen = description.Create();
            EvictIfNeeded(instance.Value.penCache, instance.Value.penOrder);
            instance.Value.penCache[key] = pen;
            instance.Value.penOrder.AddLast(key);
        }

        return pen;
    }

    /// <summary>
    /// Retrieves or creates a Brush based on the provided description.
    /// </summary>
    public static Brush GetBrush(BrushSpec description)
    {
        EnsureOnUIThread();

        if (!instance.Value.brushCache.TryGetValue(description, out Brush? brush))
        {
            BrushSpec key = description.Clone();
            brush = description.Create();
            EvictIfNeeded(instance.Value.brushCache, instance.Value.brushOrder);
            instance.Value.brushCache[key] = brush;
            instance.Value.brushOrder.AddLast(key);
        }

        return brush;
    }

    /// <summary>
    /// Retrieves or creates a Font based on the provided description.
    /// </summary>
    public static Font GetFont(FontSpec description)
    {
        EnsureOnUIThread();

        if (!instance.Value.fontCache.TryGetValue(description, out Font? font))
        {
            FontSpec key = description.Clone();
            font = description.Create();
            EvictIfNeeded(instance.Value.fontCache, instance.Value.fontOrder);
            instance.Value.fontCache[key] = font;
            instance.Value.fontOrder.AddLast(key);
        }

        return font;
    }

    private static void EvictIfNeeded<TSpec, TResource>(Dictionary<TSpec, TResource> cache, LinkedList<TSpec> order)
        where TSpec : notnull
        where TResource : IDisposable
    {
        if (cache.Count >= MaxCacheSize)
        {
            TSpec oldest = order.First!.Value;
            cache[oldest].Dispose();
            cache.Remove(oldest);
            order.RemoveFirst();
        }
    }

    /// <summary>
    /// Clears all cached resources, disposing them.
    /// </summary>
    public static void Clear()
    {
        EnsureOnUIThread();

        foreach (Pen pen in instance.Value.penCache.Values)
        {
            pen.Dispose();
        }

        instance.Value.penCache.Clear();
        instance.Value.penOrder.Clear();

        foreach (Brush brush in instance.Value.brushCache.Values)
        {
            brush.Dispose();
        }

        instance.Value.brushCache.Clear();
        instance.Value.brushOrder.Clear();

        foreach (Font font in instance.Value.fontCache.Values)
        {
            font.Dispose();
        }

        instance.Value.fontCache.Clear();
        instance.Value.fontOrder.Clear();
    }

    /// <summary>
    /// Disposes all cached resources.
    /// </summary>
    public void Dispose() => Clear();
}
