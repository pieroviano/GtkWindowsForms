/*
 * Based on GTK components, it is a cross-platform interface component that is compatible with the native C# control winform interface.
 * Use this component GtkSystem.Windows.Forms instead of Microsoft.WindowsDesktop.App.WindowsForms, compile once, and run across Windows, Linux, and macOS
 * Technical support 438865652@qq.com，https://www.gtkapp.com, https://gitee.com/easywebfactory, https://github.com/easywebfactory438865652@qq.com，https://www.gtkapp.com, https://gitee.com/easywebfactory, https://github.com/easywebfactory
 * author:chenhongjin
 */
using GLib;
using Gtk;
using GtkSystem.Windows.Forms.GtkControls.ControlBase;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Linq;

namespace System.Windows.Forms
{
    /// <summary>
    /// Represents a Windows control to display a list of items.
    /// </summary>
    /// <filterpriority>
    /// 1
    /// </filterpriority>
    [DesignerCategory("Component")]
    [DefaultEvent("SelectedIndexChanged")]
    [DefaultProperty("Items")]
    [DefaultBindingProperty("SelectedValue")]
    public partial class ListBox : ListControl
    {

        public SelectionMode _SelectionMode;

        public const int DefaultItemHeight = 13;

        public const int NoMatches = -1;

        public readonly ListBoxBase self = new ListBoxBase();

        internal bool IsUpdateing = false;

        private ControlBindingsCollection _collect;

        #region listcontrol
        private object _DataSource;

        private ObjectCollection _items;

        private bool _sorted;

        private int _topIndex;
        /// <summary>
        /// Initializes a new instance of the
        /// <see cref = "System.Windows.Forms.ListBox"/> class.
        /// </summary>
        public ListBox() : base()
        {
            self.ListBox.Halign = Gtk.Align.Fill;
            self.ListBox.Valign = Gtk.Align.Fill;
            self.ListBox.Hexpand = true;
            self.ListBox.Vexpand = true;
            _collect = new ControlBindingsCollection(this);
            _items = new ObjectCollection(this);
            self.ListBox.Realized += Self_Realized;
            self.ListBox.SelectedRowsChanged += ListBox_SelectedRowsChanged;
            this.BorderStyle = BorderStyle.Fixed3D;
        }
        /// <summary>
        /// Gets or sets the width of columns in a multicolumn
        /// <see cref = "System.Windows.Forms.ListBox"/>
        /// </summary>
        /// <returns>The width, in pixels, of each column in the control. The default is 0.
        /// </returns>
        /// <exception cref = "System.ArgumentException">A value less than zero is assigned to the property.
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence"/>
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        /// </PermissionSet>
        [Localizable(true)]
        [DefaultValue(0)]
        public int ColumnWidth { get; set; }

        /// <summary>
        /// Gets the width of the tabs between the items in the
        /// <see cref = "System.Windows.Forms.ListBox"/>
        /// </summary>
        /// <returns>A collection of integers representing the tab widths.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public IntegerCollection CustomTabOffsets { get; }

        #endregion
        public override ControlBindingsCollection DataBindings { get => _collect; }

        public override object DataSource
        {
            get => _DataSource;
            set
            {
                _DataSource = value;
                if (self.ListBox.IsVisible)
                {
                    OnSetDataSource();
                }
            }
        }

        /// <summary>
        /// Gets or sets the drawing mode for the control.
        /// </summary>
        /// <returns>One of the
        /// <see cref = "System.Windows.Forms.DrawMode"/> values representing the mode for drawing the items of the control. The default is DrawMode.Normal.
        /// </returns>
        /// <exception cref = "System.ComponentModel.InvalidEnumArgumentException">The value assigned to the property is not a member of the
        /// <see cref = "System.Windows.Forms.DrawMode"/> enumeration.
        /// </exception>
        /// <exception cref = "System.ArgumentException">A multicolumn
        /// <see cref = "System.Windows.Forms.ListBox"/> cannot have a variable-sized height.
        /// </exception>
        /// <filterpriority>2
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence"/>
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        /// </PermissionSet>
        [DefaultValue(DrawMode.Normal)]
        public virtual DrawMode DrawMode
        {
            get
            {
                throw new System.NotImplementedException();
            }

            set
            {
                throw new System.NotImplementedException();
            }
        }

        public override object GtkControl => self;

        /// <summary>
        /// Gets or sets the width by which the horizontal scroll bar of a
        /// <see cref = "System.Windows.Forms.ListBox"/> can scroll.
        /// </summary>
        /// <returns>The width, in pixels, that the horizontal scroll bar can scroll the control. The default is zero.
        /// </returns>
        /// <filterpriority>2
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence"/>
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        /// </PermissionSet>
        [DefaultValue(0)]
        [Localizable(true)]
        public int HorizontalExtent { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether a horizontal scroll bar is displayed in the control.
        /// </summary>
        /// <returns>true to display a horizontal scroll bar in the control; otherwise, false. The default is false.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence"/>
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        /// </PermissionSet>
        [DefaultValue(false)]
        [Localizable(true)]
        public bool HorizontalScrollbar { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the control should resize to avoid showing partial items.
        /// </summary>
        /// <returns>true if the control resizes so that it does not display partial items; otherwise, false. The default is true.
        /// </returns>
        /// <filterpriority>2
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence"/>
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        /// </PermissionSet>
        [DefaultValue(true)]
        [Localizable(true)]
        public bool IntegralHeight { get; set; }

        public override IControlGtk ISelf { get => self; }

        /// <summary>
        /// Gets or sets the height of an item in the
        /// <see cref = "System.Windows.Forms.ListBox"/>
        /// </summary>
        /// <returns>The height, in pixels, of an item in the control.
        /// </returns>
        /// <exception cref = "System.ArgumentOutOfRangeException">The
        /// <see cref = "System.Windows.Forms.ListBox.ItemHeight"/> property was set to less than 0 or more than 255 pixels.
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence"/>
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        /// </PermissionSet>
        [Localizable(true)]
        public virtual int ItemHeight { get; set; }

        /// <summary>
        /// Gets the items of the
        /// <see cref = "System.Windows.Forms.ListBox"/>
        /// </summary>
        /// <returns>An
        /// <see cref = "System.Windows.Forms.ListBox.ObjectCollection"/> representing the items in the
        /// <see cref = "System.Windows.Forms.ListBox"/>
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        [Localizable(true)]
        public ObjectCollection Items { get => _items; }

        /// <summary>
        /// Gets or sets a value indicating whether the
        /// <see cref = "System.Windows.Forms.ListBox"/> supports multiple columns.
        /// </summary>
        /// <returns>true if the
        /// <see cref = "System.Windows.Forms.ListBox"/> supports multiple columns; otherwise, false. The default is false.
        /// </returns>
        /// <exception cref = "System.ArgumentException">A multicolumn
        /// <see cref = "System.Windows.Forms.ListBox"/> cannot have a variable-sized height.
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence"/>
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        /// </PermissionSet>
        [DefaultValue(false)]
        public bool MultiColumn { get; set; }

        /// <summary>
        /// This property is not relevant to this class.
        /// </summary>
        /// <returns>A
        /// <see cref = "System.Windows.Forms.Padding"/> value.
        /// </returns>
        public new Padding Padding { get; set; }

        /// <summary>
        /// Gets the combined height of all items in the
        /// <see cref = "System.Windows.Forms.ListBox"/>
        /// </summary>
        /// <returns>The combined height, in pixels, of all items in the control.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence"/>
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        /// </PermissionSet>
        [Browsable(false)]
        public int PreferredHeight { get; }

        /// <summary>
        /// Gets or sets a value indicating whether the vertical scroll bar is shown at all times.
        /// </summary>
        /// <returns>true if the vertical scroll bar should always be displayed; otherwise, false. The default is false.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence"/>
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        /// </PermissionSet>
        [DefaultValue(false)]
        [Localizable(true)]
        public bool ScrollAlwaysVisible { get; set; }

        /// <summary>
        /// Gets or sets the zero-based index of the currently selected item in a
        /// <see cref = "System.Windows.Forms.ListBox"/>
        /// </summary>
        /// <returns>A zero-based index of the currently selected item. A value of negative one (-1) is returned if no item is selected.
        /// </returns>
        /// <exception cref = "System.ArgumentOutOfRangeException">The assigned value is less than -1 or greater than or equal to the item count.
        /// </exception>
        /// <exception cref = "System.ArgumentException">The
        /// <see cref = "System.Windows.Forms.ListBox.SelectionMode"/> property is set to None.
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence"/>
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        /// </PermissionSet>
        public override int SelectedIndex { get => self.ListBox.SelectedRow == null ? -1 : self.ListBox.SelectedRow.Index; set => SelectedItems.SetSelected(value, true); }

        /// <summary>
        /// Gets a collection that contains the zero-based indexes of all currently selected items in the
        /// <see cref = "System.Windows.Forms.ListBox"/>
        /// </summary>
        /// <returns>A
        /// <see cref = "System.Windows.Forms.ListBox.SelectedIndexCollection"/> containing the indexes of the currently selected items in the control. If no items are currently selected, an empty
        /// <see cref = "System.Windows.Forms.ListBox.SelectedIndexCollection"/> is returned.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        [Browsable(false)]
        public SelectedIndexCollection SelectedIndices
        {
            get
            {
                SelectedIndexCollection indexs = new SelectedIndexCollection(this);

                return indexs;
            }
        }

        /// <summary>
        /// Gets or sets the currently selected item in the
        /// <see cref = "System.Windows.Forms.ListBox"/>
        /// </summary>
        /// <returns>An object that represents the current selection in the control.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence"/>
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        /// </PermissionSet>
        [Browsable(false)]
        [Bindable(true)]
        public object SelectedItem
        {
            get
            {
                int index = SelectedIndex;

                return index == -1 ? null : _items[SelectedIndex];
            }

            set
            {
                ClearSelected();
                int index = _items.IndexOf(value);
                SelectedItems.SetSelected(index, true);
            }
        }

        /// <summary>
        /// Gets a collection containing the currently selected items in the
        /// <see cref = "System.Windows.Forms.ListBox"/>
        /// </summary>
        /// <returns>A
        /// <see cref = "System.Windows.Forms.ListBox.SelectedObjectCollection"/> containing the currently selected items in the control.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        [Browsable(false)]
        public SelectedObjectCollection SelectedItems
        {
            get
            {
                SelectedObjectCollection indexs = new SelectedObjectCollection(this);

                return indexs;
            }
        }

        [DefaultValue(null)]
        [Browsable(false)]
        public override object SelectedValue
        {
            get
            {
                int index = SelectedIndex;

                return index == -1 ? null : _items[SelectedIndex];
            }

            set
            {
                ClearSelected();
                int index = _items.IndexOf(value);
                SelectedItems.SetSelected(index, true);
            }
        }

        /// <summary>
        /// Gets or sets the method in which items are selected in the
        /// <see cref = "System.Windows.Forms.ListBox"/>
        /// </summary>
        /// <returns>One of the
        /// <see cref = "System.Windows.Forms.SelectionMode"/> values. The default is SelectionMode.One.
        /// </returns>
        /// <exception cref = "System.ComponentModel.InvalidEnumArgumentException">The assigned value is not one of the
        /// <see cref = "System.Windows.Forms.SelectionMode"/> values.
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence"/>
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        /// </PermissionSet>
        [DefaultValue(SelectionMode.One)]
        public virtual SelectionMode SelectionMode
        {
            get
            {
                return _SelectionMode;
            }

            set
            {
                if (value == SelectionMode.None)
                {
                    self.ListBox.SelectionMode = Gtk.SelectionMode.None;
                }
                else if (value == SelectionMode.One)
                {
                    self.ListBox.SelectionMode = Gtk.SelectionMode.Single;
                }
                else if (value == SelectionMode.MultiSimple)
                {
                    self.ListBox.SelectionMode = Gtk.SelectionMode.Multiple;
                }
                else if (value == SelectionMode.MultiExtended)
                {
                    self.ListBox.SelectionMode = Gtk.SelectionMode.Multiple;
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the items in the
        /// <see cref = "System.Windows.Forms.ListBox"/> are sorted alphabetically.
        /// </summary>
        /// <returns>true if items in the control are sorted; otherwise, false. The default is false.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence"/>
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        /// </PermissionSet>
        [DefaultValue(false)]
        public bool Sorted { get => _sorted; set => _sorted = value; }

        /// <summary>
        /// Gets or searches for the text of the currently selected item in the
        /// <see cref = "System.Windows.Forms.ListBox"/>
        /// </summary>
        /// <returns>The text of the currently selected item in the control.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence"/>
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        /// </PermissionSet>
        [Browsable(false)]
        public override string Text
        {
            get
            {
                if (self.ListBox.SelectionMode == Gtk.SelectionMode.Multiple)
                {
                    var texts = new List<string>();
                    foreach (Gtk.ListBoxRow row in self.ListBox.SelectedRows)
                    {
                        texts.Add(((Gtk.Label)row.Child).Text);
                    }

                    return string.Join(",", texts);
                }
                else
                {
                    var row = self.ListBox.SelectedRow;
                    if (row == null)
                        return string.Empty;
                    else
                        return ((Gtk.Label)row.Child).Text;
                }
            }

            set
            {
                foreach (var row in self.ListBox.Children)
                {
                    if (row is Gtk.ListBoxRow box)
                    {
                        if (((Gtk.Label)box.Child).Text == value)
                            self.ListBox.SelectRow(box);
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets the index of the first visible item in the
        /// <see cref = "System.Windows.Forms.ListBox"/>
        /// </summary>
        /// <returns>The zero-based index of the first visible item in the control.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence"/>
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        /// </PermissionSet>
        public int TopIndex
        {
            get => _topIndex;
            set
            {
                _topIndex = value;
                GLib.Timeout.Add(100, new TimeoutHandler(() =>
                {
                    int rowheight = ItemHeight;
                    if (rowheight < 14)
                    {
                        if (self.ListBox.Children.Length > 0)
                            rowheight = self.ListBox.Children[0].AllocatedHeight;
                        else
                            rowheight = 18;
                    }

                    var adjustment = self.Vadjustment;
                    adjustment.Value = value * rowheight - Height + 5;
                    return false;
                }));
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the
        /// <see cref = "System.Windows.Forms.ListBox"/> recognizes and expands tab characters when it draws its strings by using the
        /// <see cref = "System.Windows.Forms.ListBox.CustomTabOffsets"/> integer array.
        /// </summary>
        /// <returns>true if the
        /// <see cref = "System.Windows.Forms.ListBox"/> recognizes and expands tab characters; otherwise, false. The default is false.
        /// </returns>
        /// <filterpriority>2
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence"/>
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        /// </PermissionSet>
        [DefaultValue(false)]
        [Browsable(false)]
        public bool UseCustomTabOffsets { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the
        /// <see cref = "System.Windows.Forms.ListBox"/> can recognize and expand tab characters when drawing its strings.
        /// </summary>
        /// <returns>true if the control can expand tab characters; otherwise, false. The default is true.
        /// </returns>
        /// <filterpriority>2
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence"/>
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        /// </PermissionSet>
        [DefaultValue(true)]
        public bool UseTabStops { get; set; }
        internal bool ShowCheckBox { get; set; }

        internal bool ShowImage { get; set; }
        /// <summary>
        /// Maintains performance while items are added to the
        /// <see cref = "System.Windows.Forms.ListBox"/> one at a time by preventing the control from drawing until the
        /// <see cref = "System.Windows.Forms.ListBox.EndUpdate"/> method is called.
        /// </summary>
        /// <filterpriority>2
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence"/>
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        /// </PermissionSet>
        public void BeginUpdate()
        {
            IsUpdateing = true;
        }

        /// <summary>
        /// Unselects all items in the
        /// <see cref = "System.Windows.Forms.ListBox"/>
        /// </summary>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence"/>
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        /// </PermissionSet>
        public void ClearSelected()
        {
            self.ListBox.UnselectAll();
        }

        /// <summary>
        /// Resumes painting the
        /// <see cref = "System.Windows.Forms.ListBox"/> control after painting is suspended by the
        /// <see cref = "System.Windows.Forms.ListBox.BeginUpdate"/> method.
        /// </summary>
        /// <filterpriority>2
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence"/>
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        /// </PermissionSet>
        public void EndUpdate()
        {
            IsUpdateing = false;
            self.ListBox.ShowAll();
        }

        /// <summary>
        /// Finds the first item in the
        /// <see cref = "System.Windows.Forms.ListBox"/> that starts with the specified string.
        /// </summary>
        /// <returns>The zero-based index of the first item found; returns ListBox.NoMatches if no match is found.
        /// </returns>
        /// <param name = "s">The text to search for.
        /// </param>
        /// <exception cref = "System.ArgumentOutOfRangeException">The value of the
        /// <paramref name = "s"/> parameter is less than -1 or greater than or equal to the item count.
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        public int FindString(string s)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Finds the first item in the
        /// <see cref = "System.Windows.Forms.ListBox"/> that starts with the specified string. The search starts at a specific starting index.
        /// </summary>
        /// <returns>The zero-based index of the first item found; returns ListBox.NoMatches if no match is found.
        /// </returns>
        /// <param name = "s">The text to search for.
        /// </param>
        /// <param name = "startIndex">The zero-based index of the item before the first item to be searched. Set to negative one (-1) to search from the beginning of the control.
        /// </param>
        /// <exception cref = "System.ArgumentOutOfRangeException">The
        /// <paramref name = "startIndex"/> parameter is less than zero or greater than or equal to the value of the
        /// <see cref = "System.Windows.Forms.ListBox.ObjectCollection.Count"/> property of the
        /// <see cref = "System.Windows.Forms.ListBox.ObjectCollection"/> class.
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        public int FindString(string s, int startIndex)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Finds the first item in the
        /// <see cref = "System.Windows.Forms.ListBox"/> that exactly matches the specified string.
        /// </summary>
        /// <returns>The zero-based index of the first item found; returns ListBox.NoMatches if no match is found.
        /// </returns>
        /// <param name = "s">The text to search for.
        /// </param>
        /// <filterpriority>1
        /// </filterpriority>
        public int FindStringExact(string s)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Finds the first item in the
        /// <see cref = "System.Windows.Forms.ListBox"/> that exactly matches the specified string. The search starts at a specific starting index.
        /// </summary>
        /// <returns>The zero-based index of the first item found; returns ListBox.NoMatches if no match is found.
        /// </returns>
        /// <param name = "s">The text to search for.
        /// </param>
        /// <param name = "startIndex">The zero-based index of the item before the first item to be searched. Set to negative one (-1) to search from the beginning of the control.
        /// </param>
        /// <exception cref = "System.ArgumentOutOfRangeException">The
        /// <paramref name = "startIndex"/> parameter is less than zero or greater than or equal to the value of the
        /// <see cref = "System.Windows.Forms.ListBox.ObjectCollection.Count"/> property of the
        /// <see cref = "System.Windows.Forms.ListBox.ObjectCollection"/> class.
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        public int FindStringExact(string s, int startIndex)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Returns the height of an item in the
        /// <see cref = "System.Windows.Forms.ListBox"/>
        /// </summary>
        /// <returns>The height, in pixels, of the specified item.
        /// </returns>
        /// <param name = "index">The zero-based index of the item to return the height for.
        /// </param>
        /// <exception cref = "System.ArgumentOutOfRangeException">The specified value of the
        /// <paramref name = "index"/> parameter is less than zero or greater than the item count.
        /// </exception>
        /// <filterpriority>2
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence"/>
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        /// </PermissionSet>
        public int GetItemHeight(int index)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Returns the bounding rectangle for an item in the
        /// <see cref = "System.Windows.Forms.ListBox"/>
        /// </summary>
        /// <returns>A
        /// <see cref = "System.Drawing.Rectangle"/> that represents the bounding rectangle for the specified item.
        /// </returns>
        /// <param name = "index">The zero-based index of item whose bounding rectangle you want to return.
        /// </param>
        /// <exception cref = "System.ArgumentOutOfRangeException">The
        /// <paramref name = "index"/> parameter is less than zero or greater than or equal to the value of the
        /// <see cref = "System.Windows.Forms.ListBox.ObjectCollection.Count"/> property of the
        /// <see cref = "System.Windows.Forms.ListBox.ObjectCollection"/> class.
        /// </exception>
        /// <filterpriority>2
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence"/>
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        /// </PermissionSet>
        public Drawing.Rectangle GetItemRectangle(int index)
        {
            throw new System.NotImplementedException();
        }

        public override string GetItemText(object item)
        {
            if (item is ItemArray.Entry entry)
            {
                return entry.Item?.ToString();
            }

            return item?.ToString();
        }

        /// <summary>
        /// Returns a value indicating whether the specified item is selected.
        /// </summary>
        /// <returns>true if the specified item is currently selected in the
        /// <see cref = "System.Windows.Forms.ListBox"/>; otherwise, false.
        /// </returns>
        /// <param name = "index">The zero-based index of the item that determines whether it is selected.
        /// </param>
        /// <exception cref = "System.ArgumentOutOfRangeException">The
        /// <paramref name = "index"/> parameter is less than zero or greater than or equal to the value of the
        /// <see cref = "System.Windows.Forms.ListBox.ObjectCollection.Count"/> property of the
        /// <see cref = "System.Windows.Forms.ListBox.ObjectCollection"/> class.
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence"/>
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        /// </PermissionSet>
        public bool GetSelected(int index)
        {
            return self.ListBox.GetRowAtIndex(index).IsSelected;
        }

        /// <summary>
        /// Returns the zero-based index of the item at the specified coordinates.
        /// </summary>
        /// <returns>The zero-based index of the item found at the specified coordinates; returns ListBox.NoMatches if no match is found.
        /// </returns>
        /// <param name = "p">A
        /// <see cref = "System.Drawing.Point"/> object containing the coordinates used to obtain the item index.
        /// </param>
        /// <filterpriority>2
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence"/>
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        /// </PermissionSet>
        public int IndexFromPoint(Drawing.Point p)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Returns the zero-based index of the item at the specified coordinates.
        /// </summary>
        /// <returns>The zero-based index of the item found at the specified coordinates; returns ListBox.NoMatches if no match is found.
        /// </returns>
        /// <param name = "x">The x-coordinate of the location to search.
        /// </param>
        /// <param name = "y">The y-coordinate of the location to search.
        /// </param>
        /// <filterpriority>2
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence"/>
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        /// </PermissionSet>
        public int IndexFromPoint(int x, int y)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Forces the control to invalidate its client area and immediately redraw itself and any child controls.
        /// </summary>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence"/>
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        /// </PermissionSet>
        public override void Refresh()
        {
            self.ListBox.ShowAll();
        }

        /// <filterpriority>
        /// 2
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        /// </PermissionSet>
        public override void ResetBackColor()
        {
        }

        /// <filterpriority>
        /// 2
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        /// </PermissionSet>
        public override void ResetForeColor()
        {
        }

        /// <summary>
        /// Selects or clears the selection for the specified item in a
        /// <see cref = "System.Windows.Forms.ListBox"/>
        /// </summary>
        /// <param name = "index">The zero-based index of the item in a
        /// <see cref = "System.Windows.Forms.ListBox"/> to select or clear the selection for.
        /// </param>
        /// <param name = "value">true to select the specified item; otherwise, false.
        /// </param>
        /// <exception cref = "System.ArgumentOutOfRangeException">The specified index was outside the range of valid values.
        /// </exception>
        /// <exception cref = "System.InvalidOperationException">The
        /// <see cref = "System.Windows.Forms.ListBox.SelectionMode"/> property was set to None.
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence"/>
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        /// </PermissionSet>
        public void SetSelected(int index, bool value)
        {
            if (value == true)
                self.ListBox.SelectRow(self.ListBox.GetRowAtIndex(index));
            else
                self.ListBox.UnselectRow(self.ListBox.GetRowAtIndex(index));
        }

        protected void NativeAdd(object item)
        {
            Gtk.ListBoxRow row = new Gtk.ListBoxRow();
            row.HeightRequest = ItemHeight > 0 ? ItemHeight : DefaultItemHeight;
            row.Add(new Gtk.Label(item.ToString()) { Valign = Align.Center, Halign = Align.Start, Expand = true });
            self.ListBox.Add(row);
            if (self.ListBox.IsVisible && !IsUpdateing)
            {
                self.ListBox.ShowAll();
            }
        }

        protected void NativeClear()
        {
            int count = self.ListBox.Children.Length;
            while (count > 0)
            {
                self.ListBox.Remove(self.ListBox.GetRowAtIndex(count - 1));
                count--;
                //System.Threading.Thread.Sleep(3);
            }
        }

        protected string NativeGetItemText(int index)
        {
            Gtk.Label row = self.ListBox.GetRowAtIndex(index).Child as Gtk.Label;

            return row?.Text;
        }

        protected void NativeInsert(int index, object item)
        {
            Gtk.ListBoxRow row = new Gtk.ListBoxRow();
            row.HeightRequest = ItemHeight > 0 ? ItemHeight : DefaultItemHeight;
            row.Add(new Gtk.Label(item.ToString()) { Valign = Align.Center, Halign = Align.Start, Expand = true });
            self.ListBox.Insert(row, index);
            if (self.ListBox.IsVisible && !IsUpdateing)
            {
                self.ListBox.ShowAll();
            }
        }

        protected void NativeRemoveAt(int index)
        {
            self.ListBox.Remove(self.ListBox.GetRowAtIndex(index));
        }

        /// <param name = "e">
        /// Event object with the details
        /// </param>
        protected void OnSelectedIndexChanged(EventArgs e)
        {
            if (self.ListBox.SelectedRow != null)
                self.ListBox.SelectRow(self.ListBox.SelectedRow);
        }

        protected override void SetStyle(Widget widget)
        {
            self.ListBox.Name = this.Name;
            base.SetStyle(self.ListBox);
        }

        private void CheckNoDataSource()
        {
            //if (DataSource != null)
            //{
            //    throw new ArgumentException("SR.DataSourceLocksItems");
            //}
        }

        private void ListBox_SelectedRowsChanged(object sender, EventArgs e)
        {
            if (self.ListBox.IsVisible)
            {
                ((EventHandler)Events["SelectedIndexChanged"])?.Invoke(this, e);
                ((EventHandler)Events["SelectedValueChanged"])?.Invoke(this, e);
                ((EventHandler)Events["SelectedItemChanged"])?.Invoke(this, e);
            }
        }

        private void OnSetDataSource()
        {
            if (_DataSource != null)
            {
                if (_DataSource is IListSource listSource)
                {
                    IEnumerator list = listSource.GetList().GetEnumerator();
                    SetDataSource(list);
                }
                else if (_DataSource is IEnumerable list1)
                {
                    SetDataSource(list1.GetEnumerator());
                }
            }
        }

        private void propertyNotity(object o, NotifyArgs args)
        {
            Binding binding = DataBindings[args.Property];
            binding.WriteValue();
        }

        private void Self_Realized(object sender, EventArgs e)
        {
            OnSetDataSource();
            foreach (Binding binding in DataBindings)
                self.ListBox.AddNotification(binding.PropertyName, propertyNotity);
        }

        private void SetDataSource(IEnumerator enumerator)
        {
            _items.Clear();
            if (enumerator != null)
            {
                if (string.IsNullOrWhiteSpace(DisplayMember))
                {
                    while (enumerator.MoveNext())
                    {
                        var o = enumerator.Current;
                        if (o is DataRowView row)
                            _items.Add(row[0]);
                        else
                            _items.Add(enumerator.Current);
                    }
                }
                else
                {
                    while (enumerator.MoveNext())
                    {
                        var o = enumerator.Current;
                        if (o is DataRowView row)
                            _items.Add(row[DisplayMember]);
                        else
                            _items.Add(o.GetType().GetProperty(DisplayMember)?.GetValue(o));
                    }
                }
            }
        }

        public class ListBoxItem : Gtk.Label
        {
            public ListBoxItem()
            {
                base.Xalign = 0;
            }
            public object CheckValue { get; set; }

            public object DisplayText
            {
                get
                {
                    return base.Text;
                }

                set
                {
                    base.Text = value?.ToString();
                }
            }

            public object ItemValue { get; set; }

            public override string ToString()
            {
                return DisplayText?.ToString();
            }
        }
    }
}
