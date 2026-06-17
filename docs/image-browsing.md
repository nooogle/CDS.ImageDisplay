# Image browsing

`ImageListPanel` is a `UserControl` that displays a folder's image files as a thumbnail list. It uses `ListView` virtual mode so it remains responsive with folders of any size (including network and cloud-backed paths).

```csharp
imageListPanel.SetFolder(@"C:\Photos");
imageListPanel.SelectionChanged += (_, e) => Console.WriteLine(e.FilePath);
imageListPanel.ItemDoubleClicked += (_, e) => OpenFile(e.FilePath);
```

## Thumbnail loading

Thumbnails are debounced and loaded asynchronously. Loading only begins after scrolling has been idle for `DebounceDelayMs` (150 ms), so rapidly-scrolled items generate no I/O. Up to `MaxCachedThumbnails` (400) decoded thumbnails are kept in an LRU cache; older entries are evicted when the cache is full.

## File filtering

The file list is built from the current folder using a configurable regex:

```csharp
imageListPanel.RegexFilter = @"(?i)\.(png|jpg)$";  // PNGs and JPEGs only
```

The default regex matches `.png .jpg .jpeg .bmp .gif .tif .tiff`.

## Custom file provider

Supply a `FileProvider` delegate to override how files are enumerated (e.g. recursive search, database-backed list):

```csharp
imageListPanel.FileProvider = (folder, regex) =>
    Directory.EnumerateFiles(folder, "*", SearchOption.AllDirectories)
             .Where(f => regex.IsMatch(f))
             .OrderBy(f => f, StringComparer.OrdinalIgnoreCase);
```

## Per-item status indicators

Attach a `StatusProvider` delegate to show a coloured row indicator next to each item. The host maps its own domain semantics to colours; the library only renders.

```csharp
imageListPanel.StatusProvider = filePath =>
{
    if (approvedFiles.Contains(filePath))
        return new ImageItemStatus(Color.LightGreen, "OK");
    if (rejectedFiles.Contains(filePath))
        return new ImageItemStatus(Color.LightCoral, "ERR");
    return null;   // no indicator
};
```

`ImageItemStatus` carries a `Color` (applied as a row background tint) and an optional `BadgeText` (prepended to the filename as `[BadgeText] name.jpg`).

When statuses change (e.g. after a batch operation), call `RefreshStatuses()` to repaint the visible rows without rebuilding the file list or flushing the thumbnail cache:

```csharp
approvedFiles.Add(filePath);
imageListPanel.RefreshStatuses();
```

When `StatusProvider` is `null` (the default), appearance and behaviour are unchanged.

## Navigation helpers

```csharp
imageListPanel.MoveToNext();
imageListPanel.MoveToPrevious();
imageListPanel.SelectItemAt(42);
string? path = imageListPanel.SelectedFilePath;
IReadOnlyList<string> all = imageListPanel.GetAllFilePaths();
```
