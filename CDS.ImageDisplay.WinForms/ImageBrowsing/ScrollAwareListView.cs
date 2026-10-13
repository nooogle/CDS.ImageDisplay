namespace CDS.ImageDisplay.WinForms.ImageBrowsing;

/// <summary>
/// A <see cref="ListView"/> that reports when the user starts and stops dragging the vertical
/// scrollbar thumb, so callers can defer expensive work until the drag is released.
/// </summary>
internal sealed class ScrollAwareListView : ListView
{
    private const int WM_VSCROLL = 0x0115;
    private const int SB_THUMBTRACK = 5;
    private const int SB_ENDSCROLL = 8;

    /// <summary>Gets a value indicating whether the scrollbar thumb is currently being dragged.</summary>
    public bool IsThumbDragging { get; private set; }

    /// <summary>Raised when a scrollbar thumb drag begins.</summary>
    public event EventHandler? ThumbDragStarted;

    /// <summary>Raised when the scrollbar is released (the scroll gesture has ended).</summary>
    public event EventHandler? ScrollEnded;

    /// <inheritdoc/>
    protected override void WndProc(ref Message m)
    {
        if (m.Msg != WM_VSCROLL)
        {
            base.WndProc(ref m);
            return;
        }

        int code = (int)((long)m.WParam & 0xFFFF);
        if (code == SB_THUMBTRACK && !IsThumbDragging)
        {
            IsThumbDragging = true;
            ThumbDragStarted?.Invoke(this, EventArgs.Empty);
        }

        base.WndProc(ref m);

        if (code == SB_ENDSCROLL)
        {
            IsThumbDragging = false;
            ScrollEnded?.Invoke(this, EventArgs.Empty);
        }
    }
}
