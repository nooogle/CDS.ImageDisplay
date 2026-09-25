namespace UnitTests.BitmapDisplay;

/// <summary>
/// A synchronization context that queues posted callbacks instead of running them, so a test
/// can count them and choose exactly when they run.
/// </summary>
internal sealed class QueuedSynchronizationContext : SynchronizationContext
{
    private readonly List<(SendOrPostCallback Callback, object? State)> _queue = [];
    private int _totalPostCount;


    /// <summary>
    /// How many callbacks are queued but not yet run.
    /// </summary>
    public int PendingCount
    {
        get { lock (_queue) { return _queue.Count; } }
    }


    /// <summary>
    /// How many callbacks have been posted over this context's lifetime, including ones that
    /// have since been run or discarded.
    /// </summary>
    public int TotalPostCount
    {
        get { lock (_queue) { return _totalPostCount; } }
    }


    /// <summary>
    /// When set, <see cref="Post"/> throws instead of queueing, standing in for a context whose
    /// marshalling window has gone — the failure <c>UIDispatcher.TryPost</c> reports as false.
    /// </summary>
    public bool RefusePosts { get; set; }


    public override void Post(SendOrPostCallback d, object? state)
    {
        if (RefusePosts)
        {
            throw new InvalidOperationException("the marshalling window no longer exists");
        }

        lock (_queue)
        {
            _queue.Add((d, state));
            _totalPostCount++;
        }
    }


    /// <summary>
    /// Runs and removes every queued callback. Callbacks posted while this is running are left
    /// queued, so a test can tell a re-post apart from the original.
    /// </summary>
    public void RunAll()
    {
        (SendOrPostCallback Callback, object? State)[] queued;

        lock (_queue)
        {
            queued = [.. _queue];
            _queue.Clear();
        }

        foreach ((SendOrPostCallback callback, object? state) in queued)
        {
            callback(state);
        }
    }


    /// <summary>
    /// Discards every queued callback without running it, standing in for a message loop that
    /// shut down before the work it was given could be processed.
    /// </summary>
    public void DiscardAll()
    {
        lock (_queue) { _queue.Clear(); }
    }
}
