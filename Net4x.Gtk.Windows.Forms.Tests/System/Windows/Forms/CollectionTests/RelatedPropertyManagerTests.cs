using System.Collections;
using System.ComponentModel;
using Application = System.Windows.Forms.Application;

#pragma warning disable CS8610 // Nullability of reference types in type of parameter doesn't match overridden member.

namespace System.Windows.Forms.CollectionTests;

public class RelatedPropertyManagerTests
{
    public RelatedPropertyManagerTests()
    {
        Application.Init(true);
    }

    [Fact]
    public void Bind_InvalidProperty_Throws()
    {
        var parent = new TestBindingManager(new PropertyDescriptorCollection(null));

        Assert.Throws<ArgumentException>(() =>
            new RelatedPropertyManager(parent, "Invalid"));
    }

    [Fact]
    public void BindType_ReturnsPropertyType()
    {
        var parent = new TestBindingManager(CreateProperties())
        {
            CurrentItem = new TestItem(),
            CountValue = 1,
            PositionValue = 0
        };

        var manager = new RelatedPropertyManager(parent, "Name");

        Assert.Equal(typeof(string), manager.BindType);
    }

    [Fact]
    public void Ctor_WithValidParent_BindsSuccessfully()
    {
        var parent = new TestBindingManager(CreateProperties())
        {
            CurrentItem = new TestItem(),
            CountValue = 1,
            PositionValue = 0
        };

        var manager = new RelatedPropertyManager(parent, "Name");

        Assert.NotNull(manager);
    }

    [Fact]
    public void Current_ReturnsNull_WhenDataSourceNull()
    {
        var parent = new TestBindingManager(CreateProperties())
        {
            CurrentItem = null,
            CountValue = 0,
            PositionValue = -1
        };

        var manager = new RelatedPropertyManager(parent, "Name");

        var current = manager.Current;

        Assert.Null(current);
    }

    [Fact]
    public void Current_ReturnsPropertyValue()
    {
        var item = new TestItem { Name = "Expected" };

        var parent = new TestBindingManager(CreateProperties())
        {
            CurrentItem = item,
            CountValue = 1,
            PositionValue = 0
        };

        var manager = new RelatedPropertyManager(parent, "Name");

        Assert.Equal("Expected", manager.Current);
    }

    [Fact]
    public void GetCurrentOrNull_OutOfBounds_ReturnsNull()
    {
        var parent = new TestBindingManager(CreateProperties())
        {
            CurrentItem = new TestItem(),
            CountValue = 1,
            PositionValue = 5 // invalid
        };

        var manager = new RelatedPropertyManager(parent, "Name");

        Assert.Null(manager.Current);
    }

    [Fact]
    public void GetItemProperties_DelegatesToParent()
    {
        var parent = new TestBindingManager(CreateProperties())
        {
            CurrentItem = new TestItem(),
            CountValue = 1,
            PositionValue = 0
        };

        var manager = new RelatedPropertyManager(parent, "Name");

        var result = manager.GetItemProperties(null);

        Assert.NotNull(result);
    }

    [Fact]
    public void GetItemProperties_WithAccessors_PrependsField()
    {
        var parent = new TestBindingManager(CreateProperties())
        {
            CurrentItem = new TestItem(),
            CountValue = 1,
            PositionValue = 0
        };

        var manager = new RelatedPropertyManager(parent, "Name");

        var accessor = CreateProperty();

        var result = manager.GetItemProperties(new[] { accessor });

        Assert.NotNull(result);
    }

    [Fact]
    public void GetListName_DelegatesToParent()
    {
        var parent = new TestBindingManager(CreateProperties())
        {
            CurrentItem = new TestItem(),
            CountValue = 1,
            PositionValue = 0
        };

        var manager = new RelatedPropertyManager(parent, "Name");

        var name = manager.GetListName();

        Assert.Equal("TestList", name);
    }

    [Fact]
    public void ParentChange_TriggersRefresh()
    {
        var item1 = new TestItem { Name = "First" };
        var item2 = new TestItem { Name = "Second" };

        var parent = new TestBindingManager(CreateProperties())
        {
            CurrentItem = item1,
            CountValue = 1,
            PositionValue = 0
        };

        var manager = new RelatedPropertyManager(parent, "Name");

        Assert.Equal("First", manager.Current);

        // Change parent current
        parent.CurrentItem = item2;

        parent.RaiseCurrentItemChanged();

        Assert.Equal("Second", manager.Current);
    }

    private static PropertyDescriptorCollection CreateProperties() => new(new[] { CreateProperty() });

    private static PropertyDescriptor CreateProperty()
        => TypeDescriptor.GetProperties(typeof(TestItem))["Name"]!;

    private class TestBindingManager : BindingManagerBase
    {
        private readonly PropertyDescriptorCollection _properties;

        public TestBindingManager(PropertyDescriptorCollection properties)
        {
            _properties = properties;
        }

        public object? CurrentItem { get; set; }
        public int PositionValue { get; set; }
        public int CountValue { get; set; }

        public override object? Current => CurrentItem;

        public override int Count => CountValue;

        public override int Position
        {
            get => PositionValue;
            set => PositionValue = value;
        }

        internal override Type? BindType { get; }
        internal override object? DataSource { get; }
        internal override bool IsBinding { get; }

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
            => "TestList";

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

    private class TestItem
    {
        public string Name { get; set; } = "Hello";
    }
}