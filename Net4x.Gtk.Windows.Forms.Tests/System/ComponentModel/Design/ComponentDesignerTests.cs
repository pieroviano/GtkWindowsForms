using System.Collections;

namespace System.ComponentModel.Design;

public class ComponentDesignerTests
{
    // Subclass for NotImplementedException tests
    internal class NotImplementedComponentDesigner : ComponentDesigner
    {
        public override System.Collections.ICollection Children => throw new NotImplementedException();
        public override IComponent Component => throw new NotImplementedException();
        public override IDesigner Parent { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public override DesignerVerbCollection Verbs => throw new NotImplementedException();
    }

    private readonly ComponentDesigner _sut = new NotImplementedComponentDesigner();

    [Fact]
    public void Children_ShouldThrow_NotImplementedException()
    {
        var designer = new NotImplementedComponentDesigner();
        Assert.Throws<NotImplementedException>(() => { var _ = designer.Children; });
    }

    [Fact]
    public void Component_ShouldThrow_NotImplementedException()
    {
        var designer = new NotImplementedComponentDesigner();
        Assert.Throws<NotImplementedException>(() => { var _ = designer.Component; });
    }

    [Fact]
    public void Parent_ShouldThrow_NotImplementedException()
    {
        var designer = new NotImplementedComponentDesigner();
        Assert.Throws<NotImplementedException>(() => { var _ = designer.Parent; });
    }

    [Fact]
    public void Verbs_ShouldThrow_NotImplementedException()
    {
        var designer = new NotImplementedComponentDesigner();
        Assert.Throws<NotImplementedException>(() => { var _ = designer.Verbs; });
    }

    [Fact]
    public void Dispose_ShouldNotThrow()
    {
        _sut.Dispose();
    }

    [Fact]
    public void DoDefaultAction_ShouldNotThrow()
    {
        _sut.DoDefaultAction();
    }

    [Fact]
    public void Initialize_ShouldNotThrow()
    {
        var component = new TestComponent();

        _sut.Initialize(component);
    }

    [Fact]
    public void InitializeExistingComponent_ShouldNotThrow()
    {
        IDictionary dict = new Hashtable();

        _sut.InitializeExistingComponent(dict);
    }

    [Fact]
    public void InitializeNewComponent_ShouldNotThrow()
    {
        IDictionary dict = new Hashtable();

        _sut.InitializeNewComponent(dict);
    }

    [Fact]
    public void PostFilterAttributes_ShouldNotThrow()
    {
        IDictionary dict = new Hashtable();

        _sut.PostFilterAttributes(dict);
    }

    [Fact]
    public void PostFilterEvents_ShouldNotThrow()
    {
        IDictionary dict = new Hashtable();

        _sut.PostFilterEvents(dict);
    }

    [Fact]
    public void PostFilterProperties_ShouldNotThrow()
    {
        IDictionary dict = new Hashtable();

        _sut.PostFilterProperties(dict);
    }

    [Fact]
    public void PreFilterAttributes_ShouldNotThrow()
    {
        IDictionary dict = new Hashtable();

        _sut.PreFilterAttributes(dict);
    }

    [Fact]
    public void PreFilterEvents_ShouldNotThrow()
    {
        IDictionary dict = new Hashtable();

        _sut.PreFilterEvents(dict);
    }

    [Fact]
    public void PreFilterProperties_ShouldNotThrow()
    {
        IDictionary dict = new Hashtable();

        _sut.PreFilterProperties(dict);
    }

    private class TestComponent : IComponent
    {
        public ISite Site { get; set; }

        public event EventHandler Disposed;

        public void Dispose()
        {
            Disposed?.Invoke(this, EventArgs.Empty);
        }
    }
}