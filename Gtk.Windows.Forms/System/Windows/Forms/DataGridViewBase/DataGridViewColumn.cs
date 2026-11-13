/*
 * Based on GTK components, it is a cross-platform interface component that is compatible with the native C# control winform interface.
 * Use this component Gtk.Windows.Forms instead of Microsoft.WindowsDesktop.App.WindowsForms, compile once, and run across Windows, Linux, and macOS
 * Technical support 438865652@qq.com，https://www.gtkapp.com, https://gitee.com/easywebfactory, https://github.com/easywebfactory438865652@qq.com，https://www.gtkapp.com, https://gitee.com/easywebfactory, https://github.com/easywebfactory
 * author:chenhongjin
 */

using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms.GtkRender;
using Gtk;

namespace System.Windows.Forms;

/// <summary>
/// Represents a column in a
/// <see cref="T:System.Windows.Forms.DataGridView" /> control.
/// </summary>
/// <filterpriority>
/// 2
/// </filterpriority>
public class DataGridViewColumn : TreeViewColumn
{

    /// <summary>
    /// The editable widget currently used to edit a cell of this column, if any.
    /// </summary>
    public ICellEditable? CellEditable;

    internal string? _cellEditablePath;

    internal bool _cellEditing;

    internal ICellRenderer? _cellRenderer;

    internal DataGridViewCell _cellTemplate;

    internal DataGridView? _gridview;

    internal Gtk.TreeView? _treeView;

    private readonly Gtk.CssProvider css = new();

    private DataGridViewAutoSizeColumnMode _AutoSizeMode;

    private string _DataPropertyName = string.Empty;

    private DataGridViewCellStyle? _DefaultCellStyle;

    //protected override void Dispose(bool disposing) {  }

    private int _index = -1;

    private string? _markup;

    private bool _ReadOnly;

    private DataGridViewColumnSortMode _SortMode = DataGridViewColumnSortMode.Automatic;

    /// <summary>
    /// The disposed.
    /// </summary>
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public virtual event EventHandler? Disposed;

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.DataGridViewColumn" /> class using the specified owner and a default cell template.
    /// </summary>
    /// <param name="ownerGridView">
    /// The owning grid view.
    /// </param>
    public DataGridViewColumn(DataGridView ownerGridView) : this(ownerGridView, new DataGridViewTextBoxCell())
    {
    }

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.DataGridViewColumn" /> class with no owner.
    /// </summary>
    public DataGridViewColumn() : this(null, new DataGridViewTextBoxCell())
    {
    }

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.DataGridViewColumn" /> class using the provided cell template.
    /// </summary>
    /// <param name="cellTemplate">
    /// The template cell to use for new cells.
    /// </param>
    public DataGridViewColumn(DataGridViewCell cellTemplate) : this(null, cellTemplate)
    {
    }

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.DataGridViewColumn" /> class with explicit column index, owner and cell template.
    /// </summary>
    /// <param name="ownerGridView">
    /// Owning grid view.
    /// </param>
    /// <param name="cellTemplate">
    /// Template cell to use.
    /// </param>
    public DataGridViewColumn(DataGridView? ownerGridView, DataGridViewCell cellTemplate) : this(0, ownerGridView, cellTemplate)
    {
        var renderer = new CellRendererValue(this)
        {
            Editable = true,
            Mode = CellRendererMode.Editable
        };
        renderer.Edited += Renderer_Edited;
        renderer.EditingStarted += Renderer_EditingStarted;
        renderer.PlaceholderText = "";
        renderer.Height = RowHeight;
        base.PackStart(renderer, true);
        _cellRenderer = renderer as ICellRenderer;
    }

    /// <summary>
    /// Protected constructor used by derived column implementations.
    /// </summary>
    /// <param name="column">
    /// Column index.
    /// </param>
    /// <param name="ownerGridView">
    /// Owning grid view.
    /// </param>
    /// <param name="cellTemplate">
    /// Template cell.
    /// </param>
    protected DataGridViewColumn(int column, DataGridView? ownerGridView, DataGridViewCell cellTemplate) : base()
    {
        if (ownerGridView != null)
        {
            _gridview = ownerGridView;
            _treeView = ownerGridView.GridView;
        }
        _cellTemplate = cellTemplate;
        base.Resizable = true;
        base.Sizing = TreeViewColumnSizing.GrowOnly;
    }

    /// <summary>
    /// Gets or sets the auto-size mode for the column.
    /// </summary>
    [DefaultValue(DataGridViewAutoSizeColumnMode.NotSet)]
    [RefreshProperties(RefreshProperties.Repaint)]
    public DataGridViewAutoSizeColumnMode AutoSizeMode
    {
        get => _AutoSizeMode;
        set
        {
            // The backing field was only assigned in the None branch, so every other mode was dropped
            // and AutoSizeMode always read back as NotSet.
            _AutoSizeMode = value;
            if (value == DataGridViewAutoSizeColumnMode.None) { base.Resizable = false; base.Sizing = TreeViewColumnSizing.Fixed; } else { base.Resizable = true; base.Sizing = TreeViewColumnSizing.GrowOnly; }
        }
    }

    /// <summary>
    /// Gets or sets the template cell used when creating new cells for this column.
    /// </summary>
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public virtual DataGridViewCell CellTemplate
    {
        get => _cellTemplate;
        set => _cellTemplate = value;
    }

    /// <summary>
    /// Gets the CLR type of the cells in this column.
    /// </summary>
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public virtual Type CellType { get { return _cellTemplate.GetType(); } }

    /// <summary>
    /// Gets or sets the context menu strip for the column header and cells.
    /// </summary>
    [DefaultValue(null)]
    public ContextMenuStrip? ContextMenuStrip
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the
    /// <see cref="T:System.Windows.Forms.DataGridViewColumn.DataGridView" /> that contains this column.
    /// </summary>
    public DataGridView? DataGridView
    {
        get { return _gridview; }
        set { _gridview = value; _treeView = value?.GridView; }
    }

    /// <summary>
    /// Gets or sets the name of the data property bound to the column.
    /// </summary>
    [Browsable(true)]
    [DefaultValue("")]
    public string DataPropertyName
    {
        get => _DataPropertyName;
        set => _DataPropertyName = value;
    }

    /// <summary>
    /// Gets or sets the default cell style applied to cells in this column.
    /// </summary>
    [Browsable(true)]
    public DataGridViewCellStyle? DefaultCellStyle
    {
        get => _DefaultCellStyle;
        set { _DefaultCellStyle = value; _cellRenderer?.ColumnStyle = value; SetDefaultStyle(_DefaultCellStyle); }
    }

    /// <summary>
    /// Gets or sets the display index of the column.
    /// </summary>
    [Browsable(false)]
    public int DisplayIndex
    {
        get => _treeView == null ? -1 : Array.IndexOf(_treeView.Columns, (TreeViewColumn)this);
        set
        {
            if (_gridview != null && _treeView != null && value >= 0 && _gridview.Columns.Count > value && this.Handle != _treeView.Columns[value].Handle)
            {
                // Moving after the column at [value] left this column at value + 1, so
                // DisplayIndex = 0 was unreachable; the head position needs a null base column.
                _treeView.MoveColumnAfter((TreeViewColumn)this, value == 0 ? null : _treeView.Columns[value - 1]);
            }
        }
    }

    /// <summary>
    /// Gets or sets the width of the divider that follows the column.
    /// </summary>
    [DefaultValue(0)]
    public int DividerWidth
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the relative fill weight for column sizing when using fill mode.
    /// </summary>
    [DefaultValue(100)]
    public float FillWeight
    {
        get;
        set;
    } = 100;

    /// <summary>
    /// Gets or sets a value indicating whether the column is frozen.
    /// </summary>
    [DefaultValue(false)]
    [RefreshProperties(RefreshProperties.All)]
    public bool Frozen
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the header cell for the column.
    /// </summary>
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public DataGridViewColumnHeaderCell? HeaderCell
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the header text displayed for the column.
    /// </summary>
    [DefaultValue("")]
    public string HeaderText
    {
        get { return this.Title; }
        set { this.Title = value; }
    }

    /// <summary>
    /// Gets or sets the runtime index of the column in the grid.
    /// </summary>
    public int Index
    {
        get => _index;

        internal set
        {
            if (_index == -1)
                base.AddAttribute(base.Cells[0], "cellvalue", 0);
            _index = value;
            if (_SortMode != DataGridViewColumnSortMode.NotSortable)
            {
                base.SortColumnId = _index;
            }
        }
    }

    /// <summary>
    /// Gets the inherited auto-size mode for the column.
    /// </summary>
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public DataGridViewAutoSizeColumnMode InheritedAutoSizeMode { get; }

    /// <summary>
    /// Gets the inherited cell style for the column.
    /// </summary>
    [Browsable(false)]
    public DataGridViewCellStyle? InheritedStyle { get; }

    /// <summary>
    /// Gets a value indicating whether the column is data-bound.
    /// </summary>
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool IsDataBound { get; }

    /// <summary>
    /// Gets or sets the Pango markup used for rendering text in this column.
    /// </summary>
    public string? Markup
    {
        get => _markup;
        set { _markup = value; _cellRenderer?.Markup = value; }
    }

    /// <summary>
    /// Gets or sets the minimum width for the column.
    /// </summary>
    [DefaultValue(5)]
    [Localizable(true)]
    [RefreshProperties(RefreshProperties.Repaint)]
    public int MinimumWidth
    {
        get => base.MinWidth;
        set { base.MinWidth = value; }
    }

    /// <summary>
    /// Gets or sets the name associated with the column.
    /// </summary>
    [DefaultValue("")]
    public string Name
    {
        get => base.Button.Name;
        set { base.Button.Name = value; }
    }

    /// <summary>
    /// Gets or sets a value indicating whether the column is read-only.
    /// </summary>
    public bool ReadOnly
    {
        get => _ReadOnly;
        set
        {
            _ReadOnly = value;
            if (_cellRenderer != null)
            {
                _cellRenderer.Editable = !_ReadOnly;
                _cellRenderer.Activatable = !_ReadOnly;
            }
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether the column is resizable.
    /// </summary>
    public new DataGridViewTriState Resizable
    {
        get { return base.Resizable == true ? DataGridViewTriState.True : DataGridViewTriState.False; }
        set { base.Resizable = value == DataGridViewTriState.True; }
    }

    /// <summary>
    /// Gets the height used for rows in this column.
    /// </summary>
    public int RowHeight
    {
        get
        {
            if (_gridview != null)
            {
                return _gridview.RowTemplate.Height;
            }

            return DefaultHeight;
        }
    }

    /// <summary>
    /// Gets or sets the site for the column when used in a designer.
    /// </summary>
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public ISite? Site
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the sort mode for the column.
    /// </summary>
    public DataGridViewColumnSortMode SortMode
    {
        get => _SortMode;
        set
        {
            _SortMode = value;
            if (value == DataGridViewColumnSortMode.NotSortable)
                base.SortColumnId = -1;
            else
                base.SortColumnId = _index;
        }
    }

    /// <summary>
    /// Gets the current element state for the column.
    /// </summary>
    public DataGridViewElementStates State
    {
        get { return DataGridViewElementStates.None; }
        internal set { }
    }

    /// <summary>
    /// Gets or sets the tooltip text for the column header.
    /// </summary>
    [DefaultValue("")]
    [Localizable(true)]
    public string? ToolTipText
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the type of values stored in the column's cells.
    /// </summary>
    //[DefaultValue(true)]
    //[Localizable(true)]
    //public bool Visible { get; set; }
    [Browsable(false)]
    [DefaultValue(null)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public Type ValueType
    {
        get;
        set;
    } = typeof(string);

    /// <summary>
    /// Gets or sets the width of the column in pixels.
    /// </summary>
    [Localizable(true)]
    [RefreshProperties(RefreshProperties.Repaint)]
    public new int Width
    {
        get { return base.FixedWidth; }
        set { base.FixedWidth = value; _cellRenderer?.WidthChars = value; }
    }

    private int DefaultHeight
    {
        get
        {
            if (_treeView != null)
            {
                var size = _treeView.PangoContext.FontDescription.Size / Pango.Scale.PangoScale;
                return (int)size + 16;
            }

            return 28;
        }
    }

    /// <summary>
    /// Creates a shallow clone of the column.
    /// </summary>
    /// <returns>
    /// A new column of the same runtime type carrying this column's configuration.
    /// </returns>
    public object? Clone()
    {
        // This returned null, so any code that duplicates columns crashed.
        var clone = (DataGridViewColumn)Activator.CreateInstance(GetType())!;
        clone.Name = Name;
        clone.HeaderText = HeaderText;
        clone.DataPropertyName = DataPropertyName;
        clone.ValueType = ValueType;
        clone.Width = Width;
        clone.MinimumWidth = MinimumWidth;
        clone.ReadOnly = ReadOnly;
        clone.Frozen = Frozen;
        clone.SortMode = SortMode;
        clone.ToolTipText = ToolTipText;
        clone.DefaultCellStyle = DefaultCellStyle;

        return clone;
    }

    /// <summary>
    /// Finishes the pending edit operation, if any, and releases the editing widget.
    /// </summary>
    public void EditableEditingDone()
    {
        if (CellEditable != null)
        {
            try
            {
                CellEditable.EditingDone += Editable_EditingDone;
                CellEditable.FinishEditing();
                CellEditable.EditingDone -= Editable_EditingDone;
            }
            catch (Exception ex)
            {
                System.Diagnostics.GtkLogger.Debug($"Unexpected exception in EditableEditingDone", ex);
            }
            finally
            {
                CellEditable?.RemoveWidget();
                CellEditable = null;
                _cellEditing = false;
            }
        }
    }

    /// <summary>
    /// Returns an estimate of the preferred width for the column.
    /// </summary>
    /// <param name="autoSizeColumnMode">
    /// The auto-size mode.
    /// </param>
    /// <param name="fixedHeight">
    /// True to compute for a fixed height; otherwise false.
    /// </param>
    /// <returns>
    /// An integer representing the preferred width in pixels.
    /// </returns>
    public virtual int GetPreferredWidth(DataGridViewAutoSizeColumnMode autoSizeColumnMode, bool fixedHeight)
    {
        return RowHeight;
    }

    /// <summary>
    /// Commits the value edited in the cell identified by the specified tree path.
    /// </summary>
    /// <param name="treepath">
    /// The GTK tree path of the edited row.
    /// </param>
    /// <param name="value">
    /// The new value entered by the user.
    /// </param>
    public void RendererEdited(string treepath, string value)
    {
        _cellEditing = false;
        var path = new TreePath(treepath);
        var model = _treeView?.Model;
        if (model != null)
        {
            model.GetIter(out var iter, path);
            var row = model.GetValue(iter, 0);
            if (row is DataGridViewRow val)
            {
                val.Cells[Index].Value = value;
                _gridview?.CellValueChanagedHandler(Index, iter, path);
            }
        }
        this.QueueResize();
    }

    /// <summary>
    /// Applies the provided default
    /// <see cref="T:System.Windows.Forms.DataGridViewCellStyle" /> to the column renderer.
    /// </summary>
    /// <param name="cellStyle">
    /// The default style to apply.
    /// </param>
    public void SetDefaultStyle(DataGridViewCellStyle? cellStyle)
    {
        if (cellStyle != null)
        {
            if (cellStyle.WrapMode == DataGridViewTriState.True)
            {
                if (_cellRenderer != null)
                {
                    _cellRenderer.WrapMode = Pango.WrapMode.WordChar;
                    _cellRenderer.WrapWidth = Width;
                }
            }
        }
    }

    /// <summary>
    /// Refreshes the renderer and the column header appearance from the current styles of the owning grid.
    /// </summary>
    public void UpdateDefaultStyle()
    {
        if (_gridview != null)
        {
            if (_cellRenderer != null)
            {
                if (_ReadOnly || _gridview.ReadOnly)
                {
                    _cellRenderer.Editable = false;
                    _cellRenderer.Activatable = false;
                }
                else
                {
                    _cellRenderer.Editable = true;
                    _cellRenderer.Activatable = true;
                }
                if (_gridview.AutoSizeRowsMode == DataGridViewAutoSizeRowsMode.AllCells || _gridview.AutoSizeRowsMode == DataGridViewAutoSizeRowsMode.DisplayedCells || _gridview.AutoSizeRowsMode == DataGridViewAutoSizeRowsMode.AllCellsExceptHeaders || _gridview.AutoSizeRowsMode == DataGridViewAutoSizeRowsMode.DisplayedCellsExceptHeaders)
                {
                    _cellRenderer.Height = -1;
                }
                else
                {
                    _cellRenderer.Height = RowHeight;
                }
                if (_gridview.DefaultCellStyle != null && _gridview.DefaultCellStyle.WrapMode == DataGridViewTriState.True)
                {
                    _cellRenderer.WrapMode = Pango.WrapMode.WordChar;
                    _cellRenderer.WrapWidth = Width;
                }
            }
            if (_gridview.GridView.IsRealized)
                this.QueueResize();

            var _cellStyle = this.DefaultCellStyle;
            if (_cellStyle == null && _gridview.ColumnHeadersDefaultCellStyle != null)
                _cellStyle = _gridview.ColumnHeadersDefaultCellStyle;

            if (_cellStyle != null)
            {
                var header = ((Gtk.Button)this.Button);
                var style = new StringBuilder();
                if (_cellStyle.BackColor.Name != "0")
                {
                    var backColor = _cellStyle.BackColor;
                    var color = $"rgba({backColor.R},{backColor.G},{backColor.B},{backColor.A})";
                    style.AppendFormat("background-color:{0};background:{0};", color);
                }

                if (_cellStyle.ForeColor.Name != "0")
                {
                    var foreColor = _cellStyle.ForeColor;
                    var color = $"rgba({foreColor.R},{foreColor.G},{foreColor.B},{foreColor.A})";
                    style.AppendFormat("color:{0};", color);
                }
                if (_cellStyle.Font != null)
                {
                    var font = _cellStyle.Font;
                    if (font.Unit == GraphicsUnit.Pixel)
                        style.AppendFormat("font-size:{0}px;", font.Size);
                    else if (font.Unit == GraphicsUnit.Inch)
                        style.AppendFormat("font-size:{0}in;", font.Size);
                    else if (font.Unit == GraphicsUnit.Point)
                        style.AppendFormat("font-size:{0}pt;", font.Size);
                    else if (font.Unit == GraphicsUnit.Millimeter)
                        style.AppendFormat("font-size:{0}mm;", font.Size);
                    else if (font.Unit == GraphicsUnit.Document)
                        style.AppendFormat("font-size:{0}cm;", font.Size);
                    else if (font.Unit == GraphicsUnit.Display)
                        style.AppendFormat("font-size:{0}pc;", font.Size);
                    else
                        style.AppendFormat("font-size:{0}pt;", font.Size);

                    if (string.IsNullOrWhiteSpace(font.FontFamily?.Name) == false)
                    {
                        style.AppendFormat("font-family:\"{0}\";", font.FontFamily?.Name);
                    }
                    if (font.Bold)
                    {
                        style.Append("font-weight:bold;");
                    }
                    if (font.Italic)
                    {
                        style.Append("font-style:italic;");
                    }
                    if (font.Underline)
                    {
                        style.Append("text-decoration:underline;");
                    }
                    if (font.Strikeout)
                    {
                        style.Append("text-decoration:line-through;");
                    }
                }

                if (style.Length > 9)
                {
                    css.LoadFromData($".columnheadercell{{{style.ToString()}}}");
                    if (header.StyleContext.HasClass("columnheadercell"))
                    {
                        header.StyleContext.RemoveClass("columnheadercell");
                        header.StyleContext.RemoveProvider(css);
                    }
                    header.StyleContext.AddProvider(css, StyleProviderPriority.User);
                    header.StyleContext.AddClass("columnheadercell");
                }
                switch (_cellStyle.Alignment)
                {
                    case DataGridViewContentAlignment.TopLeft:
                    case DataGridViewContentAlignment.MiddleLeft:
                    case DataGridViewContentAlignment.BottomLeft:
                        this.Alignment = 0f;
                        break;

                    case DataGridViewContentAlignment.TopCenter:
                    case DataGridViewContentAlignment.MiddleCenter:
                    case DataGridViewContentAlignment.BottomCenter:
                        this.Alignment = 0.5f;
                        break;

                    case DataGridViewContentAlignment.TopRight:
                    case DataGridViewContentAlignment.MiddleRight:
                    case DataGridViewContentAlignment.BottomRight:
                        this.Alignment = 1.0f;
                        break;

                    default:
                        this.Alignment = 0f;
                        break;
                }
            }
        }
        SetDefaultStyle(this.DefaultCellStyle);
    }

    /// <summary>
    /// Returns the name of the type.
    /// </summary>
    /// <returns>
    /// Type name string.
    /// </returns>
    public override string ToString() { return this.GetType().Name; }

    internal void AtrributesClone(DataGridViewCell newcell)
    {
        newcell.Style = _cellTemplate.Style?.Clone();
        newcell.OwningRow = _cellTemplate.OwningRow;
        newcell.ReadOnly = _cellTemplate.ReadOnly;
    }

    internal virtual DataGridViewCell? NewCell(object? value = null, Type? valueType = null)
    {
        var newcell = new DataGridViewTextBoxCell();
        AtrributesClone(newcell);
        newcell.Value = value;
        newcell.ValueType = valueType ?? typeof(string);

        return newcell;
    }

    /// <summary>
    /// Raises the Disposed event.
    /// </summary>
    protected virtual void OnDisposed(System.EventArgs e)
    {
        Disposed?.Invoke(this, e);
    }

    private void Editable_EditingDone(object? sender, EventArgs e)
    {
        if (_cellEditing == true && _cellEditablePath != null)
        {
            if (sender is Gtk.Entry enry)
            {
                RendererEdited(_cellEditablePath, enry.Text);
            }
            else if (sender is Gtk.ComboBox com)
            {
                RendererEdited(_cellEditablePath, com.Entry.Text);
            }
        }
    }

    private void Renderer_Edited(object o, EditedArgs args)
    {
        RendererEdited(args.Path, args.NewText);
    }

    private void Renderer_EditingStarted(object o, EditingStartedArgs args)
    {
        _cellEditing = true;
        _cellEditablePath = args.Path;
        CellEditable = args.Editable;
    }
}