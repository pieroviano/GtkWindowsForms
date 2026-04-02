using System.ComponentModel;
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.

namespace System.Windows.Forms.Collection;

public class ControlBindingsCollectionTests
{
    private class TestComponent : Component, IBindableComponent
    {
        public ControlBindingsCollection DataBindings { get; }

        public TestComponent()
        {
            DataBindings = new ControlBindingsCollection(this);
        }

        public BindingContext? BindingContext { get; set; } = new BindingContext();
    }

    private class TestModel
    {
        public string Name { get; set; } = "John";
        public int Age { get; set; } = 30;
    }

    [Fact]
    public void Constructor_ShouldSetBindableComponent()
    {
        var comp = new TestComponent();

        Assert.NotNull(comp.DataBindings.BindableComponent);
        Assert.Equal(comp, comp.DataBindings.BindableComponent);
    }

    [Fact]
    public void Control_Property_ShouldReturnNull_ForNonControl()
    {
        var comp = new TestComponent();

        Assert.Null(comp.DataBindings.Control);
    }

    [Fact]
    public void DefaultDataSourceUpdateMode_ShouldBeOnValidation()
    {
        var comp = new TestComponent();

        Assert.Equal(DataSourceUpdateMode.OnValidation, comp.DataBindings.DefaultDataSourceUpdateMode);
    }

    [Fact]
    public void Add_Binding_ShouldWork()
    {
        var comp = new TestComponent();
        var model = new TestModel();

        var binding = comp.DataBindings.Add("Name", model, nameof(TestModel.Name));

        Assert.Single(comp.DataBindings);
        Assert.Equal(binding, comp.DataBindings[0]);
    }

    [Fact]
    public void Add_WithNullDataSource_ShouldThrow()
    {
        var comp = new TestComponent();

        Assert.Throws<ArgumentNullException>(() =>
            comp.DataBindings.Add("Name", null, "Name"));
    }

    [Fact]
    public void Indexer_ByPropertyName_ShouldBeCaseInsensitive()
    {
        var comp = new TestComponent();
        var model = new TestModel();

        comp.DataBindings.Add("Name", model, "Name");

        var binding = comp.DataBindings["name"];

        Assert.NotNull(binding);
        Assert.Equal("Name", binding.PropertyName);
    }

    [Fact]
    public void Indexer_ShouldReturnNull_WhenNotFound()
    {
        var comp = new TestComponent();

        var result = comp.DataBindings["missing"];

        Assert.Null(result);
    }

    [Fact]
    public void Remove_ShouldRemoveBinding()
    {
        var comp = new TestComponent();
        var model = new TestModel();

        var binding = comp.DataBindings.Add("Name", model, "Name");

        comp.DataBindings.Remove(binding);

        Assert.Empty(comp.DataBindings);
    }

    [Fact]
    public void Remove_Null_ShouldThrow()
    {
        var comp = new TestComponent();

        Assert.Throws<ArgumentNullException>(() =>
            comp.DataBindings.Remove(null));
    }

    [Fact]
    public void Remove_NotOwnedBinding_ShouldThrow()
    {
        var comp1 = new TestComponent();
        var comp2 = new TestComponent();
        var model = new TestModel();

        var binding = comp1.DataBindings.Add("Name", model, "Name");

        Assert.Throws<ArgumentException>(() =>
            comp2.DataBindings.Remove(binding));
    }

    [Fact]
    public void RemoveAt_ShouldRemoveCorrectBinding()
    {
        var comp = new TestComponent();
        var model = new TestModel();

        comp.DataBindings.Add("Name", model, "Name");
        comp.DataBindings.Add("Age", model, "Age");

        comp.DataBindings.RemoveAt(0);

        Assert.Single(comp.DataBindings);
        Assert.Equal("Age", comp.DataBindings[0].PropertyName);
    }

    [Fact]
    public void Clear_ShouldRemoveAllBindings()
    {
        var comp = new TestComponent();
        var model = new TestModel();

        comp.DataBindings.Add("Name", model, "Name");
        comp.DataBindings.Add("Age", model, "Age");

        comp.DataBindings.Clear();

        Assert.Empty(comp.DataBindings);
    }

    [Fact]
    public void Clear_ShouldDetachBindingsFromComponent()
    {
        var comp = new TestComponent();
        var model = new TestModel();

        var binding = comp.DataBindings.Add("Name", model, "Name");

        comp.DataBindings.Clear();

        Assert.Null(binding.BindableComponent);
    }

    [Fact]
    public void AddCore_ShouldSetBindableComponent()
    {
        var comp = new TestComponent();
        var model = new TestModel();

        var binding = comp.DataBindings.Add("Name", model, "Name");

        Assert.Equal(comp, binding.BindableComponent);
    }

    [Fact]
    public void AddCore_BindingAlreadyAssigned_ShouldThrow()
    {
        var comp = new TestComponent();
        var model = new TestModel();

        var binding = new Binding("Name", model, "Name");

        // simulate already assigned
        binding.SetBindableComponent(comp);

        Assert.Throws<ArgumentException>(() =>
            comp.DataBindings.Add(binding));
    }

    [Fact]
    public void CheckDuplicates_ShouldThrow_WhenDuplicateProperty()
    {
        var comp = new TestComponent();
        var model = new TestModel();

        var b1 = comp.DataBindings.Add("Name", model, "Name");
        var b2 = new Binding("Name", model, "Name");

        Assert.Throws<ArgumentException>(() =>
            comp.DataBindings.CheckDuplicates(b2));
    }

    [Fact]
    public void CheckDuplicates_ShouldNotThrow_WhenDifferentProperty()
    {
        var comp = new TestComponent();
        var model = new TestModel();

        comp.DataBindings.Add("Name", model, "Name");
        var b2 = new Binding("Age", model, "Age");

        comp.DataBindings.CheckDuplicates(b2);
    }
}