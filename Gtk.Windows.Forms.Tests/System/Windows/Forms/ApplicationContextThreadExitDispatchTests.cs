using System.Windows.Forms;

namespace GtkWinForms.Tests.System.Windows.Forms;

/// <summary>Pins the contract that <see cref="ApplicationContext.ExitThreadCore"/> raises
/// <see cref="ApplicationContext.ThreadExit"/> through <c>OnThreadExit</c>. It used to raise the event
/// directly, so an <c>OnThreadExit</c> override never ran.</summary>
public class ApplicationContextThreadExitDispatchTests
{
    private sealed class SpyContext : ApplicationContext
    {
        public int OnThreadExitCalls { get; private set; }

        protected override void OnThreadExit(EventArgs e) => OnThreadExitCalls++;
    }

    [Fact]
    public void ExitThread_InvokesTheOnThreadExitOverride()
    {
        using var context = new SpyContext();

        context.ExitThread();

        Assert.Equal(1, context.OnThreadExitCalls);
    }
}
