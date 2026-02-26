/*
 * Based on GTK components, it is a cross-platform interface component that is compatible with the native C# control winform interface.
 * Use this component GtkSystem.Windows.Forms instead of Microsoft.WindowsDesktop.App.WindowsForms, compile once, and run across Windows, Linux, and macOS
 * Technical support 438865652@qq.com，https://www.gtkapp.com, https://gitee.com/easywebfactory, https://github.com/easywebfactory438865652@qq.com，https://www.gtkapp.com, https://gitee.com/easywebfactory, https://github.com/easywebfactory
 * author:chenhongjin
 */

using GtkSystem.Windows.Forms.GtkControls.ControlBase;
using System.Collections;
using System.ComponentModel;

namespace System.Windows.Forms
{
    /// <summary>
    /// Displays a
    /// <see cref="System.Windows.Forms.ListBox" />
    /// in which a check box is displayed to the left of each item.
    /// </summary>
    /// <filterpriority>
    /// 2
    /// </filterpriority>
    [DesignerCategory("Component")]
    public partial class CheckedListBox : ContainerControl
    {

        public readonly CheckedListBoxBase self = new CheckedListBoxBase();

        public override object GtkControl => self;

        internal Gtk.FlowBox _flow = new Gtk.FlowBox();

        private ObjectCollection _items;
        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="System.Windows.Forms.CheckedListBox" /> class.
        /// </summary>
        public CheckedListBox() : base()
        {
            _items = new ObjectCollection(this);
            _flow.Orientation = Gtk.Orientation.Horizontal;
            _flow.MinChildrenPerLine = 1;
            _flow.MaxChildrenPerLine = 99999;
            _flow.Halign = Gtk.Align.Fill;
            _flow.Valign = Gtk.Align.Fill;
            _flow.ChildActivated += Control_ChildActivated;
            self.AutoScroll = true;
            self.Add(_flow);

            this.BorderStyle = BorderStyle.Fixed3D;
        }
        private void Control_ChildActivated(object o, Gtk.ChildActivatedArgs args)
        {
            int rowIndex = args.Child.Index;
            if (this.CheckOnClick == true)
                args.Child.Child.SetProperty("active", new GLib.Value(true));

            if (SelectedIndexChanged != null)
                SelectedIndexChanged(this, args);
            if (SelectedValueChanged != null)
                SelectedValueChanged(this, args);
            if (SelectedItemChanged != null)
                SelectedItemChanged(this, args);
        }


        public int ColumnWidth { get; set; }

        public bool MultiColumn { get { return _flow.Orientation == Gtk.Orientation.Vertical; } set { if (value == false) { _flow.Orientation = Gtk.Orientation.Horizontal; } else { _flow.Orientation = Gtk.Orientation.Vertical; } } }

        public bool HorizontalScrollbar { get; set; }

        public bool FormattingEnabled { get; set; }
        /// <summary>
        /// Gets the height of the item area.
        /// </summary>
        /// <returns>The height, in pixels, of the item area.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public int ItemHeight { get; set; }
        /// <summary>
        /// Gets or sets a value specifying the selection mode.
        /// </summary>
        /// <returns>Either the One or None value of
        /// <see cref="System.Windows.Forms.SelectionMode" />
        /// </returns>
        /// <exception cref="System.ArgumentException">An attempt was made to assign a value that is not a
        /// <see cref="System.Windows.Forms.SelectionMode" /> value of One or None.
        /// </exception>
        /// <exception cref="System.ComponentModel.InvalidEnumArgumentException">An attempt was made to assign the MultiExtended value of
        /// <see cref="System.Windows.Forms.SelectionMode" /> to the control.
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public SelectionMode SelectionMode { get; set; }
        public void ClearSelected()
        {
            foreach (Gtk.FlowBoxChild wi in _flow.Children)
            {
                Gtk.Widget box = ((Gtk.Box)wi.Child).Children[0];
                if (box is Gtk.CheckButton check)
                {
                    check.Active = false;
                }
            }
            ;
            _flow.UnselectAll();
        }
        /// <summary>
        /// Gets or sets a value indicating whether the check box should be toggled when an item is selected.
        /// </summary>
        /// <returns>true if the check mark is applied immediately; otherwise, false. The default is false.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public bool CheckOnClick { get; set; }
        /// <summary>
        /// Gets the collection of items in this
        /// <see cref="System.Windows.Forms.CheckedListBox" />
        /// </summary>
        /// <returns>The
        /// <see cref="System.Windows.Forms.CheckedListBox.ObjectCollection" /> collection representing the items in the
        /// <see cref="System.Windows.Forms.CheckedListBox" />
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public ObjectCollection Items
        {
            get
            {
                return _items;
            }
        }
        public SelectedItemCollection SelectedItems
        {
            get
            {
                SelectedItemCollection items = new SelectedItemCollection();
                foreach (Gtk.FlowBoxChild child in _flow.Children)
                {
                    if (child.IsSelected == true)
                        items.Add(child.Child.GetProperty("label").Val);
                }
                return items;
            }
        }
        /// <summary>
        /// Collection of checked items in this
        /// <see cref="System.Windows.Forms.CheckedListBox" />
        /// </summary>
        /// <returns>The
        /// <see cref="System.Windows.Forms.CheckedListBox.CheckedItemCollection" /> collection for the
        /// <see cref="System.Windows.Forms.CheckedListBox" />
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public CheckedItemCollection CheckedItems
        {
            get
            {
                CheckedItemCollection items = new CheckedItemCollection();
                foreach (Gtk.FlowBoxChild child in _flow.Children)
                {
                    if ((bool)child.Child.GetProperty("active") == true)
                        items.Add(child.Child.GetProperty("label").Val);
                }
                return items;
            }
        }
        /// <summary>
        /// Collection of checked indexes in this
        /// <see cref="System.Windows.Forms.CheckedListBox" />
        /// </summary>
        /// <returns>The
        /// <see cref="System.Windows.Forms.CheckedListBox.CheckedIndexCollection" /> collection for the
        /// <see cref="System.Windows.Forms.CheckedListBox" />
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public CheckedIndexCollection CheckedIndices
        {
            get
            {
                CheckedIndexCollection items = new CheckedIndexCollection();
                foreach (Gtk.FlowBoxChild child in _flow.Children)
                {
                    if ((bool)child.Child.GetProperty("active") == true)
                        items.Add(child.Index);
                }
                return items;
            }
        }


        public event ItemCheckEventHandler ItemCheck;

        public event EventHandler SelectedIndexChanged;

        public event EventHandler SelectedValueChanged;

        public event EventHandler SelectedItemChanged;
        internal void NativeAdd(CheckBox checkbox, bool isChecked, int position)
        {
            checkbox.self.Toggled += (object sender, EventArgs e) =>
            {
                Gtk.CheckButton box = (Gtk.CheckButton)sender;
                Gtk.FlowBoxChild item = box.Parent as Gtk.FlowBoxChild;
                if (this.ItemCheck != null)
                {
                    CheckBox checkBox = _items.GetCheckedListBoxItem(item.Index);
                    this.ItemCheck(checkBox, new ItemCheckEventArgs(item.Index, box.Active == true ? CheckState.Checked : CheckState.Unchecked, box.Active == false ? CheckState.Checked : CheckState.Unchecked));
                }
            };
            Gtk.FlowBoxChild boxitem = new Gtk.FlowBoxChild();
            boxitem.HeightRequest = this.ItemHeight;
            if (this.MultiColumn && this.ColumnWidth > 0)
            {
                boxitem.WidthRequest = this.ColumnWidth;
            }
            boxitem.Valign = Gtk.Align.Start;
            boxitem.Halign = Gtk.Align.Start;
            boxitem.TooltipText = checkbox.Text;
            boxitem.Add(checkbox.self);
            if (position < 0)
            {
                this._flow.Add(boxitem);
            }
            else
            {
                this._flow.Insert(boxitem, position);
            }
        }
        internal void NativeRemove(int index)
        {
            if (_flow.Children.Length > index)
                _flow.Remove(_flow.Children[index]);
        }
        internal void NativeClear()
        {
            foreach (var item in _flow.Children)
                _flow.Remove(item);
        }
        /// <summary>
        /// Sets
        /// <see cref="System.Windows.Forms.CheckState" /> for the item at the specified index to Checked.
        /// </summary>
        /// <param name="index">The index of the item to set the check state for.
        /// </param>
        /// <param name="value">true to set the item as checked; otherwise, false.
        /// </param>
        /// <exception cref="System.ArgumentException">The index specified is less than zero.-or- The index is greater than the count of items in the list.
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public void SetItemChecked(int index, bool value)
        {
            CheckBox item = _items.GetCheckedListBoxItem(index);
            if (item != null)
            {
                item.Checked = value;
            }
        }
        /// <summary>
        /// Sets the check state of the item at the specified index.
        /// </summary>
        /// <param name="index">The index of the item to set the state for.
        /// </param>
        /// <param name="value">One of the
        /// <see cref="System.Windows.Forms.CheckState" /> values.
        /// </param>
        /// <exception cref="System.ArgumentOutOfRangeException">The
        /// <paramref name="index" /> specified is less than zero.-or- The
        /// <paramref name="index" /> is greater than or equal to the count of items in the list.
        /// </exception>
        /// <exception cref="System.ComponentModel.InvalidEnumArgumentException">The
        /// <paramref name="value" /> is not one of the
        /// <see cref="System.Windows.Forms.CheckState" /> values.
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public void SetItemCheckState(int index, System.Windows.Forms.CheckState value)
        {
            CheckBox item = _items.GetCheckedListBoxItem(index);
            if (item != null)
            {
                item.Checked = value == System.Windows.Forms.CheckState.Checked;
            }
        }
        /// <summary>
        /// Represents the collection of items in a
        /// <see cref="System.Windows.Forms.CheckedListBox" />
        /// </summary>
        public class ObjectCollection : ArrayList
        {

            private CheckedListBox _owner;
            public ObjectCollection(CheckedListBox owner)
            {
                _owner = owner;
            }
            public override object this[int index]
            {
                get => ((CheckBox)base[index]).Text;
                set => ((CheckBox)base[index]).Text = value?.ToString();
            }
            public CheckBox GetCheckedListBoxItem(int index)
            {
                if (index < Count)
                    return base[index] as CheckBox;
                else
                    return null;
            }

            public override int Add(object value)
            {
                return AddCore(value, false, -1);
            }
            public int Add(object item, bool isChecked)
            {
                return AddCore(item, isChecked, -1);
            }

            public int AddCore(object item, bool isChecked, int position)
            {
                CheckBox listBoxItem = new CheckBox();
                listBoxItem.Text = item.ToString();
                listBoxItem.Checked = isChecked;
                _owner.NativeAdd(listBoxItem, isChecked, position);
                if (position < 0)
                {
                    return base.Add(listBoxItem);
                }
                else
                {
                    base.Insert(position, listBoxItem);
                    return position;
                }
            }

            public override void AddRange(ICollection c)
            {
                foreach (object o in c)
                    AddCore(o, false, -1);
            }
            public override void RemoveAt(int index)
            {
                _owner.NativeRemove(index);
                base.RemoveAt(index);
            }
            public override void Remove(object obj)
            {
                int index = base.IndexOf(obj);
                if (index > -1)
                {
                    RemoveAt(index);
                }
            }
            public override void Clear()
            {
                _owner.NativeClear();
                base.Clear();
            }
        }
        /// <summary>
        /// Encapsulates the collection of indexes of checked items (including items in an indeterminate state) in a
        /// <see cref="System.Windows.Forms.CheckedListBox" />
        /// </summary>
        public class CheckedIndexCollection : List<int>
        {
        }
        /// <summary>
        /// Encapsulates the collection of checked items, including items in an indeterminate state, in a
        /// <see cref="System.Windows.Forms.CheckedListBox" /> control.
        /// </summary>
        public class CheckedItemCollection : List<object>
        {

        }
        public class SelectedItemCollection : List<object>
        {

        }
    }
}
