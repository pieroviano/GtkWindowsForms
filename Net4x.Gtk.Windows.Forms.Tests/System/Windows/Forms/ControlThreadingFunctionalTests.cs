using System.Threading;
using System.Windows.Forms;

namespace Net4x.Gtk.Windows.Forms.Tests.System.Windows.Forms;

/// <summary>Behavioural tests for <see cref="Control"/>'s threading contract. The WinForms idiom
/// <c>if (InvokeRequired) Invoke(...)</c> only works if InvokeRequired actually distinguishes threads and
/// Invoke actually blocks until the delegate has run and hands back its result (or its exception).</summary>
[Collection("NonParallelCollection")]
public class ControlThreadingFunctionalTests
{
    /// <summary>Runs <paramref name="work" /> on a thread that is definitely not the test thread.</summary>
    private static T OnADedicatedThread<T>(Func<T> work)
    {
        var result = default(T)!;
        var thread = new Thread(() => result = work());
        thread.Start();
        thread.Join();

        return result;
    }

    [Fact]
    public void InvokeRequired_IsFalseOnTheThreadThatOwnsTheControl()
    {
        using var panel = new Panel();

        Assert.False(panel.InvokeRequired, "no marshalling is needed on the owning thread");
    }

    [Fact]
    public void InvokeRequired_IsTrueOnAnotherThread()
    {
        using var panel = new Panel();

        var fromWorker = OnADedicatedThread(() => panel.InvokeRequired);

        Assert.True(fromWorker, "a worker thread must be told to marshal");
    }

    [Fact]
    public void Invoke_OnTheOwningThread_RunsInlineAndReturnsTheResult()
    {
        using var panel = new Panel();

        var result = panel.Invoke(new Func<string>(() => "computed"));

        Assert.Equal("computed", result);
    }

    [Fact]
    public void Invoke_RunsTheDelegateBeforeItReturns()
    {
        using var panel = new Panel();
        var ran = false;

        panel.Invoke(new Action(() => ran = true));

        Assert.True(ran, "Invoke must not return before the delegate has run");
    }

    [Fact]
    public void Invoke_PassesTheArgumentsThrough()
    {
        using var panel = new Panel();

        var result = panel.Invoke(new Func<int, int, int>((a, b) => a + b), 3, 4);

        Assert.Equal(7, result);
    }

    [Fact]
    public void Invoke_WithAnAction_RunsItSynchronously()
    {
        using var panel = new Panel();
        var ran = false;

        panel.Invoke(() => ran = true);

        Assert.True(ran, "the Action overload must run the delegate before returning");
    }

    [Fact]
    public void Invoke_Generic_ReturnsTheTypedResult()
    {
        using var panel = new Panel();

        var result = panel.Invoke(() => 42);

        Assert.Equal(42, result);
    }

    [Fact]
    public void Invoke_RethrowsTheDelegatesExceptionOnTheCaller()
    {
        using var panel = new Panel();

        var ex = Assert.Throws<InvalidOperationException>(
            () => panel.Invoke(new Action(() => throw new InvalidOperationException("boom"))));

        Assert.Equal("boom", ex.Message);
    }

    [Fact]
    public void Invoke_SetsAControlPropertyThatIsVisibleAfterwards()
    {
        using var panel = new Panel();

        panel.Invoke(new Action(() => panel.Text = "set from invoke"));

        Assert.Equal("set from invoke", panel.Text);
    }

    [Fact]
    public void BeginInvoke_ReturnsAPendingHandleWithoutRunningOnAPoolThread()
    {
        using var panel = new Panel();
        var ranOnThread = 0;

        var handle = panel.BeginInvoke(new Action(() => ranOnThread = Environment.CurrentManagedThreadId));

        Assert.NotNull(handle);
        // Without a running GTK main loop the work stays queued; what matters is that it was never
        // dispatched to an arbitrary thread-pool thread, which is what the old implementation did.
        Assert.Equal(0, ranOnThread);
    }

    [Fact]
    public void BeginInvoke_WithADelegate_AlsoReturnsAHandle()
    {
        using var panel = new Panel();

        var handle = panel.BeginInvoke(new Func<int>(() => 1), null);

        Assert.NotNull(handle);
    }

    [Fact]
    public void InvokeRequired_IsConsistentAcrossControls()
    {
        using var first = new Panel();
        using var second = new Button();

        Assert.Equal(first.InvokeRequired, second.InvokeRequired);

        var worker = OnADedicatedThread(() => (first.InvokeRequired, second.InvokeRequired));

        Assert.True(worker.Item1 && worker.Item2, "both controls must agree that a worker thread needs to marshal");
    }

    [Fact]
    public void Invoke_FromTheOwningThread_DoesNotDeadlock()
    {
        using var panel = new Panel();
        var completed = new ManualResetEventSlim(false);

        panel.Invoke(new Action(() => completed.Set()));

        Assert.True(completed.IsSet, "an inline invoke must complete without waiting on the main loop");
    }
}
