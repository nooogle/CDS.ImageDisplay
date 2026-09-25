using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using AwesomeAssertions;
using CDS.ImageDisplay.WinForms.BitmapDisplay;

namespace UnitTests.BitmapDisplay;

/// <summary>
/// Tests for setting images on a <see cref="BitmapDisplayPanel"/> from non-UI threads.
/// </summary>
/// <remarks>
/// Each test runs on its own STA thread that plays the part of the UI thread, so the
/// WinForms synchronization context installed by the panel doesn't leak onto a test
/// runner thread.
/// </remarks>
[TestClass]
[TestCategory("Threading")]
public sealed class BitmapDisplayPanelThreadingTests
{
    private static readonly TimeSpan s_timeout = TimeSpan.FromSeconds(5);


    /// <summary>
    /// Before the handle exists InvokeRequired is false on every thread; the image must
    /// still be applied on the UI thread rather than on the calling worker thread.
    /// </summary>
    [TestMethod]
    public void SetImage_FromWorkerBeforeHandleCreated_IsAppliedOnUIThread()
    {
        RunOnUIThread(() =>
        {
            using var panel = new BitmapDisplayPanel();
            int uiThreadId = Environment.CurrentManagedThreadId;
            int eventThreadId = 0;
            panel.ImageSizeChanged += (_, _) => eventThreadId = Environment.CurrentManagedThreadId;

            RunOnWorker(() => SetImage(panel, 10, 20));
            PumpUntil(() => panel.ImageSize == new Size(10, 20));

            panel.IsHandleCreated.Should().BeFalse();
            panel.ImageSize.Should().Be(new Size(10, 20));
            eventThreadId.Should().Be(uiThreadId);
        });
    }


    /// <summary>
    /// Verifies the normal case: a worker image is applied once the UI thread pumps messages.
    /// </summary>
    [TestMethod]
    public void SetImage_FromWorkerAfterHandleCreated_IsApplied()
    {
        RunOnUIThread(() =>
        {
            using var panel = new BitmapDisplayPanel();
            _ = panel.Handle;

            RunOnWorker(() => SetImage(panel, 12, 8));
            PumpUntil(() => panel.ImageSize == new Size(12, 8));

            panel.ImageSize.Should().Be(new Size(12, 8));
        });
    }


    /// <summary>
    /// Verifies that when several frames arrive before the UI thread runs, the latest is shown.
    /// </summary>
    [TestMethod]
    public void SetImage_ManyFramesFromWorker_LatestFrameWins()
    {
        RunOnUIThread(() =>
        {
            using var panel = new BitmapDisplayPanel();
            _ = panel.Handle;

            RunOnWorker(() =>
            {
                for (int size = 1; size <= 20; size++)
                {
                    SetImage(panel, size, size);
                }
            });
            PumpUntil(() => panel.ImageSize == new Size(20, 20));

            panel.ImageSize.Should().Be(new Size(20, 20));
        });
    }


    /// <summary>
    /// An image set on the UI thread is newer than one a worker queued earlier, so the
    /// queued callback must not replace it with the older frame.
    /// </summary>
    [TestMethod]
    public void SetImage_FromUIThreadAfterWorker_IsNotOverwrittenByQueuedFrame()
    {
        RunOnUIThread(() =>
        {
            using var panel = new BitmapDisplayPanel();
            _ = panel.Handle;

            RunOnWorker(() => SetImage(panel, 10, 10));
            SetImage(panel, 30, 30);
            Pump(TimeSpan.FromMilliseconds(200));

            panel.ImageSize.Should().Be(new Size(30, 30));
        });
    }


    /// <summary>
    /// Verifies that a worker can clear the image.
    /// </summary>
    [TestMethod]
    public void ClearImage_FromWorker_ClearsImage()
    {
        RunOnUIThread(() =>
        {
            using var panel = new BitmapDisplayPanel();
            _ = panel.Handle;
            SetImage(panel, 10, 10);

            RunOnWorker(panel.ClearImage);
            PumpUntil(() => panel.ImageSize == Size.Empty);

            panel.ImageSize.Should().Be(Size.Empty);
            panel.DisplayImage.Should().BeNull();
        });
    }


    /// <summary>
    /// A camera thread may still be delivering frames while the form closes; this must
    /// not throw on the worker thread.
    /// </summary>
    [TestMethod]
    public void SetImage_FromWorkerAfterDispose_IsIgnoredWithoutThrowing()
    {
        RunOnUIThread(() =>
        {
            var panel = new BitmapDisplayPanel();
            _ = panel.Handle;
            panel.Dispose();

            Exception? workerException = null;
            RunOnWorker(() =>
            {
                try { SetImage(panel, 10, 10); }
                catch (Exception ex) { workerException = ex; }
            });
            Pump(TimeSpan.FromMilliseconds(100));

            workerException.Should().BeNull();
            panel.DisplayImage.Should().BeNull();
        });
    }


    /// <summary>
    /// A frame queued just before disposal must not be applied afterwards.
    /// </summary>
    [TestMethod]
    public void SetImage_QueuedFromWorkerThenDisposed_IsNotApplied()
    {
        RunOnUIThread(() =>
        {
            var panel = new BitmapDisplayPanel();
            _ = panel.Handle;

            RunOnWorker(() => SetImage(panel, 10, 10));
            panel.Dispose();
            Pump(TimeSpan.FromMilliseconds(100));

            panel.DisplayImage.Should().BeNull();
        });
    }


    /// <summary>
    /// Verifies that the UI thread still sets images immediately.
    /// </summary>
    [TestMethod]
    public void SetImage_FromUIThread_TakesImmediateEffect()
    {
        RunOnUIThread(() =>
        {
            using var panel = new BitmapDisplayPanel();

            SetImage(panel, 7, 9);

            panel.ImageSize.Should().Be(new Size(7, 9));
        });
    }


    private static void SetImage(BitmapDisplayPanel panel, int width, int height)
    {
        using var bitmap = new Bitmap(width, height, PixelFormat.Format24bppRgb);
        panel.SetImage(bitmap);
    }


    private static void RunOnWorker(Action action)
    {
        var thread = new Thread(() => action()) { IsBackground = true };
        thread.Start();
        thread.Join(s_timeout).Should().BeTrue("the worker should not block");
    }


    private static void PumpUntil(Func<bool> condition)
    {
        DateTime deadline = DateTime.UtcNow + s_timeout;

        while (!condition() && (DateTime.UtcNow < deadline))
        {
            Application.DoEvents();
            Thread.Sleep(5);
        }
    }


    private static void Pump(TimeSpan duration) => PumpUntil(Elapsed(duration));


    private static Func<bool> Elapsed(TimeSpan duration)
    {
        DateTime end = DateTime.UtcNow + duration;
        return () => DateTime.UtcNow >= end;
    }


    /// <summary>
    /// Runs <paramref name="test"/> on a dedicated STA thread and rethrows any failure.
    /// </summary>
    private static void RunOnUIThread(Action test)
    {
        Exception? failure = null;

        var thread = new Thread(() =>
        {
            try { test(); }
            catch (Exception ex) { failure = ex; }
        });

        thread.SetApartmentState(ApartmentState.STA);
        thread.IsBackground = true;
        thread.Start();
        thread.Join(TimeSpan.FromSeconds(30)).Should().BeTrue("the UI thread test should complete");

        if (failure != null)
        {
            System.Runtime.ExceptionServices.ExceptionDispatchInfo.Capture(failure).Throw();
        }
    }
}
