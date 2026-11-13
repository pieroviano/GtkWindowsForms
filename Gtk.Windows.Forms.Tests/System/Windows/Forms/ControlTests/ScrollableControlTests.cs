using System.Drawing;
using Gtk;
using Gtk.Windows.Forms.Controls.ControlBase;
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

namespace System.Windows.Forms.ControlTests;

[Collection("NonParallelCollection")]
public class ScrollableControlTests
{
    [Fact]
    public void AutoScroll_DoesNotSetBackend_WhenAutoSizeTrue()
    {
        var control = new TestScrollableControl();
        var fake = new FakeScrollableBox();

        control.SetScrolledWindow(fake);
        control.AutoSize = true;

        control.AutoScroll = true;

        Assert.False(fake.AutoScroll); // suppressed
    }

    [Fact]
    public void AutoScroll_SetsBackend_WhenAutoSizeFalse()
    {
        var control = new TestScrollableControl();
        var fake = new FakeScrollableBox();

        control.SetScrolledWindow(fake);
        control.AutoSize = false;

        control.AutoScroll = true;

        Assert.True(fake.AutoScroll);
    }

    [Fact]
    public void AutoScrollProperties_CanBeSet()
    {
        var control = new TestScrollableControl
        {
            AutoScrollMargin = new Size(1, 2),
            AutoScrollMinSize = new Size(3, 4),
            AutoScrollPosition = new Point(5, 6)
        };

        Assert.Equal(1, control.AutoScrollMargin.Width);
        Assert.Equal(4, control.AutoScrollMinSize.Height);
        Assert.Equal(5, control.AutoScrollPosition.X);
    }

    [Fact]
    public void AutoSize_False_RestoresAutoScroll()
    {
        var control = new TestScrollableControl();
        var fake = new FakeScrollableBox();

        control.SetScrolledWindow(fake);

        control.AutoScroll = true;
        control.AutoSize = true; // disables
        control.AutoSize = false; // should restore

        Assert.True(fake.AutoScroll);
    }

    [Fact]
    public void AutoSize_True_DisablesBackendAutoScroll()
    {
        var control = new TestScrollableControl();
        var fake = new FakeScrollableBox();

        control.SetScrolledWindow(fake);
        control.AutoScroll = true;

        control.AutoSize = true;

        Assert.False(fake.AutoScroll);
    }

    [Fact]
    public void HScroll_DelegatesToBackend()
    {
        var control = new TestScrollableControl();
        var fake = new FakeScrollableBox();

        control.SetScrolledWindow(fake);

        control.SetHScroll(true);

        Assert.True(fake.HScroll);
        Assert.True(control.GetHScroll());
    }

    [Fact]
    public void Scroll_Event_IsForwarded()
    {
        var control = new TestScrollableControl();
        var fake = new FakeScrollableBox();

        control.SetScrolledWindow(fake);

        var raised = false;
        control.Scroll += (_, _) => raised = true;

        fake.RaiseScroll();

        Assert.True(raised);
    }

    [Fact]
    public void Scroll_Event_NotThrown_WhenNoBackend()
    {
        var control = new TestScrollableControl();

        var raised = false;
        control.Scroll += (_, _) => raised = true;

        // no backend → nothing happens
        Assert.False(raised);
    }

    [Fact]
    public void VScroll_DelegatesToBackend()
    {
        var control = new TestScrollableControl();
        var fake = new FakeScrollableBox();

        control.SetScrolledWindow(fake);

        control.SetVScroll(true);

        Assert.True(fake.VScroll);
        Assert.True(control.GetVScroll());
    }

    public class TestScrollableControl : ScrollableControl
    {
        public bool GetHScroll() => HScroll;

        public bool GetVScroll() => VScroll;
        public void SetHScroll(bool value) => HScroll = value;
        public void SetVScroll(bool value) => VScroll = value;
    }

    public class FakeScrollableBox : IScrollableBoxBase
    {
        public bool AutoScroll { get; set; }
        public Widget Child { get; set; }
        public int HeightRequest { get; set; }
        public bool HScroll { get; set; }
        public GtkControlOverride Override { get; set; }
        public ScrolledWindow ScrolledWindow { get; set; }
        public bool VScroll { get; set; }
        public int WidthRequest { get; set; }
        public void Add(Widget child) => throw new NotImplementedException();
        public bool ScrollView(double hscrollValue, double vscrollValue) => throw new NotImplementedException();

        public void AddClass(string cssClass) => throw new NotImplementedException();

        public event ScrollEventHandler Scroll;

        public void RaiseScroll()
        {
            Scroll?.Invoke(this, new ScrollEventArgs(ScrollEventType.EndScroll, 0));
        }
    }
}