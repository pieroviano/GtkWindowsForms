/*
 * Based on GTK components, it is a cross-platform interface component that is compatible with the native C# control winform interface.
 * Use this component GtkSystem.Windows.Forms instead of Microsoft.WindowsDesktop.App.WindowsForms, compile once, and run across Windows, Linux, and macOS
 * Technical support 438865652@qq.com，https://www.gtkapp.com, https://gitee.com/easywebfactory, https://github.com/easywebfactory438865652@qq.com，https://www.gtkapp.com, https://gitee.com/easywebfactory, https://github.com/easywebfactory
 * author:chenhongjin
 */

using GLib;
using GtkSystem.Windows.Forms.GtkControls.ControlBase;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Common;
using System.Drawing;
using GtkControls;
using static System.Windows.Forms.TableLayoutControlCollection;

namespace System.Windows.Forms
{
    /// <summary>
    /// Represents a panel that dynamically lays out its contents in a grid composed of rows and columns.
    /// </summary>
    //[ProvideProperty("ColumnSpan", typeof(Control))]
    //[ProvideProperty("RowSpan", typeof(Control))]
    //[ProvideProperty("Row", typeof(Control))]
    //[ProvideProperty("Column", typeof(Control))]
    //[ProvideProperty("CellPosition", typeof(Control))]
    //[DefaultProperty("ColumnCount")]
    [DesignerCategory("Component")]
    public partial class TableLayoutPanel : ContainerControl, IExtenderProvider
    {

        /// <summary>
        /// The self
        /// </summary>
        public readonly TableLayoutPanelBase self = new TableLayoutPanelBase();

        /// <summary>
        /// The column count
        /// </summary>
        private int _ColumnCount;

        /// <summary>
        /// The column styles
        /// </summary>
        private TableLayoutColumnStyleCollection _columnStyles;

        /// <summary>
        /// The controls
        /// </summary>
        private TableLayoutControlCollection _controls;

        /// <summary>
        /// The row count
        /// </summary>
        private int _RowCount;

        /// <summary>
        /// The row styles
        /// </summary>
        private TableLayoutRowStyleCollection _rowStyles;

        /// <summary>
        /// Initializes a new instance of the <see cref="TableLayoutPanel"/> class.
        /// </summary>
        public TableLayoutPanel() : base()
        {
            _controls = new TableLayoutControlCollection(this);
            _columnStyles = new TableLayoutColumnStyleCollection();
            _rowStyles = new TableLayoutRowStyleCollection();
        }

        /// <summary>
        /// Gets or sets the border style for the control.
        /// </summary>
        /// <value>The border style.</value>
        [Browsable(false)]

        public override BorderStyle BorderStyle { get => base.BorderStyle; set => base.BorderStyle = value; }

        /// <summary>
        /// Gets or sets the cell border style.
        /// </summary>
        /// <value>The cell border style.</value>
        [Localizable(true)]
        public TableLayoutPanelCellBorderStyle CellBorderStyle
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the column count.
        /// </summary>
        /// <value>The column count.</value>
        [DefaultValue(0)]
        [Localizable(true)]
        public int ColumnCount
        {
            get => _ColumnCount;
            set
            {
                for (int r = 0; r < _RowCount; r++)
                {
                    for (int c = 0; c < value; c++)
                    {
                        if (grid.GetChildAt(c, r) == null)
                        {
                            grid.Attach(new Gtk.Viewport() { Vexpand = true, Hexpand = true, BorderWidth = 0, Valign = Gtk.Align.Fill, Halign = Gtk.Align.Fill }, c, r, 1, 1);
                        }
                    }
                }
                for (int c = value; c < _ColumnCount; c++)
                {
                    grid.RemoveColumn(value);
                }
                _ColumnCount = value;
                SetColumnsStyles();
            }
        }

        /// <summary>
        /// Gets the column styles.
        /// </summary>
        /// <value>The column styles.</value>
        [DisplayName("Columns")]
        [Browsable(false)]
        public TableLayoutColumnStyleCollection ColumnStyles
        {
            get => _columnStyles;
        }

        /// <summary>
        /// Gets the controls.
        /// </summary>
        /// <value>The controls.</value>
        [Browsable(false)]
        public new TableLayoutControlCollection Controls
        {
            get => _controls;
        }

        /// <summary>
        /// Gets the grid.
        /// </summary>
        /// <value>The grid.</value>
        public Gtk.Grid grid => self.grid;

        /// <summary>
        /// Gets or sets the grow style.
        /// </summary>
        /// <value>The grow style.</value>
        public TableLayoutPanelGrowStyle GrowStyle
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the GTK control.
        /// </summary>
        /// <value>The GTK control.</value>
        public override object GtkControl => self;

        /// <summary>
        /// Gets or sets the row count.
        /// </summary>
        /// <value>The row count.</value>
        [DefaultValue(0)]
        [Localizable(true)]
        public int RowCount
        {
            get => _RowCount;
            set
            {
                for (int r = 0; r < value; r++)
                {
                    for (int c = 0; c < _ColumnCount; c++)
                    {
                        if (grid.GetChildAt(c, r) == null)
                        {
                            grid.Attach(new Gtk.Viewport() { Vexpand = true, Hexpand = true, BorderWidth = 0, Valign = Gtk.Align.Fill, Halign = Gtk.Align.Fill }, c, r, 1, 1);
                        }
                    }
                }
                for (int r = value; r < _RowCount; r++)
                {
                    grid.RemoveRow(value);
                }
                _RowCount = value;
                SetRowsStyles();
            }
        }

        /// <summary>
        /// Gets the row styles.
        /// </summary>
        /// <value>The row styles.</value>
        [DisplayName("Rows")]
        [Browsable(false)]
        public TableLayoutRowStyleCollection RowStyles
        {
            get => _rowStyles;
        }

        /// <summary>
        /// Gets or sets the size.
        /// </summary>
        /// <value>The size.</value>
        public override Size Size
        {
            get => base.Size;
            set
            {
                base.Size = value;
                SetColumnsStyles();
                SetRowsStyles();
            }
        }

        /// <summary>
        /// Gets the cell position.
        /// </summary>
        /// <param name="control">The control.</param>
        /// <returns>TableLayoutPanelCellPosition.</returns>
        [DisplayName("Cell")]
        public TableLayoutPanelCellPosition GetCellPosition(Control control)
        {
            TableLayoutPanelCellPosition cellPosition = new TableLayoutPanelCellPosition(-1, -1);
            GridCell cell = control.GetGridCell(grid);
            if (cell != null)
            {
                cellPosition.Column = cell.LeftAttach;
                cellPosition.Row = cell.TopAttach;
            }

            return cellPosition;
        }

        /// <summary>
        /// Gets the column.
        /// </summary>
        /// <param name="control">The control.</param>
        /// <returns>System.Int32.</returns>
        [DefaultValue(-1)]
        [DisplayName("Column")]
        public int GetColumn(Control control)
        {
            return control.GetGridCell(grid).LeftAttach;
        }

        /// <summary>
        /// Gets the column span.
        /// </summary>
        /// <param name="control">The control.</param>
        /// <returns>System.Int32.</returns>
        [DefaultValue(1)]
        [DisplayName("ColumnSpan")]
        public int GetColumnSpan(Control control)
        {
            return control.Widget.MarginStart;
        }

        /// <summary>
        /// Gets the column widths.
        /// </summary>
        /// <returns>System.Int32[].</returns>
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public int[] GetColumnWidths()
        {
            List<int> list = new List<int>();
            if (RowCount > 0)
            {
                for (int c = 0; c < ColumnCount; c++)
                    list.Add(grid.GetChildAt(c, 0).WidthRequest);
            }

            return list.ToArray();
        }

        /// <summary>
        /// Gets the control from position.
        /// </summary>
        /// <param name="column">The column.</param>
        /// <param name="row">The row.</param>
        /// <returns>Control.</returns>
        public Control GetControlFromPosition(int column, int row)
        {
            return grid.GetChildAt(column, row).Data["Control"] as Control;
        }

        /// <summary>
        /// Gets the position from control.
        /// </summary>
        /// <param name="control">The control.</param>
        /// <returns>TableLayoutPanelCellPosition.</returns>
        public TableLayoutPanelCellPosition GetPositionFromControl(Control control)
        {
            TableLayoutPanelCellPosition cellPosition = new TableLayoutPanelCellPosition(-1, -1);
            GridCell cell = control.GetGridCell(grid);
            if (cell != null)
            {
                cellPosition.Column = cell.LeftAttach;
                cellPosition.Row = cell.TopAttach;
            }

            return cellPosition;
        }

        /// <summary>
        /// Gets the row.
        /// </summary>
        /// <param name="control">The control.</param>
        /// <returns>System.Int32.</returns>
        [DefaultValue(-1)]
        [DisplayName("Row")]
        public int GetRow(Control control)
        {
            return control.GetGridCell(grid).TopAttach;
        }

        /// <summary>
        /// Gets the row heights.
        /// </summary>
        /// <returns>System.Int32[].</returns>
        [Browsable(false)]
        public int[] GetRowHeights()
        {
            List<int> list = new List<int>();
            if (ColumnCount > 0)
            {
                for (int r = 0; r < RowCount; r++)
                    list.Add(grid.GetChildAt(0, r).WidthRequest);
            }

            return list.ToArray();
        }

        /// <summary>
        /// Gets the row span.
        /// </summary>
        /// <param name="control">The control.</param>
        /// <returns>System.Int32.</returns>
        [DefaultValue(1)]
        [DisplayName("RowSpan")]
        public int GetRowSpan(Control control)
        {
            return control.Widget.MarginTop;
        }

        /// <summary>
        /// Forces the control to perform layout logic.
        /// </summary>
        public override void PerformLayout()
        {
            SetColumnsStyles();
            SetRowsStyles();
        }

        /// <summary>
        /// Sets the cell position.
        /// </summary>
        /// <param name="control">The control.</param>
        /// <param name="position">The position.</param>
        public void SetCellPosition(Control control, TableLayoutPanelCellPosition position)
        {
            GridCell cell = control.GetGridCell(grid);
            if (cell != null)
            {
                cell.LeftAttach = position.Column;
                cell.TopAttach = position.Row;
            }
        }

        /// <summary>
        /// Sets the column.
        /// </summary>
        /// <param name="control">The control.</param>
        /// <param name="column">The column.</param>
        public void SetColumn(Control control, int column)
        {
            GridCell child = control.GetGridCell(grid);
            if (child != null)
                child.LeftAttach = column;

        }

        /// <summary>
        /// Sets the column span.
        /// </summary>
        /// <param name="control">The control.</param>
        /// <param name="value">The value.</param>
        public void SetColumnSpan(Control control, int value)
        {
            control.Widget.MarginStart = value;
            control.Widget.MarginEnd = value;
        }

        /// <summary>
        /// Sets the row.
        /// </summary>
        /// <param name="control">The control.</param>
        /// <param name="row">The row.</param>
        public void SetRow(Control control, int row)
        {
            control.GetGridCell(grid).TopAttach = row;
        }

        /// <summary>
        /// Sets the row span.
        /// </summary>
        /// <param name="control">The control.</param>
        /// <param name="value">The value.</param>
        public void SetRowSpan(Control control, int value)
        {
            control.Widget.MarginTop = value;
            control.Widget.MarginBottom = value;
        }

        /// <summary>
        /// Determines whether this instance can extend the specified object.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns><c>true</c> if this instance can extend the specified object; otherwise, <c>false</c>.</returns>
        /// <exception cref="System.NotImplementedException"></exception>
        bool IExtenderProvider.CanExtend(object obj)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Sets the columns styles.
        /// </summary>
        private void SetColumnsStyles()
        {
            Size size = this.Size;
            int cidx = 0;
            foreach (ColumnStyle cs in ColumnStyles)
            {
                if (cidx < ColumnCount)
                {
                    if (cs.SizeType == SizeType.Absolute)
                    {
                        for (int r = 0; r < RowCount; r++)
                            grid.GetChildAt(cidx, r).WidthRequest = Convert.ToInt32(cs.Width);
                    }
                    else if (cs.SizeType == SizeType.Percent)
                    {
                        for (int r = 0; r < RowCount; r++)
                        {
                            grid.GetChildAt(cidx, r).WidthRequest = Convert.ToInt32(size.Width * cs.Width * 0.01);
                        }
                    }
                    cidx++;
                }
            }
        }

        /// <summary>
        /// Sets the rows styles.
        /// </summary>
        private void SetRowsStyles()
        {
            Size size = this.Size;
            int ridx = 0;
            foreach (RowStyle rs in RowStyles)
            {
                if (ridx < RowCount)
                {
                    if (rs.SizeType == SizeType.Absolute)
                    {
                        Console.WriteLine(ColumnCount);
                        Console.WriteLine(Convert.ToInt32(rs.Height));
                        for (int c = 0; c < ColumnCount; c++)
                            grid.GetChildAt(c, ridx).HeightRequest = Convert.ToInt32(rs.Height);
                    }
                    else if (rs.SizeType == SizeType.Percent)
                    {
                        for (int c = 0; c < ColumnCount; c++)
                            grid.GetChildAt(c, ridx).HeightRequest = Convert.ToInt32(size.Height * rs.Height * 0.01);
                    }
                    ridx++;
                }
            }
        }

    }
}
