#pragma warning disable CS0612

using System.Windows.Forms;

namespace AutoClosingMessageBox.Tests;

/// <summary>
/// Behavioural tests for <see cref="AutoClosingMessageBox" />' configuration and instancing.
/// <para>
/// The suite's other test drives the sample application end to end, which covers the dialog actually
/// appearing and closing but needs a desktop session and tells you only "exit code 0". These cover the
/// parts that decide *when* a dialog closes and *which* instance is doing the closing - the state a
/// caller configures before any dialog exists - and need no display.
/// </para>
/// </summary>
[Collection("NonParallelAutoClosing")]
public class AutoClosingMessageBoxBehaviourTests : IDisposable
{
    private readonly int _originalTimeout = global::System.Windows.Forms.AutoClosingMessageBox.Instance.MessageBoxTimeout;

    /// <summary>The instance is a thread-static singleton, so each test restores what it changed.</summary>
    public void Dispose() =>
        global::System.Windows.Forms.AutoClosingMessageBox.Instance.MessageBoxTimeout = _originalTimeout;

    [Fact]
    public void Instance_OnOneThread_IsASingleton()
    {
        var first = global::System.Windows.Forms.AutoClosingMessageBox.Instance;
        var second = global::System.Windows.Forms.AutoClosingMessageBox.Instance;

        Assert.Same(first, second);
    }


    [Fact]
    public void Init_SetsTheTimeoutUsedForAutoClosing()
    {
        global::System.Windows.Forms.AutoClosingMessageBox.Init(1234);

        Assert.Equal(1234, global::System.Windows.Forms.AutoClosingMessageBox.Instance.MessageBoxTimeout);
    }

    [Fact]
    public void Init_WithoutAnInterval_UsesTheFiveSecondDefault()
    {
        global::System.Windows.Forms.AutoClosingMessageBox.Init(999);

        global::System.Windows.Forms.AutoClosingMessageBox.Init();

        Assert.Equal(5000, global::System.Windows.Forms.AutoClosingMessageBox.Instance.MessageBoxTimeout);
    }

    [Fact]
    public void MessageBoxTimeout_IsSettableDirectly()
    {
        global::System.Windows.Forms.AutoClosingMessageBox.Instance.MessageBoxTimeout = 250;

        Assert.Equal(250, global::System.Windows.Forms.AutoClosingMessageBox.Instance.MessageBoxTimeout);
    }

    [Fact]
    public void Init_IsIdempotentForTheInstance()
    {
        var before = global::System.Windows.Forms.AutoClosingMessageBox.Instance;

        global::System.Windows.Forms.AutoClosingMessageBox.Init(2000);

        // Re-initialising reconfigures the singleton; it must not replace it, or a caller holding the
        // previous reference would be configuring an object nothing uses.
        Assert.Same(before, global::System.Windows.Forms.AutoClosingMessageBox.Instance);
    }

    [Fact]
    public void Instance_IsThreadStatic()
    {
        var onThisThread = global::System.Windows.Forms.AutoClosingMessageBox.Instance;
        object? onAnotherThread = null;

        var thread = new global::System.Threading.Thread(
            () => onAnotherThread = global::System.Windows.Forms.AutoClosingMessageBox.Instance);
        thread.Start();
        thread.Join();

        // GTK dialogs belong to the thread that created them, so the instance tracking "the dialog to
        // close" has to be per-thread as well.
        Assert.NotNull(onAnotherThread);
        Assert.NotSame(onThisThread, onAnotherThread);
    }

    [Fact]
    public void Instance_ImplementsIMessageBox()
    {
        // It is installed *as* the message box implementation, so the interface is load-bearing.
        Assert.IsAssignableFrom<IMessageBox>(global::System.Windows.Forms.AutoClosingMessageBox.Instance);
    }

    /// <summary>
    /// <c>DialogAvailable</c> is the documented hook for inspecting or restyling the dialog before it is
    /// dismissed, and it could never fire: the class subscribed to the inner message box's event, stored
    /// the dialog in a field, and stopped - nothing called <c>OnDialogAvailable</c>, so a subscriber was
    /// never called back. Raising it needs no display, because the notification originates from the inner
    /// implementation rather than from GTK.
    /// </summary>
    [Fact]
    public void DialogAvailable_ForwardsTheInnerMessageBoxNotificationToSubscribers()
    {
        var instance = global::System.Windows.Forms.AutoClosingMessageBox.Instance;

        // InnerMessageBox is internal to the Extensions assembly, which is signed and grants
        // InternalsVisibleTo only to the strong-named test assemblies; reflection keeps this test
        // exercising the real wiring without widening that grant.
        var inner = (IMessageBox?)typeof(global::System.Windows.Forms.AutoClosingMessageBox)
            .GetField("InnerMessageBox", global::System.Reflection.BindingFlags.Static |
                                         global::System.Reflection.BindingFlags.NonPublic)!
            .GetValue(null);
        Assert.NotNull(inner);

        DialogEventArgs? received = null;
        object? sender = null;
        void Handler(object? s, DialogEventArgs e)
        {
            sender = s;
            received = e;
        }

        instance.DialogAvailable += Handler;
        try
        {
            var args = new DialogEventArgs(new global::Gtk.Dialog());
            inner!.GetType()
                .GetMethod("OnDialogAvailable", global::System.Reflection.BindingFlags.Instance |
                                                global::System.Reflection.BindingFlags.NonPublic)!
                .Invoke(inner, new object[] { args });

            Assert.Same(args, received);
            Assert.Same(instance, sender);
        }
        finally
        {
            instance.DialogAvailable -= Handler;
        }
    }
}

/// <summary>
/// The instance is thread-static singleton state, so these tests must not interleave with each other.
/// </summary>
[CollectionDefinition("NonParallelAutoClosing", DisableParallelization = true)]
public class NonParallelAutoClosingCollection
{
}

#pragma warning restore CS0612
