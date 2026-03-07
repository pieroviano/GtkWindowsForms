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
    /// <filterpriority>1
    /// </filterpriority>
    [Designer($"System.Windows.Forms.Design.PropertyGridDesigner, {AssemblyRef.SystemDesign}")]
    public partial class PropertyGrid : ContainerControl, IComPropertyBrowser
    {

        public readonly PropertyGridBase self = new PropertyGridBase();

        private PropertyGridView _propertyView;

        private object[] _selectedObjects;

        private readonly List<TabInfo> _tabs = new List<TabInfo>();

        private static readonly object s_comComponentNameChangedEvent = new();

        private static readonly object s_propertySortChangedEvent = new();

        private static readonly object s_propertyTabChangedEvent = new();

        private static readonly object s_propertyValueChangedEvent = new();

        private static readonly object s_selectedGridItemChangedEvent = new();

        private static readonly object s_selectedObjectsChangedEvent = new();

        public event ComponentRenameEventHandler ComComponentNameChanged;
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public new event EventHandler MouseEnter
        {
            add => base.MouseEnter += value;
            remove => base.MouseEnter -= value;
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public new event EventHandler MouseLeave
        {
            add => base.MouseLeave += value;
            remove => base.MouseLeave -= value;
        }

        public event EventHandler PropertySortChanged
        {
            add => Events.AddHandler(s_propertySortChangedEvent, value);
            remove => Events.RemoveHandler(s_propertySortChangedEvent, value);
        }

        public event PropertyTabChangedEventHandler PropertyTabChanged
        {
            add => Events.AddHandler(s_propertyTabChangedEvent, value);
            remove => Events.RemoveHandler(s_propertyTabChangedEvent, value);
        }

        public event PropertyValueChangedEventHandler PropertyValueChanged
        {
            add => Events.AddHandler(s_propertyValueChangedEvent, value);
            remove => Events.RemoveHandler(s_propertyValueChangedEvent, value);
        }

        public event SelectedGridItemChangedEventHandler SelectedGridItemChanged
        {
            add => Events.AddHandler(s_selectedGridItemChangedEvent, value);
            remove => Events.RemoveHandler(s_selectedGridItemChangedEvent, value);
        }

        public event EventHandler SelectedObjectsChanged
        {
            add => Events.AddHandler(s_selectedObjectsChangedEvent, value);
            remove => Events.RemoveHandler(s_selectedObjectsChangedEvent, value);
        }
        public PropertyGrid()
        {
            _propertyView = new PropertyGridView(this);
            self.child1.Add(_propertyView.tree);
            _propertyView.PropertyValueChanged += Self_PropertyValueChanged;
            _propertyView.SelectedGridItemChanged += Self_SelectedGridItemChanged;
        }
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]

        public override bool AutoScroll { get; set; }

        public override Color BackColor { get; set; }

#nullable disable
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]

        public override Image BackgroundImage { get; set; }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]

        public override ImageLayout BackgroundImageLayout { get; set; }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [EditorBrowsable(EditorBrowsableState.Advanced)]

        public AttributeCollection BrowsableAttributes { get; set; }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Advanced)]

        public virtual bool CanShowCommands { get; }

        [DefaultValue(true)]

        public bool CanShowVisualStyleGlyphs { get; set; }

        [DefaultValue(typeof(Color), "ControlText")]

        public Color CategoryForeColor { get; set; }

        [DefaultValue(typeof(Color), "Control")]

        public Color CategorySplitterColor { get; set; }

        public Color CommandsActiveLinkColor { get; set; }

        public Color CommandsBackColor { get; set; }

        [DefaultValue(typeof(Color), "ControlDark")]

        public Color CommandsBorderColor { get; set; }

        public Color CommandsDisabledLinkColor { get; set; }

        public Color CommandsForeColor { get; set; }

        public Color CommandsLinkColor { get; set; }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Advanced)]

        public virtual bool CommandsVisible { get; }

        [DefaultValue(true)]

        public virtual bool CommandsVisibleIfAvailable { get; set; }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [EditorBrowsable(EditorBrowsableState.Advanced)]

        public Point ContextMenuDefaultLocation { get; }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [EditorBrowsable(EditorBrowsableState.Never)]

        public override ControlCollection Controls { get; }

        [DefaultValue(typeof(Color), "GrayText")]

        public Color DisabledItemForeColor { get; set; }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]

        public override Color ForeColor { get; set; }

        public override object GtkControl => self;

        [DefaultValue(typeof(Color), "Control")]

        public Color HelpBackColor { get; set; }

        [DefaultValue(typeof(Color), "ControlDark")]

        public Color HelpBorderColor { get; set; }

        [DefaultValue(typeof(Color), "ControlText")]

        public Color HelpForeColor { get; set; }

        [DefaultValue(true)]
        [Localizable(true)]

        public virtual bool HelpVisible { get; set; }

        public bool InPropertySet => true;

        [DefaultValue(false)]

        public bool LargeButtons { get; set; }

        //[Browsable(false)]
        //[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        //[EditorBrowsable(EditorBrowsableState.Never)]
        //public Padding Padding { get; set; }
        [DefaultValue(typeof(Color), "InactiveBorder")]

        public Color LineColor { get; set; }

        [DefaultValue(PropertySort.CategorizedAlphabetical)]
        public PropertySort PropertySort { get; set; }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [EditorBrowsable(EditorBrowsableState.Advanced)]

        public PropertyTabCollection PropertyTabs { get; }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [EditorBrowsable(EditorBrowsableState.Advanced)]

        public GridItem SelectedGridItem { get; set; }

        [DefaultValue(typeof(Color), "Highlight")]

        public Color SelectedItemWithFocusBackColor { get; set; }

        [DefaultValue(typeof(Color), "HighlightText")]

        public Color SelectedItemWithFocusForeColor { get; set; }

        [DefaultValue(null)]
        public object SelectedObject
        {
            get => _selectedObjects is null || _selectedObjects.Length == 0 ? null : _selectedObjects[0];
            set => SelectedObjects = value is null ? Array.Empty<object>() : (new object[] { value });
        }

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

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [EditorBrowsable(EditorBrowsableState.Advanced)]

        public PropertyTab SelectedTab { get; }

        public override ISite Site { get; set; }

        [Browsable(true)]
        [Description(" control size（ measured in pixels）。")]

        public override Size Size { get => base.Size; set { base.Size = value; self.Position = value.Height - 60; } }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]

        public override string Text { get; set; }

        [DefaultValue(true)]

        public virtual bool ToolbarVisible { get; set; }

        [DefaultValue(false)]

        public bool UseCompatibleTextRendering { get; set; }

        [DefaultValue(typeof(Color), "Window")]

        public Color ViewBackColor { get; set; }

        [DefaultValue(typeof(Color), "ControlDark")]

        public Color ViewBorderColor { get; set; }

        [DefaultValue(typeof(Color), "WindowText")]

        public Color ViewForeColor { get; set; }

        protected internal override bool ShowFocusCues { get; }

        protected override Size DefaultSize { get; }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [EditorBrowsable(EditorBrowsableState.Advanced)]

        protected virtual Type DefaultTabType { get; }

        protected bool DrawFlatToolbar { get; set; }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [EditorBrowsable(EditorBrowsableState.Advanced)]

        protected virtual Bitmap ShowPropertyPageImage { get; }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [EditorBrowsable(EditorBrowsableState.Advanced)]

        protected virtual Bitmap SortByCategoryImage { get; }

        //protected ToolStripRenderer ToolStripRenderer { get; set; }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [EditorBrowsable(EditorBrowsableState.Advanced)]

        protected virtual Bitmap SortByPropertyImage { get; }
        public void CollapseAllGridItems()
        {
        }

        public override void Refresh()
        {
        }

        public void RefreshTabs(PropertyTabScope tabScope)
        {
        }

        public void ResetSelectedProperty()
        {
        }

        void IComPropertyBrowser.DropDownDone()
        {
            throw new NotImplementedException();
        }

        bool IComPropertyBrowser.EnsurePendingChangesCommitted()
        {
            throw new NotImplementedException();
        }

        void IComPropertyBrowser.HandleF4()
        {
            throw new NotImplementedException();
        }

        void IComPropertyBrowser.LoadState(RegistryKey key)
        {
            throw new NotImplementedException();
        }

        void SaveState(RegistryKey key)
        {
            throw new NotImplementedException();
        }

        void IComPropertyBrowser.SaveState(RegistryKey key)
        {
            throw new NotImplementedException();
        }

        private void Self_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            Events[s_propertyValueChangedEvent]?.DynamicInvoke(this, e);
        }

        private void Self_SelectedGridItemChanged(object sender, SelectedGridItemChangedEventArgs e)
        {
            Events[s_selectedGridItemChangedEvent]?.DynamicInvoke(this, e);
        }
    }
}
