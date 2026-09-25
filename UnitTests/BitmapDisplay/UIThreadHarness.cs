using AwesomeAssertions;

namespace UnitTests.BitmapDisplay;

/// <summary>
/// Runs a test body on a dedicated STA thread that plays the part of the UI thread, with a
/// <see cref="QueuedSynchronizationContext"/> installed in place of the WinForms one.
/// </summary>
/// <remarks>
/// <para>
/// Controls capture whatever <see cref="SynchronizationContext.Current"/> is when they are
/// constructed, so installing a context the test controls makes cross-thread image updates
/// fully deterministic: the test decides exactly when the UI thread processes posted work,
/// and can count the posts. Pumping a real message loop with <c>Application.DoEvents</c> and
/// sleeps would cover the same code with timing-dependent assertions.
/// </para>
/// <para>
/// WinForms installs its own context when the first control is constructed on a thread, but
/// only over a context that is null or exactly <see cref="SynchronizationContext"/>; a derived
/// one is left alone. That is why nothing here touches
/// <c>WindowsFormsSynchronizationContext.AutoInstall</c> — it is process-wide, and tests run
/// with method-level parallelism, so switching it off would disable the real context under
/// whatever else happened to be constructing a control at the time.
/// </para>
/// </remarks>
internal static class UIThreadHarness
{
    private static readonly TimeSpan s_uiThreadTimeout = TimeSpan.FromSeconds(30);
    private static readonly TimeSpan s_workerTimeout = TimeSpan.FromSeconds(30);


    /// <summary>
    /// Runs <paramref name="test"/> on an STA thread with <paramref name="context"/> installed
    /// as that thread's synchronization context, and rethrows any failure on the caller.
    /// </summary>
    public static void Run(QueuedSynchronizationContext context, Action test)
    {
        Exception? failure = null;

        var thread = new Thread(() =>
        {
            try
            {
                SynchronizationContext.SetSynchronizationContext(context);
                test();
            }
            catch (Exception ex)
            {
                failure = ex;
            }
        });

        thread.SetApartmentState(ApartmentState.STA);
        thread.IsBackground = true;
        thread.Start();
        thread.Join(s_uiThreadTimeout).Should().BeTrue("the UI thread test should complete");

        Rethrow(failure);
    }


    /// <summary>
    /// Runs <paramref name="work"/> to completion on a worker thread, failing the test if it
    /// blocks — setting an image from a non-UI thread must always return promptly.
    /// </summary>
    public static void RunOnWorker(Action work)
    {
        Exception? failure = null;

        var thread = new Thread(() =>
        {
            try { work(); }
            catch (Exception ex) { failure = ex; }
        })
        { IsBackground = true };

        thread.Start();
        thread.Join(s_workerTimeout).Should().BeTrue("the worker should not block");

        Rethrow(failure);
    }


    private static void Rethrow(Exception? failure)
    {
        if (failure is not null)
        {
            System.Runtime.ExceptionServices.ExceptionDispatchInfo.Capture(failure).Throw();
        }
    }
}
