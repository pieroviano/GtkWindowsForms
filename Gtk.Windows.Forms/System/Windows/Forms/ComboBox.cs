/*
 * Based on GTK components, it is a cross-platform interface component that is compatible with the native C# control winform interface.
 * Use this component Gtk.Windows.Forms instead of Microsoft.WindowsDesktop.App.WindowsForms, compile once, and run across Windows, Linux, and macOS
 * Technical support 438865652@qq.com，https://www.gtkapp.com, https://gitee.com/easywebfactory, https://github.com/easywebfactory438865652@qq.com，https://www.gtkapp.com, https://gitee.com/easywebfactory, https://github.com/easywebfactory
 * author:chenhongjin
 */

using System.Collections;
using System.ComponentModel;
using System.Data;
using Gtk;
using Gtk.Windows.Forms.Controls.ControlBase;

namespace System.Windows.Forms;

/// <summary>
/// ObjectCollection is the collection of items in the ComboBox. It is used when you want to add items to the ComboBox or retrieve items from it.
/// </summary>
/// <seealso cref="T:System.Collections.IList" />
[DesignerCategory("Component")]
public partial class ComboBox : ListControl
{
    /// <summary>
    /// Backing storage for the data source assigned to the combo box.
    /// </summary>
    public object? _DataSource;

    /// <summary>
    /// Underlying GTK combo box wrapper used to render and manage the native widget.
    /// </summary>
    public readonly ComboBoxBase self = new();

    /// <summary>Set between <see cref="BeginUpdate" /> and <see cref="EndUpdate" />, as on a list box.</summary>
    internal bool IsUpdateing;

    internal int _selectedIndex;

    private readonly ObjectCollection __itemsData;

    private ComboBoxStyle _DropDownStyle;

    private bool _sorted;

    private bool Is_Self_Realized;

    /// <summary>
    /// Occurs when the drop-down portion of the combo box is displayed.
    /// </summary>
    public virtual event EventHandler? DropDown;

    /// <summary>
    /// Occurs when the drop-down portion of the combo box is closed.
    /// </summary>
    public virtual event EventHandler? DropDownClosed;

    /// <summary>
    /// Occurs when the selected index changes. Handlers are stored in the internal
    /// <see cref="T:System.Windows.Forms.ListControl.Events" /> list.
    /// </summary>
    public override event EventHandler SelectedIndexChanged
    {
        add { _selectedIndexChanged += value; }
#pragma warning disable CS8601 // Possible null reference assignment.
        remove { _selectedIndexChanged -= value; }
#pragma warning restore CS8601 // Possible null reference assignment.
    }

    /// <summary>
    /// Occurs when the selected item changes. Handlers are stored in the internal
    /// <see cref="T:System.Windows.Forms.ListControl.Events" /> list.
    /// </summary>
    public override event EventHandler SelectedItemChanged
    {
        add { _selectedItemChanged += value; }
#pragma warning disable CS8601 // Possible null reference assignment.
        remove { _selectedItemChanged -= value; }
#pragma warning restore CS8601 // Possible null reference assignment.
    }

    /// <summary>
    /// Occurs when the selected value changes. Handlers are stored in the internal
    /// <see cref="T:System.Windows.Forms.ListControl.Events" /> list.
    /// </summary>
    public override event EventHandler SelectedValueChanged
    {
        add { _selectedValueChanged += value; }
#pragma warning disable CS8601 // Possible null reference assignment.
        remove { _selectedValueChanged -= value; }
#pragma warning restore CS8601 // Possible null reference assignment.
    }

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.ComboBox" /> class.
    /// Sets up the underlying GTK widget and prepares the items collection.
    /// </summary>
    public ComboBox() : base()
    {
        self.Override.sender = this;
        self.Entry.HasFrame = false;
        self.Entry.WidthChars = 0;
        self.Entry.MaxWidthChars = 1;

        __itemsData = new ObjectCollection(this);
        self.Realized += Self_Realized;
        self.Changed += Self_Changed;
        // Gtk 4: this per-control raw-event handler used a removed *EventArgs type; control-specific
        // input is re-wired via event controllers / native signals and verified at runtime (migration #11).
    }

    /// <summary>
    /// Gets or sets the data source for the combo box. When set, the control will attempt
    /// to populate its items from the provided data structure (DataTable, DataView or IList).
    /// </summary>
    public override object? DataSource
    {
        get => _DataSource;
        set
        {
            _DataSource = value;
            if (self.IsRealized)
            {
                OnSetDataSource();
            }
        }
    }

    /// <summary>
    /// Gets or sets the style of the combo box (editable or drop-down list).
    /// Changing this property updates internal GTK classes and behavior.
    /// </summary>
    public ComboBoxStyle DropDownStyle
    {
        get => _DropDownStyle;
        set
        {
            _DropDownStyle = value;
            if (value == ComboBoxStyle.DropDown)
            {
                self.StyleContext.RemoveClass("DropDownList");
            }
            else if (value == ComboBoxStyle.DropDownList)
            {
                self.StyleContext.AddClass("DropDownList");
                self.Entry.IsEditable = false;
                self.Entry.CanFocus = false;
            }
        }
    }

    /// <summary>
    /// Gets the underlying GTK control used by this wrapper.
    /// </summary>
    public override object GtkControl => self;

    /// <summary>
    /// Gets the collection of items in the combo box.
    /// </summary>
    public ObjectCollection Items { get { return __itemsData; } }

    /// <summary>
    /// Gets or sets the zero-based index of the currently selected item.
    /// Setting to -1 clears the selection and text.
    /// </summary>
    public override int SelectedIndex
    {
        get { return self.Active; }
        set { self.Active = value; _selectedIndex = value; if (value == -1) { Text = ""; } }
    }

    /// <summary>
    /// Gets or sets the currently selected item object from the Items collection, or null.
    /// </summary>
    public object? SelectedItem
    {
        get { return SelectedIndex == -1 ? null : __itemsData[SelectedIndex]; }
        set { var _index = __itemsData.IndexOf(value); if (_index != -1) { SelectedIndex = _index; } }
    }

    /// <summary>
    /// Gets or sets the value of the selected item. Uses the GTK ActiveId for the underlying model.
    /// </summary>
    public override object? SelectedValue
    {
        get { return self.ActiveId; }
        set => self.ActiveId = value?.ToString();
    }

    /// <summary>
    /// Gets or sets a value indicating whether the items are sorted.
    /// </summary>
    public bool Sorted
    {
        get => _sorted;
        set => _sorted = value;
    }

    /// <summary>
    /// Gets or sets the text associated with the combo box's editable portion.
    /// </summary>
    public override string Text
    {
        get => self.Entry.Text;
        set { self.Entry.Text = value; }
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
    /// Returns the display text for the specified item. The method respects DataRow and primitive types when bound.
    /// </summary>
    /// <param name="item">
    /// The item to retrieve text for.
    /// </param>
    /// <returns>
    /// String representation to display for the item.
    /// </returns>
    public override string? GetItemText(object? item)
    {
        if (item is ObjectCollection.Entry entry)
        {
            var type = entry.Item?.GetType();
            if (entry.Item is DataRow dr)
                return dr[DisplayMember]?.ToString();
            if (type != null && type.IsValueType && type.IsPrimitive)
                return type.GetProperty(DisplayMember)?.GetValue(entry)?.ToString();
            return item?.ToString();
        }

        return item?.ToString();
    }

    /// <summary>
    /// Adds an item to the underlying GTK model at the given index with value and display text.
    /// </summary>
    /// <param name="index">
    /// Index at which to insert; ignored when sorted is true.
    /// </param>
    /// <param name="value">
    /// Internal value identifier for the item.
    /// </param>
    /// <param name="text">
    /// Display text for the item.
    /// </param>
    public void NativeAdd(int index, string? value, string? text)
    {
        if (_sorted == false && index > -1)
        {
            self.Insert(index, value, text);
        }
        else
        {

            self.Append(value, text);
        }
    }

    /// <summary>
    /// Retrieves the display text for the item at the specified index from the native GTK model.
    /// </summary>
    /// <param name="index">
    /// Zero-based item index.
    /// </param>
    /// <returns>
    /// Display text, or null as string if unavailable.
    /// </returns>
    public string? NativeGetItemText(int index)
    {
        self.Model.GetIter(out var iter, new TreePath(new int[] { index }));
        var val = self.Model.GetValue(iter, 1);

        return val?.ToString();
    }

    /// <summary>
    /// Raises the DropDown event.
    /// </summary>
    protected virtual void OnDropDown(System.EventArgs e)
    {
        DropDown?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the DropDownClosed event.
    /// </summary>
    protected virtual void OnDropDownClosed(System.EventArgs e)
    {
        DropDownClosed?.Invoke(this, e);
    }

    /// <summary>
    /// Suspends the layout work of adding items, so a batch of them costs one pass instead of one each.
    /// </summary>
    public void BeginUpdate()
    {
        IsUpdateing = true;
    }

    /// <summary>Resumes after <see cref="BeginUpdate" /> and asks for the layout the batch made stale.</summary>
    public void EndUpdate()
    {
        IsUpdateing = false;
        self.QueueResize();
    }

    /// <summary>
    /// Raises the
    /// <see cref="SelectedIndexChanged" /> event.
    /// </summary>
    /// <param name="e">The
    /// <see cref="T:System.EventArgs" /> instance containing the event data.
    /// </param>
    protected virtual void OnSelectedIndexChanged(EventArgs e) => _selectedIndexChanged?.Invoke(this, e);

    /// <summary>
    /// Raises the
    /// <see cref="SelectedItemChanged" /> event.
    /// </summary>
    /// <param name="e">The
    /// <see cref="T:System.EventArgs" /> instance containing the event data.
    /// </param>
    protected virtual void OnSelectedItemChanged(EventArgs e) => _selectedItemChanged?.Invoke(this, e);

    /// <summary>
    /// Raises the
    /// <see cref="SelectedValueChanged" /> event.
    /// </summary>
    /// <param name="e">The
    /// <see cref="T:System.EventArgs" /> instance containing the event data.
    /// </param>
    protected virtual void OnSelectedValueChanged(EventArgs e) => _selectedValueChanged?.Invoke(this, e);

    private void LoadDataTableSource(DataTable dtable)
    {
        __itemsData.Clear();
        if (dtable.Columns.Contains(ValueMember) && dtable.Columns.Contains(DisplayMember))
        {
            foreach (DataRow row in dtable.Rows)
                __itemsData.Add(row[ValueMember].ToString(), row[DisplayMember].ToString(), row);
        }
        else if (dtable.Columns.Contains(DisplayMember))
        {
            foreach (DataRow row in dtable.Rows)
                __itemsData.Add("", row[DisplayMember].ToString(), row);
        }
        else
        {
            throw new Exception("DisplayMember the property is not assigned or the field name does not exist");
        }
    }

    private void LoadListSource(IList list)
    {
        __itemsData.Clear();
        if (list.Count > 0)
        {
            var type = list[0].GetType();
            var valproperty = type.GetProperty(ValueMember);
            var disproperty = type.GetProperty(DisplayMember);
            foreach (var entry in list)
                __itemsData.Add(valproperty?.GetValue(entry)?.ToString(), disproperty?.GetValue(entry)?.ToString(), entry);
        }
    }

    private void OnSetDataSource()
    {
        if (_DataSource != null)
        {
            if (_DataSource is DataTable dtable)
            {
                LoadDataTableSource(dtable);
            }
            else if (_DataSource is DataView dview)
            {
                LoadDataTableSource(dview.Table);
            }
            else if (_DataSource is IList list)
            {
                LoadListSource(list);
            }
        }
    }

    private void Self_Changed(object sender, EventArgs e)
    {
        if (self.IsVisible)
        {
            OnSelectedIndexChanged(e);
            OnSelectedValueChanged(e);
            OnSelectedItemChanged(e);
        }
    }

    private void Self_Realized(object sender, EventArgs e)
    {
        if (!Is_Self_Realized)
        {
            Is_Self_Realized = true;
            OnSetDataSource();
            if (_DropDownStyle == ComboBoxStyle.DropDownList)
            {
                // A DropDownList shows its selection and cannot be typed into. Making the entry unusable is
                // enough; it must keep its width. Squashing it to 1px and widening the arrow button to the
                // whole control instead made the button, not the combo, carry the width - so the arrow tore
                // away from the text and the combo's natural width grew past the size the form asked for.
                self.Entry.IsEditable = false;
                self.Entry.CanFocus = false;
            }
        }
    }
}