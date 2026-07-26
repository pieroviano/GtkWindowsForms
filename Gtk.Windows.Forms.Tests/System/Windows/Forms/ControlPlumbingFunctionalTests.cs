extern alias gtk;
using System.Drawing;
using System.Windows.Forms;
using Font = gtk::System.Drawing.Font;
using FontFamily = gtk::System.Drawing.FontFamily;

namespace GtkWinForms.Tests.System.Windows.Forms;

/// <summary>Behavioural tests for <see cref="Control"/> geometry, hierarchy and invoke plumbing.
/// They found two defects: <see cref="Control.FindForm"/> re-read <c>this.Parent</c> while walking
/// up, so any control nested deeper than one level below its form spun forever, and
/// <see cref="Control.EndInvoke"/> returned the <see cref="Exception"/> object for a faulted
/// delegate instead of rethrowing it as WinForms does.</summary>
[Collection("NonParallelCollection")]
public class ControlPlumbingFunctionalTests
{
    // ---- FindForm ---------------------------------------------------------

    [Fact]
    public void FindFormWalksUpFromADirectChild()
    {
        using var form = new Form();
        using var button = new Button();
        form.Controls.Add(button);

        Assert.Same(form, button.FindForm());
    }

    [Fact]
    public void FindFormWalksThroughNestedContainers()
    {
        // Regression: the parent walk used to re-read this.Parent and never advance.
        using var form = new Form();
        using var panel = new Panel();
        using var inner = new Panel();
        using var label = new Label();
        inner.Controls.Add(label);
        panel.Controls.Add(inner);
        form.Controls.Add(panel);

        Assert.Same(form, label.FindForm());
    }

    [Fact]
    public void FindFormIsNullForAnOrphanControl()
    {
        using var panel = new Panel();
        using var label = new Label();
        panel.Controls.Add(label);

        Assert.Null(label.FindForm());
    }

    // ---- GetChildAtPoint --------------------------------------------------

    [Fact]
    public void GetChildAtPointFindsTheChildCoveringThePoint()
    {
        using var panel = new Panel();
        using var first = new Button { Location = new Point(0, 0), Size = new Size(50, 20) };
        using var second = new Button { Location = new Point(100, 100), Size = new Size(50, 20) };
        panel.Controls.Add(first);
        panel.Controls.Add(second);

        Assert.Same(second, panel.GetChildAtPoint(new Point(120, 110)));
        Assert.Same(first, panel.GetChildAtPoint(new Point(10, 10)));
        Assert.Null(panel.GetChildAtPoint(new Point(500, 500)));
    }

    [Fact]
    public void GetChildAtPointCanSkipInvisibleAndDisabledChildren()
    {
        using var panel = new Panel();
        using var hidden = new Button { Location = new Point(0, 0), Size = new Size(50, 20), Visible = false };
        using var disabled = new Button { Location = new Point(100, 0), Size = new Size(50, 20), Enabled = false };
        panel.Controls.Add(hidden);
        panel.Controls.Add(disabled);

        Assert.Same(hidden, panel.GetChildAtPoint(new Point(10, 10), GetChildAtPointSkip.None));
        Assert.Null(panel.GetChildAtPoint(new Point(10, 10), GetChildAtPointSkip.Invisible));
        Assert.Same(disabled, panel.GetChildAtPoint(new Point(110, 10), GetChildAtPointSkip.None));
        Assert.Null(panel.GetChildAtPoint(new Point(110, 10), GetChildAtPointSkip.Disabled));
    }

    // ---- SetBounds --------------------------------------------------------

    [Fact]
    public void SetBoundsWithAllAppliesEverything()
    {
        using var panel = new Panel();

        panel.SetBounds(new Rectangle(10, 20, 30, 40), BoundsSpecified.All);

        Assert.Equal(new Point(10, 20), panel.Location);
        Assert.Equal(new Size(30, 40), panel.Size);
    }

    [Fact]
    public void SetBoundsHonoursThePartialFlags()
    {
        using var panel = new Panel { Location = new Point(1, 2), Size = new Size(30, 40) };

        panel.SetBounds(new Rectangle(50, 60, 70, 80), BoundsSpecified.X);
        Assert.Equal(new Point(50, 2), panel.Location);
        Assert.Equal(new Size(30, 40), panel.Size);

        panel.SetBounds(new Rectangle(0, 90, 0, 0), BoundsSpecified.Y);
        Assert.Equal(new Point(50, 90), panel.Location);

        panel.SetBounds(new Rectangle(0, 0, 75, 0), BoundsSpecified.Width);
        Assert.Equal(new Size(75, 40), panel.Size);

        panel.SetBounds(new Rectangle(0, 0, 0, 85), BoundsSpecified.Height);
        Assert.Equal(new Size(75, 85), panel.Size);
    }

    [Fact]
    public void SetBoundsWithNoneChangesNothing()
    {
        using var panel = new Panel { Location = new Point(1, 2), Size = new Size(30, 40) };

        panel.SetBounds(new Rectangle(50, 60, 70, 80), BoundsSpecified.None);

        Assert.Equal(new Point(1, 2), panel.Location);
        Assert.Equal(new Size(30, 40), panel.Size);
    }

    // ---- coordinate mapping (headless: no window origin to offset by) -----

    [Fact]
    public void CoordinateMappingIsTheIdentityWithoutAWindow()
    {
        using var panel = new Panel();

        Assert.Equal(new Point(5, 7), panel.PointToScreen(new Point(5, 7)));
        Assert.Equal(new Point(5, 7), panel.PointToClient(new Point(5, 7)));
        Assert.Equal(new Rectangle(1, 2, 3, 4), panel.RectangleToScreen(new Rectangle(1, 2, 3, 4)));
        Assert.Equal(new Rectangle(1, 2, 3, 4), panel.RectangleToClient(new Rectangle(1, 2, 3, 4)));
    }

    [Fact]
    public void LogicalToDeviceUnitsScalesByTheDeviceDpi()
    {
        // DeviceDpi was a never-assigned auto-property reporting 0, which zeroed out any
        // WinForms-style scaling arithmetic.
        using var panel = new Panel();

        Assert.True(panel.DeviceDpi >= 96, $"DeviceDpi should never be below the 96 baseline but was {panel.DeviceDpi}");
        Assert.Equal(panel.DeviceDpi, panel.LogicalToDeviceUnits(96));
        Assert.Equal(0, panel.LogicalToDeviceUnits(0));
        Assert.Equal(new Size(panel.DeviceDpi, panel.DeviceDpi * 2), panel.LogicalToDeviceUnits(new Size(96, 192)));
    }

    // ---- PerformClick and static input state ------------------------------

    [Fact]
    public void PerformClickRaisesClickOnce()
    {
        using var panel = new Panel();
        var clicks = 0;
        object? sender = null;
        panel.Click += (s, _) => { clicks++; sender = s; };

        panel.PerformClick();

        Assert.Equal(1, clicks);
        Assert.Same(panel, sender);
    }

    // Control.ModifierKeys / Control.MouseButtons deliberately have no test: they report the live
    // GDK device state, so any assertion races against the person using the machine.

    // ---- Reset family -----------------------------------------------------

    [Fact]
    public void ResetTextClearsTheText()
    {
        using var panel = new Panel { Text = "something" };

        panel.ResetText();

        Assert.Equal(string.Empty, panel.Text);
    }

    [Fact]
    public void ResetCursorRestoresTheDefaultCursor()
    {
        using var panel = new Panel { Cursor = Cursors.Hand };

        panel.ResetCursor();

        Assert.Equal(Cursors.Default, panel.Cursor);
    }

    [Fact]
    public void ResetFontRestoresTheNinePointSansDefault()
    {
        using var panel = new Panel { Font = new Font(FontFamily.GenericSerif, 20f) };

        panel.ResetFont();

        var font = panel.Font;
        Assert.NotNull(font);
        Assert.Equal(9f, font!.Size, 3);
        Assert.Equal(FontFamily.GenericSansSerif.Name, font.FontFamily?.Name);
    }

    [Fact]
    public void ResetForeColorAndBackColorDropTheOverrides()
    {
        using var panel = new Panel { ForeColor = Color.Red, BackColor = Color.Lime };

        panel.ResetForeColor();
        panel.ResetBackColor();

        Assert.NotEqual(Color.Red, panel.ForeColor);
        Assert.NotEqual(Color.Lime, panel.BackColor);
    }

    [Fact]
    public void ResetRightToLeftLeavesTheDefault()
    {
        using var panel = new Panel { RightToLeft = RightToLeft.Yes };

        panel.ResetRightToLeft();

        Assert.NotEqual(RightToLeft.Yes, panel.RightToLeft);
    }

    // ---- BeginInvoke / EndInvoke ------------------------------------------

    private static void PumpUntilCompleted(IAsyncResult handle)
    {
        for (var i = 0; i < 1000 && !handle.IsCompleted; i++)
        {
            while (global::Gtk.Application.EventsPending())
            {
                global::Gtk.Application.RunIteration(false);
            }

            global::GLib.MainContext.Iteration();
        }

        Assert.True(handle.IsCompleted, "the queued BeginInvoke work never ran");
    }

    [Fact]
    public void EndInvokeReturnsTheDelegateResult()
    {
        using var panel = new Panel();

        var handle = panel.BeginInvoke(new Func<int>(() => 42), null);
        PumpUntilCompleted(handle);

        Assert.Equal(42, panel.EndInvoke(handle));
    }

    [Fact]
    public void EndInvokePassesTheArgumentsThrough()
    {
        using var panel = new Panel();

        var handle = panel.BeginInvoke(new Func<int, int>(x => x * 2), new object[] { 21 });
        PumpUntilCompleted(handle);

        Assert.Equal(42, panel.EndInvoke(handle));
    }

    [Fact]
    public void EndInvokeRethrowsTheDelegateException()
    {
        // Regression: a faulted delegate used to come back as the Exception object, silently.
        using var panel = new Panel();

        var handle = panel.BeginInvoke(new Action(() => throw new InvalidOperationException("kaboom")));
        PumpUntilCompleted(handle);

        var ex = Assert.Throws<InvalidOperationException>(() => panel.EndInvoke(handle));
        Assert.Equal("kaboom", ex.Message);
    }

    [Fact]
    public void EndInvokeOfAnActionReturnsNull()
    {
        using var panel = new Panel();

        var handle = panel.BeginInvoke(new Action(() => { }));
        PumpUntilCompleted(handle);

        Assert.Null(panel.EndInvoke(handle));
    }
}
