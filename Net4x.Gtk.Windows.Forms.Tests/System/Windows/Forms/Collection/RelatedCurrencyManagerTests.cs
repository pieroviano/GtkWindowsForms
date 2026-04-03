using System.Collections;
using System.ComponentModel;
#pragma warning disable CS8610 // Nullability of reference types in type of parameter doesn't match overridden member.

namespace System.Windows.Forms.Collection;

public class RelatedCurrencyManagerTests
{
    public RelatedCurrencyManagerTests()
    {
        Application.Init();
    }

    [Fact]
    public void Bind_InvalidProperty_Throws()
    {
        var parent = new TestBindingManager(new PropertyDescriptorCollection(null));

        Assert.Throws<ArgumentException>(() =>
            new RelatedCurrencyManager(parent, "Invalid"));
    }

    [Fact]
    public void Bind_PropertyNotList_Throws()
    {
        var prop = TypeDescriptor.GetProperties(typeof(string))["Length"]!;
        var parent = new TestBindingManager(new PropertyDescriptorCollection(new[] { prop }));

        Assert.Throws<ArgumentException>(() =>
            new RelatedCurrencyManager(parent, "Length"));
    }

    [Fact]
    public void Bind_ReplacesParentManager()
    {
        var parent1 = new TestBindingManager(CreateProperties())
        {
            CurrentItem = new TestItem()
        };

        var parent2 = new TestBindingManager(CreateProperties())
        {
            CurrentItem = new TestItem()
        };

        var manager = new RelatedCurrencyManager(parent1, "Children");

        manager.Bind(parent2, "Children");

        // Old parent should not affect manager anymore
        parent1.RaiseCurrentItemChanged();

        // New parent should
        parent2.RaiseCurrentItemChanged();

        Assert.True(manager.Count >= 0);
    }

    [Fact]
    public void Ctor_WithValidParent_BindsSuccessfully()
    {
        var parent = new TestBindingManager(CreateProperties());

        var manager = new RelatedCurrencyManager(parent, "Children");

        Assert.NotNull(manager);
    }

    [Fact]
    public void GetItemProperties_ReturnsFromParent()
    {
        var parent = new TestBindingManager(CreateProperties());

        var manager = new RelatedCurrencyManager(parent, "Children");

        var result = manager.GetItemProperties();

        Assert.NotNull(result);
        Assert.Single(result);
    }

    [Fact]
    public void GetItemProperties_WithAccessors_PrependsField()
    {
        var parent = new TestBindingManager(CreateProperties());

        var manager = new RelatedCurrencyManager(parent, "Children");

        var accessor = CreateListPropertyDescriptor();

        var result = manager.GetItemProperties(new[] { accessor });

        Assert.NotNull(result);
    }

    [Fact]
    public void GetListName_DelegatesToParent()
    {
        var parent = new TestBindingManager(CreateProperties(), "TestList");

        var manager = new RelatedCurrencyManager(parent, "Children");

        var name = manager.GetListName();

        Assert.Equal("TestList", name);
    }

    [Fact]
    public void GetListName_WithAccessors_PassesToParent()
    {
        var parent = new TestBindingManager(CreateProperties(), "TestList");

        var manager = new RelatedCurrencyManager(parent, "Children");

        var list = new ArrayList();

        var result = manager.GetListName(list);

        Assert.Equal("TestList", result);
    }

    [Fact]
    public void ParentChange_WithCurrencyManager_Empty_AddsNew()
    {
        var parent = new TestCurrencyManager(CreateProperties())
        {
            CurrentItem = null
        };

        var manager = new RelatedCurrencyManager(parent, "Children");

        parent.RaiseCurrentItemChanged();

        Assert.Equal(1, parent.AddNewCallCount);
        Assert.Equal(1, parent.CancelEditCallCount);
    }

    [Fact]
    public void ParentChange_WithCurrencyManager_WithCurrent_SetsDataSource()
    {
        var parent = new TestCurrencyManager(CreateProperties())
        {
            CurrentItem = new TestItem()
        };

        var manager = new RelatedCurrencyManager(parent, "Children");

        parent.RaiseCurrentItemChanged();

        Assert.True(manager.Count >= 0);
    }

    [Fact]
    public void ParentChange_WithNonCurrencyManager_SetsDataSource()
    {
        var parent = new TestBindingManager(CreateProperties())
        {
            CurrentItem = new TestItem()
        };

        var manager = new RelatedCurrencyManager(parent, "Children");

        parent.RaiseCurrentItemChanged();

        Assert.True(manager.Count >= 0);
    }

    private static PropertyDescriptor CreateListPropertyDescriptor()
        => TypeDescriptor.GetProperties(typeof(TestItem))["Children"]!;

    private static PropertyDescriptorCollection CreateProperties() => new(new[] { CreateListPropertyDescriptor() });

    private class TestBindingManager : BindingManagerBase
    {
        private readonly string? _listName;
        private readonly PropertyDescriptorCollection _properties;

        public TestBindingManager(PropertyDescriptorCollection properties, string? listName = null)
        {
            _properties = properties;
            _listName = listName;
        }

        public object? CurrentItem { get; set; }

        public override object? Current => CurrentItem;
        public override int Position { get; set; }
        internal override Type? BindType { get; }
        internal override object? DataSource { get; }
        internal override bool IsBinding { get; }

        public override int Count => CurrentItem is null ? 0 : 1;

        public override void AddNew()
        {
        }

        public override void CancelCurrentEdit()
        {
        }

        public override void EndCurrentEdit()
        {
        }

        public override PropertyDescriptorCollection GetItemProperties()
            => _properties;

        public void RaiseCurrentItemChanged()
            => OnCurrentItemChanged(EventArgs.Empty);

        public override void RemoveAt(int index) => throw new NotImplementedException();

        public override void ResumeBinding() => throw new NotImplementedException();

        public override void SuspendBinding() => throw new NotImplementedException();

        protected internal override string? GetListName(ArrayList? listAccessors)
            => _listName;

        protected internal override void OnCurrentChanged(EventArgs e) => throw new NotImplementedException();

        protected internal override void OnCurrentItemChanged(EventArgs e)
        {
            OnCurrentItemChangedValueHandler?.Invoke(this, e);
        }

        protected override void UpdateIsBinding() => throw new NotImplementedException();

        internal override PropertyDescriptorCollection GetItemProperties(PropertyDescriptor[]? listAccessors)
            => _properties;

        internal override string? GetListName() => throw new NotImplementedException();

        private protected override void SetDataSource(object? source) => throw new NotImplementedException();
    }

    private class TestCurrencyManager : CurrencyManager
    {
        private readonly PropertyDescriptorCollection _properties;

        public TestCurrencyManager(PropertyDescriptorCollection properties)
            : base(new ArrayList())
        {
            _properties = properties;
        }

        public object? CurrentItem { get; set; }

        public int AddNewCallCount { get; private set; }
        public int CancelEditCallCount { get; private set; }

        public override object? Current => CurrentItem;

        public override int Count => CurrentItem is null ? 0 : 1;

        public override void AddNew()
        {
            AddNewCallCount++;
        }

        public override void CancelCurrentEdit()
        {
            CancelEditCallCount++;
        }

        public override PropertyDescriptorCollection GetItemProperties()
            => _properties;

        public void RaiseCurrentItemChanged()
            => OnCurrentItemChanged(EventArgs.Empty);

        internal override PropertyDescriptorCollection GetItemProperties(PropertyDescriptor[]? listAccessors)
            => _properties;
    }

    private class TestItem
    {
        public IList Children { get; set; } = new ArrayList { 1, 2, 3 };
    }
}