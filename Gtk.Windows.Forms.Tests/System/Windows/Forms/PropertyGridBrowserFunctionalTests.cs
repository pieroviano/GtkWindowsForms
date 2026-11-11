#pragma warning disable CS0612

#pragma warning disable CS0612

using System.ComponentModel;
using System.ComponentModel.Design;
using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Windows.Forms;
using System.Windows.Forms.ComponentModel.Com2Interop;
using System.Windows.Forms.PropertyGridInternal;

namespace GtkWinForms.Tests.System.Windows.Forms;

/// <summary>
/// Behavioural tests for the property browser's editing machinery.
/// <para>
/// <see cref="GridEntry" /> claims to be an <see cref="ITypeDescriptorContext" /> - that is what a type
/// converter or UI type editor is handed while a property is being edited - but every member of that
/// contract threw <see cref="NotImplementedException" />, so any conversion that consulted its context
/// failed at runtime. The six <see cref="IComPropertyBrowser" /> members threw as well, and
/// <c>PropertyGrid.InPropertySet</c> answered a constant <see langword="true" />.
/// </para>
/// </summary>
[Collection("NonParallelCollection")]
public class PropertyGridBrowserFunctionalTests
{
    /// <summary>A sited component, so the entry has a container and a service provider to defer to.</summary>
    private sealed class Widget : Component
    {
        public string Caption { get; set; } = "before";

        public int Count { get; set; }
    }

    private sealed class StubContainer : Container
    {
    }

    private sealed class StubSite : ISite
    {
        private readonly Dictionary<Type, object> _services = new();

        public StubSite(IComponent component, IContainer container)
        {
            Component = component;
            Container = container;
        }

        public IComponent Component { get; }

        public IContainer Container { get; }

        public bool DesignMode => true;

        public string? Name { get; set; }

        public void AddService(Type serviceType, object service) => _services[serviceType] = service;

        public object? GetService(Type serviceType) =>
            _services.TryGetValue(serviceType, out var service) ? service : null;
    }

    /// <summary>Records what the entry reports, and can refuse a change the way a designer checkout does.</summary>
    private sealed class RecordingChangeService : IComponentChangeService
    {
        public List<(object Component, MemberDescriptor? Member)> Changing { get; } = new();

        public List<(object Component, MemberDescriptor? Member)> Changed { get; } = new();

        public bool RefuseChanges { get; set; }

#pragma warning disable CS0067 // the interface requires these events; nothing here raises them
        public event ComponentEventHandler? ComponentAdded;

        public event ComponentEventHandler? ComponentAdding;

        public event ComponentChangedEventHandler? ComponentChanged;

        public event ComponentChangingEventHandler? ComponentChanging;

        public event ComponentEventHandler? ComponentRemoved;

        public event ComponentEventHandler? ComponentRemoving;

        public event ComponentRenameEventHandler? ComponentRename;
#pragma warning restore CS0067

        public void OnComponentChanged(object component, MemberDescriptor? member, object? oldValue, object? newValue) =>
            Changed.Add((component, member));

        public void OnComponentChanging(object component, MemberDescriptor? member)
        {
            if (RefuseChanges)
            {
                throw CheckoutException.Canceled;
            }

            Changing.Add((component, member));
        }
    }

    private static GridEntry EntryFor(Widget widget, string property, PropertyGrid? ownerGrid = null) =>
        new(null, GridItemType.Property, 1, property, widget, "description")
        {
            Editable = true,
            ValueType = typeof(Widget).GetProperty(property)!.PropertyType,
            PropertyInfo = typeof(Widget).GetProperty(property),
            OwnerGrid = ownerGrid
        };

    private static (Widget Widget, StubSite Site, RecordingChangeService Changes) SitedWidget()
    {
        var widget = new Widget();
        var site = new StubSite(widget, new StubContainer());
        var changes = new RecordingChangeService();
        site.AddService(typeof(IComponentChangeService), changes);
        widget.Site = site;

        return (widget, site, changes);
    }

    [Fact]
    public void Instance_IsTheObjectWhosePropertyIsBeingEdited()
    {
        var widget = new Widget();
        var entry = EntryFor(widget, nameof(Widget.Caption));

        Assert.Same(widget, ((ITypeDescriptorContext)entry).Instance);
    }

    [Fact]
    public void Container_IsTheContainerOfTheEditedComponent()
    {
        var (widget, site, _) = SitedWidget();
        var entry = EntryFor(widget, nameof(Widget.Caption));

        Assert.Same(site.Container, ((ITypeDescriptorContext)entry).Container);
    }

    [Fact]
    public void Container_IsNullWhenNothingIsSited()
    {
        var entry = EntryFor(new Widget(), nameof(Widget.Caption));

        Assert.Null(((ITypeDescriptorContext)entry).Container);
    }

    [Fact]
    public void PropertyDescriptor_DescribesThePropertyTheEntryWrites()
    {
        var entry = EntryFor(new Widget(), nameof(Widget.Caption));

        var descriptor = entry.PropertyDescriptor;

        Assert.NotNull(descriptor);
        Assert.Equal(nameof(Widget.Caption), descriptor!.Name);
        Assert.Equal(typeof(string), descriptor.PropertyType);
    }

    [Fact]
    public void GetService_AnswersForItselfAndDefersToTheSite()
    {
        var (widget, _, changes) = SitedWidget();
        var entry = EntryFor(widget, nameof(Widget.Caption));

        Assert.Same(entry, entry.GetService(typeof(ITypeDescriptorContext)));
        Assert.Same(changes, entry.GetService(typeof(IComponentChangeService)));
        Assert.Null(entry.GetService(typeof(IServiceProvider)));
    }

    [Fact]
    public void GetService_RejectsNull()
    {
        var entry = EntryFor(new Widget(), nameof(Widget.Caption));

        Assert.Throws<ArgumentNullException>(() => entry.GetService(null!));
    }

    [Fact]
    public void OnComponentChanged_ReportsTheEditedComponentAndProperty()
    {
        var (widget, _, changes) = SitedWidget();
        var entry = EntryFor(widget, nameof(Widget.Caption));

        entry.OnComponentChanged();

        var reported = Assert.Single(changes.Changed);
        Assert.Same(widget, reported.Component);
        Assert.Equal(nameof(Widget.Caption), reported.Member?.Name);
    }

    [Fact]
    public void OnComponentChanging_IsTrueWhenTheDesignerAgrees()
    {
        var (widget, _, changes) = SitedWidget();
        var entry = EntryFor(widget, nameof(Widget.Caption));

        Assert.True(entry.OnComponentChanging());
        Assert.Single(changes.Changing);
    }

    [Fact]
    public void OnComponentChanging_IsFalseWhenTheDesignerRefuses()
    {
        var (widget, _, changes) = SitedWidget();
        changes.RefuseChanges = true;
        var entry = EntryFor(widget, nameof(Widget.Caption));

        Assert.False(entry.OnComponentChanging());
    }

    [Fact]
    public void OnComponentChanging_IsTrueWithNoDesignSurface()
    {
        var entry = EntryFor(new Widget(), nameof(Widget.Caption));

        Assert.True(entry.OnComponentChanging(), "at run time there is nobody to refuse the change");
    }

    [Fact]
    public void SetValue_WritesThePropertyAndNotifiesTheDesigner()
    {
        var (widget, _, changes) = SitedWidget();
        var entry = EntryFor(widget, nameof(Widget.Caption));

        entry.SetValue("after");

        Assert.Equal("after", widget.Caption);
        Assert.Single(changes.Changing);
        Assert.Single(changes.Changed);
    }

    [Fact]
    public void SetValue_LeavesThePropertyAloneWhenTheChangeIsRefused()
    {
        var (widget, _, changes) = SitedWidget();
        changes.RefuseChanges = true;
        var entry = EntryFor(widget, nameof(Widget.Caption));

        entry.SetValue("after");

        Assert.Equal("before", widget.Caption);
        Assert.Empty(changes.Changed);
    }

    [Fact]
    public void Value_ReadsThroughTheProperty()
    {
        var widget = new Widget { Count = 7 };
        var entry = EntryFor(widget, nameof(Widget.Count));

        Assert.Equal(7, entry.Value);
    }

    /// <summary>
    /// Runs <paramref name="body" /> against a live <see cref="PropertyGrid" />. The grid is a GTK widget
    /// that only survives inside a realized form, so - as the other property-grid tests do - it is hosted
    /// on a form and exercised from its Load handler; a failure inside the handler is re-thrown on the
    /// test's own thread so xUnit reports it.
    /// </summary>
    private static void OnAGrid(Action<PropertyGrid> body)
    {
        var form = new Form();
        var grid = new PropertyGrid();
        form.Controls.Add(grid);
        ExceptionDispatchInfo? failure = null;

        form.Load += (_, _) =>
        {
            try
            {
                body(grid);
            }
            catch (Exception ex)
            {
                failure = ExceptionDispatchInfo.Capture(ex);
            }
            finally
            {
                form.Close();
            }
        };

        form.ShowDialog();
        failure?.Throw();
    }

    [Fact]
    public void ComPropertyBrowser_MembersDoNotThrow()
    {
        OnAGrid(grid =>
        {
            var browser = (IComPropertyBrowser)grid;

            // Nothing is being edited, so committing has nothing to do and must report success.
            Assert.True(browser.EnsurePendingChangesCommitted());
            browser.DropDownDone();
            browser.HandleF4();
            browser.SaveState(null);
            browser.LoadState(null);
        });
    }

    [Fact]
    public void LoadState_RestoresTheDocumentedDefaults()
    {
        OnAGrid(grid =>
        {
            grid.PropertySort = PropertySort.NoSort;
            grid.HelpVisible = false;
            grid.CommandsVisibleIfAvailable = true;

            ((IComPropertyBrowser)grid).LoadState(null);

            Assert.Equal(PropertySort.CategorizedAlphabetical, grid.PropertySort);
            Assert.True(grid.HelpVisible);
            Assert.False(grid.CommandsVisibleIfAvailable);
        });
    }

    [Fact]
    public void InPropertySet_IsFalseWhileNothingIsBeingSet()
    {
        OnAGrid(grid =>
            Assert.False(grid.InPropertySet, "the grid is not setting a property just by existing"));
    }

    [Fact]
    public void SelectedObject_PopulatesTheGridAndRaisesSelectedObjectsChanged()
    {
        OnAGrid(grid =>
        {
            var raised = 0;
            grid.SelectedObjectsChanged += (_, _) => raised++;

            grid.SelectedObject = new Widget();

            Assert.Equal(1, raised);
            Assert.True(RowCount(grid) > 0, "assigning SelectedObject must load the object's properties");
        });
    }

    [Fact]
    public void SelectedObject_RoundTripsThroughSelectedObjects()
    {
        OnAGrid(grid =>
        {
            var widget = new Widget();

            grid.SelectedObject = widget;

            Assert.Same(widget, grid.SelectedObject);
            Assert.Same(widget, Assert.Single(grid.SelectedObjects));
        });
    }

    [Fact]
    public void SelectedObject_Null_ClearsTheGrid()
    {
        OnAGrid(grid =>
        {
            grid.SelectedObject = new Widget();

            grid.SelectedObject = null!;

            Assert.Empty(grid.SelectedObjects);
            Assert.Equal(0, RowCount(grid));
        });
    }

    /// <summary>
    /// How many rows the view is actually showing. The view is private, so this reaches for it directly -
    /// the point of the test is that the rows exist at all, which no public member reports.
    /// </summary>
    private static int RowCount(PropertyGrid grid)
    {
        var view = typeof(PropertyGrid)
            .GetField("_propertyView", BindingFlags.Instance | BindingFlags.NonPublic)!
            .GetValue(grid)!;
        var store = (global::Gtk.TreeStore)view.GetType()
            .GetField("store", BindingFlags.Instance | BindingFlags.Public)!
            .GetValue(view)!;

        return store.IterNChildren();
    }
}

#pragma warning restore CS0612
