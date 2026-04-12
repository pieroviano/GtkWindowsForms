extern alias gtk;

using System.ComponentModel;
using System.ComponentModel.Design;
using gtk::System.Drawing.Design;
#pragma warning disable CS8767 // Nullability of reference types in type of parameter doesn't match implicitly implemented member (possibly because of nullability attributes).
#pragma warning disable CS8603 // Possible null reference return.
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
#pragma warning disable CS8602 // Dereference of a possibly null reference.
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.

namespace System.Windows.Forms.Design;

public class DocumentDesignerTests
{
    [Fact]
    public void SupportedTechnologies_ReturnsDefault()
    {
        var designer = CreateDesigner();

        var result = designer.SupportedTechnologies;

        Assert.Single(result);
        Assert.Equal(ViewTechnology.Default, result[0]);
    }

    [Fact]
    public void GetToolSupported_Null_ReturnsFalse()
    {
        var designer = CreateDesigner();

        var result = designer.GetToolSupported(null);

        Assert.False(result);
    }

    [Fact]
    public void GetToolSupported_ControlType_ReturnsTrue()
    {
        var designer = CreateDesigner();
        var toolboxItem = new ToolboxItem(typeof(Button));

        var result = designer.GetToolSupported(toolboxItem);

        Assert.True(result);
    }

    [Fact]
    public void GetToolSupported_NonControlType_ReturnsFalse()
    {
        var designer = CreateDesigner();
        var toolboxItem = new ToolboxItem(typeof(string));

        var result = designer.GetToolSupported(toolboxItem);

        Assert.False(result);
    }

    [Fact]
    public void GetView_InvalidTechnology_Throws()
    {
        var designer = CreateDesigner();

        Assert.Throws<ArgumentException>(() =>
            designer.GetView((ViewTechnology)999));
    }

    [Fact]
    public void GetView_ComponentIsControl_ReturnsControl()
    {
        var designer = CreateDesignerWithRoot(new Form());

        var view = designer.GetView(ViewTechnology.Default);

        Assert.IsType<Form>(view);
    }

    [Fact]
    public void GetView_ComponentNotControl_Throws()
    {
        var designer = CreateDesignerWithRoot(new TestComponent());

        Assert.Throws<NotSupportedException>(() =>
            designer.GetView(ViewTechnology.Default));
    }

    [Fact]
    public void ToolPicked_Null_DoesNothing()
    {
        var designer = CreateDesigner();

        var ex = Record.Exception(() => designer.ToolPicked(null));

        Assert.Null(ex);
    }

    [Fact]
    public void ToolPicked_NoHost_DoesNothing()
    {
        var designer = CreateDesignerWithRoot(new Form());

        var toolboxItem = new ToolboxItem(typeof(Button));

        var ex = Record.Exception(() => designer.ToolPicked(toolboxItem));

        Assert.Null(ex);
    }

    [Fact]
    public void ToolPicked_AddsControl_WhenHostAvailable()
    {
        var root = new Form();
        var host = new FakeDesignerHost(root);

        var designer = CreateDesignerWithHost(root, host);

        var toolboxItem = new ToolboxItem(typeof(Button));

        designer.ToolPicked(toolboxItem);

        Assert.Single(root.Controls);
        Assert.IsType<Button>(root.Controls[0]);
    }

    // Helpers

    private static DocumentDesigner CreateDesigner()
    {
        return (DocumentDesigner)Activator.CreateInstance(typeof(DocumentDesigner), nonPublic: true);
    }

    private static DocumentDesigner CreateDesignerWithRoot(IComponent component)
    {
        var designer = CreateDesigner();
        designer.Component = component;
        return designer;
    }

    private static DocumentDesigner CreateDesignerWithHost(Control root, IDesignerHost host)
    {
        var designer = CreateDesigner();

        var site = new FakeSite(host);
        root.Site = site;

        designer.Component = root;

        return designer;
    }

    // Fake implementations

    class TestComponent : Component { }

    class FakeSite : ISite
    {
        private readonly IServiceProvider _provider;

        public FakeSite(IServiceProvider provider) => _provider = provider;

        public IComponent Component => null;
        public IContainer Container => null;
        public bool DesignMode => true;
        public string Name { get; set; }

        public object GetService(Type serviceType) => _provider.GetService(serviceType);
    }

    class FakeDesignerHost : IDesignerHost
    {
        public IComponent RootComponent { get; }
        public string RootComponentClassName { get; set; }

        public FakeDesignerHost(IComponent root) => RootComponent = root;

        public DesignerTransaction CreateTransaction() => throw new NotImplementedException();

        public DesignerTransaction CreateTransaction(string description) => new FakeTransaction();

        public object GetService(Type serviceType)
            => serviceType == typeof(IDesignerHost) ? this : null;

        // Minimal required members
        public void Activate() { }
        public IContainer Container => null;
        public bool InTransaction => false;
        public bool Loading => false;
        public string TransactionDescription => "";
        public event EventHandler Activated { add { } remove { } }
        public event EventHandler Deactivated { add { } remove { } }
        public event EventHandler LoadComplete { add { } remove { } }
        public event DesignerTransactionCloseEventHandler TransactionClosed { add { } remove { } }
        public event DesignerTransactionCloseEventHandler TransactionClosing { add { } remove { } }
        public event EventHandler TransactionOpened { add { } remove { } }
        public event EventHandler TransactionOpening { add { } remove { } }

        public IComponent CreateComponent(Type componentType) => Activator.CreateInstance(componentType) as IComponent;
        public IComponent CreateComponent(Type componentType, string name) => CreateComponent(componentType);
        public void DestroyComponent(IComponent component) { }
        public IDesigner GetDesigner(IComponent component) => null;
        public Type GetType(string typeName) => Type.GetType(typeName);
        public void AddService(Type serviceType, ServiceCreatorCallback callback) => throw new NotImplementedException();

        public void AddService(Type serviceType, ServiceCreatorCallback callback, bool promote) => throw new NotImplementedException();

        public void AddService(Type serviceType, object serviceInstance) => throw new NotImplementedException();

        public void AddService(Type serviceType, object serviceInstance, bool promote) => throw new NotImplementedException();

        public void RemoveService(Type serviceType) => throw new NotImplementedException();

        public void RemoveService(Type serviceType, bool promote) => throw new NotImplementedException();
    }

    class FakeTransaction : DesignerTransaction
    {
        protected override void OnCommit() { }
        protected override void OnCancel() { }
    }
}