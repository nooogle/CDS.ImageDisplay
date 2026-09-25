using System;
using System.Threading;

namespace CDS.ImageDisplay.WinForms.Utils;

/// <summary>
/// Captures the UI thread's <see cref="SynchronizationContext"/> and thread identity so
/// that code running on any thread can tell whether it is on the UI thread and, if not,
/// post work to it.
/// </summary>
/// <remarks>
/// <para>
/// This is a more robust alternative to <c>Control.InvokeRequired</c>/<c>Control.BeginInvoke</c>:
/// </para>
/// <list type="bullet">
/// <item><c>InvokeRequired</c> returns <see langword="false"/> on <i>every</i> thread when the
/// control has no window handle (before it is shown, or after it is destroyed), so a
/// worker thread would wrongly conclude it is on the UI thread. <see cref="IsOnUIThread"/>
/// compares thread identity, which does not depend on a handle.</item>
/// <item><c>Control.BeginInvoke</c> throws when the control's handle does not exist or is being
/// destroyed. <see cref="TryPost"/> posts through the captured context, which is independent
/// of any single control's handle, and reports failure instead of throwing.</item>
/// </list>
/// <para>
/// Call <see cref="Capture()"/> on the UI thread; for a control the constructor is the natural
/// place, since WinForms installs a <c>WindowsFormsSynchronizationContext</c> when the first
/// control is created on a thread. All members are thread-safe.
/// </para>
/// </remarks>
public sealed class UIDispatcher
{
    private readonly object _lock = new();
    private SynchronizationContext? _context;
    private int _uiThreadId;


    /// <summary>
    /// True once <see cref="Capture()"/> has been called with a non-null context,
    /// i.e. work can be posted to the UI thread.
    /// </summary>
    public bool CanPost
    {
        get { lock (_lock) { return _context != null; } }
    }


    /// <summary>
    /// True if the calling thread is the captured UI thread.
    /// </summary>
    /// <remarks>
    /// Returns <see langword="false"/> if nothing has been captured yet.
    /// </remarks>
    public bool IsOnUIThread
    {
        get
        {
            int uiThreadId;
            lock (_lock) { uiThreadId = _uiThreadId; }
            return (uiThreadId != 0) && (uiThreadId == Environment.CurrentManagedThreadId);
        }
    }


    /// <summary>
    /// Captures the calling thread as the UI thread, together with its current
    /// <see cref="SynchronizationContext"/>. Must be called on the UI thread.
    /// </summary>
    public void Capture() => Capture(SynchronizationContext.Current);


    /// <summary>
    /// Captures the calling thread as the UI thread, together with the supplied context.
    /// Must be called on the UI thread.
    /// </summary>
    /// <param name="context">
    /// The context used to post work to the UI thread; may be <see langword="null"/>,
    /// in which case <see cref="TryPost"/> will return <see langword="false"/> until a
    /// later capture supplies one.
    /// </param>
    public void Capture(SynchronizationContext? context)
    {
        lock (_lock)
        {
            _uiThreadId = Environment.CurrentManagedThreadId;

            // Never downgrade from a usable context to none (e.g. a re-capture on a
            // thread whose context has been temporarily swapped out).
            if (context != null)
            {
                _context = context;
            }
        }
    }


    /// <summary>
    /// Posts <paramref name="action"/> to run asynchronously on the UI thread and
    /// returns immediately.
    /// </summary>
    /// <returns>
    /// <see langword="true"/> if the action was queued; <see langword="false"/> if no
    /// context has been captured or the context can no longer accept work (for example
    /// the UI thread's message loop has shut down). When <see langword="false"/> is
    /// returned the action will never run.
    /// </returns>
    public bool TryPost(Action action)
    {
        if (action == null) { throw new ArgumentNullException(nameof(action)); }

        SynchronizationContext? context;
        lock (_lock) { context = _context; }

        if (context == null)
        {
            return false;
        }

        try
        {
            context.Post(static state => ((Action)state!).Invoke(), action);
            return true;
        }
        catch (InvalidOperationException)
        {
            // The context's marshalling window no longer exists
            return false;
        }
        catch (ObjectDisposedException)
        {
            return false;
        }
    }
}
