using System.Windows.Forms;

namespace GtkWinForms.Tests.System.Windows.Forms;

/// <summary>
/// Behavioural tests for <see cref="Application.ThreadException" /> and
/// <see cref="Application.SetUnhandledExceptionMode(UnhandledExceptionMode)" /> - the last line of defence
/// a Windows Forms application installs, and the policy that decides what happens when nothing catches an
/// exception on the way out of an event handler.
/// </summary>
[Collection("NonParallelCollection")]
public class ApplicationThreadExceptionFunctionalTests : IDisposable
{
    /// <summary>The mode is process-wide static state; every test restores it.</summary>
    public void Dispose() => Application.SetUnhandledExceptionMode(UnhandledExceptionMode.Automatic);

    [Theory]
    // Automatic routes to the handler when there is one, and otherwise lets the process die - the
    // default a console-hosted app wants, because swallowing would hide the failure entirely.
    [InlineData(UnhandledExceptionMode.Automatic, true, (int)Application.UnhandledExceptionDisposition.RaiseThreadException)]
    [InlineData(UnhandledExceptionMode.Automatic, false, (int)Application.UnhandledExceptionDisposition.Rethrow)]
    // ThrowException never routes, handler or not.
    [InlineData(UnhandledExceptionMode.ThrowException, true, (int)Application.UnhandledExceptionDisposition.Rethrow)]
    [InlineData(UnhandledExceptionMode.ThrowException, false, (int)Application.UnhandledExceptionDisposition.Rethrow)]
    // CatchException keeps the application alive even with nobody listening - that is what distinguishes
    // it from Automatic.
    [InlineData(UnhandledExceptionMode.CatchException, true, (int)Application.UnhandledExceptionDisposition.RaiseThreadException)]
    [InlineData(UnhandledExceptionMode.CatchException, false, (int)Application.UnhandledExceptionDisposition.ReportAndContinue)]
    // `expected` is an int because the disposition enum is internal to the library and a public test
    // method cannot take a less-accessible parameter type.
    public void Decide_CoversTheWholePolicyMatrix(UnhandledExceptionMode mode, bool hasHandler, int expected)
    {
        Assert.Equal((Application.UnhandledExceptionDisposition)expected, Application.Decide(mode, hasHandler));
    }

    [Fact]
    public void OnThreadException_WithAHandler_DeliversTheExceptionAndContinues()
    {
        Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
        Exception? received = null;
        void Handler(object? sender, global::System.Threading.ThreadExceptionEventArgs e) => received = e.Exception;
        Application.ThreadException += Handler;

        try
        {
            var thrown = new InvalidOperationException("boom");

            var handled = Application.OnThreadException(thrown);

            Assert.True(handled, "an exception delivered to a handler must not also terminate the application");
            Assert.Same(thrown, received);
        }
        finally
        {
            Application.ThreadException -= Handler;
        }
    }

    [Fact]
    public void OnThreadException_ThrowExceptionMode_DoesNotDeliverAndDoesNotHandle()
    {
        Application.SetUnhandledExceptionMode(UnhandledExceptionMode.ThrowException);
        var delivered = false;
        void Handler(object? sender, global::System.Threading.ThreadExceptionEventArgs e) => delivered = true;
        Application.ThreadException += Handler;

        try
        {
            var handled = Application.OnThreadException(new InvalidOperationException("boom"));

            Assert.False(handled, "ThrowException must let the exception terminate the process");
            Assert.False(delivered, "ThrowException must not route to the handler either");
        }
        finally
        {
            Application.ThreadException -= Handler;
        }
    }

    [Fact]
    public void OnThreadException_CatchExceptionWithNoHandler_KeepsTheApplicationAlive()
    {
        Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);

        Assert.True(Application.OnThreadException(new InvalidOperationException("boom")));
    }

    [Fact]
    public void OnThreadException_AutomaticWithNoHandler_DoesNotSwallow()
    {
        Application.SetUnhandledExceptionMode(UnhandledExceptionMode.Automatic);

        Assert.False(Application.OnThreadException(new InvalidOperationException("boom")),
            "with no handler and no explicit opt-in, an unhandled exception must not be silently swallowed");
    }

    [Fact]
    public void OnThreadException_HandlerThatThrows_DoesNotRecurse()
    {
        Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
        var entries = 0;
        void Handler(object? sender, global::System.Threading.ThreadExceptionEventArgs e)
        {
            entries++;
            throw new InvalidOperationException("the handler itself failed");
        }

        Application.ThreadException += Handler;
        try
        {
            var handled = Application.OnThreadException(new InvalidOperationException("boom"));

            Assert.Equal(1, entries);
            Assert.False(handled, "a handler that throws cannot be said to have handled anything");
        }
        finally
        {
            Application.ThreadException -= Handler;
        }
    }

    [Fact]
    public void OnThreadException_NullException_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => Application.OnThreadException(null!));
    }

    [Fact]
    public void SetUnhandledExceptionMode_RejectsAValueOutsideTheEnum()
    {
        Assert.Throws<global::System.ComponentModel.InvalidEnumArgumentException>(
            () => Application.SetUnhandledExceptionMode((UnhandledExceptionMode)42));
    }

    [Fact]
    public void SetUnhandledExceptionMode_TakesEffectForSubsequentExceptions()
    {
        var delivered = 0;
        void Handler(object? sender, global::System.Threading.ThreadExceptionEventArgs e) => delivered++;
        Application.ThreadException += Handler;

        try
        {
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
            Application.OnThreadException(new InvalidOperationException("first"));
            Assert.Equal(1, delivered);

            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.ThrowException);
            Application.OnThreadException(new InvalidOperationException("second"));
            Assert.Equal(1, delivered);
        }
        finally
        {
            Application.ThreadException -= Handler;
        }
    }

    [Fact]
    public void SetUnhandledExceptionMode_ThreadScopeOverload_IsAccepted()
    {
        // Source compatibility: WinForms code calls this overload; it must compile and behave.
        Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException, false);

        Assert.True(Application.OnThreadException(new InvalidOperationException("boom")));
    }
}
