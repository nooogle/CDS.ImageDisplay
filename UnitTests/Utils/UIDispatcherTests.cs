using AwesomeAssertions;
using CDS.ImageDisplay.WinForms.Utils;

namespace UnitTests.Utils;

/// <summary>
/// Tests for <see cref="UIDispatcher"/>.
/// </summary>
[TestClass]
public sealed class UIDispatcherTests
{
    /// <summary>
    /// Verifies that nothing is considered the UI thread before a capture.
    /// </summary>
    [TestMethod]
    public void IsOnUIThread_BeforeCapture_ReturnsFalse()
    {
        var dispatcher = new UIDispatcher();

        dispatcher.IsOnUIThread.Should().BeFalse();
    }


    /// <summary>
    /// Verifies that the capturing thread is recognised as the UI thread.
    /// </summary>
    [TestMethod]
    public void IsOnUIThread_OnCapturingThread_ReturnsTrue()
    {
        var dispatcher = new UIDispatcher();

        dispatcher.Capture(new RecordingSynchronizationContext());

        dispatcher.IsOnUIThread.Should().BeTrue();
    }


    /// <summary>
    /// Verifies that a thread other than the capturing thread is not the UI thread.
    /// </summary>
    [TestMethod]
    public void IsOnUIThread_OnOtherThread_ReturnsFalse()
    {
        var dispatcher = new UIDispatcher();
        dispatcher.Capture(new RecordingSynchronizationContext());

        bool isOnUIThread = true;
        var thread = new Thread(() => isOnUIThread = dispatcher.IsOnUIThread);
        thread.Start();
        thread.Join();

        isOnUIThread.Should().BeFalse();
    }


    /// <summary>
    /// Verifies that capturing without a context still records the UI thread.
    /// </summary>
    [TestMethod]
    public void Capture_WithNullContext_RecordsThreadButCannotPost()
    {
        var dispatcher = new UIDispatcher();

        dispatcher.Capture(null);

        dispatcher.IsOnUIThread.Should().BeTrue();
        dispatcher.CanPost.Should().BeFalse();
    }


    /// <summary>
    /// Verifies that a later capture with no context doesn't discard a usable one.
    /// </summary>
    [TestMethod]
    public void Capture_WithNullContextAfterContext_KeepsExistingContext()
    {
        var dispatcher = new UIDispatcher();
        var context = new RecordingSynchronizationContext();
        dispatcher.Capture(context);

        dispatcher.Capture(null);

        dispatcher.TryPost(() => { }).Should().BeTrue();
        context.PostCount.Should().Be(1);
    }


    /// <summary>
    /// Verifies that a capture with no context doesn't move the UI thread identity away from
    /// the thread whose context is still the one posts go to.
    /// </summary>
    [TestMethod]
    public void Capture_WithNullContextOnAnotherThread_LeavesTheCapturedPairIntact()
    {
        var dispatcher = new UIDispatcher();
        var context = new RecordingSynchronizationContext();
        dispatcher.Capture(context);

        var thread = new Thread(() => dispatcher.Capture(null));
        thread.Start();
        thread.Join();

        dispatcher.IsOnUIThread.Should().BeTrue("the thread owning the captured context is still the UI thread");
        dispatcher.TryPost(() => { }).Should().BeTrue();
        context.PostCount.Should().Be(1);
    }


    /// <summary>
    /// Verifies that the parameterless capture uses the current thread's context.
    /// </summary>
    [TestMethod]
    public void Capture_Parameterless_UsesCurrentSynchronizationContext()
    {
        var dispatcher = new UIDispatcher();
        var context = new RecordingSynchronizationContext();
        SynchronizationContext? original = SynchronizationContext.Current;

        try
        {
            SynchronizationContext.SetSynchronizationContext(context);
            dispatcher.Capture();
        }
        finally
        {
            SynchronizationContext.SetSynchronizationContext(original);
        }

        dispatcher.TryPost(() => { }).Should().BeTrue();
        context.PostCount.Should().Be(1);
    }


    /// <summary>
    /// Verifies that posting fails when nothing has been captured.
    /// </summary>
    [TestMethod]
    public void TryPost_WithoutContext_ReturnsFalse()
    {
        var dispatcher = new UIDispatcher();

        dispatcher.TryPost(() => { }).Should().BeFalse();
        dispatcher.CanPost.Should().BeFalse();
    }


    /// <summary>
    /// Verifies that a posted action is queued on the captured context and runs when it's processed.
    /// </summary>
    [TestMethod]
    public void TryPost_WithContext_QueuesActionOnContext()
    {
        var dispatcher = new UIDispatcher();
        var context = new RecordingSynchronizationContext();
        dispatcher.Capture(context);
        bool ran = false;

        bool posted = dispatcher.TryPost(() => ran = true);

        posted.Should().BeTrue();
        ran.Should().BeFalse("posting is asynchronous");

        context.RunAll();
        ran.Should().BeTrue();
    }


    /// <summary>
    /// Verifies that posting from a non-UI thread reaches the captured context.
    /// </summary>
    [TestMethod]
    public void TryPost_FromOtherThread_QueuesActionOnContext()
    {
        var dispatcher = new UIDispatcher();
        var context = new RecordingSynchronizationContext();
        dispatcher.Capture(context);
        bool posted = false;

        var thread = new Thread(() => posted = dispatcher.TryPost(() => { }));
        thread.Start();
        thread.Join();

        posted.Should().BeTrue();
        context.PostCount.Should().Be(1);
    }


    /// <summary>
    /// Verifies that failures from a shut-down context are reported rather than thrown.
    /// </summary>
    [TestMethod]
    [DataRow(typeof(InvalidOperationException))]
    [DataRow(typeof(ObjectDisposedException))]
    public void TryPost_WhenContextThrows_ReturnsFalse(Type exceptionType)
    {
        var dispatcher = new UIDispatcher();
        dispatcher.Capture(new ThrowingSynchronizationContext(exceptionType));

        dispatcher.TryPost(() => { }).Should().BeFalse();
    }


    /// <summary>
    /// Verifies that a null action is rejected.
    /// </summary>
    [TestMethod]
    public void TryPost_NullAction_Throws()
    {
        var dispatcher = new UIDispatcher();

        Action act = () => dispatcher.TryPost(null!);

        act.Should().Throw<ArgumentNullException>();
    }


    /// <summary>
    /// Queues posted callbacks so tests can control when they run.
    /// </summary>
    private sealed class RecordingSynchronizationContext : SynchronizationContext
    {
        private readonly List<(SendOrPostCallback Callback, object? State)> _posted = [];

        public int PostCount
        {
            get { lock (_posted) { return _posted.Count; } }
        }

        public override void Post(SendOrPostCallback d, object? state)
        {
            lock (_posted) { _posted.Add((d, state)); }
        }

        public void RunAll()
        {
            (SendOrPostCallback Callback, object? State)[] posted;
            lock (_posted) { posted = [.. _posted]; _posted.Clear(); }

            foreach (var (callback, state) in posted)
            {
                callback(state);
            }
        }
    }


    /// <summary>
    /// Simulates a context whose message loop has gone away.
    /// </summary>
    private sealed class ThrowingSynchronizationContext(Type exceptionType) : SynchronizationContext
    {
        public override void Post(SendOrPostCallback d, object? state) =>
            throw (exceptionType == typeof(ObjectDisposedException)
                ? new ObjectDisposedException("context")
                : new InvalidOperationException());
    }
}
