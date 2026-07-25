using System.Drawing;
using System.Windows.Forms;

namespace Net4x.Gtk.Windows.Forms.Tests.System.Windows.Forms;

/// <summary>
/// Tests for the bounds-core / scaling / handle-lifecycle override surface (§5.2 of API-COMPLETENESS).
/// These protected methods were absent, so a custom control could not intercept sizing or handle
/// recreation. The tests pin the contract that makes overriding worthwhile: every bounds change funnels
/// through <c>SetBoundsCore</c>, scaling arithmetic matches WinForms, and <c>RecreateHandle</c> destroys
/// then creates.
/// </summary>
[Collection("NonParallelCollection")]
public class ControlScalingFunctionalTests
{
    static ControlScalingFunctionalTests()
    {
        using var _ = new Form();
    }

    private sealed class Probe : Button
    {
        public int SetBoundsCoreCalls { get; private set; }
        public BoundsSpecified LastSpecified { get; private set; }
        public string HandleLog { get; private set; } = "";

        protected override void SetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified)
        {
            SetBoundsCoreCalls++;
            LastSpecified = specified;
            base.SetBoundsCore(x, y, width, height, specified);
        }

        protected override void CreateHandle()
        {
            HandleLog += "C";
            base.CreateHandle();
        }

        protected override void DestroyHandle()
        {
            HandleLog += "D";
            base.DestroyHandle();
        }

        public Rectangle Scaled(Rectangle b, SizeF f, BoundsSpecified s) => GetScaledBounds(b, f, s);

        public Size ClientToSize(Size c) => SizeFromClientSize(c);

        public void DoScale(SizeF f, BoundsSpecified s) => ScaleControl(f, s);

        public void DoRecreate() => RecreateHandle();
    }

    [Fact]
    public void SetBounds_RoutesThroughSetBoundsCore()
    {
        using var probe = new Probe();

        probe.SetBounds(1, 2, 3, 4);

        Assert.Equal(1, probe.SetBoundsCoreCalls);
        Assert.Equal(BoundsSpecified.All, probe.LastSpecified);
    }

    [Fact]
    public void GetScaledBounds_ScalesLocationAndSizeIndependently()
    {
        using var probe = new Probe();

        var scaled = probe.Scaled(new Rectangle(10, 20, 30, 40), new SizeF(2f, 3f), BoundsSpecified.All);

        Assert.Equal(new Rectangle(20, 60, 60, 120), scaled);
    }

    [Fact]
    public void GetScaledBounds_HonoursTheSpecifiedComponents()
    {
        using var probe = new Probe();

        // Only Width selected: X, Y, Height stay put; Width doubles.
        var scaled = probe.Scaled(new Rectangle(10, 20, 30, 40), new SizeF(2f, 2f), BoundsSpecified.Width);

        Assert.Equal(new Rectangle(10, 20, 60, 40), scaled);
    }

    [Fact]
    public void SizeFromClientSize_IsIdentityWithNoNonClientBorder()
    {
        using var probe = new Probe();

        Assert.Equal(new Size(50, 60), probe.ClientToSize(new Size(50, 60)));
    }

    [Fact]
    public void ScaleControl_FunnelsThroughSetBoundsCore()
    {
        using var probe = new Probe();

        probe.DoScale(new SizeF(2f, 2f), BoundsSpecified.All);

        Assert.True(probe.SetBoundsCoreCalls >= 1, "scaling must go through the bounds-core override");
    }

    [Fact]
    public void RecreateHandle_DestroysThenCreates()
    {
        using var probe = new Probe();

        probe.DoRecreate();

        Assert.Equal("DC", probe.HandleLog);
    }
}
