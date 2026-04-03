namespace System.Windows.Forms.ApplicationTests;

public class ApplicationContextTests
{
    private class TestForm : Form
    {
        public bool DestroyHandleCalled;

        public void TriggerHandleDestroyed()
        {
            OnHandleDestroyed(EventArgs.Empty);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }
    }

    private class TestApplicationContext : ApplicationContext
    {
        public bool ExitThreadCoreCalled;

        protected override void ExitThreadCore()
        {
            ExitThreadCoreCalled = true;
            base.ExitThreadCore();
        }
    }

    [Fact]
    public void Ctor_Default_MainFormIsNull()
    {
        var context = new ApplicationContext();

        Assert.Null(context.MainForm);
    }

    [Fact]
    public void Ctor_WithForm_SetsMainForm()
    {
        var form = new Form();

        var context = new ApplicationContext(form);

        Assert.Equal(form, context.MainForm);
    }

    [Fact]
    public void MainForm_Set_ReplacesOldForm()
    {
        var form1 = new Form();
        var form2 = new Form();

        var context = new ApplicationContext(form1);

        context.MainForm = form2;

        Assert.Equal(form2, context.MainForm);
    }

    [Fact]
    public void Tag_SetAndGet_Works()
    {
        var context = new ApplicationContext();

        context.Tag = "test";

        Assert.Equal("test", context.Tag);
    }

    [Fact]
    public void ExitThread_RaisesThreadExit()
    {
        var context = new ApplicationContext();

        bool called = false;
        context.ThreadExit += (_, _) => called = true;

        context.ExitThread();

        Assert.True(called);
    }

    [Fact]
    public void Dispose_DisposesMainForm()
    {
        var form = new Form();
        var context = new ApplicationContext(form);

        context.Dispose();

        Assert.True(form.IsDisposed);
        Assert.Null(context.MainForm);
    }

    [Fact]
    public void Dispose_DoesNotThrow_WhenMainFormNull()
    {
        var context = new ApplicationContext();

        var ex = Record.Exception(() => context.Dispose());

        Assert.Null(ex);
    }

    [Fact]
    public void ExitThreadCore_Called_WhenExitThreadInvoked()
    {
        var context = new TestApplicationContext();

        context.ExitThread();

        Assert.True(context.ExitThreadCoreCalled);
    }

    [Fact]
    public void MainForm_Destroyed_TriggersThreadExit()
    {
        var form = new TestForm();
        var context = new ApplicationContext(form);

        bool called = false;
        context.ThreadExit += (_, _) => called = true;

        form.TriggerHandleDestroyed();

        Assert.True(called);
    }

    [Fact]
    public void MainForm_Destroyed_DoesNotTrigger_WhenRecreatingHandle()
    {
        var form = new Form();
        var context = new ApplicationContext(form);

        bool called = false;
        context.ThreadExit += (_, _) => called = true;

        // simulate recreating handle via reflection
        var prop = typeof(Control).GetProperty("RecreatingHandle",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        prop?.SetValue(form, true);

        form.Dispose(); // triggers HandleDestroyed

        Assert.False(called);
    }

    [Fact]
    public void OnThreadExit_RaisesEvent()
    {
        var context = new ApplicationContext();

        bool called = false;
        context.ThreadExit += (_, _) => called = true;

        // call protected via subclass
        var method = typeof(ApplicationContext)
            .GetMethod("OnThreadExit", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        method!.Invoke(context, new object[] { EventArgs.Empty });

        Assert.True(called);
    }

    [Fact]
    public void ChangingMainForm_UnhooksOldForm()
    {
        var form1 = new TestForm();
        var form2 = new TestForm();

        var context = new ApplicationContext(form1);

        context.MainForm = form2;

        bool called = false;
        context.ThreadExit += (_, _) => called = true;

        // trigger old form → should NOT fire
        form1.TriggerHandleDestroyed();

        Assert.False(called);

        // trigger new form → should fire
        form2.TriggerHandleDestroyed();

        Assert.True(called);
    }
}