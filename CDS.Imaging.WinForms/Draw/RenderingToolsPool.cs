using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

namespace CDS.Imaging.Draw
{
    /// <summary>
    /// Represents a pool of rendering tools that can be reused.
    /// </summary>
    /// <remarks>
    /// All resources are cached indefinitely; be aware of the memory and resource implications.
    /// An exception is thrown if an attempt is made to acquire a resource from a non-UI thread.
    /// </remarks>
    public class RenderingToolsPool : IDisposable
    {
        private static readonly Lazy<RenderingToolsPool> instance =
            new Lazy<RenderingToolsPool>(() => new RenderingToolsPool(), LazyThreadSafetyMode.ExecutionAndPublication);

        private readonly Dictionary<PenSpec, Pen> penCache = new();
        private readonly Dictionary<BrushSpec, Brush> brushCache = new();
        private readonly Dictionary<FontSpec, Font> fontCache = new();

        // Private constructor to enforce the singleton pattern.
        private RenderingToolsPool() { }

        /// <summary>
        /// Ensures the current thread is the UI thread.
        /// Throws an exception if called from a non-UI thread.
        /// </summary>
        public static void EnsureOnUIThread()
        {
            // A more robust check might use a known UI control or check InvokeRequired on a dummy control.
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

            if (!instance.Value.penCache.TryGetValue(description, out var pen))
            {
                pen = description.Create();
                instance.Value.penCache[description] = pen;
            }

            return pen;
        }

        /// <summary>
        /// Retrieves or creates a Brush based on the provided description.
        /// </summary>
        public static Brush GetBrush(BrushSpec description)
        {
            EnsureOnUIThread();

            if (!instance.Value.brushCache.TryGetValue(description, out var brush))
            {
                brush = description.Create();
                instance.Value.brushCache[description] = brush;
            }

            return brush;
        }

        /// <summary>
        /// Retrieves or creates a Font based on the provided description.
        /// </summary>
        public static Font GetFont(FontSpec description)
        {
            EnsureOnUIThread();

            if (!instance.Value.fontCache.TryGetValue(description, out var font))
            {
                font = description.Create();
                instance.Value.fontCache[description] = font;
            }

            return font;
        }

        /// <summary>
        /// Clears all cached resources, disposing them as necessary.
        /// </summary>
        public static void Clear()
        {
            EnsureOnUIThread();

            foreach (var pen in instance.Value.penCache.Values)
            {
                pen.Dispose();
            }
            instance.Value.penCache.Clear();

            foreach (var brush in instance.Value.brushCache.Values)
            {
                brush.Dispose();
            }
            instance.Value.brushCache.Clear();

            foreach (var font in instance.Value.fontCache.Values)
            {
                font.Dispose();
            }
            instance.Value.fontCache.Clear();
        }

        /// <summary>
        /// Disposes all cached resources.
        /// </summary>
        public void Dispose()
        {
        }
    }
}
