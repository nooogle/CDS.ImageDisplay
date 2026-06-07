using System.ComponentModel;
using System.Text.RegularExpressions;

namespace CDS.ImageDisplay.ImageBrowsing;

/// <summary>
/// A panel that displays a folder's image files as a thumbnail list.
/// </summary>
/// <remarks>
/// <para>
/// Uses <see cref="ListView"/> virtual mode so the control stays responsive with folders of
/// any size — including network or cloud-backed paths such as Dropbox. Only the file names are
/// enumerated up front; pixel data is never touched until an item scrolls into view.
/// </para>
/// <para>
/// Thumbnail loading is debounced: loading begins only after scrolling has been idle for
/// <see cref="DebounceDelayMs"/> milliseconds, so rapidly-scrolled items generate no I/O.
/// Loaded thumbnails are kept in a bounded LRU cache of up to <see cref="MaxCachedThumbnails"/>
/// entries. When the cache is full the least-recently-viewed thumbnail is evicted and its
/// slot is reused.
/// </para>
/// </remarks>
public partial class ImageListPanel : UserControl
{
    /// <summary>Default thumbnail edge length in pixels.</summary>
    private const int DefaultThumbnailHeight = 48;

    /// <summary>Default regex pattern — matches common image file extensions.</summary>
    private const string DefaultRegexPattern = @"(?i)\.(png|jpg|jpeg|bmp|gif|tif|tiff)$";

    /// <summary>Maximum number of decoded thumbnails kept in the LRU cache.</summary>
    public const int MaxCachedThumbnails = 400;

    /// <summary>Scroll-idle time in milliseconds before thumbnail loading begins.</summary>
    public const int DebounceDelayMs = 150;

    /// <summary>Number of items loaded beyond each edge of the visible range to pre-populate the cache for keyboard navigation.</summary>
    public const int PreloadOverrun = 5;

    // -----------------------------------------------------------------------
    // File list
    // -----------------------------------------------------------------------

    private readonly List<string> _files = [];
    private string _folder = string.Empty;
    private string _regexFilter = DefaultRegexPattern;

    // -----------------------------------------------------------------------
    // LRU thumbnail cache — maps file path to an ImageList slot index.
    //
    // Slot 0 is the placeholder.  Slots 1..N are allocated on demand up to
    // MaxCachedThumbnails, then reused in LRU order.
    // -----------------------------------------------------------------------

    private readonly Dictionary<string, int> _thumbnailSlots = [];
    private readonly LinkedList<string> _lruOrder = new();
    private readonly Dictionary<string, LinkedListNode<string>> _lruNodes = [];
    private int _nextSlot = 1;
    private Bitmap? _placeholder;

    // -----------------------------------------------------------------------
    // Scroll / load state
    // -----------------------------------------------------------------------

    private CancellationTokenSource? _cts;
    private int _pendingStart;
    private int _pendingEnd;
    private int _thumbnailHeight = DefaultThumbnailHeight;

    // -----------------------------------------------------------------------
    // Events
    // -----------------------------------------------------------------------

    /// <summary>Raised when the user selects a different item in the list.</summary>
    public event EventHandler<ImageFileEventArgs>? SelectionChanged;

    /// <summary>Raised when the user activates an item (double-click or Enter).</summary>
    public event EventHandler<ImageFileEventArgs>? ItemDoubleClicked;

    // -----------------------------------------------------------------------
    // Properties
    // -----------------------------------------------------------------------

    /// <summary>
    /// Gets or sets the regular expression used to filter files in the folder.
    /// Defaults to common image extensions (PNG, JPG, BMP, GIF, TIFF).
    /// Changing this property re-populates the list.
    /// </summary>
    [DefaultValue(DefaultRegexPattern)]
    public string RegexFilter
    {
        get => _regexFilter;
        set
        {
            if (_regexFilter == value) { return; }

            _regexFilter = value;
            RefreshList();
        }
    }

    /// <summary>
    /// Gets or sets the height (and width) of each thumbnail in pixels.
    /// Minimum 16. Defaults to 48. Changing this property flushes the cache and re-populates the list.
    /// </summary>
    [DefaultValue(DefaultThumbnailHeight)]
    public int ThumbnailHeight
    {
        get => _thumbnailHeight;
        set
        {
            int clamped = Math.Max(16, value);
            if (_thumbnailHeight == clamped) { return; }

            _thumbnailHeight = clamped;
            _imageList.ImageSize = new Size(_thumbnailHeight, _thumbnailHeight);
            RefreshList();
        }
    }

    /// <summary>
    /// Gets the full path of the currently selected file,
    /// or <see langword="null"/> if nothing is selected.
    /// </summary>
    [Browsable(false)]
    public string? SelectedFilePath
    {
        get
        {
            if (_listView.SelectedIndices.Count == 0) { return null; }

            int index = _listView.SelectedIndices[0];
            return index < _files.Count ? _files[index] : null;
        }
    }

    // -----------------------------------------------------------------------
    // Constructor
    // -----------------------------------------------------------------------

    /// <summary>Initialises a new instance of <see cref="ImageListPanel"/>.</summary>
    public ImageListPanel()
    {
        InitializeComponent();
    }

    // -----------------------------------------------------------------------
    // Public API
    // -----------------------------------------------------------------------

    /// <summary>
    /// Sets the folder whose image files are displayed and re-populates the list.
    /// </summary>
    /// <param name="folder">Full path to the folder to display.</param>
    public void SetFolder(string folder)
    {
        ArgumentNullException.ThrowIfNull(folder);
        _folder = folder;
        RefreshList();
    }

    /// <summary>Returns the full file paths of all items currently shown in the list.</summary>
    public IReadOnlyList<string> GetAllFilePaths() => _files.AsReadOnly();

    /// <summary>
    /// Selects the item at the given zero-based index and scrolls it into view.
    /// Does nothing if <paramref name="index"/> is out of range.
    /// </summary>
    public void SelectItemAt(int index)
    {
        if (index < 0 || index >= _files.Count) { return; }

        _listView.SelectedIndices.Clear();
        _listView.SelectedIndices.Add(index);
        _listView.EnsureVisible(index);
    }

    /// <summary>
    /// Selects the item immediately after the currently selected item.
    /// If nothing is selected, selects the first item.
    /// Does nothing if the list is empty or the last item is already selected.
    /// </summary>
    public void MoveToNext()
    {
        if (_files.Count == 0) { return; }

        int current = _listView.SelectedIndices.Count > 0 ? _listView.SelectedIndices[0] : -1;
        int next = current + 1;
        if (next >= _files.Count) { return; }

        _listView.SelectedIndices.Clear();
        _listView.SelectedIndices.Add(next);
        _listView.EnsureVisible(next);
    }

    /// <summary>
    /// Selects the item immediately before the currently selected item.
    /// Does nothing if the list is empty, nothing is selected, or the first item is already selected.
    /// </summary>
    public void MoveToPrevious()
    {
        if (_files.Count == 0 || _listView.SelectedIndices.Count == 0) { return; }

        int current = _listView.SelectedIndices[0];
        int prev = current - 1;
        if (prev < 0) { return; }

        _listView.SelectedIndices.Clear();
        _listView.SelectedIndices.Add(prev);
        _listView.EnsureVisible(prev);
    }

    // -----------------------------------------------------------------------
    // List management
    // -----------------------------------------------------------------------

    private void RefreshList()
    {
        ResetCacheState();

        _files.Clear();
        _listView.VirtualListSize = 0;

        if (!Directory.Exists(_folder)) { return; }

        try
        {
            var regex = new Regex(_regexFilter);
            _files.AddRange(
                Directory.EnumerateFiles(_folder)
                         .Where(f => regex.IsMatch(f))
                         .OrderBy(f => f, StringComparer.OrdinalIgnoreCase));
        }
        catch (Exception)
        {
            // Invalid regex or inaccessible folder — show nothing.
            return;
        }

        _listView.VirtualListSize = _files.Count;
        ScheduleVisibleLoad();
    }

    private void ScheduleVisibleLoad()
    {
        if (_files.Count == 0) { return; }

        int topIndex = _listView.TopItem?.Index ?? 0;
        int rowHeight = Math.Max(1, _thumbnailHeight + 6);
        int visibleCount = _listView.ClientSize.Height / rowHeight + 2;

        _pendingStart = topIndex;
        _pendingEnd = Math.Min(_files.Count - 1, topIndex + visibleCount - 1);
        _debounceTimer.Start();
    }

    private void ResetCacheState()
    {
        _cts?.Cancel();
        _cts?.Dispose();
        _cts = null;
        _debounceTimer.Stop();

        _thumbnailSlots.Clear();
        _lruOrder.Clear();
        _lruNodes.Clear();
        _nextSlot = 1;

        _imageList.Images.Clear();
        _placeholder?.Dispose();
        _placeholder = CreatePlaceholder();
        _imageList.Images.Add(_placeholder); // slot 0 — permanent placeholder
    }

    // -----------------------------------------------------------------------
    // LRU cache management
    // -----------------------------------------------------------------------

    /// <summary>
    /// Allocates an ImageList slot for a new thumbnail, evicting the LRU entry if at capacity.
    /// </summary>
    private int AllocateOrEvictSlot()
    {
        if (_nextSlot <= MaxCachedThumbnails)
        {
            _imageList.Images.Add(_placeholder!); // grow the list by one slot
            return _nextSlot++;
        }

        // Evict the least-recently-used entry and reuse its slot.
        string lruPath = _lruOrder.Last!.Value;
        int slot = _thumbnailSlots[lruPath];

        _thumbnailSlots.Remove(lruPath);
        _lruNodes.Remove(lruPath);
        _lruOrder.RemoveLast();

        _imageList.Images[slot] = _placeholder!; // reset so the evicted row shows a placeholder
        return slot;
    }

    private void AssignThumbnail(string filePath, Bitmap thumb)
    {
        int slot;

        if (_thumbnailSlots.TryGetValue(filePath, out int existingSlot))
        {
            slot = existingSlot;
            TouchLru(filePath);
        }
        else
        {
            slot = AllocateOrEvictSlot();
            _thumbnailSlots[filePath] = slot;
            _lruNodes[filePath] = _lruOrder.AddFirst(filePath);
        }

        _imageList.Images[slot] = thumb; // ImageList copies the pixels internally
    }

    private void TouchLru(string filePath)
    {
        if (!_lruNodes.TryGetValue(filePath, out var node)) { return; }

        _lruOrder.Remove(node);
        _lruOrder.AddFirst(node);
    }

    // -----------------------------------------------------------------------
    // Async thumbnail loading
    // -----------------------------------------------------------------------

    private void ScheduleLoad(int startIndex, int endIndex)
    {
        _cts?.Cancel();
        _cts?.Dispose();
        _cts = new CancellationTokenSource();
        _ = LoadRangeAsync(startIndex, endIndex, _cts.Token);
    }

    private async Task LoadRangeAsync(int startIndex, int endIndex, CancellationToken token)
    {
        for (int i = startIndex; i <= endIndex; i++)
        {
            if (token.IsCancellationRequested) { return; }

            string file = _files[i];
            if (_thumbnailSlots.ContainsKey(file)) { continue; } // already cached

            Bitmap? thumb = null;
            try
            {
                int size = _thumbnailHeight;
                thumb = await Task.Run(() => LoadThumbnail(file, size), token).ConfigureAwait(true);
            }
            catch (OperationCanceledException)
            {
                return;
            }
            catch (Exception)
            {
                continue; // unreadable file — leave as placeholder
            }

            if (token.IsCancellationRequested)
            {
                thumb?.Dispose();
                return;
            }

            AssignThumbnail(file, thumb);
            thumb.Dispose(); // ImageList copied the pixels; the managed wrapper is no longer needed
            _listView.RedrawItems(i, i, true);
        }
    }

    private static Bitmap LoadThumbnail(string filePath, int size)
    {
        using var original = new Bitmap(filePath);
        var thumb = new Bitmap(size, size);
        using var g = Graphics.FromImage(thumb);
        g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
        g.Clear(Color.Transparent);

        float scale = Math.Min((float)size / original.Width, (float)size / original.Height);
        int w = (int)(original.Width * scale);
        int h = (int)(original.Height * scale);
        int x = (size - w) / 2;
        int y = (size - h) / 2;
        g.DrawImage(original, x, y, w, h);

        return thumb;
    }

    private Bitmap CreatePlaceholder()
    {
        var bmp = new Bitmap(_thumbnailHeight, _thumbnailHeight);
        using var g = Graphics.FromImage(bmp);
        g.Clear(Color.LightGray);
        return bmp;
    }

    // -----------------------------------------------------------------------
    // ListView event handlers
    // -----------------------------------------------------------------------

    private void OnListViewRetrieveVirtualItem(object? sender, RetrieveVirtualItemEventArgs e)
    {
        string file = _files[e.ItemIndex];

        int imageIndex = 0;
        if (_thumbnailSlots.TryGetValue(file, out int slot))
        {
            imageIndex = slot;
            TouchLru(file); // protect recently-viewed items from eviction
        }

        e.Item = new ListViewItem(Path.GetFileName(file))
        {
            Tag = file,
            ImageIndex = imageIndex,
        };
    }

    private void OnListViewCacheVirtualItems(object? sender, CacheVirtualItemsEventArgs e)
    {
        // Called during scrolling with the index range the ListView is about to render.
        // Reset the debounce timer so loading only starts once scrolling settles.
        _pendingStart = e.StartIndex;
        _pendingEnd = e.EndIndex;
        _debounceTimer.Stop();
        _debounceTimer.Start();
    }

    private void OnDebounceTimerTick(object? sender, EventArgs e)
    {
        _debounceTimer.Stop();
        int start = Math.Max(0, _pendingStart - PreloadOverrun);
        int end = Math.Min(_files.Count - 1, _pendingEnd + PreloadOverrun);
        ScheduleLoad(start, end);
    }

    private void OnListViewSelectedIndexChanged(object? sender, EventArgs e)
    {
        var path = SelectedFilePath;
        if (path is not null)
        {
            SelectionChanged?.Invoke(this, new ImageFileEventArgs(path));
        }
    }

    private void OnListViewItemActivate(object? sender, EventArgs e)
    {
        var path = SelectedFilePath;
        if (path is not null)
        {
            ItemDoubleClicked?.Invoke(this, new ImageFileEventArgs(path));
        }
    }

    private void OnListViewSizeChanged(object? sender, EventArgs e)
    {
        if (_listView.Columns.Count > 0)
        {
            _listView.Columns[0].Width = _listView.ClientSize.Width;
        }
    }
}
