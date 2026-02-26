/*
 * Based on GTK components, it is a cross-platform interface component that is compatible with the native C# control winform interface.
 * Use this component GtkSystem.Windows.Forms instead of Microsoft.WindowsDesktop.App.WindowsForms, compile once, and run across Windows, Linux, and macOS
 * Technical support 438865652@qq.com，https://www.gtkapp.com, https://gitee.com/easywebfactory, https://github.com/easywebfactory438865652@qq.com，https://www.gtkapp.com, https://gitee.com/easywebfactory, https://github.com/easywebfactory
 * author:chenhongjin
 */
using Gtk;
using System.Collections;
using System.ComponentModel;


namespace System.Windows.Forms
{
    /// <summary>
    /// A collection of
    /// <see cref="System.Windows.Forms.DataGridViewRow" /> objects.
    /// </summary>
    /// <filterpriority>
    /// 2
    /// </filterpriority>
    [ListBindable(false)]
    public class DataGridViewRowCollection : ICollection, IEnumerable, IList
    {

        private ArrayList items = new ArrayList();

        private DataGridView dataGridView;

        private DataGridViewRow parentRow;
        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="System.Windows.Forms.DataGridViewRowCollection" /> class.
        /// </summary>
        /// <param name="dataGridView">The
        /// <see cref="System.Windows.Forms.DataGridView" /> that owns the
        /// <see cref="System.Windows.Forms.DataGridViewRowCollection" />
        /// </param>
        public DataGridViewRowCollection(DataGridView dataGridView)
        {
            this.dataGridView = dataGridView;
        }
        public DataGridViewRowCollection(DataGridView dataGridView, DataGridViewRow parentRow)
        {
            this.dataGridView = dataGridView;
            this.parentRow = parentRow;
        }
        private TreeIter AddGtkStore(DataGridViewCellCollection cells)
        {
            int ncolumns = dataGridView.Store.NColumns;
            int columnscount = dataGridView.Columns.Count;
            for (int i = cells.Count; i < columnscount; i++)
                cells.Add(dataGridView.Columns[i].NewCell());
            for (int i = cells.Count; i < ncolumns; i++)
                cells.Add(new DataGridViewTextBoxCell());
            for (int i = 0; i < ncolumns; i++)
                cells[i].ColumnIndex = i;

            if (ncolumns == cells.Count)
                return dataGridView.Store.AppendValues(cells.ConvertAll(o => o).ToArray());
            else
                return dataGridView.Store.AppendValues(cells.ConvertAll(o => o).Take(ncolumns).ToArray());
        }
        private TreeIter AddGtkStore(TreeIter parent, DataGridViewCellCollection cells)
        {
            int ncolumns = dataGridView.Store.NColumns;
            int columnscount = dataGridView.Columns.Count;
            for (int i = cells.Count; i < columnscount; i++)
                cells.Add(dataGridView.Columns[i].NewCell());
            for (int i = cells.Count; i < ncolumns; i++)
                cells.Add(new DataGridViewTextBoxCell());
            for (int i = 0; i < ncolumns; i++)
                cells[i].ColumnIndex = i;
            if (ncolumns == cells.Count)
                return dataGridView.Store.AppendValues(parent, cells.ConvertAll(o => o).ToArray());
            else
                return dataGridView.Store.AppendValues(parent, cells.ConvertAll(o => o).Take(ncolumns).ToArray());
        }
        private void AddGtkStore(params DataGridViewRow[] dataGridViewRows)
        {
            int rowindex = items.Count;
            foreach (DataGridViewRow row in dataGridViewRows)
            {
                row.DataGridView = dataGridView;
                row.Index = rowindex;
                DataGridViewCellStyle _cellStyle = dataGridView.DefaultCellStyle;
                if (dataGridView.RowsDefaultCellStyle != null)
                    _cellStyle = dataGridView.RowsDefaultCellStyle;
                if (rowindex % 2 != 0 && dataGridView.AlternatingRowsDefaultCellStyle != null)
                    _cellStyle = dataGridView.AlternatingRowsDefaultCellStyle;
                if (row.DefaultCellStyle != null)
                    _cellStyle = row.DefaultCellStyle;

                foreach (DataGridViewCell cell in row.Cells)
                {
                    cell.RowIndex = rowindex;
                    cell.RowStyle = _cellStyle;
                }
                items.Add(row);
                rowindex++;
                if (row.TreeIter.Equals(TreeIter.Zero))
                {
                    row.TreeIter = AddGtkStore(row.Cells);
                    foreach (DataGridViewRow child in row.Children)
                    {
                        items.Add(child);
                        AddGtkStore(row.TreeIter, ref rowindex, child);
                    }
                }
                else
                {
                    Gtk.TreeIter piter = AddGtkStore(row.TreeIter, row.Cells);
                    foreach (DataGridViewRow child in row.Children)
                    {
                        items.Add(child);
                        AddGtkStore(piter, ref rowindex, child);
                    }
                }
            }
        }
        private void AddGtkStore(TreeIter parent, ref int rowindex, DataGridViewRow row)
        {
            row.Index = rowindex;
            row.DataGridView = dataGridView;
            DataGridViewCellStyle _cellStyle = dataGridView.DefaultCellStyle;
            if (dataGridView.RowsDefaultCellStyle != null)
                _cellStyle = dataGridView.RowsDefaultCellStyle;
            if (rowindex % 2 != 0 && dataGridView.AlternatingRowsDefaultCellStyle != null)
                _cellStyle = dataGridView.AlternatingRowsDefaultCellStyle;
            if (row.DefaultCellStyle != null)
                _cellStyle = row.DefaultCellStyle;

            foreach (DataGridViewCell cell in row.Cells)
                cell.RowStyle = _cellStyle;

            items.Add(row);
            row.TreeIter = AddGtkStore(parent, row.Cells);
            rowindex++;
            foreach (DataGridViewRow child in row.Children)
            {
                items.Add(child);
                AddGtkStore(row.TreeIter, ref rowindex, child);
            }
        }
        private TreeIter InsertGtkStore(int rowIndex, DataGridViewCellCollection cells)
        {
            int ncolumns = dataGridView.Store.NColumns;
            int columnscount = dataGridView.Columns.Count;
            for (int i = cells.Count; i < columnscount; i++)
                cells.Add(dataGridView.Columns[i].NewCell());
            for (int i = cells.Count; i < ncolumns; i++)
                cells.Add(new DataGridViewTextBoxCell());
            for (int i = 0; i < ncolumns; i++)
                cells[i].ColumnIndex = i;
            if (ncolumns == cells.Count)
                return dataGridView.Store.InsertWithValues(rowIndex, cells.ConvertAll(o => o).ToArray());
            else
                return dataGridView.Store.InsertWithValues(rowIndex, cells.ConvertAll(o => o).Take(ncolumns).ToArray());
        }
        private void InsertGtkStore(int rowIndex, params DataGridViewRow[] dataGridViewRows)
        {
            int idx = rowIndex;
            foreach (DataGridViewRow row in dataGridViewRows)
            {
                DataGridViewCellStyle _cellStyle = dataGridView.DefaultCellStyle;
                if (dataGridView.RowsDefaultCellStyle != null)
                    _cellStyle = dataGridView.RowsDefaultCellStyle;
                if (idx % 2 != 0 && dataGridView.AlternatingRowsDefaultCellStyle != null)
                    _cellStyle = dataGridView.AlternatingRowsDefaultCellStyle;
                if (row.DefaultCellStyle != null)
                    _cellStyle = row.DefaultCellStyle;

                foreach (DataGridViewCell cell in row.Cells)
                {
                    cell.RowIndex = idx;
                    cell.RowStyle = _cellStyle;
                }

                items.Insert(idx, row);
                row.TreeIter = InsertGtkStore(idx, row.Cells);
                idx++;
            }
            int newindex = idx;
            for (int i = idx; i < items.Count; i++)
            {
                ((DataGridViewRow)items[i]).Index = idx;
            }
        }


        public DataGridViewRow this[int index]
        {
            get
            {
                DataGridViewRow dataGridViewRow = SharedRow(index);
                return dataGridViewRow;
            }
        }
        /// <summary>
        /// Gets the
        /// <see cref="System.Windows.Forms.DataGridView" /> that owns the collection.
        /// </summary>
        /// <returns>The
        /// <see cref="System.Windows.Forms.DataGridView" /> that owns the
        /// <see cref="System.Windows.Forms.DataGridViewRowCollection" />
        /// </returns>
        protected DataGridView DataGridView { get { return dataGridView; } }

        internal ArrayList SharedList => items;
        /// <summary>
        /// Gets an array of
        /// <see cref="System.Windows.Forms.DataGridViewRow" /> objects.
        /// </summary>
        /// <returns>An array of
        /// <see cref="System.Windows.Forms.DataGridViewRow" /> objects.
        /// </returns>
        protected ArrayList List { get { return items; } }
        /// <summary>
        /// Gets the number of rows in the collection.
        /// </summary>
        /// <returns>The number of rows in the
        /// <see cref="System.Windows.Forms.DataGridViewRowCollection" />
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public int Count => items.Count;
        /// <summary>
        /// Gets a value indicating whether the collection has a fixed size.
        /// </summary>
        /// <returns>false in all cases.
        /// </returns>
        bool IList.IsFixedSize => false;
        /// <summary>
        /// Gets a value indicating whether the collection is read-only.
        /// </summary>
        /// <returns>false in all cases.
        /// </returns>
        bool IList.IsReadOnly => false;
        /// <summary>
        /// Gets a value indicating whether access to the collection is synchronized (thread safe).
        /// </summary>
        /// <returns>false in all cases.
        /// </returns>
        bool ICollection.IsSynchronized => false;
        /// <summary>
        /// Gets an object that can be used to synchronize access to the collection.
        /// </summary>
        /// <returns>An object that can be used to synchronize access to the
        /// <see cref="System.Windows.Forms.DataGridViewRowCollection" />
        /// </returns>
        object ICollection.SyncRoot => this;

        object IList.this[int index] { get => items[index]; set => throw new NotSupportedException(); }


        public event CollectionChangeEventHandler CollectionChanged;
        /// <summary>
        /// Adds a new row to the collection.
        /// </summary>
        /// <returns>The index of the new row.
        /// </returns>
        /// <exception cref="System.InvalidOperationException">The associated
        /// <see cref="System.Windows.Forms.DataGridView" /> control is performing one of the following actions that temporarily prevents new rows from being added:Selecting all cells in the control.Clearing the selection.-or-This method is being called from a handler for one of the following
        /// <see cref="System.Windows.Forms.DataGridView" /> events:
        /// <see cref="System.Windows.Forms.DataGridView.CellEnter" />
        /// <see cref="System.Windows.Forms.DataGridView.CellLeave" />
        /// <see cref="System.Windows.Forms.DataGridView.CellValidating" />
        /// <see cref="System.Windows.Forms.DataGridView.CellValidated" />
        /// <see cref="System.Windows.Forms.DataGridView.RowEnter" />
        /// <see cref="System.Windows.Forms.DataGridView.RowLeave" />
        /// <see cref="System.Windows.Forms.DataGridView.RowValidated" />
        /// <see cref="System.Windows.Forms.DataGridView.RowValidating" />-or-The
        /// <see cref="System.Windows.Forms.DataGridView.DataSource" /> property of the
        /// <see cref="System.Windows.Forms.DataGridView" /> is not null.-or-The
        /// <see cref="System.Windows.Forms.DataGridView" /> has no columns.-or-This operation would add a frozen row after unfrozen rows.
        /// </exception>
        /// <exception cref="System.ArgumentException">The row returned by the
        /// <see cref="System.Windows.Forms.DataGridView.RowTemplate" /> property has more cells than there are columns in the control.
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public virtual int Add()
        {
            DataGridViewRow row = new DataGridViewRow();
            AddGtkStore(row);
            return row.Index;
        }
        /// <summary>
        /// Adds the specified
        /// <see cref="System.Windows.Forms.DataGridViewRow" /> to the collection.
        /// </summary>
        /// <returns>The index of the new
        /// <see cref="System.Windows.Forms.DataGridViewRow" />
        /// </returns>
        /// <param name="dataGridViewRow">The
        /// <see cref="System.Windows.Forms.DataGridViewRow" /> to add to the
        /// <see cref="System.Windows.Forms.DataGridViewRowCollection" />
        /// </param>
        /// <exception cref="System.InvalidOperationException">The associated
        /// <see cref="System.Windows.Forms.DataGridView" /> control is performing one of the following actions that temporarily prevents new rows from being added:Selecting all cells in the control.Clearing the selection.-or-This method is being called from a handler for one of the following
        /// <see cref="System.Windows.Forms.DataGridView" /> events:
        /// <see cref="System.Windows.Forms.DataGridView.CellEnter" />
        /// <see cref="System.Windows.Forms.DataGridView.CellLeave" />
        /// <see cref="System.Windows.Forms.DataGridView.CellValidating" />
        /// <see cref="System.Windows.Forms.DataGridView.CellValidated" />
        /// <see cref="System.Windows.Forms.DataGridView.RowEnter" />
        /// <see cref="System.Windows.Forms.DataGridView.RowLeave" />
        /// <see cref="System.Windows.Forms.DataGridView.RowValidated" />
        /// <see cref="System.Windows.Forms.DataGridView.RowValidating" />-or-The
        /// <see cref="System.Windows.Forms.DataGridView.DataSource" /> property of the
        /// <see cref="System.Windows.Forms.DataGridView" /> is not null.-or-The
        /// <see cref="System.Windows.Forms.DataGridView" /> has no columns.-or-The
        /// <see cref="System.Windows.Forms.DataGridViewElement.DataGridView" /> property of the
        /// <paramref name="dataGridViewRow" /> is not null.-or-
        /// <paramref name="dataGridViewRow" /> has a
        /// <see cref="System.Windows.Forms.DataGridViewRow.Selected" /> property value of true. -or-This operation would add a frozen row after unfrozen rows.
        /// </exception>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="dataGridViewRow" /> is null.
        /// </exception>
        /// <exception cref="System.ArgumentException">
        /// <paramref name="dataGridViewRow" />
        /// has more cells than there are columns in the control.
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public virtual int Add(DataGridViewRow dataGridViewRow)
        {
            int rowindex = items.Count;
            if (parentRow == null)
                AddGtkStore(dataGridViewRow);
            else
                AddGtkStore(parentRow.TreeIter, ref rowindex, dataGridViewRow);

            return dataGridViewRow.Index;
        }
        /// <summary>
        /// Adds a new row to the collection, and populates the cells with the specified objects.
        /// </summary>
        /// <returns>The index of the new row.
        /// </returns>
        /// <param name="values">A variable number of objects that populate the cells of the new
        /// <see cref="System.Windows.Forms.DataGridViewRow" />
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="values" /> is null.
        /// </exception>
        /// <exception cref="System.InvalidOperationException">The associated
        /// <see cref="System.Windows.Forms.DataGridView" /> control is performing one of the following actions that temporarily prevents new rows from being added:Selecting all cells in the control.Clearing the selection.-or-This method is being called from a handler for one of the following
        /// <see cref="System.Windows.Forms.DataGridView" /> events:
        /// <see cref="System.Windows.Forms.DataGridView.CellEnter" />
        /// <see cref="System.Windows.Forms.DataGridView.CellLeave" />
        /// <see cref="System.Windows.Forms.DataGridView.CellValidating" />
        /// <see cref="System.Windows.Forms.DataGridView.CellValidated" />
        /// <see cref="System.Windows.Forms.DataGridView.RowEnter" />
        /// <see cref="System.Windows.Forms.DataGridView.RowLeave" />
        /// <see cref="System.Windows.Forms.DataGridView.RowValidated" />
        /// <see cref="System.Windows.Forms.DataGridView.RowValidating" />-or-The
        /// <see cref="System.Windows.Forms.DataGridView.VirtualMode" /> property of the
        /// <see cref="System.Windows.Forms.DataGridView" /> is set to true.- or -The
        /// <see cref="System.Windows.Forms.DataGridView.DataSource" /> property of the
        /// <see cref="System.Windows.Forms.DataGridView" /> is not null.-or-The
        /// <see cref="System.Windows.Forms.DataGridView" /> has no columns. -or-The row returned by the
        /// <see cref="System.Windows.Forms.DataGridView.RowTemplate" /> property has more cells than there are columns in the control.-or-This operation would add a frozen row after unfrozen rows.
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public virtual int Add(params object[] values)
        {
            DataGridViewRow newRow = new DataGridViewRow();
            var cols = dataGridView.Columns;
            int valueslength = values.Length;
            for (int i = 0; i < cols.Count; i++)
            {
                if (i >= valueslength)
                    newRow.Cells.Add(dataGridView.Columns[i].NewCell());
                else if (values[i] is DataGridViewCell cellval)
                    newRow.Cells.Add(cellval);
                else
                    newRow.Cells.Add(dataGridView.Columns[i].NewCell(values[i]));
            }
            return Add(newRow);
        }
        /// <summary>
        /// Adds the specified number of new rows to the collection.
        /// </summary>
        /// <returns>The index of the last row that was added.
        /// </returns>
        /// <param name="count">The number of rows to add to the
        /// <see cref="System.Windows.Forms.DataGridViewRowCollection" />
        /// </param>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// <paramref name="count" /> is less than 1.
        /// </exception>
        /// <exception cref="System.InvalidOperationException">The associated
        /// <see cref="System.Windows.Forms.DataGridView" /> control is performing one of the following actions that temporarily prevents new rows from being added:Selecting all cells in the control.Clearing the selection.-or-This method is being called from a handler for one of the following
        /// <see cref="System.Windows.Forms.DataGridView" /> events:
        /// <see cref="System.Windows.Forms.DataGridView.CellEnter" />
        /// <see cref="System.Windows.Forms.DataGridView.CellLeave" />
        /// <see cref="System.Windows.Forms.DataGridView.CellValidating" />
        /// <see cref="System.Windows.Forms.DataGridView.CellValidated" />
        /// <see cref="System.Windows.Forms.DataGridView.RowEnter" />
        /// <see cref="System.Windows.Forms.DataGridView.RowLeave" />
        /// <see cref="System.Windows.Forms.DataGridView.RowValidated" />
        /// <see cref="System.Windows.Forms.DataGridView.RowValidating" />-or-The
        /// <see cref="System.Windows.Forms.DataGridView.DataSource" /> property of the
        /// <see cref="System.Windows.Forms.DataGridView" /> is not null.-or-The
        /// <see cref="System.Windows.Forms.DataGridView" /> has no columns.-or-The row returned by the
        /// <see cref="System.Windows.Forms.DataGridView.RowTemplate" /> property has more cells than there are columns in the control. -or-This operation would add frozen rows after unfrozen rows.
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public virtual int Add(int count)
        {
            int start = items.Count;
            for (int i = 0; i < count; i++)
            {
                DataGridViewRow row = new DataGridViewRow() { Index = start + i };
                AddGtkStore(row);
                items.Add(row);
            }

            return start + count;
        }
        /// <summary>
        /// Adds the specified number of rows to the collection based on the row at the specified index.
        /// </summary>
        /// <returns>The index of the last row that was added.
        /// </returns>
        /// <param name="indexSource">The index of the row on which to base the new rows.
        /// </param>
        /// <param name="count">The number of rows to add to the
        /// <see cref="System.Windows.Forms.DataGridViewRowCollection" />
        /// </param>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// <paramref name="indexSource" /> is less than zero or greater than or equal to the number of rows in the control.-or-
        /// <paramref name="count" /> is less than zero.
        /// </exception>
        /// <exception cref="System.InvalidOperationException">The associated
        /// <see cref="System.Windows.Forms.DataGridView" /> control is performing one of the following actions that temporarily prevents new rows from being added:Selecting all cells in the control.Clearing the selection.-or-This method is being called from a handler for one of the following
        /// <see cref="System.Windows.Forms.DataGridView" /> events:
        /// <see cref="System.Windows.Forms.DataGridView.CellEnter" />
        /// <see cref="System.Windows.Forms.DataGridView.CellLeave" />
        /// <see cref="System.Windows.Forms.DataGridView.CellValidating" />
        /// <see cref="System.Windows.Forms.DataGridView.CellValidated" />
        /// <see cref="System.Windows.Forms.DataGridView.RowEnter" />
        /// <see cref="System.Windows.Forms.DataGridView.RowLeave" />
        /// <see cref="System.Windows.Forms.DataGridView.RowValidated" />
        /// <see cref="System.Windows.Forms.DataGridView.RowValidating" />-or-The
        /// <see cref="System.Windows.Forms.DataGridView.DataSource" /> property of the
        /// <see cref="System.Windows.Forms.DataGridView" /> is not null.-or-This operation would add a frozen row after unfrozen rows.
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public virtual int AddCopies(int indexSource, int count)
        {
            DataGridViewRow[] arr = new DataGridViewRow[count];
            items.CopyTo(0, arr, indexSource, count);
            this.AddRange(arr);
            return count;
        }
        /// <summary>
        /// Adds a new row based on the row at the specified index.
        /// </summary>
        /// <returns>The index of the new row.
        /// </returns>
        /// <param name="indexSource">The index of the row on which to base the new row.
        /// </param>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// <paramref name="indexSource" /> is less than zero or greater than or equal to the number of rows in the collection.
        /// </exception>
        /// <exception cref="System.InvalidOperationException">The associated
        /// <see cref="System.Windows.Forms.DataGridView" /> control is performing one of the following actions that temporarily prevents new rows from being added:Selecting all cells in the control.Clearing the selection.-or-This method is being called from a handler for one of the following
        /// <see cref="System.Windows.Forms.DataGridView" /> events:
        /// <see cref="System.Windows.Forms.DataGridView.CellEnter" />
        /// <see cref="System.Windows.Forms.DataGridView.CellLeave" />
        /// <see cref="System.Windows.Forms.DataGridView.CellValidating" />
        /// <see cref="System.Windows.Forms.DataGridView.CellValidated" />
        /// <see cref="System.Windows.Forms.DataGridView.RowEnter" />
        /// <see cref="System.Windows.Forms.DataGridView.RowLeave" />
        /// <see cref="System.Windows.Forms.DataGridView.RowValidated" />
        /// <see cref="System.Windows.Forms.DataGridView.RowValidating" />-or-The
        /// <see cref="System.Windows.Forms.DataGridView.DataSource" /> property of the
        /// <see cref="System.Windows.Forms.DataGridView" /> is not null.-or-This operation would add a frozen row after unfrozen rows.
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public virtual int AddCopy(int indexSource)
        {
            return AddCopies(indexSource, 1);
        }
        /// <summary>
        /// Adds the specified
        /// <see cref="System.Windows.Forms.DataGridViewRow" /> objects to the collection.
        /// </summary>
        /// <param name="dataGridViewRows">An array of
        /// <see cref="System.Windows.Forms.DataGridViewRow" /> objects to be added to the
        /// <see cref="System.Windows.Forms.DataGridViewRowCollection" />
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="dataGridViewRows" /> is null.
        /// </exception>
        /// <exception cref="System.ArgumentException">
        /// <paramref name="dataGridViewRows" />
        /// contains only one row, and the row it contains has more cells than there are columns in the control.
        /// </exception>
        /// <exception cref="System.InvalidOperationException">The associated
        /// <see cref="System.Windows.Forms.DataGridView" /> control is performing one of the following actions that temporarily prevents new rows from being added:Selecting all cells in the control.Clearing the selection.-or-This method is being called from a handler for one of the following
        /// <see cref="System.Windows.Forms.DataGridView" /> events:
        /// <see cref="System.Windows.Forms.DataGridView.CellEnter" />
        /// <see cref="System.Windows.Forms.DataGridView.CellLeave" />
        /// <see cref="System.Windows.Forms.DataGridView.CellValidating" />
        /// <see cref="System.Windows.Forms.DataGridView.CellValidated" />
        /// <see cref="System.Windows.Forms.DataGridView.RowEnter" />
        /// <see cref="System.Windows.Forms.DataGridView.RowLeave" />
        /// <see cref="System.Windows.Forms.DataGridView.RowValidated" />
        /// <see cref="System.Windows.Forms.DataGridView.RowValidating" />-or-The
        /// <see cref="System.Windows.Forms.DataGridView.DataSource" /> property of the
        /// <see cref="System.Windows.Forms.DataGridView" /> is not null.-or-At least one entry in the
        /// <paramref name="dataGridViewRows" /> array is null.-or-The
        /// <see cref="System.Windows.Forms.DataGridView" /> has no columns.-or-At least one row in the
        /// <paramref name="dataGridViewRows" /> array has a
        /// <see cref="System.Windows.Forms.DataGridViewElement.DataGridView" /> property value that is not null.-or-At least one row in the
        /// <paramref name="dataGridViewRows" /> array has a
        /// <see cref="System.Windows.Forms.DataGridViewRow.Selected" /> property value of true.-or-Two or more rows in the
        /// <paramref name="dataGridViewRows" /> array are identical.-or-At least one row in the
        /// <paramref name="dataGridViewRows" /> array contains one or more cells of a type that is incompatible with the type of the corresponding column in the control.-or-At least one row in the
        /// <paramref name="dataGridViewRows" /> array contains more cells than there are columns in the control.-or-This operation would add frozen rows after unfrozen rows.
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public virtual void AddRange(params DataGridViewRow[] dataGridViewRows)
        {
            if (this.dataGridView.Store.NColumns < this.dataGridView.GridView.Columns.Length)
                this.dataGridView.Columns.Invalidate();
            if (parentRow == null)
                AddGtkStore(dataGridViewRows);
            else
            {
                int rowindex = items.Count;
                foreach (DataGridViewRow row in dataGridViewRows)
                {
                    AddGtkStore(parentRow.TreeIter, ref rowindex, row);
                }
            }
        }
        /// <summary>
        /// Clears the collection.
        /// </summary>
        /// <exception cref="System.InvalidOperationException">The collection is data bound and the underlying data source does not support clearing the row data.-or-The associated
        /// <see cref="System.Windows.Forms.DataGridView" /> control is performing one of the following actions that temporarily prevents the row collection from being modified:Selecting all cells in the control.Clearing the selection.-or-This method is being called from a handler for one of the following
        /// <see cref="System.Windows.Forms.DataGridView" /> events:
        /// <see cref="System.Windows.Forms.DataGridView.CellEnter" />
        /// <see cref="System.Windows.Forms.DataGridView.CellLeave" />
        /// <see cref="System.Windows.Forms.DataGridView.CellValidating" />
        /// <see cref="System.Windows.Forms.DataGridView.CellValidated" />
        /// <see cref="System.Windows.Forms.DataGridView.RowEnter" />
        /// <see cref="System.Windows.Forms.DataGridView.RowLeave" />
        /// <see cref="System.Windows.Forms.DataGridView.RowValidated" />
        /// <see cref="System.Windows.Forms.DataGridView.RowValidating" />
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public virtual void Clear()
        {
            dataGridView.Store.Clear();
            items.Clear();
        }
        /// <summary>
        /// Determines whether the specified
        /// <see cref="System.Windows.Forms.DataGridViewRow" /> is in the collection.
        /// </summary>
        /// <returns>true if the
        /// <see cref="System.Windows.Forms.DataGridViewRow" /> is in the
        /// <see cref="System.Windows.Forms.DataGridViewRowCollection" />; otherwise, false.
        /// </returns>
        /// <param name="dataGridViewRow">The
        /// <see cref="System.Windows.Forms.DataGridViewRow" /> to locate in the
        /// <see cref="System.Windows.Forms.DataGridViewRowCollection" />
        /// </param>
        /// <filterpriority>1
        /// </filterpriority>
        public virtual bool Contains(DataGridViewRow dataGridViewRow)
        {
            return items.IndexOf(dataGridViewRow) != -1;
        }
        /// <summary>
        /// Copies the items from the collection into the specified
        /// <see cref="System.Windows.Forms.DataGridViewRow" /> array, starting at the specified index.
        /// </summary>
        /// <param name="array">A
        /// <see cref="System.Windows.Forms.DataGridViewRow" /> array that is the destination of the items copied from the
        /// <see cref="System.Windows.Forms.DataGridViewRowCollection" />
        /// </param>
        /// <param name="index">The zero-based index in
        /// <paramref name="array" /> at which copying begins.
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="array" /> is null.
        /// </exception>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// <paramref name="index" />
        /// is less than zero.
        /// </exception>
        /// <exception cref="System.ArgumentException">
        /// <paramref name="array" /> is multidimensional.-or- The number of elements in the
        /// <see cref="System.Windows.Forms.DataGridViewRowCollection" /> is greater than the available space from
        /// <paramref name="index" /> to the end of
        /// <paramref name="array" />
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        public void CopyTo(DataGridViewRow[] array, int index)
        {
            items.CopyTo(array, index);
        }
        /// <summary>
        /// Returns the index of the first
        /// <see cref="System.Windows.Forms.DataGridViewRow" /> that meets the specified criteria.
        /// </summary>
        /// <returns>The index of the first
        /// <see cref="System.Windows.Forms.DataGridViewRow" /> that has the attributes specified by
        /// <paramref name="includeFilter" />; -1 if no row is found.
        /// </returns>
        /// <param name="includeFilter">A bitwise combination of
        /// <see cref="System.Windows.Forms.DataGridViewElementStates" /> values.
        /// </param>
        /// <exception cref="System.ArgumentException">
        /// <paramref name="includeFilter" /> is not a valid bitwise combination of
        /// <see cref="System.Windows.Forms.DataGridViewElementStates" /> values.
        /// </exception>
        public int GetFirstRow(DataGridViewElementStates includeFilter)
        {
            int idx = -1;
            for (int i = 0; i < items.Count; i++)
            {
                if (((DataGridViewRow)items[i]).State == includeFilter)
                {
                    idx = i;
                    break;
                }
            }
            return idx;
        }
        /// <summary>
        /// Returns the index of the first
        /// <see cref="System.Windows.Forms.DataGridViewRow" /> that meets the specified inclusion and exclusion criteria.
        /// </summary>
        /// <returns>The index of the first
        /// <see cref="System.Windows.Forms.DataGridViewRow" /> that has the attributes specified by
        /// <paramref name="includeFilter" />, and does not have the attributes specified by
        /// <paramref name="excludeFilter" />; -1 if no row is found.
        /// </returns>
        /// <param name="includeFilter">A bitwise combination of
        /// <see cref="System.Windows.Forms.DataGridViewElementStates" /> values.
        /// </param>
        /// <param name="excludeFilter">A bitwise combination of
        /// <see cref="System.Windows.Forms.DataGridViewElementStates" /> values.
        /// </param>
        /// <exception cref="System.ArgumentException">One or both of the specified filter values is not a valid bitwise combination of
        /// <see cref="System.Windows.Forms.DataGridViewElementStates" /> values.
        /// </exception>
        public int GetFirstRow(DataGridViewElementStates includeFilter, DataGridViewElementStates excludeFilter)
        {
            int idx = -1;
            for (int i = 0; i < items.Count; i++)
            {
                if (((DataGridViewRow)items[i]).State == includeFilter || ((DataGridViewRow)items[i]).State == excludeFilter)
                {
                    idx = i;
                    break;
                }
            }
            return idx;
        }
        /// <summary>
        /// Returns the index of the last
        /// <see cref="System.Windows.Forms.DataGridViewRow" /> that meets the specified criteria.
        /// </summary>
        /// <returns>The index of the last
        /// <see cref="System.Windows.Forms.DataGridViewRow" /> that has the attributes specified by
        /// <paramref name="includeFilter" />; -1 if no row is found.
        /// </returns>
        /// <param name="includeFilter">A bitwise combination of
        /// <see cref="System.Windows.Forms.DataGridViewElementStates" /> values.
        /// </param>
        /// <exception cref="System.ArgumentException">
        /// <paramref name="includeFilter" /> is not a valid bitwise combination of
        /// <see cref="System.Windows.Forms.DataGridViewElementStates" /> values.
        /// </exception>
        public int GetLastRow(DataGridViewElementStates includeFilter)
        {
            int idx = -1;
            for (int i = 0; i < items.Count; i++)
            {
                if (((DataGridViewRow)items[i]).State == includeFilter)
                {
                    idx = i;
                }
            }
            return idx;
        }
        /// <summary>
        /// Returns the index of the next
        /// <see cref="System.Windows.Forms.DataGridViewRow" /> that meets the specified criteria.
        /// </summary>
        /// <returns>The index of the first
        /// <see cref="System.Windows.Forms.DataGridViewRow" /> after
        /// <paramref name="indexStart" /> that has the attributes specified by
        /// <paramref name="includeFilter" />, or -1 if no row is found.
        /// </returns>
        /// <param name="indexStart">The index of the row where the method should begin to look for the next
        /// <see cref="System.Windows.Forms.DataGridViewRow" />
        /// </param>
        /// <param name="includeFilter">A bitwise combination of
        /// <see cref="System.Windows.Forms.DataGridViewElementStates" /> values.
        /// </param>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// <paramref name="indexStart" /> is less than -1.
        /// </exception>
        /// <exception cref="System.ArgumentException">
        /// <paramref name="includeFilter" />
        /// is not a valid bitwise combination of
        /// <see cref="System.Windows.Forms.DataGridViewElementStates" /> values.
        /// </exception>
        public int GetNextRow(int indexStart, DataGridViewElementStates includeFilter)
        {
            int idx = -1;
            for (int i = indexStart - 1; i < items.Count; i++)
            {
                if (((DataGridViewRow)items[i]).State == includeFilter)
                {
                    idx = i;
                    break;
                }
            }
            return idx;
        }
        /// <summary>
        /// Returns the index of the next
        /// <see cref="System.Windows.Forms.DataGridViewRow" /> that meets the specified inclusion and exclusion criteria.
        /// </summary>
        /// <returns>The index of the next
        /// <see cref="System.Windows.Forms.DataGridViewRow" /> that has the attributes specified by
        /// <paramref name="includeFilter" />, and does not have the attributes specified by
        /// <paramref name="excludeFilter" />; -1 if no row is found.
        /// </returns>
        /// <param name="indexStart">The index of the row where the method should begin to look for the next
        /// <see cref="System.Windows.Forms.DataGridViewRow" />
        /// </param>
        /// <param name="includeFilter">A bitwise combination of
        /// <see cref="System.Windows.Forms.DataGridViewElementStates" /> values.
        /// </param>
        /// <param name="excludeFilter">A bitwise combination of
        /// <see cref="System.Windows.Forms.DataGridViewElementStates" /> values.
        /// </param>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// <paramref name="indexStart" /> is less than -1.
        /// </exception>
        /// <exception cref="System.ArgumentException">One or both of the specified filter values is not a valid bitwise combination of
        /// <see cref="System.Windows.Forms.DataGridViewElementStates" /> values.
        /// </exception>
        public int GetNextRow(int indexStart, DataGridViewElementStates includeFilter, DataGridViewElementStates excludeFilter)
        {
            int idx = -1;
            for (int i = indexStart - 1; i < items.Count; i++)
            {
                if (((DataGridViewRow)items[i]).State == includeFilter || ((DataGridViewRow)items[i]).State == excludeFilter)
                {
                    idx = i;
                    break;
                }
            }
            return idx;
        }
        /// <summary>
        /// Returns the index of the previous
        /// <see cref="System.Windows.Forms.DataGridViewRow" /> that meets the specified inclusion and exclusion criteria.
        /// </summary>
        /// <returns>The index of the previous
        /// <see cref="System.Windows.Forms.DataGridViewRow" /> that has the attributes specified by
        /// <paramref name="includeFilter" />, and does not have the attributes specified by
        /// <paramref name="excludeFilter" />; -1 if no row is found.
        /// </returns>
        /// <param name="indexStart">The index of the row where the method should begin to look for the previous
        /// <see cref="System.Windows.Forms.DataGridViewRow" />
        /// </param>
        /// <param name="includeFilter">A bitwise combination of
        /// <see cref="System.Windows.Forms.DataGridViewElementStates" /> values.
        /// </param>
        /// <param name="excludeFilter">A bitwise combination of
        /// <see cref="System.Windows.Forms.DataGridViewElementStates" /> values.
        /// </param>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// <paramref name="indexStart" /> is greater than the number of rows in the collection.
        /// </exception>
        /// <exception cref="System.ArgumentException">One or both of the specified filter values is not a valid bitwise combination of
        /// <see cref="System.Windows.Forms.DataGridViewElementStates" /> values.
        /// </exception>
        public int GetPreviousRow(int indexStart, DataGridViewElementStates includeFilter, DataGridViewElementStates excludeFilter)
        {
            int idx = -1;
            for (int i = indexStart - 1; i > -1; i--)
            {
                if (((DataGridViewRow)items[i]).State == includeFilter || ((DataGridViewRow)items[i]).State == excludeFilter)
                {
                    idx = i;
                    break;
                }
            }
            return idx;
        }
        /// <summary>
        /// Returns the index of the previous
        /// <see cref="System.Windows.Forms.DataGridViewRow" /> that meets the specified criteria.
        /// </summary>
        /// <returns>The index of the previous
        /// <see cref="System.Windows.Forms.DataGridViewRow" /> that has the attributes specified by
        /// <paramref name="includeFilter" />; -1 if no row is found.
        /// </returns>
        /// <param name="indexStart">The index of the row where the method should begin to look for the previous
        /// <see cref="System.Windows.Forms.DataGridViewRow" />
        /// </param>
        /// <param name="includeFilter">A bitwise combination of
        /// <see cref="System.Windows.Forms.DataGridViewElementStates" /> values.
        /// </param>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// <paramref name="indexStart" /> is greater than the number of rows in the collection.
        /// </exception>
        /// <exception cref="System.ArgumentException">
        /// <paramref name="includeFilter" />
        /// is not a valid bitwise combination of
        /// <see cref="System.Windows.Forms.DataGridViewElementStates" /> values.
        /// </exception>
        public int GetPreviousRow(int indexStart, DataGridViewElementStates includeFilter)
        {
            int idx = -1;
            for (int i = indexStart - 1; i > -1; i--)
            {
                if (((DataGridViewRow)items[i]).State == includeFilter)
                {
                    idx = i;
                    break;
                }
            }
            return idx;
        }
        /// <summary>
        /// Returns the number of
        /// <see cref="System.Windows.Forms.DataGridViewRow" /> objects in the collection that meet the specified criteria.
        /// </summary>
        /// <returns>The number of
        /// <see cref="System.Windows.Forms.DataGridViewRow" /> objects in the
        /// <see cref="System.Windows.Forms.DataGridViewRowCollection" /> that have the attributes specified by
        /// <paramref name="includeFilter" />
        /// </returns>
        /// <param name="includeFilter">A bitwise combination of
        /// <see cref="System.Windows.Forms.DataGridViewElementStates" /> values.
        /// </param>
        /// <exception cref="System.ArgumentException">
        /// <paramref name="includeFilter" /> is not a valid bitwise combination of
        /// <see cref="System.Windows.Forms.DataGridViewElementStates" /> values.
        /// </exception>
        public int GetRowCount(DataGridViewElementStates includeFilter)
        {

            if (includeFilter == DataGridViewElementStates.None)
                return items.Count;
            else
            {
                int num = 0;
                for (int i = 0; i < items.Count; i++)
                {
                    if ((GetRowState(i) & includeFilter) == includeFilter)
                    {
                        num++;
                    }
                }
                return num;
            }
        }
        /// <summary>
        /// Returns the cumulative height of the
        /// <see cref="System.Windows.Forms.DataGridViewRow" /> objects that meet the specified criteria.
        /// </summary>
        /// <returns>The cumulative height of
        /// <see cref="System.Windows.Forms.DataGridViewRow" /> objects in the
        /// <see cref="System.Windows.Forms.DataGridViewRowCollection" /> that have the attributes specified by
        /// <paramref name="includeFilter" />
        /// </returns>
        /// <param name="includeFilter">A bitwise combination of
        /// <see cref="System.Windows.Forms.DataGridViewElementStates" /> values.
        /// </param>
        /// <exception cref="System.ArgumentException">
        /// <paramref name="includeFilter" /> is not a valid bitwise combination of
        /// <see cref="System.Windows.Forms.DataGridViewElementStates" /> values.
        /// </exception>
        public int GetRowsHeight(DataGridViewElementStates includeFilter)
        {
            int num = 0;
            for (int i = 0; i < items.Count; i++)
            {
                if ((GetRowState(i) & includeFilter) == includeFilter)
                {
                    num += ((DataGridViewRow)items[i]).GetHeight(i);
                }
            }
            return num;
        }
        /// <summary>
        /// Gets the state of the row with the specified index.
        /// </summary>
        /// <returns>A bitwise combination of
        /// <see cref="System.Windows.Forms.DataGridViewElementStates" /> values indicating the state of the specified row.
        /// </returns>
        /// <param name="rowIndex">The index of the row.
        /// </param>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// <paramref name="rowIndex" /> is less than zero and greater than the number of rows in the collection minus one.
        /// </exception>
        public virtual DataGridViewElementStates GetRowState(int rowIndex)
        {
            return ((DataGridViewRow)items[rowIndex]).State;
        }
        /// <summary>
        /// Returns the index of a specified item in the collection.
        /// </summary>
        /// <returns>The index of
        /// <paramref name="value" /> if it is a
        /// <see cref="System.Windows.Forms.DataGridViewRow" /> found in the
        /// <see cref="System.Windows.Forms.DataGridViewRowCollection" />; otherwise, -1.
        /// </returns>
        /// <param name="dataGridViewRow">The
        /// <see cref="System.Windows.Forms.DataGridViewRow" /> to locate in the
        /// <see cref="System.Windows.Forms.DataGridViewRowCollection" />
        /// </param>
        /// <filterpriority>1
        /// </filterpriority>
        public int IndexOf(DataGridViewRow dataGridViewRow)
        {
            return items.IndexOf(dataGridViewRow);
        }
        /// <summary>
        /// Inserts a row into the collection at the specified position, and populates the cells with the specified objects.
        /// </summary>
        /// <param name="rowIndex">The position at which to insert the row.
        /// </param>
        /// <param name="values">A variable number of objects that populate the cells of the new row.
        /// </param>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// <paramref name="rowIndex" /> is less than zero or greater than the number of rows in the collection.
        /// </exception>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="values" />
        /// is null.
        /// </exception>
        /// <exception cref="System.InvalidOperationException">The associated
        /// <see cref="System.Windows.Forms.DataGridView" /> control is performing one of the following actions that temporarily prevents new rows from being added:Selecting all cells in the control.Clearing the selection.-or-This method is being called from a handler for one of the following
        /// <see cref="System.Windows.Forms.DataGridView" /> events:
        /// <see cref="System.Windows.Forms.DataGridView.CellEnter" />
        /// <see cref="System.Windows.Forms.DataGridView.CellLeave" />
        /// <see cref="System.Windows.Forms.DataGridView.CellValidating" />
        /// <see cref="System.Windows.Forms.DataGridView.CellValidated" />
        /// <see cref="System.Windows.Forms.DataGridView.RowEnter" />
        /// <see cref="System.Windows.Forms.DataGridView.RowLeave" />
        /// <see cref="System.Windows.Forms.DataGridView.RowValidated" />
        /// <see cref="System.Windows.Forms.DataGridView.RowValidating" />-or-The
        /// <see cref="System.Windows.Forms.DataGridView.VirtualMode" /> property of the
        /// <see cref="System.Windows.Forms.DataGridView" /> is set to true.-or-The
        /// <see cref="System.Windows.Forms.DataGridView.DataSource" /> property of the
        /// <see cref="System.Windows.Forms.DataGridView" /> is not null.-or-The
        /// <see cref="System.Windows.Forms.DataGridView" /> has no columns.-or-
        /// <paramref name="rowIndex" /> is equal to the number of rows in the collection and the
        /// <see cref="System.Windows.Forms.DataGridView.AllowUserToAddRows" /> property of the
        /// <see cref="System.Windows.Forms.DataGridView" /> is set to true.-or-The
        /// <see cref="System.Windows.Forms.DataGridViewElement.DataGridView" /> property of the row returned by the control's
        /// <see cref="System.Windows.Forms.DataGridView.RowTemplate" /> property is not null. -or-This operation would insert a frozen row after unfrozen rows or an unfrozen row before frozen rows.
        /// </exception>
        /// <exception cref="System.ArgumentException">The row returned by the control's
        /// <see cref="System.Windows.Forms.DataGridView.RowTemplate" /> property has more cells than there are columns in the control.
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public virtual void Insert(int rowIndex, params object[] values)
        {
            DataGridViewRow newRow = new DataGridViewRow();
            var cols = dataGridView.Columns;
            int valueslength = values.Length;
            for (int i = 0; i < cols.Count; i++)
            {
                if (i >= valueslength)
                    newRow.Cells.Add(dataGridView.Columns[i].NewCell());
                else if (values[i] is DataGridViewCell cellval)
                    newRow.Cells.Add(cellval);
                else
                    newRow.Cells.Add(dataGridView.Columns[i].NewCell(values[i]));
            }

            InsertGtkStore(rowIndex, newRow);
        }
        /// <summary>
        /// Inserts the specified
        /// <see cref="System.Windows.Forms.DataGridViewRow" /> into the collection.
        /// </summary>
        /// <param name="rowIndex">The position at which to insert the row.
        /// </param>
        /// <param name="dataGridViewRow">The
        /// <see cref="System.Windows.Forms.DataGridViewRow" /> to insert into the
        /// <see cref="System.Windows.Forms.DataGridViewRowCollection" />
        /// </param>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// <paramref name="rowIndex" /> is less than zero or greater than the number of rows in the collection.
        /// </exception>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="dataGridViewRow" />
        /// is null.
        /// </exception>
        /// <exception cref="System.InvalidOperationException">The associated
        /// <see cref="System.Windows.Forms.DataGridView" /> control is performing one of the following actions that temporarily prevents new rows from being added:Selecting all cells in the control.Clearing the selection.-or-This method is being called from a handler for one of the following
        /// <see cref="System.Windows.Forms.DataGridView" /> events:
        /// <see cref="System.Windows.Forms.DataGridView.CellEnter" />
        /// <see cref="System.Windows.Forms.DataGridView.CellLeave" />
        /// <see cref="System.Windows.Forms.DataGridView.CellValidating" />
        /// <see cref="System.Windows.Forms.DataGridView.CellValidated" />
        /// <see cref="System.Windows.Forms.DataGridView.RowEnter" />
        /// <see cref="System.Windows.Forms.DataGridView.RowLeave" />
        /// <see cref="System.Windows.Forms.DataGridView.RowValidated" />
        /// <see cref="System.Windows.Forms.DataGridView.RowValidating" />-or-The
        /// <see cref="System.Windows.Forms.DataGridView.DataSource" /> property of the
        /// <see cref="System.Windows.Forms.DataGridView" /> is not null.-or-
        /// <paramref name="rowIndex" /> is equal to the number of rows in the collection and the
        /// <see cref="System.Windows.Forms.DataGridView.AllowUserToAddRows" /> property of the
        /// <see cref="System.Windows.Forms.DataGridView" /> is set to true.-or-The
        /// <see cref="System.Windows.Forms.DataGridView" /> has no columns.-or-The
        /// <see cref="System.Windows.Forms.DataGridViewElement.DataGridView" /> property of
        /// <paramref name="dataGridViewRow" /> is not null.-or-
        /// <paramref name="dataGridViewRow" /> has a
        /// <see cref="System.Windows.Forms.DataGridViewRow.Selected" /> property value of true. -or-This operation would insert a frozen row after unfrozen rows or an unfrozen row before frozen rows.
        /// </exception>
        /// <exception cref="System.ArgumentException">
        /// <paramref name="dataGridViewRow" /> has more cells than there are columns in the control.
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public virtual void Insert(int rowIndex, DataGridViewRow dataGridViewRow)
        {
            InsertGtkStore(rowIndex, dataGridViewRow);
        }
        /// <summary>
        /// Inserts the specified number of rows into the collection at the specified location.
        /// </summary>
        /// <param name="rowIndex">The position at which to insert the rows.
        /// </param>
        /// <param name="count">The number of rows to insert into the
        /// <see cref="System.Windows.Forms.DataGridViewRowCollection" />
        /// </param>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// <paramref name="rowIndex" /> is less than zero or greater than the number of rows in the collection. -or-
        /// <paramref name="count" /> is less than 1.
        /// </exception>
        /// <exception cref="System.InvalidOperationException">The associated
        /// <see cref="System.Windows.Forms.DataGridView" /> control is performing one of the following actions that temporarily prevents new rows from being added:Selecting all cells in the control.Clearing the selection.-or-This method is being called from a handler for one of the following
        /// <see cref="System.Windows.Forms.DataGridView" /> events:
        /// <see cref="System.Windows.Forms.DataGridView.CellEnter" />
        /// <see cref="System.Windows.Forms.DataGridView.CellLeave" />
        /// <see cref="System.Windows.Forms.DataGridView.CellValidating" />
        /// <see cref="System.Windows.Forms.DataGridView.CellValidated" />
        /// <see cref="System.Windows.Forms.DataGridView.RowEnter" />
        /// <see cref="System.Windows.Forms.DataGridView.RowLeave" />
        /// <see cref="System.Windows.Forms.DataGridView.RowValidated" />
        /// <see cref="System.Windows.Forms.DataGridView.RowValidating" />-or-The
        /// <see cref="System.Windows.Forms.DataGridView.DataSource" /> property of the
        /// <see cref="System.Windows.Forms.DataGridView" /> is not null.-or-The
        /// <see cref="System.Windows.Forms.DataGridView" /> has no columns.-or-
        /// <paramref name="rowIndex" /> is equal to the number of rows in the collection and the
        /// <see cref="System.Windows.Forms.DataGridView.AllowUserToAddRows" /> property of the
        /// <see cref="System.Windows.Forms.DataGridView" /> is set to true.-or-The row returned by the
        /// <see cref="System.Windows.Forms.DataGridView.RowTemplate" /> property has more cells than there are columns in the control. -or-This operation would insert a frozen row after unfrozen rows or an unfrozen row before frozen rows.
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public virtual void Insert(int rowIndex, int count)
        {
            DataGridViewRow[] rows = new DataGridViewRow[count];
            for (int r = 0; r < count; r++)
            {
                foreach (var col in dataGridView.Columns)
                {
                    var cell = col.CellTemplate;
                    rows[r].Cells.Add(cell);
                }
            }
            InsertRange(rowIndex, rows);
        }
        /// <summary>
        /// Inserts rows into the collection at the specified position.
        /// </summary>
        /// <param name="indexSource">The index of the
        /// <see cref="System.Windows.Forms.DataGridViewRow" /> on which to base the new rows.
        /// </param>
        /// <param name="indexDestination">The position at which to insert the rows.
        /// </param>
        /// <param name="count">The number of
        /// <see cref="System.Windows.Forms.DataGridViewRow" /> objects to add to the
        /// <see cref="System.Windows.Forms.DataGridViewRowCollection" />
        /// </param>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// <paramref name="indexSource" /> is less than zero or greater than the number of rows in the collection minus one.-or-
        /// <paramref name="indexDestination" /> is less than zero or greater than the number of rows in the collection.-or-
        /// <paramref name="count" /> is less than 1.
        /// </exception>
        /// <exception cref="System.InvalidOperationException">The associated
        /// <see cref="System.Windows.Forms.DataGridView" /> control is performing one of the following actions that temporarily prevents new rows from being added:Selecting all cells in the control.Clearing the selection.-or-This method is being called from a handler for one of the following
        /// <see cref="System.Windows.Forms.DataGridView" /> events:
        /// <see cref="System.Windows.Forms.DataGridView.CellEnter" />
        /// <see cref="System.Windows.Forms.DataGridView.CellLeave" />
        /// <see cref="System.Windows.Forms.DataGridView.CellValidating" />
        /// <see cref="System.Windows.Forms.DataGridView.CellValidated" />
        /// <see cref="System.Windows.Forms.DataGridView.RowEnter" />
        /// <see cref="System.Windows.Forms.DataGridView.RowLeave" />
        /// <see cref="System.Windows.Forms.DataGridView.RowValidated" />
        /// <see cref="System.Windows.Forms.DataGridView.RowValidating" />-or-
        /// <paramref name="indexDestination" /> is equal to the number of rows in the collection and
        /// <see cref="System.Windows.Forms.DataGridView.AllowUserToAddRows" /> is true.-or-This operation would insert frozen rows after unfrozen rows or unfrozen rows before frozen rows.
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public virtual void InsertCopies(int indexSource, int indexDestination, int count) { }
        /// <summary>
        /// Inserts a row into the collection at the specified position, based on the row at specified position.
        /// </summary>
        /// <param name="indexSource">The index of the row on which to base the new row.
        /// </param>
        /// <param name="indexDestination">The position at which to insert the row.
        /// </param>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// <paramref name="indexSource" /> is less than zero or greater than the number of rows in the collection minus one.-or-
        /// <paramref name="indexDestination" /> is less than zero or greater than the number of rows in the collection.
        /// </exception>
        /// <exception cref="System.InvalidOperationException">The associated
        /// <see cref="System.Windows.Forms.DataGridView" /> control is performing one of the following actions that temporarily prevents new rows from being added:Selecting all cells in the control.Clearing the selection.-or-This method is being called from a handler for one of the following
        /// <see cref="System.Windows.Forms.DataGridView" /> events:
        /// <see cref="System.Windows.Forms.DataGridView.CellEnter" />
        /// <see cref="System.Windows.Forms.DataGridView.CellLeave" />
        /// <see cref="System.Windows.Forms.DataGridView.CellValidating" />
        /// <see cref="System.Windows.Forms.DataGridView.CellValidated" />
        /// <see cref="System.Windows.Forms.DataGridView.RowEnter" />
        /// <see cref="System.Windows.Forms.DataGridView.RowLeave" />
        /// <see cref="System.Windows.Forms.DataGridView.RowValidated" />
        /// <see cref="System.Windows.Forms.DataGridView.RowValidating" />-or-
        /// <paramref name="indexDestination" /> is equal to the number of rows in the collection and
        /// <see cref="System.Windows.Forms.DataGridView.AllowUserToAddRows" /> is true. -or-This operation would insert a frozen row after unfrozen rows or an unfrozen row before frozen rows.
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public virtual void InsertCopy(int indexSource, int indexDestination) { }
        /// <summary>
        /// Inserts the
        /// <see cref="System.Windows.Forms.DataGridViewRow" /> objects into the collection at the specified position.
        /// </summary>
        /// <param name="rowIndex">The position at which to insert the rows.
        /// </param>
        /// <param name="dataGridViewRows">An array of
        /// <see cref="System.Windows.Forms.DataGridViewRow" /> objects to add to the
        /// <see cref="System.Windows.Forms.DataGridViewRowCollection" />
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="dataGridViewRows" /> is null.
        /// </exception>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// <paramref name="rowIndex" />
        /// is less than zero or greater than the number of rows in the collection.
        /// </exception>
        /// <exception cref="System.ArgumentException">
        /// <paramref name="dataGridViewRows" /> contains only one row, and the row it contains has more cells than there are columns in the control.
        /// </exception>
        /// <exception cref="System.InvalidOperationException">The associated
        /// <see cref="System.Windows.Forms.DataGridView" /> control is performing one of the following actions that temporarily prevents new rows from being added:Selecting all cells in the control.Clearing the selection.-or-This method is being called from a handler for one of the following
        /// <see cref="System.Windows.Forms.DataGridView" /> events:
        /// <see cref="System.Windows.Forms.DataGridView.CellEnter" />
        /// <see cref="System.Windows.Forms.DataGridView.CellLeave" />
        /// <see cref="System.Windows.Forms.DataGridView.CellValidating" />
        /// <see cref="System.Windows.Forms.DataGridView.CellValidated" />
        /// <see cref="System.Windows.Forms.DataGridView.RowEnter" />
        /// <see cref="System.Windows.Forms.DataGridView.RowLeave" />
        /// <see cref="System.Windows.Forms.DataGridView.RowValidated" />
        /// <see cref="System.Windows.Forms.DataGridView.RowValidating" />-or-
        /// <paramref name="rowIndex" /> is equal to the number of rows in the collection and
        /// <see cref="System.Windows.Forms.DataGridView.AllowUserToAddRows" /> is true.-or-The
        /// <see cref="System.Windows.Forms.DataGridView.DataSource" /> property of the
        /// <see cref="System.Windows.Forms.DataGridView" /> is not null.-or-At least one entry in the
        /// <paramref name="dataGridViewRows" /> array is null.-or-The
        /// <see cref="System.Windows.Forms.DataGridView" /> has no columns.-or-At least one row in the
        /// <paramref name="dataGridViewRows" /> array has a
        /// <see cref="System.Windows.Forms.DataGridViewElement.DataGridView" /> property value that is not null.-or-At least one row in the
        /// <paramref name="dataGridViewRows" /> array has a
        /// <see cref="System.Windows.Forms.DataGridViewRow.Selected" /> property value of true.-or-Two or more rows in the
        /// <paramref name="dataGridViewRows" /> array are identical.-or-At least one row in the
        /// <paramref name="dataGridViewRows" /> array contains one or more cells of a type that is incompatible with the type of the corresponding column in the control.-or-At least one row in the
        /// <paramref name="dataGridViewRows" /> array contains more cells than there are columns in the control. -or-This operation would insert frozen rows after unfrozen rows or unfrozen rows before frozen rows.
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public virtual void InsertRange(int rowIndex, params DataGridViewRow[] dataGridViewRows)
        {
            InsertGtkStore(rowIndex, dataGridViewRows);
        }
        /// <summary>
        /// Removes the row from the collection.
        /// </summary>
        /// <param name="dataGridViewRow">The row to remove from the
        /// <see cref="System.Windows.Forms.DataGridViewRowCollection" />
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="dataGridViewRow" /> is null.
        /// </exception>
        /// <exception cref="System.ArgumentException">
        /// <paramref name="dataGridViewRow" />
        /// is not contained in this collection.-or-
        /// <paramref name="dataGridViewRow" /> is a shared row.
        /// </exception>
        /// <exception cref="System.InvalidOperationException">The associated
        /// <see cref="System.Windows.Forms.DataGridView" /> control is performing one of the following actions that temporarily prevents new rows from being added:Selecting all cells in the control.Clearing the selection.-or-This method is being called from a handler for one of the following
        /// <see cref="System.Windows.Forms.DataGridView" /> events:
        /// <see cref="System.Windows.Forms.DataGridView.CellEnter" />
        /// <see cref="System.Windows.Forms.DataGridView.CellLeave" />
        /// <see cref="System.Windows.Forms.DataGridView.CellValidating" />
        /// <see cref="System.Windows.Forms.DataGridView.CellValidated" />
        /// <see cref="System.Windows.Forms.DataGridView.RowEnter" />
        /// <see cref="System.Windows.Forms.DataGridView.RowLeave" />
        /// <see cref="System.Windows.Forms.DataGridView.RowValidated" />
        /// <see cref="System.Windows.Forms.DataGridView.RowValidating" />-or-
        /// <paramref name="dataGridViewRow" /> is the row for new records.-or-The associated
        /// <see cref="System.Windows.Forms.DataGridView" /> control is bound to an
        /// <see cref="System.ComponentModel.IBindingList" /> implementation with
        /// <see cref="System.ComponentModel.IBindingList.AllowRemove" /> and
        /// <see cref="System.ComponentModel.IBindingList.SupportsChangeNotification" /> property values that are not both true.
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public virtual void Remove(DataGridViewRow dataGridViewRow)
        {
            RemoveAt(dataGridViewRow.Index);
        }
        /// <summary>
        /// Removes the row at the specified position from the collection.
        /// </summary>
        /// <param name="index">The position of the row to remove.
        /// </param>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// <paramref name="index" /> is less than zero and greater than the number of rows in the collection minus one.
        /// </exception>
        /// <exception cref="System.InvalidOperationException">The associated
        /// <see cref="System.Windows.Forms.DataGridView" /> control is performing one of the following actions that temporarily prevents new rows from being added:Selecting all cells in the control.Clearing the selection.-or-This method is being called from a handler for one of the following
        /// <see cref="System.Windows.Forms.DataGridView" /> events:
        /// <see cref="System.Windows.Forms.DataGridView.CellEnter" />
        /// <see cref="System.Windows.Forms.DataGridView.CellLeave" />
        /// <see cref="System.Windows.Forms.DataGridView.CellValidating" />
        /// <see cref="System.Windows.Forms.DataGridView.CellValidated" />
        /// <see cref="System.Windows.Forms.DataGridView.RowEnter" />
        /// <see cref="System.Windows.Forms.DataGridView.RowLeave" />
        /// <see cref="System.Windows.Forms.DataGridView.RowValidated" />
        /// <see cref="System.Windows.Forms.DataGridView.RowValidating" />-or-
        /// <paramref name="index" /> is equal to the number of rows in the collection and the
        /// <see cref="System.Windows.Forms.DataGridView.AllowUserToAddRows" /> property of the
        /// <see cref="System.Windows.Forms.DataGridView" /> is set to true.-or-The associated
        /// <see cref="System.Windows.Forms.DataGridView" /> control is bound to an
        /// <see cref="System.ComponentModel.IBindingList" /> implementation with
        /// <see cref="System.ComponentModel.IBindingList.AllowRemove" /> and
        /// <see cref="System.ComponentModel.IBindingList.SupportsChangeNotification" /> property values that are not both true.
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public virtual void RemoveAt(int index)
        {
            if (dataGridView.Store.GetIter(out TreeIter iter, new TreePath(new int[] { index })))
            {
                dataGridView.Store.Remove(ref iter);
            }
            items.RemoveAt(index);
            //reset id
            for (int i = 0; i < items.Count; i++)
            {
                ((DataGridViewRow)items[i]).Index = i;
            }
        }
        /// <summary>
        /// Returns the
        /// <see cref="System.Windows.Forms.DataGridViewRow" /> at the specified index.
        /// </summary>
        /// <returns>The
        /// <see cref="System.Windows.Forms.DataGridViewRow" /> positioned at the specified index.
        /// </returns>
        /// <param name="rowIndex">The index of the
        /// <see cref="System.Windows.Forms.DataGridViewRow" /> to get.
        /// </param>
        /// <filterpriority>1
        /// </filterpriority>
        public DataGridViewRow SharedRow(int rowIndex)
        {
            bool hasiter = dataGridView.Store.GetIter(out TreeIter iter, new TreePath(new int[] { rowIndex }));
            if (hasiter)
            {
                foreach (DataGridViewRow item in items)
                {
                    if (item.TreeIter.Equals(iter.UserData))
                        return item;
                }
            }
            return (DataGridViewRow)SharedList[rowIndex];
        }
        /// <summary>
        /// Raises the
        /// <see cref="System.Windows.Forms.DataGridViewRowCollection.CollectionChanged" /> event.
        /// </summary>
        /// <param name="e">A
        /// <see cref="System.ComponentModel.CollectionChangeEventArgs" /> that contains the event data.
        /// </param>
//**************************************
        protected virtual void OnCollectionChanged(CollectionChangeEventArgs e)
        {
            if (CollectionChanged != null)
                CollectionChanged(dataGridView, e);
        }
        /// <summary>
        /// Adds a
        /// <see cref="System.Windows.Forms.DataGridViewRow" /> to the collection.
        /// </summary>
        /// <returns>The index of the new
        /// <see cref="System.Windows.Forms.DataGridViewRow" />
        /// </returns>
        /// <param name="value">The
        /// <see cref="System.Object" /> to add to the
        /// <see cref="System.Collections.IList" />
        /// </param>
        /// <exception cref="System.InvalidCastException">
        /// <paramref name="value" /> is not a
        /// <see cref="System.Windows.Forms.DataGridViewRow" />
        /// </exception>
        /// <exception cref="System.InvalidOperationException">The associated
        /// <see cref="System.Windows.Forms.DataGridView" /> control is performing one of the following actions that temporarily prevents new rows from being added:Selecting all cells in the control.Clearing the selection.-or-This method is being called from a handler for one of the following
        /// <see cref="System.Windows.Forms.DataGridView" /> events:
        /// <see cref="System.Windows.Forms.DataGridView.CellEnter" />
        /// <see cref="System.Windows.Forms.DataGridView.CellLeave" />
        /// <see cref="System.Windows.Forms.DataGridView.CellValidating" />
        /// <see cref="System.Windows.Forms.DataGridView.CellValidated" />
        /// <see cref="System.Windows.Forms.DataGridView.RowEnter" />
        /// <see cref="System.Windows.Forms.DataGridView.RowLeave" />
        /// <see cref="System.Windows.Forms.DataGridView.RowValidated" />
        /// <see cref="System.Windows.Forms.DataGridView.RowValidating" />-or-The
        /// <see cref="System.Windows.Forms.DataGridView.DataSource" /> property of the
        /// <see cref="System.Windows.Forms.DataGridView" /> is not null.-or-The
        /// <see cref="System.Windows.Forms.DataGridView" /> has no columns.-or-The
        /// <see cref="System.Windows.Forms.DataGridViewElement.DataGridView" /> property of the
        /// <paramref name="value" /> is not null.-or-
        /// <paramref name="value" /> has a
        /// <see cref="System.Windows.Forms.DataGridViewRow.Selected" /> property value of true.-or-This operation would add a frozen row after unfrozen rows.
        /// </exception>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="value" />
        /// is null.
        /// </exception>
        /// <exception cref="System.ArgumentException">
        /// <paramref name="value" /> has more cells than there are columns in the control.
        /// </exception>
        int IList.Add(object value)
        {
            return Add((DataGridViewRow)value);
        }
        /// <summary>
        /// Removes all items from the collection.
        /// </summary>
        /// <exception cref="System.InvalidOperationException">The collection is data bound and the underlying data source does not support clearing the row data.-or-The associated
        /// <see cref="System.Windows.Forms.DataGridView" /> control is performing one of the following actions that temporarily prevents new rows from being added:Selecting all cells in the control.Clearing the selection.-or-This method is being called from a handler for one of the following
        /// <see cref="System.Windows.Forms.DataGridView" /> events:
        /// <see cref="System.Windows.Forms.DataGridView.CellEnter" />
        /// <see cref="System.Windows.Forms.DataGridView.CellLeave" />
        /// <see cref="System.Windows.Forms.DataGridView.CellValidating" />
        /// <see cref="System.Windows.Forms.DataGridView.CellValidated" />
        /// <see cref="System.Windows.Forms.DataGridView.RowEnter" />
        /// <see cref="System.Windows.Forms.DataGridView.RowLeave" />
        /// <see cref="System.Windows.Forms.DataGridView.RowValidated" />
        /// <see cref="System.Windows.Forms.DataGridView.RowValidating" />
        /// </exception>
        void IList.Clear()
        {
            Clear();
        }
        /// <summary>
        /// Determines whether the collection contains the specified item.
        /// </summary>
        /// <returns>true if
        /// <paramref name="value" /> is a
        /// <see cref="System.Windows.Forms.DataGridViewRow" /> found in the
        /// <see cref="System.Windows.Forms.DataGridViewRowCollection" />; otherwise, false.
        /// </returns>
        /// <param name="value">The item to locate in the
        /// <see cref="System.Windows.Forms.DataGridViewRowCollection" />
        /// </param>
        bool IList.Contains(object value)
        {
            return items.Contains(value);
        }
        /// <summary>
        /// Returns the index of a specified item in the collection.
        /// </summary>
        /// <returns>The index of
        /// <paramref name="value" /> if it is a
        /// <see cref="System.Windows.Forms.DataGridViewRow" /> found in the list; otherwise, -1.
        /// </returns>
        /// <param name="value">The object to locate in the
        /// <see cref="System.Windows.Forms.DataGridViewRowCollection" />
        /// </param>
        int IList.IndexOf(object value)
        {
            return items.IndexOf(value);
        }
        /// <summary>
        /// Inserts a
        /// <see cref="System.Windows.Forms.DataGridViewRow" /> into the collection at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which
        /// <paramref name="value" /> should be inserted.
        /// </param>
        /// <param name="value">The
        /// <see cref="System.Windows.Forms.DataGridViewRow" /> to insert into the
        /// <see cref="System.Windows.Forms.DataGridViewRowCollection" />
        /// </param>
        /// <exception cref="System.InvalidCastException">
        /// <paramref name="value" /> is not a
        /// <see cref="System.Windows.Forms.DataGridViewRow" />
        /// </exception>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// <paramref name="index" />
        /// is less than zero or greater than the number of rows in the collection.
        /// </exception>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="value" /> is null.
        /// </exception>
        /// <exception cref="System.InvalidOperationException">The associated
        /// <see cref="System.Windows.Forms.DataGridView" /> control is performing one of the following actions that temporarily prevents new rows from being added:Selecting all cells in the control.Clearing the selection.-or-This method is being called from a handler for one of the following
        /// <see cref="System.Windows.Forms.DataGridView" /> events:
        /// <see cref="System.Windows.Forms.DataGridView.CellEnter" />
        /// <see cref="System.Windows.Forms.DataGridView.CellLeave" />
        /// <see cref="System.Windows.Forms.DataGridView.CellValidating" />
        /// <see cref="System.Windows.Forms.DataGridView.CellValidated" />
        /// <see cref="System.Windows.Forms.DataGridView.RowEnter" />
        /// <see cref="System.Windows.Forms.DataGridView.RowLeave" />
        /// <see cref="System.Windows.Forms.DataGridView.RowValidated" />
        /// <see cref="System.Windows.Forms.DataGridView.RowValidating" />-or-The
        /// <see cref="System.Windows.Forms.DataGridView.DataSource" /> property of the
        /// <see cref="System.Windows.Forms.DataGridView" /> is not null.-or-
        /// <paramref name="index" /> is equal to the number of rows in the collection and the
        /// <see cref="System.Windows.Forms.DataGridView.AllowUserToAddRows" /> property of the
        /// <see cref="System.Windows.Forms.DataGridView" /> is set to true.-or-The
        /// <see cref="System.Windows.Forms.DataGridView" /> has no columns.-or-The
        /// <see cref="System.Windows.Forms.DataGridViewElement.DataGridView" /> property of the
        /// <paramref name="value" /> is not null.-or-
        /// <paramref name="value" /> has a
        /// <see cref="System.Windows.Forms.DataGridViewRow.Selected" /> property value of true.-or-This operation would insert a frozen row after unfrozen rows or an unfrozen row before frozen rows.
        /// </exception>
        /// <exception cref="System.ArgumentException">
        /// <paramref name="value" />
        /// has more cells than there are columns in the control.
        /// </exception>
        void IList.Insert(int index, object value)
        {
            Insert(index, (DataGridViewRow)value);
        }
        /// <summary>
        /// Removes the specified
        /// <see cref="System.Windows.Forms.DataGridViewRow" /> from the collection.
        /// </summary>
        /// <param name="value">The
        /// <see cref="System.Windows.Forms.DataGridViewRow" /> to remove from the
        /// <see cref="System.Windows.Forms.DataGridViewRowCollection" />
        /// </param>
        /// <exception cref="System.InvalidCastException">
        /// <paramref name="value" /> is not a
        /// <see cref="System.Windows.Forms.DataGridViewRow" />
        /// </exception>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="value" />
        /// is null.
        /// </exception>
        /// <exception cref="System.ArgumentException">
        /// <paramref name="value" /> is not contained in this collection.-or-
        /// <paramref name="value" /> is a shared row.
        /// </exception>
        /// <exception cref="System.InvalidOperationException">The associated
        /// <see cref="System.Windows.Forms.DataGridView" /> control is performing one of the following actions that temporarily prevents new rows from being added:Selecting all cells in the control.Clearing the selection.-or-This method is being called from a handler for one of the following
        /// <see cref="System.Windows.Forms.DataGridView" /> events:
        /// <see cref="System.Windows.Forms.DataGridView.CellEnter" />
        /// <see cref="System.Windows.Forms.DataGridView.CellLeave" />
        /// <see cref="System.Windows.Forms.DataGridView.CellValidating" />
        /// <see cref="System.Windows.Forms.DataGridView.CellValidated" />
        /// <see cref="System.Windows.Forms.DataGridView.RowEnter" />
        /// <see cref="System.Windows.Forms.DataGridView.RowLeave" />
        /// <see cref="System.Windows.Forms.DataGridView.RowValidated" />
        /// <see cref="System.Windows.Forms.DataGridView.RowValidating" />-or-
        /// <paramref name="value" /> is the row for new records.-or-The associated
        /// <see cref="System.Windows.Forms.DataGridView" /> control is bound to an
        /// <see cref="System.ComponentModel.IBindingList" /> implementation with
        /// <see cref="System.ComponentModel.IBindingList.AllowRemove" /> and
        /// <see cref="System.ComponentModel.IBindingList.SupportsChangeNotification" /> property values that are not both true.
        /// </exception>
        void IList.Remove(object value)
        {
            Remove((DataGridViewRow)value);
        }
        /// <summary>
        /// Removes the
        /// <see cref="System.Windows.Forms.DataGridViewRow" /> from the collection at the specified position.
        /// </summary>
        /// <param name="index">The zero-based index of the
        /// <see cref="System.Windows.Forms.DataGridViewRow" /> to remove.
        /// </param>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// <paramref name="index" /> is less than zero and greater than the number of rows in the collection minus one.
        /// </exception>
        /// <exception cref="System.InvalidOperationException">The associated
        /// <see cref="System.Windows.Forms.DataGridView" /> control is performing one of the following actions that temporarily prevents new rows from being added:Selecting all cells in the control.Clearing the selection.-or-This method is being called from a handler for one of the following
        /// <see cref="System.Windows.Forms.DataGridView" /> events:
        /// <see cref="System.Windows.Forms.DataGridView.CellEnter" />
        /// <see cref="System.Windows.Forms.DataGridView.CellLeave" />
        /// <see cref="System.Windows.Forms.DataGridView.CellValidating" />
        /// <see cref="System.Windows.Forms.DataGridView.CellValidated" />
        /// <see cref="System.Windows.Forms.DataGridView.RowEnter" />
        /// <see cref="System.Windows.Forms.DataGridView.RowLeave" />
        /// <see cref="System.Windows.Forms.DataGridView.RowValidated" />
        /// <see cref="System.Windows.Forms.DataGridView.RowValidating" />-or-
        /// <paramref name="index" /> is equal to the number of rows in the collection and the
        /// <see cref="System.Windows.Forms.DataGridView.AllowUserToAddRows" /> property of the
        /// <see cref="System.Windows.Forms.DataGridView" /> is set to true.-or-The associated
        /// <see cref="System.Windows.Forms.DataGridView" /> control is bound to an
        /// <see cref="System.ComponentModel.IBindingList" /> implementation with
        /// <see cref="System.ComponentModel.IBindingList.AllowRemove" /> and
        /// <see cref="System.ComponentModel.IBindingList.SupportsChangeNotification" /> property values that are not both true.
        /// </exception>
        void IList.RemoveAt(int index)
        {
            RemoveAt(index);
        }
        /// <summary>
        /// Copies the elements of the collection to an
        /// <see cref="System.Array" />, starting at the specified index.
        /// </summary>
        /// <param name="array">The one-dimensional
        /// <see cref="System.Array" /> that is the destination of the elements copied from the
        /// <see cref="System.Windows.Forms.DataGridViewRowCollection" /> The
        /// <see cref="System.Array" /> must have zero-based indexing.
        /// </param>
        /// <param name="index">The zero-based index in
        /// <paramref name="array" /> at which copying begins.
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="array" /> is null.
        /// </exception>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// <paramref name="index" />
        /// is less than zero.
        /// </exception>
        /// <exception cref="System.ArgumentException">
        /// <paramref name="array" /> is multidimensional.-or- The number of elements in the
        /// <see cref="System.Windows.Forms.DataGridViewRowCollection" /> is greater than the available space from
        /// <paramref name="index" /> to the end of
        /// <paramref name="array" />
        /// </exception>
        /// <exception cref="System.InvalidCastException">The type of the source
        /// <see cref="System.Windows.Forms.DataGridViewRowCollection" /> cannot be cast automatically to the type of
        /// <paramref name="array" />
        /// </exception>
        void ICollection.CopyTo(Array array, int index)
        {
            items.CopyTo(array, index);
        }
        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>An
        /// <see cref="System.Collections.IEnumerator" /> that can be used to iterate through the collection.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return items.GetEnumerator();
        }

    }
}
