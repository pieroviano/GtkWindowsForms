using System.Drawing;
using System.Windows.Forms.Layout;

#pragma warning disable CS0108, CS0114

namespace System.Windows.Forms.CollectionTests;

public class ArrangedElementTests
{
    [Fact]
    public void Bounds_InitiallyEmpty()
    {
        var sut = new TestArrangedElement();

        Assert.Equal(Rectangle.Empty, sut.Bounds);
    }

    [Fact]
    public void Constructor_SetsDefaults()
    {
        var sut = new TestArrangedElement();

        Assert.Equal(Padding.Empty.ToString(), sut.Padding.ToString());
        Assert.Equal(Padding.Empty.ToString(), sut.Margin.ToString());
        Assert.False(sut.Visible); // default state bit
    }

    [Fact]
    public void DisplayRectangle_ReturnsBounds()
    {
        var sut = new TestArrangedElement();
        var rect = new Rectangle(1, 2, 3, 4);

        sut.SetBounds(rect, BoundsSpecified.All);

        Assert.Equal(rect, sut.DisplayRectangle);
    }

    [Fact]
    public void ExplicitInterface_Children_ReturnsChildren()
    {
        var sut = new TestArrangedElement();
        var element = (IArrangedElement)sut;

        Assert.Equal(sut.Children, element.Children);
    }

    [Fact]
    public void ExplicitInterface_Container_ReturnsContainer()
    {
        var sut = new TestArrangedElement();
        var container = new TestArrangedElement();
        sut.Container = container;

        var element = (IArrangedElement)sut;

        Assert.Equal(container, element.Container);
    }

    [Fact]
    public void ExplicitInterface_Properties_ReturnsPropertyStore()
    {
        var sut = new TestArrangedElement();
        var element = (IArrangedElement)sut;

        Assert.NotNull(element.Properties);
    }

    [Fact]
    public void GetPreferredSize_ReturnsInput()
    {
        var sut = new TestArrangedElement();
        var size = new Size(100, 50);

        var result = sut.GetPreferredSize(size);

        Assert.Equal(size, result);
    }

    [Fact]
    public void Handle_ThrowsNotImplemented()
    {
        var sut = new TestArrangedElement();

        Assert.Throws<NotImplementedException>(() =>
        {
            var _ = sut.Handle;
        });
    }

    [Fact]
    public void LayoutEngine_ReturnsDefault()
    {
        var sut = new TestArrangedElement();

        Assert.NotNull(sut.LayoutEngine);
    }

    [Fact]
    public void Parent_SetAndGet_Works()
    {
        var sut = new TestArrangedElement();
        var parent = new TestArrangedElement();

        sut.Parent = parent;

        Assert.Equal(parent, sut.Parent);
    }

    [Fact]
    public void ParticipatesInLayout_ReturnsVisible()
    {
        var sut = new TestArrangedElement { Visible = true };

        Assert.True(sut.ParticipatesInLayout);

        sut.Visible = false;
        Assert.False(sut.ParticipatesInLayout);
    }

    [Fact]
    public void PerformLayout_RaisesLayoutEvent()
    {
        var sut = new TestArrangedElement();
        var called = false;

        sut.Layout += (s, e) =>
        {
            called = true;
            Assert.Equal("test", e.AffectedProperty);
        };

        sut.PerformLayout(sut, "test");

        Assert.True(called);
    }

    [Fact]
    public void SetBounds_ChangesBounds_AndCallsOnBoundsChanged()
    {
        var sut = new TestArrangedElement();
        var rect = new Rectangle(1, 2, 3, 4);

        sut.SetBounds(rect, BoundsSpecified.All);

        Assert.Equal(rect, sut.Bounds);
        Assert.True(sut.BoundsChangedCalled);
        Assert.Equal(Rectangle.Empty, sut.OldBounds);
        Assert.Equal(rect, sut.NewBounds);
    }

    [Fact]
    public void SetBounds_SameBounds_DoesNotTriggerChange()
    {
        var sut = new TestArrangedElement();
        var rect = new Rectangle(1, 2, 3, 4);

        sut.SetBounds(rect, BoundsSpecified.All);
        sut.BoundsChangedCalled = false;

        sut.SetBounds(rect, BoundsSpecified.All);

        Assert.False(sut.BoundsChangedCalled);
    }

    private class TestArrangedElement : ArrangedElement
    {
        public bool BoundsChangedCalled { get; internal set; }
        public Rectangle OldBounds { get; private set; }
        public Rectangle NewBounds { get; private set; }

        public ArrangedElementCollection Children { get; } = new();
        public IArrangedElement? Container { get; set; }

        public override LayoutEngine LayoutEngine => new();

        protected override ArrangedElementCollection GetChildren() => Children;

        protected override IArrangedElement? GetContainer() => Container;

        protected override void OnBoundsChanged(Rectangle oldBounds, Rectangle newBounds)
        {
            BoundsChangedCalled = true;
            OldBounds = oldBounds;
            NewBounds = newBounds;
        }
    }
}