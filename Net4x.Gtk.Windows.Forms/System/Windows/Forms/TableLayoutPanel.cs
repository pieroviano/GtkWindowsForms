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
    /// <filterpriority>
    /// 1
    /// </filterpriority>
    //[ProvideProperty("ColumnSpan", typeof(Control))]
    //[ProvideProperty("RowSpan", typeof(Control))]
    //[ProvideProperty("Row", typeof(Control))]
    //[ProvideProperty("Column", typeof(Control))]
    //[ProvideProperty("CellPosition", typeof(Control))]
    //[DefaultProperty("ColumnCount")]
    [DesignerCategory("Component")]
    public partial class TableLayoutPanel : ContainerControl, IExtenderProvider
    {

        public readonly TableLayoutPanelBase self = new TableLayoutPanelBase();

        public override object GtkControl => self;

        private TableLayoutControlCollection _controls;

        private TableLayoutColumnStyleCollection _columnStyles;

        private TableLayoutRowStyleCollection _rowStyles;

        public Gtk.Grid grid => self.grid;
        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="System.Windows.Forms.TableLayoutPanel" /> class.
        /// </summary>
        public TableLayoutPanel() : base()
        {
            _controls = new TableLayoutControlCollection(this);
            _columnStyles = new TableLayoutColumnStyleCollection();
            _rowStyles = new TableLayoutRowStyleCollection();
        }

        public override void PerformLayout()
        {
            SetColumnsStyles();
            SetRowsStyles();
        }
        /// <summary>
        /// Gets or sets the border style for the panel.
        /// </summary>
        /// <returns>One of the
        /// <see cref="System.Windows.Forms.BorderStyle" /> values describing the style of the border of the panel. The default is
        /// <see cref="System.Windows.Forms.BorderStyle.None" />
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        [Browsable(false)]
        public override BorderStyle BorderStyle { get => base.BorderStyle; set => base.BorderStyle = value; }
        /// <summary>
        /// Gets or sets the style of the cell borders.
        /// </summary>
        /// <returns>One of the
        /// <see cref="System.Windows.Forms.TableLayoutPanelCellBorderStyle" /> values describing the style of all the cell borders in the table. The default is
        /// <see cref="System.Windows.Forms.TableLayoutPanelCellBorderStyle.None" />
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        [Localizable(true)]
        public TableLayoutPanelCellBorderStyle CellBorderStyle
        {
            get;
            set;
        }
        /// <summary>
        /// Gets the collection of controls contained within the
        /// <see cref="System.Windows.Forms.TableLayoutPanel" />
        /// </summary>
        /// <returns>A
        /// <see cref="System.Windows.Forms.TableLayoutControlCollection" /> containing the controls associated with the current
        /// <see cref="System.Windows.Forms.TableLayoutPanel" />
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        [Browsable(false)]
        public new TableLayoutControlCollection Controls
        {
            get => _controls;
        }

        private int _ColumnCount;
        /// <summary>
        /// Gets or sets the number of columns in the table.
        /// </summary>
        /// <returns>The number of columns in the
        /// <see cref="System.Windows.Forms.TableLayoutPanel" /> control. The default is 0.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
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
        /// Gets or sets a value indicating whether the
        /// <see cref="System.Windows.Forms.TableLayoutPanel" /> control should expand to accommodate new cells when all existing cells are occupied.
        /// </summary>
        /// <returns>A
        /// <see cref="System.Windows.Forms.TableLayoutPanelGrowStyle" /> indicating the growth scheme. The default is
        /// <see cref="System.Windows.Forms.TableLayoutPanelGrowStyle.AddRows" />
        /// </returns>
        /// <exception cref="System.ArgumentException">The property value is invalid for the
        /// <see cref="System.Windows.Forms.TableLayoutPanelGrowStyle" /> enumeration.
        /// </exception>
        public TableLayoutPanelGrowStyle GrowStyle
        {
            get;
            set;
        }

        private int _RowCount;
        /// <summary>
        /// Gets or sets the number of rows in the table.
        /// </summary>
        /// <returns>The number of rows in the
        /// <see cref="System.Windows.Forms.TableLayoutPanel" /> control. The default is 0.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
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
        /// Gets a collection of row styles for the
        /// <see cref="System.Windows.Forms.TableLayoutPanel" />
        /// </summary>
        /// <returns>A
        /// <see cref="System.Windows.Forms.TableLayoutRowStyleCollection" /> containing a
        /// <see cref="System.Windows.Forms.RowStyle" /> for each row in the
        /// <see cref="System.Windows.Forms.TableLayoutPanel" /> control.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        [DisplayName("Rows")]
        [Browsable(false)]
        public TableLayoutRowStyleCollection RowStyles
        {
            get => _rowStyles;
        }
        /// <summary>
        /// Gets a collection of column styles for the
        /// <see cref="System.Windows.Forms.TableLayoutPanel" />
        /// </summary>
        /// <returns>A
        /// <see cref="System.Windows.Forms.TableLayoutColumnStyleCollection" /> containing a
        /// <see cref="System.Windows.Forms.ColumnStyle" /> for each column in the
        /// <see cref="System.Windows.Forms.TableLayoutPanel" /> control.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        [DisplayName("Columns")]
        [Browsable(false)]
        public TableLayoutColumnStyleCollection ColumnStyles
        {
            get => _columnStyles;
        }
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
        /// <summary>
        /// For a description of this member, see
        /// <see cref="System.ComponentModel.IExtenderProvider.CanExtend(System.Object)" />
        /// </summary>
        /// <returns>true if this object can provide extender properties to the specified object; otherwise, false.
        /// </returns>
        /// <param name="obj">The
        /// <see cref="System.Object" /> to receive the extender properties.
        /// </param>
        bool IExtenderProvider.CanExtend(object obj)
        {
            throw new System.NotImplementedException();
        }
        /// <summary>
        /// Returns the number of columns spanned by the specified child control.
        /// </summary>
        /// <returns>The number of columns spanned by the child control. The default is 1.
        /// </returns>
        /// <param name="control">A child control of the
        /// <see cref="System.Windows.Forms.TableLayoutPanel" />
        /// </param>
        /// <filterpriority>1
        /// </filterpriority>
        [DefaultValue(1)]
        [DisplayName("ColumnSpan")]
        public int GetColumnSpan(Control control)
        {
            return control.Widget.MarginStart;
        }
        /// <summary>
        /// Sets the number of columns spanned by the child control.
        /// </summary>
        /// <param name="control">A child control of the
        /// <see cref="System.Windows.Forms.TableLayoutPanel" />
        /// </param>
        /// <param name="value">The number of columns to span.
        /// </param>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// <paramref name="value" /> is less than 1.
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        public void SetColumnSpan(Control control, int value)
        {
            control.Widget.MarginStart = value;
            control.Widget.MarginEnd = value;
        }
        /// <summary>
        /// Returns the number of rows spanned by the specified child control.
        /// </summary>
        /// <returns>The number of rows spanned by the child control. The default is 1.
        /// </returns>
        /// <param name="control">A child control of the
        /// <see cref="System.Windows.Forms.TableLayoutPanel" />
        /// </param>
        /// <filterpriority>1
        /// </filterpriority>
        [DefaultValue(1)]
        [DisplayName("RowSpan")]
        public int GetRowSpan(Control control)
        {
            return control.Widget.MarginTop;
        }
        /// <summary>
        /// Sets the number of rows spanned by the child control.
        /// </summary>
        /// <param name="control">A child control of the
        /// <see cref="System.Windows.Forms.TableLayoutPanel" />
        /// </param>
        /// <param name="value">The number of rows to span.
        /// </param>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// <paramref name="value" /> is less than 1.
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        public void SetRowSpan(Control control, int value)
        {
            control.Widget.MarginTop = value;
            control.Widget.MarginBottom = value;
        }
        /// <summary>
        /// Returns the row position of the specified child control.
        /// </summary>
        /// <returns>The row position of
        /// <paramref name="control" />, or -1 if the position of
        /// <paramref name="control" /> is determined by
        /// <see cref="System.Windows.Forms.TableLayoutPanel.LayoutEngine" />
        /// </returns>
        /// <param name="control">A child control of the
        /// <see cref="System.Windows.Forms.TableLayoutPanel" />
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="control" /> is null.
        /// </exception>
        /// <exception cref="System.NotSupportedException">
        /// <paramref name="control" /> is not a type that can be arranged by this
        /// <see cref="System.Windows.Forms.Layout.LayoutEngine" />
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        [DefaultValue(-1)]
        [DisplayName("Row")]
        public int GetRow(Control control)
        {
            return control.GetGridCell(grid).TopAttach;
        }
        /// <summary>
        /// Sets the row position of the specified child control.
        /// </summary>
        /// <param name="control">The control to move to another row.
        /// </param>
        /// <param name="row">The row to which
        /// <paramref name="control" /> will be moved.
        /// </param>
        /// <filterpriority>1
        /// </filterpriority>
        public void SetRow(Control control, int row)
        {
            control.GetGridCell(grid).TopAttach = row;
        }
        /// <summary>
        /// Gets the
        /// <see cref="System.Windows.Forms.TableLayoutPanelCellPosition" /> that represents the row and the column of the cell.
        /// </summary>
        /// <returns>A
        /// <see cref="System.Windows.Forms.TableLayoutPanelCellPosition" /> that represents the cell position.
        /// </returns>
        /// <param name="control">A control contained within a cell.
        /// </param>
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
        /// Sets the
        /// <see cref="System.Windows.Forms.TableLayoutPanelCellPosition" /> that represents the row and the column of the cell.
        /// </summary>
        /// <param name="control">A control contained within a cell.
        /// </param>
        /// <param name="position">A
        /// <see cref="System.Windows.Forms.TableLayoutPanelCellPosition" /> that represents the row and the column of the cell.
        /// </param>
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
        /// Returns the column position of the specified child control.
        /// </summary>
        /// <returns>The column position of the specified child control, or -1 if the position of
        /// <paramref name="control" /> is determined by
        /// <see cref="System.Windows.Forms.TableLayoutPanel.LayoutEngine" />
        /// </returns>
        /// <param name="control">A child control of the
        /// <see cref="System.Windows.Forms.TableLayoutPanel" />
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="control" /> is null.
        /// </exception>
        /// <exception cref="System.NotSupportedException">
        /// <paramref name="control" /> is not a type that can be arranged by this
        /// <see cref="System.Windows.Forms.Layout.LayoutEngine" />
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        [DefaultValue(-1)]
        [DisplayName("Column")]
        public int GetColumn(Control control)
        {
            return control.GetGridCell(grid).LeftAttach;
        }
        /// <summary>
        /// Sets the column position of the specified child control.
        /// </summary>
        /// <param name="control">The control to move to another column.
        /// </param>
        /// <param name="column">The column to which
        /// <paramref name="control" /> will be moved.
        /// </param>
        /// <filterpriority>1
        /// </filterpriority>
        public void SetColumn(Control control, int column)
        {
            GridCell child = control.GetGridCell(grid);
            if (child != null)
                child.LeftAttach = column;

        }
        /// <summary>
        /// Returns the child control occupying the specified position.
        /// </summary>
        /// <returns>The child control occupying the specified cell; otherwise, null if no control exists at the specified column and row, or if the control has its
        /// <see cref="System.Windows.Forms.Control.Visible" /> property set to false.
        /// </returns>
        /// <param name="column">The column position of the control to retrieve.
        /// </param>
        /// <param name="row">The row position of the control to retrieve.
        /// </param>
        /// <exception cref="System.ArgumentException">Either
        /// <paramref name="column" /> or
        /// <paramref name="row" /> (or both) is less than 0.
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        public Control GetControlFromPosition(int column, int row)
        {
            return grid.GetChildAt(column, row).Data["Control"] as Control;
        }
        /// <summary>
        /// Gets the
        /// <see cref="System.Windows.Forms.TableLayoutPanelCellPosition" /> that represents the row and the column of the cell that contains the control.
        /// </summary>
        /// <returns>A
        /// <see cref="System.Windows.Forms.TableLayoutPanelCellPosition" /> that represents the cell position.
        /// </returns>
        /// <param name="control">A control contained within a cell.
        /// </param>
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
        /// Returns an array representing the widths, in pixels, of the columns in the
        /// <see cref="System.Windows.Forms.TableLayoutPanel" />
        /// </summary>
        /// <returns>An array of type
        /// <see cref="System.Int32" /> that contains the widths, in pixels, of the columns in the
        /// <see cref="System.Windows.Forms.TableLayoutPanel" />
        /// </returns>
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
        /// Returns an array representing the heights, in pixels, of the rows in the
        /// <see cref="System.Windows.Forms.TableLayoutPanel" />
        /// </summary>
        /// <returns>An array of type
        /// <see cref="System.Int32" /> that contains the heights, in pixels, of the rows in the
        /// <see cref="System.Windows.Forms.TableLayoutPanel" />
        /// </returns>
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
    }
}
