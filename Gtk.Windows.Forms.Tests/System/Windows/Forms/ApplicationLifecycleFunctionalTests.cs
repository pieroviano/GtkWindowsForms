using System.Windows.Forms;
using Mocks;

namespace GtkWinForms.Tests.System.Windows.Forms;

/// <summary>
/// Behavioural tests for the application startup/lifecycle surface. <see cref="ApplicationContext" />
/// existed as a type but nothing consumed it, so the multi-form / no-main-form startup pattern had no
/// entry point at all; and <see cref="Control.HandleCreated" />/<see cref="Control.HandleDestroyed" />
/// were never raised, which is how a context learns its main form has closed.
/// </summary>
[Collection("NonParallelCollection")]
public class ApplicationLifecycleFunctionalTests
{
    [Fact]
    public void Run_WithAContext_IsAvailableAndRejectsNull()
    {
        // The overload must exist and validate, rather than not exist at all.
        Assert.Throws<ArgumentNullException>(() => Application.Run((ApplicationContext)null!));
    }

    [Fact]
    public void ApplicationContext_ExposesTheMainFormItWasBuiltWith()
    {
        using var form = new Form();
        using var context = new ApplicationContext(form);

        Assert.Same(form, context.MainForm);
    }

    [Fact]
    public void ApplicationContext_ExitThread_RaisesThreadExit()
    {
        using var context = new ApplicationContext();
        var raised = 0;
        context.ThreadExit += (_, _) => raised++;

        context.ExitThread();

        Assert.Equal(1, raised);
    }

    [Fact]
    public void ApplicationContext_DisposingDisposesTheMainForm()
    {
        var form = new Form();
        var context = new ApplicationContext(form);

        context.Dispose();

        Assert.True(form.IsDisposed, "the context owns its main form's lifetime");
    }

    [Fact]
    public void MessageLoop_IsFalseWhenNoLoopIsRunning()
    {
        Assert.False(Application.MessageLoop);
    }

    [Fact]
    public void HandleCreated_IsRaisedWhenTheWidgetIsRealized()
    {
        using var form = new Form();
        var raised = 0;
        form.HandleCreated += (_, _) => raised++;

        form.Show();
        Application.DoEvents();

        Assert.True(raised > 0, "HandleCreated must fire once the widget has a real handle");
    }

    [Fact]
    public void HandleDestroyed_IsRaisedWhenTheFormIsClosed()
    {
        var form = new Form();
        var raised = 0;
        form.HandleDestroyed += (_, _) => raised++;

        form.Show();
        Application.DoEvents();
        form.Close();
        Application.DoEvents();

        Assert.True(raised > 0, "HandleDestroyed must fire - ApplicationContext detects main-form close through it");
    }

    [Fact]
    public void Close_IsSynchronous_WhicheverMessageBoxIsInstalled()
    {
        // Regression: Close() handed itself to a 100 ms timer whenever MessageBox.Instance was not the
        // default NonStaticMessageBox, so installing any custom IMessageBox - something an app does once at
        // startup, and something other tests in this assembly do - silently made every form's close
        // asynchronous. HandleDestroyed then fired long after Close() returned, which is how
        // ApplicationContext learns its main form closed. Closing a form does not depend on message boxes.
        using var mock = MockMessageBox.ReplaceMessageBoxWithMock();

        var form = new Form();
        var destroyed = 0;
        form.HandleDestroyed += (_, _) => destroyed++;

        form.Show();
        Application.DoEvents();
        form.Close(); // deliberately no DoEvents afterwards: the close must already have happened

        Assert.True(destroyed > 0, "Close() must destroy the handle before it returns");
    }

    [Fact]
    public void ApplicationContext_EndsItsLoopWhenTheMainFormIsClosed()
    {
        // The whole point of the context: closing the main form must reach ExitThreadCore, which is what
        // terminates the message loop Run(context) started.
        var form = new Form();
        using var context = new ApplicationContext(form);
        var exited = 0;
        context.ThreadExit += (_, _) => exited++;

        form.Show();
        Application.DoEvents();
        form.Close();
        Application.DoEvents();

        Assert.True(exited > 0, "closing the main form must raise ThreadExit on the context");
    }
}
