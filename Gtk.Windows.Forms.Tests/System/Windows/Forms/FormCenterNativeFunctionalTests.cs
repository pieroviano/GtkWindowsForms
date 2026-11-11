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

    /// <summary>
    /// The real-world defect: a control below the opening client area makes the content-fit watch grow the window
    /// downward (anchored at the top) over up to ~2s after it is shown, so a center applied at the opening size -
    /// or on a fixed 150ms delay - leaves the final, taller window sitting too high. The center must re-apply once
    /// that growth settles. Without the fix the window stays off-center vertically by about half the growth.
    /// </summary>
    [Fact]
    public void CenterStaysCentered_AfterContentGrowsWindow()
    {
        if (!System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(
                System.Runtime.InteropServices.OSPlatform.Windows))
        {
            return;
        }

        var title = "CenterGrow_" + System.Guid.NewGuid().ToString("N");
        var form = new CenterProbeForm { Text = title, Width = 480, Height = 180 };
        form.Controls.Add(new Button { Text = "deep", Left = 40, Top = 300, Width = 120, Height = 32 });

        var measured = false;
        var dy = int.MaxValue;
        var lastHeight = -1;
        var stableTicks = 0;
        var ticks = 0;

        form.Load += (_, _) =>
        {
            form.DoCenterToScreen();

            // Poll until the window height stops changing for a good spell (~0.6s): by then the content-fit watch
            // has finished growing the window AND fired its settle re-center. Measuring only when stable avoids
            // reading a transient, mid-resize position. A tick cap closes the form if it never settles.
            GLib.Timeout.Add(50, () =>
            {
                if (!NativeHeight(title, out var height) || ++ticks > 80)
                {
                    if (ticks > 80)
                    {
                        form.Close();
                    }

                    return ticks <= 80;
                }

                if (height == lastHeight)
                {
                    stableTicks++;
                }
                else
                {
                    stableTicks = 0;
                    lastHeight = height;
                }

                if (stableTicks < 12)
                {
                    return true;
                }

                try
                {
                    if (Measure(title, out _, out var ay))
                    {
                        dy = ay;
                        measured = true;
                    }
                }
                finally
                {
                    form.Close();
                }

                return false;
            });
        };

        form.ShowDialog();

        Assert.True(measured, "The window could not be located by title while shown.");
        Assert.True(Math.Abs(dy) <= 4,
            $"Window not vertically centered after content-driven growth: dy={dy}.");

        form.Dispose();
    }

    /// <summary>Offset of this process's window titled <paramref name="title" /> from its work-area center.</summary>
    private static bool Measure(string title, out int dx, out int dy)
    {
        dx = dy = int.MaxValue;
        if (!GtkSystem.Windows.Forms.GtkControls.ControlBase.NativeWindowPlacement.TryGetWindowAndWorkArea(
                title, out var wx, out var wy, out var ww, out var wh,
                out var ax, out var ay, out var aw, out var ah))
        {
            return false;
        }

        dx = wx - (ax + Math.Max(0, (aw - ww) / 2));
        dy = wy - (ay + Math.Max(0, (ah - wh) / 2));
        return true;
    }

    /// <summary>Current outer height of this process's window titled <paramref name="title" />.</summary>
    private static bool NativeHeight(string title, out int height)
    {
        height = 0;
        if (!GtkSystem.Windows.Forms.GtkControls.ControlBase.NativeWindowPlacement.TryGetWindowAndWorkArea(
                title, out _, out _, out _, out var wh, out _, out _, out _, out _))
        {
            return false;
        }

        height = wh;
        return true;
    }
}
