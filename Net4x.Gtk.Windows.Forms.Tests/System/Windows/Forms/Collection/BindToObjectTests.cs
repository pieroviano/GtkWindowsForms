using System.Collections;
using System.ComponentModel;

#pragma warning disable CS8618
#pragma warning disable CS0067

namespace System.Windows.Forms.Collection;

public class BindToObjectTests
{
    [Fact]
    public void BindToType_WithField_ShouldReturnPropertyType()
    {
        var binding = CreateBinding();
        var sut = new BindToObject(binding, new object(), "Name");

        var manager = new TestBindingManager();
        manager.SetCurrent(new TestItem());

        // Ensure the Binding is aware of the manager so BindToObject.CheckBinding can access it.
        binding.SetListManager(manager);

        Assert.Equal(typeof(string), sut.BindToType);
    }

    [Fact]
    public void Ctor_ShouldInitializeBindingMemberInfo()
    {
        var binding = CreateBinding();

        var sut = new BindToObject(binding, new object(), "Name");

        Assert.Equal("Name", sut.BindingMemberInfo.BindingField);
    }

    [Fact]
    public void GetValue_ShouldPopulateErrorText_FromIDataErrorInfo()
    {
        var binding = CreateBinding();
        var sut = new BindToObject(binding, new object(), "Name");

        var manager = new TestBindingManager();
        var item = new TestItem();
        manager.SetCurrent(item);

        sut.SetBindingManagerBase(manager);

        sut.GetValue();

        Assert.Equal("FieldError", sut.DataErrorText);
    }

    [Fact]
    public void GetValue_ShouldReturnFieldValue()
    {
        var binding = CreateBinding();
        var sut = new BindToObject(binding, new object(), "Name");

        var manager = new TestBindingManager();
        var item = new TestItem { Name = "Hello" };
        manager.SetCurrent(item);

        sut.SetBindingManagerBase(manager);

        var value = sut.GetValue();

        Assert.Equal("Hello", value);
    }

    [Fact]
    public void GetValue_WhenNoManager_ShouldReturnNull()
    {
        var binding = CreateBinding();
        var sut = new BindToObject(binding, new object(), "Name");

        var value = sut.GetValue();

        Assert.Null(value);
    }

    [Fact]
    public void GetValue_WithNoField_ShouldReturnCurrent()
    {
        var binding = CreateBinding();
        var sut = new BindToObject(binding, new object(), "");

        var manager = new TestBindingManager();
        var item = new TestItem();
        manager.SetCurrent(item);

        sut.SetBindingManagerBase(manager);

        var value = sut.GetValue();

        Assert.Equal(item, value);
    }

    [Fact]
    public void SetBindingManagerBase_ShouldReplaceManager()
    {
        var binding = CreateBinding();
        var sut = new BindToObject(binding, new object(), "Name");

        var manager1 = new TestBindingManager();
        var manager2 = new TestBindingManager();

        sut.SetBindingManagerBase(manager1);
        sut.SetBindingManagerBase(manager2);

        Assert.Equal(manager2, sut.BindingManagerBase);
    }

    [Fact]
    public void SetValue_ReadOnlyProperty_ShouldNotThrow()
    {
        var binding = CreateBinding();
        var sut = new BindToObject(binding, new object(), "Name");

        var manager = new TestBindingManager();
        var item = new TestItem();
        manager.SetCurrent(item);

        sut.SetBindingManagerBase(manager);

        var ex = Record.Exception(() => sut.SetValue("Test"));

        Assert.Null(ex);
    }

    [Fact]
    public void SetValue_ShouldUpdateProperty()
    {
        var binding = CreateBinding();
        var sut = new BindToObject(binding, new object(), "Name");

        var manager = new TestBindingManager();
        var item = new TestItem();
        manager.SetCurrent(item);

        sut.SetBindingManagerBase(manager);

        sut.SetValue("Updated");

        Assert.Equal("Updated", item.Name);
    }

    private static Binding CreateBinding()
    {
        return new Binding("Text", new object(), "Name");
    }

    private class TestItem : IDataErrorInfo
    {
        public string Name { get; set; } = "Test";

        public string Error => "ErrorText";

        public string this[string columnName] => columnName == "Name" ? "FieldError" : "";
    }

    private class TestBindingManager : BindingManagerBase
    {
        private object? _current;

        public override object? Current => _current;
        public override int Position { get; set; }
        internal override Type? BindType { get; }
        internal override object? DataSource { get; }
        internal override bool IsBinding { get; }

        public override int Count => _current == null ? 0 : 1;

        public override void AddNew() => throw new NotImplementedException();

        public override void CancelCurrentEdit()
        {
        }

        public override event EventHandler? CurrentChanged;
        public override event EventHandler? CurrentItemChanged;

        public override void EndCurrentEdit()
        {
        }

        public override PropertyDescriptorCollection GetItemProperties() =>
            TypeDescriptor.GetProperties(typeof(TestItem));

        public override void RemoveAt(int index) => throw new NotImplementedException();

        public override void ResumeBinding() => throw new NotImplementedException();

        public void SetCurrent(object? value) => _current = value;

        public override void SuspendBinding() => throw new NotImplementedException();

        protected internal override string? GetListName(ArrayList? listAccessors) =>
            throw new NotImplementedException();

        protected internal override void OnCurrentChanged(EventArgs e)
        {
            CurrentChanged?.Invoke(this, e);
        }

        protected internal override void OnCurrentItemChanged(EventArgs e) => throw new NotImplementedException();

        protected override void UpdateIsBinding()
        {
        }

        internal override PropertyDescriptorCollection? GetItemProperties(PropertyDescriptor?[]? listAccessors) =>
            throw new NotImplementedException();

        internal override string? GetListName() => throw new NotImplementedException();
        private protected override void SetDataSource(object? source) => throw new NotImplementedException();
    }
}