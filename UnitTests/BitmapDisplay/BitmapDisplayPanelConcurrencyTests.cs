using System.Drawing;
using System.Drawing.Imaging;
using AwesomeAssertions;
using CDS.ImageDisplay.WinForms.BitmapDisplay;

namespace UnitTests.BitmapDisplay;

/// <summary>
/// Runs a producer thread against the UI thread at the same time, so the producer's copy into
/// the spare buffer genuinely overlaps the UI thread's swap, palette change and disposal.
/// </summary>
/// <remarks>
/// <para>
/// <see cref="BitmapDisplayPanelThreadingTests"/> covers the same state machine
/// deterministically but, by design, never lets the two threads touch the panel at once.
/// These tests are the opposite trade: they assert little and can only ever show the absence
/// of a failure, but they are the only place the locking is put under real contention.
/// </para>
/// <para>
/// The UI thread drains posted work through <see cref="QueuedSynchronizationContext.RunAll"/>
/// rather than <c>Application.DoEvents</c>. <c>DoEvents</c> processes messages until the queue
/// is empty, which a producer posting flat out can prevent indefinitely — it livelocks the
/// caller instead of returning. <c>RunAll</c> runs a snapshot of what is already queued, so
/// the UI thread always makes progress however fast the producer runs.
/// </para>
/// </remarks>
[TestClass]
[TestCategory("Concurrency")]
public sealed class BitmapDisplayPanelConcurrencyTests
{
    private static readonly TimeSpan s_contentionTime = TimeSpan.FromSeconds(2);


    /// <summary>
    /// A producer running flat out against a UI thread that is itself applying frames, setting
    /// images and changing the palette must not corrupt state, throw, or stall the producer.
    /// </summary>
    [TestMethod]
    public void ProducerAgainstBusyUIThread_KeepsRunningAndEndsConsistent()
    {
        var context = new QueuedSynchronizationContext();

        UIThreadHarness.Run(context, () =>
        {
            using var panel = new BitmapDisplayPanel();
            _ = panel.Handle;

            using var producer = FrameProducer.Start(panel);

            var random = new Random(Seed: 20260925);
            DateTime deadline = DateTime.UtcNow + s_contentionTime;
            while (DateTime.UtcNow < deadline)
            {
                context.RunAll();

                _ = panel.ImageSize;

                if (random.Next(20) == 0)
                {
                    panel.GreyscalePaletteMode = (GreyscalePaletteMode)random.Next(3);
                }

                if (random.Next(100) == 0)
                {
                    SetImage(panel, 100, 100, PixelFormat.Format24bppRgb);
                }
            }

            producer.StopAndVerify();
            context.RunAll();

            producer.FrameCount.Should().BeGreaterThan(100, "the producer should have run freely");
            panel.DisplayImage.Should().NotBeNull();
            panel.DisplayImage!.Size.Should().Be(panel.ImageSize,
                "the swapped-in buffer and the virtual display must agree");
        });
    }


    /// <summary>
    /// Disposing the panel while a producer is mid-frame must not throw on either thread, and
    /// must leave no image behind — the shape of a form closing while a camera still delivers.
    /// </summary>
    [TestMethod]
    public void DisposeWhileProducerRunning_IsSafeAndDropsTheImage()
    {
        var context = new QueuedSynchronizationContext();

        UIThreadHarness.Run(context, () =>
        {
            for (int iteration = 0; iteration < 50; iteration++)
            {
                var panel = new BitmapDisplayPanel();

                // Alternate, because a panel with no handle reaches the UI thread through the
                // context captured in the constructor rather than one recaptured on creation.
                if ((iteration % 2) == 0)
                {
                    _ = panel.Handle;
                }

                using var producer = FrameProducer.Start(panel);
                producer.WaitForFirstFrame();
                context.RunAll();

                panel.Dispose();
                context.RunAll();

                producer.StopAndVerify($"iteration {iteration}");
                context.RunAll();

                panel.DisplayImage.Should().BeNull($"iteration {iteration}");
            }
        });
    }


    private static void SetImage(BitmapDisplayPanel panel, int width, int height, PixelFormat pixelFormat)
    {
        using var bitmap = new Bitmap(width, height, pixelFormat);
        panel.SetImage(bitmap);
    }


    /// <summary>
    /// A thread pushing frames of varying size and pixel format at a panel until stopped,
    /// capturing anything it throws so the UI thread can fail the test with it.
    /// </summary>
    private sealed class FrameProducer : IDisposable
    {
        private static readonly TimeSpan s_joinTimeout = TimeSpan.FromSeconds(30);

        private readonly ManualResetEventSlim _stop = new();
        private readonly ManualResetEventSlim _firstFrameSent = new();
        private readonly Thread _thread;
        private Exception? _failure;
        private int _frameCount;


        private FrameProducer(BitmapDisplayPanel panel)
        {
            _thread = new Thread(() => Run(panel)) { IsBackground = true };
        }


        /// <summary>
        /// How many frames the producer has pushed.
        /// </summary>
        public int FrameCount => Volatile.Read(ref _frameCount);


        public static FrameProducer Start(BitmapDisplayPanel panel)
        {
            var producer = new FrameProducer(panel);
            producer._thread.Start();
            return producer;
        }


        /// <summary>
        /// Blocks until at least one frame has been pushed, so the panel is genuinely being
        /// written to before the UI thread does whatever the test is about.
        /// </summary>
        public void WaitForFirstFrame() =>
            _firstFrameSent.Wait(s_joinTimeout).Should().BeTrue("the producer should have sent a frame");


        /// <summary>
        /// Signals the producer to stop, waits for it, and fails the test if it blocked or threw.
        /// </summary>
        public void StopAndVerify() => StopAndVerify(string.Empty);


        /// <summary>
        /// Signals the producer to stop, waits for it, and fails the test if it blocked or threw.
        /// </summary>
        /// <param name="because">Context added to the failure message.</param>
        public void StopAndVerify(string because)
        {
            _stop.Set();
            _thread.Join(s_joinTimeout).Should().BeTrue($"the producer must never block on the UI thread {because}");

            if (_failure is not null)
            {
                throw new InvalidOperationException($"the producer threw {because}", _failure);
            }
        }


        public void Dispose()
        {
            _stop.Set();
            _thread.Join(s_joinTimeout);
            _stop.Dispose();
            _firstFrameSent.Dispose();
        }


        private void Run(BitmapDisplayPanel panel)
        {
            var random = new Random(Seed: 42);

            try
            {
                while (!_stop.IsSet)
                {
                    PixelFormat pixelFormat = (random.Next(3) == 0)
                        ? PixelFormat.Format8bppIndexed
                        : PixelFormat.Format24bppRgb;

                    SetImage(panel, random.Next(1, 64), random.Next(1, 64), pixelFormat);

                    Interlocked.Increment(ref _frameCount);
                    _firstFrameSent.Set();
                }
            }
            catch (Exception ex)
            {
                _failure = ex;
                _firstFrameSent.Set();
            }
        }
    }
}
