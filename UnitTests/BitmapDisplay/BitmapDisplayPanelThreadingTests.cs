using System.Drawing;
using System.Drawing.Imaging;
using AwesomeAssertions;
using CDS.ImageDisplay.WinForms.BitmapDisplay;

namespace UnitTests.BitmapDisplay;

/// <summary>
/// Tests for setting images on a <see cref="BitmapDisplayPanel"/> from non-UI threads.
/// </summary>
/// <remarks>
/// Each test runs on its own STA thread with a <see cref="QueuedSynchronizationContext"/>
/// standing in for the WinForms one (see <see cref="UIThreadHarness"/>), so the point at
/// which the UI thread applies a queued frame is chosen by the test rather than by a
/// message pump and a sleep. See <c>BitmapDisplayPanelConcurrencyTests</c> for the
/// genuinely-concurrent counterpart.
/// </remarks>
[TestClass]
[TestCategory("Threading")]
public sealed class BitmapDisplayPanelThreadingTests
{
    /// <summary>
    /// A frame from a worker must not be applied on the worker's own thread, and must be
    /// applied once the UI thread processes the posted callback.
    /// </summary>
    [TestMethod]
    public void SetImage_FromWorker_IsAppliedOnlyWhenUIThreadRunsCallback()
    {
        var context = new QueuedSynchronizationContext();

        UIThreadHarness.Run(context, () =>
        {
            using var panel = new BitmapDisplayPanel();
            _ = panel.Handle;

            UIThreadHarness.RunOnWorker(() => SetImage(panel, 12, 8));

            panel.ImageSize.Should().Be(Size.Empty, "the worker must not apply the image itself");
            context.PendingCount.Should().Be(1);

            context.RunAll();

            panel.ImageSize.Should().Be(new Size(12, 8));
        });
    }


    /// <summary>
    /// Before the handle exists <c>InvokeRequired</c> is false on every thread, so the panel
    /// relies on thread identity instead; the image must still be applied on the UI thread.
    /// </summary>
    [TestMethod]
    public void SetImage_FromWorkerBeforeHandleCreated_IsAppliedOnUIThread()
    {
        var context = new QueuedSynchronizationContext();

        UIThreadHarness.Run(context, () =>
        {
            using var panel = new BitmapDisplayPanel();
            int uiThreadId = Environment.CurrentManagedThreadId;
            int eventThreadId = 0;
            panel.ImageSizeChanged += (_, _) => eventThreadId = Environment.CurrentManagedThreadId;

            UIThreadHarness.RunOnWorker(() => SetImage(panel, 10, 20));
            context.RunAll();

            panel.IsHandleCreated.Should().BeFalse("the panel was never shown");
            panel.ImageSize.Should().Be(new Size(10, 20));
            eventThreadId.Should().Be(uiThreadId);
        });
    }


    /// <summary>
    /// However fast a producer runs, at most one callback may be outstanding, otherwise a
    /// camera thread would flood the message loop.
    /// </summary>
    [TestMethod]
    public void SetImage_ManyFramesFromWorker_CoalesceToASinglePost()
    {
        var context = new QueuedSynchronizationContext();

        UIThreadHarness.Run(context, () =>
        {
            using var panel = new BitmapDisplayPanel();
            _ = panel.Handle;

            UIThreadHarness.RunOnWorker(() =>
            {
                for (int size = 1; size <= 20; size++)
                {
                    SetImage(panel, size, size);
                }
            });

            context.PendingCount.Should().Be(1, "20 frames arrived before the UI thread ran any of them");
        });
    }


    /// <summary>
    /// When several frames arrive before the UI thread runs, the latest is the one displayed.
    /// </summary>
    [TestMethod]
    public void SetImage_ManyFramesFromWorker_LatestFrameWins()
    {
        var context = new QueuedSynchronizationContext();

        UIThreadHarness.Run(context, () =>
        {
            using var panel = new BitmapDisplayPanel();
            _ = panel.Handle;

            UIThreadHarness.RunOnWorker(() =>
            {
                for (int size = 1; size <= 20; size++)
                {
                    SetImage(panel, size, size);
                }
            });
            context.RunAll();

            panel.ImageSize.Should().Be(new Size(20, 20));
        });
    }


    /// <summary>
    /// Once the queued frame has been applied, the next worker frame must post again rather
    /// than assume a callback is still outstanding.
    /// </summary>
    [TestMethod]
    public void SetImage_FromWorkerAfterQueuedFrameApplied_PostsAgain()
    {
        var context = new QueuedSynchronizationContext();

        UIThreadHarness.Run(context, () =>
        {
            using var panel = new BitmapDisplayPanel();
            _ = panel.Handle;

            UIThreadHarness.RunOnWorker(() => SetImage(panel, 5, 5));
            context.RunAll();

            UIThreadHarness.RunOnWorker(() => SetImage(panel, 6, 6));
            context.PendingCount.Should().Be(1);
            context.RunAll();

            panel.ImageSize.Should().Be(new Size(6, 6));
            context.TotalPostCount.Should().Be(2);
        });
    }


    /// <summary>
    /// An image set on the UI thread is newer than one a worker queued earlier, so the queued
    /// callback must not replace it with the older frame.
    /// </summary>
    [TestMethod]
    public void SetImage_FromUIThreadAfterWorker_IsNotOverwrittenByQueuedFrame()
    {
        var context = new QueuedSynchronizationContext();

        UIThreadHarness.Run(context, () =>
        {
            using var panel = new BitmapDisplayPanel();
            _ = panel.Handle;

            UIThreadHarness.RunOnWorker(() => SetImage(panel, 10, 10));
            SetImage(panel, 30, 30);
            context.RunAll();

            panel.ImageSize.Should().Be(new Size(30, 30));
        });
    }


    /// <summary>
    /// When the frame can't be posted at all, it must be held and shown once the handle is
    /// created, rather than thrown away or reported as a failure to the producer.
    /// </summary>
    [TestMethod]
    public void SetImage_FromWorkerWhenPostFails_IsAppliedWhenHandleCreated()
    {
        var context = new QueuedSynchronizationContext();

        UIThreadHarness.Run(context, () =>
        {
            using var panel = new BitmapDisplayPanel();
            context.RefusePosts = true;

            UIThreadHarness.RunOnWorker(() => SetImage(panel, 16, 4));

            panel.ImageSize.Should().Be(Size.Empty, "there was nowhere to post the frame to");
            context.TotalPostCount.Should().Be(0);

            context.RefusePosts = false;
            _ = panel.Handle;

            panel.ImageSize.Should().Be(new Size(16, 4), "creating the handle applies the held frame");
        });
    }


    /// <summary>
    /// A callback posted to a context that was torn down before running it must not stop later
    /// frames from being posted once the panel has a live message loop again.
    /// </summary>
    [TestMethod]
    public void SetImage_AfterPostedCallbackNeverRan_PostsAgainOnceHandleCreated()
    {
        var context = new QueuedSynchronizationContext();

        UIThreadHarness.Run(context, () =>
        {
            using var panel = new BitmapDisplayPanel();

            UIThreadHarness.RunOnWorker(() => SetImage(panel, 10, 10));
            context.TotalPostCount.Should().Be(1);
            context.DiscardAll();

            _ = panel.Handle;

            UIThreadHarness.RunOnWorker(() => SetImage(panel, 11, 11));
            context.TotalPostCount.Should().Be(2, "posting must have been re-armed");
            context.RunAll();

            panel.ImageSize.Should().Be(new Size(11, 11));
        });
    }


    /// <summary>
    /// A worker can clear the image as well as set one.
    /// </summary>
    [TestMethod]
    public void ClearImage_FromWorker_ClearsImage()
    {
        var context = new QueuedSynchronizationContext();

        UIThreadHarness.Run(context, () =>
        {
            using var panel = new BitmapDisplayPanel();
            _ = panel.Handle;
            SetImage(panel, 10, 10);

            UIThreadHarness.RunOnWorker(panel.ClearImage);
            context.RunAll();

            panel.ImageSize.Should().Be(Size.Empty);
            panel.DisplayImage.Should().BeNull();
        });
    }


    /// <summary>
    /// A camera thread may still be delivering frames while the form closes; this must not
    /// throw on the worker thread.
    /// </summary>
    [TestMethod]
    public void SetImage_FromWorkerAfterDispose_IsIgnoredWithoutThrowing()
    {
        var context = new QueuedSynchronizationContext();

        UIThreadHarness.Run(context, () =>
        {
            var panel = new BitmapDisplayPanel();
            _ = panel.Handle;
            panel.Dispose();

            UIThreadHarness.RunOnWorker(() => SetImage(panel, 10, 10));
            context.RunAll();

            panel.DisplayImage.Should().BeNull();
        });
    }


    /// <summary>
    /// A frame queued just before disposal must not be applied afterwards.
    /// </summary>
    [TestMethod]
    public void SetImage_QueuedFromWorkerThenDisposed_IsNotApplied()
    {
        var context = new QueuedSynchronizationContext();

        UIThreadHarness.Run(context, () =>
        {
            var panel = new BitmapDisplayPanel();
            _ = panel.Handle;

            UIThreadHarness.RunOnWorker(() => SetImage(panel, 10, 10));
            panel.Dispose();
            context.RunAll();

            panel.DisplayImage.Should().BeNull();
        });
    }


    /// <summary>
    /// The UI thread still sets images synchronously, without going through the context.
    /// </summary>
    [TestMethod]
    public void SetImage_FromUIThread_TakesImmediateEffect()
    {
        var context = new QueuedSynchronizationContext();

        UIThreadHarness.Run(context, () =>
        {
            using var panel = new BitmapDisplayPanel();

            SetImage(panel, 7, 9);

            panel.ImageSize.Should().Be(new Size(7, 9));
            context.TotalPostCount.Should().Be(0);
        });
    }


    /// <summary>
    /// A worker frame is swapped in rather than copied, so the buffer it arrives in must carry
    /// the palette mode that was set on the panel while it was the spare.
    /// </summary>
    [TestMethod]
    public void GreyscalePaletteMode_SetBeforeWorkerFrame_AppliesToTheSwappedInImage()
    {
        var context = new QueuedSynchronizationContext();

        UIThreadHarness.Run(context, () =>
        {
            using var panel = new BitmapDisplayPanel();
            _ = panel.Handle;

            // Put a frame through the swap first, so the buffer the next worker frame lands in
            // is the one that was just on display rather than the untouched spare.
            UIThreadHarness.RunOnWorker(() => SetImage(panel, 8, 8, PixelFormat.Format8bppIndexed));
            context.RunAll();

            panel.GreyscalePaletteMode = GreyscalePaletteMode.Inverted;

            UIThreadHarness.RunOnWorker(() => SetImage(panel, 8, 8, PixelFormat.Format8bppIndexed));
            context.RunAll();

            panel.GreyscalePaletteMode.Should().Be(GreyscalePaletteMode.Inverted);
            panel.DisplayImage.Should().NotBeNull();
            panel.DisplayImage!.Palette.Entries[0].ToArgb().Should().Be(Color.White.ToArgb(),
                "the inverted palette maps 0 to white");
        });
    }


    private static void SetImage(BitmapDisplayPanel panel, int width, int height) =>
        SetImage(panel, width, height, PixelFormat.Format24bppRgb);


    private static void SetImage(BitmapDisplayPanel panel, int width, int height, PixelFormat pixelFormat)
    {
        using var bitmap = new Bitmap(width, height, pixelFormat);
        panel.SetImage(bitmap);
    }
}
