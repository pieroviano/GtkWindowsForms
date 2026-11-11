#pragma warning disable CS0612

#pragma warning disable CS0612

using System.Diagnostics;
using System.Drawing;

namespace System.Windows.Forms;

/// <summary>
/// SplitterDistance is a WinForms value that has to survive GTK clamping the paned's position while the
/// control is hidden or has no allocation, and SplitterMoving/SplitterMoved report real moves only.
/// WinForms: the distance persists while hidden/unsized and is honoured once the control has its size; setting
/// it raises SplitterMoved; the user dragging raises SplitterMoving (cancellable) then SplitterMoved.
/// Needs a display (real windows, pumped main loop).
/// </summary>
[Collection("NonParallelCollection")]
public class SplitContainerDistanceTests
{
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

    private static bool Allocated(SplitContainer split) => split.self.AllocatedWidth > 1;

    private static Form HostedSplit(out Panel host, out SplitContainer split, bool hostVisible = true)
    {
        var form = new Form { ClientSize = new Size(800, 400) };
        form.SuspendLayout();
        host = new Panel { Location = new Point(0, 0), Size = new Size(700, 300), Visible = hostVisible };
        split = new SplitContainer { Location = new Point(0, 0), Size = new Size(700, 300) };
        host.Controls.Add(split);
        form.Controls.Add(host);
        form.ResumeLayout(false);

        return form;
    }

    [Fact]
    public void DistanceSetBeforeShow_IsHonouredOnceTheControlHasItsSize()
    {
        var form = HostedSplit(out _, out var split);
        split.SplitterDistance = 300;
        try
        {
            form.Show();
            Assert.True(PumpUntil(() => Allocated(split)));
            Settle(1500);

            Assert.Equal(300, split.SplitterDistance);
            Assert.Equal(295, split.self.Position);
        }
        finally
        {
            form.Close();
        }
    }

    [Fact]
    public void DistanceSurvivesTheParentBeingHiddenAtStartAndShownLater_WithoutMoveEvents()
    {
        var form = HostedSplit(out var host, out var split, hostVisible: false);
        split.SplitterDistance = 443;
        var moved = 0;
        var moving = 0;
        split.SplitterMoved += (_, _) => moved++;
        split.SplitterMoving += (_, _) => moving++;
        var anchor = new Button { Location = new Point(0, 320), Size = new Size(50, 24) };
        form.Controls.Add(anchor);
        try
        {
            form.Show();
            Assert.True(PumpUntil(() => anchor.Widget!.AllocatedWidth > 1));
            Settle(1000);
            Assert.Equal(443, split.SplitterDistance);

            host.Visible = true;
            Assert.True(PumpUntil(() => Allocated(split)));
            Settle(1500);

            Assert.Equal(443, split.SplitterDistance);
            Assert.Equal(438, split.self.Position);
            Assert.Equal(0, moved);
            Assert.Equal(0, moving);
        }
        finally
        {
            form.Close();
        }
    }

    // The case seen in an application: visible with a distance, its parent hidden (a tab switch), shown again.
    [Fact]
    public void DistanceSurvivesTheParentBeingHiddenAndShownAgain()
    {
        var form = HostedSplit(out var host, out var split);
        split.SplitterDistance = 443;
        var moved = 0;
        try
        {
            form.Show();
            Assert.True(PumpUntil(() => Allocated(split)));
            Settle(1500);
            Assert.Equal(443, split.SplitterDistance);
            split.SplitterMoved += (_, _) => moved++;

            host.Visible = false;
            Settle(800);
            Assert.Equal(443, split.SplitterDistance);

            host.Visible = true;
            Assert.True(PumpUntil(() => Allocated(split)));
            Settle(1500);

            Assert.Equal(443, split.SplitterDistance);
            Assert.Equal(438, split.self.Position);
            Assert.Equal(0, moved);
        }
        finally
        {
            form.Close();
        }
    }

    [Fact]
    public void DistanceSetWhileHidden_IsHonouredOnShow()
    {
        var form = HostedSplit(out var host, out var split, hostVisible: false);
        var anchor = new Button { Location = new Point(0, 320), Size = new Size(50, 24) };
        form.Controls.Add(anchor);
        try
        {
            form.Show();
            Assert.True(PumpUntil(() => anchor.Widget!.AllocatedWidth > 1));
            Settle(500);
            split.SplitterDistance = 250;
            Assert.Equal(250, split.SplitterDistance);

            host.Visible = true;
            Assert.True(PumpUntil(() => Allocated(split)));
            Settle(1500);

            Assert.Equal(250, split.SplitterDistance);
            Assert.Equal(245, split.self.Position);
        }
        finally
        {
            form.Close();
        }
    }

    [Fact]
    public void DistanceIsKeptWhenTheControlIsResized_WithPanel1Fixed()
    {
        var form = HostedSplit(out var host, out var split);
        split.FixedPanel = FixedPanel.Panel1;
        split.SplitterDistance = 200;
        var moved = 0;
        try
        {
            form.Show();
            Assert.True(PumpUntil(() => Allocated(split)));
            Settle(1500);
            split.SplitterMoved += (_, _) => moved++;

            host.Size = new Size(760, 300);
            split.Size = new Size(760, 300);
            Assert.True(PumpUntil(() => split.self.AllocatedWidth == 760));
            Settle(800);

            Assert.Equal(200, split.SplitterDistance);
            Assert.Equal(0, moved);
        }
        finally
        {
            form.Close();
        }
    }

    [Fact]
    public void SettingTheDistance_RaisesSplitterMovedOnlyWhenItChanges()
    {
        var form = HostedSplit(out _, out var split);
        try
        {
            form.Show();
            Assert.True(PumpUntil(() => Allocated(split)));
            Settle(1500);
            var args = new List<SplitterEventArgs>();
            split.SplitterMoved += (_, e) => args.Add(e);

            split.SplitterDistance = 220;
            split.SplitterDistance = 220;
            split.SplitterDistance = 180;
            Settle(300);

            Assert.Equal(2, args.Count);
            Assert.Equal(220, args[0].SplitX);
            Assert.Equal(180, args[1].SplitX);
        }
        finally
        {
            form.Close();
        }
    }

    // A user drag changes the paned's position while the control is allocated: Moving (cancellable) then Moved.
    [Fact]
    public void AUserMove_RaisesMovingThenMoved_AndCancellingPutsTheSplitterBack()
    {
        var form = HostedSplit(out _, out var split);
        split.SplitterDistance = 300;
        var order = new List<string>();
        try
        {
            form.Show();
            Assert.True(PumpUntil(() => Allocated(split)));
            Settle(1500);
            split.SplitterMoving += (_, e) => order.Add("moving:" + e.SplitX);
            split.SplitterMoved += (_, e) => order.Add("moved:" + e.SplitX);

            split.self.Position = 150;
            Settle(300);

            Assert.Equal(new[] { "moving:155", "moved:155" }, order);
            Assert.Equal(155, split.SplitterDistance);

            order.Clear();
            split.SplitterMoving += (_, e) => e.Cancel = true;
            split.self.Position = 100;
            Settle(300);

            Assert.Equal(new[] { "moving:105" }, order);
            Assert.Equal(155, split.SplitterDistance);
        }
        finally
        {
            form.Close();
        }
    }

    // A control made visible reports its size before GTK has laid it out as 2x2 (its borders): code that sizes
    // against ClientSize right after showing a panel ("panel width minus 50") must see the real size, as it does in
    // WinForms (a 700x300 panel with a 1 px border has a 698x298 client area; before layout the border is not
    // yet subtracted, so 698-700 is accepted).
    [Fact]
    public void PanelShownBeforeItsFirstLayout_ReportsItsRequestedClientSize()
    {
        var form = HostedSplit(out var host, out var split, hostVisible: false);
        host.BorderStyle = BorderStyle.FixedSingle;
        var anchor = new Button { Location = new Point(0, 320), Size = new Size(50, 24) };
        form.Controls.Add(anchor);
        try
        {
            form.Show();
            Assert.True(PumpUntil(() => anchor.Widget!.AllocatedWidth > 1));
            Settle(500);

            host.Visible = true;
            var clientSize = host.ClientSize;
            // What an application does on showing the panel: size the splitter against the panel.
            split.SplitterDistance = Math.Min(400, clientSize.Width - 50);
            Assert.True(PumpUntil(() => Allocated(split)));
            Settle(1500);

            Assert.InRange(clientSize.Width, 698, 700);
            Assert.InRange(clientSize.Height, 298, 300);
            Assert.Equal(400, split.SplitterDistance);
        }
        finally
        {
            form.Close();
        }
    }

    [Fact]
    public void FixedPanelNone_LetsBothPanelsResize()
    {
        var split = new SplitContainer { FixedPanel = FixedPanel.None };
        Assert.True(split.self.ResizeStartChild);
        Assert.True(split.self.ResizeEndChild);

        split.FixedPanel = FixedPanel.Panel1;
        Assert.False(split.self.ResizeStartChild);
        Assert.True(split.self.ResizeEndChild);

        split.FixedPanel = FixedPanel.Panel2;
        Assert.True(split.self.ResizeStartChild);
        Assert.False(split.self.ResizeEndChild);
    }
}
