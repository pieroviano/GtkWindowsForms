/*
 * Based on GTK components, it is a cross-platform interface component that is compatible with the native C# control winform interface.
 * Use this component GtkSystem.Windows.Forms instead of Microsoft.WindowsDesktop.App.WindowsForms, compile once, and run across Windows, Linux, and macOS
 * Technical support 438865652@qq.com，https://www.gtkapp.com, https://gitee.com/easywebfactory, https://github.com/easywebfactory438865652@qq.com，https://www.gtkapp.com, https://gitee.com/easywebfactory, https://github.com/easywebfactory
 * author:chenhongjin
 */
using GLib;
using Gtk;
using GtkSystem.Windows.Forms.GtkControls.ControlBase;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Net.Sockets;
using System.Reflection;

namespace System.Windows.Forms
{
    [DesignerCategory("Component")]
    public partial class DataGridView : ScrollableControl
    {
        public readonly DataGridViewBase self = new DataGridViewBase();
        public override object GtkControl => self;
        private DataGridViewColumnCollection _columns;
        private DataGridViewRowCollection _rows;
        private ControlBindingsCollection _collect;
        internal Gtk.TreeStore Store = new TreeStore(typeof(DataGridViewCell));
        public Gtk.TreeView GridView { get { return self.GridView; } }
        public DataGridView():base()
        {
            this.BorderStyle = BorderStyle.FixedSingle;
            GridView.Margin = 0;
            GridView.MarginStart = 0;
            GridView.MarginEnd = 0;
            GridView.Selection.Mode = Gtk.SelectionMode.Multiple;
            GridView.HeadersClickable = true;
            GridView.HeadersVisible = true;
            GridView.ActivateOnSingleClick = false;
           
            _columns = new DataGridViewColumnCollection(this);
            _rows = new DataGridViewRowCollection(this);
            _collect = new ControlBindingsCollection(this);
            GridView.Realized += GridView_Realized;
            GridView.RowActivated += GridView_RowActivated;
            GridView.Selection.Changed += Selection_Changed;
        }

        private List<int> _selectedBandIndexes = new List<int>();
        private void Selection_Changed(object sender, EventArgs e)
        {
            _selectedBandIndexes.Clear();
            TreePath[] treePaths = GridView.Selection.GetSelectedRows();
            foreach (TreePath path in treePaths)
            {
                int idx = path.Indices.Last();
                _selectedBandIndexes.Add(idx);
            }
            if (SelectionChanged != null && Created)
                SelectionChanged(this, e);
        }

        private void GridView_RowActivated(object o, RowActivatedArgs args)
        {
            // single-line selection is effective
            if (CellClick != null)
            {
                DataGridViewColumn column = args.Column as DataGridViewColumn;
                CellClick(this, new DataGridViewCellEventArgs(column.Index, args.Path.Indices.Last()));
            }
        }

        private void GridView_Realized(object sender, EventArgs e)
        {
            OnSetDataSource();
            _columns.Invalidate();
            foreach (Binding binding in DataBindings)
                GridView.AddNotification(binding.PropertyName, propertyNotity);
        }
        private void propertyNotity(object o, NotifyArgs args)
        {
            Binding binding = DataBindings[args.Property];
            binding.WriteValue();
        }
        public event EventHandler SelectionChanged;
        public event DataGridViewCellEventHandler CellClick;
        internal void CellValueChanagedHandler(int column, int row)
        {
            if (CellValueChanged != null)
            {
                CellValueChanged(this, new DataGridViewCellEventArgs(column, row));
            }
        }
        public event DataGridViewCellEventHandler CellValueChanged;
        public void SetExpandRow(DataGridViewRow row, bool all)
        {
            GridView.ExpandRow(Store.GetPath(row.TreeIter), all);
        }
        public void SetCollapseRow(DataGridViewRow row)
        {
            GridView.CollapseRow(Store.GetPath(row.TreeIter));
        }
        private bool _MultiSelect = true;
        public bool MultiSelect { 
            get => _MultiSelect; 
            set {
                _MultiSelect = value;
                GridView.ActivateOnSingleClick = !_MultiSelect;
                if (_SelectionMode == DataGridViewSelectionMode.CellSelect)
                {
                    GridView.Selection.Mode = Gtk.SelectionMode.None;
                }
                else
                {
                    if (_MultiSelect == true)
                        GridView.Selection.Mode = Gtk.SelectionMode.Multiple;
                    else
                        GridView.Selection.Mode = Gtk.SelectionMode.Single;
                }
            } 
        }

        private DataGridViewSelectionMode _SelectionMode = DataGridViewSelectionMode.RowHeaderSelect;
        public DataGridViewSelectionMode SelectionMode {
            get => _SelectionMode;
            set {
                _SelectionMode = value;
                switch (_SelectionMode)
                {
                    case DataGridViewSelectionMode.CellSelect:
                        {
                            GridView.Selection.Mode = Gtk.SelectionMode.None;
                            break;
                        }

                    case DataGridViewSelectionMode.FullColumnSelect:
                    case DataGridViewSelectionMode.ColumnHeaderSelect:
                        {
                            GridView.Selection.Mode = Gtk.SelectionMode.Multiple;
                            break;
                        }

                    case DataGridViewSelectionMode.FullRowSelect:
                    case DataGridViewSelectionMode.RowHeaderSelect:
                        {
                            GridView.Selection.Mode = Gtk.SelectionMode.Multiple;
                            break;
                        }
                }
                
            } }
        public string Markup { get; set; } = "...";
        public bool ReadOnly { get; set; }
        public int RowHeadersWidth { get; set; }
        public int ColumnHeadersHeight { get; set; }
        public DataGridViewColumnHeadersHeightSizeMode ColumnHeadersHeightSizeMode
        {
            get;set;
        }
        public DataGridViewAutoSizeRowsMode AutoSizeRowsMode { get; set; }
        private DataGridViewRow _RowTemplate;
        public DataGridViewRow RowTemplate
        {
            get { return _RowTemplate ??= new DataGridViewRow(); }
            set { _RowTemplate = value; }
        }
        public DataGridViewCellStyle DefaultCellStyle { get; set; }
        public DataGridViewCellStyle ColumnHeadersDefaultCellStyle { get; set; }
        public DataGridViewCellStyle AlternatingRowsDefaultCellStyle { get; set; }
        public DataGridViewCellStyle RowsDefaultCellStyle { get; set; }
        public DataGridViewCellStyle RowHeadersDefaultCellStyle { get; set; }
        public override ControlBindingsCollection DataBindings { get => _collect; }
        private object _DataSource;
        public object DataSource
        {
            get { return _DataSource; }
            set
            {
                _DataSource = value;
                if (GridView.IsVisible)
                {
                    OnSetDataSource();
                }
            }
        }
        private void OnSetDataSource()
        {
            _Created = false;
            _rows.Clear();
            if (_DataSource != null)
            {
                if (_DataSource is DataTable dtable)
                {
                    LoadDataTableSource(dtable);
                }
                else if (_DataSource is DataSet dset)
                {
                    if (string.IsNullOrEmpty(DataMember))
                        LoadDataTableSource(dset.Tables[0]);
                    else
                        LoadDataTableSource(dset.Tables[DataMember]);
                }
                else if (_DataSource is DataView dview)
                {
                    LoadDataTableSource(dview.Table);
                }
                else
                {
                    LoadListSource();
                }
            }
            _Created = true;
        }
        public string DataMember { get; set; }
        private void LoadDataTableSource(DataTable dt)
        {
            foreach (DataColumn col in dt.Columns)
            {
                if (_columns.Exists(m => m.DataPropertyName == col.ColumnName) == false)
                {
                    if (col.DataType.Name == "Boolean")
                        _columns.Add(new DataGridViewCheckBoxColumn(this) { Name = col.ColumnName, HeaderText = col.ColumnName, DataPropertyName = col.ColumnName, ValueType = col.DataType });
                    else if (col.DataType.Name == "Image" || col.DataType.Name == "Bitmap")
                        _columns.Add(new DataGridViewImageColumn(this) { Name = col.ColumnName, HeaderText = col.ColumnName, DataPropertyName = col.ColumnName, ValueType = col.DataType });
                    else
                        _columns.Add(new DataGridViewColumn(this) { Name = col.ColumnName, HeaderText = col.ColumnName, DataPropertyName = col.ColumnName, ValueType = col.DataType });
                }
            }
            _columns.Invalidate();

            if (_columns.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    DataGridViewRow newRow = new DataGridViewRow();
                    foreach (DataGridViewColumn col in _columns)
                    {
                        object cellvalue = dt.Columns.Contains(col.DataPropertyName) ? dr[col.DataPropertyName] : null;
                        newRow.Cells.Add(col.NewCell(cellvalue, col.ValueType));
                    }
                    _rows.Add(newRow);
                }
            }
        }
        private void LoadListSource()
        {
            Type _type = _DataSource.GetType();
            Type[] _entityType = _type.GetGenericArguments();
            if (_entityType.Length == 1)
            {
                PropertyInfo[] pros = _entityType[0].GetProperties(BindingFlags.Public|BindingFlags.Instance);
                foreach (PropertyInfo pro in pros)
                {
                    if (_columns.Exists(m => m.DataPropertyName == pro.Name) == false)
                    {
                        if (pro.PropertyType.Name == "Boolean")
                            _columns.Add(new DataGridViewCheckBoxColumn(this) { Name = pro.Name, HeaderText = pro.Name, DataPropertyName = pro.Name, ValueType = pro.PropertyType });
                        else if (pro.PropertyType.Name == "Image" || pro.PropertyType.Name == "Bitmap")
                            _columns.Add(new DataGridViewImageColumn(this) { Name = pro.Name, HeaderText = pro.Name, DataPropertyName = pro.Name, ValueType = pro.PropertyType });
                        else
                            _columns.Add(new DataGridViewColumn(this) { Name = pro.Name, HeaderText = pro.Name, DataPropertyName = pro.Name, ValueType = pro.PropertyType });
                    }
                }
                _columns.Invalidate();

                if (_columns.Count > 0)
                {
                    IEnumerator reader = ((IEnumerable)_DataSource).GetEnumerator();
                    while (reader.MoveNext())
                    {
                        object obj = reader.Current;
                        Type type = obj.GetType();
                        DataGridViewRow newRow = new DataGridViewRow();
                        foreach (DataGridViewColumn col in _columns)
                        {
                            object cellvalue = type.GetProperty(col.DataPropertyName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty)?.GetValue(obj);
                            newRow.Cells.Add(col.NewCell(cellvalue, col.ValueType));
                        }
                        _rows.Add(newRow);
                    }
                }
            }
        }
        public DataGridViewColumnCollection Columns
        {
            get
            {
                return _columns;
            }
        }
        public DataGridViewRowCollection Rows { get { return _rows; } }
        [Browsable(false)]
        public DataGridViewSelectedCellCollection SelectedCells
        {
            get
            {
                DataGridViewSelectedCellCollection stcc = new DataGridViewSelectedCellCollection();
                switch (SelectionMode)
                {
                    case DataGridViewSelectionMode.CellSelect:
                        {
                            int cols = Store.NColumns;
                            Store.Foreach(new TreeModelForeachFunc((model, path, iter) => {
                                for (int i = 0; i < cols; i++)
                                {
                                    DataGridViewCell cell = (DataGridViewCell)model.GetValue(iter, i);
                                    if (cell.Selected)
                                        stcc.Add(cell);
                                }
                                return false;
                            }));
                            break;
                        }

                    case DataGridViewSelectionMode.FullColumnSelect:
                    case DataGridViewSelectionMode.ColumnHeaderSelect:
                        {
                            foreach (int columnIndex in _selectedBandIndexes)
                            {
                                foreach (DataGridViewRow dataGridViewRow in Rows)   // unshares all rows!
                                {
                                    stcc.Add(dataGridViewRow.Cells[columnIndex]);
                                }
                            }
                            break;
                        }

                    case DataGridViewSelectionMode.FullRowSelect:
                    case DataGridViewSelectionMode.RowHeaderSelect:
                        {
                            foreach (int rowIndex in _selectedBandIndexes)
                            {
                                DataGridViewRow dataGridViewRow = (DataGridViewRow)Rows[rowIndex];
                                foreach (DataGridViewCell dataGridViewCell in dataGridViewRow.Cells)
                                {
                                    stcc.Add(dataGridViewCell);
                                }
                            }
                            break;
                        }
                }

                return stcc;
            }
        }

        [Browsable(false)]
        public DataGridViewSelectedColumnCollection SelectedColumns
        {
            get
            {
                DataGridViewSelectedColumnCollection strc = new DataGridViewSelectedColumnCollection();
                switch (SelectionMode)
                {
                    case DataGridViewSelectionMode.CellSelect:
                    case DataGridViewSelectionMode.FullRowSelect:
                    case DataGridViewSelectionMode.RowHeaderSelect:
                        break;
                    case DataGridViewSelectionMode.FullColumnSelect:
                    case DataGridViewSelectionMode.ColumnHeaderSelect:
                        foreach (int columnIndex in _selectedBandIndexes)
                        {
                            strc.Add(Columns[columnIndex]);
                        }

                        break;
                }

                return strc;
            }
        }

        [Browsable(false)]
        public DataGridViewSelectedRowCollection SelectedRows
        {
            get
            {
                DataGridViewSelectedRowCollection strc = new DataGridViewSelectedRowCollection();
                switch (SelectionMode)
                {
                    case DataGridViewSelectionMode.CellSelect:
                    case DataGridViewSelectionMode.FullColumnSelect:
                    case DataGridViewSelectionMode.ColumnHeaderSelect:
                        break;
                    case DataGridViewSelectionMode.FullRowSelect:
                    case DataGridViewSelectionMode.RowHeaderSelect:
                        foreach (int rowIndex in _selectedBandIndexes)
                        {
                            strc.Add(Rows[rowIndex]);
                        }
                        break;
                }

                return strc;
            }
        }
        public bool NativeRowGetSelected(int rowindex)
        {
            switch (SelectionMode)
            {
                case DataGridViewSelectionMode.FullRowSelect:
                case DataGridViewSelectionMode.RowHeaderSelect:
                    return _selectedBandIndexes.Any(i => i == rowindex);
                    break;
                default:
                    return false;
            }
        }
        public void NativeRowSetSelected(int rowindex,bool selected)
        {
            Gtk.Application.Invoke(delegate
            {
                if (selected)
                    GridView.Selection.SelectIter(Rows[rowindex].TreeIter);
                else
                    GridView.Selection.UnselectIter(Rows[rowindex].TreeIter);
            });
        }

        public override void BeginInit()
        {
            _Created = false;
        }

        public override void EndInit()
        {
            _Created = true;
        }
        public void ClearSelection()
        {
            GridView.Selection.UnselectAll();
        }
        public void OnCellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (CellPainting != null)
                CellPainting(sender, e);
        }
        //[Obsolete(" this event is not implemented， develop independently")]
        //public event EventHandler BackgroundImageChanged;
        [Obsolete(" this event is not implemented， develop independently")]
        public event EventHandler BackgroundColorChanged;
        //[Obsolete(" this event is not implemented， develop independently")]
        //public event EventHandler BackColorChanged;
        [Obsolete(" this event is not implemented， develop independently")]
        public event DataGridViewAutoSizeModeEventHandler AutoSizeRowsModeChanged;
        [Obsolete(" this event is not implemented， develop independently")]
        public event DataGridViewAutoSizeColumnsModeEventHandler AutoSizeColumnsModeChanged;
        [Obsolete(" this event is not implemented， develop independently")]
        public event EventHandler AutoGenerateColumnsChanged;
        [Obsolete(" this event is not implemented， develop independently")]
        public event EventHandler AlternatingRowsDefaultCellStyleChanged;
        [Obsolete(" this event is not implemented， develop independently")]
        public event EventHandler AllowUserToResizeRowsChanged;
        [Obsolete(" this event is not implemented， develop independently")]
        public event EventHandler AllowUserToResizeColumnsChanged;
        [Obsolete(" this event is not implemented， develop independently")]
        public event EventHandler AllowUserToDeleteRowsChanged;
        [Obsolete(" this event is not implemented， develop independently")]
        public event EventHandler AllowUserToAddRowsChanged;
        [Obsolete(" this event is not implemented， develop independently")]
        public event EventHandler Sorted;
        [Obsolete(" this event is not implemented， develop independently")]
        public event DataGridViewSortCompareEventHandler SortCompare;
        //[Obsolete(" this event is not implemented， develop independently")]
        //public event EventHandler SelectionChanged;
        [Obsolete(" this event is not implemented， develop independently")]
        public event EventHandler AllowUserToOrderColumnsChanged;
        //[Obsolete(" this event is not implemented， develop independently")]
        //public event EventHandler StyleChanged;
        //[Obsolete(" this event is not implemented， develop independently")]
        //public event EventHandler BackgroundImageLayoutChanged;
        [Obsolete(" this event is not implemented， develop independently")]
        public event EventHandler CellBorderStyleChanged;
        [Obsolete(" this event is not implemented， develop independently")]
        public event DataGridViewDataErrorEventHandler DataError;
        [Obsolete(" this event is not implemented， develop independently")]
        public event DataGridViewBindingCompleteEventHandler DataBindingComplete;
        [Obsolete(" this event is not implemented， develop independently")]
        public event EventHandler CurrentCellDirtyStateChanged;
        [Obsolete(" this event is not implemented， develop independently")]
        public event EventHandler CurrentCellChanged;
        [Obsolete(" this event is not implemented， develop independently")]
        public event DataGridViewColumnEventHandler ColumnWidthChanged;
        [Obsolete(" this event is not implemented， develop independently")]
        public event DataGridViewColumnEventHandler ColumnToolTipTextChanged;
        [Obsolete(" this event is not implemented， develop independently")]
        public event DataGridViewColumnStateChangedEventHandler ColumnStateChanged;
        [Obsolete(" this event is not implemented， develop independently")]
        public event DataGridViewColumnEventHandler ColumnSortModeChanged;
        [Obsolete(" this event is not implemented， develop independently")]
        public event DataGridViewColumnEventHandler ColumnRemoved;
        [Obsolete(" this event is not implemented， develop independently")]
        public event DataGridViewColumnEventHandler ColumnNameChanged;
        [Obsolete(" this event is not implemented， develop independently")]
        public event DataGridViewRowEventHandler DefaultValuesNeeded;
        [Obsolete(" this event is not implemented， develop independently")]
        public event DataGridViewColumnEventHandler ColumnMinimumWidthChanged;
        [Obsolete(" this event is not implemented， develop independently")]
        public event DataGridViewCellMouseEventHandler ColumnHeaderMouseDoubleClick;
        [Obsolete(" this event is not implemented， develop independently")]
        public event DataGridViewCellMouseEventHandler ColumnHeaderMouseClick;
        [Obsolete(" this event is not implemented， develop independently")]
        public event DataGridViewColumnEventHandler ColumnDividerWidthChanged;
        [Obsolete(" this event is not implemented， develop independently")]
        public event DataGridViewColumnDividerDoubleClickEventHandler ColumnDividerDoubleClick;
        [Obsolete(" this event is not implemented， develop independently")]
        public event DataGridViewColumnEventHandler ColumnDisplayIndexChanged;
        [Obsolete(" this event is not implemented， develop independently")]
        public event DataGridViewColumnEventHandler ColumnDefaultCellStyleChanged;
        [Obsolete(" this event is not implemented， develop independently")]
        public event DataGridViewColumnEventHandler ColumnDataPropertyNameChanged;
        [Obsolete(" this event is not implemented， develop independently")]
        public event DataGridViewColumnEventHandler ColumnContextMenuStripChanged;
        [Obsolete(" this event is not implemented， develop independently")]
        public event DataGridViewColumnEventHandler ColumnAdded;
        [Obsolete(" this event is not implemented， develop independently")]
        public event DataGridViewCellValueEventHandler CellValuePushed;
        [Obsolete(" this event is not implemented， develop independently")]
        public event DataGridViewColumnEventHandler ColumnHeaderCellChanged;
        [Obsolete(" this event is not implemented， develop independently")]
        public event DataGridViewCellValueEventHandler CellValueNeeded;
        [Obsolete(" this event is not implemented， develop independently")]
        public event DataGridViewEditingControlShowingEventHandler EditingControlShowing;
        [Obsolete(" this event is not implemented， develop independently")]
        public event DataGridViewRowEventHandler RowContextMenuStripChanged;
        [Obsolete(" this event is not implemented， develop independently")]
        public event DataGridViewRowEventHandler RowUnshared;
        [Obsolete(" this event is not implemented， develop independently")]
        public event DataGridViewRowStateChangedEventHandler RowStateChanged;
        [Obsolete(" this event is not implemented， develop independently")]
        public event DataGridViewRowsRemovedEventHandler RowsRemoved;
        [Obsolete(" this event is not implemented， develop independently")]
        public event DataGridViewRowsAddedEventHandler RowsAdded;
        [Obsolete(" this event is not implemented， develop independently")]
        public event DataGridViewRowPrePaintEventHandler RowPrePaint;
        [Obsolete(" this event is not implemented， develop independently")]
        public event DataGridViewRowPostPaintEventHandler RowPostPaint;
        [Obsolete(" this event is not implemented， develop independently")]
        public event DataGridViewRowEventHandler RowMinimumHeightChanged;
        [Obsolete(" this event is not implemented， develop independently")]
        public event DataGridViewCellEventHandler RowLeave;
        [Obsolete(" this event is not implemented， develop independently")]
        public event DataGridViewRowHeightInfoPushedEventHandler RowHeightInfoPushed;
        [Obsolete(" this event is not implemented， develop independently")]
        public event DataGridViewRowHeightInfoNeededEventHandler RowHeightInfoNeeded;
        [Obsolete(" this event is not implemented， develop independently")]
        public event DataGridViewRowEventHandler NewRowNeeded;
        [Obsolete(" this event is not implemented， develop independently")]
        public event DataGridViewRowEventHandler RowHeightChanged;
        [Obsolete(" this event is not implemented， develop independently")]
        public event DataGridViewCellMouseEventHandler RowHeaderMouseDoubleClick;
        [Obsolete(" this event is not implemented， develop independently")]
        public event DataGridViewCellMouseEventHandler RowHeaderMouseClick;
        [Obsolete(" this event is not implemented， develop independently")]
        public event DataGridViewRowErrorTextNeededEventHandler RowErrorTextNeeded;
        [Obsolete(" this event is not implemented， develop independently")]
        public event DataGridViewRowEventHandler RowErrorTextChanged;
        [Obsolete(" this event is not implemented， develop independently")]
        public event DataGridViewCellEventHandler RowEnter;
        [Obsolete(" this event is not implemented， develop independently")]
        public event DataGridViewRowEventHandler RowDividerHeightChanged;
        [Obsolete(" this event is not implemented， develop independently")]
        public event DataGridViewRowDividerDoubleClickEventHandler RowDividerDoubleClick;
        [Obsolete(" this event is not implemented， develop independently")]
        public event QuestionEventHandler RowDirtyStateNeeded;
        [Obsolete(" this event is not implemented， develop independently")]
        public event DataGridViewRowEventHandler RowDefaultCellStyleChanged;
        [Obsolete(" this event is not implemented， develop independently")]
        public event DataGridViewRowContextMenuStripNeededEventHandler RowContextMenuStripNeeded;
        [Obsolete(" this event is not implemented， develop independently")]
        public event DataGridViewRowEventHandler RowHeaderCellChanged;
        //[Obsolete(" this event is not implemented， develop independently")]
        //public event DataGridViewCellEventHandler CellValueChanged;
        [Obsolete(" this event is not implemented， develop independently")]
        public event DataGridViewCellValidatingEventHandler CellValidating;
        [Obsolete(" this event is not implemented， develop independently")]
        public event DataGridViewCellEventHandler CellValidated;
        [Obsolete(" this event is not implemented， develop independently")]
        public event QuestionEventHandler CancelRowEdit;
        [Obsolete(" this event is not implemented， develop independently")]
        public event DataGridViewAutoSizeColumnModeEventHandler AutoSizeColumnModeChanged;
        //[Obsolete(" this event is not implemented， develop independently")]
        //public event EventHandler TextChanged;
        [Obsolete(" this event is not implemented， develop independently")]
        public event EventHandler RowsDefaultCellStyleChanged;
        [Obsolete(" this event is not implemented， develop independently")]
        public event DataGridViewAutoSizeModeEventHandler RowHeadersWidthSizeModeChanged;
        [Obsolete(" this event is not implemented， develop independently")]
        public event EventHandler RowHeadersWidthChanged;
        [Obsolete(" this event is not implemented， develop independently")]
        public event EventHandler RowHeadersDefaultCellStyleChanged;
        [Obsolete(" this event is not implemented， develop independently")]
        public event EventHandler RowHeadersBorderStyleChanged;
        [Obsolete(" this event is not implemented， develop independently")]
        public event EventHandler ReadOnlyChanged;
        //[Obsolete(" this event is not implemented， develop independently")]
        //public event EventHandler PaddingChanged;
        [Obsolete(" this event is not implemented， develop independently")]
        public event DataGridViewCellCancelEventHandler CellBeginEdit;
        [Obsolete(" this event is not implemented， this event is not implemented， develop independently")]
        public event EventHandler MultiSelectChanged;
        //[Obsolete(" this event is not implemented， develop independently")]
        //public event EventHandler FontChanged;
        //[Obsolete(" this event is not implemented， develop independently")]
        //public event EventHandler ForeColorChanged;
        [Obsolete(" this event is not implemented， develop independently")]
        public event EventHandler EditModeChanged;
        [Obsolete(" this event is not implemented， develop independently")]
        public event EventHandler DefaultCellStyleChanged;
        [Obsolete(" this event is not implemented， develop independently")]
        public event EventHandler DataSourceChanged;
        [Obsolete(" this event is not implemented， develop independently")]
        public event EventHandler DataMemberChanged;
        [Obsolete(" this event is not implemented， develop independently")]
        public event DataGridViewAutoSizeModeEventHandler ColumnHeadersHeightSizeModeChanged;
        [Obsolete(" this event is not implemented， develop independently")]
        public event EventHandler ColumnHeadersHeightChanged;
        [Obsolete(" this event is not implemented， develop independently")]
        public event EventHandler ColumnHeadersDefaultCellStyleChanged;
        [Obsolete(" this event is not implemented， develop independently")]
        public event EventHandler ColumnHeadersBorderStyleChanged;
        [Obsolete(" this event is not implemented， develop independently")]
        public event EventHandler GridColorChanged;
        //[Obsolete(" this event is not implemented， develop independently")]
        //public event DataGridViewCellEventHandler CellClick;
        [Obsolete(" this event is not implemented， develop independently")]
        public event DataGridViewCellEventHandler CellContentClick;
        [Obsolete(" this event is not implemented， develop independently")]
        public event DataGridViewCellEventHandler CellContentDoubleClick;
        [Obsolete(" this event is not implemented， develop independently")]
        public event DataGridViewCellToolTipTextNeededEventHandler CellToolTipTextNeeded;
        [Obsolete(" this event is not implemented， develop independently")]
        public event DataGridViewCellEventHandler CellToolTipTextChanged;
        [Obsolete(" this event is not implemented， develop independently")]
        public event DataGridViewCellStyleContentChangedEventHandler CellStyleContentChanged;
        [Obsolete(" this event is not implemented， develop independently")]
        public event DataGridViewCellEventHandler CellStyleChanged;
        [Obsolete(" this event is not implemented， develop independently")]
        public event DataGridViewCellStateChangedEventHandler CellStateChanged;
        [Obsolete(" this event is not implemented， develop independently")]
        public event DataGridViewCellParsingEventHandler CellParsing;
        public event DataGridViewCellPaintingEventHandler CellPainting;
        [Obsolete(" this event is not implemented， develop independently")]
        public event DataGridViewCellMouseEventHandler CellMouseUp;
        [Obsolete(" this event is not implemented， develop independently")]
        public event DataGridViewCellMouseEventHandler CellMouseMove;
        [Obsolete(" this event is not implemented， develop independently")]
        public event DataGridViewCellEventHandler CellMouseLeave;
        [Obsolete(" this event is not implemented， develop independently")]
        public event DataGridViewCellEventHandler CellMouseEnter;
        [Obsolete(" this event is not implemented， develop independently")]
        public event DataGridViewCellMouseEventHandler CellMouseDown;
        [Obsolete(" this event is not implemented， develop independently")]
        public event DataGridViewCellMouseEventHandler CellMouseDoubleClick;
        [Obsolete(" this event is not implemented， develop independently")]
        public event DataGridViewCellMouseEventHandler CellMouseClick;
        [Obsolete(" this event is not implemented， develop independently")]
        public event DataGridViewCellEventHandler CellLeave;
        [Obsolete(" this event is not implemented， develop independently")]
        public event DataGridViewCellFormattingEventHandler CellFormatting;
        [Obsolete(" this event is not implemented， develop independently")]
        public event DataGridViewCellErrorTextNeededEventHandler CellErrorTextNeeded;
        [Obsolete(" this event is not implemented， develop independently")]
        public event DataGridViewCellEventHandler CellErrorTextChanged;
        [Obsolete(" this event is not implemented， develop independently")]
        public event DataGridViewCellEventHandler CellEnter;
        [Obsolete(" this event is not implemented， develop independently")]
        public event DataGridViewCellEventHandler CellEndEdit;
        [Obsolete(" this event is not implemented， develop independently")]
        public event DataGridViewCellEventHandler CellDoubleClick;
        [Obsolete(" this event is not implemented， develop independently")]
        public event DataGridViewCellContextMenuStripNeededEventHandler CellContextMenuStripNeeded;
        [Obsolete(" this event is not implemented， develop independently")]
        public event DataGridViewCellEventHandler CellContextMenuStripChanged;
        [Obsolete(" this event is not implemented， develop independently")]
        public event EventHandler BorderStyleChanged;
        [Obsolete(" this event is not implemented， develop independently")]
        public event DataGridViewCellEventHandler RowValidated;
        [Obsolete(" this event is not implemented， develop independently")]
        public event DataGridViewCellCancelEventHandler RowValidating;
        [Obsolete(" this event is not implemented， develop independently")]
        public event DataGridViewRowCancelEventHandler UserDeletingRow;
        [Obsolete(" this event is not implemented， develop independently")]
        public event DataGridViewRowEventHandler UserDeletedRow;
        [Obsolete(" this event is not implemented， develop independently")]
        public event DataGridViewRowEventHandler UserAddedRow;
    }
}