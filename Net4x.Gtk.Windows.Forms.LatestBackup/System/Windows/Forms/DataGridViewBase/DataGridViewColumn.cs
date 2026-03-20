/*
 * Based on GTK components, it is a cross-platform interface component that is compatible with the native C# control winform interface.
 * Use this component GtkSystem.Windows.Forms instead of Microsoft.WindowsDesktop.App.WindowsForms, compile once, and run across Windows, Linux, and macOS
 * Technical support 438865652@qq.com，https://www.gtkapp.com, https://gitee.com/easywebfactory, https://github.com/easywebfactory438865652@qq.com，https://www.gtkapp.com, https://gitee.com/easywebfactory, https://github.com/easywebfactory
 * author:chenhongjin
 */
using System.Collections;
using System.ComponentModel;
using System.Reflection;
using System.Windows.Forms.GtkRender;
using Gtk;

namespace System.Windows.Forms;

/// <summary>
/// Hosts a collection of
/// <see cref="T:System.Windows.Forms.DataGridViewButtonCell" /> objects.
/// </summary>
/// <filterpriority>
/// 2
/// </filterpriority>
public class DataGridViewButtonColumn : DataGridViewColumn
{
    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.DataGridViewButtonColumn" /> class.
    /// </summary>
    public DataGridViewButtonColumn() : this(null)
    {
    }

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.DataGridViewButtonColumn" /> class attached to the specified grid.
    /// </summary>
    /// <param name="ownerGridView">
    /// The owning
    /// <see cref="T:System.Windows.Forms.DataGridView" />.
    /// </param>
    public DataGridViewButtonColumn(DataGridView ownerGridView) : base(0, ownerGridView, new DataGridViewButtonCell())
    {
        var renderer = new CellRendererButtonValue(this)
        {
            Editable = false,
            Mode = CellRendererMode.Activatable,
            Height = RowHeight
        };
        renderer.SetAlignment(0.5f, 0.5f);
        base.PackStart(renderer, false);
        base.SortMode = DataGridViewColumnSortMode.NotSortable;
        base.Sizing = TreeViewColumnSizing.GrowOnly;
        _cellRenderer = renderer;
    }

    internal override DataGridViewCell NewCell(object value = null, Type valueType = null)
    {
        var newcell = new DataGridViewButtonCell();
        AtrributesClone(newcell);
        newcell.Value = value;
        newcell.ValueType = valueType;

        return newcell;
    }

    private void TreeView_RowActivated(object o, RowActivatedArgs args)
    {
        if (args.Column.Handle == this.Handle)
        {
            if (args.Column.Cells[0] is CellRendererText cell)
            {
                TreePath path = args.Path;
                _gridview.CellValueChanagedHandler(this.Index, path.Indices.Last());
            }
        }
    }

}

/// <summary>
/// Hosts a collection of
/// <see cref="T:System.Windows.Forms.DataGridViewCheckBoxCell" /> objects.
/// </summary>
/// <filterpriority>
/// 2
/// </filterpriority>
public class DataGridViewCheckBoxColumn : DataGridViewColumn
{
    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.DataGridViewCheckBoxColumn" /> class.
    /// </summary>
    public DataGridViewCheckBoxColumn() : this(null)
    {
    }

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.DataGridViewCheckBoxColumn" /> class attached to the specified grid.
    /// </summary>
    /// <param name="ownerGridView">
    /// The owning
    /// <see cref="T:System.Windows.Forms.DataGridView" />.
    /// </param>
    public DataGridViewCheckBoxColumn(DataGridView ownerGridView) : base(0, ownerGridView, new DataGridViewCheckBoxCell())
    {
        ValueType = typeof(bool);

        var renderer = new CellRendererToggleValue(this)
        {
            Editable = true,
            Mode = CellRendererMode.Activatable,
            Height = RowHeight
        };
        renderer.Toggled += CellName_Toggled;
        base.PackStart(renderer, true);
        base.SortMode = DataGridViewColumnSortMode.NotSortable;
        base.Sizing = TreeViewColumnSizing.GrowOnly;
        _cellRenderer = renderer;
    }

    internal override DataGridViewCell NewCell(object value = null, Type valueType = null)
    {
        var newcell = new DataGridViewCheckBoxCell();
        AtrributesClone(newcell);
        newcell.Value = value;
        newcell.ValueType = valueType;

        return newcell;
    }

    private void CellName_Toggled(object o, ToggledArgs args)
    {
        var path = new TreePath(args.Path);
        var model = _treeView.Model;
        model.GetIter(out TreeIter iter, path);
        object cell = model.GetValue(iter, this.Index);
        if (cell is DataGridViewCell val)
        {
            var tggle = (CellRendererToggleValue)o;
            val.Value = tggle.Active == false;
            _gridview.CellValueChanagedHandler(this.Index, path.Indices.Last());
        }
    }

}

/// <summary>
/// Represents a column in a
/// <see cref="T:System.Windows.Forms.DataGridView" /> control.
/// </summary>
/// <filterpriority>
/// 2
/// </filterpriority>
public class DataGridViewColumn : TreeViewColumn
{

    internal ICellRenderer _cellRenderer;

    internal DataGridViewCell _cellTemplate;

    internal DataGridView _gridview;

    internal Gtk.TreeView _treeView;

    private DataGridViewAutoSizeColumnMode _AutoSizeMode;

    private string _DataPropertyName = string.Empty;

    private DataGridViewCellStyle _DefaultCellStyle;

    private int _DisplayIndex;

    //protected override void Dispose(bool disposing) {  }

    private int _index;

    private string _markup;

    private bool _ReadOnly;

    private DataGridViewColumnSortMode _SortMode = DataGridViewColumnSortMode.Automatic;

    /// <summary>
    /// The disposed.
    /// </summary>
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public virtual event EventHandler Disposed;

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
    public DataGridViewColumn(DataGridView ownerGridView, DataGridViewCell cellTemplate) : this(0, ownerGridView, cellTemplate)
    {
        var renderer = new CellRendererValue(this)
        {
            Editable = true,
            Mode = CellRendererMode.Editable
        };
        renderer.Edited += Renderer_Edited;
        renderer.PlaceholderText = "---";
        renderer.Height = RowHeight;
        base.PackStart(renderer, true);
        base.Sizing = TreeViewColumnSizing.GrowOnly;
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
    protected DataGridViewColumn(int column, DataGridView ownerGridView, DataGridViewCell cellTemplate) : base()
    {
        if (ownerGridView != null)
        {
            _gridview = ownerGridView;
            _treeView = ownerGridView.GridView;
        }
        _cellTemplate = cellTemplate;
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
            if (value == DataGridViewAutoSizeColumnMode.None) { _AutoSizeMode = value; base.Resizable = false; base.Sizing = TreeViewColumnSizing.Fixed; } else { base.Resizable = true; base.Sizing = TreeViewColumnSizing.GrowOnly; }
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
    public ContextMenuStrip ContextMenuStrip
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the
    /// <see cref="T:System.Windows.Forms.DataGridViewColumn.DataGridView" /> that contains this column.
    /// </summary>
    public DataGridView DataGridView
    {
        get { return _gridview; }
        set { _gridview = value; _treeView = value.GridView; }
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
    public DataGridViewCellStyle DefaultCellStyle
    {
        get => _DefaultCellStyle;

        set { _DefaultCellStyle = value; _cellRenderer.ColumnStyle = value; }
    }

    /// <summary>
    /// Gets or sets the display index of the column.
    /// </summary>
    [Browsable(false)]
    public int DisplayIndex
    {
        get => _DisplayIndex;
        set { _DisplayIndex = value; }
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
    }

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
    public DataGridViewColumnHeaderCell HeaderCell
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
            _index = value; if (_SortMode != DataGridViewColumnSortMode.NotSortable) { base.SortColumnId = value; }
            foreach (var cell in base.Cells) { base.AddAttribute(cell, "cellvalue", _index); }
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
    public DataGridViewCellStyle InheritedStyle { get; }

    /// <summary>
    /// Gets a value indicating whether the column is data-bound.
    /// </summary>
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool IsDataBound { get; }

    /// <summary>
    /// Gets or sets the Pango markup used for rendering text in this column.
    /// </summary>
    public string Markup
    {
        get => _markup;
        set { _markup = value; _cellRenderer.Markup = value; }
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
            _cellRenderer.Editable = !_ReadOnly;
            _cellRenderer.Activatable = !_ReadOnly;
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
    public ISite Site
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
    public string ToolTipText
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
        set { base.FixedWidth = value; }
    }

    private int DefaultHeight
    {
        get
        {
            if (_treeView != null)
            {
                double size = _treeView.PangoContext.FontDescription.Size / Pango.Scale.PangoScale;
                return (int)size + 16;
            }

            return 28;
        }
    }

    /// <summary>
    /// Creates a shallow clone of the column.
    /// </summary>
    /// <returns>
    /// A cloned object (implementation returns null in this port).
    /// </returns>
    public object Clone()
    {
        return null;
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
    /// Applies the provided default
    /// <see cref="T:System.Windows.Forms.DataGridViewCellStyle" /> to the column renderer and updates wrapping/height behavior.
    /// </summary>
    /// <param name="cellStyle">
    /// The default style to apply.
    /// </param>
    public void SetGridViewDefaultStyle(DataGridViewCellStyle cellStyle)
    {
        if (cellStyle != null)
        {
            if (cellStyle.WrapMode == DataGridViewTriState.True)
            {
                _cellRenderer.WrapMode = Pango.WrapMode.WordChar;
                _cellRenderer.WrapWidth = 0;
                _cellRenderer.WidthChars = 0;
            }
        }
        if (_gridview != null)
        {
            if (_gridview.AutoSizeRowsMode == DataGridViewAutoSizeRowsMode.AllCells || _gridview.AutoSizeRowsMode == DataGridViewAutoSizeRowsMode.DisplayedCells || _gridview.RowTemplate.Resizable == DataGridViewTriState.True)
            {

            }
            else
            {
                _cellRenderer.Height = RowHeight;
            }
        }
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
        newcell.ReadOnly = _cellTemplate.ReadOnly;
    }

    internal virtual DataGridViewCell NewCell(object value = null, Type valueType = null)
    {
        var newcell = new DataGridViewTextBoxCell();
        AtrributesClone(newcell);
        newcell.Value = value;
        newcell.ValueType = valueType;

        return newcell;
    }

    /// <summary>
    /// Raises the Disposed event.
    /// </summary>
    protected virtual void OnDisposed(System.EventArgs e)
    {
        Disposed?.Invoke(this, e);
    }

    private void Renderer_Edited(object o, EditedArgs args)
    {
        var path = new TreePath(args.Path);
        var model = _treeView.Model;
        model.GetIter(out TreeIter iter, path);
        object cell = model.GetValue(iter, this.Index);
        if (cell is DataGridViewCell val)
        {
            val.Value = args.NewText;
            _gridview.CellValueChanagedHandler(this.Index, path.Indices.Last());
        }
    }

}

/// <summary>
/// Represents a column of
/// <see cref="T:System.Windows.Forms.DataGridViewComboBoxCell" /> objects.
/// </summary>
/// <filterpriority>
/// 2
/// </filterpriority>
public class DataGridViewComboBoxColumn : DataGridViewColumn
{

    internal Gtk.ListStore model = new Gtk.ListStore(typeof(string));

    readonly ObjectCollection _items;

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.DataGridViewComboBoxColumn" /> class.
    /// </summary>
    public DataGridViewComboBoxColumn() : this(null)
    {
    }

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.DataGridViewComboBoxColumn" /> class attached to the specified grid.
    /// </summary>
    /// <param name="ownerGridView">
    /// The owning
    /// <see cref="T:System.Windows.Forms.DataGridView" />.
    /// </param>
    public DataGridViewComboBoxColumn(DataGridView ownerGridView) : base(0, ownerGridView, new DataGridViewComboBoxCell())
    {
        _items = new ObjectCollection(this);
        var renderer = new CellRendererComboValue(this)
        {
            Editable = true
        };
        renderer.Edited += Renderer_Edited;
        renderer.TextColumn = 0;
        renderer.Height = RowHeight;
        renderer.Model = model;
        base.PackStart(renderer, true);
        base.SortMode = DataGridViewColumnSortMode.NotSortable;
        base.Sizing = TreeViewColumnSizing.GrowOnly;
        _cellRenderer = renderer;
    }

    /// <summary>
    /// Gets the collection of items presented by the combo box cells.
    /// </summary>
    public ObjectCollection Items => _items;

    internal override DataGridViewCell NewCell(object value = null, Type valueType = null)
    {
        var newcell = new DataGridViewComboBoxCell();
        AtrributesClone(newcell);
        newcell.Value = value;
        newcell.ValueType = valueType;

        return newcell;
    }

    private void Renderer_Edited(object o, EditedArgs args)
    {
        var path = new TreePath(args.Path);
        var model = _treeView.Model;
        model.GetIter(out TreeIter iter, path);
        object cell = model.GetValue(iter, this.Index);
        if (cell is DataGridViewCell val)
        {
            val.Value = args.NewText;
            _gridview.CellValueChanagedHandler(this.Index, path.Indices.Last());
        }
    }

    /// <summary>
    /// Simple collection class for combo box items used by the column.
    /// </summary>
    public class ObjectCollection : ArrayList
    {

        private readonly DataGridViewComboBoxColumn _owner;

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="T:System.Windows.Forms.DataGridViewComboBoxColumn.ObjectCollection" /> class.
        /// </summary>
        /// <param name="owner">
        /// Owning combo box column.
        /// </param>
        public ObjectCollection(DataGridViewComboBoxColumn owner)
        {
            _owner = owner;
        }

        /// <summary>
        /// Adds an item to the collection and the underlying GTK model.
        /// </summary>
        /// <param name="value">
        /// Item to add.
        /// </param>
        /// <returns>
        /// The index at which the item was added.
        /// </returns>
        public override int Add(object value)
        {
            _owner.model.AppendValues(value);

            return base.Add(value);
        }

        /// <summary>
        /// Adds a range of items to the collection.
        /// </summary>
        /// <param name="items">
        /// Array of items to add.
        /// </param>
        public void AddRange(object[] items)
        {
            foreach (object item in items)
                Add(item);
        }

    }
}

/// <summary>
/// Hosts a collection of
/// <see cref="T:System.Windows.Forms.DataGridViewImageCell" /> objects.
/// </summary>
/// <filterpriority>
/// 2
/// </filterpriority>
public class DataGridViewImageColumn : DataGridViewColumn
{
    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.DataGridViewImageColumn" /> class.
    /// </summary>
    public DataGridViewImageColumn() : this(null)
    {
    }

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.DataGridViewImageColumn" /> class attached to the specified grid.
    /// </summary>
    /// <param name="ownerGridView">
    /// The owning
    /// <see cref="T:System.Windows.Forms.DataGridView" />.
    /// </param>
    public DataGridViewImageColumn(DataGridView ownerGridView) : base(0, ownerGridView, new DataGridViewImageCell())
    {
        ValueType = typeof(Drawing.Image);
        var renderer = new CellRendererPixbufValue(this)
        {
            Height = RowHeight
        };
        base.PackStart(renderer, false);
        base.SortMode = DataGridViewColumnSortMode.NotSortable;
        base.Sizing = TreeViewColumnSizing.GrowOnly;
        _cellRenderer = renderer;
    }

    internal override DataGridViewCell NewCell(object value = null, Type valueType = null)
    {
        var newcell = new DataGridViewImageCell();
        AtrributesClone(newcell);
        newcell.Value = value;
        newcell.ValueType = valueType;

        return newcell;
    }

}

/// <summary>
/// Represents a column of cells that contain links in a
/// <see cref="T:System.Windows.Forms.DataGridView" /> control.
/// </summary>
/// <filterpriority>
/// 2
/// </filterpriority>
public class DataGridViewLinkColumn : DataGridViewColumn
{
    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.DataGridViewLinkColumn" /> class.
    /// </summary>
    public DataGridViewLinkColumn() : this(null)
    {
    }

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.DataGridViewLinkColumn" /> class attached to the specified grid.
    /// </summary>
    /// <param name="ownerGridView">
    /// The owning
    /// <see cref="T:System.Windows.Forms.DataGridView" />.
    /// </param>
    public DataGridViewLinkColumn(DataGridView ownerGridView) : base(0, ownerGridView, new DataGridViewLinkCell())
    {

    }

    internal override DataGridViewCell NewCell(object value = null, Type valueType = null)
    {
        var newcell = new DataGridViewLinkCell();
        AtrributesClone(newcell);
        newcell.Value = value;
        newcell.ValueType = valueType;

        return newcell;
    }

}

/// <summary>
/// Represents a column of radio-button style toggle cells.
/// </summary>
public class DataGridViewRadioColumn : DataGridViewColumn
{
    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.DataGridViewRadioColumn" /> class.
    /// </summary>
    public DataGridViewRadioColumn() : this(null)
    {
    }

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.DataGridViewRadioColumn" /> class attached to the specified grid.
    /// </summary>
    /// <param name="ownerGridView">
    /// The owning
    /// <see cref="T:System.Windows.Forms.DataGridView" />.
    /// </param>
    public DataGridViewRadioColumn(DataGridView ownerGridView) : base(0, ownerGridView, new DataGridViewRadioCell())
    {
        ValueType = typeof(bool);
        var renderer = new CellRendererToggleValue(this)
        {
            Editable = true,
            Mode = CellRendererMode.Activatable,
            Radio = true,
            Height = RowHeight
        };
        renderer.Toggled += CellName_Toggled;
        base.PackStart(renderer, true);
        base.SortMode = DataGridViewColumnSortMode.NotSortable;
        base.Sizing = TreeViewColumnSizing.GrowOnly;
        _cellRenderer = renderer;
    }

    internal override DataGridViewCell NewCell(object value = null, Type valueType = null)
    {
        var newcell = new DataGridViewRadioCell();
        AtrributesClone(newcell);
        newcell.Value = value;
        newcell.ValueType = valueType;

        return newcell;
    }

    private void CellName_Toggled(object o, ToggledArgs args)
    {
        var path = new TreePath(args.Path);
        var model = _treeView.Model;
        model.GetIter(out TreeIter iter, path);
        object cell = model.GetValue(iter, this.Index);
        if (cell is DataGridViewCell val)
        {
            var tggle = (CellRendererToggleValue)o;
            val.Value = tggle.Active == false;
            _gridview.CellValueChanagedHandler(this.Index, path.Indices.Last());
        }
    }

}

/// <summary>
/// Hosts a collection of
/// <see cref="T:System.Windows.Forms.DataGridViewTextBoxCell" /> cells.
/// </summary>
/// <filterpriority>
/// 2
/// </filterpriority>
public class DataGridViewTextBoxColumn : DataGridViewColumn
{
    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.DataGridViewTextBoxColumn" /> class using the default text box cell template.
    /// </summary>
    public DataGridViewTextBoxColumn() : base(new DataGridViewTextBoxCell())
    {
    }

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.DataGridViewTextBoxColumn" /> class attached to the specified grid.
    /// </summary>
    /// <param name="ownerGridView">
    /// The owning
    /// <see cref="T:System.Windows.Forms.DataGridView" />.
    /// </param>
    public DataGridViewTextBoxColumn(DataGridView ownerGridView) : base(ownerGridView, new DataGridViewTextBoxCell())
    {
    }

}