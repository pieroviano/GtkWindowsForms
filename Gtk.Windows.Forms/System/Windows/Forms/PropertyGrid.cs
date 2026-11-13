/*
 * Based on GTK components, it is a cross-platform interface component that is compatible with the native C# control winform interface.
 * Use this component Gtk.Windows.Forms instead of Microsoft.WindowsDesktop.App.WindowsForms, compile once, and run across Windows, Linux, and macOS
 * Technical support 438865652@qq.com，https://www.gtkapp.com, https://gitee.com/easywebfactory, https://github.com/easywebfactory438865652@qq.com，https://www.gtkapp.com, https://gitee.com/easywebfactory, https://github.com/easywebfactory
 * author:chenhongjin
 */

using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Windows.Forms.ComponentModel.Com2Interop;
using System.Windows.Forms.Design;
using System.Windows.Forms.PropertyGridInternal;
using Gtk.Windows.Forms.Controls.ControlBase;
using Microsoft.Win32;

namespace System.Windows.Forms;

/// <summary>
/// Class PropertyGrid. Implements the
/// <see cref="T:System.Windows.Forms.ContainerControl" /> Implements the
/// <see cref="T:System.Windows.Forms.ComponentModel.Com2Interop.IComPropertyBrowser" />
/// </summary>
/// <seealso cref="T:System.Windows.Forms.ContainerControl" />
/// <seealso cref="T:System.Windows.Forms.ComponentModel.Com2Interop.IComPropertyBrowser" />
[Designer($"System.Windows.Forms.Design.PropertyGridDesigner, {AssemblyRef.SystemDesign}")]
public partial class PropertyGrid : ContainerControl, IComPropertyBrowser
{
    /// <summary>
    /// The self
    /// </summary>
    public readonly PropertyGridBase self = new();

    private readonly PropertyGridView _propertyView;

    private object[]? _selectedObjects;

    private readonly List<TabInfo> _tabs = new();
    private Delegate? _propertyValueChanged;
    private Delegate? _selectedGridItemChanged;

    private static readonly object s_comComponentNameChangedEvent = new();

    private static readonly object s_propertySortChangedEvent = new();

    private static readonly object s_propertyTabChangedEvent = new();

    private static readonly object s_propertyValueChangedEvent = new();

    private static readonly object s_selectedGridItemChangedEvent = new();

    private static readonly object s_selectedObjectsChangedEvent = new();

    /// <summary>
    /// Occurs when the
    /// <see cref="T:System.Windows.Forms.PropertyGrid" /> control is browsing a COM object and the user renames the object.
    /// </summary>
    public virtual event ComponentRenameEventHandler? ComComponentNameChanged;

    /// <summary>
    /// Occurs when a grid item is activated.
    /// </summary>
    public virtual event EventHandler? GridItemActivated;

    /// <summary>
    /// Occurs when mouse enter.
    /// </summary>
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public new event EventHandler MouseEnter
    {
        add => base.MouseEnter += value;
        remove => base.MouseEnter -= value;
    }

    /// <summary>
    /// Occurs when mouse leave.
    /// </summary>
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public new event EventHandler MouseLeave
    {
        add => base.MouseLeave += value;
        remove => base.MouseLeave -= value;
    }

    /// <summary>
    /// Occurs when property sort changed.
    /// </summary>
    public virtual event EventHandler PropertySortChanged
    {
        add => Events.AddHandler(s_propertySortChangedEvent, value);
        remove => Events.RemoveHandler(s_propertySortChangedEvent, value);
    }

    /// <summary>
    /// Occurs when property tab changed.
    /// </summary>
    public virtual event PropertyTabChangedEventHandler PropertyTabChanged
    {
        add => Events.AddHandler(s_propertyTabChangedEvent, value);
        remove => Events.RemoveHandler(s_propertyTabChangedEvent, value);
    }

    /// <summary>
    /// Occurs when property value changed.
    /// </summary>
    public virtual event PropertyValueChangedEventHandler PropertyValueChanged
    {
        add
        {
            Events.AddHandler(s_propertyValueChangedEvent, value);
            _propertyValueChanged = Events[s_propertyValueChangedEvent];
        }
        remove
        {
            Events.RemoveHandler(s_propertyValueChangedEvent, value);
            _propertyValueChanged = Events[s_propertyValueChangedEvent];
        }
    }

    /// <summary>
    /// Occurs when selected grid item changed.
    /// </summary>
    public virtual event SelectedGridItemChangedEventHandler SelectedGridItemChanged
    {
        add
        {
            Events.AddHandler(s_selectedGridItemChangedEvent, value);
            _selectedGridItemChanged = Events[s_selectedGridItemChangedEvent];
        }
        remove
        {
            Events.RemoveHandler(s_selectedGridItemChangedEvent, value);
            _selectedGridItemChanged = Events[s_selectedGridItemChangedEvent];
        }
    }

    /// <summary>
    /// Occurs when selected objects changed.
    /// </summary>
    public virtual event EventHandler SelectedObjectsChanged
    {
        add => Events.AddHandler(s_selectedObjectsChangedEvent, value);
        remove => Events.RemoveHandler(s_selectedObjectsChangedEvent, value);
    }

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.PropertyGrid" /> class.
    /// </summary>
    public PropertyGrid()
    {
        self.Override.sender = this;
        _propertyView = new PropertyGridView(this);
        self.child1.AddChild(_propertyView.tree);
        _propertyView.PropertyValueChanged += Self_PropertyValueChanged;
        _propertyView.SelectedGridItemChanged += Self_SelectedGridItemChanged;
        _propertyView.tree.RowActivated += _propertyView_RowActivated;
    }

    /// <summary>
    /// Gets or sets a value indicating whether the control is automatically scrolled when its contents extend beyond its client area.
    /// </summary>
    /// <value>
    /// <c>
    /// true
    /// </c> if automatic scroll; otherwise,
    /// <c>
    /// false
    /// </c>.
    /// </value>
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public override bool AutoScroll
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the color of the back.
    /// </summary>
    /// <value>
    /// The color of the back.
    /// </value>
    public override Color BackColor
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the background image.
    /// </summary>
    /// <value>
    /// The background image.
    /// </value>
#nullable disable
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]

    public override Image BackgroundImage
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the background image layout.
    /// </summary>
    /// <value>
    /// The background image layout.
    /// </value>
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public override ImageLayout BackgroundImageLayout
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the browsable attributes.
    /// </summary>
    /// <value>
    /// The browsable attributes.
    /// </value>
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public AttributeCollection BrowsableAttributes
    {
        get;
        set;
    }

    /// <summary>
    /// Gets a value indicating whether this instance can show commands.
    /// </summary>
    /// <value>
    /// <c>
    /// true
    /// </c> if this instance can show commands; otherwise,
    /// <c>
    /// false
    /// </c>.
    /// </value>
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public virtual bool CanShowCommands { get; }

    /// <summary>
    /// Gets or sets a value indicating whether this instance can show visual style glyphs.
    /// </summary>
    /// <value>
    /// <c>
    /// true
    /// </c> if this instance can show visual style glyphs; otherwise,
    /// <c>
    /// false
    /// </c>.
    /// </value>
    [DefaultValue(true)]
    public bool CanShowVisualStyleGlyphs
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the color of the category fore.
    /// </summary>
    /// <value>
    /// The color of the category fore.
    /// </value>
    [DefaultValue(typeof(Color), "ControlText")]
    public Color CategoryForeColor
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the color of the category splitter.
    /// </summary>
    /// <value>
    /// The color of the category splitter.
    /// </value>
    [DefaultValue(typeof(Color), "Control")]
    public Color CategorySplitterColor
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the color of the commands active link.
    /// </summary>
    /// <value>
    /// The color of the commands active link.
    /// </value>
    public Color CommandsActiveLinkColor
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the color of the commands back.
    /// </summary>
    /// <value>
    /// The color of the commands back.
    /// </value>
    public Color CommandsBackColor
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the color of the commands border.
    /// </summary>
    /// <value>
    /// The color of the commands border.
    /// </value>
    [DefaultValue(typeof(Color), "ControlDark")]
    public Color CommandsBorderColor
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the color of the commands disabled link.
    /// </summary>
    /// <value>
    /// The color of the commands disabled link.
    /// </value>
    public Color CommandsDisabledLinkColor
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the color of the commands fore.
    /// </summary>
    /// <value>
    /// The color of the commands fore.
    /// </value>
    public Color CommandsForeColor
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the color of the commands link.
    /// </summary>
    /// <value>
    /// The color of the commands link.
    /// </value>
    public Color CommandsLinkColor
    {
        get;
        set;
    }

    /// <summary>
    /// Gets a value indicating whether commands visible.
    /// </summary>
    /// <value>
    /// <c>
    /// true
    /// </c> if commands visible; otherwise,
    /// <c>
    /// false
    /// </c>.
    /// </value>
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public virtual bool CommandsVisible { get; }

    /// <summary>
    /// Gets or sets a value indicating whether commands visible if available.
    /// </summary>
    /// <value>
    /// <c>
    /// true
    /// </c> if commands visible if available; otherwise,
    /// <c>
    /// false
    /// </c>.
    /// </value>
    [DefaultValue(true)]
    public virtual bool CommandsVisibleIfAvailable
    {
        get;
        set;
    }

    /// <summary>
    /// Gets the context menu default location.
    /// </summary>
    /// <value>
    /// The context menu default location.
    /// </value>
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public Point ContextMenuDefaultLocation { get; }

    /// <summary>
    /// Gets the controls.
    /// </summary>
    /// <value>
    /// The controls.
    /// </value>
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public override ControlCollection Controls { get; }

    /// <summary>
    /// Gets or sets the color of the disabled item fore.
    /// </summary>
    /// <value>
    /// The color of the disabled item fore.
    /// </value>
    [DefaultValue(typeof(Color), "GrayText")]
    public Color DisabledItemForeColor
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the color of the fore.
    /// </summary>
    /// <value>
    /// The color of the fore.
    /// </value>
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public override Color ForeColor
    {
        get;
        set;
    }

    /// <summary>
    /// Gets the GTK control.
    /// </summary>
    /// <value>
    /// The GTK control.
    /// </value>
    public override object GtkControl => self;

    /// <summary>
    /// Gets or sets the color of the help back.
    /// </summary>
    /// <value>
    /// The color of the help back.
    /// </value>
    [DefaultValue(typeof(Color), "Control")]
    public Color HelpBackColor
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the color of the help border.
    /// </summary>
    /// <value>
    /// The color of the help border.
    /// </value>
    [DefaultValue(typeof(Color), "ControlDark")]
    public Color HelpBorderColor
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the color of the help fore.
    /// </summary>
    /// <value>
    /// The color of the help fore.
    /// </value>
    [DefaultValue(typeof(Color), "ControlText")]
    public Color HelpForeColor
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets a value indicating whether help visible.
    /// </summary>
    /// <value>
    /// <c>
    /// true
    /// </c> if help visible; otherwise,
    /// <c>
    /// false
    /// </c>.
    /// </value>
    [DefaultValue(true)]
    [Localizable(true)]
    public virtual bool HelpVisible
    {
        get;
        set;
    }

    /// <summary>
    /// Gets a value indicating whether the
    /// <see cref="T:System.Windows.Forms.PropertyGrid" /> control is currently setting one of the properties of its selected object.
    /// </summary>
    /// <value>
    /// <c>
    /// true
    /// </c> if in property set; otherwise,
    /// <c>
    /// false
    /// </c>.
    /// </value>
    public bool InPropertySet => _propertyView.InPropertySet;

    /// <summary>
    /// Gets or sets a value indicating whether large buttons.
    /// </summary>
    /// <value>
    /// <c>
    /// true
    /// </c> if large buttons; otherwise,
    /// <c>
    /// false
    /// </c>.
    /// </value>
    [DefaultValue(false)]
    public bool LargeButtons
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the color of the line.
    /// </summary>
    /// <value>
    /// The color of the line.
    /// </value>
    //[Browsable(false)]
    //[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    //[EditorBrowsable(EditorBrowsableState.Never)]
    //public Padding Padding { get; set; }
    [DefaultValue(typeof(Color), "InactiveBorder")]
    public Color LineColor
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the property sort.
    /// </summary>
    /// <value>
    /// The property sort.
    /// </value>
    [DefaultValue(PropertySort.CategorizedAlphabetical)]
    public PropertySort PropertySort
    {
        get;
        set;
    }

    /// <summary>
    /// Gets the property tabs.
    /// </summary>
    /// <value>
    /// The property tabs.
    /// </value>
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public PropertyTabCollection PropertyTabs { get; }

    /// <summary>
    /// Gets or sets the selected grid item.
    /// </summary>
    /// <value>
    /// The selected grid item.
    /// </value>
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public GridItem SelectedGridItem
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the color of the selected item with focus back.
    /// </summary>
    /// <value>
    /// The color of the selected item with focus back.
    /// </value>
    [DefaultValue(typeof(Color), "Highlight")]
    public Color SelectedItemWithFocusBackColor
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the color of the selected item with focus fore.
    /// </summary>
    /// <value>
    /// The color of the selected item with focus fore.
    /// </value>
    [DefaultValue(typeof(Color), "HighlightText")]
    public Color SelectedItemWithFocusForeColor
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the selected object.
    /// </summary>
    /// <value>
    /// The selected object.
    /// </value>
    [DefaultValue(null)]
    public object SelectedObject
    {
        get => _selectedObjects is null || _selectedObjects.Length == 0 ? null : _selectedObjects[0];
        // Routed through SelectedObjects rather than writing the field: assigning SelectedObject has to
        // populate the grid and raise SelectedObjectsChanged, exactly as assigning a one-element
        // SelectedObjects does.
        set => SelectedObjects = value is null ? Array.Empty<object>() : new[] { value };
    }

    /// <summary>
    /// Gets or sets the selected objects.
    /// </summary>
    /// <value>
    /// The selected objects.
    /// </value>
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public object[] SelectedObjects
    {
        get => _selectedObjects is null ? Array.Empty<object>() : (object[])_selectedObjects.Clone();
        set
        {
            _selectedObjects = value is null ? Array.Empty<object>() : (object[])value.Clone();
            // only one object is supported
            if (_selectedObjects.Length == 0)
                _propertyView.LoadPropertyInfo(null);
            else
                _propertyView.LoadPropertyInfo(_selectedObjects[0]);

            OnSelectedObjectsChanged(EventArgs.Empty);
        }
    }

    /// <summary>
    /// Gets the selected tab.
    /// </summary>
    /// <value>
    /// The selected tab.
    /// </value>
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public PropertyTab SelectedTab { get; }

    /// <summary>
    /// Gets or sets the
    /// <see cref="T:System.ComponentModel.ISite">
    /// </see> of the
    /// <see cref="T:System.ComponentModel.Component">
    /// </see>.
    /// </summary>
    /// <value>
    /// The site.
    /// </value>
    public override ISite Site
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the size.
    /// </summary>
    /// <value>
    /// The size.
    /// </value>
    [Browsable(true)]
    [Description(" control size（ measured in pixels）。")]
    public override Size Size
    {
        get => base.Size;
        set { base.Size = value; self.Position = value.Height - 60; }
    }

    /// <summary>
    /// Gets or sets the text.
    /// </summary>
    /// <value>
    /// The text.
    /// </value>
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public override string Text
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets a value indicating whether toolbar visible.
    /// </summary>
    /// <value>
    /// <c>
    /// true
    /// </c> if toolbar visible; otherwise,
    /// <c>
    /// false
    /// </c>.
    /// </value>
    [DefaultValue(true)]
    public virtual bool ToolbarVisible
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets a value indicating whether use compatible text rendering.
    /// </summary>
    /// <value>
    /// <c>
    /// true
    /// </c> if use compatible text rendering; otherwise,
    /// <c>
    /// false
    /// </c>.
    /// </value>
    [DefaultValue(false)]
    public bool UseCompatibleTextRendering
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the color of the view back.
    /// </summary>
    /// <value>
    /// The color of the view back.
    /// </value>
    [DefaultValue(typeof(Color), "Window")]
    public Color ViewBackColor
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the color of the view border.
    /// </summary>
    /// <value>
    /// The color of the view border.
    /// </value>
    [DefaultValue(typeof(Color), "ControlDark")]
    public Color ViewBorderColor
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the color of the view fore.
    /// </summary>
    /// <value>
    /// The color of the view fore.
    /// </value>
    [DefaultValue(typeof(Color), "WindowText")]
    public Color ViewForeColor
    {
        get;
        set;
    }

    /// <summary>
    /// Gets a value indicating whether show focus cues.
    /// </summary>
    /// <value>
    /// <c>
    /// true
    /// </c> if show focus cues; otherwise,
    /// <c>
    /// false
    /// </c>.
    /// </value>
    protected internal override bool ShowFocusCues { get; }

    /// <summary>
    /// Gets the default size for the control.
    /// </summary>
    /// <value>
    /// The default size.
    /// </value>
    protected override Size DefaultSize { get; }

    /// <summary>
    /// Gets the default type of the tab.
    /// </summary>
    /// <value>
    /// The default type of the tab.
    /// </value>
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual Type DefaultTabType { get; }

    /// <summary>
    /// Gets or sets a value indicating whether draw flat toolbar.
    /// </summary>
    /// <value>
    /// <c>
    /// true
    /// </c> if draw flat toolbar; otherwise,
    /// <c>
    /// false
    /// </c>.
    /// </value>
    protected bool DrawFlatToolbar
    {
        get;
        set;
    }

    /// <summary>
    /// Gets the show property page image.
    /// </summary>
    /// <value>
    /// The show property page image.
    /// </value>
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual Bitmap ShowPropertyPageImage { get; }

    /// <summary>
    /// Gets the sort by category image.
    /// </summary>
    /// <value>
    /// The sort by category image.
    /// </value>
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual Bitmap SortByCategoryImage { get; }

    /// <summary>
    /// Gets the sort by property image.
    /// </summary>
    /// <value>
    /// The sort by property image.
    /// </value>
    //protected ToolStripRenderer ToolStripRenderer { get; set; }
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual Bitmap SortByPropertyImage { get; }

    /// <summary>
    /// Collapses all grid items.
    /// </summary>
    public void CollapseAllGridItems()
    {
    }

    /// <summary>
    /// Causes the control to be redrawn.
    /// </summary>
    public override void Refresh()
    {
        _propertyView.tree.Selection.UnselectAll();
        _propertyView.tree.QueueDraw();
    }

    /// <summary>
    /// Refreshes the tabs.
    /// </summary>
    /// <param name="tabScope">
    /// The tab scope.
    /// </param>
    public void RefreshTabs(PropertyTabScope tabScope)
    {
    }

    /// <summary>
    /// Resets the selected property.
    /// </summary>
    public void ResetSelectedProperty()
    {
    }

    void IComPropertyBrowser.DropDownDone() => _propertyView.DropDownDone();

    bool IComPropertyBrowser.EnsurePendingChangesCommitted() => _propertyView.EnsurePendingChangesCommitted();

    void IComPropertyBrowser.HandleF4()
    {
        Select();
        _propertyView.FocusGrid();
    }

    void IComPropertyBrowser.LoadState(RegistryKey key) => LoadState(key);

    /// <summary>
    /// Applies the persisted view state.
    /// </summary>
    /// <param name="key">
    /// The key the state was saved under. There is no registry outside Windows, so the shim's
    /// <see cref="T:Microsoft.Win32.RegistryKey" /> carries no values and the WinForms
    /// <see langword="null" />-key path - restore the defaults - is taken whatever is passed. Documented
    /// deviation.
    /// </param>
    private void LoadState(RegistryKey key)
    {
        PropertySort = PropertySort.CategorizedAlphabetical;
        HelpVisible = true;
        CommandsVisibleIfAvailable = false;
    }

    /// <summary>
    /// Persists the view state.
    /// </summary>
    /// <param name="key">
    /// The key to save under. No registry exists outside Windows and the shim's
    /// <see cref="T:Microsoft.Win32.RegistryKey" /> holds no values, so nothing is written. Documented
    /// deviation; the state is re-derived from the defaults on the next
    /// <see cref="M:System.Windows.Forms.ComponentModel.Com2Interop.IComPropertyBrowser.LoadState(Microsoft.Win32.RegistryKey)" />.
    /// </param>
    private void SaveState(RegistryKey key)
    {
    }

    void IComPropertyBrowser.SaveState(RegistryKey key) => SaveState(key);

    /// <summary>
    /// Raises the ComComponentNameChanged event.
    /// </summary>
    protected virtual void OnComComponentNameChanged(ComponentRenameEventArgs e)
    {
        ComComponentNameChanged?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the GridItemActivated event.
    /// </summary>
    protected virtual void OnGridItemActivated(System.EventArgs e)
    {
        GridItemActivated?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the SelectedObjectsChanged event.
    /// </summary>
    /// <param name="e">The event data.</param>
    protected virtual void OnSelectedObjectsChanged(System.EventArgs e)
    {
        ((EventHandler)Events[s_selectedObjectsChangedEvent])?.Invoke(this, e);
    }

    private void _propertyView_RowActivated(object o, Gtk.RowActivatedArgs args)
    {
        if (_propertyView.store.GetIter(out var iter, args.Path))
        {
            if (_propertyView.store.GetValue(iter, 0) is GridEntry ov)
            {
                self.ShowDescription(ov.Label, ov.Description);
            }
        }
        if (args.Column.XOffset > 50)
        {
            OnGridItemActivated(args);
        }
    }

    private void Self_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
    {
        _propertyValueChanged?.DynamicInvoke(this, e);
    }

    private void Self_SelectedGridItemChanged(object sender, SelectedGridItemChangedEventArgs e)
    {
        _selectedGridItemChanged?.DynamicInvoke(this, e);
    }
}