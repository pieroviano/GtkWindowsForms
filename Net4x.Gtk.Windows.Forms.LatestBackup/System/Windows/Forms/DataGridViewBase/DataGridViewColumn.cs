/*
 * Based on GTK components, it is a cross-platform interface component that is compatible with the native C# control winform interface.
 * Use this component GtkSystem.Windows.Forms instead of Microsoft.WindowsDesktop.App.WindowsForms, compile once, and run across Windows, Linux, and macOS
 * Technical support 438865652@qq.com，https://www.gtkapp.com, https://gitee.com/easywebfactory, https://github.com/easywebfactory438865652@qq.com，https://www.gtkapp.com, https://gitee.com/easywebfactory, https://github.com/easywebfactory
 * author:chenhongjin
 */
using Gtk;
using System.Collections;
using System.ComponentModel;
using System.Reflection;
using System.Windows.Forms.GtkRender;

namespace System.Windows.Forms
{
    /// <summary>
    /// Hosts a collection of
    /// <see cref="T:System.Windows.Forms.DataGridViewButtonCell" /> objects.
    /// </summary>
    /// <filterpriority>
    /// 2
    /// </filterpriority>
    public class DataGridViewButtonColumn : DataGridViewColumn
    {
        public DataGridViewButtonColumn() : this(null)
        {
        }

        public DataGridViewButtonColumn(DataGridView ownerGridView) : base(0, ownerGridView, new DataGridViewButtonCell())
        {
            var renderer = new CellRendererButtonValue(this);
            renderer.Editable = false;
            renderer.Mode = CellRendererMode.Activatable;
            renderer.Height = RowHeight;
            renderer.SetAlignment(0.5f, 0.5f);
            base.PackStart(renderer, false);
            base.SortMode = DataGridViewColumnSortMode.NotSortable;
            base.Sizing = TreeViewColumnSizing.GrowOnly;
            _cellRenderer = renderer;
        }

        internal override DataGridViewCell NewCell(object value = null, Type valueType = null)
        {
            DataGridViewButtonCell newcell = new DataGridViewButtonCell();
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
        public DataGridViewCheckBoxColumn() : this(null)
        {
        }

        public DataGridViewCheckBoxColumn(DataGridView ownerGridView) : base(0, ownerGridView, new DataGridViewCheckBoxCell())
        {
            ValueType = typeof(bool);

            var renderer = new CellRendererToggleValue(this);
            renderer.Editable = true;
            renderer.Mode = CellRendererMode.Activatable;
            renderer.Height = RowHeight;
            renderer.Toggled += CellName_Toggled;
            base.PackStart(renderer, true);
            base.SortMode = DataGridViewColumnSortMode.NotSortable;
            base.Sizing = TreeViewColumnSizing.GrowOnly;
            _cellRenderer = renderer;
        }

        internal override DataGridViewCell NewCell(object value = null, Type valueType = null)
        {
            DataGridViewCheckBoxCell newcell = new DataGridViewCheckBoxCell();
            AtrributesClone(newcell);
            newcell.Value = value;
            newcell.ValueType = valueType;

            return newcell;
        }

        private void CellName_Toggled(object o, ToggledArgs args)
        {
            TreePath path = new TreePath(args.Path);
            var model = _treeView.Model;
            model.GetIter(out TreeIter iter, path);
            object cell = model.GetValue(iter, this.Index);
            if (cell is DataGridViewCell val)
            {
                CellRendererToggleValue tggle = (CellRendererToggleValue)o;
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

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public event EventHandler Disposed;

        public DataGridViewColumn(DataGridView ownerGridView) : this(ownerGridView, new DataGridViewTextBoxCell())
        {
        }

        public DataGridViewColumn() : this(null, new DataGridViewTextBoxCell())
        {
        }

        public DataGridViewColumn(DataGridViewCell cellTemplate) : this(null, cellTemplate)
        {
        }

        public DataGridViewColumn(DataGridView ownerGridView, DataGridViewCell cellTemplate) : this(0, ownerGridView, cellTemplate)
        {
            var renderer = new CellRendererValue(this);
            renderer.Editable = true;
            renderer.Mode = CellRendererMode.Editable;
            renderer.Edited += Renderer_Edited;
            renderer.PlaceholderText = "---";
            renderer.Height = RowHeight;
            base.PackStart(renderer, true);
            base.Sizing = TreeViewColumnSizing.GrowOnly;
            _cellRenderer = renderer as ICellRenderer;
        }

        protected DataGridViewColumn(int column, DataGridView ownerGridView, DataGridViewCell cellTemplate) : base()
        {
            if (ownerGridView != null)
            {
                _gridview = ownerGridView;
                _treeView = ownerGridView.GridView;
            }
            _cellTemplate = cellTemplate;
        }

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

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public virtual DataGridViewCell CellTemplate
        {
            get => _cellTemplate;
            set => _cellTemplate = value;
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Advanced)]

        public virtual Type CellType { get { return _cellTemplate.GetType(); } }

        [DefaultValue(null)]

        public ContextMenuStrip ContextMenuStrip { get; set; }

        public DataGridView DataGridView { get { return _gridview; } set { _gridview = value; _treeView = value.GridView; } }

        [Browsable(true)]
        [DefaultValue("")]
        public string DataPropertyName { get => _DataPropertyName; set => _DataPropertyName = value; }

        [Browsable(true)]
        public DataGridViewCellStyle DefaultCellStyle
        {
            get => _DefaultCellStyle;
            set { _DefaultCellStyle = value; _cellRenderer.ColumnStyle = value; }
        }

        [Browsable(false)]

        public int DisplayIndex { get => _DisplayIndex; set { _DisplayIndex = value; } }

        [DefaultValue(0)]

        public int DividerWidth { get; set; }

        [DefaultValue(100)]

        public float FillWeight { get; set; }

        [DefaultValue(false)]
        [RefreshProperties(RefreshProperties.All)]

        public bool Frozen { get; set; }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public DataGridViewColumnHeaderCell HeaderCell { get; set; }

        [DefaultValue("")]

        public string HeaderText { get { return this.Title; } set { this.Title = value; } }

        public int Index
        {
            get => _index;
            internal set
            {
                _index = value; if (_SortMode != DataGridViewColumnSortMode.NotSortable) { base.SortColumnId = value; }
                foreach (var cell in base.Cells) { base.AddAttribute(cell, "cellvalue", _index); }
            }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [EditorBrowsable(EditorBrowsableState.Advanced)]

        public DataGridViewAutoSizeColumnMode InheritedAutoSizeMode { get; }

        [Browsable(false)]

        public DataGridViewCellStyle InheritedStyle { get; }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]

        public bool IsDataBound { get; }

        public string Markup { get => _markup; set { _markup = value; _cellRenderer.Markup = value; } }

        [DefaultValue(5)]
        [Localizable(true)]
        [RefreshProperties(RefreshProperties.Repaint)]

        public int MinimumWidth { get => base.MinWidth; set { base.MinWidth = value; } }

        [DefaultValue("")]
        public string Name { get => base.Button.Name; set { base.Button.Name = value; } }

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

        public new DataGridViewTriState Resizable
        {
            get { return base.Resizable == true ? DataGridViewTriState.True : DataGridViewTriState.False; }
            set { base.Resizable = value == DataGridViewTriState.True; }
        }

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

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]

        public ISite Site { get; set; }

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

        public DataGridViewElementStates State { get { return DataGridViewElementStates.None; } internal set { } }

        [DefaultValue("")]
        [Localizable(true)]

        public string ToolTipText { get; set; }

        //[DefaultValue(true)]
        //[Localizable(true)]
        //public bool Visible { get; set; }
        [Browsable(false)]
        [DefaultValue(null)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Type ValueType { get; set; } = typeof(string);

        [Localizable(true)]
        [RefreshProperties(RefreshProperties.Repaint)]
        public new int Width { get { return base.FixedWidth; } set { base.FixedWidth = value; } }

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

        public object Clone()
        {
            return null;
        }

        public virtual int GetPreferredWidth(DataGridViewAutoSizeColumnMode autoSizeColumnMode, bool fixedHeight)
        {
            return RowHeight;
        }

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

        public override string ToString() { return this.GetType().Name; }

        internal void AtrributesClone(DataGridViewCell newcell)
        {
            newcell.Style = _cellTemplate.Style?.Clone();
            newcell.ReadOnly = _cellTemplate.ReadOnly;
        }

        internal virtual DataGridViewCell NewCell(object value = null, Type valueType = null)
        {
            DataGridViewTextBoxCell newcell = new DataGridViewTextBoxCell();
            AtrributesClone(newcell);
            newcell.Value = value;
            newcell.ValueType = valueType;

            return newcell;
        }

        private void Renderer_Edited(object o, EditedArgs args)
        {
            TreePath path = new TreePath(args.Path);
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

        ObjectCollection _items;

        public DataGridViewComboBoxColumn() : this(null)
        {
        }

        public DataGridViewComboBoxColumn(DataGridView ownerGridView) : base(0, ownerGridView, new DataGridViewComboBoxCell())
        {
            _items = new ObjectCollection(this);
            CellRendererComboValue renderer = new CellRendererComboValue(this);
            renderer.Editable = true;
            renderer.Edited += Renderer_Edited;
            renderer.TextColumn = 0;
            renderer.Height = RowHeight;
            renderer.Model = model;
            base.PackStart(renderer, true);
            base.SortMode = DataGridViewColumnSortMode.NotSortable;
            base.Sizing = TreeViewColumnSizing.GrowOnly;
            _cellRenderer = renderer;
        }

        public ObjectCollection Items => _items;

        internal override DataGridViewCell NewCell(object value = null, Type valueType = null)
        {
            DataGridViewComboBoxCell newcell = new DataGridViewComboBoxCell();
            AtrributesClone(newcell);
            newcell.Value = value;
            newcell.ValueType = valueType;

            return newcell;
        }

        private void Renderer_Edited(object o, EditedArgs args)
        {
            TreePath path = new TreePath(args.Path);
            var model = _treeView.Model;
            model.GetIter(out TreeIter iter, path);
            object cell = model.GetValue(iter, this.Index);
            if (cell is DataGridViewCell val)
            {
                val.Value = args.NewText;
                _gridview.CellValueChanagedHandler(this.Index, path.Indices.Last());
            }
        }

        public class ObjectCollection : ArrayList
        {

            private DataGridViewComboBoxColumn _owner;

            public ObjectCollection(DataGridViewComboBoxColumn owner)
            {
                _owner = owner;
            }

            public override int Add(object value)
            {
                _owner.model.AppendValues(value);

                return base.Add(value);
            }

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
        public DataGridViewImageColumn() : this(null)
        {
        }

        public DataGridViewImageColumn(DataGridView ownerGridView) : base(0, ownerGridView, new DataGridViewImageCell())
        {
            ValueType = typeof(Drawing.Image);
            var renderer = new CellRendererPixbufValue(this);
            renderer.Height = RowHeight;
            base.PackStart(renderer, false);
            base.SortMode = DataGridViewColumnSortMode.NotSortable;
            base.Sizing = TreeViewColumnSizing.GrowOnly;
            _cellRenderer = renderer;
        }

        internal override DataGridViewCell NewCell(object value = null, Type valueType = null)
        {
            DataGridViewImageCell newcell = new DataGridViewImageCell();
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
        public DataGridViewLinkColumn() : this(null)
        {
        }

        public DataGridViewLinkColumn(DataGridView ownerGridView) : base(0, ownerGridView, new DataGridViewLinkCell())
        {

        }

        internal override DataGridViewCell NewCell(object value = null, Type valueType = null)
        {
            DataGridViewLinkCell newcell = new DataGridViewLinkCell();
            AtrributesClone(newcell);
            newcell.Value = value;
            newcell.ValueType = valueType;

            return newcell;
        }

    }

    public class DataGridViewRadioColumn : DataGridViewColumn
    {
        public DataGridViewRadioColumn() : this(null)
        {
        }

        public DataGridViewRadioColumn(DataGridView ownerGridView) : base(0, ownerGridView, new DataGridViewRadioCell())
        {
            ValueType = typeof(bool);
            var renderer = new CellRendererToggleValue(this);
            renderer.Editable = true;
            renderer.Mode = CellRendererMode.Activatable;
            renderer.Radio = true;
            renderer.Height = RowHeight;
            renderer.Toggled += CellName_Toggled;
            base.PackStart(renderer, true);
            base.SortMode = DataGridViewColumnSortMode.NotSortable;
            base.Sizing = TreeViewColumnSizing.GrowOnly;
            _cellRenderer = renderer;
        }

        internal override DataGridViewCell NewCell(object value = null, Type valueType = null)
        {
            DataGridViewRadioCell newcell = new DataGridViewRadioCell();
            AtrributesClone(newcell);
            newcell.Value = value;
            newcell.ValueType = valueType;

            return newcell;
        }

        private void CellName_Toggled(object o, ToggledArgs args)
        {
            TreePath path = new TreePath(args.Path);
            var model = _treeView.Model;
            model.GetIter(out TreeIter iter, path);
            object cell = model.GetValue(iter, this.Index);
            if (cell is DataGridViewCell val)
            {
                CellRendererToggleValue tggle = (CellRendererToggleValue)o;
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
        public DataGridViewTextBoxColumn() : base(new DataGridViewTextBoxCell())
        {
        }

        public DataGridViewTextBoxColumn(DataGridView ownerGridView) : base(ownerGridView, new DataGridViewTextBoxCell())
        {
        }

    }

}
