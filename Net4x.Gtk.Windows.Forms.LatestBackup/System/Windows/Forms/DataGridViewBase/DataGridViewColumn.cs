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
    /// <see cref="System.Windows.Forms.DataGridViewTextBoxCell" /> cells.
    /// </summary>
    /// <filterpriority>
    /// 2
    /// </filterpriority>
    public class DataGridViewTextBoxColumn : DataGridViewColumn
    {
        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="System.Windows.Forms.DataGridViewTextBoxColumn" /> class to the default state.
        /// </summary>
        public DataGridViewTextBoxColumn() : base(new DataGridViewTextBoxCell())
        {
        }
        public DataGridViewTextBoxColumn(DataGridView ownerGridView) : base(ownerGridView, new DataGridViewTextBoxCell())
        {
        }
    }
    /// <summary>
    /// Hosts a collection of
    /// <see cref="System.Windows.Forms.DataGridViewCheckBoxCell" /> objects.
    /// </summary>
    /// <filterpriority>
    /// 2
    /// </filterpriority>
    public class DataGridViewCheckBoxColumn : DataGridViewColumn
    {
        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="System.Windows.Forms.DataGridViewCheckBoxColumn" /> class to the default state.
        /// </summary>
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
        internal override DataGridViewCell NewCell(object value = null, Type valueType = null)
        {
            DataGridViewCheckBoxCell newcell = new DataGridViewCheckBoxCell();
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
        internal override DataGridViewCell NewCell(object value = null, Type valueType = null)
        {
            DataGridViewRadioCell newcell = new DataGridViewRadioCell();
            AtrributesClone(newcell);
            newcell.Value = value;
            newcell.ValueType = valueType;
            return newcell;
        }
    }
    /// <summary>
    /// Represents a column of
    /// <see cref="System.Windows.Forms.DataGridViewComboBoxCell" /> objects.
    /// </summary>
    /// <filterpriority>
    /// 2
    /// </filterpriority>
    public class DataGridViewComboBoxColumn : DataGridViewColumn
    {
        ObjectCollection _items;

        internal Gtk.ListStore model = new Gtk.ListStore(typeof(string));
        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="System.Windows.Forms.DataGridViewTextBoxColumn" /> class to the default state.
        /// </summary>
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
        internal override DataGridViewCell NewCell(object value = null, Type valueType = null)
        {
            DataGridViewComboBoxCell newcell = new DataGridViewComboBoxCell();
            AtrributesClone(newcell);
            newcell.Value = value;
            newcell.ValueType = valueType;
            return newcell;
        }
        /// <summary>
        /// Gets the collection of objects used as selections in the combo boxes.
        /// </summary>
        /// <returns>An
        /// <see cref="System.Windows.Forms.DataGridViewComboBoxCell.ObjectCollection" /> that represents the selections in the combo boxes.
        /// </returns>
        /// <exception cref="System.InvalidOperationException">The value of the
        /// <see cref="System.Windows.Forms.DataGridViewComboBoxColumn.CellTemplate" /> property is null.
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        public ObjectCollection Items => _items;
        public class ObjectCollection : ArrayList
        {

            private DataGridViewComboBoxColumn _owner;
            public ObjectCollection(DataGridViewComboBoxColumn owner)
            {
                _owner = owner;
            }
            public void AddRange(object[] items)
            {
                foreach (object item in items)
                    Add(item);
            }
            public override int Add(object value)
            {
                _owner.model.AppendValues(value);
                return base.Add(value);
            }
        }
    }
    /// <summary>
    /// Hosts a collection of
    /// <see cref="System.Windows.Forms.DataGridViewButtonCell" /> objects.
    /// </summary>
    /// <filterpriority>
    /// 2
    /// </filterpriority>
    public class DataGridViewButtonColumn : DataGridViewColumn
    {
        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="System.Windows.Forms.DataGridViewButtonColumn" /> class to the default state.
        /// </summary>
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
        internal override DataGridViewCell NewCell(object value = null, Type valueType = null)
        {
            DataGridViewButtonCell newcell = new DataGridViewButtonCell();
            AtrributesClone(newcell);
            newcell.Value = value;
            newcell.ValueType = valueType;
            return newcell;
        }
    }
    /// <summary>
    /// Hosts a collection of
    /// <see cref="System.Windows.Forms.DataGridViewImageCell" /> objects.
    /// </summary>
    /// <filterpriority>
    /// 2
    /// </filterpriority>
    public class DataGridViewImageColumn : DataGridViewColumn
    {
        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="System.Windows.Forms.DataGridViewImageColumn" /> class, configuring it for use with cell values of type
        /// <see cref="System.Drawing.Image" />
        /// </summary>
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
    /// <see cref="System.Windows.Forms.DataGridView" /> control.
    /// </summary>
    /// <filterpriority>
    /// 2
    /// </filterpriority>
    public class DataGridViewLinkColumn : DataGridViewColumn
    {
        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="System.Windows.Forms.DataGridViewLinkColumn" /> class.
        /// </summary>
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
    /// <summary>
    /// Represents a column in a
    /// <see cref="System.Windows.Forms.DataGridView" /> control.
    /// </summary>
    /// <filterpriority>
    /// 2
    /// </filterpriority>
    public class DataGridViewColumn : TreeViewColumn
    {

        internal DataGridViewCell _cellTemplate;

        internal Gtk.TreeView _treeView;

        internal DataGridView _gridview;

        internal ICellRenderer _cellRenderer;
        protected DataGridViewColumn(int column, DataGridView ownerGridView, DataGridViewCell cellTemplate) : base()
        {
            if (ownerGridView != null)
            {
                _gridview = ownerGridView;
                _treeView = ownerGridView.GridView;
            }
            _cellTemplate = cellTemplate;
        }
        public DataGridViewColumn(DataGridView ownerGridView) : this(ownerGridView, new DataGridViewTextBoxCell())
        {
        }
        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="System.Windows.Forms.DataGridViewColumn" /> class to the default state.
        /// </summary>
        public DataGridViewColumn() : this(null, new DataGridViewTextBoxCell())
        {
        }
        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="System.Windows.Forms.DataGridViewColumn" /> class using an existing
        /// <see cref="System.Windows.Forms.DataGridViewCell" /> as a template.
        /// </summary>
        /// <param name="cellTemplate">An existing
        /// <see cref="System.Windows.Forms.DataGridViewCell" /> to use as a template.
        /// </param>
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
        internal virtual DataGridViewCell NewCell(object value = null, Type valueType = null)
        {
            DataGridViewTextBoxCell newcell = new DataGridViewTextBoxCell();
            AtrributesClone(newcell);
            newcell.Value = value;
            newcell.ValueType = valueType;
            return newcell;
        }
        internal void AtrributesClone(DataGridViewCell newcell)
        {
            newcell.Style = _cellTemplate.Style?.Clone();
            newcell.ReadOnly = _cellTemplate.ReadOnly;
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

        public DataGridView DataGridView { get { return _gridview; } set { _gridview = value; _treeView = value.GridView; } }

        private string _markup;
        public string Markup { get => _markup; set { _markup = value; _cellRenderer.Markup = value; } }


        public DataGridViewElementStates State { get { return DataGridViewElementStates.None; } internal set { } }
        /// <summary>
        /// Gets or sets the caption text on the column's header cell.
        /// </summary>
        /// <returns>A
        /// <see cref="System.String" /> with the desired text. The default is an empty string ("").
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        [DefaultValue("")]
        public string HeaderText { get { return this.Title; } set { this.Title = value; } }

        private int _DisplayIndex;
        /// <summary>
        /// Gets or sets the display order of the column relative to the currently displayed columns.
        /// </summary>
        /// <returns>The zero-based position of the column as it is displayed in the associated
        /// <see cref="System.Windows.Forms.DataGridView" />, or -1 if the band is not contained within a control.
        /// </returns>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// <see cref="System.Windows.Forms.DataGridViewElement.DataGridView" /> is not null and the specified value when setting this property is less than 0 or greater than or equal to the number of columns in the control.-or-
        /// <see cref="System.Windows.Forms.DataGridViewElement.DataGridView" /> is null and the specified value when setting this property is less than -1.-or-The specified value when setting this property is equal to
        /// <see cref="System.Int32.MaxValue" />
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        [Browsable(false)]
        public int DisplayIndex { get => _DisplayIndex; set { _DisplayIndex = value; } }
        /// <summary>
        /// Gets or sets the current width of the column.
        /// </summary>
        /// <returns>The width, in pixels, of the column. The default is 100.
        /// </returns>
        /// <exception cref="System.ArgumentOutOfRangeException">The specified value when setting this property is greater than 65536.
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        [Localizable(true)]
        [RefreshProperties(RefreshProperties.Repaint)]
        public new int Width { get { return base.FixedWidth; } set { base.FixedWidth = value; } }
        /// <summary>
        /// Gets or sets the data type of the values in the column's cells.
        /// </summary>
        /// <returns>A
        /// <see cref="System.Type" /> that describes the run-time class of the values stored in the column's cells.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        //[DefaultValue(true)]
        //[Localizable(true)]
        //public bool Visible { get; set; }
        [Browsable(false)]
        [DefaultValue(null)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Type ValueType { get; set; } = typeof(string);
        /// <summary>
        /// Gets or sets the text used for ToolTips.
        /// </summary>
        /// <returns>The text to display as a ToolTip for the column.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        [DefaultValue("")]
        [Localizable(true)]
        public string ToolTipText { get; set; }

        private DataGridViewColumnSortMode _SortMode = DataGridViewColumnSortMode.Automatic;
        /// <summary>
        /// Gets or sets the sort mode for the column.
        /// </summary>
        /// <returns>A
        /// <see cref="System.Windows.Forms.DataGridViewColumnSortMode" /> that specifies the criteria used to order the rows based on the cell values in a column.
        /// </returns>
        /// <exception cref="System.InvalidOperationException">The value assigned to the property conflicts with
        /// <see cref="System.Windows.Forms.DataGridView.SelectionMode" />
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
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
        /// Gets or sets the site of the column.
        /// </summary>
        /// <returns>The
        /// <see cref="System.ComponentModel.ISite" /> associated with the column, if any.
        /// </returns>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ISite Site { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether the column is resizable.
        /// </summary>
        /// <returns>One of the
        /// <see cref="System.Windows.Forms.DataGridViewTriState" /> values. The default is
        /// <see cref="System.Windows.Forms.DataGridViewTriState.True" />
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public new DataGridViewTriState Resizable
        {
            get { return base.Resizable == true ? DataGridViewTriState.True : DataGridViewTriState.False; }
            set { base.Resizable = value == DataGridViewTriState.True; }
        }

        private bool _ReadOnly;
        /// <summary>
        /// Gets or sets a value indicating whether the user can edit the column's cells.
        /// </summary>
        /// <returns>true if the user cannot edit the column's cells; otherwise, false.
        /// </returns>
        /// <exception cref="System.InvalidOperationException">This property is set to false for a column that is bound to a read-only data source.
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
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
        /// Gets or sets the name of the column.
        /// </summary>
        /// <returns>A
        /// <see cref="System.String" /> that contains the name of the column. The default is an empty string ("").
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        [DefaultValue("")]
        public string Name { get => base.Button.Name; set { base.Button.Name = value; } }
        /// <summary>
        /// Gets or sets the minimum width, in pixels, of the column.
        /// </summary>
        /// <returns>The number of pixels, from 2 to
        /// <see cref="System.Int32.MaxValue" />, that specifies the minimum width of the column. The default is 5.
        /// </returns>
        /// <exception cref="System.ArgumentOutOfRangeException">The value is less than 2 or greater than
        /// <see cref="System.Int32.MaxValue" />
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        [DefaultValue(5)]
        [Localizable(true)]
        [RefreshProperties(RefreshProperties.Repaint)]
        public int MinimumWidth { get => base.MinWidth; set { base.MinWidth = value; } }
        /// <summary>
        /// Gets a value indicating whether the column is bound to a data source.
        /// </summary>
        /// <returns>true if the column is connected to a data source; otherwise, false.
        /// </returns>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsDataBound { get; }
        /// <summary>
        /// Gets the cell style currently applied to the column.
        /// </summary>
        /// <returns>A
        /// <see cref="System.Windows.Forms.DataGridViewCellStyle" /> that represents the cell style used to display the column.
        /// </returns>
        [Browsable(false)]
        public DataGridViewCellStyle InheritedStyle { get; }
        /// <summary>
        /// Gets the sizing mode in effect for the column.
        /// </summary>
        /// <returns>The
        /// <see cref="System.Windows.Forms.DataGridViewAutoSizeColumnMode" /> value in effect for the column.
        /// </returns>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public DataGridViewAutoSizeColumnMode InheritedAutoSizeMode { get; }
        /// <summary>
        /// Gets or sets the
        /// <see cref="System.Windows.Forms.DataGridViewColumnHeaderCell" /> that represents the column header.
        /// </summary>
        /// <returns>A
        /// <see cref="System.Windows.Forms.DataGridViewColumnHeaderCell" /> that represents the header cell for the column.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public DataGridViewColumnHeaderCell HeaderCell { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether a column will move when a user scrolls the
        /// <see cref="System.Windows.Forms.DataGridView" /> control horizontally.
        /// </summary>
        /// <returns>true to freeze the column; otherwise, false.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        [DefaultValue(false)]
        [RefreshProperties(RefreshProperties.All)]
        public bool Frozen { get; set; }
        /// <summary>
        /// Gets or sets a value that represents the width of the column when it is in fill mode relative to the widths of other fill-mode columns in the control.
        /// </summary>
        /// <returns>A
        /// <see cref="System.Single" /> representing the width of the column when it is in fill mode relative to the widths of other fill-mode columns. The default is 100.
        /// </returns>
        /// <exception cref="System.ArgumentOutOfRangeException">The specified value when setting this property is less than or equal to 0.
        /// </exception>
        [DefaultValue(100)]
        public float FillWeight { get; set; }
        /// <summary>
        /// Gets or sets the width, in pixels, of the column divider.
        /// </summary>
        /// <returns>The thickness, in pixels, of the divider (the column's right margin).
        /// </returns>
        [DefaultValue(0)]
        public int DividerWidth { get; set; }


        private DataGridViewCellStyle _DefaultCellStyle;
        /// <summary>
        /// Gets or sets the column's default cell style.
        /// </summary>
        /// <returns>A
        /// <see cref="System.Windows.Forms.DataGridViewCellStyle" /> that represents the default style of the cells in the column.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        [Browsable(true)]
        public DataGridViewCellStyle DefaultCellStyle
        {
            get => _DefaultCellStyle;
            set { _DefaultCellStyle = value; _cellRenderer.ColumnStyle = value; }
        }

        private string _DataPropertyName = string.Empty;
        /// <summary>
        /// Gets or sets the name of the data source property or database column to which the
        /// <see cref="System.Windows.Forms.DataGridViewColumn" /> is bound.
        /// </summary>
        /// <returns>The case-insensitive name of the property or database column associated with the
        /// <see cref="System.Windows.Forms.DataGridViewColumn" />
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        [Browsable(true)]
        [DefaultValue("")]
        public string DataPropertyName { get => _DataPropertyName; set => _DataPropertyName = value; }
        /// <summary>
        /// Gets or sets the shortcut menu for the column.
        /// </summary>
        /// <returns>The
        /// <see cref="System.Windows.Forms.ContextMenuStrip" /> associated with the current
        /// <see cref="System.Windows.Forms.DataGridViewColumn" /> The default is null.
        /// </returns>
        [DefaultValue(null)]
        public ContextMenuStrip ContextMenuStrip { get; set; }
        /// <summary>
        /// Gets the run-time type of the cell template.
        /// </summary>
        /// <returns>The
        /// <see cref="System.Type" /> of the
        /// <see cref="System.Windows.Forms.DataGridViewCell" /> used as a template for this column. The default is null.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public virtual Type CellType { get { return _cellTemplate.GetType(); } }
        /// <summary>
        /// Gets or sets the template used to create new cells.
        /// </summary>
        /// <returns>A
        /// <see cref="System.Windows.Forms.DataGridViewCell" /> that all other cells in the column are modeled after. The default is null.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public virtual DataGridViewCell CellTemplate
        {
            get => _cellTemplate;
            set => _cellTemplate = value;
        }
        /// <summary>
        /// Gets or sets the mode by which the column automatically adjusts its width.
        /// </summary>
        /// <returns>A
        /// <see cref="System.Windows.Forms.DataGridViewAutoSizeColumnMode" /> value that determines whether the column will automatically adjust its width and how it will determine its preferred width. The default is
        /// <see cref="System.Windows.Forms.DataGridViewAutoSizeColumnMode.NotSet" />
        /// </returns>
        /// <exception cref="System.ComponentModel.InvalidEnumArgumentException">The specified value when setting this property is a
        /// <see cref="System.Windows.Forms.DataGridViewAutoSizeColumnMode" /> that is not valid.
        /// </exception>
        /// <exception cref="System.InvalidOperationException">The specified value when setting this property results in an
        /// <see cref="System.Windows.Forms.DataGridViewColumn.InheritedAutoSizeMode" /> value of
        /// <see cref="System.Windows.Forms.DataGridViewAutoSizeColumnMode.ColumnHeader" /> for a visible column when column headers are hidden.-or-The specified value when setting this property results in an
        /// <see cref="System.Windows.Forms.DataGridViewColumn.InheritedAutoSizeMode" /> value of
        /// <see cref="System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill" /> for a visible column that is frozen.
        /// </exception>
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

        private DataGridViewAutoSizeColumnMode _AutoSizeMode;

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public event EventHandler Disposed;
        /// <returns>
        /// An
        /// <see cref="System.Object" /> that represents the cloned
        /// <see cref="System.Windows.Forms.DataGridViewBand" />
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public object Clone()
        {
            return null;
        }
        /// <summary>
        /// Calculates the ideal width of the column based on the specified criteria.
        /// </summary>
        /// <returns>The ideal width, in pixels, of the column.
        /// </returns>
        /// <param name="autoSizeColumnMode">A
        /// <see cref="System.Windows.Forms.DataGridViewAutoSizeColumnMode" /> value that specifies an automatic sizing mode.
        /// </param>
        /// <param name="fixedHeight">true to calculate the width of the column based on the current row heights; false to calculate the width with the expectation that the row heights will be adjusted.
        /// </param>
        /// <exception cref="System.ArgumentException">
        /// <paramref name="autoSizeColumnMode" /> is
        /// <see cref="System.Windows.Forms.DataGridViewAutoSizeColumnMode.NotSet" />,
        /// <see cref="System.Windows.Forms.DataGridViewAutoSizeColumnMode.None" />, or
        /// <see cref="System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill" />
        /// </exception>
        /// <exception cref="System.ComponentModel.InvalidEnumArgumentException">
        /// <paramref name="autoSizeColumnMode" /> is not a valid
        /// <see cref="System.Windows.Forms.DataGridViewAutoSizeColumnMode" /> value.
        /// </exception>
        public virtual int GetPreferredWidth(DataGridViewAutoSizeColumnMode autoSizeColumnMode, bool fixedHeight)
        {
            return RowHeight;
        }
        /// <summary>
        /// Gets a string that describes the column.
        /// </summary>
        /// <returns>A
        /// <see cref="System.String" /> that describes the column.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public override string ToString() { return this.GetType().Name; }
        //protected override void Dispose(bool disposing) {  }

        private int _index;
        public int Index
        {
            get => _index;
            internal set
            {
                _index = value; if (_SortMode != DataGridViewColumnSortMode.NotSortable) { base.SortColumnId = value; }
                foreach (var cell in base.Cells) { base.AddAttribute(cell, "cellvalue", _index); }
            }
        }
    }


}
