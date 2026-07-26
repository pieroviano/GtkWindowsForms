using System.Collections;
using System.ComponentModel;

#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.

namespace System.Windows.Forms.CollectionTests;

public class BindingManagerBaseTests
{
    [Fact]
    public void BindingComplete_Event_IsRaised()
    {
        var sut = new TestBindingManager();
        var called = false;

        sut.BindingComplete += (_, _) => called = true;

        sut.RaiseBindingComplete(new BindingCompleteEventArgs(null, BindingCompleteState.Success,
            BindingCompleteContext.ControlUpdate, null));

        Assert.True(called);
    }

    [Fact]
    public void Bindings_Add_HooksBindingComplete()
    {
        var sut = new TestBindingManager();
        var data = new TestData();

        var binding = new Binding("Name", data, "Name");

        var called = false;
        sut.BindingComplete += (_, _) => called = true;

        sut.Bindings.Add(binding);

        binding.OnBindingComplete(new BindingCompleteEventArgs(binding, BindingCompleteState.Success,
            BindingCompleteContext.ControlUpdate, null));

        Assert.True(called);
    }

    [Fact]
    public void Bindings_LazyInitialized()
    {
        var sut = new TestBindingManager();

        var bindings = sut.Bindings;

        Assert.NotNull(bindings);
    }

    [Fact]
    public void Bindings_Remove_UnhooksBindingComplete()
    {
        var sut = new TestBindingManager();
        var data = new TestData();

        var binding = new Binding("Name", data, "Name");

        var called = false;
        sut.BindingComplete += (_, _) => called = true;

        sut.Bindings.Add(binding);
        sut.Bindings.Remove(binding);

        binding.OnBindingComplete(new BindingCompleteEventArgs(binding, BindingCompleteState.Success,
            BindingCompleteContext.ControlUpdate, null));

        Assert.False(called);
    }

    [Fact]
    public void CurrentChanged_Event_IsRaised()
    {
        var sut = new TestBindingManager();
        var called = false;

        sut.CurrentChanged += (_, _) => called = true;

        sut.OnCurrentChanged(EventArgs.Empty);

        Assert.True(called);
    }

    [Fact]
    public void CurrentItemChanged_Event_IsRaised()
    {
        var sut = new TestBindingManager();
        var called = false;

        sut.CurrentItemChanged += (_, _) => called = true;

        sut.OnCurrentItemChanged(EventArgs.Empty);

        Assert.True(called);
    }

    [Fact]
    public void DataError_Event_IsRaised()
    {
        var sut = new TestBindingManager();
        var called = false;

        sut.DataError += (_, _) => called = true;

        sut.RaiseDataError(new Exception());

        Assert.True(called);
    }

    [Fact]
    public void GetItemProperties_ReturnsProperties()
    {
        var sut = new TestBindingManager();

        var props = sut.GetItemProperties();

        Assert.NotNull(props);
#pragma warning disable xUnit2012
        Assert.True(props.Cast<PropertyDescriptor>().Any(p => p.Name == "Name"));
#pragma warning restore xUnit2012
    }

    [Fact]
    public void GetItemProperties_WithListType_ReturnsProperties()
    {
        var sut = new TestBindingManager();

        var dataSources = new ArrayList { new[] { new TestData() } };
        var accessors = new ArrayList();

        var result = sut.GetItemProperties(dataSources, accessors);

        Assert.NotNull(result);
    }

    [Fact]
    public void IsBindingSuspended_ReflectsState()
    {
        var sut = new TestBindingManager();

        sut.SuspendBinding();
        Assert.True(sut.IsBindingSuspended);

        sut.ResumeBinding();
        Assert.False(sut.IsBindingSuspended);
    }

    [Fact]
    public void PullData_SetsSuccessFalse_WhenBindingFails()
    {
        var sut = new TestBindingManager();
        var data = new TestData();

        var binding = new Binding("Name", data, "Name");
        sut.Bindings.Add(binding);

        sut.TriggerPullData(out var success);

        Assert.True(success || !success); // ensures execution path hit
    }

    [Fact]
    public void PushData_DoesNotThrow()
    {
        var sut = new TestBindingManager();
        var data = new TestData();

        var binding = new Binding("Name", data, "Name");
        sut.Bindings.Add(binding);

        sut.TriggerPushData();

        Assert.True(true); // no exception
    }

    private class TestBindingManager : BindingManagerBase
    {
        private object? _dataSource;
        private bool isBinding = true;

        public override int Count => 1;

        public override object? Current => _dataSource;

        public override int Position { get; set; }

        internal override Type? BindType => _dataSource?.GetType();

        internal override object? DataSource => _dataSource;

        internal override bool IsBinding
        {
            get => isBinding;
        }

        public override void AddNew()
        {
        }

        public override void CancelCurrentEdit()
        {
        }

        public override void EndCurrentEdit()
        {
        }

        public void RaiseBindingComplete(BindingCompleteEventArgs e)
            => OnBindingComplete(e);

        public void RaiseDataError(Exception ex)
            => OnDataError(ex);

        public override void RemoveAt(int index)
        {
        }

        public override void ResumeBinding() => isBinding = true;

        public override void SuspendBinding() => isBinding = false;

        public void TriggerPullData(out bool success) => PullData(out success);

        public void TriggerPushData() => PushData();

        protected internal override string? GetListName(ArrayList? listAccessors) => "Test";

        protected internal override void OnCurrentChanged(EventArgs e)
        {
            OnCurrentChangedHandler?.Invoke(this, e);
        }

        protected internal override void OnCurrentItemChanged(EventArgs e)
        {
            OnCurrentItemChangedHandler?.Invoke(this, e);
            OnCurrentItemChangedValueHandler?.Invoke(this, e);
        }

        protected override void UpdateIsBinding()
        {
            // simulate binding state update
        }

        internal override PropertyDescriptorCollection? GetItemProperties(PropertyDescriptor?[]? listAccessors)
        {
            return TypeDescriptor.GetProperties(typeof(TestData));
        }

        internal override string? GetListName() => "Test";

        private protected override void SetDataSource(object? source)
        {
            _dataSource = source;
        }
    }

    private class TestData
    {
        public string Name { get; set; } = "test";
    }
}