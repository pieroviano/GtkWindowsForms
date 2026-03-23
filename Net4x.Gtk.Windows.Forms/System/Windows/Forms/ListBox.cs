/*
 * Based on GTK components, it is a cross-platform interface component that is compatible with the native C# control winform interface.
 * Use this component GtkSystem.Windows.Forms instead of Microsoft.WindowsDesktop.App.WindowsForms, compile once, and run across Windows, Linux, and macOS
 * Technical support 438865652@qq.com，https://www.gtkapp.com, https://gitee.com/easywebfactory, https://github.com/easywebfactory438865652@qq.com，https://www.gtkapp.com, https://gitee.com/easywebfactory, https://github.com/easywebfactory
 * author:chenhongjin
 */
using System.Collections;
using System.ComponentModel;
using System.Data;
using GLib;
using Gtk;
using GtkSystem.Windows.Forms.GtkControls.ControlBase;

namespace System.Windows.Forms;

/// <summary>
/// Class ListBox. Implements the
/// <see cref="T:System.Windows.Forms.ListControl" />
/// </summary>
/// <seealso cref="T:System.Windows.Forms.ListControl" />
[DesignerCategory("Component")]
[DefaultEvent("SelectedIndexChanged")]
[DefaultProperty("Items")]
[DefaultBindingProperty("SelectedValue")]
public partial class ListBox : ListControl
{
    /// <summary>
    /// The selection mode
    /// </summary>
    public SelectionMode _SelectionMode;

    /// <summary>
    /// The default item height
    /// </summary>
    public const int DefaultItemHeight = 13;

    /// <summary>
    /// The no matches
    /// </summary>
    public const int NoMatches = -1;

    /// <summary>
    /// The self
    /// </summary>
    public readonly ListBoxBase self = new ListBoxBase();

    internal bool IsUpdateing = false;

    private readonly ControlBindingsCollection _collect;

    private object _DataSource;

    private readonly ObjectCollection _items;

    private bool _sorted;

    private int _topIndex;

    /// <summary>
    /// Occurs when the selected index changes. Handlers are stored in the internal
    /// <see cref="T:System.Windows.Forms.ListControl.Events" /> list.
    /// </summary>
    public override event EventHandler SelectedIndexChanged
    {
        add { _selectedIndexChanged += value; }
        remove { _selectedIndexChanged -= value; }
    }

    /// <summary>
    /// Occurs when the selected item changes. Handlers are stored in the internal
    /// <see cref="T:System.Windows.Forms.ListControl.Events" /> list.
    /// </summary>
    public override event EventHandler SelectedItemChanged
    {
        add { _selectedItemChanged += value; }
        remove { _selectedItemChanged -= value; }
    }

    /// <summary>
    /// Occurs when the selected value changes. Handlers are stored in the internal
    /// <see cref="T:System.Windows.Forms.ListControl.Events" /> list.
    /// </summary>
    public override event EventHandler SelectedValueChanged
    {
        add { _selectedValueChanged += value; }
        remove { _selectedValueChanged -= value; }
    }

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.ListBox" /> class.
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
    /// Gets or sets the width of the column.
    /// </summary>
    /// <value>
    /// The width of the column.
    /// </value>
    [Localizable(true)]
    [DefaultValue(0)]
    public int ColumnWidth
    {
        get;
        set;
    }

    /// <summary>
    /// Gets the custom tab offsets.
    /// </summary>
    /// <value>
    /// The custom tab offsets.
    /// </value>
    public IntegerCollection CustomTabOffsets
    {
        get;
    }

    /// <summary>
    /// Gets the data bindings.
    /// </summary>
    /// <value>
    /// The data bindings.
    /// </value>

    public override ControlBindingsCollection DataBindings { get => _collect; }

    /// <summary>
    /// Gets or sets the data source for the control. The data source can be any object that implements IList or IListSource.
    /// </summary>
    /// <value>
    /// The data source.
    /// </value>
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
    /// Gets or sets the draw mode.
    /// </summary>
    /// <value>
    /// The draw mode.
    /// </value>
    /// <exception cref="T:System.NotImplementedException">
    /// </exception>
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

    /// <summary>
    /// Gets the GTK control.
    /// </summary>
    /// <value>
    /// The GTK control.
    /// </value>
    public override object GtkControl => self;

    /// <summary>
    /// Gets or sets the horizontal extent.
    /// </summary>
    /// <value>
    /// The horizontal extent.
    /// </value>
    [DefaultValue(0)]
    [Localizable(true)]
    public int HorizontalExtent
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets a value indicating whether horizontal scrollbar.
    /// </summary>
    /// <value>
    /// <c>
    /// true
    /// </c> if horizontal scrollbar; otherwise,
    /// <c>
    /// false
    /// </c>.
    /// </value>
    [DefaultValue(false)]
    [Localizable(true)]
    public bool HorizontalScrollbar
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets a value indicating whether integral height.
    /// </summary>
    /// <value>
    /// <c>
    /// true
    /// </c> if integral height; otherwise,
    /// <c>
    /// false
    /// </c>.
    /// </value>
    [DefaultValue(true)]
    [Localizable(true)]
    public bool IntegralHeight
    {
        get;
        set;
    }

    /// <summary>
    /// Gets the i self.
    /// </summary>
    /// <value>
    /// The i self.
    /// </value>
    public override IControlGtk ISelf { get => self; }

    /// <summary>
    /// Gets or sets the height of the item.
    /// </summary>
    /// <value>
    /// The height of the item.
    /// </value>
    [Localizable(true)]
    public virtual int ItemHeight
    {
        get;
        set;
    }

    /// <summary>
    /// Gets the items.
    /// </summary>
    /// <value>
    /// The items.
    /// </value>
    [Localizable(true)]
    public ObjectCollection Items
    {
        get => _items;
    }

    /// <summary>
    /// Gets or sets a value indicating whether multi column.
    /// </summary>
    /// <value>
    /// <c>
    /// true
    /// </c> if multi column; otherwise,
    /// <c>
    /// false
    /// </c>.
    /// </value>
    [DefaultValue(false)]
    public bool MultiColumn
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the padding.
    /// </summary>
    /// <value>
    /// The padding.
    /// </value>
    public new Padding Padding
    {
        get;
        set;
    }

    /// <summary>
    /// Gets the height of the preferred.
    /// </summary>
    /// <value>
    /// The height of the preferred.
    /// </value>
    [Browsable(false)]
    public int PreferredHeight
    {
        get;
    }

    /// <summary>
    /// Gets or sets a value indicating whether scroll always visible.
    /// </summary>
    /// <value>
    /// <c>
    /// true
    /// </c> if scroll always visible; otherwise,
    /// <c>
    /// false
    /// </c>.
    /// </value>
    [DefaultValue(false)]
    [Localizable(true)]
    public bool ScrollAlwaysVisible
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the index of the currently selected item.
    /// Derived classes must implement selection indexing.
    /// </summary>
    /// <value>
    /// The index of the selected.
    /// </value>
    public override int SelectedIndex
    {
        get => self.ListBox.SelectedRow == null ? -1 : self.ListBox.SelectedRow.Index;
        set => SelectedItems.SetSelected(value, true);
    }

    /// <summary>
    /// Gets the selected indices.
    /// </summary>
    /// <value>
    /// The selected indices.
    /// </value>
    [Browsable(false)]
    public SelectedIndexCollection SelectedIndices
    {
        get
        {
            var indexs = new SelectedIndexCollection(this);

            return indexs;
        }
    }

    /// <summary>
    /// Gets or sets the selected item.
    /// </summary>
    /// <value>
    /// The selected item.
    /// </value>
    [Browsable(false)]
    [Bindable(true)]
    public object SelectedItem
    {
        get
        {
            var index = SelectedIndex;

            return index == -1 ? null : _items[SelectedIndex];
        }
        set
        {
            ClearSelected();
            var index = _items.IndexOf(value);
            SelectedItems.SetSelected(index, true);
        }
    }

    /// <summary>
    /// Gets the selected items.
    /// </summary>
    /// <value>
    /// The selected items.
    /// </value>
    [Browsable(false)]
    public SelectedObjectCollection SelectedItems
    {
        get
        {
            var indexs = new SelectedObjectCollection(this);

            return indexs;
        }
    }

    /// <summary>
    /// Gets or sets the value of the selected item, as determined by the
    /// <see cref="!:ValueMember" />.
    /// </summary>
    /// <value>
    /// The selected value.
    /// </value>
    [DefaultValue(null)]
    [Browsable(false)]
    public override object SelectedValue
    {
        get
        {
            var index = SelectedIndex;

            return index == -1 ? null : _items[SelectedIndex];
        }
        set
        {
            ClearSelected();
            var index = _items.IndexOf(value);
            SelectedItems.SetSelected(index, true);
        }
    }

    /// <summary>
    /// Gets or sets the selection mode.
    /// </summary>
    /// <value>
    /// The selection mode.
    /// </value>
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
    /// Gets or sets a value indicating whether this
    /// <see cref="T:System.Windows.Forms.ListBox" /> is sorted.
    /// </summary>
    /// <value>
    /// <c>
    /// true
    /// </c> if sorted; otherwise,
    /// <c>
    /// false
    /// </c>.
    /// </value>
    [DefaultValue(false)]
    public bool Sorted
    {
        get => _sorted;
        set => _sorted = value;
    }

    /// <summary>
    /// Gets or sets the text.
    /// </summary>
    /// <value>
    /// The text.
    /// </value>
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
    /// Gets or sets the index of the top.
    /// </summary>
    /// <value>
    /// The index of the top.
    /// </value>
    public int TopIndex
    {
        get => _topIndex;
        set
        {
            _topIndex = value;
            GLib.Timeout.Add(100, new TimeoutHandler(() =>
            {
                var rowheight = ItemHeight;
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
    /// Gets or sets a value indicating whether use custom tab offsets.
    /// </summary>
    /// <value>
    /// <c>
    /// true
    /// </c> if use custom tab offsets; otherwise,
    /// <c>
    /// false
    /// </c>.
    /// </value>
    [DefaultValue(false)]
    [Browsable(false)]
    public bool UseCustomTabOffsets
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets a value indicating whether use tab stops.
    /// </summary>
    /// <value>
    /// <c>
    /// true
    /// </c> if use tab stops; otherwise,
    /// <c>
    /// false
    /// </c>.
    /// </value>
    [DefaultValue(true)]
    public bool UseTabStops
    {
        get;
        set;
    }

    internal bool ShowCheckBox
    {
        get;
        set;
    }

    internal bool ShowImage
    {
        get;
        set;
    }

    private EventHandler _selectedIndexChanged
    {
        get { return ((EventHandler)Events["SelectedIndexChanged"]); }
        set { Events["SelectedIndexChanged"] = value; }
    }

    private EventHandler _selectedItemChanged
    {
        get { return ((EventHandler)Events["SelectedItemChanged"]); }
        set { Events["SelectedItemChanged"] = value; }
    }

    private EventHandler _selectedValueChanged
    {
        get { return ((EventHandler)Events["SelectedValueChanged"]); }
        set { Events["SelectedValueChanged"] = value; }
    }

    /// <summary>
    /// Begins the update.
    /// </summary>
    public void BeginUpdate()
    {
        IsUpdateing = true;
    }

    /// <summary>
    /// Clears the selected.
    /// </summary>
    public void ClearSelected()
    {
        self.ListBox.UnselectAll();
    }

    /// <summary>
    /// Ends the update.
    /// </summary>
    public void EndUpdate()
    {
        IsUpdateing = false;
        self.ListBox.ShowAll();
    }

    /// <summary>
    /// Finds the string.
    /// </summary>
    /// <param name="s">
    /// The s.
    /// </param>
    /// <returns>
    /// System.Int32.
    /// </returns>
    /// <exception cref="T:System.NotImplementedException">
    /// </exception>
    public int FindString(string s)
    {
        throw new System.NotImplementedException();
    }

    /// <summary>
    /// Finds the string.
    /// </summary>
    /// <param name="s">
    /// The s.
    /// </param>
    /// <param name="startIndex">
    /// The start index.
    /// </param>
    /// <returns>
    /// System.Int32.
    /// </returns>
    /// <exception cref="T:System.NotImplementedException">
    /// </exception>
    public int FindString(string s, int startIndex)
    {
        throw new System.NotImplementedException();
    }

    /// <summary>
    /// Finds the string exact.
    /// </summary>
    /// <param name="s">
    /// The s.
    /// </param>
    /// <returns>
    /// System.Int32.
    /// </returns>
    /// <exception cref="T:System.NotImplementedException">
    /// </exception>
    public int FindStringExact(string s)
    {
        throw new System.NotImplementedException();
    }

    /// <summary>
    /// Finds the string exact.
    /// </summary>
    /// <param name="s">
    /// The s.
    /// </param>
    /// <param name="startIndex">
    /// The start index.
    /// </param>
    /// <returns>
    /// System.Int32.
    /// </returns>
    /// <exception cref="T:System.NotImplementedException">
    /// </exception>
    public int FindStringExact(string s, int startIndex)
    {
        throw new System.NotImplementedException();
    }

    /// <summary>
    /// Gets the height of the item.
    /// </summary>
    /// <param name="index">
    /// The index.
    /// </param>
    /// <returns>
    /// System.Int32.
    /// </returns>
    /// <exception cref="T:System.NotImplementedException">
    /// </exception>
    public int GetItemHeight(int index)
    {
        throw new System.NotImplementedException();
    }

    /// <summary>
    /// Gets the item rectangle.
    /// </summary>
    /// <param name="index">
    /// The index.
    /// </param>
    /// <returns>
    /// Drawing.Rectangle.
    /// </returns>
    /// <exception cref="T:System.NotImplementedException">
    /// </exception>
    public Drawing.Rectangle GetItemRectangle(int index)
    {
        throw new System.NotImplementedException();
    }

    /// <summary>
    /// Returns the textual representation for the given item. By default this calls
    /// <see cref="T:System.Object.ToString" /> on the item.
    /// Override to provide custom behavior.
    /// </summary>
    /// <param name="item">
    /// Item to obtain text for.
    /// </param>
    /// <returns>
    /// Display text for the item.
    /// </returns>
    public override string GetItemText(object item)
    {
        if (item is ItemArray.Entry entry)
        {
            return entry.Item?.ToString();
        }

        return item?.ToString();
    }

    /// <summary>
    /// Gets the selected.
    /// </summary>
    /// <param name="index">
    /// The index.
    /// </param>
    /// <returns>
    /// <c>
    /// true
    /// </c> if XXXX,
    /// <c>
    /// false
    /// </c> otherwise.
    /// </returns>
    public bool GetSelected(int index)
    {
        return self.ListBox.GetRowAtIndex(index).IsSelected;
    }

    /// <summary>
    /// Indexes from point.
    /// </summary>
    /// <param name="p">
    /// The p.
    /// </param>
    /// <returns>
    /// System.Int32.
    /// </returns>
    /// <exception cref="T:System.NotImplementedException">
    /// </exception>
    public int IndexFromPoint(Drawing.Point p)
    {
        throw new System.NotImplementedException();
    }

    /// <summary>
    /// Indexes from point.
    /// </summary>
    /// <param name="x">
    /// The x.
    /// </param>
    /// <param name="y">
    /// The y.
    /// </param>
    /// <returns>
    /// System.Int32.
    /// </returns>
    /// <exception cref="T:System.NotImplementedException">
    /// </exception>
    public int IndexFromPoint(int x, int y)
    {
        throw new System.NotImplementedException();
    }

    /// <summary>
    /// Causes the control to be redrawn.
    /// </summary>
    public override void Refresh()
    {
        self.ListBox.ShowAll();
    }

    /// <summary>
    /// Resets the BackColor property to its default value.
    /// </summary>
    public override void ResetBackColor()
    {

    }

    /// <summary>
    /// Resets the ForeColor property to its default value.
    /// </summary>
    public override void ResetForeColor()
    {

    }

    /// <summary>
    /// Sets the selected.
    /// </summary>
    /// <param name="index">
    /// The index.
    /// </param>
    /// <param name="value">
    /// if set to
    /// <c>
    /// true
    /// </c>value.
    /// </param>
    public void SetSelected(int index, bool value)
    {
        if (value == true)
            self.ListBox.SelectRow(self.ListBox.GetRowAtIndex(index));
        else
            self.ListBox.UnselectRow(self.ListBox.GetRowAtIndex(index));
    }

    /// <summary>
    /// Natives the add.
    /// </summary>
    /// <param name="item">
    /// The item.
    /// </param>
    protected void NativeAdd(object item)
    {
        var row = new Gtk.ListBoxRow
        {
            HeightRequest = ItemHeight > 0 ? ItemHeight : DefaultItemHeight
        };
        row.Add(new Gtk.Label(item.ToString()) { Valign = Align.Center, Halign = Align.Start, Expand = true });
        self.ListBox.Add(row);
        if (self.ListBox.IsVisible && !IsUpdateing)
        {
            self.ListBox.ShowAll();
        }
    }

    /// <summary>
    /// Natives the clear.
    /// </summary>
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

    /// <summary>
    /// Natives the get item text.
    /// </summary>
    /// <param name="index">
    /// The index.
    /// </param>
    /// <returns>
    /// System.String.
    /// </returns>
    protected string NativeGetItemText(int index)
    {
        var row = self.ListBox.GetRowAtIndex(index).Child as Gtk.Label;

        return row?.Text;
    }

    /// <summary>
    /// Natives the insert.
    /// </summary>
    /// <param name="index">
    /// The index.
    /// </param>
    /// <param name="item">
    /// The item.
    /// </param>
    protected void NativeInsert(int index, object item)
    {
        var row = new Gtk.ListBoxRow
        {
            HeightRequest = ItemHeight > 0 ? ItemHeight : DefaultItemHeight
        };
        row.Add(new Gtk.Label(item.ToString()) { Valign = Align.Center, Halign = Align.Start, Expand = true });
        self.ListBox.Insert(row, index);
        if (self.ListBox.IsVisible && !IsUpdateing)
        {
            self.ListBox.ShowAll();
        }
    }

    /// <summary>
    /// Natives the remove at.
    /// </summary>
    /// <param name="index">
    /// The index.
    /// </param>
    protected void NativeRemoveAt(int index)
    {
        self.ListBox.Remove(self.ListBox.GetRowAtIndex(index));
    }

    /// <summary>
    /// Handles the
    /// <see cref="!:SelectedIndexChanged" /> event.
    /// </summary>
    /// <param name="e">
    /// The
    /// <see cref="T:System.EventArgs" /> instance containing the event data.
    /// </param>
    protected void OnSelectedIndexChanged(EventArgs e)
    {
        if (self.ListBox.SelectedRow != null)
            self.ListBox.SelectRow(self.ListBox.SelectedRow);
        _selectedIndexChanged?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the
    /// <see cref="SelectedItemChanged" /> event.
    /// </summary>
    /// <param name="e">The
    /// <see cref="T:System.EventArgs" /> instance containing the event data.
    /// </param>
    protected void OnSelectedItemChanged(EventArgs e) => _selectedItemChanged?.Invoke(this, e);

    /// <summary>
    /// Raises the
    /// <see cref="SelectedValueChanged" /> event.
    /// </summary>
    /// <param name="e">The
    /// <see cref="T:System.EventArgs" /> instance containing the event data.
    /// </param>
    protected void OnSelectedValueChanged(EventArgs e) => _selectedValueChanged?.Invoke(this, e);

    /// <summary>
    /// Applies styling to the GTK widget based on control properties.
    /// </summary>
    /// <param name="widget">
    /// GTK widget to style.
    /// </param>
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
            OnSelectedIndexChanged(e);
            OnSelectedValueChanged(e);
            OnSelectedItemChanged(e);
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

    /// <summary>
    /// Class ListBoxItem. Implements the
    /// <see cref="T:System.Windows.Forms.Label" />
    /// </summary>
    /// <seealso cref="T:System.Windows.Forms.Label" />
    public class ListBoxItem : Gtk.Label
    {
        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="T:System.Windows.Forms.ListBox.ListBoxItem" /> class.
        /// </summary>
        public ListBoxItem()
        {
            base.Xalign = 0;
        }

        /// <summary>
        /// Gets or sets the check value.
        /// </summary>
        /// <value>
        /// The check value.
        /// </value>
        public object CheckValue
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the display text.
        /// </summary>
        /// <value>
        /// The display text.
        /// </value>
        public object DisplayText
        {
            get { return base.Text; }
            set { base.Text = value?.ToString(); }
        }

        /// <summary>
        /// Gets or sets the item value.
        /// </summary>
        /// <value>
        /// The item value.
        /// </value>
        public object ItemValue
        {
            get;
            set;
        }

        /// <summary>
        /// Returns a
        /// <see cref="T:System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A
        /// <see cref="T:System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return DisplayText?.ToString();
        }
    }
}