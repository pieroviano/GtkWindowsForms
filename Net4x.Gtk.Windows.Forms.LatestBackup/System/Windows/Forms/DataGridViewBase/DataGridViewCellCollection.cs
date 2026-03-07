using Atk;
using GLib;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;

namespace System.Windows.Forms
{
    /// <summary>
    /// Represents a collection of cells in a
    /// <see cref="System.Windows.Forms.DataGridViewRow" />
    /// </summary>
    /// <filterpriority> 2
    /// </filterpriority>
    [ListBindable(false)]
    public class DataGridViewCellCollection : BaseCollection, IList, ICollection, IEnumerable
    {

        private ArrayList items = new ArrayList();

        private DataGridViewRow owner;

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="System.Windows.Forms.DataGridViewCellCollection" /> class.
        /// </summary>
        /// <param name="dataGridViewRow">The
        /// <see cref="System.Windows.Forms.DataGridViewRow" /> that owns the collection.
        /// </param>
        public DataGridViewCellCollection(DataGridViewRow dataGridViewRow)
        {
            owner = dataGridViewRow;
        }

        public override int Count
        {
            get { return items.Count; }
        }

        public bool IsFixedSize => false;

        public DataGridViewRow Row { set => owner = value; get => owner; }

        /// <summary>
        /// Gets an
        /// <see cref="System.Collections.ArrayList" /> containing
        /// <see cref="System.Windows.Forms.DataGridViewCellCollection" /> objects.
        /// </summary>
        /// <returns>
        /// <see cref="System.Collections.ArrayList" />
        /// </returns>
        protected override ArrayList List => items;

        public DataGridViewCell this[int index]
        {
            get
            {
                return (DataGridViewCell)items[index];
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }
                items[index] = value;
            }
        }

        public DataGridViewCell this[string columnName]
        {
            get
            {
                DataGridViewColumn dataGridViewColumn = null;
                if (owner.DataGridView != null)
                {
                    dataGridViewColumn = owner.DataGridView.Columns[columnName];
                }

                if (dataGridViewColumn == null)
                {
                    throw new ArgumentException("DataGridViewColumnCollection_ColumnNotFound", "columnName");
                }

                return (DataGridViewCell)items[dataGridViewColumn.Index];
            }
            set
            {
                DataGridViewColumn dataGridViewColumn = null;
                if (owner.DataGridView != null)
                {
                    dataGridViewColumn = owner.DataGridView.Columns[columnName];
                }

                if (dataGridViewColumn == null)
                {
                    throw new ArgumentException("DataGridViewColumnCollection_ColumnNotFound", "columnName");
                }

                this[dataGridViewColumn.Index] = value;
            }
        }

        object IList.this[int index] { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        /// <summary>
        /// Adds a cell to the collection.
        /// </summary>
        /// <returns>The position in which to insert the new element.
        /// </returns>
        /// <param name="dataGridViewCell">A
        /// <see cref="System.Windows.Forms.DataGridViewCell" /> to add to the collection.
        /// </param>
        /// <exception cref="System.InvalidOperationException">The row that owns this
        /// <see cref="System.Windows.Forms.DataGridViewCellCollection" /> already belongs to a
        /// <see cref="System.Windows.Forms.DataGridView" /> control.-or-
        /// <paramref name="dataGridViewCell" /> already belongs to a
        /// <see cref="System.Windows.Forms.DataGridViewRow" />
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        //*****************
        public virtual int Add(DataGridViewCell dataGridViewCell)
        {
            if (owner.DataGridView != null)
            {
                throw new InvalidOperationException("DataGridViewCellCollection_OwningRowAlreadyBelongsToDataGridView");
            }

            if (dataGridViewCell.OwningRow != null)
            {
                throw new InvalidOperationException("DataGridViewCellCollection_CellAlreadyBelongsToDataGridViewRow");
            }
            dataGridViewCell.OwningRowInternal = owner;

            return items.Add(dataGridViewCell);
        }

        /// <summary>
        /// Adds an array of cells to the collection.
        /// </summary>
        /// <param name="dataGridViewCells">The array of
        /// <see cref="System.Windows.Forms.DataGridViewCell" /> objects to add to the collection.
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="dataGridViewCells" /> is null.
        /// </exception>
        /// <exception cref="System.InvalidOperationException">The row that owns this
        /// <see cref="System.Windows.Forms.DataGridViewCellCollection" /> already belongs to a
        /// <see cref="System.Windows.Forms.DataGridView" /> control.-or-At least one value in
        /// <paramref name="dataGridViewCells" /> is null.-or-At least one cell in
        /// <paramref name="dataGridViewCells" /> already belongs to a
        /// <see cref="System.Windows.Forms.DataGridViewRow" />-or-At least two values in
        /// <paramref name="dataGridViewCells" /> are references to the same
        /// <see cref="System.Windows.Forms.DataGridViewCell" />
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        public virtual void AddRange(params DataGridViewCell[] dataGridViewCells)
        {
            if (dataGridViewCells == null)
            {
                throw new ArgumentNullException("dataGridViewCells");
            }

            items.AddRange(dataGridViewCells);
            foreach (DataGridViewCell dataGridViewCell2 in dataGridViewCells)
            {
                dataGridViewCell2.OwningRowInternal = owner;
            }
        }

        public void AddRange(IEnumerable<DataGridViewCell> dataGridViewCells)
        {
            if (dataGridViewCells == null)
            {
                throw new ArgumentNullException("dataGridViewCells");
            }

            foreach (DataGridViewCell dataGridViewCell2 in dataGridViewCells)
            {
                dataGridViewCell2.OwningRowInternal = owner;
                items.Add(dataGridViewCell2);
            }
        }

        /// <summary>
        /// Clears all cells from the collection.
        /// </summary>
        /// <exception cref="System.InvalidOperationException">The row that owns this
        /// <see cref="System.Windows.Forms.DataGridViewCellCollection" /> already belongs to a
        /// <see cref="System.Windows.Forms.DataGridView" /> control.
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        public virtual void Clear()
        {
            if (owner.DataGridView != null)
            {
                throw new InvalidOperationException("DataGridViewCellCollection_OwningRowAlreadyBelongsToDataGridView");
            }

            foreach (DataGridViewCell item in items)
            {
                item.OwningRowInternal = null;
            }

            items.Clear();
        }

        /// <summary>
        /// Determines whether the specified cell is contained in the collection.
        /// </summary>
        /// <returns>true if
        /// <paramref name="dataGridViewCell" /> is in the collection; otherwise, false.
        /// </returns>
        /// <param name="dataGridViewCell">A
        /// <see cref="System.Windows.Forms.DataGridViewCell" /> to locate in the collection.
        /// </param>
        /// <filterpriority>1
        /// </filterpriority>
        public virtual bool Contains(DataGridViewCell dataGridViewCell)
        {
            int num = items.IndexOf(dataGridViewCell);

            return num != -1;
        }

        /// <summary>
        /// Copies the entire collection of cells into an array at a specified location within the array.
        /// </summary>
        /// <param name="array">The destination array to which the contents will be copied.
        /// </param>
        /// <param name="index">The index of the element in
        /// <paramref name="array" /> at which to start copying.
        /// </param>
        /// <filterpriority>1
        /// </filterpriority>
        public void CopyTo(DataGridViewCell[] array, int index)
        {
            items.CopyTo(array, index);
        }

        public override IEnumerator GetEnumerator()
        {
            return items.GetEnumerator();
        }

        /// <summary>
        /// Returns the index of the specified cell.
        /// </summary>
        /// <returns>The zero-based index of the value of
        /// <paramref name="dataGridViewCell" /> parameter, if it is found in the collection; otherwise, -1.
        /// </returns>
        /// <param name="dataGridViewCell">The cell to locate in the collection.
        /// </param>
        /// <filterpriority>1
        /// </filterpriority>
        public int IndexOf(DataGridViewCell dataGridViewCell)
        {
            return items.IndexOf(dataGridViewCell);
        }

        /// <summary>
        /// Inserts a cell into the collection at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which to place
        /// <paramref name="dataGridViewCell" />
        /// </param>
        /// <param name="dataGridViewCell">The
        /// <see cref="System.Windows.Forms.DataGridViewCell" /> to insert.
        /// </param>
        /// <exception cref="System.InvalidOperationException">The row that owns this
        /// <see cref="System.Windows.Forms.DataGridViewCellCollection" /> already belongs to a
        /// <see cref="System.Windows.Forms.DataGridView" /> control.-or-
        /// <paramref name="dataGridViewCell" /> already belongs to a
        /// <see cref="System.Windows.Forms.DataGridViewRow" />
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        public virtual void Insert(int index, DataGridViewCell dataGridViewCell)
        {
            if (owner.DataGridView != null)
            {
                throw new InvalidOperationException("DataGridViewCellCollection_OwningRowAlreadyBelongsToDataGridView");
            }

            if (dataGridViewCell.OwningRow != null)
            {
                throw new InvalidOperationException("DataGridViewCellCollection_CellAlreadyBelongsToDataGridViewRow");
            }

            items.Insert(index, dataGridViewCell);
            dataGridViewCell.OwningRowInternal = owner;
        }

        /// <summary>
        /// Removes the specified cell from the collection.
        /// </summary>
        /// <param name="cell">The
        /// <see cref="System.Windows.Forms.DataGridViewCell" /> to remove from the collection.
        /// </param>
        /// <exception cref="System.InvalidOperationException">The row that owns this
        /// <see cref="System.Windows.Forms.DataGridViewCellCollection" /> already belongs to a
        /// <see cref="System.Windows.Forms.DataGridView" /> control.
        /// </exception>
        /// <exception cref="System.ArgumentException">
        /// <paramref name="cell" /> could not be found in the collection.
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public virtual void Remove(DataGridViewCell cell)
        {
            if (owner.DataGridView != null)
            {
                throw new InvalidOperationException("DataGridViewCellCollection_OwningRowAlreadyBelongsToDataGridView");
            }

            int num = -1;
            int count = items.Count;
            for (int i = 0; i < count; i++)
            {
                if (items[i] == cell)
                {
                    num = i;
                    break;
                }
            }

            if (num == -1)
            {
                throw new ArgumentException("DataGridViewCellCollection_CellNotFound");
            }

            RemoveAt(num);
        }

        /// <summary>
        /// Removes the cell at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the
        /// <see cref="System.Windows.Forms.DataGridViewCell" /> to be removed.
        /// </param>
        /// <exception cref="System.InvalidOperationException">The row that owns this
        /// <see cref="System.Windows.Forms.DataGridViewCellCollection" /> already belongs to a
        /// <see cref="System.Windows.Forms.DataGridView" /> control.
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
            if (owner.DataGridView != null)
            {
                throw new InvalidOperationException("DataGridViewCellCollection_OwningRowAlreadyBelongsToDataGridView");
            }

            DataGridViewCell dataGridViewCell = (DataGridViewCell)items[index];
            dataGridViewCell.OwningRowInternal = null;
            items.RemoveAt(index);
        }

        internal List<TOutput> ConvertAll<TOutput>(Converter<DataGridViewCell, TOutput> converter)
        {
            List<TOutput> data = new List<TOutput>();
            foreach (DataGridViewCell cell in items)
                data.Add(converter(cell));

            return data;
        }

        /// <summary>
        /// Adds an item to the collection.
        /// </summary>
        /// <returns>The position into which the new element was inserted.
        /// </returns>
        /// <param name="value">The
        /// <see cref="System.Windows.Forms.DataGridViewCell" /> to add to the collection.
        /// </param>
        /// <exception cref="System.InvalidCastException">
        /// <paramref name="value" /> is not a
        /// <see cref="System.Windows.Forms.DataGridViewCell" />
        /// </exception>
        /// <exception cref="System.InvalidOperationException">The row that owns this
        /// <see cref="System.Windows.Forms.DataGridViewCellCollection" /> already belongs to a
        /// <see cref="System.Windows.Forms.DataGridView" /> control.-or-
        /// <paramref name="value" /> represents a cell that already belongs to a
        /// <see cref="System.Windows.Forms.DataGridViewRow" />
        /// </exception>
        int IList.Add(object value)
        {
            return Add((DataGridViewCell)value);
        }

        /// <summary>
        /// Clears the collection.
        /// </summary>
        /// <exception cref="System.InvalidOperationException">The row that owns this
        /// <see cref="System.Windows.Forms.DataGridViewCellCollection" /> already belongs to a
        /// <see cref="System.Windows.Forms.DataGridView" /> control.
        /// </exception>
        void IList.Clear()
        {
            Clear();
        }

        /// <summary>
        /// Determines whether the collection contains the specified value.
        /// </summary>
        /// <returns>true if the
        /// <paramref name="value" /> is found in the
        /// <see cref="System.Windows.Forms.DataGridViewCellCollection" />; otherwise, false.
        /// </returns>
        /// <param name="value">The object to locate in the
        /// <see cref="System.Windows.Forms.DataGridViewCellCollection" />
        /// </param>
        bool IList.Contains(object value)
        {
            return items.Contains(value);
        }

        /// <summary>
        /// Copies the elements of the collection to the specified array, starting at the specified index.
        /// </summary>
        /// <param name="array">The one-dimensional array that is the destination of the elements copied from the collection. The array must have zero-based indexing.
        /// </param>
        /// <param name="index">The zero-based index in the array at which copying begins.
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="array" /> is null.
        /// </exception>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// <paramref name="index" /> is less than zero.
        /// </exception>
        /// <exception cref="System.ArgumentException">
        /// <paramref name="array" /> is multidimensional.-or-The number of elements in the
        /// <see cref="System.Windows.Forms.DataGridViewCellCollection" /> is greater than the available space from
        /// <paramref name="index" /> to the end of
        /// <paramref name="array" />
        /// </exception>
        /// <exception cref="System.InvalidCastException">The
        /// <see cref="System.Windows.Forms.DataGridViewCellCollection" /> cannot be cast automatically to the type of
        /// <paramref name="array" />
        /// </exception>
        void ICollection.CopyTo(Array array, int index)
        {
            items.CopyTo(array, index);
        }

        /// <summary>
        /// Determines the index of a specific item in a collection.
        /// </summary>
        /// <returns>The index of value if found in the list; otherwise, -1.
        /// </returns>
        /// <param name="value">The object to locate in the
        /// <see cref="System.Windows.Forms.DataGridViewCellCollection" />
        /// </param>
        int IList.IndexOf(object value)
        {
            return items.IndexOf(value);
        }

        /// <summary>
        /// Inserts an item into the collection at the specified position.
        /// </summary>
        /// <param name="index">The zero-based index at which value should be inserted.
        /// </param>
        /// <param name="value">The
        /// <see cref="System.Windows.Forms.DataGridViewCell" /> to insert into the
        /// <see cref="System.Windows.Forms.DataGridViewCellCollection.System#Collections#IList#Insert(System.Int32,System.Object)" />
        /// </param>
        /// <exception cref="System.InvalidCastException">
        /// <paramref name="value" /> is not a
        /// <see cref="System.Windows.Forms.DataGridViewCell" />
        /// </exception>
        /// <exception cref="System.InvalidOperationException">The row that owns this
        /// <see cref="System.Windows.Forms.DataGridViewCellCollection" /> already belongs to a
        /// <see cref="System.Windows.Forms.DataGridView" /> control.-or-
        /// <paramref name="dataGridViewCell" /> already belongs to a
        /// <see cref="System.Windows.Forms.DataGridViewRow" />
        /// </exception>
        void IList.Insert(int index, object value)
        {
            Insert(index, (DataGridViewCell)value);
        }

        /// <summary>
        /// Removes the first occurrence of a specific object from the collection.
        /// </summary>
        /// <param name="value">The
        /// <see cref="System.Windows.Forms.DataGridViewCell" /> to remove from the collection.
        /// </param>
        /// <exception cref="System.InvalidCastException">
        /// <paramref name="value" /> is not a
        /// <see cref="System.Windows.Forms.DataGridViewCell" />
        /// </exception>
        /// <exception cref="System.InvalidOperationException">The row that owns this
        /// <see cref="System.Windows.Forms.DataGridViewCellCollection" /> already belongs to a
        /// <see cref="System.Windows.Forms.DataGridView" /> control.
        /// </exception>
        /// <exception cref="System.ArgumentException">
        /// <paramref name="cell" /> could not be found in the collection.
        /// </exception>
        void IList.Remove(object value)
        {
            Remove((DataGridViewCell)value);
        }

        /// <summary>
        /// Removes the
        /// <see cref="System.Windows.Forms.DataGridViewCell" /> at the specified index.
        /// </summary>
        /// <param name="index">The position at which to remove the
        /// <see cref="System.Windows.Forms.DataGridViewCell" />
        /// </param>
        /// <exception cref="System.InvalidOperationException">The row that owns this
        /// <see cref="System.Windows.Forms.DataGridViewCellCollection" /> already belongs to a
        /// <see cref="System.Windows.Forms.DataGridView" /> control.
        /// </exception>
        void IList.RemoveAt(int index)
        {
            RemoveAt(index);
        }

    }
}
