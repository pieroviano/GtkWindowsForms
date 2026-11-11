namespace System.Windows.Forms;

extern alias gtk;

/// <summary>
/// Verifies that <c>CenterToScreen</c> actually MOVES the native top-level window to the center of its
/// monitor's work area on Windows. Gtk4 has no managed window positioning, so the move is performed against
/// the Win32 HWND (see <c>NativeWindowPlacement</c>); this checks the resulting on-screen rectangle from a
/// Load handler, while the window is realized.
/// </summary>
[Collection("NonParallelCollection")]
public class FormCenterNativeFunctionalTests
{
    private sealed class CenterProbeForm : Form
    {
        public void DoCenterToScreen() => CenterToScreen();
    }

    [Fact]
    public void CenterToScreen_MovesWindowToWorkAreaCenter()
    {
        if (!System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(
                System.Runtime.InteropServices.OSPlatform.Windows))
        {
            return; // the native move is asserted on Windows; X11/macOS are verified at runtime
        }

        var title = "CenterProbe_" + System.Guid.NewGuid().ToString("N");
        var form = new CenterProbeForm { Text = title, Width = 420, Height = 300 };

        var measured = false;
        var dx = int.MaxValue;
        var dy = int.MaxValue;

        form.Load += (_, _) =>
        {
            try
            {
                form.DoCenterToScreen();
                if (GtkSystem.Windows.Forms.GtkControls.ControlBase.NativeWindowPlacement.TryGetWindowAndWorkArea(
                        title, out var wx, out var wy, out var ww, out var wh,
                        out var ax, out var ay, out var aw, out var ah))
                {
                    var expectedX = ax + Math.Max(0, (aw - ww) / 2);
                    var expectedY = ay + Math.Max(0, (ah - wh) / 2);
                    dx = wx - expectedX;
                    dy = wy - expectedY;
                    measured = true;
                }
            }
            finally
            {
                form.Close();
            }
        };

        form.ShowDialog();

        Assert.True(measured, "The window could not be located by title while shown.");
        Assert.True(Math.Abs(dx) <= 2 && Math.Abs(dy) <= 2, $"Window not centered: offset dx={dx}, dy={dy}.");

        form.Dispose();
    }
}
