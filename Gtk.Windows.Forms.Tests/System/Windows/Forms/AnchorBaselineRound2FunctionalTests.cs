using System.Diagnostics;
using System.Drawing;

namespace System.Windows.Forms;

/// <summary>
/// Anchoring edge cases that follow the container's size, pinned to what WinForms does: a control's
/// distances to its parent's edges are kept from when it was positioned and survive reparenting, scaling,
/// clamping and visibility. Needs a display (real windows, pumped main loop).
/// </summary>
[Collection("NonParallelCollection")]
public class AnchorBaselineRound2FunctionalTests
{
    private const AnchorStyles TopRight = AnchorStyles.Top | AnchorStyles.Right;
    private const AnchorStyles BottomRight = AnchorStyles.Bottom | AnchorStyles.Right;
    private const AnchorStyles BottomLeft = AnchorStyles.Bottom | AnchorStyles.Left;

    private static bool PumpUntil(Func<bool> condition, int timeoutMs = 5000)
    {
        var watch = Stopwatch.StartNew();
        while (watch.ElapsedMilliseconds < timeoutMs)
        {
            Application.DoEvents();
            if (condition())
            {
                return true;
            }

            Threading.Thread.Sleep(15);
        }

        return condition();
    }

    private static void Settle(int ms)
    {
        var watch = Stopwatch.StartNew();
        while (watch.ElapsedMilliseconds < ms)
        {
            Application.DoEvents();
            Threading.Thread.Sleep(15);
        }
    }

    private static void ShowSettled(Form form, Control probe, int settleMs = 3000)
    {
        form.Show();
        Assert.True(PumpUntil(() => probe.Widget!.AllocatedWidth > 1), "the form never got its allocation");
        Settle(settleMs);
    }

    // A StatusStrip lives in the window's content area, not in the anchor overlay, even when it is the
    // first control: baselines, DisplayRectangle and new anchors must be measured on the overlay.
    [Fact]
    public void StatusStripAddedFirst_DoesNotCorruptBaselinesOrDisplayRectangle()
    {
        var form = new Form { ClientSize = new Size(600, 400) };
        form.SuspendLayout();
        var strip = new StatusStrip();
        var button = new Button { Location = new Point(400, 300), Size = new Size(100, 24), Anchor = BottomRight };
        var other = new Button { Location = new Point(10, 10), Size = new Size(100, 24) };
        form.Controls.Add(strip);
        form.Controls.Add(button);
        form.Controls.Add(other);
        form.ResumeLayout(false);

        try
        {
            ShowSettled(form, button);
            var designedTop = button.Top;
            Assert.Equal(300, designedTop);
            Assert.True(form.DisplayRectangle.Height > 300, "DisplayRectangle reported the status strip's height");

            button.Left = 380;
            other.Visible = false;
            Settle(500);
            Assert.Equal(new Rectangle(380, 300, 100, 24), button.Bounds);

            var added = new Button { Location = new Point(400, 300), Size = new Size(100, 24), Anchor = BottomRight };
            form.Controls.Add(added);
            Settle(300);
            other.Visible = true;
            Settle(500);
            Assert.Equal(300, added.Top);
            Assert.Equal(400, added.Left);
        }
        finally
        {
            form.Close();
        }
    }

    // WinForms stores anchor distances per parent: a control removed and re-added, or moved to another
    // parent, is positioned for the container it joins as it is at that moment.
    [Fact]
    public void ControlReAddedAfterItsContainerWasResized_KeepsItsPosition()
    {
        var form = new Form { ClientSize = new Size(900, 600) };
        form.SuspendLayout();
        var panel = new Panel { Location = new Point(10, 10), Size = new Size(500, 300) };
        var button = new Button { Location = new Point(400, 10), Size = new Size(80, 24), Anchor = TopRight };
        var sibling = new Button { Location = new Point(10, 10), Size = new Size(80, 24) };
        panel.Controls.Add(button);
        panel.Controls.Add(sibling);
        form.Controls.Add(panel);
        form.ResumeLayout(false);

        try
        {
            ShowSettled(form, button, 2500);
            panel.Controls.Remove(button);
            panel.Size = new Size(700, 300);
            Assert.True(PumpUntil(() => panel.Widget!.AllocatedWidth == 700));
            Settle(500);
            panel.Controls.Add(button);
            Settle(300);
            sibling.Visible = false;
            Settle(500);

            Assert.Equal(400, button.Left);
        }
        finally
        {
            form.Close();
        }
    }

    [Fact]
    public void ControlMovedToASmallerPanel_KeepsItsPosition()
    {
        var form = new Form { ClientSize = new Size(900, 600) };
        form.SuspendLayout();
        var first = new Panel { Location = new Point(10, 10), Size = new Size(500, 300) };
        var second = new Panel { Location = new Point(10, 320), Size = new Size(400, 200) };
        var button = new Button { Location = new Point(300, 10), Size = new Size(80, 24), Anchor = TopRight };
        var sibling = new Button { Location = new Point(10, 10), Size = new Size(80, 24) };
        first.Controls.Add(button);
        second.Controls.Add(sibling);
        form.Controls.Add(first);
        form.Controls.Add(second);
        form.ResumeLayout(false);

        try
        {
            ShowSettled(form, button, 2500);
            first.Controls.Remove(button);
            second.Controls.Add(button);
            Settle(300);
            sibling.Visible = false;
            Settle(500);

            Assert.Equal(300, button.Left);
        }
        finally
        {
            form.Close();
        }
    }

    // A hidden control that has no size request (an AutoSize label) must be moved, not stamped 0x0.
    [Fact]
    public void HiddenAutoSizeLabel_IsMovedButNotCollapsed()
    {
        var form = new Form { ClientSize = new Size(600, 400) };
        form.SuspendLayout();
        var visible = new Label { Text = "Hello world label", AutoSize = true, Location = new Point(400, 10), Anchor = TopRight };
        var hidden = new Label { Text = "Hello world label", AutoSize = true, Location = new Point(400, 40), Anchor = TopRight, Visible = false };
        form.Controls.Add(visible);
        form.Controls.Add(hidden);
        form.ResumeLayout(false);

        try
        {
            ShowSettled(form, visible);
            var width0 = form.DisplayRectangle.Width;
            form.WindowState = FormWindowState.Maximized;
            Assert.True(PumpUntil(() => form.DisplayRectangle.Width > width0 + 100));
            Settle(1500);

            hidden.Visible = true;
            Settle(500);

            Assert.True(hidden.Width > 0 && hidden.Height > 0, $"hidden label collapsed to {hidden.Size}");
            Assert.Equal(visible.Left, hidden.Left);
            Assert.Equal(visible.Size, hidden.Size);
        }
        finally
        {
            form.Close();
        }
    }

    // ClientSize assigned by the designer is what the children are positioned for; assigned later it is a
    // resize: the bottom-anchored child keeps its 50 px gap (WinForms).
    [Theory]
    [InlineData("before-show")]
    [InlineData("load")]
    [InlineData("shown")]
    public void ClientSizeHeightChangedAfterTheDesigner_MovesBottomAnchoredChildren(string when)
    {
        var form = new Form { ClientSize = new Size(937, 590) };
        form.SuspendLayout();
        var bottom = new Panel { Location = new Point(12, 500), Size = new Size(200, 40), Anchor = BottomLeft };
        form.Controls.Add(bottom);
        form.ResumeLayout(false);

        void Change() => form.ClientSize = new Size(937, 700);
        switch (when)
        {
            case "before-show":
                Change();
                break;
            case "load":
                form.Load += (_, _) => Change();
                break;
            default:
                form.Shown += (_, _) => Change();
                break;
        }

        try
        {
            ShowSettled(form, bottom, 3500);

            Assert.Equal(50, form.DisplayRectangle.Height - bottom.Bottom);
        }
        finally
        {
            form.Close();
        }
    }

    // A form designed with Size only (no ClientSize): a later Height / Width change still moves the
    // children. Expected gaps are those of an identical form that is not changed.
    [Theory]
    [InlineData("height")]
    [InlineData("width")]
    public void FormDesignedWithSizeOnly_FollowsALaterSizeChange(string axis)
    {
        Form Build(out Button button)
        {
            var f = new Form { Size = new Size(800, 600) };
            f.SuspendLayout();
            button = new Button { Location = new Point(600, 450), Size = new Size(100, 24), Anchor = BottomRight };
            f.Controls.Add(button);
            f.ResumeLayout(false);

            return f;
        }

        var reference = Build(out var referenceButton);
        int gapRight;
        int gapBottom;
        try
        {
            ShowSettled(reference, referenceButton, 3500);
            gapRight = reference.DisplayRectangle.Width - referenceButton.Right;
            gapBottom = reference.DisplayRectangle.Height - referenceButton.Bottom;
        }
        finally
        {
            reference.Close();
        }

        var form = Build(out var button);
        form.Load += (_, _) =>
        {
            if (axis == "height")
            {
                form.Height = 800;
            }
            else
            {
                form.Width = 1000;
            }
        };

        try
        {
            ShowSettled(form, button, 3500);

            Assert.Equal(gapRight, form.DisplayRectangle.Width - button.Right);
            Assert.Equal(gapBottom, form.DisplayRectangle.Height - button.Bottom);
        }
        finally
        {
            form.Close();
        }
    }

    // Scaling is not a resize. Form scaled by 1.5 before Show: client 600x400 -> 900x600, a button at
    // x=450 width 100 -> x=675 width 150, so its gap to the right edge is 50*1.5 = 75 (WinForms).
    [Fact]
    public void FormScaledBeforeShow_DoesNotMoveAnchoredChildrenTwice()
    {
        var form = new Form { ClientSize = new Size(600, 400) };
        form.SuspendLayout();
        var button = new Button { Location = new Point(450, 10), Size = new Size(100, 24), Anchor = TopRight };
        form.Controls.Add(button);
        form.ResumeLayout(false);
        form.Scale(new SizeF(1.5f, 1.5f));

        try
        {
            ShowSettled(form, button, 3500);

            Assert.Equal(675, button.Left);
            Assert.Equal(75, form.DisplayRectangle.Width - button.Right);
        }
        finally
        {
            form.Close();
        }
    }

    // A panel built at runtime and resized (before or after it joins the form) moves its anchored child:
    // 150 + (400 - 200) = 350.
    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void RuntimePanelResized_MovesItsAnchoredChild(bool resizeBeforeAdding)
    {
        var form = new Form { ClientSize = new Size(800, 500) };
        form.SuspendLayout();
        var anchor = new Button { Location = new Point(10, 10), Size = new Size(80, 24) };
        form.Controls.Add(anchor);
        form.ResumeLayout(false);

        try
        {
            ShowSettled(form, anchor);
            var panel = new Panel { Location = new Point(10, 50), Size = new Size(200, 100) };
            var child = new Button { Location = new Point(150, 10), Size = new Size(40, 24), Anchor = TopRight };
            panel.Controls.Add(child);
            if (resizeBeforeAdding)
            {
                panel.Width = 400;
                form.Controls.Add(panel);
            }
            else
            {
                form.Controls.Add(panel);
                panel.Width = 400;
            }

            Settle(1000);

            Assert.Equal(350, child.Left);
        }
        finally
        {
            form.Close();
        }
    }

    // GTK margins and size requests cannot be negative, but WinForms recovers a control from its stored
    // distances: shrinking the container past the control and growing it back restores it (100 -> 100).
    [Fact]
    public void ChildPushedPastTheEdge_IsRestoredWhenTheContainerGrowsBack()
    {
        var form = new Form { ClientSize = new Size(900, 700) };
        form.SuspendLayout();
        var container = new Panel { Location = new Point(10, 10), Size = new Size(300, 200) };
        var child = new Button { Location = new Point(100, 10), Size = new Size(50, 24), Anchor = TopRight };
        container.Controls.Add(child);
        form.Controls.Add(container);
        form.ResumeLayout(false);

        try
        {
            ShowSettled(form, child, 2500);
            container.Size = new Size(100, 200);
            Assert.True(PumpUntil(() => container.Widget!.AllocatedWidth == 100));
            Settle(300);
            container.Size = new Size(300, 200);
            Assert.True(PumpUntil(() => container.Widget!.AllocatedWidth == 300));
            Settle(500);

            Assert.Equal(100, child.Left);
        }
        finally
        {
            form.Close();
        }
    }
}
