using Gtk;

namespace System.Windows.Forms;

/// <summary>
/// Represents a row in a
/// <see cref="T:System.Windows.Forms.DataGridView" /> control.
/// </summary>
/// <filterpriority>
/// 2
/// </filterpriority>
public class DataGridViewRow
{

    private readonly DataGridViewCellCollection _cell;

    private readonly DataGridViewRowCollection _children;

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.DataGridViewRow" /> class.
    /// </summary>
    public DataGridViewRow()
    {
        _cell = new DataGridViewCellCollection(this);
    }

    /// <summary>
    /// Gets the accessibility object.
    /// </summary>
    /// <value>
    /// The accessibility object.
    /// </value>
    public AccessibleObject AccessibilityObject { get; }

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
    public DataGridViewRowCollection Children { get => _children ?? new DataGridViewRowCollection(DataGridView, this); }

    /// <summary>
    /// Gets or sets the context menu strip.
    /// </summary>
    /// <value>
    /// The context menu strip.
    /// </value>
    public ContextMenuStrip ContextMenuStrip
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
    public object DataBoundItem { get; }

    /// <summary>
    /// Gets or sets the data grid view.
    /// </summary>
    /// <value>
    /// The data grid view.
    /// </value>
    public DataGridView DataGridView
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the default cell style.
    /// </summary>
    /// <value>
    /// The default cell style.
    /// </value>
    public DataGridViewCellStyle DefaultCellStyle
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
    public bool Displayed { get; }

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
    public string ErrorText
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
    public bool Frozen
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
    public DataGridViewRowHeaderCell HeaderCell
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
    } = 28;

    /// <summary>
    /// Gets the index.
    /// </summary>
    /// <value>
    /// The index.
    /// </value>
    public int Index
    {
        get;
        internal set;
    }

    /// <summary>
    /// Gets the inherited style.
    /// </summary>
    /// <value>
    /// The inherited style.
    /// </value>
    public DataGridViewCellStyle InheritedStyle { get; }

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
    public bool ReadOnly
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
    public DataGridViewTriState Resizable
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
    public bool Selected
    {
        get => DataGridView.NativeRowGetSelected(Index);
        set => DataGridView.NativeRowSetSelected(Index, value);
    }

    /// <summary>
    /// Gets the state.
    /// </summary>
    /// <value>
    /// The state.
    /// </value>
    //public bool Selected { get; set; }
    public DataGridViewElementStates State { get { return DataGridViewElementStates.None; } }

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
    public bool Visible
    {
        get;
        set;
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