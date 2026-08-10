/*
 * Based on GTK components, it is a cross-platform interface component that is compatible with the native C# control winform interface.
 * Use this component GtkSystem.Windows.Forms instead of Microsoft.WindowsDesktop.App.WindowsForms, compile once, and run across Windows, Linux, and macOS
 * Technical support 438865652@qq.com，https://www.gtkapp.com, https://gitee.com/easywebfactory, https://github.com/easywebfactory438865652@qq.com，https://www.gtkapp.com, https://gitee.com/easywebfactory, https://github.com/easywebfactory
 * author:chenhongjin
 */

using System.Collections;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using Gtk;
using GtkSystem.Windows.Forms.GtkControls.ControlBase;
using Pango;

namespace System.Windows.Forms;

/// <summary>
/// Represents a Windows list view control, which displays a collection of items that can be displayed using one of four different views.
/// </summary>
[DefaultEvent("SelectedIndexChanged")]
public class ListView : ContainerControl
{
    /// <summary>
    /// The self
    /// </summary>
    public readonly ListViewBase self = new ListViewBase();

    internal Gtk.Box flowBoxContainer = new Gtk.Box(Gtk.Orientation.Vertical, 0);

    internal Gtk.Box header = new Gtk.Box(Gtk.Orientation.Horizontal, 0);

    internal Gtk.ScrolledWindow headerView = new Gtk.ScrolledWindow();

    internal Gtk.ScrolledWindow scrolledWindow = new Gtk.ScrolledWindow();

    private int __headerheight = 30;

    private ColumnHeaderCollection _columns;

    private ListViewGroup? _defaultGroup;

    private int _fontSize;

    private ListViewGroupCollection _groups;

    private ListViewItemCollection _items;

    private bool _MultiSelect = true;

    private bool Is_Control_Realized = false;

    private bool IsCacheUpdate;

    private int SelectedColumnIndex = -1;

    private double sizex = 0;

    private double startx = 0;

    /// <summary>
    /// Occurs when checked changed.
    /// </summary>
    public virtual event EventHandler<object?>? CheckedChanged;

    /// <summary>
    /// Occurs when column click.
    /// </summary>
    public virtual event ColumnClickEventHandler? ColumnClick;

    /// <summary>
    /// Occurs when column reordered.
    /// </summary>
    public virtual event ColumnReorderedEventHandler? ColumnReordered;

    /// <summary>
    /// Occurs when item activate.
    /// </summary>
    public virtual event EventHandler? ItemActivate;

    /// <summary>
    /// Occurs when item check.
    /// </summary>
    public virtual event ItemCheckEventHandler? ItemCheck;

    /// <summary>
    /// Occurs when item checked.
    /// </summary>
    public virtual event ItemCheckedEventHandler? ItemChecked;

    /// <summary>
    /// Occurs when item selection changed.
    /// </summary>
    public virtual event ListViewItemSelectionChangedEventHandler? ItemSelectionChanged;

    /// <summary>
    /// Occurs when selected index changed.
    /// </summary>
    public virtual event EventHandler? SelectedIndexChanged;

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.ListView" /> class.
    /// </summary>
    public ListView() : base()
    {
        self.Override.sender = this;
        _items = new ListViewItemCollection(this);
        _groups = new ListViewGroupCollection(this);
        _columns = new ColumnHeaderCollection(this);
        self.Realized += Control_Realized;

        flowBoxContainer.Halign = Gtk.Align.Fill;
        flowBoxContainer.Valign = Gtk.Align.Start;
        scrolledWindow.Halign = Gtk.Align.Fill;
        scrolledWindow.Valign = Gtk.Align.Fill;
        scrolledWindow.Add(flowBoxContainer);
        scrolledWindow.Hadjustment.ValueChanged += Hadjustment_ValueChanged;

        header.LayoutStyle = ButtonBoxStyle.Expand;
        header.Homogeneous = false;
        header.Spacing = 0;
        header.BorderWidth = 0;
        header.Halign = Gtk.Align.Start;
        header.Valign = Gtk.Align.Fill;

        headerView.Halign = Gtk.Align.Fill;
        headerView.Valign = Gtk.Align.Start;
        headerView.NoShowAll = true;
        headerView.HeightRequest = __headerheight;
        headerView.HscrollbarPolicy = PolicyType.External;
        headerView.VscrollbarPolicy = PolicyType.Never;
        headerView.StyleContext.AddClass("ListViewHeader");
        headerView.Add(header);
        self.box.PackStart(headerView, false, true, 0);
        self.box.PackStart(scrolledWindow, true, true, 0);
        this.BorderStyle = BorderStyle.Fixed3D;
    }

    /// <summary>
    /// Gets or sets the activation.
    /// </summary>
    /// <value>
    /// The activation.
    /// </value>
    public ItemActivation Activation
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the alignment.
    /// </summary>
    /// <value>
    /// The alignment.
    /// </value>
    public System.Windows.Forms.ListViewAlignment Alignment
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets a value indicating whether allow column reorder.
    /// </summary>
    /// <value>
    /// <c>
    /// true
    /// </c> if allow column reorder; otherwise,
    /// <c>
    /// false
    /// </c>.
    /// </value>
    public bool AllowColumnReorder
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets a value indicating whether icons are automatically arranged.
    /// </summary>
    /// <value>
    /// <c>true</c> if icons are automatically arranged; otherwise, <c>false</c>.
    /// </value>
    public bool AutoArrange
    {
        get;
        set;
    } = true;

    /// <summary>
    /// Gets or sets a value indicating whether check boxes are displayed.
    /// </summary>
    /// <value>
    /// <c>
    /// true
    /// </c> if check boxes; otherwise,
    /// <c>
    /// false
    /// </c>.
    /// </value>
    public bool CheckBoxes
    {
        get;
        set;
    }

    /// <summary>
    /// Gets the checked indices.
    /// </summary>
    /// <value>
    /// The checked indices.
    /// </value>
    public CheckedIndexCollection CheckedIndices
    {
        get
        {
            var selecteditems = new CheckedIndexCollection(this);
            foreach (ListViewItem item in CheckedItems)
            {
                selecteditems.Add(item.Index);
            }

            return selecteditems;
        }
    }

    /// <summary>
    /// Gets the checked items.
    /// </summary>
    /// <value>
    /// The checked items.
    /// </value>
    public CheckedListViewItemCollection CheckedItems
    {
        get
        {
            var selecteditems = new CheckedListViewItemCollection(this);
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
    /// Gets the columns.
    /// </summary>
    /// <value>
    /// The columns.
    /// </value>
    public ColumnHeaderCollection Columns
    {
        get
        {
            return _columns;
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether full row select.
    /// </summary>
    /// <value>
    /// <c>
    /// true
    /// </c> if full row select; otherwise,
    /// <c>
    /// false
    /// </c>.
    /// </value>
    public bool FullRowSelect
    {
        get;
        set;
    } = true;

    /// <summary>
    /// Gets or sets a value indicating whether grid lines.
    /// </summary>
    /// <value>
    /// <c>
    /// true
    /// </c> if grid lines; otherwise,
    /// <c>
    /// false
    /// </c>.
    /// </value>
    public bool GridLines
    {
        get;
        set;
    } = true;

    /// <summary>
    /// Gets or sets the group image list.
    /// </summary>
    /// <value>
    /// The group image list.
    /// </value>
    public ImageList? GroupImageList
    {
        get;
        set;
    }

    /// <summary>
    /// Gets the groups.
    /// </summary>
    /// <value>
    /// The groups.
    /// </value>
    public ListViewGroupCollection Groups
    {
        get
        {
            return _groups;
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
    /// Gets or sets the header style.
    /// </summary>
    /// <value>
    /// The header style.
    /// </value>
    public System.Windows.Forms.ColumnHeaderStyle HeaderStyle
    {
        get;
        set;
    } = ColumnHeaderStyle.Clickable;

    /// <summary>
    /// Gets or sets a value indicating whether hide selection.
    /// </summary>
    /// <value>
    /// <c>
    /// true
    /// </c> if hide selection; otherwise,
    /// <c>
    /// false
    /// </c>.
    /// </value>
    public bool HideSelection
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets a value indicating whether hover selection is enabled.
    /// </summary>
    /// <value>
    /// <c>
    /// true
    /// </c> if hover selection; otherwise,
    /// <c>
    /// false
    /// </c>.
    /// </value>
    public bool HoverSelection
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
    public ListViewItemCollection Items
    {
        get
        {
            return _items;
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether label edit.
    /// </summary>
    /// <value>
    /// <c>
    /// true
    /// </c> if label edit; otherwise,
    /// <c>
    /// false
    /// </c>.
    /// </value>
    public bool LabelEdit
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets a value indicating whether label wrap is enabled.
    /// </summary>
    /// <value>
    /// <c>
    /// true
    /// </c> if label wrap; otherwise,
    /// <c>
    /// false
    /// </c>.
    /// </value>
    public bool LabelWrap
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the large image list.
    /// </summary>
    /// <value>
    /// The large image list.
    /// </value>
    public ImageList? LargeImageList
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the ListView item sorter.
    /// </summary>
    /// <value>
    /// The ListView item sorter.
    /// </value>
    public IComparer? ListViewItemSorter
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets a value indicating whether multi select.
    /// </summary>
    /// <value>
    /// <c>
    /// true
    /// </c> if multi select; otherwise,
    /// <c>
    /// false
    /// </c>.
    /// </value>
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
    /// Gets or sets a value indicating whether owner draw.
    /// </summary>
    /// <value>
    /// <c>
    /// true
    /// </c> if owner draw; otherwise,
    /// <c>
    /// false
    /// </c>.
    /// </value>
    public bool OwnerDraw
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets a value indicating whether this
    /// <see cref="T:System.Windows.Forms.ListView" /> is scrollable.
    /// </summary>
    /// <value>
    /// <c>
    /// true
    /// </c> if scrollable; otherwise,
    /// <c>
    /// false
    /// </c>.
    /// </value>
    public bool Scrollable
    {
        get;
        set;
    }

    /// <summary>
    /// Gets the selected indices.
    /// </summary>
    /// <value>
    /// The selected indices.
    /// </value>
    public SelectedIndexCollection SelectedIndices
    {
        get
        {
            var selecteditems = new SelectedIndexCollection();
            foreach (ListViewItem item in SelectedItems)
            {
                selecteditems.Add(item.Index);
            }

            return selecteditems;
        }
    }

    /// <summary>
    /// Gets the selected items.
    /// </summary>
    /// <value>
    /// The selected items.
    /// </value>
    public SelectedListViewItemCollection SelectedItems
    {
        get
        {
            var selecteditems = new SelectedListViewItemCollection();
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
    /// Gets or sets a value indicating whether show groups.
    /// </summary>
    /// <value>
    /// <c>
    /// true
    /// </c> if show groups; otherwise,
    /// <c>
    /// false
    /// </c>.
    /// </value>
    public bool ShowGroups
    {
        get;
        set;
    } = true;

    /// <summary>
    /// Gets or sets a value indicating whether show item tool tips.
    /// </summary>
    /// <value>
    /// <c>
    /// true
    /// </c> if show item tool tips; otherwise,
    /// <c>
    /// false
    /// </c>.
    /// </value>
    public bool ShowItemToolTips
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the small image list.
    /// </summary>
    /// <value>
    /// The small image list.
    /// </value>
    public ImageList? SmallImageList
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the column used to sort the items.
    /// </summary>
    /// <value>
    /// The sort column.
    /// </value>
    public ColumnHeader? SortColumn
    {
        set;
        get;
    }

    /// <summary>
    /// Gets or sets a value indicating whether this
    /// <see cref="T:System.Windows.Forms.ListView" /> is sorted.
    /// </summary>
    /// <value>
    /// <c>
    /// true
    /// </c> if sorted; otherwise,
    /// <c>
    /// false
    /// </c>.
    /// </value>
    public bool Sorted
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the sorting.
    /// </summary>
    /// <value>
    /// The sorting.
    /// </value>
    public System.Windows.Forms.SortOrder Sorting
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the state image list.
    /// </summary>
    /// <value>
    /// The state image list.
    /// </value>
    public ImageList? StateImageList
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets a value indicating whether use compatible state image behavior.
    /// </summary>
    /// <value>
    /// <c>
    /// true
    /// </c> if use compatible state image behavior; otherwise,
    /// <c>
    /// false
    /// </c>.
    /// </value>
    public bool UseCompatibleStateImageBehavior
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the view.
    /// </summary>
    /// <value>
    /// The view.
    /// </value>
    public System.Windows.Forms.View View
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the number of items in the virtual list.
    /// </summary>
    /// <value>
    /// The size of the virtual list.
    /// </value>
    public int VirtualListSize
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets a value indicating whether the list view is in virtual mode.
    /// </summary>
    /// <value>
    /// <c>true</c> if the list view is in virtual mode; otherwise, <c>false</c>.
    /// </value>
    /// <remarks>
    /// This was spelled <c>VirtualModel</c>, so designer-generated code from a real WinForms project
    /// did not compile. The old spelling is kept as an obsolete alias.
    /// </remarks>
    public bool VirtualMode
    {
        get;
        set;
    } = true;

    /// <summary>
    /// Obsolete misspelling of <see cref="P:System.Windows.Forms.ListView.VirtualMode" />.
    /// </summary>
    [Obsolete("Use VirtualMode; VirtualModel was a misspelling of the WinForms property name.")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public bool VirtualModel
    {
        get => VirtualMode;
        set => VirtualMode = value;
    }

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
                if (Font != null && Font.Size > 5)
                {
                    if (Font.Unit == GraphicsUnit.Point)
                        _fontSize = (int)Font.Size * 96 / 72;
                    else
                        _fontSize = (int)Font.Size;
                }
                else
                {
                    _fontSize = 12;
                }
            }

            return _fontSize;
        }
    }

    /// <summary>
    /// Begins the update.
    /// </summary>
    public void BeginUpdate()
    {
        IsCacheUpdate = true;
    }

    /// <summary>
    /// Clears this instance.
    /// </summary>
    public void Clear()
    {
        // WinForms Clear() removes the columns too; they were being left behind.
        Items.Clear();
        Groups.Clear();
        Columns.Clear();
    }

    /// <summary>
    /// Ends the update.
    /// </summary>
    public void EndUpdate()
    {
        IsCacheUpdate = false;
        self.Window?.ProcessUpdates(true);
        self.ShowAll();
    }

    /// <summary>
    /// Finds the item with text.
    /// </summary>
    /// <param name="text">
    /// The text.
    /// </param>
    /// <returns>
    /// ListViewItem.
    /// </returns>
    public ListViewItem FindItemWithText(string text)
    {
        return Items.Find(w => w.Text == text);
    }

    /// <summary>
    /// Finds the item with text.
    /// </summary>
    /// <param name="text">
    /// The text.
    /// </param>
    /// <param name="includeSubItemsInSearch">
    /// if set to
    /// <c>
    /// true
    /// </c>include sub items in search.
    /// </param>
    /// <param name="startIndex">
    /// The start index.
    /// </param>
    /// <returns>
    /// ListViewItem.
    /// </returns>
    public ListViewItem? FindItemWithText(string text, bool includeSubItemsInSearch, int startIndex)
    {
        return FindItemWithText(text, includeSubItemsInSearch, startIndex, false);
    }

    /// <summary>
    /// Finds the item with text.
    /// </summary>
    /// <param name="text">
    /// The text.
    /// </param>
    /// <param name="includeSubItemsInSearch">
    /// if set to
    /// <c>
    /// true
    /// </c>include sub items in search.
    /// </param>
    /// <param name="startIndex">
    /// The start index.
    /// </param>
    /// <param name="isPrefixSearch">
    /// if set to
    /// <c>
    /// true
    /// </c>is prefix search.
    /// </param>
    /// <returns>
    /// ListViewItem.
    /// </returns>
    public ListViewItem? FindItemWithText(string text, bool includeSubItemsInSearch, int startIndex, bool isPrefixSearch)
    {
        // Both flags used to be ignored: sub-items were never searched and a prefix search behaved as an
        // exact match.
        if (text == null || startIndex < 0 || startIndex >= Items.Count)
        {
            return null;
        }

        bool Matches(string? candidate) => isPrefixSearch
            ? candidate != null && candidate.StartsWith(text, StringComparison.CurrentCultureIgnoreCase)
            : string.Equals(candidate, text, StringComparison.CurrentCultureIgnoreCase);

        int idx = Items.FindIndex(startIndex, item =>
        {
            if (Matches(item.Text))
            {
                return true;
            }

            if (!includeSubItemsInSearch || item.SubItems == null)
            {
                return false;
            }

            for (var i = 0; i < item.SubItems.Count; i++)
            {
                if (Matches(item.SubItems[i].Text))
                {
                    return true;
                }
            }

            return false;
        });

        return idx == -1 ? null : Items[idx];
    }

    /// <summary>
    /// Gets the item at.
    /// </summary>
    /// <param name="x">
    /// The x.
    /// </param>
    /// <param name="y">
    /// The y.
    /// </param>
    /// <returns>
    /// ListViewItem.
    /// </returns>
    public ListViewItem? GetItemAt(int x, int y)
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
    /// Gets the item rect.
    /// </summary>
    /// <param name="index">
    /// The index.
    /// </param>
    /// <returns>
    /// Drawing.Rectangle.
    /// </returns>
    /// <exception cref="T:System.NotImplementedException">
    /// </exception>
    public Drawing.Rectangle GetItemRect(int index)
    {
        throw new System.NotImplementedException();
    }

    /// <summary>
    /// Sorts this instance.
    /// </summary>
    public void Sort()
    {
        // ListViewItemSorter was ignored entirely - Sort() only refreshed GTK column sorting, so
        // the WinForms custom-sorter API was a no-op. Sort the item model with it; the visual
        // order of an already-realized view still follows column sorting only.
        if (ListViewItemSorter != null)
        {
            Items.Sort((x, y) => ListViewItemSorter.Compare(x, y));
        }

        if (SortColumn != null)
        {
            foreach (var group in GetAllGroups())
            {
                group.FlowBox.InvalidateSort();
            }
        }
    }

    internal void GetSubItemAt(int x, int y, out int iItem, out int iSubItem)
    {
        iItem = -1;
        iSubItem = -1;
        var position = -(int)headerView.Hadjustment.Value;
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
            var padding = 5;
            var flowitem = new Gtk.FlowBoxChild();
            flowitem.TooltipText = item.Text;
            flowitem.Data.Add("ItemId", item.Index);
            flowitem.Halign = Gtk.Align.Start;
            flowitem.Valign = Gtk.Align.Start;
            flowitem.BorderWidth = 0;
            flowitem.Margin = 0;
            flowitem.HeightRequest = FontSize + 10;
            item._flowBoxChild = flowitem;
            var hBox = new BoxBase(Gtk.Orientation.Horizontal, 0);
            if (item.BackColor.HasValue)
            {
                hBox.Override.BackColor = Drawing.Color.FromArgb(180, item.BackColor.Value);
            }
            hBox.Valign = Gtk.Align.Fill;
            hBox.Halign = Gtk.Align.Fill;
            hBox.BorderWidth = 0;
            hBox.Spacing = 0;
            hBox.Homogeneous = false;

            foreach (ColumnHeader col in Columns)
            {
                if (item.SubItems != null && item.SubItems.Count > col.Index)
                {
                    flowitem.Data.Add(col.Index, item.SubItems[col.Index].Text);
                }
                else
                    flowitem.Data.Add(col.Index, string.Empty);
            }

            flowitem.Add(hBox);

            Gtk.FlowBox flowBox = DefaultGroup.FlowBox;
            if (this.ShowGroups == true && this.View != View.List && this.View != View.Tile)
            {
                if (Groups.Exists(g => g.Name == item.Group?.Name))
                    flowBox = item.Group?.FlowBox ?? flowBox;
                else
                    NativeGroupAdd(DefaultGroup, -1);
            }
            else
            {
                NativeGroupAdd(DefaultGroup, -1);
            }

            if (position == -1)
                flowBox.Add(flowitem);
            else
                flowBox.Insert(flowitem, position);

            var fistcell = new Gtk.Overlay();
            fistcell.Halign = Gtk.Align.Fill;
            fistcell.Valign = Gtk.Align.Fill;
            fistcell.BorderWidth = 1;
            if (Columns.Count > 0)
                fistcell.WidthRequest = Columns[0].Width;
            else
                fistcell.WidthRequest = Width - padding - padding;
            var checkBox = new CheckBox();
            CheckBoxBase checkboxself = checkBox.self;
            checkboxself.Halign = Gtk.Align.Start;
            checkboxself.Valign = Gtk.Align.Center;
            checkboxself.BorderWidth = 0;
            checkboxself.Relief = ReliefStyle.None;
            checkboxself.StyleContext.AddClass("ButtonNone");
            checkboxself.Label = item.Text;
            checkboxself.Active = item.Checked;
            checkboxself.DrawIndicator = this.CheckBoxes;
        // Gtk 4: this per-control raw-event handler used a removed *EventArgs type; control-specific
        // input is re-wired via event controllers / native signals and verified at runtime (migration #11).
            if (checkboxself.Child is Gtk.Label label)
            {
                label.Halign = Gtk.Align.Start;
                label.Valign = Gtk.Align.Fill;
                if (item.ForeColor.HasValue)
                {
                    label.Attributes = new Pango.AttrList();
                    var fg = new Pango.AttrForeground(Convert.ToUInt16(item.ForeColor.Value.R * 257), Convert.ToUInt16(item.ForeColor.Value.G * 257), Convert.ToUInt16(item.ForeColor.Value.B * 257));
                    label.Attributes.Insert(fg);
                }
            }

            if (fistcell.WidthRequest == -1)
                fistcell.Add(checkboxself);
            else
                fistcell.AddOverlay(checkboxself);

            checkboxself.Xalign = 0.0f;
            checkboxself.Yalign = 0.5f;
            Gtk.Viewport viewport = new Viewport();
            viewport.BorderWidth = 0;
            viewport.Add(fistcell);
            hBox.PackStart(viewport, true, true, 0);
            if (this.View == View.Details)
            {
                headerView.ShowAll();
                flowBox.MaxChildrenPerLine = 1;
                if (this.SmallImageList != null)
                {
                    int width = this.SmallImageList.ImageSize.Width;
                    int height = this.SmallImageList.ImageSize.Height;
                    if (height > flowitem.HeightRequest)
                        flowitem.HeightRequest = height;

                    checkboxself.AlwaysShowImage = true;
                    checkboxself.ImagePosition = PositionType.Left;
                    if (!string.IsNullOrWhiteSpace(item.ImageKey))
                    {
                        Drawing.Image? img = this.SmallImageList.GetBitmap(item.ImageKey);
                        if (img != null)
                        {
                            checkboxself.Image = new Gtk.Image(img.Pixbuf) { WidthRequest = width, HeightRequest = height, Halign = Align.Start };
                        }
                    }
                    else if (item.ImageIndex > -1)
                    {
                        Drawing.Image? img = this.SmallImageList.GetBitmap(item.ImageIndex);
                        if (img != null)
                        {
                            checkboxself.Image = new Gtk.Image(img.Pixbuf) { WidthRequest = width, HeightRequest = height, Halign = Align.Start };
                        }
                    }
                }
                var index = 0;
                foreach (ColumnHeader col in Columns)
                {
                    if (index > 0)
                    {
                        var sublayout = new Gtk.Overlay();
                        sublayout.Halign = Gtk.Align.Fill;
                        sublayout.Valign = Gtk.Align.Fill;
                        sublayout.WidthRequest = col.Width;
                        if (item.SubItems != null && item.SubItems.Count > index)
                        {
                            ListViewItem.ListViewSubItem subitem = item.SubItems[index];
                            var sublabel = new Gtk.Label();
                            subitem._label = sublabel;

                            if (subitem.ForeColor.HasValue)
                            {
                                sublabel.Attributes = new Pango.AttrList();
                                var fg = new Pango.AttrForeground(Convert.ToUInt16(subitem.ForeColor.Value.R * 257), Convert.ToUInt16(subitem.ForeColor.Value.G * 257), Convert.ToUInt16(subitem.ForeColor.Value.B * 257));
                                sublabel.Attributes.Insert(fg);
                            }
                            sublabel.WidthChars = 0;
                            sublabel.MaxWidthChars = 0;
                            sublabel.Halign = Gtk.Align.Fill;
                            sublabel.Valign = Gtk.Align.Center;
                            sublabel.MarginStart = padding;
                            sublabel.Xalign = 0.0f;
                            sublabel.Yalign = 0.5f;
                            sublabel.Ellipsize = Pango.EllipsizeMode.End;
                            sublabel.Text = subitem.Text;
                            sublayout.AddOverlay(sublabel);
                        }
                        Gtk.Viewport sviewport = new Viewport();
                        sviewport.BorderWidth = 0;
                        sviewport.Add(sublayout);
                        hBox.PackStart(sviewport, true, true, 0);
                    }
                    index++;
                }
            }
            else
            {
                headerView.Hide();
                if (this.View == View.LargeIcon)
                {
                    if (this.LargeImageList != null)
                    {
                        int width = this.LargeImageList.ImageSize.Width;
                        int height = this.LargeImageList.ImageSize.Height;
                        flowitem.WidthRequest = width + 40;
                        flowitem.HeightRequest = height + FontSize + 20;

                        checkboxself.AlwaysShowImage = true;
                        checkboxself.ImagePosition = PositionType.Top;
                        if (!string.IsNullOrWhiteSpace(item.ImageKey))
                        {
                            Drawing.Image? img = this.LargeImageList.GetBitmap(item.ImageKey);
                            if (img != null)
                            {
                                checkboxself.Image = new Gtk.Image(img.Pixbuf) { WidthRequest = width, HeightRequest = height, Halign = Align.Start };
                            }
                        }
                        else if (item.ImageIndex > -1)
                        {
                            Drawing.Image? img = this.LargeImageList.GetBitmap(item.ImageIndex);
                            if (img != null)
                            {
                                checkboxself.Image = new Gtk.Image(img.Pixbuf) { WidthRequest = width, HeightRequest = height, Halign = Align.Start };
                            }
                        }
                    }
                }
                else
                {
                    if (this.SmallImageList != null)
                    {
                        int width = this.SmallImageList.ImageSize.Width;
                        int height = this.SmallImageList.ImageSize.Height;
                        if (height > flowitem.HeightRequest)
                            flowitem.HeightRequest = height;

                        checkboxself.AlwaysShowImage = true;
                        checkboxself.ImagePosition = PositionType.Left;
                        if (!string.IsNullOrWhiteSpace(item.ImageKey))
                        {
                            var img = this.SmallImageList.GetBitmap(item.ImageKey);
                            if (img != null)
                            {
                                checkboxself.Image = new Gtk.Image(img.Pixbuf) { WidthRequest = width, HeightRequest = height, Halign = Align.Start };
                            }
                        }
                        else if (item.ImageIndex > -1)
                        {
                            var img = this.SmallImageList.GetBitmap(item.ImageIndex);
                            if (img != null)
                            {
                                checkboxself.Image = new Gtk.Image(img.Pixbuf) { WidthRequest = width, HeightRequest = height, Halign = Align.Start };
                            }
                        }
                    }
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
            var box = item._flowBoxChild.Child as Gtk.Box;
            if (this.View == View.Details)
            {
                var viewport = box?.Children[0] as Gtk.Viewport;
                var layout = viewport?.Child as Gtk.Overlay;
                foreach (var chk in layout?.Children ?? [])
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
                foreach (var chk in box?.Children ?? [])
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
                if (Sorting == SortOrder.Descending)
                {
                    position = Groups.OrderByDescending(o => o.Header).ToList().FindIndex(g => g.Name == group.Name);
                }
                else if (Sorting == SortOrder.Ascending)
                {
                    position = Groups.OrderBy(o => o.Header).ToList().FindIndex(g => g.Name == group.Name);
                }
            }

            Gtk.FlowBox _flow = DefaultGroup.FlowBox;
            var hBox = new Gtk.Box(Gtk.Orientation.Vertical, 0);
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
                var groupline = new Gtk.Viewport();
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
                    }
                    if (this.View == View.Details)
                        _flow.StyleContext.AddClass("GridBorder");
                }
                else
                {
                    if (DefaultGroup._groupbox != null && DefaultGroup._groupbox is Gtk.Box defbox)
                    {
                        defbox.NoShowAll = false;
                        defbox.Visible = true;
                    }
                    if (this.View == View.Details)
                        _flow.StyleContext.AddClass("GridBorder");
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
            _flow.MaxChildrenPerLine = 50u;
            _flow.MinChildrenPerLine = 0u;
            _flow.Halign = Gtk.Align.Fill;
            _flow.Valign = Gtk.Align.Start;
            _flow.SelectionMode = _MultiSelect == false ? Gtk.SelectionMode.Single : Gtk.SelectionMode.Multiple;
            _flow.ActivateOnSingleClick = _MultiSelect == false;
            _flow.SortFunc = new Gtk.FlowBoxSortFunc((fbc1, fbc2) =>
            {
                if (SortColumn != null)
                {
                    if (this.Sorting == SortOrder.Ascending)
                        return fbc2.Data[SortColumn.Index].ToString().CompareTo(fbc1.Data[SortColumn.Index].ToString());
                    else if (this.Sorting == SortOrder.Descending)
                        return fbc1.Data[SortColumn.Index].ToString().CompareTo(fbc2.Data[SortColumn.Index].ToString());
                    else
                        return fbc2.Index.CompareTo(fbc1.Index);
                }
                else
                    return 0;
            });
            _flow.SelectedChildrenChanged += _flow_SelectedChildrenChanged;
        // Gtk 4: this per-control raw-event handler used a removed *EventArgs type; control-specific
        // input is re-wired via event controllers / native signals and verified at runtime (migration #11).
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
            var label = new Gtk.Label(col.Text) { Xalign = 0, Xpad = 5, MaxWidthChars = 0, Halign = Gtk.Align.Fill, Valign = Gtk.Align.End, Ellipsize = Pango.EllipsizeMode.End, Wrap = false, LineWrap = false };
            label.TooltipText = col.Text;
            label.Name = $"column_t_{col.Index}";
        // Gtk 4: Widget.Drawn is gone and there is no overridable OnSnapshot; custom paint moves to CSS (deferred).
            var columbt = new Gtk.Button(label) { MarginStart = 0, WidthRequest = col.Width, Halign = Gtk.Align.Fill, Valign = Gtk.Align.Fill };
            columbt.Name = $"column_b_{col.Index}";
            columbt.ActionTargetValue = new GLib.Variant(col.Index);
            header.PackStart(columbt, false, true, 0);
            header.ReorderChild(columbt, col.Index);
        }
    }

    internal void NativeHeaderClear()
    {
        // This called the public Clear(), which now also clears the columns - and clearing the columns
        // comes back here. Only the item rows need to go before the headers are removed.
        Items.Clear();
        foreach (var col in header.Children)
        {
            header.Remove(col);
            col.Destroy();
        }
        header.Hide();
    }

    internal void NativeHeaderRemove(ColumnHeader column)
    {
        Items.Clear();
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
            var box = item._flowBoxChild.Child as Gtk.Box;
            if (this.View == View.Details)
            {
                var viewport = box?.Children[0] as Gtk.Viewport;
                var layout = viewport?.Child as Gtk.Overlay;
                foreach (var lab in layout?.Children ?? [])
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
                foreach (var lab in box?.Children ?? [])
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
                var vbox = box?.Children[0] as Gtk.Box;
                foreach (var lab in vbox?.Children ?? [])
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
                foreach (var lab in box?.Children ?? [])
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

    /// <summary>
    /// Raises the ColumnClick event.
    /// </summary>
    protected virtual void OnColumnClick(System.Windows.Forms.ColumnClickEventArgs e)
    {
        ColumnClick?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the ColumnReordered event.
    /// </summary>
    protected virtual void OnColumnReordered(System.Windows.Forms.ColumnReorderedEventArgs e)
    {
        ColumnReordered?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the ItemActivate event.
    /// </summary>
    protected virtual void OnItemActivate(System.EventArgs e)
    {
        ItemActivate?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the ItemCheck event.
    /// </summary>
    protected virtual void OnItemCheck(System.Windows.Forms.ItemCheckEventArgs e)
    {
        ItemCheck?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the ItemChecked event.
    /// </summary>
    protected virtual void OnItemChecked(System.Windows.Forms.ItemCheckedEventArgs e)
    {
        ItemChecked?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the ItemSelectionChanged event.
    /// </summary>
    protected virtual void OnItemSelectionChanged(System.Windows.Forms.ListViewItemSelectionChangedEventArgs e)
    {
        ItemSelectionChanged?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the SelectedIndexChanged event.
    /// </summary>
    protected virtual void OnSelectedIndexChanged(System.EventArgs e)
    {
        SelectedIndexChanged?.Invoke(this, e);
    }

    private void _flow_SelectedChildrenChanged(object sender, EventArgs e)
    {
        var widget = (Gtk.FlowBox)sender;
        var selecteds = new List<IntPtr>();
        foreach (var group in GetAllGroups())
        {
            foreach (FlowBoxChild o in group.FlowBox.SelectedChildren)
            {
                selecteds.Add(o.Handle);
            }
        }
        foreach (ListViewItem item in Items)
        {
            if (item._flowBoxChild != null && selecteds.Contains(item._flowBoxChild.Handle))
            {
                if (item._selected == false)
                {
                    item._selected = true;
                    OnItemSelectionChanged(new ListViewItemSelectionChangedEventArgs(item, item.Index, item.Selected));
                }
            }
            else if (item._selected == true)
            {
                item._selected = false;
                OnItemSelectionChanged(new ListViewItemSelectionChangedEventArgs(item, item.Index, item.Selected));
            }
        }
        OnSelectedIndexChanged(e);
    }

    private void ColomnSort(object sender)
    {
        if (this.HeaderStyle == ColumnHeaderStyle.Clickable)
        {
            var btn = (Gtk.Button)sender;
            var actioncolumn = (int)btn.ActionTargetValue;
            if (SelectedColumnIndex != actioncolumn)
                Sorting = SortOrder.None;
            SelectedColumnIndex = actioncolumn;
            SortColumn = this.Columns[actioncolumn];
            if (Sorting == SortOrder.Ascending)
                Sorting = SortOrder.Descending;
            else if (Sorting == SortOrder.Descending)
                Sorting = SortOrder.None;
            else
                Sorting = SortOrder.Ascending;

            this.Sort();

            OnColumnClick(new ColumnClickEventArgs((int)btn.ActionTargetValue));
        }
    }

    private void Control_Realized(object sender, EventArgs e)
    {
        if (!Is_Control_Realized)
        {
            Is_Control_Realized = true;
            if (this.View == View.Details && HeaderStyle != ColumnHeaderStyle.None)
            {
                headerView.NoShowAll = false;
                headerView.ShowAll();
            }
            foreach (ColumnHeader cheader in this.Columns)
            {
                NativeHeaderAdd(cheader);
            }
            foreach (ListViewGroup g in Groups)
            {
                NativeGroupAdd(g, -1);
            }
            foreach (ListViewItem item in Items)
            {
                NativeAdd(item, -1);
            }

            MultiSelect = _MultiSelect == true;
            self.ShowAll();
        }
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

    private void OnCheckedChanged(object? sender)
    {
        var box = sender as CheckBox;
        var checkitem = box?.self.Parent.Parent.Parent.Parent as Gtk.FlowBoxChild;
        var thisitem = this.Items.Find(m => m._flowBoxChild?.Handle == checkitem?.Handle);
        if (thisitem != null)
        {
            thisitem.Checked = box?.self.Active ?? false;
            ItemCheck?.Invoke(sender!, new ItemCheckEventArgs(checkitem?.Index ?? -1, (box?.self.Active ?? false) ? CheckState.Checked : CheckState.Unchecked, (box?.self.Active ?? false) ? CheckState.Unchecked : CheckState.Checked));
            if ((box?.self.Active ?? false) == true)
            {
                ItemChecked?.Invoke(sender!, new ItemCheckedEventArgs(thisitem));
            }
        }
    }

    /// <summary>
    /// Class CheckedIndexCollection.
    /// Implements the System.Collections.Generic.List&lt;System.Int32&gt;
    /// </summary>
    /// System.Collections.Generic.List&lt;System.Int32&gt;
    ///
    ///
    ///
    ///
    public class CheckedIndexCollection : List<int>
    {
        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="T:System.Windows.Forms.ListView.CheckedIndexCollection" /> class.
        /// </summary>
        /// <param name="owner">
        /// The owner.
        /// </param>
        public CheckedIndexCollection(ListView owner)
        {

        }
    }

    /// <summary>
    /// Class CheckedListViewItemCollection.
    /// Implements the System.Collections.Generic.List&lt;System.Windows.Forms.ListViewItem&gt;
    /// </summary>
    [ListBindable(false)]
    public class CheckedListViewItemCollection : List<ListViewItem>
    {
        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="T:System.Windows.Forms.ListView.CheckedListViewItemCollection" /> class.
        /// </summary>
        /// <param name="owner">
        /// The owner.
        /// </param>
        public CheckedListViewItemCollection(ListView owner)
        {

        }

        /// <summary>
        /// Gets the
        /// <see cref="T:System.Windows.Forms.ListViewItem" /> with the specified key.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <returns>
        /// ListViewItem.
        /// </returns>
        public virtual ListViewItem this[string key]
        {
            get
            {
                return this.Find(w => w.Name == key);
            }
        }
    }

    /// <summary>
    /// Class ColumnHeaderCollection.
    /// Implements the System.Collections.Generic.List&lt;System.Windows.Forms.ColumnHeader&gt;
    /// </summary>
    [ListBindable(false)]
    public class ColumnHeaderCollection : List<ColumnHeader>
    {
        readonly ListView _owner;

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="T:System.Windows.Forms.ListView.ColumnHeaderCollection" /> class.
        /// </summary>
        /// <param name="owner">
        /// The owner.
        /// </param>
        public ColumnHeaderCollection(ListView owner)
        {
            _owner = owner;
        }

        /// <summary>
        /// Gets a value indicating whether the
        /// <see cref="T:System.Collections.Generic.ICollection`1">
        /// </see> is read-only.
        /// </summary>
        /// <value>
        /// <c>
        /// true
        /// </c> if this instance is read only; otherwise,
        /// <c>
        /// false
        /// </c>.
        /// </value>
        public bool IsReadOnly
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets the
        /// <see cref="T:System.Windows.Forms.ColumnHeader" /> with the specified key.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <returns>
        /// ColumnHeader.
        /// </returns>
        public virtual ColumnHeader this[string key]
        {
            get
            {
                return base.Find(o => o.Name == key);
            }
        }

        /// <summary>
        /// Adds the specified header.
        /// </summary>
        /// <param name="header">
        /// The header.
        /// </param>
        /// <returns>
        /// System.Int32.
        /// </returns>
        public new virtual int Add(ColumnHeader header)
        {
            header._index = Count;
            if (header.DisplayIndex == 0)
                header.DisplayIndex = header._index;
            var index= ((IList)this).Add(header);
            _owner.NativeHeaderAdd(header);

            return index;
        }

        /// <summary>
        /// Adds the specified text.
        /// </summary>
        /// <param name="text">
        /// The text.
        /// </param>
        /// <param name="width">
        /// The width.
        /// </param>
        /// <param name="textAlign">
        /// The text align.
        /// </param>
        /// <returns>
        /// ColumnHeader.
        /// </returns>
        public virtual ColumnHeader Add(string text, int width, HorizontalAlignment textAlign)
        {
            return Add("", text, width, textAlign, "");
        }

        /// <summary>
        /// Adds the specified text.
        /// </summary>
        /// <param name="text">
        /// The text.
        /// </param>
        /// <returns>
        /// ColumnHeader.
        /// </returns>
        public virtual ColumnHeader Add(string text)
        {
            return Add("", text, 60, HorizontalAlignment.Left, "");
        }

        /// <summary>
        /// Adds the specified text.
        /// </summary>
        /// <param name="text">
        /// The text.
        /// </param>
        /// <param name="width">
        /// The width.
        /// </param>
        /// <returns>
        /// ColumnHeader.
        /// </returns>
        public virtual ColumnHeader Add(string text, int width)
        {
            return Add("", text, width, HorizontalAlignment.Left, "");
        }

        /// <summary>
        /// Adds the specified key.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <param name="text">
        /// The text.
        /// </param>
        /// <returns>
        /// ColumnHeader.
        /// </returns>
        public virtual ColumnHeader Add(string key, string text)
        {
            return Add(key, text, 60, HorizontalAlignment.Left, "");
        }

        /// <summary>
        /// Adds the specified key.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <param name="text">
        /// The text.
        /// </param>
        /// <param name="width">
        /// The width.
        /// </param>
        /// <returns>
        /// ColumnHeader.
        /// </returns>
        public virtual ColumnHeader Add(string key, string text, int width)
        {
            return Add(key, text, width, HorizontalAlignment.Left, "");
        }

        /// <summary>
        /// Adds the specified key.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <param name="text">
        /// The text.
        /// </param>
        /// <param name="width">
        /// The width.
        /// </param>
        /// <param name="textAlign">
        /// The text align.
        /// </param>
        /// <param name="imageKey">
        /// The image key.
        /// </param>
        /// <returns>
        /// ColumnHeader.
        /// </returns>
        public virtual ColumnHeader Add(string key, string text, int width, HorizontalAlignment textAlign, string imageKey)
        {
            var header = new ColumnHeader();
            header.Name = key;
            header.Text = text;
            header.Width = width;
            header.TextAlign = textAlign;
            header.ImageKey = imageKey;
            header.ImageIndex = -1;
            this.Add(header);

            return header;
        }

        /// <summary>
        /// Adds the specified key.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <param name="text">
        /// The text.
        /// </param>
        /// <param name="width">
        /// The width.
        /// </param>
        /// <param name="textAlign">
        /// The text align.
        /// </param>
        /// <param name="imageIndex">
        /// Index of the image.
        /// </param>
        /// <returns>
        /// ColumnHeader.
        /// </returns>
        public virtual ColumnHeader Add(string key, string text, int width, HorizontalAlignment textAlign, int imageIndex)
        {
            var header = new ColumnHeader();
            header.Name = key;
            header.Text = text;
            header.Width = width;
            header.TextAlign = textAlign;
            header.ImageIndex = imageIndex;
            this.Add(header);

            return header;
        }

        /// <summary>
        /// Adds the range.
        /// </summary>
        /// <param name="values">
        /// The values.
        /// </param>
        public virtual void AddRange(ColumnHeader[] values)
        {
            foreach (ColumnHeader value in values)
            {
                this.Add(value);
            }
        }

        /// <summary>
        /// Clears this instance.
        /// </summary>
        public new void Clear()
        {
            base.Clear();
            _owner.NativeHeaderClear();
        }

        /// <summary>
        /// Determines whether the specified key contains key.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <returns>
        /// <c>
        /// true
        /// </c> if the specified key contains key; otherwise,
        /// <c>
        /// false
        /// </c>.
        /// </returns>
        public virtual bool ContainsKey(string key)
        {
            return base.Contains(base.Find(o => o.Name == key));
        }

        /// <summary>
        /// Indexes the of key.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <returns>
        /// System.Int32.
        /// </returns>
        public virtual int IndexOfKey(string key)
        {
            return base.FindIndex(o => o.Name == key);
        }

        /// <summary>
        /// Inserts the specified index.
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <param name="header">
        /// The header.
        /// </param>
        public new void Insert(int index, ColumnHeader header)
        {
            header._index = index;
            if (header.DisplayIndex == 0)
                header.DisplayIndex = header._index;

            base.Insert(index, header);
            var idx = 0;
            foreach (var item in this)
            {
                header._index = idx;
                if (header.DisplayIndex == 0)
                    header.DisplayIndex = header._index;
                idx++;
            }
            _owner.NativeHeaderAdd(header);
        }

        /// <summary>
        /// Inserts the specified index.
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <param name="text">
        /// The text.
        /// </param>
        /// <param name="width">
        /// The width.
        /// </param>
        /// <param name="textAlign">
        /// The text align.
        /// </param>
        public void Insert(int index, string text, int width, HorizontalAlignment textAlign)
        {
            Insert(index, "", text, width, textAlign, null);
        }

        /// <summary>
        /// Inserts the specified index.
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <param name="text">
        /// The text.
        /// </param>
        public void Insert(int index, string text)
        {
            Insert(index, "", text, 0, HorizontalAlignment.Center, null);
        }

        /// <summary>
        /// Inserts the specified index.
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <param name="text">
        /// The text.
        /// </param>
        /// <param name="width">
        /// The width.
        /// </param>
        public void Insert(int index, string text, int width)
        {
            Insert(index, "", text, width, HorizontalAlignment.Center, null);
        }

        /// <summary>
        /// Inserts the specified index.
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <param name="text">
        /// The text.
        /// </param>
        public void Insert(int index, string key, string text)
        {
            Insert(index, key, text, 0, HorizontalAlignment.Center, null);

        }

        /// <summary>
        /// Inserts the specified index.
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <param name="text">
        /// The text.
        /// </param>
        /// <param name="width">
        /// The width.
        /// </param>
        public void Insert(int index, string key, string text, int width)
        {
            Insert(index, key, text, width, HorizontalAlignment.Center, null);
        }

        /// <summary>
        /// Inserts the specified index.
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <param name="text">
        /// The text.
        /// </param>
        /// <param name="width">
        /// The width.
        /// </param>
        /// <param name="textAlign">
        /// The text align.
        /// </param>
        /// <param name="imageKey">
        /// The image key.
        /// </param>
        public void Insert(int index, string key, string text, int width, HorizontalAlignment textAlign, string? imageKey)
        {
            var header = new ColumnHeader();
            header._index = index;
            header.DisplayIndex = index;
            header.Name = key;
            header.Text = text;
            header.Width = width;
            header.TextAlign = textAlign;
            header.ImageKey = imageKey;
            Insert(index, header);
        }

        /// <summary>
        /// Inserts the specified index.
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <param name="text">
        /// The text.
        /// </param>
        /// <param name="width">
        /// The width.
        /// </param>
        /// <param name="textAlign">
        /// The text align.
        /// </param>
        /// <param name="imageIndex">
        /// Index of the image.
        /// </param>
        public void Insert(int index, string key, string text, int width, HorizontalAlignment textAlign, int imageIndex)
        {
            var header = new ColumnHeader();
            header._index = index;
            header.DisplayIndex = index;
            header.Name = key;
            header.Text = text;
            header.Width = width;
            header.TextAlign = textAlign;
            header.ImageIndex = imageIndex;
            Insert(index, header);
        }

        /// <summary>
        /// Removes the by key.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        public virtual void RemoveByKey(string key)
        {
            ColumnHeader column = base.Find(o => o.Name == key);
            base.Remove(column);
            _owner.NativeHeaderRemove(column);
        }
    }

    /// <summary>
    /// Class ListViewItemCollection.
    /// Implements the System.Collections.Generic.List&lt;System.Windows.Forms.ListViewItem&gt;
    /// Implements the
    /// <see cref="T:System.Collections.IList" />
    /// </summary>
    /// <seealso cref="T:System.Collections.IList" />
    [ListBindable(false)]
    public class ListViewItemCollection : List<ListViewItem>, IList
    {
        readonly ListView? _owner;

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="T:System.Windows.Forms.ListView.ListViewItemCollection" /> class.
        /// </summary>
        /// <param name="owner">
        /// The owner.
        /// </param>
        public ListViewItemCollection(ListView? owner)
        {
            _owner = owner;
        }

        /// <summary>
        /// Gets the
        /// <see cref="T:System.Windows.Forms.ListViewItem" /> with the specified key.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <returns>
        /// ListViewItem.
        /// </returns>
        public virtual ListViewItem this[string key]
        {
            get
            {
                return base.Find(w => w.Name == key);
            }
        }

        /// <summary>
        /// Adds the specified item.
        /// </summary>
        /// <param name="item">
        /// The object to be added to the end of the
        /// <see cref="T:System.Collections.Generic.List`1">
        /// </see>. The value can be null for reference types.
        /// </param>
        public new void Add(ListViewItem item)
        {
            AddCore(item, -1);
        }

        /// <summary>
        /// Adds the specified text.
        /// </summary>
        /// <param name="text">
        /// The text.
        /// </param>
        /// <returns>
        /// ListViewItem.
        /// </returns>
        public virtual ListViewItem Add(string text)
        {
            return Add("", text, -1);
        }

        /// <summary>
        /// Adds the specified text.
        /// </summary>
        /// <param name="text">
        /// The text.
        /// </param>
        /// <param name="imageIndex">
        /// Index of the image.
        /// </param>
        /// <returns>
        /// ListViewItem.
        /// </returns>
        public virtual ListViewItem Add(string text, int imageIndex)
        {
            return Add("", text, imageIndex);
        }

        /// <summary>
        /// Adds the specified text.
        /// </summary>
        /// <param name="text">
        /// The text.
        /// </param>
        /// <param name="imageKey">
        /// The image key.
        /// </param>
        /// <returns>
        /// ListViewItem.
        /// </returns>
        public virtual ListViewItem Add(string text, string imageKey)
        {
            return Add("", text, imageKey);
        }

        /// <summary>
        /// Adds the specified key.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <param name="text">
        /// The text.
        /// </param>
        /// <param name="imageKey">
        /// The image key.
        /// </param>
        /// <returns>
        /// ListViewItem.
        /// </returns>
        public virtual ListViewItem Add(string key, string text, string imageKey)
        {
            var item = new ListViewItem(text, imageKey);
            item.Name = key;
            item.Text = text;
            AddCore(item, -1);

            return item;
        }

        /// <summary>
        /// Adds the specified key.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <param name="text">
        /// The text.
        /// </param>
        /// <param name="imageIndex">
        /// Index of the image.
        /// </param>
        /// <returns>
        /// ListViewItem.
        /// </returns>
        public virtual ListViewItem Add(string key, string text, int imageIndex)
        {
            var item = new ListViewItem(text, imageIndex);
            item.Name = key;
            AddCore(item, -1);

            return item;
        }

        /// <summary>
        /// Adds the range.
        /// </summary>
        /// <param name="items">
        /// The items.
        /// </param>
        public void AddRange(ListViewItem[] items)
        {
            foreach (ListViewItem item in items)
            {
                AddCore(item, -1);
            }
        }

        /// <summary>
        /// Adds the range.
        /// </summary>
        /// <param name="items">
        /// The items.
        /// </param>
        public void AddRange(ListViewItemCollection items)
        {
            foreach (ListViewItem item in items)
                AddCore(item, -1);
        }

        /// <summary>
        /// Clears this instance.
        /// </summary>
        public new void Clear()
        {
            if (_owner != null)
                _owner.NativeItemsClear();

            foreach (var item in this)
            {
                item.SubItems?.Clear();
            }
            base.Clear();
        }

        /// <summary>
        /// Determines whether the specified key contains key.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <returns>
        /// <c>
        /// true
        /// </c> if the specified key contains key; otherwise,
        /// <c>
        /// false
        /// </c>.
        /// </returns>
        public virtual bool ContainsKey(string key)
        {
            return base.FindIndex(w => w.Name == key) > -1;
        }

        /// <summary>
        /// Copies to.
        /// </summary>
        /// <param name="dest">
        /// The dest.
        /// </param>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <exception cref="T:System.NotImplementedException">
        /// </exception>
        public void CopyTo(Array dest, int index)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Finds the specified key.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <param name="searchAllSubItems">
        /// if set to
        /// <c>
        /// true
        /// </c>search all sub items.
        /// </param>
        /// <returns>
        /// ListViewItem[.
        /// </returns>
        public ListViewItem[] Find(string key, bool searchAllSubItems)
        {
            if (searchAllSubItems)
                return base.FindAll(w => w.Name == key && (w.SubItems?.ContainsKey(key) ?? false)).ToArray();
            else
                return base.FindAll(w => w.Name == key).ToArray();
        }

        /// <summary>
        /// Indexes the of key.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <returns>
        /// System.Int32.
        /// </returns>
        public virtual int IndexOfKey(string key)
        {
            return base.FindIndex(w => w.Name == key);
        }

        /// <summary>
        /// Inserts the specified index.
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <param name="text">
        /// The text.
        /// </param>
        /// <returns>
        /// ListViewItem.
        /// </returns>
        public ListViewItem Insert(int index, string text)
        {
            return Insert(index, "", text, -1);
        }

        /// <summary>
        /// Inserts the specified index.
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <param name="text">
        /// The text.
        /// </param>
        /// <param name="imageIndex">
        /// Index of the image.
        /// </param>
        /// <returns>
        /// ListViewItem.
        /// </returns>
        public ListViewItem Insert(int index, string text, int imageIndex)
        {
            return Insert(index, "", text, imageIndex);
        }

        /// <summary>
        /// Inserts the specified index.
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <param name="text">
        /// The text.
        /// </param>
        /// <param name="imageKey">
        /// The image key.
        /// </param>
        /// <returns>
        /// ListViewItem.
        /// </returns>
        public ListViewItem Insert(int index, string text, string imageKey)
        {
            return Insert(index, "", text, imageKey);
        }

        /// <summary>
        /// Inserts the specified index.
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <param name="text">
        /// The text.
        /// </param>
        /// <param name="imageKey">
        /// The image key.
        /// </param>
        /// <returns>
        /// ListViewItem.
        /// </returns>
        public virtual ListViewItem Insert(int index, string key, string text, string imageKey)
        {
            var item = new ListViewItem(text, imageKey);
            item.Name = key;
            base.Insert(index, item);

            return item;
        }

        /// <summary>
        /// Inserts the specified index.
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <param name="text">
        /// The text.
        /// </param>
        /// <param name="imageIndex">
        /// Index of the image.
        /// </param>
        /// <returns>
        /// ListViewItem.
        /// </returns>
        public virtual ListViewItem Insert(int index, string key, string text, int imageIndex)
        {
            var item = new ListViewItem(text, imageIndex);
            item.Name = key;
            base.Insert(index, item);

            return item;
        }

        /// <summary>
        /// Removes the specified item from the collection and from the underlying view.
        /// </summary>
        /// <param name="item">
        /// The item.
        /// </param>
        public new void Remove(ListViewItem item)
        {
            if (item != null)
            {
                if (item._flowBoxChild?.Parent is Gtk.FlowBox flow)
                {
                    flow.Remove(item._flowBoxChild);
                    item._flowBoxChild.Destroy();
                }
                base.Remove(item);
            }
        }

        /// <summary>
        /// Removes the item at the specified index.
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        public new void RemoveAt(int index)
        {
            if (index < Count && index >= 0)
            {
                var item = this[index];
                Remove(item);
            }
        }

        /// <summary>
        /// Removes the by key.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        public virtual void RemoveByKey(string key)
        {
            ListViewItem item = base.Find(w => w.Name == key);
            if (item != null)
                Remove(item);
        }

        private void AddCore(ListViewItem item, int position)
        {
            item._listView = _owner;
            item.Index = Count;
            if (item.Group == null)
                item.Group = _owner?.DefaultGroup;

            base.Add(item);
            _owner?.NativeAdd(item, position);
        }
    }

    /// <summary>
    /// Class SelectedIndexCollection.
    /// Implements the System.Collections.Generic.List&lt;System.Int32&gt;
    /// </summary>
    /// System.Collections.Generic.List&lt;System.Int32&gt;
    ///
    ///
    ///
    ///
    [ListBindable(false)]
    public class SelectedIndexCollection : List<int>
    {

    }

    /// <summary>
    /// Class SelectedListViewItemCollection.
    /// Implements the System.Collections.Generic.List&lt;System.Windows.Forms.ListViewItem&gt;
    /// </summary>
    [ListBindable(false)]
    public class SelectedListViewItemCollection : List<ListViewItem>
    {

    }
}
