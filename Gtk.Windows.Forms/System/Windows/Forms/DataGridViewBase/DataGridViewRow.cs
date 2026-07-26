using Gtk;

namespace System.Windows.Forms;

/// <summary>
/// Represents a row in a
/// <see cref="T:System.Windows.Forms.DataGridView" /> control.
/// </summary>
/// <filterpriority>
/// 2
/// </filterpriority>
public class DataGridViewRow : DataGridViewBand
{

    private readonly DataGridViewCellCollection _cell;

    private DataGridViewRowCollection? _children;

    private bool _visible = true;

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.DataGridViewRow" /> class.
    /// </summary>
    public DataGridViewRow() : this(null)
    {
    }

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.DataGridViewRow" /> class.
    /// </summary>
    /// <param name="databounditem">
    /// The data bound item.
    /// </param>
    public DataGridViewRow(object? databounditem)
    {
        _cell = new DataGridViewCellCollection(this);
        DataBoundItem = databounditem;
    }

    /// <summary>
    /// Gets the accessibility object.
    /// </summary>
    /// <value>
    /// The accessibility object.
    /// </value>
    public AccessibleObject? AccessibilityObject { get; }

    /// <summary>
    /// Gets the cells.
    /// </summary>
    /// <value>
    /// The cells.
    /// </value>
    public DataGridViewCellCollection Cells { get { return _cell; } }

    /// <summary>
    /// Gets the children.
    /// </summary>
    /// <value>
    /// The children.
    /// </value>
    public DataGridViewRowCollection Children
    {
        get
        {
            if (_children == null) { _children = new DataGridViewRowCollection(DataGridView, this); }

            return _children;
        }
    }

    /// <summary>
    /// Gets or sets the context menu strip.
    /// </summary>
    /// <value>
    /// The context menu strip.
    /// </value>
    public override ContextMenuStrip? ContextMenuStrip
    {
        get;
        set;
    }

    /// <summary>
    /// Gets the data bound item.
    /// </summary>
    /// <value>
    /// The data bound item.
    /// </value>
    public object? DataBoundItem { get; }

    /// <summary>
    /// Gets or sets the data grid view.
    /// </summary>
    /// <value>
    /// The data grid view.
    /// </value>
    public override DataGridView? DataGridView
    {
        get;
        internal set;
    }

    /// <summary>
    /// Gets or sets the default cell style.
    /// </summary>
    /// <value>
    /// The default cell style.
    /// </value>
    public override DataGridViewCellStyle? DefaultCellStyle
    {
        get;
        set;
    }

    /// <summary>
    /// Gets a value indicating whether this
    /// <see cref="T:System.Windows.Forms.DataGridViewRow" /> is displayed.
    /// </summary>
    /// <value>
    /// <c>
    /// true
    /// </c> if displayed; otherwise,
    /// <c>
    /// false
    /// </c>.
    /// </value>
    public override bool Displayed { get; }

    /// <summary>
    /// Gets or sets the height of the divider.
    /// </summary>
    /// <value>
    /// The height of the divider.
    /// </value>
    public int DividerHeight
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the error text.
    /// </summary>
    /// <value>
    /// The error text.
    /// </value>
    public string? ErrorText
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets a value indicating whether this
    /// <see cref="T:System.Windows.Forms.DataGridViewRow" /> is frozen.
    /// </summary>
    /// <value>
    /// <c>
    /// true
    /// </c> if frozen; otherwise,
    /// <c>
    /// false
    /// </c>.
    /// </value>
    public override bool Frozen
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the header cell.
    /// </summary>
    /// <value>
    /// The header cell.
    /// </value>
    public DataGridViewRowHeaderCell? HeaderCell
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the height.
    /// </summary>
    /// <value>
    /// The height.
    /// </value>
    public int Height
    {
        get;
        set;
    } = 29;

    /// <summary>
    /// Gets the index.
    /// </summary>
    /// <value>
    /// The index.
    /// </value>
    public override int Index
    {
        get
        {
            if (Parent == null)
            {
                return DataGridView?.Rows.IndexOf(this) ?? -1;
            }
            else
            {
                return Parent.Children.IndexOf(this);
            }
        }
    }

    /// <summary>
    /// Gets the inherited style.
    /// </summary>
    /// <value>
    /// The inherited style.
    /// </value>
    public override DataGridViewCellStyle? InheritedStyle { get; }

    /// <summary>
    /// Gets a value indicating whether this instance is new row.
    /// </summary>
    /// <value>
    /// <c>
    /// true
    /// </c> if this instance is new row; otherwise,
    /// <c>
    /// false
    /// </c>.
    /// </value>
    public bool IsNewRow { get; }

    /// <summary>
    /// Gets or sets the minimum height.
    /// </summary>
    /// <value>
    /// The minimum height.
    /// </value>
    public int MinimumHeight
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the parent row of this row.
    /// </summary>
    /// <value>
    /// The parent row, or
    /// <c>
    /// null
    /// </c> when this row is a top level row.
    /// </value>
    public DataGridViewRow? Parent
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets a value indicating whether read only.
    /// </summary>
    /// <value>
    /// <c>
    /// true
    /// </c> if read only; otherwise,
    /// <c>
    /// false
    /// </c>.
    /// </value>
    public override bool ReadOnly
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the resizable.
    /// </summary>
    /// <value>
    /// The resizable.
    /// </value>
    public override DataGridViewTriState Resizable
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets a value indicating whether this
    /// <see cref="T:System.Windows.Forms.DataGridViewRow" /> is selected.
    /// </summary>
    /// <value>
    /// <c>
    /// true
    /// </c> if selected; otherwise,
    /// <c>
    /// false
    /// </c>.
    /// </value>
    public override bool Selected
    {
        get => DataGridView?.NativeRowGetSelected(TreeIter) ?? false;
        set => DataGridView?.NativeRowSetSelected(TreeIter, value);
    }

    /// <summary>
    /// Gets the state.
    /// </summary>
    /// <value>
    /// The state.
    /// </value>
    public override DataGridViewElementStates State
    {
        // This always returned None, so every state-filtered operation on the row collection
        // (GetFirstRow, GetRowCount, GetRowsHeight, ...) matched nothing.
        get
        {
            var state = DataGridViewElementStates.None;
            if (Displayed)
            {
                state |= DataGridViewElementStates.Displayed;
            }

            if (Frozen)
            {
                state |= DataGridViewElementStates.Frozen;
            }

            if (ReadOnly)
            {
                state |= DataGridViewElementStates.ReadOnly;
            }

            if (Resizable == DataGridViewTriState.True)
            {
                state |= DataGridViewElementStates.Resizable;
            }

            if (Selected)
            {
                state |= DataGridViewElementStates.Selected;
            }

            if (Visible)
            {
                state |= DataGridViewElementStates.Visible;
            }

            return state;
        }

        internal set { }
    }

    /// <summary>
    /// Gets the tree iter.
    /// </summary>
    /// <value>
    /// The tree iter.
    /// </value>
    public TreeIter TreeIter
    {
        get;
        internal set;
    }

    /// <summary>
    /// Gets or sets a value indicating whether this
    /// <see cref="T:System.Windows.Forms.DataGridViewRow" /> is visible.
    /// </summary>
    /// <value>
    /// <c>
    /// true
    /// </c> if visible; otherwise,
    /// <c>
    /// false
    /// </c>.
    /// </value>
    public override bool Visible
    {
        get => _visible;
        set
        {
            if (value != _visible && DataGridView != null)
            {
                if (DataGridView.UseModelFilter && DataGridView.GridView.Model is TreeModelFilter filter)
                {
                    // this mode column does not support sorting
                    _visible = value;
                    filter.Refilter();
                }
                else
                {
                    // using this mode, the column supports sorting
                    if (value == false)
                    {
                        var iter = TreeIter;
                        if (Gtk.TreeIter.Zero.Equals(TreeIter) == false && TreeIter.UserData != IntPtr.Zero)
                            DataGridView.Store.Remove(ref iter);
                        TreeIter = Gtk.TreeIter.Zero;
                    }
                    else if (Gtk.TreeIter.Zero.Equals(TreeIter))
                    {
                        var idx = Index + 1;
                        var count = DataGridView.Rows.Count;
                        for (var i = idx; i < count; i++)
                        {
                            var row = DataGridView.Rows[i];
                            if (row.Visible)
                            {
                                var path = DataGridView.Store.GetPath(row.TreeIter);
                                if (path != null)
                                    DataGridView.Rows.InsertRowsStore(path.Indices.Last(), this);
                                break;
                            }
                        }
                    }
                    _visible = value;
                }
            }
        }
    }

    /// <summary>
    /// Gets the height.
    /// </summary>
    /// <param name="index">
    /// The index.
    /// </param>
    /// <returns>
    /// </returns>
    public int GetHeight(int index)
    {
        return Height;
    }
}