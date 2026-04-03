using System.Reflection;
using System.Collections;
using System.ComponentModel;

namespace System.Windows.Forms.Collection;

public class ListManagerBindingsCollectionTests
{
    // Use a small concrete test implementation instead of a NSubstitute proxy.
    // DynamicProxy cannot implement internal abstract members on BindingManagerBase which
    // caused the TypeLoadException at test runtime.
    private readonly BindingManagerBase _bindingManager = new TestBindingManager();

    private Binding CreateBinding()
    {
        // Binding has no public interface → use real instance
        return new Binding("Text", new object(), "Prop");
    }

    // Reflection helper to call internal SetListManager on Binding
    private static void SetListManager(Binding binding, BindingManagerBase? manager)
    {
        var mi = typeof(Binding).GetMethod("SetListManager", BindingFlags.Instance | BindingFlags.NonPublic);
        if (mi != null)
        {
            mi.Invoke(binding, new object?[] { manager });
        }
        else
        {
            throw new MissingMethodException("Binding.SetListManager not found");
        }
    }

    // -------------------------
    // AddCore
    // -------------------------

    [Fact]
    public void AddCore_Null_Throws()
    {
        var sut = new ListManagerBindingsCollection(_bindingManager);

        Assert.Throws<ArgumentNullException>(() =>
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            sut.Add(null));
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
    }

    [Fact]
    public void AddCore_WhenBindingAlreadyHasManager_Throws()
    {
        var sut = new ListManagerBindingsCollection(_bindingManager);
        var binding = CreateBinding();

        // simulate already assigned manager (internal API -> use reflection)
        SetListManager(binding, _bindingManager);

        Assert.Throws<ArgumentException>(() =>
            sut.Add(binding));
    }

    [Fact]
    public void AddCore_WhenBindingHasDifferentManager_Throws()
    {
        var sut = new ListManagerBindingsCollection(_bindingManager);
        var binding = CreateBinding();

        var otherManager = new TestBindingManager();
        SetListManager(binding, otherManager);

        Assert.Throws<ArgumentException>(() =>
            sut.Add(binding));
    }

    [Fact]
    public void AddCore_ValidBinding_AssignsManagerAndAdds()
    {
        var sut = new ListManagerBindingsCollection(_bindingManager);
        var binding = CreateBinding();

        sut.Add(binding);

        Assert.Equal(_bindingManager, binding.BindingManagerBase);
        // BaseCollection exposes a non-generic enumerator. Use LINQ Cast to provide IEnumerable<Binding>.
        Assert.Contains(binding, sut.Cast<Binding>());
    }

    // -------------------------
    // ClearCore
    // -------------------------

    [Fact]
    public void ClearCore_RemovesManagerFromAllBindings()
    {
        var sut = new ListManagerBindingsCollection(_bindingManager);

        var b1 = CreateBinding();
        var b2 = CreateBinding();

        sut.Add(b1);
        sut.Add(b2);

        sut.Clear();

        Assert.Null(b1.BindingManagerBase);
        Assert.Null(b2.BindingManagerBase);
        Assert.Empty(sut);
    }

    // -------------------------
    // RemoveCore
    // -------------------------

    [Fact]
    public void RemoveCore_WhenBindingHasDifferentManager_Throws()
    {
        var sut = new ListManagerBindingsCollection(_bindingManager);
        var binding = CreateBinding();

        var otherManager = new TestBindingManager();
        SetListManager(binding, otherManager);

        Assert.Throws<ArgumentException>(() =>
            sut.Remove(binding));
    }

    [Fact]
    public void RemoveCore_ValidBinding_RemovesAndClearsManager()
    {
        var sut = new ListManagerBindingsCollection(_bindingManager);
        var binding = CreateBinding();

        sut.Add(binding);

        sut.Remove(binding);

        Assert.Null(binding.BindingManagerBase);
        // Use LINQ Cast so Assert.DoesNotContain has an IEnumerable<Binding>.
        Assert.DoesNotContain(binding, sut.Cast<Binding>());
    }

    // Minimal concrete BindingManagerBase for tests.
    internal sealed class TestBindingManager : BindingManagerBase
    {
        public override int Count => 0;
        public override object? Current => null;
        public override int Position { get; set; }

        internal override Type? BindType => null;
        internal override object? DataSource => null;
        internal override bool IsBinding => false;

        public override void AddNew() => throw new NotSupportedException();
        public override void CancelCurrentEdit() { }
        public override void EndCurrentEdit() { }
        public override void RemoveAt(int index) => throw new NotSupportedException();
        public override void ResumeBinding() { }
        public override void SuspendBinding() { }

        internal override PropertyDescriptorCollection? GetItemProperties(PropertyDescriptor?[]? listAccessors) => null;
        internal override string? GetListName() => null;
        protected internal override string? GetListName(ArrayList? listAccessors) => null;
        protected internal override void OnCurrentChanged(EventArgs e) { }
        protected internal override void OnCurrentItemChanged(EventArgs e) { }
        private protected override void SetDataSource(object? source) => throw new NotImplementedException();

        protected override void UpdateIsBinding() => throw new NotImplementedException();
    }
}