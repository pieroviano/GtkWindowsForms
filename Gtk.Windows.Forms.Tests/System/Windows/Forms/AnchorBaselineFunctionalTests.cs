using System.Diagnostics;
using System.Drawing;

namespace System.Windows.Forms;

/// <summary>
/// Anchoring must follow every change of the container's client size since the child was positioned,
/// whether or not the child was visible during the change and whether or not the window had been mapped.
/// <para>
/// The anchor pass is per child (each child remembers the container size its bounds correspond to), as
/// WinForms stores per-control distances to the parent's edges. Expected values are the anchor arithmetic
/// of <see cref="Control.AnchorBounds" />, which <c>LayoutArrangementFunctionalTests</c> pins against real
/// WinForms: right-anchored moves by the delta, left+right stretches by it, none moves by half of it.
/// </para>
/// These tests show real windows and pump the GTK main loop, so they need a display.
/// </summary>
[Collection("NonParallelCollection")]
public class AnchorBaselineFunctionalTests
{
    private const int ChildX = 200;
    private const int ChildY = 120;
    private const int ChildW = 150;
    private const int ChildH = 100;

    private const AnchorStyles R = AnchorStyles.Top | AnchorStyles.Right;
    private const AnchorStyles LR = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
    private const AnchorStyles B = AnchorStyles.Left | AnchorStyles.Bottom;
    private const AnchorStyles TB = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Bottom;
    private const AnchorStyles None = AnchorStyles.None;
    private const AnchorStyles All = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;

    /// <summary>Pumps the main loop until <paramref name="condition" /> holds or the timeout passes.</summary>
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

    /// <summary>Pumps the main loop for a fixed time, so queued allocations and timers run.</summary>
    private static void Settle(int ms)
    {
        var watch = Stopwatch.StartNew();
        while (watch.ElapsedMilliseconds < ms)
        {
            Application.DoEvents();
            Threading.Thread.Sleep(15);
        }
    }

    private static bool Allocated(Control control, int width, int height) =>
        control.Widget is { } widget && widget.AllocatedWidth == width && widget.AllocatedHeight == height;

    // Single-edge anchors in WinForms leave the other axis unanchored (None moves half), so each case
    // anchors one axis explicitly (Top|Left default).
    // Container grows by (+100,+60) or shrinks by (-100,-60) while the child is hidden. The expected
    // bounds are the child's designed bounds moved by AnchorBounds' rule for that delta.
    [Theory]
    [InlineData(R, 100, 60, 300, 120, 150, 100, false)]
    [InlineData(LR, 100, 60, 200, 120, 250, 100, false)]
    [InlineData(B, 100, 60, 200, 180, 150, 100, false)]
    [InlineData(TB, 100, 60, 200, 120, 150, 160, false)]
    [InlineData(None, 100, 60, 250, 150, 150, 100, false)]
    [InlineData(All, 100, 60, 200, 120, 250, 160, false)]
    [InlineData(R, -100, -60, 100, 120, 150, 100, false)]
    [InlineData(LR, -100, -60, 200, 120, 50, 100, false)]
    [InlineData(B, -100, -60, 200, 60, 150, 100, false)]
    [InlineData(TB, -100, -60, 200, 120, 150, 40, false)]
    [InlineData(None, -100, -60, 150, 90, 150, 100, false)]
    [InlineData(R, 100, 60, 300, 120, 150, 100, true)]
    [InlineData(LR, 100, 60, 200, 120, 250, 100, true)]
    [InlineData(B, 100, 60, 200, 180, 150, 100, true)]
    [InlineData(TB, -100, -60, 200, 120, 150, 40, true)]
    [InlineData(None, -100, -60, 150, 90, 150, 100, true)]
    public void HiddenChild_CatchesUpWithTheResizeItMissed_WhenShown(
        AnchorStyles anchor, int deltaWidth, int deltaHeight, int x, int y, int width, int height, bool hiddenFromStart)
    {
        var form = new Form { ClientSize = new Size(900, 700) };
        var container = new Panel { Location = new Point(10, 10), Size = new Size(500, 300) };
        var child = new Panel
        {
            Location = new Point(ChildX, ChildY),
            Size = new Size(ChildW, ChildH),
            Anchor = anchor,
        };
        container.Controls.Add(child);
        form.Controls.Add(container);
        form.ResumeLayout(false);
        if (hiddenFromStart)
        {
            child.Visible = false;
        }

        try
        {
            form.Show();
            Assert.True(PumpUntil(() => Allocated(container, 500, 300)), "container never got its real allocation");
            Settle(300);
            if (!hiddenFromStart)
            {
                child.Visible = false;
            }

            container.Size = new Size(500 + deltaWidth, 300 + deltaHeight);
            Assert.True(
                PumpUntil(() => Allocated(container, 500 + deltaWidth, 300 + deltaHeight)),
                "container was not resized");
            Settle(300);

            child.Visible = true;
            PumpUntil(() => child.Bounds == new Rectangle(x, y, width, height), 2000);

            Assert.Equal(new Rectangle(x, y, width, height), child.Bounds);
        }
        finally
        {
            form.Close();
        }
    }

    [Fact]
    public void ChildBoundsAssignedWhileHidden_AreNotMovedByAnEarlierResize()
    {
        var form = new Form { ClientSize = new Size(900, 700) };
        var container = new Panel { Location = new Point(10, 10), Size = new Size(500, 300) };
        var child = new Panel { Location = new Point(200, 120), Size = new Size(150, 100), Anchor = R };
        container.Controls.Add(child);
        form.Controls.Add(container);
        form.ResumeLayout(false);

        try
        {
            form.Show();
            Assert.True(PumpUntil(() => Allocated(container, 500, 300)));
            Settle(300);
            child.Visible = false;
            container.Size = new Size(600, 300);
            Assert.True(PumpUntil(() => Allocated(container, 600, 300)));
            Settle(300);

            // Positioned by code for the container as it is now: showing it must not apply the old resize.
            child.Left = 400;
            child.Visible = true;
            Settle(300);

            Assert.Equal(400, child.Left);
        }
        finally
        {
            form.Close();
        }
    }

    // A form enlarged after construction but before its first real allocation: in Load, before Show, and
    // (the content-fit watch must not swallow it) in Shown. Real WinForms: width +363 -> right-anchored
    // moves by 363, left+right stretches by 363.
    [Theory]
    [InlineData(1300, "load")]
    [InlineData(1100, "load")]
    [InlineData(1300, "before-show")]
    [InlineData(1100, "before-show")]
    [InlineData(1300, "shown")]
    [InlineData(1100, "shown")]
    public void FormEnlargedBeforeItsFirstAllocation_MovesAnchoredChildren(int newWidth, string when)
    {
        var form = new Form { ClientSize = new Size(937, 590) };
        var stretched = new Panel
        {
            Location = new Point(12, 40),
            Size = new Size(912, 100),
            Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
        };
        var right = new Button { Location = new Point(798, 8), Size = new Size(120, 24), Anchor = AnchorStyles.Top | AnchorStyles.Right };
        var fixedChild = new Button { Location = new Point(12, 8), Size = new Size(100, 24) };
        form.Controls.Add(stretched);
        form.Controls.Add(right);
        form.Controls.Add(fixedChild);
        form.ResumeLayout(false);

        var grow = newWidth - 937;
        void Enlarge() => form.Width = newWidth;
        switch (when)
        {
            case "load":
                form.Load += (_, _) => Enlarge();
                break;
            case "shown":
                form.Shown += (_, _) => Enlarge();
                break;
            default:
                Enlarge();
                break;
        }

        try
        {
            form.Show();
            Assert.True(PumpUntil(() => stretched.Width == 912 + grow, 6000),
                $"stretched panel is {stretched.Width} wide, expected {912 + grow}");
            Settle(3000);

            Assert.Equal(912 + grow, stretched.Width);
            Assert.Equal(12, stretched.Left);
            Assert.Equal(798 + grow, right.Left);
            Assert.Equal(120, right.Width);
            Assert.Equal(12, fixedChild.Left);
            Assert.Equal(100, fixedChild.Width);
        }
        finally
        {
            form.Close();
        }
    }

    // Height has window chrome (titlebar) in it, so the bottom anchor follows the real client height change,
    // not the change of the requested window height.
    [Theory]
    [InlineData(700)]
    [InlineData(800)]
    public void FormEnlargedInHeightInLoad_MovesBottomAnchoredChildByTheClientChange(int newHeight)
    {
        var form = new Form { ClientSize = new Size(937, 590) };
        var bottom = new Panel { Location = new Point(12, 500), Size = new Size(200, 40), Anchor = AnchorStyles.Bottom | AnchorStyles.Left };
        var tall = new Panel { Location = new Point(300, 40), Size = new Size(200, 300), Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left };
        form.Controls.Add(bottom);
        form.Controls.Add(tall);
        form.ResumeLayout(false);
        form.Load += (_, _) => form.Height = newHeight;

        try
        {
            form.Show();
            Assert.True(PumpUntil(() => bottom.Top > 500, 6000), "the enlarged height never reached the bottom-anchored child");
            Settle(3000);

            var clientGrow = form.ClientSize.Height - 590;
            Assert.True(clientGrow > 0);
            Assert.Equal(500 + clientGrow, bottom.Top);
            Assert.Equal(300 + clientGrow, tall.Height);
            Assert.Equal(40, tall.Top);
        }
        finally
        {
            form.Close();
        }
    }

    // The deliberate compensations: opening at the designed ClientSize (GTK titlebar, content-fit growth)
    // must leave every anchored child at its designed bounds.
    [Fact]
    public void FormOpenedAtItsDesignedSize_LeavesAnchoredChildrenWhereTheyWereDesigned()
    {
        var form = new Form { ClientSize = new Size(937, 590) };
        var bottomLabel = new Label { Location = new Point(12, 520), Size = new Size(200, 20), Anchor = AnchorStyles.Bottom | AnchorStyles.Left };
        var below = new Panel { Location = new Point(12, 545), Size = new Size(200, 30) };
        var right = new Panel { Location = new Point(700, 12), Size = new Size(200, 30), Anchor = AnchorStyles.Top | AnchorStyles.Right };
        var hidden = new Panel { Location = new Point(12, 12), Size = new Size(600, 400), Anchor = All, Visible = false };
        form.Controls.Add(bottomLabel);
        form.Controls.Add(below);
        form.Controls.Add(right);
        form.Controls.Add(hidden);
        form.ResumeLayout(false);

        try
        {
            form.Show();
            Assert.True(PumpUntil(() => form.Widget is { IsMapped: true } && right.Widget!.AllocatedWidth > 1));
            Settle(3000);

            Assert.Equal(new Rectangle(12, 520, 200, 20), bottomLabel.Bounds);
            Assert.Equal(new Rectangle(12, 545, 200, 30), below.Bounds);
            Assert.Equal(new Rectangle(700, 12, 200, 30), right.Bounds);

            // Compensation applies to hidden children too: shown later it is still at its designed bounds.
            hidden.Visible = true;
            Settle(300);
            Assert.Equal(new Rectangle(12, 12, 600, 400), hidden.Bounds);
        }
        finally
        {
            form.Close();
        }
    }
}
