/*
 * Based on GTK components, it is a cross-platform interface component that is compatible with the native C# control winform interface.
 * Use this component GtkSystem.Windows.Forms instead of Microsoft.WindowsDesktop.App.WindowsForms, compile once, and run across Windows, Linux, and macOS
 * Technical support 438865652@qq.com，https://www.gtkapp.com, https://gitee.com/easywebfactory, https://github.com/easywebfactory438865652@qq.com，https://www.gtkapp.com, https://gitee.com/easywebfactory, https://github.com/easywebfactory
 * author:chenhongjin
 */

using GtkSystem.Windows.Forms.GtkControls.ControlBase;
using Microsoft.Win32;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Windows.Forms.ComponentModel.Com2Interop;
using System.Windows.Forms.Design;
using System.Windows.Forms.PropertyGridInternal;
namespace System.Windows.Forms
{
    /// <summary>
    /// Provides a user interface for browsing the properties of an object.
    /// </summary>
    /// <filterpriority>
    /// 1
    /// </filterpriority>
    [Designer($"System.Windows.Forms.Design.PropertyGridDesigner, {AssemblyRef.SystemDesign}")]
    public partial class PropertyGrid : ContainerControl, IComPropertyBrowser
    {

        public readonly PropertyGridBase self = new PropertyGridBase();

        public override object GtkControl => self;

        private static readonly object s_propertyValueChangedEvent = new();

        private static readonly object s_comComponentNameChangedEvent = new();

        private static readonly object s_propertyTabChangedEvent = new();

        private static readonly object s_selectedGridItemChangedEvent = new();

        private static readonly object s_propertySortChangedEvent = new();

        private static readonly object s_selectedObjectsChangedEvent = new();

        private object[] _selectedObjects;
        private readonly List<TabInfo> _tabs = new List<TabInfo>();


        private PropertyGridView _propertyView;
        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="System.Windows.Forms.PropertyGrid" /> class.
        /// </summary>
        public PropertyGrid()
        {
            _propertyView = new PropertyGridView(this);
            self.child1.Add(_propertyView.tree);
            _propertyView.PropertyValueChanged += Self_PropertyValueChanged;
            _propertyView.SelectedGridItemChanged += Self_SelectedGridItemChanged;
        }

        private void Self_SelectedGridItemChanged(object sender, SelectedGridItemChangedEventArgs e)
        {
            Events[s_selectedGridItemChangedEvent]?.DynamicInvoke(this, e);
        }

        private void Self_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            Events[s_propertyValueChangedEvent]?.DynamicInvoke(this, e);
        }
        /// <summary>
        /// Gets or sets the currently selected objects.
        /// </summary>
        /// <returns>An array of type
        /// <see cref="System.Object" /> The default is an empty array.
        /// </returns>
        /// <exception cref="System.ArgumentException">One of the items in the array of objects had a null value.
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public object[] SelectedObjects
        {
            get => _selectedObjects is null ? Array.Empty<object>() : (object[])_selectedObjects.Clone();
            set
            {
                _selectedObjects = value is null ? Array.Empty<object>() : (object[])value.Clone();
                // only one object is supported
                _propertyView.LoadPropertyInfo(_selectedObjects[0]);
            }
        }
        /// <summary>
        /// Gets or sets the object for which the grid displays properties.
        /// </summary>
        /// <returns>The first object in the object list. If there is no currently selected object the return is null.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        [DefaultValue(null)]
        public object SelectedObject
        {
            get => _selectedObjects is null || _selectedObjects.Length == 0 ? null : _selectedObjects[0];
            set => SelectedObjects = value is null ? Array.Empty<object>() : (new object[] { value });
        }

        [Browsable(true)]
        [Description(" control size（ measured in pixels）。")]

        public override Size Size { get => base.Size; set { base.Size = value; self.Position = value.Height - 60; } }
        /// <summary>
        /// Gets or sets a value indicating whether the Help text is visible.
        /// </summary>
        /// <returns>true if the help text is visible; otherwise, false. The default is true.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        [DefaultValue(true)]
        [Localizable(true)]
        public virtual bool HelpVisible { get; set; }
        [DefaultValue(typeof(Color), "ControlDark")]

        public Color HelpBorderColor { get; set; }
        /// <summary>
        /// Gets or sets the foreground color for the Help region.
        /// </summary>
        /// <returns>One of the
        /// <see cref="System.Drawing.Color" /> values. The default is the default system color for control text.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        [DefaultValue(typeof(Color), "ControlText")]
        public Color HelpForeColor { get; set; }
        /// <summary>
        /// Gets or sets the background color for the Help region.
        /// </summary>
        /// <returns>One of the
        /// <see cref="System.Drawing.Color" /> values. The default is the default system color for controls.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        [DefaultValue(typeof(Color), "Control")]
        public Color HelpBackColor { get; set; }
        /// <summary>
        /// This property is not relevant for this class.
        /// </summary>
        /// <returns>A
        /// <see cref="System.Drawing.Color" />
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override Color ForeColor { get; set; }
        /// <summary>
        /// This property is not relevant for this class.
        /// </summary>
        /// <returns>true if enabled; otherwise, false.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override bool AutoScroll { get; set; }
        /// <returns>
        /// A
        /// <see cref="System.Drawing.Color" /> that represents the background color of the control. The default is the value of the
        /// <see cref="System.Windows.Forms.Control.DefaultBackColor" /> property.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public override Color BackColor { get; set; }
        /// <summary>
        /// This property is not relevant for this class.
        /// </summary>
        /// <returns>An
        /// <see cref="System.Drawing.Image" />
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
#nullable disable
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]

        public override Image BackgroundImage { get; set; }
        /// <summary>
        /// This property is not relevant for this class.
        /// </summary>
        /// <returns>An
        /// <see cref="System.Windows.Forms.ImageLayout" />
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override ImageLayout BackgroundImageLayout { get; set; }
        [DefaultValue(typeof(Color), "Highlight")]

        public Color SelectedItemWithFocusBackColor { get; set; }
        /// <summary>
        /// Gets or sets the browsable attributes associated with the object that the property grid is attached to.
        /// </summary>
        /// <returns>The collection of browsable attributes associated with the object.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public AttributeCollection BrowsableAttributes { get; set; }
        /// <summary>
        /// Gets or sets the text color used for category headings.
        /// </summary>
        /// <returns>A
        /// <see cref="System.Drawing.Color" /> structure representing the text color.
        /// </returns>
        [DefaultValue(typeof(Color), "ControlText")]
        public Color CategoryForeColor { get; set; }
        /// <summary>
        /// Gets or sets the background color of the hot commands region.
        /// </summary>
        /// <returns>One of the
        /// <see cref="System.Drawing.Color" /> values. The default is the default system color for controls.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public Color CommandsBackColor { get; set; }
        /// <summary>
        /// Gets or sets the foreground color for the hot commands region.
        /// </summary>
        /// <returns>One of the
        /// <see cref="System.Drawing.Color" /> values. The default is the default system color for control text.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public Color CommandsForeColor { get; set; }
        /// <summary>
        /// Gets or sets the link color for the executable commands region.
        /// </summary>
        /// <returns>A
        /// <see cref="System.Drawing.Color" /> structure representing the link color for the executable commands region.
        /// </returns>
        public Color CommandsLinkColor { get; set; }
        /// <summary>
        /// Gets or sets the color of active links in the executable commands region.
        /// </summary>
        /// <returns>A
        /// <see cref="System.Drawing.Color" /> structure representing the active link color.
        /// </returns>
        public Color CommandsActiveLinkColor { get; set; }
        /// <summary>
        /// Gets or sets the unavailable link color for the executable commands region.
        /// </summary>
        /// <returns>A
        /// <see cref="System.Drawing.Color" /> structure representing the unavailable link color.
        /// </returns>
        public Color CommandsDisabledLinkColor { get; set; }
        [DefaultValue(typeof(Color), "ControlDark")]

        public Color CommandsBorderColor { get; set; }
        /// <summary>
        /// Gets a value indicating whether the commands pane is visible.
        /// </summary>
        /// <returns>true if the commands pane is visible; otherwise, false. The default is false.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public virtual bool CommandsVisible { get; }
        /// <summary>
        /// Gets or sets a value indicating whether the commands pane is visible for objects that expose verbs.
        /// </summary>
        /// <returns>true if the commands pane is visible; otherwise, false. The default is true.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        [DefaultValue(true)]
        public virtual bool CommandsVisibleIfAvailable { get; set; }
        /// <summary>
        /// Gets a value indicating whether the commands pane can be made visible for the currently selected objects.
        /// </summary>
        /// <returns>true if the commands pane can be made visible; otherwise, false.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public virtual bool CanShowCommands { get; }
        /// <summary>
        /// Gets the default location for the shortcut menu.
        /// </summary>
        /// <returns>The default location for the shortcut menu if the command is invoked. Typically, this is centered over the selected property.
        /// </returns>
        /// <filterpriority>2
        /// </filterpriority>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public Point ContextMenuDefaultLocation { get; }
        [DefaultValue(typeof(Color), "HighlightText")]

        public Color SelectedItemWithFocusForeColor { get; set; }
        [DefaultValue(typeof(Color), "Control")]

        public Color CategorySplitterColor { get; set; }
        /// <summary>
        /// Gets or sets a value that determines whether to use the
        /// <see cref="System.Drawing.Graphics" /> class (GDI+) or the
        /// <see cref="System.Windows.Forms.TextRenderer" /> class (GDI) to render text.
        /// </summary>
        /// <returns>true if the
        /// <see cref="System.Drawing.Graphics" /> class should be used to perform text rendering for compatibility with versions 1.0 and 1.1. of the .NET Framework; otherwise, false. The default is false.
        /// </returns>
        [DefaultValue(false)]
        public bool UseCompatibleTextRendering { get; set; }
        [DefaultValue(typeof(Color), "ControlDark")]

        public Color ViewBorderColor { get; set; }
        /// <summary>
        /// Gets or sets a value indicating the color of the text in the grid.
        /// </summary>
        /// <returns>One of the
        /// <see cref="System.Drawing.Color" /> values. The default is current system color for text in windows.
        /// </returns>
        /// <filterpriority>2
        /// </filterpriority>
        [DefaultValue(typeof(Color), "WindowText")]
        public Color ViewForeColor { get; set; }
        /// <summary>
        /// Gets or sets a value indicating the background color in the grid.
        /// </summary>
        /// <returns>One of the
        /// <see cref="System.Drawing.Color" /> values. The default is the default system color for windows.
        /// </returns>
        /// <filterpriority>2
        /// </filterpriority>
        [DefaultValue(typeof(Color), "Window")]
        public Color ViewBackColor { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether the toolbar is visible.
        /// </summary>
        /// <returns>true if the toolbar is visible; otherwise, false. The default is true.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        [DefaultValue(true)]
        public virtual bool ToolbarVisible { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether buttons appear in standard size or in large size.
        /// </summary>
        /// <returns>true if buttons on the control appear large; otherwise, false. The default is false.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        [DefaultValue(false)]
        public bool LargeButtons { get; set; }
        [DefaultValue(typeof(Color), "GrayText")]

        public Color DisabledItemForeColor { get; set; }
        /// <returns>
        /// The
        /// <see cref="System.ComponentModel.ISite" /> associated with the
        /// <see cref="System.Windows.Forms.Control" />, if any.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public override ISite Site { get; set; }
        /// <summary>
        /// Gets or sets the selected grid item.
        /// </summary>
        /// <returns>The currently selected row in the property grid.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public GridItem SelectedGridItem { get; set; }
        /// <returns>
        /// The text associated with this control.
        /// </returns>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override string Text { get; set; }
        /// <summary>
        /// Gets the collection of property tabs that are displayed in the grid.
        /// </summary>
        /// <returns>A
        /// <see cref="System.Windows.Forms.PropertyGrid.PropertyTabCollection" /> containing the collection of
        /// <see cref="System.Windows.Forms.Design.PropertyTab" /> objects being displayed by the
        /// <see cref="System.Windows.Forms.PropertyGrid" />
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public PropertyTabCollection PropertyTabs { get; }
        /// <summary>
        /// Gets or sets the type of sorting the
        /// <see cref="System.Windows.Forms.PropertyGrid" /> uses to display properties.
        /// </summary>
        /// <returns>One of the
        /// <see cref="System.Windows.Forms.PropertySort" /> values. The default is
        /// <see cref="System.Windows.Forms.PropertySort.Categorized" /> or
        /// <see cref="System.Windows.Forms.PropertySort.Alphabetical" />
        /// </returns>
        /// <exception cref="System.ComponentModel.InvalidEnumArgumentException">The assigned value is not one of the
        /// <see cref="System.Windows.Forms.PropertySort" /> values.
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        [DefaultValue(PropertySort.CategorizedAlphabetical)]
        public PropertySort PropertySort { get; set; }
        /// <summary>
        /// Gets or sets the color of the gridlines and borders.
        /// </summary>
        /// <returns>One of the
        /// <see cref="System.Drawing.Color" /> values. The default is the default system color for scroll bars.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        //[Browsable(false)]
        //[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        //[EditorBrowsable(EditorBrowsableState.Never)]
        //public Padding Padding { get; set; }
        [DefaultValue(typeof(Color), "InactiveBorder")]
        public Color LineColor { get; set; }
        [DefaultValue(true)]

        public bool CanShowVisualStyleGlyphs { get; set; }
        /// <summary>
        /// Gets the currently selected property tab.
        /// </summary>
        /// <returns>The
        /// <see cref="System.Windows.Forms.Design.PropertyTab" /> that is providing the selected view.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public PropertyTab SelectedTab { get; }
        /// <summary>
        /// This property is not relevant for this class.
        /// </summary>
        /// <returns>A
        /// <see cref="System.Windows.Forms.Control.ControlCollection" />
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override ControlCollection Controls { get; }

        public bool InPropertySet => true;
        /// <summary>
        /// Gets or sets a value indicating whether the
        /// <see cref="System.Windows.Forms.PropertyGrid" /> control paints its toolbar with flat buttons.
        /// </summary>
        /// <returns>true if the
        /// <see cref="System.Windows.Forms.PropertyGrid" /> paints its toolbar with flat buttons; otherwise false. The default is false.
        /// </returns>
        protected bool DrawFlatToolbar { get; set; }
        //protected ToolStripRenderer ToolStripRenderer { get; set; }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [EditorBrowsable(EditorBrowsableState.Advanced)]

        protected virtual Bitmap SortByPropertyImage { get; }
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [EditorBrowsable(EditorBrowsableState.Advanced)]

        protected virtual Bitmap SortByCategoryImage { get; }
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [EditorBrowsable(EditorBrowsableState.Advanced)]

        protected virtual Bitmap ShowPropertyPageImage { get; }
        /// <summary>
        /// Gets the type of the default tab.
        /// </summary>
        /// <returns>A
        /// <see cref="System.Type" /> representing the default tab.
        /// </returns>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual Type DefaultTabType { get; }
        /// <returns>
        /// The default
        /// <see cref="System.Drawing.Size" /> of the control.
        /// </returns>
        protected override Size DefaultSize { get; }
        /// <returns>
        /// true if the control should display focus rectangles; otherwise, false.
        /// </returns>
        protected internal override bool ShowFocusCues { get; }

        public event ComponentRenameEventHandler ComComponentNameChanged;
        public event EventHandler SelectedObjectsChanged
        {
            add => Events.AddHandler(s_selectedObjectsChangedEvent, value);
            remove => Events.RemoveHandler(s_selectedObjectsChangedEvent, value);
        }

        public event SelectedGridItemChangedEventHandler SelectedGridItemChanged
        {
            add => Events.AddHandler(s_selectedGridItemChangedEvent, value);
            remove => Events.RemoveHandler(s_selectedGridItemChangedEvent, value);
        }
        public event EventHandler PropertySortChanged
        {
            add => Events.AddHandler(s_propertySortChangedEvent, value);
            remove => Events.RemoveHandler(s_propertySortChangedEvent, value);
        }
        public event PropertyValueChangedEventHandler PropertyValueChanged
        {
            add => Events.AddHandler(s_propertyValueChangedEvent, value);
            remove => Events.RemoveHandler(s_propertyValueChangedEvent, value);
        }

        public event PropertyTabChangedEventHandler PropertyTabChanged
        {
            add => Events.AddHandler(s_propertyTabChangedEvent, value);
            remove => Events.RemoveHandler(s_propertyTabChangedEvent, value);
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public new event EventHandler MouseLeave
        {
            add => base.MouseLeave += value;
            remove => base.MouseLeave -= value;
        }
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public new event EventHandler MouseEnter
        {
            add => base.MouseEnter += value;
            remove => base.MouseEnter -= value;
        }
        /// <summary>
        /// Collapses all the categories in the
        /// <see cref="System.Windows.Forms.PropertyGrid" />
        /// </summary>
        /// <filterpriority>2
        /// </filterpriority>
        public void CollapseAllGridItems()
        {
        }
        /// <filterpriority>
        /// 2
        /// </filterpriority>
        public override void Refresh()
        {
        }
        /// <summary>
        /// Refreshes the property tabs of the specified scope.
        /// </summary>
        /// <param name="tabScope">Either the Component or Document value of
        /// <see cref="System.ComponentModel.PropertyTabScope" />
        /// </param>
        /// <exception cref="System.ArgumentException">The
        /// <paramref name="tabScope" /> parameter is not the Component or Document value of
        /// <see cref="System.ComponentModel.PropertyTabScope" />
        /// </exception>
        /// <filterpriority>2
        /// </filterpriority>
        public void RefreshTabs(PropertyTabScope tabScope)
        {
        }
        /// <summary>
        /// Resets the selected property to its default value.
        /// </summary>
        /// <filterpriority>1
        /// </filterpriority>
        public void ResetSelectedProperty()
        {
        }
        void SaveState(RegistryKey key)
        {
            throw new NotImplementedException();
        }
        void IComPropertyBrowser.SaveState(RegistryKey key)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Closes any open drop-down controls on the
        /// <see cref="System.Windows.Forms.PropertyGrid" /> control. For a description of this member, see
        /// <see cref="System.Windows.Forms.ComponentModel.Com2Interop.IComPropertyBrowser.DropDownDone" />
        /// </summary>
        void IComPropertyBrowser.DropDownDone()
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Commits all pending changes to the
        /// <see cref="System.Windows.Forms.PropertyGrid" /> control. For a description of this member, see
        /// <see cref="System.Windows.Forms.ComponentModel.Com2Interop.IComPropertyBrowser.EnsurePendingChangesCommitted" />
        /// </summary>
        /// <returns>true if all the
        /// <see cref="System.Windows.Forms.PropertyGrid" /> successfully commits changes; otherwise, false.
        /// </returns>
        bool IComPropertyBrowser.EnsurePendingChangesCommitted()
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Activates the
        /// <see cref="System.Windows.Forms.PropertyGrid" /> control when the user chooses properties for a control in Design view. For a description of this member, see
        /// <see cref="System.Windows.Forms.ComponentModel.Com2Interop.IComPropertyBrowser.HandleF4" />
        /// </summary>
        void IComPropertyBrowser.HandleF4()
        {
            throw new NotImplementedException();
        }

        void IComPropertyBrowser.LoadState(RegistryKey key)
        {
            throw new NotImplementedException();
        }
    }
}
