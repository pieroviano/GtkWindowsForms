using Gtk;
using System.Collections.Generic;

namespace System.Windows.Forms
{
    /// <summary>
    /// Represents a row in a
    /// <see cref="T:System.Windows.Forms.DataGridView" /> control.
    /// </summary>
    /// <filterpriority>
    /// 2
    /// </filterpriority>
    public class DataGridViewRow
    {

        private DataGridViewCellCollection _cell;

        private DataGridViewRowCollection _children;

        public DataGridViewRow()
        {
            _cell = new DataGridViewCellCollection(this);
        }

        public AccessibleObject AccessibilityObject { get; }

        public DataGridViewCellCollection Cells { get { return _cell; } }

        public DataGridViewRowCollection Children { get => _children ?? new DataGridViewRowCollection(DataGridView, this); }

        public ContextMenuStrip ContextMenuStrip { get; set; }

        public object DataBoundItem { get; }

        public DataGridView DataGridView { get; set; }

        public DataGridViewCellStyle DefaultCellStyle { get; set; }

        public bool Displayed { get; }

        public int DividerHeight { get; set; }

        public string ErrorText { get; set; }

        public bool Frozen { get; set; }

        public DataGridViewRowHeaderCell HeaderCell { get; set; }

        public int Height { get; set; } = 28;

        public int Index { get; internal set; }

        public DataGridViewCellStyle InheritedStyle { get; }

        public bool IsNewRow { get; }

        public int MinimumHeight { get; set; }

        public bool ReadOnly { get; set; }

        public DataGridViewTriState Resizable { get; set; }

        public bool Selected { get => DataGridView.NativeRowGetSelected(Index); set => DataGridView.NativeRowSetSelected(Index, value); }

        //public bool Selected { get; set; }

        public DataGridViewElementStates State { get { return DataGridViewElementStates.None; } }

        public TreeIter TreeIter { get; internal set; }

        public bool Visible { get; set; }

        public int GetHeight(int index)
        {
            return Height;
        }

    }
}
