using AwesomeAssertions;
using CDS.ImageDisplay.WinForms.ImageBrowsing;
using FlaUI.Core;
using FlaUI.Core.AutomationElements;
using FlaUI.Core.Capturing;
using FlaUI.Core.Definitions;
using FlaUI.Core.Input;
using FlaUI.Core.WindowsAPI;
using FlaUI.UIA3;
using Application = FlaUI.Core.Application; // disambiguate from System.Windows.Forms.Application

namespace UITests;

/// <summary>
/// Drives a real <see cref="ImageListPanel"/>, hosted in the <c>CDS.ImageDisplay.WinForms.TestHarness</c>
/// app, through various "page scroll" interactions and checks that every item left visible
/// afterward ends up with a real thumbnail rather than the placeholder tile.
/// </summary>
/// <remarks>
/// Regression coverage for a confirmed bug in <see cref="ImageListPanel"/>'s scroll-debounce
/// handling: <c>RedrawItems(i, i, true)</c> on an owner-data <c>ListView</c> re-issues a
/// <c>CacheVirtualItems</c> hint for whatever it just redrew, and <c>OnListViewCacheVirtualItems</c>
/// used to treat that echo exactly like a genuine scroll — overwriting the pending load range and
/// restarting the debounce timer. With fast-decoding thumbnails this was harmless (each echo
/// arrived well inside the still-pending debounce window and never actually fired); with large
/// source images — decode time exceeding <see cref="ImageListPanel.DebounceDelayMs"/> per item —
/// the debounce fired on that stale one-item hint mid-load, cancelling and restarting
/// <c>LoadRangeAsync</c> one item at a time for the whole page, at which point a genuine second
/// scroll (ordinary behaviour when browsing a slow-loading folder) could land on a hint that didn't
/// reach back far enough, permanently dropping whatever the interrupted load hadn't reached yet.
/// The fix tracks the range currently being loaded and ignores hints entirely inside it.
/// <para>
/// This is exploratory, end-to-end UI Automation rather than a unit test asserting the internal
/// mechanism directly, so it exercises the real control through several different "page scroll"
/// interactions — a large keyboard jump, rapid repeated jumps, the UI Automation scroll pattern,
/// a genuine mouse click in the scrollbar track (the interaction actually reported in the field),
/// and a second click partway through the first page's load.
/// </para>
/// <para>
/// The window runs maximized with small thumbnails (see <see cref="ThumbnailHeight"/>) so a single
/// page covers many more items, and <see cref="SampleImageFactory"/> generates large source images
/// so decode time realistically exceeds the debounce window — both conditions were necessary to
/// reproduce this with a fast local disk; a small window with fast-decoding thumbnails (the
/// original design of this suite) passed reliably even before the fix.
/// </para>
/// </remarks>
[TestClass]
public sealed class ImageListPanelScrollTests
{
    private const int ImageCount = 150; // large images (see SampleImageFactory) — fewer needed since decode time, not count, is what matters here
    private const int ThumbnailHeight = 16;
    private const int WindowWidth = 360; // restored size only — the window runs maximized
    private const int WindowHeight = 260;

    /// <summary>
    /// How long to wait for loading to settle before treating a still-visible placeholder as a
    /// failure. Generous on purpose: with large source images, decoding a full page can genuinely
    /// take a while, but a real drop from the suspected bug is permanent — nothing re-triggers a
    /// load for indices whose <c>CacheVirtualItems</c> hint got overwritten — so waiting longer
    /// cannot mask a real failure, only avoid a false one against slow-but-working loading.
    /// </summary>
    private static readonly TimeSpan SettleTimeout = TimeSpan.FromSeconds(90);

    private static readonly Color PlaceholderColor = Color.LightGray;

    private string? _sampleFolder;
    private Application? _app;
    private UIA3Automation? _automation;

    [TestCleanup]
    public void Cleanup()
    {
        try
        {
            _app?.Close();
        }
        catch (Exception)
        {
            // Best-effort cleanup — the process may already be gone.
        }

        _app?.Dispose();
        _automation?.Dispose();

        if (_sampleFolder is not null && Directory.Exists(_sampleFolder))
        {
            try
            {
                Directory.Delete(_sampleFolder, recursive: true);
            }
            catch (IOException)
            {
                // Best-effort cleanup — a file may still be memory-mapped by a lingering handle.
            }
        }
    }

    [TestMethod]
    [TestCategory("Repro")]
    [DataRow(VirtualKeyShort.NEXT, DisplayName = "PageDown")]
    [DataRow(VirtualKeyShort.END, DisplayName = "End")]
    public void LargeJump_SettlesOnNewPage_AllVisibleItemsGetThumbnails(VirtualKeyShort jumpKey)
    {
        // Arrange
        AutomationElement listView = LaunchHarnessAndWaitForInitialLoad();

        // Act — a single big jump, matching the field report of a page scroll.
        listView.Focus();
        Keyboard.Type(jumpKey);

        // Assert
        WaitAndAssertNoVisiblePlaceholders(listView);
    }

    [TestMethod]
    [TestCategory("Repro")]
    public void RapidRepeatedPageDown_ThenSettle_AllVisibleItemsGetThumbnails()
    {
        // Arrange
        AutomationElement listView = LaunchHarnessAndWaitForInitialLoad();

        // Act — several page-down jumps fired faster than DebounceDelayMs apart, so each new
        // CacheVirtualItems hint lands while the previous one is still pending. If ImageListPanel
        // ever reports more than one hint for what ends up being the same settled viewport, this
        // is the shape of interaction most likely to catch a dropped, un-overlapping fragment of it.
        listView.Focus();
        for (int i = 0; i < 8; i++)
        {
            Keyboard.Type(VirtualKeyShort.NEXT);
            Thread.Sleep(40); // well under DebounceDelayMs (150ms)
        }

        // Assert
        WaitAndAssertNoVisiblePlaceholders(listView);
    }

    [TestMethod]
    [TestCategory("Repro")]
    public void MouseScrollBarLargeIncrement_ThenSettle_AllVisibleItemsGetThumbnails()
    {
        // Arrange
        AutomationElement listView = LaunchHarnessAndWaitForInitialLoad();

        // Act — a scrollbar "large increment" is the mouse equivalent of a page scroll (clicking
        // the track below the thumb) and reaches the native ListView via WM_VSCROLL/SB_PAGEDOWN,
        // a different code path than the keyboard's VK_NEXT handling used by the other tests here.
        var scrollPattern = listView.Patterns.Scroll.Pattern;
        for (int i = 0; i < 5 && scrollPattern.VerticallyScrollable; i++)
        {
            scrollPattern.Scroll(ScrollAmount.NoAmount, ScrollAmount.LargeIncrement);
            Thread.Sleep(40); // well under DebounceDelayMs (150ms) — no time to settle between increments
        }

        // Assert
        WaitAndAssertNoVisiblePlaceholders(listView);
    }

    [TestMethod]
    [TestCategory("Repro")]
    public void ScrollBarTrackClick_PageDown_AllVisibleItemsGetThumbnails()
    {
        // Arrange
        AutomationElement listView = LaunchHarnessAndWaitForInitialLoad();
        string nameBeforeScroll = GetFirstVisibleItemName(listView);

        // Act — a genuine mouse click in the scrollbar track below the thumb: the exact interaction
        // reported in the field, and distinct from ScrollPattern.Scroll() above, which drives the
        // control through UI Automation's own provider rather than real mouse input.
        for (int i = 0; i < 5; i++)
        {
            Point clickPoint = GetScrollBarPageDownClickPoint(listView);
            Mouse.Click(clickPoint);
            Thread.Sleep(40); // well under DebounceDelayMs (150ms) — no time to settle between clicks
        }

        // Assert — the clicks must have actually scrolled the list; otherwise "no placeholders
        // visible" is a meaningless pass because nothing changed.
        string nameAfterScroll = GetFirstVisibleItemName(listView);
        nameAfterScroll.Should().NotBe(nameBeforeScroll,
            "the scrollbar-track clicks should have paged the list down — if the top visible item " +
            "is unchanged, the click point missed the scrollbar and this test proves nothing");

        WaitAndAssertNoVisiblePlaceholders(listView);
    }

    [TestMethod]
    [TestCategory("Repro")]
    public void ScrollBarTrackClick_ThenClickAgainMidLoad_AllVisibleItemsGetThumbnails()
    {
        // Arrange
        AutomationElement listView = LaunchHarnessAndWaitForInitialLoad();

        // Act — page down once, then page down again partway through the first page's (slow, with
        // large source images) load — an entirely ordinary way to browse a big folder, not a
        // contrived edge case. The second click's CacheVirtualItems hint is for a genuinely
        // different range, unlike the single-item hints ImageListPanel's own RedrawItems calls
        // generate — which the earlier single-click test showed self-correct because they stay
        // adjacent to the in-progress load. This one doesn't.
        Mouse.Click(GetScrollBarPageDownClickPoint(listView));

        // Wait for a demonstrably partial state — some visible items loaded, some still
        // placeholders — rather than guessing a sleep duration: decode time depends on machine
        // load and isn't worth pinning down precisely, and a fixed sleep either clicks again too
        // early (nothing loaded yet, page hasn't moved) or too late (the page already finished).
        int visibleCount = CountVisibleItems(listView);
        WaitUntil(
            () =>
            {
                int stillPlaceholder = VisibleItemIndicesStillShowingPlaceholder(listView).Count;
                return stillPlaceholder > 0 && stillPlaceholder < visibleCount;
            },
            SettleTimeout,
            "the page never reached a partially-loaded state to interrupt — loading finished (or never started) too fast or too slow for this check");

        Mouse.Click(GetScrollBarPageDownClickPoint(listView));

        // Assert
        WaitAndAssertNoVisiblePlaceholders(listView);
    }

    /// <summary>
    /// Finds a screen point inside the vertical scrollbar's "page down" region — below the thumb.
    /// The list always starts scrolled to the top, so any point comfortably below the track's top
    /// lands below the thumb regardless of its size; deliberately not the very bottom of the
    /// track (the down arrow) because a maximized window with an auto-hide taskbar leaves no gap
    /// there — that pixel belongs to the taskbar's reveal zone, not the app.
    /// </summary>
    private static Point GetScrollBarPageDownClickPoint(AutomationElement listView)
    {
        Rectangle listBounds = listView.BoundingRectangle;

        AutomationElement? scrollBar = listView.FindFirstChild(cf => cf.ByControlType(ControlType.ScrollBar));
        Rectangle trackBounds = scrollBar?.BoundingRectangle ?? listBounds;

        int x = scrollBar is not null
            ? trackBounds.X + trackBounds.Width / 2
            : listBounds.Right - SystemInformation.VerticalScrollBarWidth / 2 - 1;
        int y = trackBounds.Top + (int)(trackBounds.Height * 0.85); // well past the thumb, well clear of the bottom edge

        // Clamp inside the list's own rectangle no matter what the above computed: a DPI mismatch
        // between this test process and the (per-monitor-DPI-aware) target process, or a scrollbar
        // element reporting unreliable bounds, could otherwise send the click outside the app
        // window entirely — e.g. onto the desktop or taskbar — with unpredictable side effects.
        return new Point(
            Math.Clamp(x, listBounds.Left + 1, listBounds.Right - 1),
            Math.Clamp(y, listBounds.Top + 1, listBounds.Bottom - 1));
    }

    /// <summary>Returns the display text of the topmost currently-visible item, for detecting whether a scroll actually happened.</summary>
    private static string GetFirstVisibleItemName(AutomationElement listView)
    {
        Rectangle listBounds = listView.BoundingRectangle;
        AutomationElement[] items = listView.FindAllChildren(cf => cf.ByControlType(ControlType.ListItem));

        AutomationElement? topMost = items
            .Where(item => Rectangle.Intersect(listBounds, item.BoundingRectangle) is { Width: >= 4, Height: >= 4 })
            .OrderBy(item => item.BoundingRectangle.Y)
            .FirstOrDefault();

        topMost.Should().NotBeNull("at least one item should be visible");
        return topMost!.Name;
    }

    private AutomationElement LaunchHarnessAndWaitForInitialLoad()
    {
        _sampleFolder = SampleImageFactory.CreateFolder(ImageCount, imageSize: 8000);
        string args = $"\"{_sampleFolder}\" {WindowWidth} {WindowHeight} {ThumbnailHeight} true";
        _app = Application.Launch(HarnessLauncher.ResolveExePath(), args);
        _automation = new UIA3Automation();

        Window? window = _app.GetMainWindow(_automation, TimeSpan.FromSeconds(10));
        window.Should().NotBeNull("the harness app should have opened its main window within the timeout");
        AutomationElement listView = WaitForListView(window!);

        WaitUntil(
            () => VisibleItemIndicesStillShowingPlaceholder(listView).Count == 0,
            SettleTimeout,
            "the initial page failed to load — a harness/environment problem, not the bug under test");

        return listView;
    }

    /// <summary>
    /// Polls until every visible item has a real thumbnail or <see cref="SettleTimeout"/> elapses,
    /// then asserts none are left showing the placeholder. Polling (rather than a fixed sleep)
    /// tolerates slow, large-image decoding without weakening what a failure means: a genuine drop
    /// from the suspected bug is permanent, so it will still be non-empty however long we wait.
    /// </summary>
    private static void WaitAndAssertNoVisiblePlaceholders(AutomationElement listView)
    {
        List<int> stillPlaceholder = [];
        DateTime deadline = DateTime.UtcNow + SettleTimeout;
        while (DateTime.UtcNow < deadline)
        {
            stillPlaceholder = VisibleItemIndicesStillShowingPlaceholder(listView);
            if (stillPlaceholder.Count == 0)
            {
                return;
            }

            Thread.Sleep(250);
        }

        stillPlaceholder.Should().BeEmpty(
            "every item left visible after scrolling should have loaded its thumbnail instead of " +
            "still showing the gray placeholder tile — items at indices [{0}] still hadn't after waiting {1}",
            string.Join(", ", stillPlaceholder), SettleTimeout);
    }

    /// <summary>Counts items actually on screen, the same way <see cref="VisibleItemIndicesStillShowingPlaceholder"/> filters them.</summary>
    private static int CountVisibleItems(AutomationElement listView)
    {
        Rectangle listBounds = listView.BoundingRectangle;
        return listView.FindAllChildren(cf => cf.ByControlType(ControlType.ListItem))
            .Count(item => Rectangle.Intersect(listBounds, item.BoundingRectangle) is { Width: >= 4, Height: >= 4 });
    }

    private static AutomationElement WaitForListView(Window window)
    {
        AutomationElement? listView = null;
        WaitUntil(
            () => (listView = window.FindFirstDescendant(cf => cf.ByAutomationId("_listView"))) is not null,
            TimeSpan.FromSeconds(10),
            "could not find the ImageListPanel's internal ListView by AutomationId '_listView'");

        return listView!;
    }

    /// <summary>
    /// Checks only the items actually on screen. An owner-data <c>ListView</c> reports a computed
    /// <see cref="AutomationElement.BoundingRectangle"/> for every virtual item — including the
    /// hundreds currently scrolled off-window — so children must be filtered down to the ones
    /// overlapping the list's own bounds before screenshotting; otherwise the "sample" pixel comes
    /// from whatever happens to be on screen at that unrelated, often off-monitor, coordinate.
    /// </summary>
    private static List<int> VisibleItemIndicesStillShowingPlaceholder(AutomationElement listView)
    {
        Rectangle listBounds = listView.BoundingRectangle;
        AutomationElement[] items = listView.FindAllChildren(cf => cf.ByControlType(ControlType.ListItem));
        var result = new List<int>();

        for (int i = 0; i < items.Length; i++)
        {
            Rectangle itemBounds = items[i].BoundingRectangle;
            Rectangle visiblePart = Rectangle.Intersect(listBounds, itemBounds);
            if (visiblePart.Width < 4 || visiblePart.Height < 4)
            {
                continue; // scrolled off-window, or only a sliver clipped in view — nothing to check
            }

            using CaptureImage capture = Capture.Rectangle(visiblePart);
            Bitmap bitmap = capture.Bitmap;

            Color sample = bitmap.GetPixel(Math.Min(8, bitmap.Width - 1), bitmap.Height / 2);
            if (IsCloseTo(sample, PlaceholderColor))
            {
                result.Add(i);
            }
        }

        return result;
    }

    private static bool IsCloseTo(Color a, Color b, int tolerance = 8) =>
        Math.Abs(a.R - b.R) <= tolerance &&
        Math.Abs(a.G - b.G) <= tolerance &&
        Math.Abs(a.B - b.B) <= tolerance;

    private static void WaitUntil(Func<bool> condition, TimeSpan timeout, string failureMessage)
    {
        DateTime deadline = DateTime.UtcNow + timeout;
        while (DateTime.UtcNow < deadline)
        {
            if (condition())
            {
                return;
            }

            Thread.Sleep(250); // UIA calls are relatively expensive — avoid crowding out the target app's own message pump
        }

        condition().Should().BeTrue(failureMessage);
    }
}
