/*
 * Based on GTK components, it is a cross-platform interface component that is compatible with the native C# control winform interface.
 * Use this component GtkSystem.Windows.Forms instead of Microsoft.WindowsDesktop.App.WindowsForms, compile once, and run across Windows, Linux, and macOS
 * Technical support 438865652@qq.com，https://www.gtkapp.com, https://gitee.com/easywebfactory, https://github.com/easywebfactory438865652@qq.com，https://www.gtkapp.com, https://gitee.com/easywebfactory, https://github.com/easywebfactory
 * author:chenhongjin
 */

using GLib;
using Gtk;
using GtkSystem.Windows.Forms.GtkControls.ControlBase;
using Pango;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;

namespace System.Windows.Forms
{
    /// <summary>
    /// Represents a Windows list view control, which displays a collection of items that can be displayed using one of four different views.
    /// </summary>
    /// <filterpriority>
    /// 1
    /// </filterpriority>
    [DefaultEvent("SelectedIndexChanged")]
    public class ListView : ContainerControl
    {

        public readonly ListViewBase self = new ListViewBase();

        internal Gtk.Box flowBoxContainer = new Gtk.Box(Gtk.Orientation.Vertical, 0);

        internal Gtk.Box header = new Gtk.Box(Gtk.Orientation.Horizontal, 0);

        internal Gtk.Layout headerView;

        internal Gtk.ScrolledWindow scrolledWindow = new Gtk.ScrolledWindow();

        private int __headerheight = 30;

        private ColumnHeaderCollection _columns;

        private ListViewGroup _defaultGroup;

        private int _fontSize;

        private ListViewGroupCollection _groups;

        private ListViewItemCollection _items;

        private bool _MultiSelect = true;

        private bool ControlRealized = false;

        private bool IsCacheUpdate;

        private int SortingColumnIndex = -1;

        public event ColumnClickEventHandler ColumnClick;

        public event ColumnReorderedEventHandler ColumnReordered;

        public event EventHandler ItemActivate;

        public event ItemCheckEventHandler ItemCheck;

        public event ItemCheckedEventHandler ItemChecked;

        public event ListViewItemSelectionChangedEventHandler ItemSelectionChanged;

        public event EventHandler SelectedIndexChanged;
        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="System.Windows.Forms.ListView" /> class.
        /// </summary>
        public ListView() : base()
        {
            _items = new ListViewItemCollection(this);
            _groups = new ListViewGroupCollection(this);
            _columns = new ColumnHeaderCollection(this);
            self.Realized += Control_Realized;

            header.StyleContext.AddClass("ListViewHeader");
            header.Spacing = 0;
            header.BorderWidth = 0;
            header.Halign = Gtk.Align.Fill;
            header.Valign = Gtk.Align.Fill;
            header.HeightRequest = 1;
            header.Homogeneous = false;
            flowBoxContainer.Halign = Gtk.Align.Fill;
            flowBoxContainer.Valign = Gtk.Align.Start;
            scrolledWindow.Halign = Gtk.Align.Fill;
            scrolledWindow.Valign = Gtk.Align.Fill;
            scrolledWindow.Add(flowBoxContainer);
            scrolledWindow.OverlayScrolling = false;
            scrolledWindow.Hadjustment.ValueChanged += Hadjustment_ValueChanged;
            headerView = new Gtk.Layout(null, null);
            headerView.Halign = Gtk.Align.Fill;
            headerView.Valign = Gtk.Align.Start;
            headerView.Hexpand = true;
            headerView.Vexpand = false;
            headerView.HeightRequest = 1;
            headerView.Width = 100000;
            headerView.Add(header);
            self.box.PackStart(headerView, false, true, 0);
            self.box.PackStart(scrolledWindow, true, true, 0);
            this.BorderStyle = BorderStyle.Fixed3D;
        }
        /// <summary>
        /// Gets or sets the type of action the user must take to activate an item.
        /// </summary>
        /// <returns>One of the
        /// <see cref="System.Windows.Forms.ItemActivation" /> values. The default is
        /// <see cref="System.Windows.Forms.ItemActivation.Standard" />
        /// </returns>
        /// <exception cref="System.ComponentModel.InvalidEnumArgumentException">The value specified is not one of the
        /// <see cref="System.Windows.Forms.ItemActivation" /> members.
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public ItemActivation Activation { get; set; }

        /// <summary>
        /// Gets or sets the alignment of items in the control.
        /// </summary>
        /// <returns>One of the
        /// <see cref="System.Windows.Forms.ListViewAlignment" /> values. The default is
        /// <see cref="System.Windows.Forms.ListViewAlignment.Top" />
        /// </returns>
        /// <exception cref="System.ComponentModel.InvalidEnumArgumentException">The value specified is not one of the
        /// <see cref="System.Windows.Forms.ListViewAlignment" /> values.
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public System.Windows.Forms.ListViewAlignment Alignment { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the user can drag column headers to reorder columns in the control.
        /// </summary>
        /// <returns>true if drag-and-drop column reordering is allowed; otherwise, false. The default is false.
        /// </returns>
        /// <filterpriority>2
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public bool AllowColumnReorder { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether a check box appears next to each item in the control.
        /// </summary>
        /// <returns>true if a check box appears next to each item in the
        /// <see cref="System.Windows.Forms.ListView" /> control; otherwise, false. The default is false.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public bool CheckBoxes { get; set; }

        /// <summary>
        /// Gets the indexes of the currently checked items in the control.
        /// </summary>
        /// <returns>A
        /// <see cref="System.Windows.Forms.ListView.CheckedIndexCollection" /> that contains the indexes of the currently checked items. If no items are currently checked, an empty
        /// <see cref="System.Windows.Forms.ListView.CheckedIndexCollection" /> is returned.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public CheckedIndexCollection CheckedIndices
        {
            get
            {
                CheckedIndexCollection selecteditems = new CheckedIndexCollection(this);
                foreach (ListViewItem item in CheckedItems)
                {
                    selecteditems.Add(item.Index);
                }

                return selecteditems;
            }
        }

        /// <summary>
        /// Gets the currently checked items in the control.
        /// </summary>
        /// <returns>A
        /// <see cref="System.Windows.Forms.ListView.CheckedListViewItemCollection" /> that contains the currently checked items. If no items are currently checked, an empty
        /// <see cref="System.Windows.Forms.ListView.CheckedListViewItemCollection" /> is returned.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public CheckedListViewItemCollection CheckedItems
        {
            get
            {
                CheckedListViewItemCollection selecteditems = new CheckedListViewItemCollection(this);
                foreach (ListViewItem item in this.Items)
                {
                    if (item.Checked)
                    {
                        selecteditems.Add(item);
                    }
                }

                return selecteditems;
            }
        }

        /// <summary>
        /// Gets the collection of all column headers that appear in the control.
        /// </summary>
        /// <returns>A
        /// <see cref="System.Windows.Forms.ListView.ColumnHeaderCollection" /> that represents the column headers that appear when the
        /// <see cref="System.Windows.Forms.ListView.View" /> property is set to
        /// <see cref="System.Windows.Forms.View.Details" />
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public ColumnHeaderCollection Columns
        {
            get
            {
                return _columns;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether clicking an item selects all its subitems.
        /// </summary>
        /// <returns>true if clicking an item selects the item and all its subitems; false if clicking an item selects only the item itself. The default is false.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public bool FullRowSelect { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether grid lines appear between the rows and columns containing the items and subitems in the control.
        /// </summary>
        /// <returns>true if grid lines are drawn around items and subitems; otherwise, false. The default is false.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public bool GridLines { get; set; } = true;

        public ImageList GroupImageList { get; set; }

        /// <summary>
        /// Gets the collection of
        /// <see cref="System.Windows.Forms.ListViewGroup" /> objects assigned to the control.
        /// </summary>
        /// <returns>A
        /// <see cref="System.Windows.Forms.ListViewGroupCollection" /> that contains all the groups in the
        /// <see cref="System.Windows.Forms.ListView" /> control.
        /// </returns>
        /// <filterpriority>2
        /// </filterpriority>
        public ListViewGroupCollection Groups
        {
            get
            {
                return _groups;
            }
        }

        public override object GtkControl => self;

        /// <summary>
        /// Gets or sets the column header style.
        /// </summary>
        /// <returns>One of the
        /// <see cref="System.Windows.Forms.ColumnHeaderStyle" /> values. The default is
        /// <see cref="System.Windows.Forms.ColumnHeaderStyle.Clickable" />
        /// </returns>
        /// <exception cref="System.ComponentModel.InvalidEnumArgumentException">The value specified is not one of the
        /// <see cref="System.Windows.Forms.ColumnHeaderStyle" /> values.
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public System.Windows.Forms.ColumnHeaderStyle HeaderStyle { get; set; } = ColumnHeaderStyle.Clickable;

        /// <summary>
        /// Gets or sets a value indicating whether the selected item in the control remains highlighted when the control loses focus.
        /// </summary>
        /// <returns>true if the selected item does not appear highlighted when the control loses focus; false if the selected item still appears highlighted when the control loses focus. The default is true.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public bool HideSelection { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether an item is automatically selected when the mouse pointer remains over the item for a few seconds.
        /// </summary>
        /// <returns>true if an item is automatically selected when the mouse pointer hovers over it; otherwise, false. The default is false.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public bool HoverSelection { get; set; }

        /// <summary>
        /// Gets a collection containing all items in the control.
        /// </summary>
        /// <returns>A
        /// <see cref="System.Windows.Forms.ListView.ListViewItemCollection" /> that contains all the items in the
        /// <see cref="System.Windows.Forms.ListView" /> control.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public ListViewItemCollection Items
        {
            get
            {
                return _items;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the user can edit the labels of items in the control.
        /// </summary>
        /// <returns>true if the user can edit the labels of items at run time; otherwise, false. The default is false.
        /// </returns>
        /// <filterpriority>2
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public bool LabelEdit { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether item labels wrap when items are displayed in the control as icons.
        /// </summary>
        /// <returns>true if item labels wrap when items are displayed as icons; otherwise, false. The default is true.
        /// </returns>
        /// <filterpriority>2
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public bool LabelWrap { get; set; }

        /// <summary>
        /// Gets or sets the
        /// <see cref="System.Windows.Forms.ImageList" /> to use when displaying items as large icons in the control.
        /// </summary>
        /// <returns>An
        /// <see cref="System.Windows.Forms.ImageList" /> that contains the icons to use when the
        /// <see cref="System.Windows.Forms.ListView.View" /> property is set to
        /// <see cref="System.Windows.Forms.View.LargeIcon" /> The default is null.
        /// </returns>
        /// <filterpriority>2
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public ImageList LargeImageList { get; set; }

        /// <summary>
        /// Gets or sets the sorting comparer for the control.
        /// </summary>
        /// <returns>An
        /// <see cref="System.Collections.IComparer" /> that represents the sorting comparer for the control.
        /// </returns>
        /// <filterpriority>2
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public IComparer ListViewItemSorter
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether multiple items can be selected.
        /// </summary>
        /// <returns>true if multiple items in the control can be selected at one time; otherwise, false. The default is true.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public virtual bool MultiSelect
        {
            get
            {
                return _MultiSelect;
            }
            set
            {
                _MultiSelect = value;
                foreach (var group in GetAllGroups())
                {
                    if (value == true)
                    {
                        group.FlowBox.SelectionMode = Gtk.SelectionMode.Multiple;
                    }
                    else
                    {
                        group.FlowBox.SelectionMode = Gtk.SelectionMode.Single;
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the
        /// <see cref="System.Windows.Forms.ListView" /> control is drawn by the operating system or by code that you provide.
        /// </summary>
        /// <returns>true if the
        /// <see cref="System.Windows.Forms.ListView" /> control is drawn by code that you provide; false if the
        /// <see cref="System.Windows.Forms.ListView" /> control is drawn by the operating system. The default is false.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public bool OwnerDraw { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether a scroll bar is added to the control when there is not enough room to display all items.
        /// </summary>
        /// <returns>true if scroll bars are added to the control when necessary to allow the user to see all the items; otherwise, false. The default is true.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public bool Scrollable { get; set; }

        /// <summary>
        /// Gets the indexes of the selected items in the control.
        /// </summary>
        /// <returns>A
        /// <see cref="System.Windows.Forms.ListView.SelectedIndexCollection" /> that contains the indexes of the selected items. If no items are currently selected, an empty
        /// <see cref="System.Windows.Forms.ListView.SelectedIndexCollection" /> is returned.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public SelectedIndexCollection SelectedIndices
        {
            get
            {
                SelectedIndexCollection selecteditems = new SelectedIndexCollection();
                foreach (ListViewItem item in SelectedItems)
                {
                    selecteditems.Add(item.Index);
                }

                return selecteditems;
            }
        }

        /// <summary>
        /// Gets the items that are selected in the control.
        /// </summary>
        /// <returns>A
        /// <see cref="System.Windows.Forms.ListView.SelectedListViewItemCollection" /> that contains the items that are selected in the control. If no items are currently selected, an empty
        /// <see cref="System.Windows.Forms.ListView.SelectedListViewItemCollection" /> is returned.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public SelectedListViewItemCollection SelectedItems
        {
            get
            {
                SelectedListViewItemCollection selecteditems = new SelectedListViewItemCollection();
                foreach (ListViewItem item in this.Items)
                {
                    if (item.Selected)
                    {
                        selecteditems.Add(item);
                    }
                }

                return selecteditems;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether items are displayed in groups.
        /// </summary>
        /// <returns>true to display items in groups; otherwise, false. The default value is true.
        /// </returns>
        /// <filterpriority>2
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public bool ShowGroups { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether ToolTips are shown for the
        /// <see cref="System.Windows.Forms.ListViewItem" /> objects contained in the
        /// <see cref="System.Windows.Forms.ListView" />
        /// </summary>
        /// <returns>true if
        /// <see cref="System.Windows.Forms.ListViewItem" /> ToolTips should be shown; otherwise, false. The default is true.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public bool ShowItemToolTips { get; set; }

        /// <summary>
        /// Gets or sets the
        /// <see cref="System.Windows.Forms.ImageList" /> to use when displaying items as small icons in the control.
        /// </summary>
        /// <returns>An
        /// <see cref="System.Windows.Forms.ImageList" /> that contains the icons to use when the
        /// <see cref="System.Windows.Forms.ListView.View" /> property is set to
        /// <see cref="System.Windows.Forms.View.SmallIcon" /> The default is null.
        /// </returns>
        /// <filterpriority>2
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public ImageList SmallImageList { get; set; }

        public bool Sorted { get; set; }

        /// <summary>
        /// Gets or sets the sort order for items in the control.
        /// </summary>
        /// <returns>One of the
        /// <see cref="System.Windows.Forms.SortOrder" /> values. The default is
        /// <see cref="System.Windows.Forms.SortOrder.None" />
        /// </returns>
        /// <exception cref="System.ComponentModel.InvalidEnumArgumentException">The value specified is not one of the
        /// <see cref="System.Windows.Forms.SortOrder" /> values.
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public System.Windows.Forms.SortOrder Sorting { get; set; }

        /// <summary>
        /// Gets or sets the
        /// <see cref="System.Windows.Forms.ImageList" /> associated with application-defined states in the control.
        /// </summary>
        /// <returns>An
        /// <see cref="System.Windows.Forms.ImageList" /> that contains a set of state images that can be used to indicate an application-defined state of an item. The default is null.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public ImageList StateImageList { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the
        /// <see cref="System.Windows.Forms.ListView" /> uses state image behavior that is compatible with the .NET Framework 1.1 or the .NET Framework 2.0.
        /// </summary>
        /// <returns>true if the state image behavior is compatible with the .NET Framework 1.1; false if the behavior is compatible with the .NET Framework 2.0. The default is true.
        /// </returns>
        public bool UseCompatibleStateImageBehavior { get; set; }

        /// <summary>
        /// Gets or sets how items are displayed in the control.
        /// </summary>
        /// <returns>One of the
        /// <see cref="System.Windows.Forms.View" /> values. The default is
        /// <see cref="System.Windows.Forms.View.LargeIcon" />
        /// </returns>
        /// <exception cref="System.ComponentModel.InvalidEnumArgumentException">The value specified is not one of the
        /// <see cref="System.Windows.Forms.View" /> values.
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        public System.Windows.Forms.View View { get; set; }
        internal ListViewGroup DefaultGroup
        {
            get
            {
                if (_defaultGroup == null)
                {
                    _defaultGroup = ListViewGroup.CreateDefaultListViewGroup();
                }

                return _defaultGroup;
            }
        }

        internal int FontSize
        {
            get
            {
                if (_fontSize < 5)
                {
                    if (Font?.Size != null && Font.Size > 5)
                    {
                        if (Font.Unit == GraphicsUnit.Point)
                            _fontSize = (int)Font.Size * 96 / 72;
                        else
                            _fontSize = (int)Font.Size;
                    }
                }

                return _fontSize;
            }
        }
        /// <summary>
        /// Prevents the control from drawing until the
        /// <see cref="System.Windows.Forms.ListView.EndUpdate" /> method is called.
        /// </summary>
        /// <filterpriority>2
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public void BeginUpdate()
        {
            IsCacheUpdate = true;
        }

        /// <summary>
        /// Removes all items and columns from the control.
        /// </summary>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public void Clear()
        {
            Items.Clear();
            Groups.Clear();
        }

        /// <summary>
        /// Resumes drawing of the list view control after drawing is suspended by the
        /// <see cref="System.Windows.Forms.ListView.BeginUpdate" /> method.
        /// </summary>
        /// <filterpriority>2
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public void EndUpdate()
        {
            IsCacheUpdate = false;
            self.Window.ProcessUpdates(true);
            self.ShowAll();
        }

        /// <summary>
        /// Finds the first
        /// <see cref="System.Windows.Forms.ListViewItem" /> that begins with the specified text value.
        /// </summary>
        /// <returns>The first
        /// <see cref="System.Windows.Forms.ListViewItem" /> that begins with the specified text value.
        /// </returns>
        /// <param name="text">The text to search for.
        /// </param>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public ListViewItem FindItemWithText(string text)
        {
            return Items.Find(w => w.Text == text);
        }

        /// <summary>
        /// Finds the first
        /// <see cref="System.Windows.Forms.ListViewItem" /> or
        /// <see cref="System.Windows.Forms.ListViewItem.ListViewSubItem" />, if indicated, that begins with the specified text value. The search starts at the specified index.
        /// </summary>
        /// <returns>The first
        /// <see cref="System.Windows.Forms.ListViewItem" /> that begins with the specified text value.
        /// </returns>
        /// <param name="text">The text to search for.
        /// </param>
        /// <param name="includeSubItemsInSearch">true to include subitems in the search; otherwise, false.
        /// </param>
        /// <param name="startIndex">The index of the item at which to start the search.
        /// </param>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// <paramref name="startIndex" /> is less 0 or more than the number items in the
        /// <see cref="System.Windows.Forms.ListView" />
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public ListViewItem FindItemWithText(string text, bool includeSubItemsInSearch, int startIndex)
        {
            int idx = Items.FindIndex(startIndex, w => w.Text == text);

            return idx == -1 ? null : Items[idx];
        }

        /// <summary>
        /// Finds the first
        /// <see cref="System.Windows.Forms.ListViewItem" /> or
        /// <see cref="System.Windows.Forms.ListViewItem.ListViewSubItem" />, if indicated, that begins with the specified text value. The search starts at the specified index.
        /// </summary>
        /// <returns>The first
        /// <see cref="System.Windows.Forms.ListViewItem" /> that begins with the specified text value.
        /// </returns>
        /// <param name="text">The text to search for.
        /// </param>
        /// <param name="includeSubItemsInSearch">true to include subitems in the search; otherwise, false.
        /// </param>
        /// <param name="startIndex">The index of the item at which to start the search.
        /// </param>
        /// <param name="isPrefixSearch">true to allow partial matches; otherwise, false.
        /// </param>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// <paramref name="startIndex" /> is less than 0 or more than the number of items in the
        /// <see cref="System.Windows.Forms.ListView" />
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public ListViewItem FindItemWithText(string text, bool includeSubItemsInSearch, int startIndex, bool isPrefixSearch)
        {
            int idx = Items.FindIndex(startIndex, w => w.Text == text);

            return idx == -1 ? null : Items[idx];
        }

        /// <summary>
        /// Retrieves the item at the specified location.
        /// </summary>
        /// <returns>A
        /// <see cref="System.Windows.Forms.ListViewItem" /> that represents the item at the specified position. If there is no item at the specified location, the method returns null.
        /// </returns>
        /// <param name="x">The x-coordinate of the location to search for an item (expressed in client coordinates).
        /// </param>
        /// <param name="y">The y-coordinate of the location to search for an item (expressed in client coordinates).
        /// </param>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public ListViewItem GetItemAt(int x, int y)
        {
            foreach (Gtk.Box vbox in flowBoxContainer.Children)
            {
                foreach (var flow in vbox.Children)
                {
                    if (flow is Gtk.FlowBox _flow)
                    {
                        int top = _flow.Allocation.Top + __headerheight - (int)scrolledWindow.Vadjustment.Value;
                        FlowBoxChild child = _flow.GetChildAtPos(x + (int)scrolledWindow.Hadjustment.Value, y - top);
                        if (child != null)
                        {
                            return this.Items.Find(m => m.Index == Convert.ToInt32(child.Data["ItemId"]));
                        }
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Retrieves the bounding rectangle for a specific item within the list view control.
        /// </summary>
        /// <returns>A
        /// <see cref="System.Drawing.Rectangle" /> that represents the bounding rectangle of the specified
        /// <see cref="System.Windows.Forms.ListViewItem" />
        /// </returns>
        /// <param name="index">The zero-based index of the item within the
        /// <see cref="System.Windows.Forms.ListView.ListViewItemCollection" /> whose bounding rectangle you want to return.
        /// </param>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public Drawing.Rectangle GetItemRect(int index)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Sorts the items of the list view.
        /// </summary>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public void Sort()
        {
            foreach (var group in GetAllGroups())
            {
                group.FlowBox.InvalidateSort();
            }
        }
        internal void GetSubItemAt(int x, int y, out int iItem, out int iSubItem)
        {
            iItem = -1;
            iSubItem = -1;
            int position = -(int)headerView.Hadjustment.Value;
            for (int i = 0; i < Columns.Count; i++)
            {
                if (position < x && position + Columns[i].Width > x)
                {
                    iSubItem = i;
                    break;
                }
                position += Columns[i].Width;
            }
        }

        internal void NativeAdd(ListViewItem item, int position)
        {
            if (self.IsRealized)
            {
                Gtk.FlowBoxChild boxitem = new Gtk.FlowBoxChild();
                boxitem.TooltipText = item.Text;
                boxitem.Data.Add("ItemId", item.Index);
                boxitem.Halign = Gtk.Align.Start;
                boxitem.Valign = Gtk.Align.Start;
                boxitem.BorderWidth = 0;
                boxitem.Margin = 0;
                boxitem.HeightRequest = FontSize + 10;
                item._flowBoxChild = boxitem;
                BoxBase hBox = new BoxBase(Gtk.Orientation.Horizontal, 0);
                if (item.BackColor.HasValue)
                {
                    hBox.Override.BackColor = Drawing.Color.FromArgb(180, item.BackColor.Value);
                }
                hBox.Valign = Gtk.Align.Fill;
                hBox.Halign = Gtk.Align.Start;
                hBox.BorderWidth = 0;
                hBox.Homogeneous = false;

                foreach (ColumnHeader col in Columns)
                {
                    if (item.SubItems != null && item.SubItems.Count > col.Index)
                    {
                        boxitem.Data.Add(col.Index, item.SubItems[col.Index].Text);
                    }
                    else
                        boxitem.Data.Add(col.Index, string.Empty);
                }

                boxitem.Add(hBox);

                Gtk.FlowBox flowBox = DefaultGroup.FlowBox;
                if (this.ShowGroups == true && this.View != View.List && this.View != View.Tile)
                {
                    if (Groups.Exists(g => g.Name == item.Group.Name))
                        flowBox = item.Group.FlowBox;
                    else
                        NativeGroupAdd(DefaultGroup, -1);
                }
                else
                {
                    NativeGroupAdd(DefaultGroup, -1);
                }

                if (position == -1)
                    flowBox.Add(boxitem);
                else
                    flowBox.Insert(boxitem, position);

                if (this.View == View.Details)
                {
                    hBox.Spacing = 0;
                    header.Visible = true;
                    flowBox.MinChildrenPerLine = 1;
                    flowBox.MaxChildrenPerLine = 1;
                    Gtk.Layout fistcell = new Gtk.Layout(new Adjustment(IntPtr.Zero), new Adjustment(IntPtr.Zero));
                    fistcell.Halign = Gtk.Align.Start;
                    fistcell.Valign = Gtk.Align.Fill;
                    fistcell.Vexpand = true;
                    fistcell.BorderWidth = 0;
                    fistcell.WidthRequest = Columns.Count > 0 ? Columns[0].Width : Width;
                    fistcell.HeightRequest = boxitem.HeightRequest;

                    int x_position = 5;
                    int padding = 2;
                    if (this.CheckBoxes == true)
                    {
                        CheckBox checkBox = new CheckBox();
                        checkBox.self.Halign = Gtk.Align.Start;
                        checkBox.self.Valign = Gtk.Align.Center;
                        checkBox.Width = 20;
                        checkBox.self.BorderWidth = 0;
                        checkBox.Checked = item.Checked;
                        checkBox.CheckedChanged += (object sender, EventArgs e) =>
                        {
                            CheckBox box = sender as CheckBox;
                            Gtk.FlowBoxChild checkitem = box.self.Parent.Parent.Parent.Parent as Gtk.FlowBoxChild;
                            ListViewItem thisitem = this.Items.Find(m => m.Index == Convert.ToInt32(checkitem.Data["ItemId"]));
                            if (thisitem != null)
                            {
                                thisitem.Checked = box.self.Active;
                                if (ItemCheck != null)
                                {
                                    ItemCheck(sender, new ItemCheckEventArgs(checkitem.Index, box.self.Active ? CheckState.Checked : CheckState.Unchecked, box.self.Active ? CheckState.Unchecked : CheckState.Checked));
                                }
                                if (ItemChecked != null)
                                {
                                    ItemChecked(sender, new ItemCheckedEventArgs(thisitem));
                                }
                            }
                        };

                        fistcell.Put(checkBox.self, x_position, padding);
                        x_position += 20;
                    }

                    if (this.SmallImageList != null)
                    {
                        int imgsize = FontSize + 2;
                        this.SmallImageList.ImageSize = new Size(imgsize, imgsize);
                        if (!string.IsNullOrWhiteSpace(item.ImageKey))
                        {
                            Drawing.Image img = this.SmallImageList.GetBitmap(item.ImageKey);
                            if (img != null)
                                fistcell.Put(new Gtk.Image(img.Pixbuf) { Halign = Gtk.Align.Start, Valign = Gtk.Align.Fill }, x_position, padding + 2);
                            x_position += imgsize + 5;
                        }
                        else if (item.ImageIndex > -1)
                        {
                            Drawing.Image img = this.SmallImageList.GetBitmap(item.ImageIndex);
                            if (img != null)
                                fistcell.Put(new Gtk.Image(img.Pixbuf) { Halign = Gtk.Align.Start, Valign = Gtk.Align.Fill }, x_position, padding + 2);
                            x_position += imgsize + 5;
                        }
                    }
                    Gtk.Label lab = new Gtk.Label();
                    Pango.AttrList attributes = new Pango.AttrList();
                    if (item.ForeColor.HasValue)
                    {
                        Pango.AttrForeground fg = new Pango.AttrForeground(Convert.ToUInt16(item.ForeColor.Value.R * 257), Convert.ToUInt16(item.ForeColor.Value.G * 257), Convert.ToUInt16(item.ForeColor.Value.B * 257));
                        attributes.Insert(fg);
                    }
                    lab.Attributes = attributes;
                    lab.Halign = Gtk.Align.Start;
                    lab.Valign = Gtk.Align.Fill;
                    lab.Ellipsize = Pango.EllipsizeMode.End;
                    lab.Text = item.Text;
                    lab.Xalign = 0;
                    lab.Xpad = 0;
                    lab.Ypad = 0;
                    lab.WidthRequest = fistcell.WidthRequest - x_position;
                    fistcell.Put(lab, x_position, padding);
                    Gtk.Viewport viewport = new Viewport();
                    viewport.WidthRequest = Columns.Count > 0 ? Columns[0].Width : Width;
                    viewport.BorderWidth = 0;
                    viewport.Add(fistcell);
                    hBox.PackStart(viewport, false, true, 0);

                    int index = 0;
                    foreach (ColumnHeader col in Columns)
                    {
                        if (index > 0)
                        {
                            Gtk.Layout sublayout = new Gtk.Layout(new Adjustment(IntPtr.Zero), new Adjustment(IntPtr.Zero));
                            sublayout.Halign = Gtk.Align.Start;
                            sublayout.Valign = Gtk.Align.Fill;
                            sublayout.WidthRequest = col.Width;
                            if (item.SubItems != null && item.SubItems.Count > index)
                            {
                                ListViewItem.ListViewSubItem subitem = item.SubItems[index];
                                Gtk.Label sublabel = new Gtk.Label();
                                subitem._label = sublabel;
                                Pango.AttrList subattributes = new Pango.AttrList();
                                if (subitem.ForeColor.HasValue)
                                {
                                    Pango.AttrForeground fg = new Pango.AttrForeground(Convert.ToUInt16(subitem.ForeColor.Value.R * 257), Convert.ToUInt16(subitem.ForeColor.Value.G * 257), Convert.ToUInt16(subitem.ForeColor.Value.B * 257));
                                    subattributes.Insert(fg);
                                    sublabel.Attributes = subattributes;
                                }
                                sublabel.Attributes = subattributes;
                                sublabel.WidthRequest = col.Width + 2;
                                sublabel.MaxWidthChars = 0;

                                sublabel.Halign = Gtk.Align.Fill;
                                sublabel.Valign = Gtk.Align.Fill;
                                sublabel.Ellipsize = Pango.EllipsizeMode.End;
                                sublabel.Text = subitem.Text;
                                sublayout.Add(sublabel);
                            }
                            Gtk.Viewport sviewport = new Viewport();
                            sviewport.WidthRequest = col.Width;
                            sviewport.BorderWidth = 0;
                            sviewport.Add(sublayout);
                            hBox.PackStart(sviewport, false, true, 0);
                        }
                        index++;
                    }
                }
                else
                {
                    if (this.CheckBoxes == true)
                    {
                        CheckBox checkBox = new CheckBox();
                        checkBox.self.Halign = Gtk.Align.Start;
                        checkBox.self.Valign = Gtk.Align.Center;
                        checkBox.Width = 20;
                        checkBox.self.BorderWidth = 0;
                        checkBox.Checked = item.Checked;
                        checkBox.CheckedChanged += (object sender, EventArgs e) =>
                        {
                            CheckBox box = sender as CheckBox;
                            Gtk.FlowBoxChild checkitem = box.self.Parent.Parent as Gtk.FlowBoxChild;
                            ListViewItem thisitem = this.Items.Find(m => m.Index == Convert.ToInt32(checkitem.Data["ItemId"]));
                            if (thisitem != null)
                            {
                                thisitem.Checked = box.self.Active;
                                if (ItemCheck != null)
                                {
                                    ItemCheck(sender, new ItemCheckEventArgs(checkitem.Index, box.self.Active ? CheckState.Checked : CheckState.Unchecked, box.self.Active ? CheckState.Unchecked : CheckState.Checked));
                                }
                                if (ItemChecked != null)
                                {
                                    ItemChecked(sender, new ItemCheckedEventArgs(thisitem));
                                }
                            }
                        };
                        hBox.PackStart(checkBox.self, false, true, 5);
                    }
                    if (this.View == View.SmallIcon)
                    {
                        header.Visible = false;
                        flowBox.MinChildrenPerLine = 1;
                        flowBox.MaxChildrenPerLine = 999;
                        if (this.SmallImageList != null)
                        {
                            this.SmallImageList.ImageSize = new Size(16, 16);
                            if (!string.IsNullOrWhiteSpace(item.ImageKey))
                            {
                                Drawing.Image img = this.SmallImageList.GetBitmap(item.ImageKey);
                                if (img != null)
                                    hBox.PackStart(new Gtk.Image(img.Pixbuf), false, false, 0);
                            }
                            else if (item.ImageIndex > -1)
                            {
                                Drawing.Image img = this.SmallImageList.GetBitmap(item.ImageIndex);
                                if (img != null)
                                    hBox.PackStart(new Gtk.Image(img.Pixbuf), false, false, 0);
                            }

                        }
                        Gtk.Label lab = new Gtk.Label();
                        Pango.AttrList attributes = new Pango.AttrList();
                        if (item.ForeColor.HasValue)
                        {
                            Pango.AttrForeground fg = new Pango.AttrForeground(Convert.ToUInt16(item.ForeColor.Value.R * 257), Convert.ToUInt16(item.ForeColor.Value.G * 257), Convert.ToUInt16(item.ForeColor.Value.B * 257));
                            attributes.Insert(fg);
                        }
                        lab.Attributes = attributes;
                        lab.Xpad = 5;
                        lab.MaxWidthChars = 100;
                        lab.Halign = Gtk.Align.Start;
                        lab.Valign = Gtk.Align.Center;
                        lab.Ellipsize = Pango.EllipsizeMode.End;
                        lab.Text = item.Text;
                        hBox.PackStart(lab, false, false, 0);
                    }
                    else if (this.View == View.LargeIcon)
                    {
                        flowBox.MinChildrenPerLine = 1;
                        flowBox.MaxChildrenPerLine = 999;
                        Gtk.Box vBox = new Gtk.Box(Gtk.Orientation.Vertical, 5);
                        if (this.LargeImageList != null)
                        {
                            this.SmallImageList.ImageSize = new Size(100, 100);
                            if (!string.IsNullOrWhiteSpace(item.ImageKey))
                            {
                                Drawing.Image img = this.LargeImageList.GetBitmap(item.ImageKey);
                                if (img != null)
                                {
                                    int width = img.Pixbuf.Width;
                                    int height = img.Pixbuf.Height;
                                    vBox.Add(new Gtk.Image(new Gdk.Pixbuf(img.PixbufData)) { WidthRequest = Math.Min(50, width), HeightRequest = Math.Min(50, height) });
                                }
                            }
                            else if (item.ImageIndex > -1)
                            {
                                Drawing.Image img = this.LargeImageList.GetBitmap(item.ImageIndex);
                                if (img != null)
                                {
                                    int width = img.Pixbuf.Width;
                                    int height = img.Pixbuf.Height;
                                    vBox.Add(new Gtk.Image(img.Pixbuf) { WidthRequest = Math.Min(50, width), HeightRequest = Math.Min(50, height) });
                                }
                            }
                        }
                        Gtk.Label lab = new Gtk.Label();
                        Pango.AttrList attributes = new Pango.AttrList();
                        if (item.ForeColor.HasValue)
                        {
                            Pango.AttrForeground fg = new Pango.AttrForeground(Convert.ToUInt16(item.ForeColor.Value.R * 257), Convert.ToUInt16(item.ForeColor.Value.G * 257), Convert.ToUInt16(item.ForeColor.Value.B * 257));
                            attributes.Insert(fg);

                        }
                        lab.Attributes = attributes;
                        lab.Xpad = 5;
                        lab.MaxWidthChars = 16;
                        lab.Halign = Gtk.Align.Center;
                        lab.Valign = Gtk.Align.Center;
                        lab.Ellipsize = Pango.EllipsizeMode.End;
                        lab.Text = item.Text;
                        vBox.Add(lab);
                        hBox.PackStart(vBox, false, false, 0);
                    }
                    else
                    {
                        header.Visible = false;
                        boxitem.Halign = Gtk.Align.Fill;

                        Gtk.Label lab = new Gtk.Label();
                        Pango.AttrList attributes = new Pango.AttrList();
                        if (item.ForeColor.HasValue)
                        {
                            Pango.AttrForeground fg = new Pango.AttrForeground(Convert.ToUInt16(item.ForeColor.Value.R * 257), Convert.ToUInt16(item.ForeColor.Value.G * 257), Convert.ToUInt16(item.ForeColor.Value.B * 257));
                            attributes.Insert(fg);

                        }
                        lab.Attributes = attributes;
                        lab.Halign = Gtk.Align.Start;
                        lab.Valign = Gtk.Align.Fill;
                        lab.Text = item.Text;
                        hBox.PackStart(lab, false, false, 0);
                    }
                }
                if (IsCacheUpdate == false)
                    self.ShowAll();
            }
        }

        internal void NativeCheckItem(ListViewItem item, bool ischecked)
        {
            if (item._flowBoxChild != null && item._flowBoxChild.IsRealized && item._flowBoxChild.Parent is Gtk.FlowBox flowBox)
            {
                Gtk.Box box = item._flowBoxChild.Child as Gtk.Box;
                if (this.View == View.Details)
                {
                    Gtk.Viewport viewport = box.Children[0] as Gtk.Viewport;
                    Gtk.Layout layout = viewport.Child as Gtk.Layout;
                    foreach (var chk in layout.Children)
                    {
                        if (chk is Gtk.CheckButton chkbutton)
                        {
                            chkbutton.Active = ischecked;
                            break;
                        }
                    }
                }
                else
                {
                    foreach (var chk in box.Children)
                    {
                        if (chk is Gtk.CheckButton chkbutton)
                        {
                            chkbutton.Active = ischecked;
                            break;
                        }
                    }
                }
            }
        }

        internal void NativeGroupAdd(ListViewGroup group, int position)
        {
            if (self.IsRealized)
            {
                if (group == null)
                    return;

                if (position == -1 && Groups.Exists(g => g.SerialGuid == group.SerialGuid) == false)
                {
                    if (AllowColumnReorder)
                    {
                        if (Sorting == SortOrder.Descending)
                        {
                            position = Groups.OrderByDescending(o => o.Header).ToList().FindIndex(g => g.Name == group.Name);
                        }
                        else if (Sorting == SortOrder.Ascending)
                        {
                            position = Groups.OrderBy(o => o.Header).ToList().FindIndex(g => g.Name == group.Name);
                        }
                    }
                }

                Gtk.FlowBox _flow = DefaultGroup.FlowBox;
                Gtk.Box hBox = new Gtk.Box(Gtk.Orientation.Vertical, 0);
                hBox.Valign = Gtk.Align.Start;
                hBox.Halign = Gtk.Align.Fill;
                if (ShowGroups == true && this.View != View.List && this.View != View.Tile)
                {
                    _flow = group.FlowBox;
                    if (_flow.Parent != null)
                        return;

                    Gtk.Box groupbox = new Box(Gtk.Orientation.Horizontal, 0);
                    groupbox.StyleContext.AddClass("GroupTitle");
                    groupbox.MarginStart = 3;
                    var title = new Gtk.Label(group.Header) { Xalign = 0, Halign = Gtk.Align.Start, Valign = Gtk.Align.Center, Ellipsize = Pango.EllipsizeMode.End };
                    groupbox.PackStart(title, false, false, 0);
                    Gtk.Viewport groupline = new Gtk.Viewport();
                    groupline.StyleContext.AddClass("GroupLine");
                    groupline.HeightRequest = 1;
                    groupline.Halign = Gtk.Align.Fill;
                    groupline.Valign = Gtk.Align.Center;
                    groupline.Vexpand = false;
                    groupbox.PackEnd(groupline, true, true, 0);
                    group._groupbox = groupbox;
                    if (Groups.Count == 0)
                    {
                        if (group.Name == ListViewGroup.defaultListViewGroupKey)
                        {
                            groupbox.NoShowAll = true;
                            groupbox.Visible = false;
                            _flow.StyleContext.AddClass("GridBorder");
                        }
                    }
                    else
                    {
                        if (DefaultGroup._groupbox != null && DefaultGroup._groupbox is Gtk.Box defbox)
                        {
                            defbox.NoShowAll = false;
                            defbox.Visible = true;
                            DefaultGroup.FlowBox.StyleContext.RemoveClass("GridBorder");
                        }
                    }

                    hBox.PackStart(groupbox, false, false, 0);
                    if (!string.IsNullOrEmpty(group.Subtitle))
                    {
                        var subtitle = new Gtk.Label(group.Subtitle) { Xalign = 0, Halign = Gtk.Align.Fill, Valign = Gtk.Align.Start, Ellipsize = Pango.EllipsizeMode.End };
                        subtitle.MarginStart = 3;
                        subtitle.MarginBottom = 2;
                        subtitle.StyleContext.AddClass("GroupSubTitle");
                        hBox.PackStart(subtitle, false, false, 0);
                    }
                }
                if (_flow.Parent != null)
                    return;

                _flow.Name = group.Name;
                _flow.ColumnSpacing = 0;
                _flow.RowSpacing = 0;
                _flow.BorderWidth = 0;
                _flow.Homogeneous = false;
                _flow.Orientation = Gtk.Orientation.Horizontal;
                _flow.MaxChildrenPerLine = 500u;
                _flow.MinChildrenPerLine = 0u;
                _flow.Halign = Gtk.Align.Fill;
                _flow.Valign = Gtk.Align.Start;
                _flow.SelectionMode = MultiSelect == false ? Gtk.SelectionMode.Single : Gtk.SelectionMode.Multiple;
                _flow.ActivateOnSingleClick = MultiSelect == false;
                _flow.SortFunc = new Gtk.FlowBoxSortFunc((fbc1, fbc2) =>
                {
                    if (SortingColumnIndex > -1)
                    {
                        if (this.Sorting == SortOrder.Ascending)
                            return fbc2.Data[SortingColumnIndex].ToString().CompareTo(fbc1.Data[SortingColumnIndex].ToString());
                        else if (this.Sorting == SortOrder.Descending)
                            return fbc1.Data[SortingColumnIndex].ToString().CompareTo(fbc2.Data[SortingColumnIndex].ToString());
                        else
                            return fbc2.Index.CompareTo(fbc1.Index);
                    }
                    else
                        return 0;
                });
                _flow.ChildActivated += _flow_ChildActivated;
                _flow.SelectedChildrenChanged += _flow_SelectedChildrenChanged;
                hBox.PackStart(_flow, false, true, 0);

                if (ShowGroups == true && this.View != View.List && this.View != View.Tile)
                {
                    if (!string.IsNullOrEmpty(group.Footer))
                    {
                        var footer = new Gtk.Label(group.Footer) { Xalign = 0, Halign = Gtk.Align.Fill, Valign = Gtk.Align.Start, Ellipsize = Pango.EllipsizeMode.End };
                        footer.MarginStart = 3;
                        footer.MarginTop = 2;
                        footer.StyleContext.AddClass("GroupSubTitle");
                        hBox.PackEnd(footer, false, false, 0);
                    }
                }
                flowBoxContainer.PackStart(hBox, false, true, 0);
                if (position > -1)
                {
                    flowBoxContainer.ReorderChild(hBox, position + 1);
                }
                if (self.IsRealized && Groups.Count > 0)
                {
                    hBox.ShowAll();
                    DefaultGroup?._groupbox?.ShowAll();
                }
            }
        }

        internal void NativeGroupsClear()
        {
            foreach (var group in flowBoxContainer.Children)
            {
                flowBoxContainer.Remove(group);
                group.Dispose();
            }
            _defaultGroup?.FlowBox?.Destroy();
            _defaultGroup = null;
        }

        internal void NativeHeaderAdd(ColumnHeader col)
        {
            if (self.IsRealized)
            {
                LabelBase label = new LabelBase(col.Text) { Xalign = 0, Xpad = 5, WidthRequest = col.Width, MaxWidthChars = 0, Halign = Gtk.Align.Start, Valign = Gtk.Align.End, Ellipsize = Pango.EllipsizeMode.End, Wrap = false, LineWrap = false };
                label.TooltipText = col.Text;
                label.Markup = col.Text;
                label.Data.Add("ColumnIndex", col.Index);
                label.Override.DrawnBackground += Override_DrawnBackground;
                var columbt = new Gtk.Button(label) { MarginStart = 0, WidthRequest = col.Width, HeightRequest = __headerheight, Halign = Gtk.Align.Start, Valign = Gtk.Align.Fill };
                columbt.Name = col.Name;
                columbt.ActionTargetValue = new GLib.Variant(col.Index);
                columbt.Data.Add("ColumnIndex", col.Index);
                columbt.Clicked += Columbt_Clicked;
                header.PackStart(columbt, false, false, 0);

                header.ReorderChild(columbt, col.Index);

                if (this.View == View.Details)
                {
                    headerView.HeightRequest = __headerheight;
                    headerView.ShowAll();
                }
                else
                {
                    headerView.HeightRequest = 1;
                }
            }
        }

        internal void NativeHeaderClear()
        {
            Clear();
            foreach (var col in header.Children)
            {
                header.Remove(col);
                col.Destroy();
            }
            header.Hide();
        }

        internal void NativeHeaderRemove(ColumnHeader column)
        {
            Clear();
            if (column.DisplayIndex < header.Children.Length)
            {
                var headercol = header.Children[column.DisplayIndex];
                header.Remove(headercol);
                headercol.Destroy();
            }
        }

        internal void NativeItemsClear()
        {
            foreach (var group in GetAllGroups())
            {
                foreach (Gtk.FlowBoxChild child in group.FlowBox.Children)
                    group.FlowBox.Remove(child);
            }
        }

        internal void NativeSelectItem(ListViewItem item, bool isselected)
        {
            if (item._flowBoxChild != null && item._flowBoxChild.Parent is Gtk.FlowBox flowBox)
            {
                if (isselected == true)
                    flowBox.SelectChild(item._flowBoxChild);
                else
                    flowBox.UnselectChild(item._flowBoxChild);
            }
        }

        internal void NativeUpdateText(ListViewItem item, string text)
        {
            if (item._flowBoxChild != null && item._flowBoxChild.Parent is Gtk.FlowBox flowBox)
            {
                Gtk.Box box = item._flowBoxChild.Child as Gtk.Box;
                if (this.View == View.Details)
                {
                    Gtk.Viewport viewport = box.Children[0] as Gtk.Viewport;
                    Gtk.Layout layout = viewport.Child as Gtk.Layout;
                    foreach (var lab in layout.Children)
                    {
                        if (lab is Gtk.Label label)
                        {
                            label.Text = text;
                            break;
                        }
                    }
                }
                else if (this.View == View.SmallIcon)
                {
                    foreach (var lab in box.Children)
                    {
                        if (lab is Gtk.Label label)
                        {
                            label.Text = text;
                            break;
                        }
                    }
                }
                else if (this.View == View.LargeIcon)
                {
                    Gtk.Box vbox = box.Children[0] as Gtk.Box;
                    foreach (var lab in vbox.Children)
                    {
                        if (lab is Gtk.Label label)
                        {
                            label.Text = text;
                            break;
                        }
                    }
                }
                else if (this.View == View.List)
                {
                    foreach (var lab in box.Children)
                    {
                        if (lab is Gtk.Label label)
                        {
                            label.Text = text;
                            break;
                        }
                    }
                }
            }
        }
        private void _flow_ChildActivated(object o, Gtk.ChildActivatedArgs args)
        {
            Gtk.FlowBox widget = (Gtk.FlowBox)o;
            ListViewItem item = this.Items.Find(m => m.Index == Convert.ToInt32(args.Child.Data["ItemId"]));
            if (item != null)
            {
                item._selected = args.Child.IsSelected;
                foreach (var group in GetAllGroups())
                {
                    if (group.FlowBox.Equals(widget) == false)
                    {
                        group.FlowBox.UnselectAll();
                    }
                }
                if (ItemActivate != null)
                    ItemActivate(this, args);
                if (ItemSelectionChanged != null)
                    ItemSelectionChanged(this, new ListViewItemSelectionChangedEventArgs(item, item.Index, item.Selected));
            }
        }

        private void _flow_SelectedChildrenChanged(object sender, EventArgs e)
        {
            Gtk.FlowBox widget = (Gtk.FlowBox)sender;
            if (MultiSelect == true)
            {
                if (ItemActivate != null)
                    ItemActivate(this, e);

                List<int> selecteds = new List<int>();
                foreach (var group in GetAllGroups())
                {
                    foreach (FlowBoxChild o in group.FlowBox.SelectedChildren)
                    {
                        if (o.IsSelected)
                        {
                            int id = Convert.ToInt32(o.Data["ItemId"]);
                            selecteds.Add(id);
                        }
                    }
                }
                foreach (ListViewItem item in Items)
                {
                    if (selecteds.Contains(item.Index))
                    {
                        if (item.Selected == false)
                        {
                            item.Selected = true;
                            if (ItemSelectionChanged != null)
                                ItemSelectionChanged(this, new ListViewItemSelectionChangedEventArgs(item, item.Index, item.Selected));
                        }
                    }
                    else if (item.Selected == true)
                    {
                        item.Selected = false;
                        if (ItemSelectionChanged != null)
                            ItemSelectionChanged(this, new ListViewItemSelectionChangedEventArgs(item, item.Index, item.Selected));
                    }
                }
            }

            if (SelectedIndexChanged != null)
                SelectedIndexChanged(this, e);
        }

        private void Columbt_Clicked(object sender, EventArgs e)
        {
            Gtk.Button btn = (Gtk.Button)sender;

            if (this.HeaderStyle == ColumnHeaderStyle.Clickable)
            {
                int actioncolumn = (int)btn.ActionTargetValue;
                if (this.Sorted == true)
                {
                    if (SortingColumnIndex == actioncolumn)
                    {
                        if (Sorting == SortOrder.Ascending)
                            Sorting = SortOrder.Descending;
                        else if (Sorting == SortOrder.Descending)
                            Sorting = SortOrder.None;
                        else
                            Sorting = SortOrder.Ascending;
                    }
                    else
                    {
                        Sorting = SortOrder.Ascending;
                    }
                    this.Sort();

                    if (ColumnReordered != null)
                        ColumnReordered(this, new ColumnReorderedEventArgs(SortingColumnIndex, actioncolumn, Columns[actioncolumn]));

                    SortingColumnIndex = actioncolumn;
                }
                if (ColumnClick != null)
                    ColumnClick(this, new ColumnClickEventArgs((int)btn.ActionTargetValue));
            }
        }

        private void Control_Realized(object sender, EventArgs e)
        {
            if (this.View == View.Details)
            {
                header.NoShowAll = false;
                header.Visible = true;
                header.ShowAll();
                headerView.HeightRequest = __headerheight;
            }
            foreach (ColumnHeader header in this.Columns)
            {
                NativeHeaderAdd(header);
            }
            foreach (ListViewGroup g in Groups)
            {
                NativeGroupAdd(g, -1);
            }
            foreach (ListViewItem item in Items)
            {
                NativeAdd(item, -1);
            }

            MultiSelect = MultiSelect == true;
            self.ShowAll();
        }

        private IEnumerable<ListViewGroup> GetAllGroups()
        {
            if (_defaultGroup == null)
                return Groups;
            else
                return Groups.Union(new ListViewGroup[] { _defaultGroup });
        }

        private void Hadjustment_ValueChanged(object sender, EventArgs e)
        {
            headerView.Hadjustment.Value = scrolledWindow.Hadjustment.Value;
        }

        private void Override_DrawnBackground(object o, DrawnArgs args)
        {
            Cairo.Rectangle rec = args.Cr.ClipExtents();
            if (rec.Width > 10 && SortingColumnIndex > -1)
            {
                LabelBase ws = (LabelBase)o;
                if (ws.Data["ColumnIndex"].ToString() == SortingColumnIndex.ToString())
                {
                    if (Sorting != SortOrder.None)
                    {
                        Cairo.Context ctx = args.Cr;
                        ctx.Save();
                        ctx.ResetClip();
                        ctx.Rectangle(rec.X, rec.Y - 4, rec.Width, rec.Height + 4);
                        ctx.Clip();
                        ctx.Translate((int)rec.Width / 2 - 16, rec.Y - 5);
                        ctx.Rotate(0.5 * Math.PI);
                        Gdk.RGBA color = ws.StyleContext.GetColor(StateFlags.Normal);
                        ctx.SetSourceRGBA(color.Red, color.Green, color.Blue, color.Alpha);
                        //Serif,Impact,Sylfaen,Arial Black,Cambria,Candara,Arial
                        ctx.SelectFontFace("Serif", Cairo.FontSlant.Normal, Cairo.FontWeight.Bold);
                        ctx.SetFontSize(13);
                        if (Sorting == SortOrder.Descending)
                            ctx.ShowText("<");
                        else if (Sorting == SortOrder.Ascending)
                            ctx.ShowText(">");
                        ctx.Stroke();
                        ctx.Restore();
                    }
                }
            }
        }
        /// <summary>
        /// Represents the collection containing the indexes to the checked items in a list view control.
        /// </summary>
        public class CheckedIndexCollection : List<int>
        {

            public CheckedIndexCollection(ListView owner)
            {

            }

        }

        /// <summary>
        /// Represents the collection of checked items in a list view control.
        /// </summary>
        [ListBindable(false)]
        public class CheckedListViewItemCollection : List<ListViewItem>
        {

            public CheckedListViewItemCollection(ListView owner)
            {

            }

            public virtual ListViewItem this[string key]
            {
                get
                {
                    return this.Find(w => w.Name == key);
                }
            }

        }

        /// <summary>
        /// Represents the collection of column headers in a
        /// <see cref="System.Windows.Forms.ListView" /> control.
        /// </summary>
        [ListBindable(false)]
        public class ColumnHeaderCollection : List<ColumnHeader>
        {
            ListView _owner;
            public ColumnHeaderCollection(ListView owner)
            {
                _owner = owner;
            }

            public bool IsReadOnly
            {
                get
                {
                    return false;
                }
            }
            public virtual ColumnHeader this[string key]
            {
                get
                {
                    return base.Find(o => o.Name == key);
                }
            }
            public new virtual int Add(ColumnHeader header)
            {
                header._index = Count;
                if (header.DisplayIndex == 0)
                    header.DisplayIndex = header._index;
                int index = ((IList)this).Add(header);
                _owner.NativeHeaderAdd(header);

                return index;
            }

            public virtual ColumnHeader Add(string text, int width, HorizontalAlignment textAlign)
            {
                return Add("", text, width, textAlign, "");
            }

            public virtual ColumnHeader Add(string text)
            {
                return Add("", text, 60, HorizontalAlignment.Left, "");
            }

            public virtual ColumnHeader Add(string text, int width)
            {
                return Add("", text, width, HorizontalAlignment.Left, "");
            }

            public virtual ColumnHeader Add(string key, string text)
            {
                return Add(key, text, 60, HorizontalAlignment.Left, "");
            }

            public virtual ColumnHeader Add(string key, string text, int width)
            {
                return Add(key, text, width, HorizontalAlignment.Left, "");
            }

            public virtual ColumnHeader Add(string key, string text, int width, HorizontalAlignment textAlign, string imageKey)
            {
                ColumnHeader header = new ColumnHeader();
                header.Name = key;
                header.Text = text;
                header.Width = width;
                header.TextAlign = textAlign;
                header.ImageKey = imageKey;
                header.ImageIndex = -1;
                this.Add(header);

                return header;
            }

            public virtual ColumnHeader Add(string key, string text, int width, HorizontalAlignment textAlign, int imageIndex)
            {
                ColumnHeader header = new ColumnHeader();
                header.Name = key;
                header.Text = text;
                header.Width = width;
                header.TextAlign = textAlign;
                header.ImageIndex = imageIndex;
                this.Add(header);

                return header;
            }

            public virtual void AddRange(ColumnHeader[] values)
            {
                foreach (ColumnHeader value in values)
                {
                    this.Add(value);
                }
            }

            public new void Clear()
            {
                base.Clear();
                _owner.NativeHeaderClear();
            }

            public virtual bool ContainsKey(string key)
            {
                return base.Contains(base.Find(o => o.Name == key));
            }

            public virtual int IndexOfKey(string key)
            {
                return base.FindIndex(o => o.Name == key);
            }

            public new void Insert(int index, ColumnHeader header)
            {
                header._index = index;
                if (header.DisplayIndex == 0)
                    header.DisplayIndex = header._index;

                base.Insert(index, header);
                int idx = 0;
                foreach (var item in this)
                {
                    header._index = idx;
                    if (header.DisplayIndex == 0)
                        header.DisplayIndex = header._index;
                    idx++;
                }
                _owner.NativeHeaderAdd(header);
            }

            public void Insert(int index, string text, int width, HorizontalAlignment textAlign)
            {
                Insert(index, "", text, width, textAlign, null);
            }

            public void Insert(int index, string text)
            {
                Insert(index, "", text, 0, HorizontalAlignment.Center, null);
            }

            public void Insert(int index, string text, int width)
            {
                Insert(index, "", text, width, HorizontalAlignment.Center, null);
            }

            public void Insert(int index, string key, string text)
            {
                Insert(index, key, text, 0, HorizontalAlignment.Center, null);

            }

            public void Insert(int index, string key, string text, int width)
            {
                Insert(index, key, text, width, HorizontalAlignment.Center, null);
            }

            public void Insert(int index, string key, string text, int width, HorizontalAlignment textAlign, string imageKey)
            {
                ColumnHeader header = new ColumnHeader();
                header._index = index;
                header.DisplayIndex = index;
                header.Name = key;
                header.Text = text;
                header.Width = width;
                header.TextAlign = textAlign;
                header.ImageKey = imageKey;
                Insert(index, header);
            }

            public void Insert(int index, string key, string text, int width, HorizontalAlignment textAlign, int imageIndex)
            {
                ColumnHeader header = new ColumnHeader();
                header._index = index;
                header.DisplayIndex = index;
                header.Name = key;
                header.Text = text;
                header.Width = width;
                header.TextAlign = textAlign;
                header.ImageIndex = imageIndex;
                Insert(index, header);
            }

            public virtual void RemoveByKey(string key)
            {
                ColumnHeader column = base.Find(o => o.Name == key);
                base.Remove(column);
                _owner.NativeHeaderRemove(column);
            }
        }

        /// <summary>
        /// Represents the collection of items in a
        /// <see cref="System.Windows.Forms.ListView" /> control or assigned to a
        /// <see cref="System.Windows.Forms.ListViewGroup" />
        /// </summary>
        [ListBindable(false)]
        public class ListViewItemCollection : List<ListViewItem>, IList
        {
            ListView _owner;

            public ListViewItemCollection(ListView owner)
            {
                _owner = owner;
            }
            public virtual ListViewItem this[string key]
            {
                get
                {
                    return base.Find(w => w.Name == key);
                }
            }

            public new void Add(ListViewItem item)
            {
                AddCore(item, -1);
            }

            public virtual ListViewItem Add(string text)
            {
                return Add("", text, -1);
            }

            public virtual ListViewItem Add(string text, int imageIndex)
            {
                return Add("", text, imageIndex);
            }

            public virtual ListViewItem Add(string text, string imageKey)
            {
                return Add("", text, imageKey);
            }

            public virtual ListViewItem Add(string key, string text, string imageKey)
            {
                ListViewItem item = new ListViewItem(text, imageKey);
                item.Name = key;
                item.Text = text;
                AddCore(item, -1);

                return item;
            }

            public virtual ListViewItem Add(string key, string text, int imageIndex)
            {
                ListViewItem item = new ListViewItem(text, imageIndex);
                item.Name = key;
                AddCore(item, -1);

                return item;
            }

            public void AddRange(ListViewItem[] items)
            {
                foreach (ListViewItem item in items)
                {
                    AddCore(item, -1);
                }
            }

            public void AddRange(ListViewItemCollection items)
            {
                foreach (ListViewItem item in items)
                    AddCore(item, -1);
            }

            public new void Clear()
            {
                if (_owner != null)
                    _owner.NativeItemsClear();

                foreach (var item in this)
                {
                    item.SubItems.Clear();
                }
                base.Clear();
            }

            public virtual bool ContainsKey(string key)
            {
                return base.FindIndex(w => w.Name == key) > -1;
            }

            public void CopyTo(Array dest, int index)
            {
                throw new System.NotImplementedException();
            }

            public ListViewItem[] Find(string key, bool searchAllSubItems)
            {
                if (searchAllSubItems)
                    return base.FindAll(w => w.Name == key && w.SubItems.ContainsKey(key)).ToArray();
                else
                    return base.FindAll(w => w.Name == key).ToArray();
            }

            public virtual int IndexOfKey(string key)
            {
                return base.FindIndex(w => w.Name == key);
            }

            public ListViewItem Insert(int index, string text)
            {
                return Insert(index, "", text, -1);
            }

            public ListViewItem Insert(int index, string text, int imageIndex)
            {
                return Insert(index, "", text, imageIndex);
            }

            public ListViewItem Insert(int index, string text, string imageKey)
            {
                return Insert(index, "", text, imageKey);
            }

            public virtual ListViewItem Insert(int index, string key, string text, string imageKey)
            {
                ListViewItem item = new ListViewItem(text, imageKey);
                item.Name = key;
                base.Insert(index, item);

                return item;
            }

            public virtual ListViewItem Insert(int index, string key, string text, int imageIndex)
            {
                ListViewItem item = new ListViewItem(text, imageIndex);
                item.Name = key;
                base.Insert(index, item);

                return item;
            }

            public virtual void RemoveByKey(string key)
            {
                base.Remove(base.Find(w => w.Name == key));
            }
            private void AddCore(ListViewItem item, int position)
            {
                item._listView = _owner;
                item.Index = Count;
                if (item.Group == null)
                    item.Group = _owner.DefaultGroup;

                base.Add(item);
                _owner.NativeAdd(item, position);
            }
        }

        /// <summary>
        /// Represents the collection that contains the indexes to the selected items in a
        /// <see cref="System.Windows.Forms.ListView" /> control.
        /// </summary>
        [ListBindable(false)]
        public class SelectedIndexCollection : List<int>
        {

        }

        /// <summary>
        /// Represents the collection of selected items in a list view control.
        /// </summary>
        [ListBindable(false)]
        public class SelectedListViewItemCollection : List<ListViewItem>
        {

        }
    }
}
