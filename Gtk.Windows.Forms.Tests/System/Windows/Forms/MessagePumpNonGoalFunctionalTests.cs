using System.Windows.Forms;

namespace GtkWinForms.Tests.System.Windows.Forms;

/// <summary>
/// Win32 message interception is a documented non-goal: this library sits on GTK, which has no Windows
/// message pump. <c>WndProc</c> used to have an empty body, so an override intended to intercept
/// WM_NCHITTEST or WM_ERASEBKGND compiled and then silently did nothing. It now states the boundary.
/// </summary>
[Collection("NonParallelCollection")]
public class MessagePumpNonGoalFunctionalTests
{
    /// <summary>Exposes the protected message entry points so the test can call them.</summary>
    private sealed class ProbeControl : Control
    {
        public void CallWndProc()
        {
            var message = default(Message);
            WndProc(ref message);
        }

        public void CallDefWndProc()
        {
            var message = default(Message);
            DefWndProc(ref message);
        }
    }

    [Fact]
    public void WndProc_StatesTheBoundaryInsteadOfSilentlyDoingNothing()
    {
        using var control = new ProbeControl();

        var exception = Assert.Throws<PlatformNotSupportedException>(control.CallWndProc);

        Assert.Contains("GTK", exception.Message);
    }

    [Fact]
    public void DefWndProc_StatesTheBoundary()
    {
        using var control = new ProbeControl();

        Assert.Throws<PlatformNotSupportedException>(control.CallDefWndProc);
    }

    [Fact]
    public void OrdinaryControlUse_IsUnaffectedByTheMessagePumpBoundary()
    {
        // The event model is the supported path and must keep working.
        using var control = new ProbeControl();
        var clicked = 0;
        control.Click += (_, _) => clicked++;

        control.PerformClick();

        Assert.Equal(1, clicked);
    }
}
