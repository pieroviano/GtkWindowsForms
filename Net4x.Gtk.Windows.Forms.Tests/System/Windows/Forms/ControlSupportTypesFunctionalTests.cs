namespace System.Windows.Forms;

/// <summary>
/// Verifies the controls-support types added for source compatibility: <see cref="LinkArea" />, the scroll
/// property objects, <see cref="NumericUpDownAcceleration" /> and its collection, the ToolStrip event-arg
/// types, and the <see cref="IMessageFilter" /> registration on <see cref="Application" />. All are model
/// types with no GTK widgets, so no form is needed.
/// </summary>
public class ControlSupportTypesFunctionalTests
{
    [Fact]
    public void LinkArea_ValueSemantics()
    {
        var a = new LinkArea(3, 5);
        Assert.Equal(3, a.Start);
        Assert.Equal(5, a.Length);
        Assert.False(a.IsEmpty);
        Assert.True(new LinkArea(0, 0).IsEmpty);

        Assert.Equal(new LinkArea(3, 5), a);
        Assert.True(new LinkArea(3, 5) == a);
        Assert.True(new LinkArea(1, 1) != a);
        Assert.Contains("Start=3", a.ToString());
    }

    [Fact]
    public void ScrollProperties_DefaultsAndRoundTrip()
    {
        var h = new HScrollProperties(null);
        Assert.True(h.Enabled);
        Assert.Equal(10, h.LargeChange);
        Assert.Equal(1, h.SmallChange);
        Assert.Equal(100, h.Maximum);
        Assert.Equal(0, h.Minimum);

        h.Value = 42;
        h.Visible = true;
        Assert.Equal(42, h.Value);
        Assert.True(h.Visible);

        Assert.IsAssignableFrom<ScrollProperties>(new VScrollProperties(null));
    }

    [Fact]
    public void NumericUpDownAcceleration_Validation()
    {
        var acc = new NumericUpDownAcceleration(2, 5m);
        Assert.Equal(2, acc.Seconds);
        Assert.Equal(5m, acc.Increment);

        Assert.Throws<ArgumentOutOfRangeException>(() => new NumericUpDownAcceleration(-1, 1m));
        Assert.Throws<ArgumentOutOfRangeException>(() => new NumericUpDownAcceleration(1, -1m));
    }

    [Fact]
    public void NumericUpDownAccelerationCollection_SortsBySecondsAndMutates()
    {
        var c = new NumericUpDownAccelerationCollection();
        Assert.False(c.IsReadOnly);

        var a3 = new NumericUpDownAcceleration(3, 10m);
        var a1 = new NumericUpDownAcceleration(1, 2m);
        c.Add(a3);
        c.Add(a1);

        // sorted ascending by Seconds
        Assert.Equal(2, c.Count);
        Assert.Same(a1, c[0]);
        Assert.Same(a3, c[1]);

        c.AddRange(new NumericUpDownAcceleration(2, 5m));
        Assert.Equal(3, c.Count);
        Assert.Equal(2, c[1].Seconds);

        Assert.True(c.Contains(a1));
        Assert.True(c.Remove(a1));
        Assert.Equal(2, c.Count);

        var seen = 0;
        foreach (var _ in c) { seen++; }
        Assert.Equal(2, seen);

        c.Clear();
        Assert.Equal(0, c.Count);
    }

    [Fact]
    public void ToolStripEventArgs_RoundTrip()
    {
        Assert.Equal(ToolStripDropDownCloseReason.ItemClicked,
            new ToolStripDropDownClosedEventArgs(ToolStripDropDownCloseReason.ItemClicked).CloseReason);

        var closing = new ToolStripDropDownClosingEventArgs(ToolStripDropDownCloseReason.Keyboard);
        Assert.Equal(ToolStripDropDownCloseReason.Keyboard, closing.CloseReason);
        Assert.False(closing.Cancel);
        closing.Cancel = true;
        Assert.True(closing.Cancel);
    }

    private sealed class ConsumingFilter : IMessageFilter
    {
        private readonly int _msg;
        public bool Called;

        public ConsumingFilter(int msg) => _msg = msg;

        public bool PreFilterMessage(ref Message m)
        {
            Called = true;
            return m.Msg == _msg;
        }
    }

    [Fact]
    public void Application_MessageFilter_AddFilterRemove()
    {
        var filter = new ConsumingFilter(0x0100);
        Application.AddMessageFilter(filter);
        try
        {
            var consumed = Message.Create(IntPtr.Zero, 0x0100, IntPtr.Zero, IntPtr.Zero);
            Assert.True(Application.FilterMessage(ref consumed));
            Assert.True(filter.Called);

            var other = Message.Create(IntPtr.Zero, 0x0200, IntPtr.Zero, IntPtr.Zero);
            Assert.False(Application.FilterMessage(ref other)); // filter runs but does not consume
        }
        finally
        {
            Application.RemoveMessageFilter(filter);
        }

        // after removal, the filter is not consulted
        var afterRemove = Message.Create(IntPtr.Zero, 0x0100, IntPtr.Zero, IntPtr.Zero);
        Assert.False(Application.FilterMessage(ref afterRemove));
    }
}
