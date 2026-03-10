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
    /// Displays a <see cref="T:System.Windows.Forms.ListBox"/> in which a check box is displayed to the left of each item.
    /// This GTK-based implementation provides behavior similar to the WinForms <see cref="CheckedListBox"/>.
    /// </summary>
    /// <filterpriority>2</filterpriority>
    [DesignerCategory("Component")]
    public partial class CheckedListBox : ContainerControl
    {

        /// <summary>
        /// The underlying GTK-based container that implements scrolling and layout.
        /// </summary>
        public readonly CheckedListBoxBase self = new CheckedListBoxBase();

        /// <summary>
        /// Internal GTK FlowBox used to host each checkable item.
        /// </summary>
        internal Gtk.FlowBox _flow = new Gtk.FlowBox();

        private ObjectCollection _items;

        /// <summary>
        /// Occurs when an item's check state is changed.
        /// </summary>
        public event ItemCheckEventHandler ItemCheck;

        /// <summary>
        /// Occurs when the selected index changes.
        /// </summary>
        public event EventHandler SelectedIndexChanged;

        /// <summary>
        /// Occurs when the selected item changes.
        /// </summary>
        public event EventHandler SelectedItemChanged;

        /// <summary>
        /// Occurs when the selected value changes.
        /// </summary>
        public event EventHandler SelectedValueChanged;

        /// <summary>
        /// Initializes a new instance of the <see cref="CheckedListBox"/> class.
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

        /// <summary>
        /// Gets a collection containing the zero-based indices of all checked items.
        /// </summary>
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

        /// <summary>
        /// Gets a collection containing the objects corresponding to checked items.
        /// </summary>
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
        /// Gets or sets a value indicating whether items should be checked when clicked.
        /// </summary>
        public bool CheckOnClick { get; set; }

        /// <summary>
        /// Gets or sets the column width used when <see cref="MultiColumn"/> is enabled.
        /// </summary>
        public int ColumnWidth { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether formatting is enabled for the display of items.
        /// </summary>
        public bool FormattingEnabled { get; set; }

        /// <summary>
        /// Gets the underlying GTK container exposed as a generic control object.
        /// </summary>
        public override object GtkControl => self;

        /// <summary>
        /// Gets or sets a value indicating whether a horizontal scrollbar is shown.
        /// </summary>
        public bool HorizontalScrollbar { get; set; }

        /// <summary>
        /// Gets or sets the height, in pixels, of each item.
        /// </summary>
        public int ItemHeight { get; set; }

        /// <summary>
        /// Gets the collection of items contained in the control.
        /// </summary>
        public ObjectCollection Items
        {
            get
            {
                return _items;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the control displays items in multiple columns.
        /// </summary>
        public bool MultiColumn { get { return _flow.Orientation == Gtk.Orientation.Vertical; } set { if (value == false) { _flow.Orientation = Gtk.Orientation.Horizontal; } else { _flow.Orientation = Gtk.Orientation.Vertical; } } }

        /// <summary>
        /// Gets a collection containing the currently selected items.
        /// </summary>
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
        /// Gets or sets the selection mode that determines how many items can be selected.
        /// </summary>
        public SelectionMode SelectionMode { get; set; }

        /// <summary>
        /// Clears the selection of all items.
        /// </summary>
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
        /// Sets the checked state of the item at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the item.</param>
        /// <param name="value">True to check the item; otherwise false.</param>
        public void SetItemChecked(int index, bool value)
        {
            CheckBox item = _items.GetCheckedListBoxItem(index);
            if (item != null)
            {
                item.Checked = value;
            }
        }

        /// <summary>
        /// Sets the check state for the item at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the item.</param>
        /// <param name="value">The check state to set.</param>
        public void SetItemCheckState(int index, System.Windows.Forms.CheckState value)
        {
            CheckBox item = _items.GetCheckedListBoxItem(index);
            if (item != null)
            {
                item.Checked = value == System.Windows.Forms.CheckState.Checked;
            }
        }

        /// <summary>
        /// Adds a GTK-native item (checkbox) to the visual list at the given position.
        /// </summary>
        /// <param name="checkbox">The checkbox control to add.</param>
        /// <param name="isChecked">Initial checked state.</param>
        /// <param name="position">Position index to insert; if negative the item is appended.</param>
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

        /// <summary>
        /// Removes all native items from the control.
        /// </summary>
        internal void NativeClear()
        {
            foreach (var item in _flow.Children)
                _flow.Remove(item);
        }

        /// <summary>
        /// Removes the native item at the specified index.
        /// </summary>
        /// <param name="index">Index of the item to remove.</param>
        internal void NativeRemove(int index)
        {
            if (_flow.Children.Length > index)
                _flow.Remove(_flow.Children[index]);
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

        /// <summary>
        /// Simple collection of checked indices.
        /// </summary>
        public class CheckedIndexCollection : List<int>
        {
        }

        /// <summary>
        /// Simple collection of checked item values.
        /// </summary>
        public class CheckedItemCollection : List<object>
        {

        }

        /// <summary>
        /// Collection that stores the items shown in the <see cref="CheckedListBox"/>.
        /// Items are represented internally as <see cref="CheckBox"/> instances.
        /// </summary>
        public class ObjectCollection : ArrayList
        {

            private CheckedListBox _owner;

            /// <summary>
            /// Initializes a new instance of the <see cref="ObjectCollection"/> class.
            /// </summary>
            /// <param name="owner">The owning CheckedListBox.</param>
            public ObjectCollection(CheckedListBox owner)
            {
                _owner = owner;
            }

            /// <summary>
            /// Gets or sets the text of the item at the specified index.
            /// </summary>
            public override object this[int index]
            {
                get => ((CheckBox)base[index]).Text;
                set => ((CheckBox)base[index]).Text = value?.ToString();
            }

            /// <summary>
            /// Adds an item to the collection and returns the index at which it was added.
            /// </summary>
            /// <param name="value">Item value to add.</param>
            /// <returns>The index of the added item.</returns>
            public override int Add(object value)
            {
                return AddCore(value, false, -1);
            }

            /// <summary>
            /// Adds an item and optionally sets its checked state.
            /// </summary>
            /// <param name="item">Item to add.</param>
            /// <param name="isChecked">Initial checked state.</param>
            /// <returns>The index of the added item.</returns>
            public int Add(object item, bool isChecked)
            {
                return AddCore(item, isChecked, -1);
            }

            /// <summary>
            /// Core add implementation used by other add overloads.
            /// </summary>
            /// <param name="item">Item to add.</param>
            /// <param name="isChecked">Initial checked state.</param>
            /// <param name="position">Insert position; negative to append.</param>
            /// <returns>Index of the item inserted.</returns>
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

            /// <summary>
            /// Adds a range of items to the collection.
            /// </summary>
            /// <param name="c">Collection of items to add.</param>
            public override void AddRange(ICollection c)
            {
                foreach (object o in c)
                    AddCore(o, false, -1);
            }

            /// <summary>
            /// Clears all items from the collection and the native control.
            /// </summary>
            public override void Clear()
            {
                _owner.NativeClear();
                base.Clear();
            }

            /// <summary>
            /// Retrieves the <see cref="CheckBox"/> instance for the item at the specified index.
            /// </summary>
            /// <param name="index">Index of the item.</param>
            /// <returns>The CheckBox instance or null if out of range.</returns>
            public CheckBox GetCheckedListBoxItem(int index)
            {
                if (index < Count)
                    return base[index] as CheckBox;
                else
                    return null;
            }

            /// <summary>
            /// Removes the specified object from the collection if present.
            /// </summary>
            /// <param name="obj">Object to remove.</param>
            public override void Remove(object obj)
            {
                int index = base.IndexOf(obj);
                if (index > -1)
                {
                    RemoveAt(index);
                }
            }

            /// <summary>
            /// Removes the item at the specified index from both the collection and the native control.
            /// </summary>
            /// <param name="index">Index to remove.</param>
            public override void RemoveAt(int index)
            {
                _owner.NativeRemove(index);
                base.RemoveAt(index);
            }

        }

        /// <summary>
        /// Simple collection type used to return selected item values.
        /// </summary>
        public class SelectedItemCollection : List<object>
        {

        }
    }
}
