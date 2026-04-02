using System.ComponentModel;

namespace System.Windows.Forms.Collection;

public class BindingsCollectionTests
{
    private static Binding CreateBinding()
    {
        return new Binding("Text", new TestData(), "Name");
    }

    private class TestData
    {
        public string Name { get; set; } = "test";
    }

    [Fact]
    public void Add_ShouldIncreaseCount_AndRaiseEvents()
    {
        var sut = new BindingsCollection();
        var binding = CreateBinding();

        var events = new List<string>();

        sut.CollectionChanging += (_, e) => events.Add($"changing:{e.Action}");
        sut.CollectionChanged += (_, e) => events.Add($"changed:{e.Action}");

        sut.Add(binding);

        Assert.Equal(1, sut.Count);
        Assert.Equal(binding, sut[0]);

        Assert.Equal(new[]
        {
            "changing:Add",
            "changed:Add"
        }, events);
    }

    [Fact]
    public void Remove_ShouldDecreaseCount_AndRaiseEvents()
    {
        var sut = new BindingsCollection();
        var binding = CreateBinding();
        sut.Add(binding);

        var events = new List<string>();

        sut.CollectionChanging += (_, e) => events.Add($"changing:{e.Action}");
        sut.CollectionChanged += (_, e) => events.Add($"changed:{e.Action}");

        sut.Remove(binding);

        Assert.Equal(0, sut.Count);

        Assert.Equal(new[]
        {
            "changing:Remove",
            "changed:Remove"
        }, events);
    }

    [Fact]
    public void RemoveAt_ShouldRemoveCorrectItem()
    {
        var sut = new BindingsCollection();
        var b1 = CreateBinding();
        var b2 = CreateBinding();

        sut.Add(b1);
        sut.Add(b2);

        sut.RemoveAt(0);

        Assert.Equal(1, sut.Count);
        Assert.Equal(b2, sut[0]);
    }

    [Fact]
    public void Clear_ShouldEmptyCollection_AndRaiseRefreshEvents()
    {
        var sut = new BindingsCollection { CreateBinding() };

        var events = new List<string>();

        sut.CollectionChanging += (_, e) => events.Add($"changing:{e.Action}");
        sut.CollectionChanged += (_, e) => events.Add($"changed:{e.Action}");

        sut.Clear();

        Assert.Equal(0, sut.Count);

        Assert.Equal(new[]
        {
            "changing:Refresh",
            "changed:Refresh"
        }, events);
    }

    [Fact]
    public void Indexer_ShouldReturnCorrectItem()
    {
        var sut = new BindingsCollection();
        var binding = CreateBinding();

        sut.Add(binding);

        Assert.Equal(binding, sut[0]);
    }

    [Fact]
    public void ShouldSerializeMyAll_WhenEmpty_ReturnsFalse()
    {
        var sut = new BindingsCollection();

        Assert.False(sut.ShouldSerializeMyAll());
    }

    [Fact]
    public void ShouldSerializeMyAll_WhenHasItems_ReturnsTrue()
    {
        var sut = new BindingsCollection { CreateBinding() };

        Assert.True(sut.ShouldSerializeMyAll());
    }

    [Fact]
    public void AddCore_CanBeOverridden()
    {
        var called = false;

        var sut = new TestBindingsCollection(() => called = true) { CreateBinding() };

        Assert.True(called);
    }

    [Fact]
    public void RemoveCore_CanBeOverridden()
    {
        var called = false;

        var sut = new TestBindingsCollection(onRemove: () => called = true);
        var binding = CreateBinding();

        sut.Add(binding);
        sut.Remove(binding);

        Assert.True(called);
    }

    [Fact]
    public void ClearCore_CanBeOverridden()
    {
        var called = false;

        var sut = new TestBindingsCollection(onClear: () => called = true) { CreateBinding() };

        sut.Clear();

        Assert.True(called);
    }

    [Fact]
    public void Events_CanBeUnsubscribed()
    {
        var sut = new BindingsCollection();
        var binding = CreateBinding();

        bool called = false;
        CollectionChangeEventHandler handler = (_, _) => called = true;

        sut.CollectionChanged += handler;
        sut.CollectionChanged -= handler;

        sut.Add(binding);

        Assert.False(called);
    }

    private class TestBindingsCollection : BindingsCollection
    {
        private readonly Action? _onAdd;
        private readonly Action? _onRemove;
        private readonly Action? _onClear;

        public TestBindingsCollection(
            Action? onAdd = null,
            Action? onRemove = null,
            Action? onClear = null)
        {
            _onAdd = onAdd;
            _onRemove = onRemove;
            _onClear = onClear;
        }

        protected override void AddCore(Binding dataBinding)
        {
            _onAdd?.Invoke();
            base.AddCore(dataBinding);
        }

        protected override void RemoveCore(Binding dataBinding)
        {
            _onRemove?.Invoke();
            base.RemoveCore(dataBinding);
        }

        protected override void ClearCore()
        {
            _onClear?.Invoke();
            base.ClearCore();
        }
    }
}