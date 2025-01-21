using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

namespace CDS.Imaging.WinForms.Draw
{
    /// <summary>
    /// Represents a pool of rendering tools that can be reused.
    /// </summary>
    /// <remarks>
    /// All resources are cached forever, so this class should be used with caution!
    /// An exceptions will be thrown if an attempt is made to acquire a resource
    /// from a non-UI thread
    /// </remarks>
    public class RenderingToolsPool
    {
        private static RenderingToolsPool instance = new RenderingToolsPool();

        private readonly Dictionary<LineSpec, Pen> penCache = new();
        private readonly Dictionary<BrushSpec, Brush> brushCache = new();
        private readonly Dictionary<FontSpec, Font> fontCache = new();


        /// <summary>
        /// Ensures the current thread is the UI thread.
        /// Throws an exception if called from a non-UI thread.
        /// </summary>
        public static void EnsureOnUIThread()
        {
            if (SynchronizationContext.Current == null || SynchronizationContext.Current.GetType() != typeof(WindowsFormsSynchronizationContext))
            {
                throw new InvalidOperationException("This method must be called from the UI thread.");
            }
        }

        /// <summary>
        /// Retrieves or creates a Pen based on the provided description.
        /// </summary>
        public static Pen GetPen(LineSpec description)
        {
            // TODO put this back!
            //EnsureOnUIThread();

            if (!instance.penCache.TryGetValue(description, out var pen))
            {
                pen = new Pen(description.Color, description.Width)
                {
                    DashStyle = description.DashStyle,
                    StartCap = description.StartCap,
                    EndCap = description.EndCap
                };

                instance.penCache[description] = pen;
            }

            return pen;
        }

        /// <summary>
        /// Retrieves or creates a Brush based on the provided description.
        /// </summary>
        public static Brush GetBrush(BrushSpec description)
        {
            // TODO put this back!
            //EnsureOnUIThread();

            if (!instance.brushCache.TryGetValue(description, out var brush))
            {
                brush = new SolidBrush(description.Color);
                instance.brushCache[description] = brush;
            }

            return brush;
        }


        /// <summary>
        /// Retrieves or creates a Font based on the provided description.
        /// </summary>
        public static Font GetFont(FontSpec description)
        {
            // TODO put this back!
            //EnsureOnUIThread();

            if (!instance.fontCache.TryGetValue(description, out var font))
            {
                font = new Font(description.FontName, description.FontSize);
                instance.fontCache[description] = font;
            }

            return font;
        }
    }
}
