using AwesomeAssertions;
using CDS.ImageDisplay.WinForms.ImageBrowsing;
using UnitTests.BitmapDisplay;

namespace UnitTests.ImageBrowsing;

/// <summary>
/// Tests for the <c>WM_VSCROLL</c> thumb-drag state machine in <see cref="ScrollAwareListView"/>.
/// </summary>
[TestClass]
public sealed class ScrollAwareListViewTests
{
    private const int WM_VSCROLL = 0x0115;
    private const int SB_LINEDOWN = 1;
    private const int SB_THUMBTRACK = 5;
    private const int SB_ENDSCROLL = 8;

    /// <summary>
    /// Verifies that repeated thumb-track messages raise <see cref="ScrollAwareListView.ThumbDragStarted"/> once.
    /// </summary>
    [TestMethod]
    [TestCategory("ScrollAwareListView")]
    public void WndProc_RepeatedThumbTrack_RaisesDragStartedOnce()
    {
        RunOnUIThread(listView =>
        {
            int startedCount = 0;
            listView.ThumbDragStarted += (_, _) => startedCount++;

            SendVerticalScroll(listView, SB_THUMBTRACK);
            SendVerticalScroll(listView, SB_THUMBTRACK);
            SendVerticalScroll(listView, SB_THUMBTRACK);

            startedCount.Should().Be(1);
            listView.IsThumbDragging.Should().BeTrue();
        });
    }

    /// <summary>
    /// Verifies that end-scroll clears the dragging state and raises <see cref="ScrollAwareListView.ScrollEnded"/>.
    /// </summary>
    [TestMethod]
    [TestCategory("ScrollAwareListView")]
    public void WndProc_EndScrollAfterThumbTrack_ClearsDraggingAndRaisesScrollEnded()
    {
        RunOnUIThread(listView =>
        {
            int endedCount = 0;
            listView.ScrollEnded += (_, _) => endedCount++;

            SendVerticalScroll(listView, SB_THUMBTRACK);
            SendVerticalScroll(listView, SB_ENDSCROLL);

            listView.IsThumbDragging.Should().BeFalse();
            endedCount.Should().Be(1);
        });
    }

    /// <summary>
    /// Verifies that a second drag after release raises <see cref="ScrollAwareListView.ThumbDragStarted"/> again.
    /// </summary>
    [TestMethod]
    [TestCategory("ScrollAwareListView")]
    public void WndProc_SecondDragAfterRelease_RaisesDragStartedAgain()
    {
        RunOnUIThread(listView =>
        {
            int startedCount = 0;
            listView.ThumbDragStarted += (_, _) => startedCount++;

            SendVerticalScroll(listView, SB_THUMBTRACK);
            SendVerticalScroll(listView, SB_ENDSCROLL);
            SendVerticalScroll(listView, SB_THUMBTRACK);

            startedCount.Should().Be(2);
            listView.IsThumbDragging.Should().BeTrue();
        });
    }

    /// <summary>
    /// Verifies that a scroll that is not a thumb drag does not report a drag.
    /// </summary>
    [TestMethod]
    [TestCategory("ScrollAwareListView")]
    public void WndProc_LineScroll_DoesNotRaiseDragStarted()
    {
        RunOnUIThread(listView =>
        {
            int startedCount = 0;
            listView.ThumbDragStarted += (_, _) => startedCount++;

            SendVerticalScroll(listView, SB_LINEDOWN);

            startedCount.Should().Be(0);
            listView.IsThumbDragging.Should().BeFalse();
        });
    }

    private static void RunOnUIThread(Action<ScrollAwareListView> test) =>
        UIThreadHarness.Run(new QueuedSynchronizationContext(), () =>
        {
            using var listView = new ScrollAwareListView();
            _ = listView.Handle;
            test(listView);
        });

    private static void SendVerticalScroll(ScrollAwareListView listView, int code) =>
        Win32SendMessage(listView.Handle, WM_VSCROLL, code, 0);

    [System.Runtime.InteropServices.DllImport("user32.dll", EntryPoint = "SendMessageW")]
    private static extern nint Win32SendMessage(nint hWnd, int msg, nint wParam, nint lParam);
}
