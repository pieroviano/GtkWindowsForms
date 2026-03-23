/*
 * Based on GTK components, it is a cross-platform interface component that is compatible with the native C# control winform interface.
 * Use this component GtkSystem.Windows.Forms instead of Microsoft.WindowsDesktop.App.WindowsForms, compile once, and run across Windows, Linux, and macOS
 * Technical support 438865652@qq.com，https://www.gtkapp.com, https://gitee.com/easywebfactory, https://github.com/easywebfactory438865652@qq.com，https://www.gtkapp.com, https://gitee.com/easywebfactory, https://github.com/easywebfactory
 * author:chenhongjin
 */

using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Reflection;
using Gtk;
using GtkSystem.Windows.Forms.GtkControls.ControlBase;

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
    public object _DataSource;

    /// <summary>
    /// Underlying GTK combo box wrapper used to render and manage the native widget.
    /// </summary>
    public readonly ComboBoxBase self = new ComboBoxBase();

    internal int _selectedIndex;

    private readonly ObjectCollection __itemsData;

    private ComboBoxStyle _DropDownStyle;

    private bool _sorted;

    /// <summary>
    /// Occurs when the drop-down portion of the combo box is displayed.
    /// </summary>
    public virtual event EventHandler DropDown;

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
    /// <see cref="T:System.Windows.Forms.ComboBox" /> class.
    /// Sets up the underlying GTK widget and prepares the items collection.
    /// </summary>
    public ComboBox() : base()
    {
        self.Entry.HasFrame = false;
        self.Entry.WidthChars = 0;

        __itemsData = new ObjectCollection(this);
        self.Realized += Self_Realized;
        self.Changed += Self_Changed;
    }

    /// <summary>
    /// Gets or sets the data source for the combo box. When set, the control will attempt
    /// to populate its items from the provided data structure (DataTable, DataView or IList).
    /// </summary>
    public override object DataSource
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
    public object SelectedItem
    {
        get { return SelectedIndex == -1 ? null : __itemsData[SelectedIndex]; }
        set { var _index = __itemsData.IndexOf(value); if (_index != -1) { SelectedIndex = _index; } }
    }

    /// <summary>
    /// Gets or sets the value of the selected item. Uses the GTK ActiveId for the underlying model.
    /// </summary>
    public override object SelectedValue
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
    public override string GetItemText(object item)
    {
        if (item is ObjectCollection.Entry entry)
        {
            Type type = entry.Item.GetType();
            if (entry.Item is DataRow dr)
                return dr[DisplayMember]?.ToString();
            else if (type.IsValueType && type.IsPrimitive)
                return type.GetProperty(DisplayMember).GetValue(entry)?.ToString();
            else
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
    public void NativeAdd(int index, string value, string text)
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
    public string NativeGetItemText(int index)
    {
        self.Model.GetIter(out TreeIter iter, new TreePath(new int[] { index }));
        object val = self.Model.GetValue(iter, 1);

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
    /// Raises the
    /// <see cref="SelectedIndexChanged" /> event.
    /// </summary>
    /// <param name="e">The
    /// <see cref="T:System.EventArgs" /> instance containing the event data.
    /// </param>
    protected void OnSelectedIndexChanged(EventArgs e) => _selectedIndexChanged?.Invoke(this, e);

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
        OnSetDataSource();
        var ws = ((Gtk.Box)self.Children[0].Parent).Children[1] as Gtk.ToggleButton;
        ws.Toggled += Ws_Toggled;
        if (DropDownStyle == ComboBoxStyle.DropDownList)
        {
            self.Entry.IsEditable = false;
            self.Entry.CanFocus = false;
            self.Entry.NoShowAll = true;
            self.Entry.WidthRequest = 1;
            ws.WidthRequest = self.WidthRequest;
            ws.Label = self.Entry.Text;
            ws.Image = Gtk.Image.NewFromIconName("pan-down", Gtk.IconSize.Button);
            ws.ImagePosition = Gtk.PositionType.Right;
            ws.AlwaysShowImage = true;
            ws.Valign = Align.Center;
            ws.Yalign = 0.5f;
            ws.Xalign = 0.95f;
            ws.Hexpand = true;
            ws.Image.Halign = Gtk.Align.End;
            ws.Image.Valign = Gtk.Align.Center;
            ws.Drawn += Ws_Drawn;
        }
    }

    private void Ws_Drawn(object o, Gtk.DrawnArgs args)
    {
        self.Entry.Visible = false;
        var ws = (Gtk.ToggleButton)o;
        ws.WidthRequest = -1;
        Pango.Context pangocontext = ws.PangoContext;
        string family = pangocontext.FontDescription.Family;
        args.Cr.SelectFontFace(family, Cairo.FontSlant.Normal, Cairo.FontWeight.Normal);
        if (this.ForeColor.Name == "0")
        {
            Gdk.RGBA color = ws.StyleContext.GetColor(StateFlags.Normal);
            args.Cr.SetSourceRGBA(color.Red, color.Green, color.Blue, color.Alpha);
        }
        else
        {
            args.Cr.SetSourceRGBA(this.ForeColor.R / 255, this.ForeColor.G / 255, this.ForeColor.B / 255, this.ForeColor.A / 255);
        }

        double fontsize = pangocontext.FontDescription.Size / Pango.Scale.PangoScale * 1.5;
        args.Cr.Save();
        args.Cr.Translate(10, (ws.AllocatedHeight - fontsize) / 2 + fontsize - 2);
        args.Cr.SetFontSize(fontsize);
        Cairo.TextExtents ext = args.Cr.TextExtents(self.Entry.Text);
        string text = self.Entry.Text;
        while (text.Length > 1 && args.Cr.TextExtents(text).Width > ws.AllocatedWidth - 40)
            text = text.Substring(0, text.Length - 1);

        args.Cr.ShowText(text);
        args.Cr.Restore();
    }

    private void Ws_Toggled(object sender, EventArgs e)
    {
        OnDropDown(e);
    }
}