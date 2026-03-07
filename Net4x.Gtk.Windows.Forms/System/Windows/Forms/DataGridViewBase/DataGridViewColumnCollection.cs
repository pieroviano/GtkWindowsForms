/*
 * Based on GTK components, it is a cross-platform interface component that is compatible with the native C# control winform interface.
 * Use this component GtkSystem.Windows.Forms instead of Microsoft.WindowsDesktop.App.WindowsForms, compile once, and run across Windows, Linux, and macOS
 * Technical support 438865652@qq.com，https://www.gtkapp.com, https://gitee.com/easywebfactory, https://github.com/easywebfactory438865652@qq.com，https://www.gtkapp.com, https://gitee.com/easywebfactory, https://github.com/easywebfactory
 * author:chenhongjin
 */
using System.ComponentModel;
using System.Windows.Forms.GtkRender;

namespace System.Windows.Forms
{
    /// <summary>
    /// Represents a collection of
    /// <see cref = "System.Windows.Forms.DataGridViewColumn"/> objects in a
    /// <see cref = "System.Windows.Forms.DataGridView"/> control.
    /// </summary>
    /// <filterpriority>
    /// 2
    /// </filterpriority>
    public class DataGridViewColumnCollection : List<DataGridViewColumn>
    {
        private DataGridView __owner;

        private Gtk.TreeView GridView;

        public event CollectionChangeEventHandler CollectionChanged;

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref = "System.Windows.Forms.DataGridViewColumnCollection"/> class for the given
        /// <see cref = "System.Windows.Forms.DataGridView"/>
        /// </summary>
        /// <param name = "dataGridView">The
        /// <see cref = "System.Windows.Forms.DataGridView"/> that created this collection.
        /// </param>
        public DataGridViewColumnCollection(DataGridView dataGridView)
        {
            __owner = dataGridView;
            GridView = dataGridView.GridView;
        }

        /// <summary>
        /// Gets the
        /// <see cref = "System.Windows.Forms.DataGridView"/> upon which the collection performs column-related operations.
        /// </summary>
        /// <returns>
        /// <see cref = "System.Windows.Forms.DataGridView"/>
        /// </returns>
        protected DataGridView DataGridView
        {
            get
            {
                return __owner;
            }
        }

        public virtual DataGridViewColumn this[string columnName]
        {
            get
            {
                return base.Find(m => m.Name == columnName);
            }
        }

        /// <summary>
        /// Adds a
        /// <see cref = "System.Windows.Forms.DataGridViewTextBoxColumn"/> with the given column name and column header text to the collection.
        /// </summary>
        /// <returns>The index of the column.
        /// </returns>
        /// <param name = "columnName">The name by which the column will be referred.
        /// </param>
        /// <param name = "headerText">The text for the column's header.
        /// </param>
        /// <exception cref = "System.InvalidOperationException">The associated
        /// <see cref = "System.Windows.Forms.DataGridView"/> control is performing one of the following actions that temporarily prevents new columns from being added:Selecting all cells in the control.Clearing the selection.Updating column
        /// <see cref = "System.Windows.Forms.DataGridViewColumn.DisplayIndex"/> property values. -or-This method is being called from a handler for one of the following
        /// <see cref = "System.Windows.Forms.DataGridView"/> events:
        /// <see cref = "System.Windows.Forms.DataGridView.CellEnter"/>
        /// <see cref = "System.Windows.Forms.DataGridView.CellLeave"/>
        /// <see cref = "System.Windows.Forms.DataGridView.CellValidating"/>
        /// <see cref = "System.Windows.Forms.DataGridView.CellValidated"/>
        /// <see cref = "System.Windows.Forms.DataGridView.RowEnter"/>
        /// <see cref = "System.Windows.Forms.DataGridView.RowLeave"/>
        /// <see cref = "System.Windows.Forms.DataGridView.RowValidated"/>
        /// <see cref = "System.Windows.Forms.DataGridView.RowValidating"/>-or-The
        /// <see cref = "System.Windows.Forms.DataGridView.SelectionMode"/> property value is
        /// <see cref = "System.Windows.Forms.DataGridViewSelectionMode.FullColumnSelect"/> or
        /// <see cref = "System.Windows.Forms.DataGridViewSelectionMode.ColumnHeaderSelect"/>, which conflicts with the default column
        /// <see cref = "System.Windows.Forms.DataGridViewColumn.SortMode"/> property value of
        /// <see cref = "System.Windows.Forms.DataGridViewColumnSortMode.Automatic"/>-or-The default column
        /// <see cref = "System.Windows.Forms.DataGridViewColumn.FillWeight"/> property value of 100 would cause the combined
        /// <see cref = "System.Windows.Forms.DataGridViewColumn.FillWeight"/> values of all columns in the control to exceed 65535.
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence"/>
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        /// </PermissionSet>
        public void Add(string columnName, string headerText)
        {
            DataGridViewColumn column = new DataGridViewColumn()
            {
                Name = columnName,
                HeaderText = headerText
            };
            Add(column);
        }

        /// <summary>
        /// Adds the given column to the collection.
        /// </summary>
        /// <returns>The index of the column.
        /// </returns>
        /// <param name = "dataGridViewColumn">The
        /// <see cref = "System.Windows.Forms.DataGridViewColumn"/> to add.
        /// </param>
        /// <exception cref = "System.ArgumentNullException">
        /// <paramref name = "dataGridViewColumn"/> is null.
        /// </exception>
        /// <exception cref = "System.InvalidOperationException">The associated
        /// <see cref = "System.Windows.Forms.DataGridView"/> control is performing one of the following actions that temporarily prevents new columns from being added:Selecting all cells in the control.Clearing the selection.Updating column
        /// <see cref = "System.Windows.Forms.DataGridViewColumn.DisplayIndex"/> property values. -or-This method is being called from a handler for one of the following
        /// <see cref = "System.Windows.Forms.DataGridView"/> events:
        /// <see cref = "System.Windows.Forms.DataGridView.CellEnter"/>
        /// <see cref = "System.Windows.Forms.DataGridView.CellLeave"/>
        /// <see cref = "System.Windows.Forms.DataGridView.CellValidating"/>
        /// <see cref = "System.Windows.Forms.DataGridView.CellValidated"/>
        /// <see cref = "System.Windows.Forms.DataGridView.RowEnter"/>
        /// <see cref = "System.Windows.Forms.DataGridView.RowLeave"/>
        /// <see cref = "System.Windows.Forms.DataGridView.RowValidated"/>
        /// <see cref = "System.Windows.Forms.DataGridView.RowValidating"/>-or-
        /// <paramref name = "dataGridViewColumn"/> already belongs to a
        /// <see cref = "System.Windows.Forms.DataGridView"/> control.-or-The
        /// <paramref name = "dataGridViewColumn"/>
        /// <see cref = "System.Windows.Forms.DataGridViewColumn.SortMode"/> property value is
        /// <see cref = "System.Windows.Forms.DataGridViewColumnSortMode.Automatic"/> and the
        /// <see cref = "System.Windows.Forms.DataGridView.SelectionMode"/> property value is
        /// <see cref = "System.Windows.Forms.DataGridViewSelectionMode.FullColumnSelect"/> or
        /// <see cref = "System.Windows.Forms.DataGridViewSelectionMode.ColumnHeaderSelect"/> Use the control
        /// <see cref = "System.Windows.Forms.DataGridView.System # ComponentModel # ISupportInitialize # BeginInit"/> and
        /// <see cref = "System.Windows.Forms.DataGridView.System # ComponentModel # ISupportInitialize # EndInit"/> methods to temporarily set conflicting property values. -or-The
        /// <paramref name = "dataGridViewColumn"/>
        /// <see cref = "System.Windows.Forms.DataGridViewColumn.InheritedAutoSizeMode"/> property value is
        /// <see cref = "System.Windows.Forms.DataGridViewAutoSizeColumnMode.ColumnHeader"/> and the
        /// <see cref = "System.Windows.Forms.DataGridView.ColumnHeadersVisible"/> property value is false.-or-
        /// <paramref name = "dataGridViewColumn"/> has an
        /// <see cref = "System.Windows.Forms.DataGridViewColumn.InheritedAutoSizeMode"/> property value of
        /// <see cref = "System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill"/> and a
        /// <see cref = "System.Windows.Forms.DataGridViewColumn.Frozen"/> property value of true.-or-
        /// <paramref name = "dataGridViewColumn"/> has a
        /// <see cref = "System.Windows.Forms.DataGridViewColumn.FillWeight"/> property value that would cause the combined
        /// <see cref = "System.Windows.Forms.DataGridViewColumn.FillWeight"/> values of all columns in the control to exceed 65535.-or-
        /// <paramref name = "dataGridViewColumn"/> has
        /// <see cref = "System.Windows.Forms.DataGridViewColumn.DisplayIndex"/> and
        /// <see cref = "System.Windows.Forms.DataGridViewColumn.Frozen"/> property values that would display it among a set of adjacent columns with the opposite
        /// <see cref = "System.Windows.Forms.DataGridViewColumn.Frozen"/> property value.-or-The
        /// <see cref = "System.Windows.Forms.DataGridView"/> control contains at least one row and
        /// <paramref name = "dataGridViewColumn"/> has a
        /// <see cref = "System.Windows.Forms.DataGridViewColumn.CellType"/> property value of null.
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence"/>
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        /// </PermissionSet>
        public new void Add(DataGridViewColumn column)
        {
            column.DataGridView = __owner;
            column.Index = Count;
            column.DisplayIndex = column.Index;
            column.SetGridViewDefaultStyle(__owner.DefaultCellStyle);
            DataGridViewCellStyle _cellStyle = column.DefaultCellStyle;
            if (__owner.ColumnHeadersDefaultCellStyle != null)
                _cellStyle = __owner.ColumnHeadersDefaultCellStyle;
            if (_cellStyle != null)
            {
                Gtk.Button header = ((Gtk.Button)column.Button);
                string style = "";
                if (_cellStyle.BackColor.Name != "0")
                {
                    string backcolor = $"rgba({_cellStyle.BackColor.R},{_cellStyle.BackColor.G},{_cellStyle.BackColor.B},{_cellStyle.BackColor.A})";
                    style += $".columnheaderbackcolor{{background-color:{backcolor};}} ";
                    header.StyleContext.AddClass("columnheaderbackcolor");
                }

                if (_cellStyle.ForeColor.Name != "0")
                {
                    string forecolor = $"rgba({_cellStyle.ForeColor.R},{_cellStyle.ForeColor.G},{_cellStyle.ForeColor.B},{_cellStyle.ForeColor.A})";
                    style += $".columnheaderforecolor{{color:{forecolor};}} ";
                    header.StyleContext.AddClass("columnheaderforecolor");
                }

                if (style.Length > 9)
                {
                    Gtk.CssProvider css = new Gtk.CssProvider();
                    css.LoadFromData(style);
                    header.StyleContext.AddProvider(css, 800);
                }

                switch (_cellStyle.Alignment)
                {
                    case DataGridViewContentAlignment.TopLeft:
                    case DataGridViewContentAlignment.MiddleLeft:
                    case DataGridViewContentAlignment.BottomLeft:
                        column.Alignment = 0f;
                        break;
                    case DataGridViewContentAlignment.TopCenter:
                    case DataGridViewContentAlignment.MiddleCenter:
                    case DataGridViewContentAlignment.BottomCenter:
                        column.Alignment = 0.5f;
                        break;
                    case DataGridViewContentAlignment.TopRight:
                    case DataGridViewContentAlignment.MiddleRight:
                    case DataGridViewContentAlignment.BottomRight:
                        column.Alignment = 1.0f;
                        break;
                    default:
                        column.Alignment = 0f;
                        break;
                }
            }

            base.Add(column);
            if (__owner.self.IsRealized)
            {
                Invalidate();
            }

            GridView.AppendColumn(column);
            OnCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Add, column));
        }

        public new void AddRange(IEnumerable<DataGridViewColumn> columns)
        {
            foreach (DataGridViewColumn column in columns)
            {
                Add(column);
            }
        }

        /// <summary>
        /// Clears the collection.
        /// </summary>
        /// <exception cref = "System.InvalidOperationException">The associated
        /// <see cref = "System.Windows.Forms.DataGridView"/> control is performing one of the following actions that temporarily prevents new columns from being added:Selecting all cells in the control.Clearing the selection.Updating column
        /// <see cref = "System.Windows.Forms.DataGridViewColumn.DisplayIndex"/> property values. -or-This method is being called from a handler for one of the following
        /// <see cref = "System.Windows.Forms.DataGridView"/> events:
        /// <see cref = "System.Windows.Forms.DataGridView.CellEnter"/>
        /// <see cref = "System.Windows.Forms.DataGridView.CellLeave"/>
        /// <see cref = "System.Windows.Forms.DataGridView.CellValidating"/>
        /// <see cref = "System.Windows.Forms.DataGridView.CellValidated"/>
        /// <see cref = "System.Windows.Forms.DataGridView.RowEnter"/>
        /// <see cref = "System.Windows.Forms.DataGridView.RowLeave"/>
        /// <see cref = "System.Windows.Forms.DataGridView.RowValidated"/>
        /// <see cref = "System.Windows.Forms.DataGridView.RowValidating"/>
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence"/>
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        /// </PermissionSet>
        public new void Clear()
        {
            base.Clear();
            foreach (var wik in GridView.Columns)
                GridView.RemoveColumn(wik);
            OnCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Add, __owner));
        }

        /// <summary>
        /// Returns the number of columns that meet the given filter requirements.
        /// </summary>
        /// <returns>The number of columns that meet the filter requirements.
        /// </returns>
        /// <param name = "includeFilter">A bitwise combination of the
        /// <see cref = "System.Windows.Forms.DataGridViewElementStates"/> values that represent the filter for inclusion.
        /// </param>
        /// <exception cref = "System.ArgumentException">
        /// <paramref name = "includeFilter"/> is not a valid bitwise combination of
        /// <see cref = "System.Windows.Forms.DataGridViewElementStates"/> values.
        /// </exception>
        public int GetColumnCount(DataGridViewElementStates includeFilter)
        {
            return FindAll(m => m.State == includeFilter).Count;
        }

        /// <summary>
        /// Returns the width, in pixels, required to display all of the columns that meet the given filter requirements.
        /// </summary>
        /// <returns>The width, in pixels, that is necessary to display all of the columns that meet the filter requirements.
        /// </returns>
        /// <param name = "includeFilter">A bitwise combination of the
        /// <see cref = "System.Windows.Forms.DataGridViewElementStates"/> values that represent the filter for inclusion.
        /// </param>
        /// <exception cref = "System.ArgumentException">
        /// <paramref name = "includeFilter"/> is not a valid bitwise combination of
        /// <see cref = "System.Windows.Forms.DataGridViewElementStates"/> values.
        /// </exception>
        public int GetColumnsWidth(DataGridViewElementStates includeFilter)
        {
            DataGridViewColumn co = Find(m => m.State == includeFilter);

            return co == null ? __owner.RowHeadersWidth : co.Width;
        }

        /// <summary>
        /// Returns the first column in display order that meets the given inclusion-filter requirements.
        /// </summary>
        /// <returns>The first column in display order that meets the given filter requirements, or null if no column is found.
        /// </returns>
        /// <param name = "includeFilter">A bitwise combination of the
        /// <see cref = "System.Windows.Forms.DataGridViewElementStates"/> values that represents the filter for inclusion.
        /// </param>
        /// <exception cref = "System.ArgumentException">
        /// <paramref name = "includeFilter"/> is not a valid bitwise combination of
        /// <see cref = "System.Windows.Forms.DataGridViewElementStates"/> values.
        /// </exception>
        public DataGridViewColumn GetFirstColumn(DataGridViewElementStates includeFilter)
        {
            return Find(m => m.State == includeFilter);
        }

        /// <summary>
        /// Returns the first column in display order that meets the given inclusion-filter and exclusion-filter requirements.
        /// </summary>
        /// <returns>The first column in display order that meets the given filter requirements, or null if no column is found.
        /// </returns>
        /// <param name = "includeFilter">A bitwise combination of the
        /// <see cref = "System.Windows.Forms.DataGridViewElementStates"/> values that represent the filter to apply for inclusion.
        /// </param>
        /// <param name = "excludeFilter">A bitwise combination of the
        /// <see cref = "System.Windows.Forms.DataGridViewElementStates"/> values that represent the filter to apply for exclusion.
        /// </param>
        /// <exception cref = "System.ArgumentException">At least one of the filter values is not a valid bitwise combination of
        /// <see cref = "System.Windows.Forms.DataGridViewElementStates"/> values.
        /// </exception>
        public DataGridViewColumn GetFirstColumn(DataGridViewElementStates includeFilter, DataGridViewElementStates excludeFilter)
        {
            return Find(m => m.State == includeFilter && m.State == excludeFilter);
        }

        /// <summary>
        /// Returns the last column in display order that meets the given filter requirements.
        /// </summary>
        /// <returns>The last displayed column in display order that meets the given filter requirements, or null if no column is found.
        /// </returns>
        /// <param name = "includeFilter">A bitwise combination of the
        /// <see cref = "System.Windows.Forms.DataGridViewElementStates"/> values that represent the filter to apply for inclusion.
        /// </param>
        /// <param name = "excludeFilter">A bitwise combination of the
        /// <see cref = "System.Windows.Forms.DataGridViewElementStates"/> values that represent the filter to apply for exclusion.
        /// </param>
        /// <exception cref = "System.ArgumentException">At least one of the filter values is not a valid bitwise combination of
        /// <see cref = "System.Windows.Forms.DataGridViewElementStates"/> values.
        /// </exception>
        public DataGridViewColumn GetLastColumn(DataGridViewElementStates includeFilter, DataGridViewElementStates excludeFilter)
        {
            return FindLast(m => m.State == includeFilter && m.State == excludeFilter);
        }

        /// <summary>
        /// Gets the first column after the given column in display order that meets the given filter requirements.
        /// </summary>
        /// <returns>The next column that meets the given filter requirements, or null if no column is found.
        /// </returns>
        /// <param name = "dataGridViewColumnStart">The column from which to start searching for the next column.
        /// </param>
        /// <param name = "includeFilter">A bitwise combination of the
        /// <see cref = "System.Windows.Forms.DataGridViewElementStates"/> values that represent the filter to apply for inclusion.
        /// </param>
        /// <param name = "excludeFilter">A bitwise combination of the
        /// <see cref = "System.Windows.Forms.DataGridViewElementStates"/> values that represent the filter to apply for exclusion.
        /// </param>
        /// <exception cref = "System.ArgumentNullException">
        /// <paramref name = "dataGridViewColumnStart"/> is null.
        /// </exception>
        /// <exception cref = "System.ArgumentException">At least one of the filter values is not a valid bitwise combination of
        /// <see cref = "System.Windows.Forms.DataGridViewElementStates"/> values.
        /// </exception>
        public DataGridViewColumn GetNextColumn(DataGridViewColumn dataGridViewColumnStart, DataGridViewElementStates includeFilter, DataGridViewElementStates excludeFilter)
        {
            int ix = FindIndex(m => m.Name == dataGridViewColumnStart.Name && m.State == includeFilter && m.State == excludeFilter);

            return ix < Count ? base[ix] : null;
        }

        public void Invalidate()
        {
            if (Count > __owner.Store.NColumns)
            {
                object[] columnTypes = new object[Count];
                __owner.Store.Clear();
                __owner.Store = new Gtk.TreeStore(Array.ConvertAll(columnTypes, o => typeof(DataGridViewCell)));
                __owner.GridView.Model = __owner.Store;
            }
            else if (__owner.GridView.Model == null)
            {
                __owner.GridView.Model = __owner.Store;
            }

            __owner.Store.DefaultSortFunc = new Gtk.TreeIterCompareFunc((Gtk.ITreeModel m, Gtk.TreeIter t1, Gtk.TreeIter t2) =>
            {
                return 0;
            });
            for (int i = 0; i < __owner.Store.NColumns; i++)
            {
                __owner.Store.SetSortFunc(i, new Gtk.TreeIterCompareFunc((Gtk.ITreeModel m, Gtk.TreeIter t1, Gtk.TreeIter t2) =>
                {
                    ((Gtk.TreeStore)m).GetSortColumnId(out int sortid, out Gtk.SortType order);
                    DataGridViewCell v1 = m.GetValue(t1, sortid) as DataGridViewCell;
                    DataGridViewCell v2 = m.GetValue(t2, sortid) as DataGridViewCell;
                    if (v1?.Value == null || v2?.Value == null)
                        return 0;
                    else if (int.TryParse(v1.Value.ToString(), out int rv1) && int.TryParse(v2.Value.ToString(), out int rv2))
                        return (rv2 - rv1);
                    else if (DateTime.TryParse(v1.Value.ToString(), out DateTime rd1) && DateTime.TryParse(v2.Value.ToString(), out DateTime rd2))
                        return (int)((rd2 - rd1).TotalSeconds);
                    else
                        return v2.Value.ToString().CompareTo(v1.Value.ToString());
                }));
            }
        }

        /// <summary>
        /// Removes the specified column from the collection.
        /// </summary>
        /// <param name = "dataGridViewColumn">The column to delete.
        /// </param>
        /// <exception cref = "System.ArgumentException">
        /// <paramref name = "dataGridViewColumn"/> is not in the collection.
        /// </exception>
        /// <exception cref = "System.ArgumentNullException">
        /// <paramref name = "dataGridViewColumn"/> is null.
        /// </exception>
        /// <exception cref = "System.InvalidOperationException">The associated
        /// <see cref = "System.Windows.Forms.DataGridView"/> control is performing one of the following actions that temporarily prevents new columns from being added:Selecting all cells in the control.Clearing the selection.Updating column
        /// <see cref = "System.Windows.Forms.DataGridViewColumn.DisplayIndex"/> property values. -or-This method is being called from a handler for one of the following
        /// <see cref = "System.Windows.Forms.DataGridView"/> events:
        /// <see cref = "System.Windows.Forms.DataGridView.CellEnter"/>
        /// <see cref = "System.Windows.Forms.DataGridView.CellLeave"/>
        /// <see cref = "System.Windows.Forms.DataGridView.CellValidating"/>
        /// <see cref = "System.Windows.Forms.DataGridView.CellValidated"/>
        /// <see cref = "System.Windows.Forms.DataGridView.RowEnter"/>
        /// <see cref = "System.Windows.Forms.DataGridView.RowLeave"/>
        /// <see cref = "System.Windows.Forms.DataGridView.RowValidated"/>
        /// <see cref = "System.Windows.Forms.DataGridView.RowValidating"/>
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence"/>
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        /// </PermissionSet>
        public new bool Remove(DataGridViewColumn column)
        {
            __owner.GridView.RemoveColumn(column);
            bool ir = base.Remove(column);
            OnCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Remove, column));

            return ir;
        }

        /// <summary>
        /// Removes the column at the given index in the collection.
        /// </summary>
        /// <param name = "index">The index of the column to delete.
        /// </param>
        /// <exception cref = "System.ArgumentOutOfRangeException">
        /// <paramref name = "index"/> is less than zero or greater than the number of columns in the control minus one.
        /// </exception>
        /// <exception cref = "System.InvalidOperationException">The associated
        /// <see cref = "System.Windows.Forms.DataGridView"/> control is performing one of the following actions that temporarily prevents new columns from being added:Selecting all cells in the control.Clearing the selection.Updating column
        /// <see cref = "System.Windows.Forms.DataGridViewColumn.DisplayIndex"/> property values. -or-This method is being called from a handler for one of the following
        /// <see cref = "System.Windows.Forms.DataGridView"/> events:
        /// <see cref = "System.Windows.Forms.DataGridView.CellEnter"/>
        /// <see cref = "System.Windows.Forms.DataGridView.CellLeave"/>
        /// <see cref = "System.Windows.Forms.DataGridView.CellValidating"/>
        /// <see cref = "System.Windows.Forms.DataGridView.CellValidated"/>
        /// <see cref = "System.Windows.Forms.DataGridView.RowEnter"/>
        /// <see cref = "System.Windows.Forms.DataGridView.RowLeave"/>
        /// <see cref = "System.Windows.Forms.DataGridView.RowValidated"/>
        /// <see cref = "System.Windows.Forms.DataGridView.RowValidating"/>
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence"/>
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        /// </PermissionSet>
        public new void RemoveAt(int index)
        {
            this.Remove(this[index]);
        }

        /// <summary>
        /// Raises the
        /// <see cref = "System.Windows.Forms.DataGridViewColumnCollection.CollectionChanged"/> event.
        /// </summary>
        /// <param name = "e">A
        /// <see cref = "System.ComponentModel.CollectionChangeEventArgs"/> that contains the event data.
        /// </param>
        protected virtual void OnCollectionChanged(CollectionChangeEventArgs e)
        {
            if (CollectionChanged != null)
                CollectionChanged(__owner, e);
        }

    }
}
