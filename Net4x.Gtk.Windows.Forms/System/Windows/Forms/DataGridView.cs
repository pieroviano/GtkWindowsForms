/*
 * Based on GTK components, it is a cross-platform interface component that is compatible with the native C# control winform interface.
 * Use this component GtkSystem.Windows.Forms instead of Microsoft.WindowsDesktop.App.WindowsForms, compile once, and run across Windows, Linux, and macOS
 * Technical support 438865652@qq.com，https://www.gtkapp.com, https://gitee.com/easywebfactory, https://github.com/easywebfactory438865652@qq.com，https://www.gtkapp.com, https://gitee.com/easywebfactory, https://github.com/easywebfactory
 * author:chenhongjin
 */
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Reflection;
using GLib;
using Gtk;
using GtkSystem.Windows.Forms.GtkControls.ControlBase;

namespace System.Windows.Forms;

/// <summary>
/// Displays data in a customizable grid control.
/// Provides a lightweight GTK-backed implementation compatible with WinForms-style DataGridView APIs.
/// </summary>
/// <filterpriority>
/// 1
/// </filterpriority>
[DesignerCategory("Component")]
public partial class DataGridView : ScrollableControl
{
    /// <summary>
    /// The self
    /// </summary>
    public readonly DataGridViewBase self = new();

    internal Gtk.TreeStore Store = new(typeof(DataGridViewCell));

    private readonly ControlBindingsCollection _collect;

    private readonly DataGridViewColumnCollection _columns;

    private object? _DataSource;

    private bool _MultiSelect = true;

    private readonly DataGridViewRowCollection _rows;

    private DataGridViewRow? _RowTemplate;

    private readonly List<int> _selectedBandIndexes = new();

    private DataGridViewSelectionMode _SelectionMode = DataGridViewSelectionMode.RowHeaderSelect;

    /// <summary>
    /// Not implemented in this GTK-backed control. Placeholder for compatibility.
    /// </summary>
    [Obsolete(" this event is not implemented， develop independently")]
    public virtual event EventHandler? AllowUserToAddRowsChanged;

    /// <summary>
    /// Not implemented in this GTK-backed control. Placeholder for compatibility.
    /// </summary>
    [Obsolete(" this event is not implemented， develop independently")]
    public virtual event EventHandler? AllowUserToDeleteRowsChanged;

    /// <summary>
    /// Not implemented in this GTK-backed control. Placeholder for compatibility.
    /// </summary>
    //[Obsolete(" this event is not implemented， develop independently")]
    //public virtual event EventHandler SelectionChanged;
    [Obsolete(" this event is not implemented， develop independently")]
    public virtual event EventHandler? AllowUserToOrderColumnsChanged;

    /// <summary>
    /// Not implemented in this GTK-backed control. Placeholder for compatibility.
    /// </summary>
    [Obsolete(" this event is not implemented， develop independently")]
    public virtual event EventHandler? AllowUserToResizeColumnsChanged;

    /// <summary>
    /// Not implemented in this GTK-backed control. Placeholder for compatibility.
    /// </summary>
    [Obsolete(" this event is not implemented， develop independently")]
    public virtual event EventHandler? AllowUserToResizeRowsChanged;

    /// <summary>
    /// Not implemented - placeholder for alternating rows default cell style change notifications.
    /// </summary>
    [Obsolete(" this event is not implemented， develop independently")]
    public virtual event EventHandler? AlternatingRowsDefaultCellStyleChanged;

    /// <summary>
    /// Raised when AutoGenerateColumns setting changes - not implemented here.
    /// </summary>
    [Obsolete(" this event is not implemented， develop independently")]
    public virtual event EventHandler? AutoGenerateColumnsChanged;

    /// <summary>
    /// Raised when the AutoSizeColumnMode changes - not implemented here.
    /// </summary>
    [Obsolete(" this event is not implemented， develop independently")]
    public virtual event DataGridViewAutoSizeColumnModeEventHandler? AutoSizeColumnModeChanged;

    /// <summary>
    /// Raised when the AutoSizeColumnsMode changes - not implemented here.
    /// </summary>
    [Obsolete(" this event is not implemented， develop independently")]
    public virtual event DataGridViewAutoSizeColumnsModeEventHandler? AutoSizeColumnsModeChanged;

    /// <summary>
    /// Raised when AutoSizeRowsMode changes - not implemented here.
    /// </summary>
    //[Obsolete(" this event is not implemented， develop independently")]
    //public virtual event EventHandler BackColorChanged;
    [Obsolete(" this event is not implemented， develop independently")]
    public virtual event DataGridViewAutoSizeModeEventHandler? AutoSizeRowsModeChanged;

    /// <summary>
    /// Raised when the background color changes - not implemented here.
    /// </summary>
    //[Obsolete(" this event is not implemented， develop independently")]
    //public virtual event EventHandler BackgroundImageChanged;
    [Obsolete(" this event is not implemented， develop independently")]
    public virtual event EventHandler? BackgroundColorChanged;

    /// <summary>
    /// Raised when the border style changes - not implemented here.
    /// </summary>
    [Obsolete(" this event is not implemented， develop independently")]
    public virtual event EventHandler? BorderStyleChanged;

    /// <summary>
    /// Raised to cancel a row edit - not implemented here.
    /// </summary>
    [Obsolete(" this event is not implemented， develop independently")]
    public virtual event QuestionEventHandler? CancelRowEdit;

    /// <summary>
    /// Raised when a cell begins edit. Placeholder for compatibility.
    /// </summary>
    //[Obsolete(" this event is not implemented， develop independently")]
    //public virtual event EventHandler PaddingChanged;
    [Obsolete(" this event is not implemented， develop independently")]
    public virtual event DataGridViewCellCancelEventHandler? CellBeginEdit;

    /// <summary>
    /// Raised when the cell border style changes - not implemented here.
    /// </summary>
    //[Obsolete(" this event is not implemented， develop independently")]
    //public virtual event EventHandler StyleChanged;
    //[Obsolete(" this event is not implemented， develop independently")]
    //public virtual event EventHandler BackgroundImageLayoutChanged;
    [Obsolete(" this event is not implemented， develop independently")]
    public virtual event EventHandler? CellBorderStyleChanged;

    /// <summary>
    /// Raised when a cell is clicked.
    /// </summary>
    public virtual event DataGridViewCellEventHandler? CellClick;

    /// <summary>
    /// Raised when a cell content is clicked - placeholder for compatibility.
    /// </summary>
    //[Obsolete(" this event is not implemented， develop independently")]
    //public virtual event DataGridViewCellEventHandler CellClick;
    [Obsolete(" this event is not implemented， develop independently")]
    public virtual event DataGridViewCellEventHandler? CellContentClick;

    /// <summary>
    /// Raised when a cell content is double-clicked - placeholder for compatibility.
    /// </summary>
    [Obsolete(" this event is not implemented， develop independently")]
    public virtual event DataGridViewCellEventHandler? CellContentDoubleClick;

    /// <summary>
    /// Raised when a cell's context menu strip is changed - placeholder for compatibility.
    /// </summary>
    [Obsolete(" this event is not implemented， develop independently")]
    public virtual event DataGridViewCellEventHandler? CellContextMenuStripChanged;

    /// <summary>
    /// Raised when a context menu strip is needed for a cell - placeholder for compatibility.
    /// </summary>
    [Obsolete(" this event is not implemented， develop independently")]
    public virtual event DataGridViewCellContextMenuStripNeededEventHandler? CellContextMenuStripNeeded;

    /// <summary>
    /// Raised when a cell is double-clicked - placeholder for compatibility.
    /// </summary>
    [Obsolete(" this event is not implemented， develop independently")]
    public virtual event DataGridViewCellEventHandler? CellDoubleClick;

    /// <summary>
    /// Raised when a cell edit ends - placeholder for compatibility.
    /// </summary>
    [Obsolete(" this event is not implemented， develop independently")]
    public virtual event DataGridViewCellEventHandler? CellEndEdit;

    /// <summary>
    /// Raised when a cell receives focus - placeholder for compatibility.
    /// </summary>
    [Obsolete(" this event is not implemented， develop independently")]
    public virtual event DataGridViewCellEventHandler? CellEnter;

    /// <summary>
    /// Raised when a cell's error text is changed - placeholder for compatibility.
    /// </summary>
    [Obsolete(" this event is not implemented， develop independently")]
    public virtual event DataGridViewCellEventHandler? CellErrorTextChanged;

    /// <summary>
    /// Raised when a cell's error text is needed - placeholder for compatibility.
    /// </summary>
    [Obsolete(" this event is not implemented， develop independently")]
    public virtual event DataGridViewCellErrorTextNeededEventHandler? CellErrorTextNeeded;

    /// <summary>
    /// Raised when a cell requires formatting - placeholder for compatibility.
    /// </summary>
    [Obsolete(" this event is not implemented， develop independently")]
    public virtual event DataGridViewCellFormattingEventHandler? CellFormatting;

    /// <summary>
    /// Raised when a cell loses focus - placeholder for compatibility.
    /// </summary>
    [Obsolete(" this event is not implemented， develop independently")]
    public virtual event DataGridViewCellEventHandler? CellLeave;

    /// <summary>
    /// Raised for cell mouse click events - placeholder for compatibility.
    /// </summary>
    [Obsolete(" this event is not implemented， develop independently")]
    public virtual event DataGridViewCellMouseEventHandler? CellMouseClick;

    /// <summary>
    /// Raised for cell mouse double-click events - placeholder for compatibility.
    /// </summary>
    [Obsolete(" this event is not implemented， develop independently")]
    public virtual event DataGridViewCellMouseEventHandler? CellMouseDoubleClick;

    /// <summary>
    /// Raised for cell mouse down events - placeholder for compatibility.
    /// </summary>
    [Obsolete(" this event is not implemented， develop independently")]
    public virtual event DataGridViewCellMouseEventHandler? CellMouseDown;

    /// <summary>
    /// Raised when mouse enters a cell - placeholder for compatibility.
    /// </summary>
    [Obsolete(" this event is not implemented， develop independently")]
    public virtual event DataGridViewCellEventHandler? CellMouseEnter;

    /// <summary>
    /// Raised when mouse leaves a cell - placeholder for compatibility.
    /// </summary>
    [Obsolete(" this event is not implemented， develop independently")]
    public virtual event DataGridViewCellEventHandler? CellMouseLeave;

    /// <summary>
    /// Raised for cell mouse move events - placeholder for compatibility.
    /// </summary>
    [Obsolete(" this event is not implemented， develop independently")]
    public virtual event DataGridViewCellMouseEventHandler? CellMouseMove;

    /// <summary>
    /// Raised for cell mouse up events - placeholder for compatibility.
    /// </summary>
    [Obsolete(" this event is not implemented， develop independently")]
    public virtual event DataGridViewCellMouseEventHandler? CellMouseUp;

    /// <summary>
    /// Raised when a cell needs custom painting. Use this to customize cell rendering.
    /// </summary>
    public virtual event DataGridViewCellPaintingEventHandler? CellPainting;

    /// <summary>
    /// Raised when a cell parsing event occurs - placeholder for compatibility.
    /// </summary>
    [Obsolete(" this event is not implemented， develop independently")]
    public virtual event DataGridViewCellParsingEventHandler? CellParsing;

    /// <summary>
    /// Raised when cell state changes - placeholder for compatibility.
    /// </summary>
    [Obsolete(" this event is not implemented， develop independently")]
    public virtual event DataGridViewCellStateChangedEventHandler? CellStateChanged;

    /// <summary>
    /// Raised when a cell's style changes - placeholder for compatibility.
    /// </summary>
    [Obsolete(" this event is not implemented， develop independently")]
    public virtual event DataGridViewCellEventHandler? CellStyleChanged;

    /// <summary>
    /// Raised when the cell style content changes - placeholder for compatibility.
    /// </summary>
    [Obsolete(" this event is not implemented， develop independently")]
    public virtual event DataGridViewCellStyleContentChangedEventHandler? CellStyleContentChanged;

    /// <summary>
    /// Raised when cell tooltip text changes - placeholder for compatibility.
    /// </summary>
    [Obsolete(" this event is not implemented， develop independently")]
    public virtual event DataGridViewCellEventHandler? CellToolTipTextChanged;

    /// <summary>
    /// Raised when a cell tooltip is needed - placeholder for compatibility.
    /// </summary>
    [Obsolete(" this event is not implemented， develop independently")]
    public virtual event DataGridViewCellToolTipTextNeededEventHandler? CellToolTipTextNeeded;

    /// <summary>
    /// Raised when a cell is validated - placeholder for compatibility.
    /// </summary>
    [Obsolete(" this event is not implemented， develop independently")]
    public virtual event DataGridViewCellEventHandler? CellValidated;

    /// <summary>
    /// Raised when a cell is validating - placeholder for compatibility.
    /// </summary>
    //[Obsolete(" this event is not implemented， develop independently")]
    //public virtual event DataGridViewCellEventHandler CellValueChanged;
    [Obsolete(" this event is not implemented， develop independently")]
    public virtual event DataGridViewCellValidatingEventHandler? CellValidating;

    /// <summary>
    /// Raised when a cell value changes.
    /// </summary>
    public virtual event DataGridViewCellEventHandler? CellValueChanged;

    /// <summary>
    /// Raised when a cell value is required - placeholder for virtual-mode support.
    /// </summary>
    [Obsolete(" this event is not implemented， develop independently")]
    public virtual event DataGridViewCellValueEventHandler? CellValueNeeded;

    /// <summary>
    /// Raised when a cell value is pushed back to the data cache - placeholder for virtual-mode support.
    /// </summary>
    [Obsolete(" this event is not implemented， develop independently")]
    public virtual event DataGridViewCellValueEventHandler? CellValuePushed;

    /// <summary>
    /// Raised when a column is added - placeholder for compatibility.
    /// </summary>
    [Obsolete(" this event is not implemented， develop independently")]
    public virtual event DataGridViewColumnEventHandler? ColumnAdded;

    /// <summary>
    /// Raised when a column's context menu strip changes - placeholder for compatibility.
    /// </summary>
    [Obsolete(" this event is not implemented， develop independently")]
    public virtual event DataGridViewColumnEventHandler? ColumnContextMenuStripChanged;

    /// <summary>
    /// Raised when a column's data property name changes - placeholder for compatibility.
    /// </summary>
    [Obsolete(" this event is not implemented， develop independently")]
    public virtual event DataGridViewColumnEventHandler? ColumnDataPropertyNameChanged;

    /// <summary>
    /// Raised when a column's default cell style changes - placeholder for compatibility.
    /// </summary>
    [Obsolete(" this event is not implemented， develop independently")]
    public virtual event DataGridViewColumnEventHandler? ColumnDefaultCellStyleChanged;

    /// <summary>
    /// Raised when a column's display index changes - placeholder for compatibility.
    /// </summary>
    [Obsolete(" this event is not implemented， develop independently")]
    public virtual event DataGridViewColumnEventHandler? ColumnDisplayIndexChanged;

    /// <summary>
    /// Raised when a column divider is double-clicked - placeholder for compatibility.
    /// </summary>
    [Obsolete(" this event is not implemented， develop independently")]
    public virtual event DataGridViewColumnDividerDoubleClickEventHandler? ColumnDividerDoubleClick;

    /// <summary>
    /// Raised when a column divider width changes - placeholder for compatibility.
    /// </summary>
    [Obsolete(" this event is not implemented， develop independently")]
    public virtual event DataGridViewColumnEventHandler? ColumnDividerWidthChanged;

    /// <summary>
    /// Raised when a column header cell changes - placeholder for compatibility.
    /// </summary>
    [Obsolete(" this event is not implemented， develop independently")]
    public virtual event DataGridViewColumnEventHandler? ColumnHeaderCellChanged;

    /// <summary>
    /// Raised for column header mouse click events - placeholder for compatibility.
    /// </summary>
    [Obsolete(" this event is not implemented， develop independently")]
    public virtual event DataGridViewCellMouseEventHandler? ColumnHeaderMouseClick;

    /// <summary>
    /// Raised for column header mouse double-click events - placeholder for compatibility.
    /// </summary>
    [Obsolete(" this event is not implemented， develop independently")]
    public virtual event DataGridViewCellMouseEventHandler? ColumnHeaderMouseDoubleClick;

    /// <summary>
    /// Raised when column headers border style changes - placeholder for compatibility.
    /// </summary>
    [Obsolete(" this event is not implemented， develop independently")]
    public virtual event EventHandler? ColumnHeadersBorderStyleChanged;

    /// <summary>
    /// Raised when column headers default cell style changes - placeholder for compatibility.
    /// </summary>
    [Obsolete(" this event is not implemented， develop independently")]
    public virtual event EventHandler? ColumnHeadersDefaultCellStyleChanged;

    /// <summary>
    /// Raised when column headers height changes - placeholder for compatibility.
    /// </summary>
    [Obsolete(" this event is not implemented， develop independently")]
    public virtual event EventHandler? ColumnHeadersHeightChanged;

    /// <summary>
    /// Raised when the column headers height size mode changes - placeholder for compatibility.
    /// </summary>
    [Obsolete(" this event is not implemented， develop independently")]
    public virtual event DataGridViewAutoSizeModeEventHandler? ColumnHeadersHeightSizeModeChanged;

    /// <summary>
    /// Raised when a column's minimum width changes - placeholder for compatibility.
    /// </summary>
    [Obsolete(" this event is not implemented， develop independently")]
    public virtual event DataGridViewColumnEventHandler? ColumnMinimumWidthChanged;

    /// <summary>
    /// Raised when a column's name changes - placeholder for compatibility.
    /// </summary>
    [Obsolete(" this event is not implemented， develop independently")]
    public virtual event DataGridViewColumnEventHandler? ColumnNameChanged;

    /// <summary>
    /// Raised when a column is removed - placeholder for compatibility.
    /// </summary>
    [Obsolete(" this event is not implemented， develop independently")]
    public virtual event DataGridViewColumnEventHandler? ColumnRemoved;

    /// <summary>
    /// Raised when a column's sort mode changes - placeholder for compatibility.
    /// </summary>
    [Obsolete(" this event is not implemented， develop independently")]
    public virtual event DataGridViewColumnEventHandler? ColumnSortModeChanged;

    /// <summary>
    /// Raised when a column state changes - placeholder for compatibility.
    /// </summary>
    [Obsolete(" this event is not implemented， develop independently")]
    public virtual event DataGridViewColumnStateChangedEventHandler? ColumnStateChanged;

    /// <summary>
    /// Raised when a column's tooltip text changes - placeholder for compatibility.
    /// </summary>
    [Obsolete(" this event is not implemented， develop independently")]
    public virtual event DataGridViewColumnEventHandler? ColumnToolTipTextChanged;

    /// <summary>
    /// Raised when a column width changes - placeholder for compatibility.
    /// </summary>
    [Obsolete(" this event is not implemented， develop independently")]
    public virtual event DataGridViewColumnEventHandler? ColumnWidthChanged;

    /// <summary>
    /// Raised when the current cell changes - placeholder for compatibility.
    /// </summary>
    [Obsolete(" this event is not implemented， develop independently")]
    public virtual event EventHandler? CurrentCellChanged;

    /// <summary>
    /// Raised when the current cell dirty state changes - placeholder for compatibility.
    /// </summary>
    [Obsolete(" this event is not implemented， develop independently")]
    public virtual event EventHandler? CurrentCellDirtyStateChanged;

    /// <summary>
    /// Raised when data binding completes - placeholder for compatibility.
    /// </summary>
    [Obsolete(" this event is not implemented， develop independently")]
    public virtual event DataGridViewBindingCompleteEventHandler? DataBindingComplete;

    /// <summary>
    /// Raised when a data error occurs - placeholder for compatibility.
    /// </summary>
    [Obsolete(" this event is not implemented， develop independently")]
    public virtual event DataGridViewDataErrorEventHandler? DataError;

    /// <summary>
    /// Raised when DataMember changes - placeholder for compatibility.
    /// </summary>
    [Obsolete(" this event is not implemented， develop independently")]
    public virtual event EventHandler? DataMemberChanged;

    /// <summary>
    /// Raised when DataSource changes - placeholder for compatibility.
    /// </summary>
    [Obsolete(" this event is not implemented， develop independently")]
    public virtual event EventHandler? DataSourceChanged;

    /// <summary>
    /// Raised when default cell style changes - placeholder for compatibility.
    /// </summary>
    [Obsolete(" this event is not implemented， develop independently")]
    public virtual event EventHandler? DefaultCellStyleChanged;

    /// <summary>
    /// Raised when default values are needed for new rows - placeholder for compatibility.
    /// </summary>
    [Obsolete(" this event is not implemented， develop independently")]
    public virtual event DataGridViewRowEventHandler? DefaultValuesNeeded;

    /// <summary>
    /// Raised when an editing control is shown - placeholder for compatibility.
    /// </summary>
    [Obsolete(" this event is not implemented， develop independently")]
    public virtual event DataGridViewEditingControlShowingEventHandler? EditingControlShowing;

    /// <summary>
    /// Raised when edit mode changes - placeholder for compatibility.
    /// </summary>
    //[Obsolete(" this event is not implemented， develop independently")]
    //public virtual event EventHandler FontChanged;
    //[Obsolete(" this event is not implemented， develop independently")]
    //public virtual event EventHandler ForeColorChanged;
    [Obsolete(" this event is not implemented， develop independently")]
    public virtual event EventHandler? EditModeChanged;

    /// <summary>
    /// Raised when grid color changes - placeholder for compatibility.
    /// </summary>
    [Obsolete(" this event is not implemented， develop independently")]
    public virtual event EventHandler? GridColorChanged;

    /// <summary>
    /// Raised when MultiSelect changes - placeholder for compatibility.
    /// </summary>
    [Obsolete(" this event is not implemented， this event is not implemented， develop independently")]
    public virtual event EventHandler? MultiSelectChanged;

    /// <summary>
    /// Raised when new rows are required - placeholder for compatibility.
    /// </summary>
    [Obsolete(" this event is not implemented， develop independently")]
    public virtual event DataGridViewRowEventHandler? NewRowNeeded;

    /// <summary>
    /// Raised when ReadOnly mode changes - placeholder for compatibility.
    /// </summary>
    [Obsolete(" this event is not implemented， develop independently")]
    public virtual event EventHandler? ReadOnlyChanged;

    /// <summary>
    /// Raised when row context menu strip changes - placeholder for compatibility.
    /// </summary>
    [Obsolete(" this event is not implemented， develop independently")]
    public virtual event DataGridViewRowEventHandler? RowContextMenuStripChanged;

    /// <summary>
    /// Raised when a row context menu strip is needed - placeholder for compatibility.
    /// </summary>
    [Obsolete(" this event is not implemented， develop independently")]
    public virtual event DataGridViewRowContextMenuStripNeededEventHandler? RowContextMenuStripNeeded;

    /// <summary>
    /// Raised when a row's default style changes - placeholder for compatibility.
    /// </summary>
    [Obsolete(" this event is not implemented， develop independently")]
    public virtual event DataGridViewRowEventHandler? RowDefaultCellStyleChanged;

    /// <summary>
    /// Raised when a row's dirty state is needed - placeholder for compatibility.
    /// </summary>
    [Obsolete(" this event is not implemented， develop independently")]
    public virtual event QuestionEventHandler? RowDirtyStateNeeded;

    /// <summary>
    /// Raised when a row divider is double-clicked - placeholder for compatibility.
    /// </summary>
    [Obsolete(" this event is not implemented， develop independently")]
    public virtual event DataGridViewRowDividerDoubleClickEventHandler? RowDividerDoubleClick;

    /// <summary>
    /// Raised when a row divider height changes - placeholder for compatibility.
    /// </summary>
    [Obsolete(" this event is not implemented， develop independently")]
    public virtual event DataGridViewRowEventHandler? RowDividerHeightChanged;

    /// <summary>
    /// Raised when entering a row - placeholder for compatibility.
    /// </summary>
    [Obsolete(" this event is not implemented， develop independently")]
    public virtual event DataGridViewCellEventHandler? RowEnter;

    /// <summary>
    /// Raised when a row error text changes - placeholder for compatibility.
    /// </summary>
    [Obsolete(" this event is not implemented， develop independently")]
    public virtual event DataGridViewRowEventHandler? RowErrorTextChanged;

    /// <summary>
    /// Raised when a row's error text is needed - placeholder for compatibility.
    /// </summary>
    [Obsolete(" this event is not implemented， develop independently")]
    public virtual event DataGridViewRowErrorTextNeededEventHandler? RowErrorTextNeeded;

    /// <summary>
    /// Raised when the row header cell changes - placeholder for compatibility.
    /// </summary>
    [Obsolete(" this event is not implemented， develop independently")]
    public virtual event DataGridViewRowEventHandler? RowHeaderCellChanged;

    /// <summary>
    /// Raised for row header mouse click events - placeholder for compatibility.
    /// </summary>
    [Obsolete(" this event is not implemented， develop independently")]
    public virtual event DataGridViewCellMouseEventHandler? RowHeaderMouseClick;

    /// <summary>
    /// Raised for row header mouse double-click events - placeholder for compatibility.
    /// </summary>
    [Obsolete(" this event is not implemented， develop independently")]
    public virtual event DataGridViewCellMouseEventHandler? RowHeaderMouseDoubleClick;

    /// <summary>
    /// Raised when row headers border style changes - placeholder for compatibility.
    /// </summary>
    [Obsolete(" this event is not implemented， develop independently")]
    public virtual event EventHandler? RowHeadersBorderStyleChanged;

    /// <summary>
    /// Raised when row headers default cell style changes - placeholder for compatibility.
    /// </summary>
    [Obsolete(" this event is not implemented， develop independently")]
    public virtual event EventHandler? RowHeadersDefaultCellStyleChanged;

    /// <summary>
    /// Raised when row headers width changes - placeholder for compatibility.
    /// </summary>
    [Obsolete(" this event is not implemented， develop independently")]
    public virtual event EventHandler? RowHeadersWidthChanged;

    /// <summary>
    /// Raised when row headers width size mode changes - placeholder for compatibility.
    /// </summary>
    [Obsolete(" this event is not implemented， develop independently")]
    public virtual event DataGridViewAutoSizeModeEventHandler? RowHeadersWidthSizeModeChanged;

    /// <summary>
    /// Raised when row height changes - placeholder for compatibility.
    /// </summary>
    [Obsolete(" this event is not implemented， develop independently")]
    public virtual event DataGridViewRowEventHandler? RowHeightChanged;

    /// <summary>
    /// Raised when row height info is needed - placeholder for compatibility.
    /// </summary>
    [Obsolete(" this event is not implemented， develop independently")]
    public virtual event DataGridViewRowHeightInfoNeededEventHandler? RowHeightInfoNeeded;

    /// <summary>
    /// Raised when row height info is pushed - placeholder for compatibility.
    /// </summary>
    [Obsolete(" this event is not implemented， develop independently")]
    public virtual event DataGridViewRowHeightInfoPushedEventHandler? RowHeightInfoPushed;

    /// <summary>
    /// Raised when a row loses focus - placeholder for compatibility.
    /// </summary>
    [Obsolete(" this event is not implemented， develop independently")]
    public virtual event DataGridViewCellEventHandler? RowLeave;

    /// <summary>
    /// Raised when the minimum height of a row changes - placeholder for compatibility.
    /// </summary>
    [Obsolete(" this event is not implemented， develop independently")]
    public virtual event DataGridViewRowEventHandler? RowMinimumHeightChanged;

    /// <summary>
    /// Raised after painting a row - placeholder for compatibility.
    /// </summary>
    [Obsolete(" this event is not implemented， develop independently")]
    public virtual event DataGridViewRowPostPaintEventHandler? RowPostPaint;

    /// <summary>
    /// Raised before painting a row - placeholder for compatibility.
    /// </summary>
    [Obsolete(" this event is not implemented， develop independently")]
    public virtual event DataGridViewRowPrePaintEventHandler? RowPrePaint;

    /// <summary>
    /// Raised when rows are added.
    /// </summary>
    [Obsolete(" this event is not implemented， develop independently")]
    public virtual event DataGridViewRowsAddedEventHandler? RowsAdded;

    /// <summary>
    /// Raised when rows default cell style changes - placeholder for compatibility.
    /// </summary>
    //[Obsolete(" this event is not implemented， develop independently")]
    //public virtual event EventHandler TextChanged;
    [Obsolete(" this event is not implemented， develop independently")]
    public virtual event EventHandler? RowsDefaultCellStyleChanged;

    /// <summary>
    /// Raised when rows are removed.
    /// </summary>
    [Obsolete(" this event is not implemented， develop independently")]
    public virtual event DataGridViewRowsRemovedEventHandler? RowsRemoved;

    /// <summary>
    /// Raised when the row state changes - placeholder for compatibility.
    /// </summary>
    [Obsolete(" this event is not implemented， develop independently")]
    public virtual event DataGridViewRowStateChangedEventHandler? RowStateChanged;

    /// <summary>
    /// Raised when a row is unshared - placeholder for compatibility.
    /// </summary>
    [Obsolete(" this event is not implemented， develop independently")]
    public virtual event DataGridViewRowEventHandler? RowUnshared;

    /// <summary>
    /// Raised when a row is validated - placeholder for compatibility.
    /// </summary>
    [Obsolete(" this event is not implemented， develop independently")]
    public virtual event DataGridViewCellEventHandler? RowValidated;

    /// <summary>
    /// Raised when a row is validating - placeholder for compatibility.
    /// </summary>
    [Obsolete(" this event is not implemented， develop independently")]
    public virtual event DataGridViewCellCancelEventHandler? RowValidating;

    /// <summary>
    /// Raised when selection changes.
    /// </summary>
    public virtual event EventHandler? SelectionChanged;

    /// <summary>
    /// Raised when sort comparison is requested - placeholder for compatibility.
    /// </summary>
    [Obsolete(" this event is not implemented， develop independently")]
    public virtual event DataGridViewSortCompareEventHandler? SortCompare;

    /// <summary>
    /// Raised when the grid is sorted - placeholder for compatibility.
    /// </summary>
    [Obsolete(" this event is not implemented， develop independently")]
    public virtual event EventHandler? Sorted;

    /// <summary>
    /// Raised when the user adds a row - placeholder for compatibility.
    /// </summary>
    [Obsolete(" this event is not implemented， develop independently")]
    public virtual event DataGridViewRowEventHandler? UserAddedRow;

    /// <summary>
    /// Raised when the user deletes a row - placeholder for compatibility.
    /// </summary>
    [Obsolete(" this event is not implemented， develop independently")]
    public virtual event DataGridViewRowEventHandler? UserDeletedRow;

    /// <summary>
    /// Raised when a user is deleting a row - placeholder for compatibility.
    /// </summary>
    [Obsolete(" this event is not implemented， develop independently")]
    public virtual event DataGridViewRowCancelEventHandler? UserDeletingRow;

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.DataGridView" /> class.
    /// Configures the underlying GTK TreeView and prepares columns/rows collections.
    /// </summary>
    public DataGridView() : base()
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

    /// <summary>
    /// Gets or sets the default cell style applied to alternating rows.
    /// </summary>
    public DataGridViewCellStyle? AlternatingRowsDefaultCellStyle
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the mode in which row heights are automatically determined.
    /// </summary>
    public DataGridViewAutoSizeRowsMode AutoSizeRowsMode
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the style applied to column headers.
    /// </summary>
    public DataGridViewCellStyle? ColumnHeadersDefaultCellStyle
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the height, in pixels, of the column headers.
    /// </summary>
    public int ColumnHeadersHeight
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the sizing mode for column header height.
    /// </summary>
    public DataGridViewColumnHeadersHeightSizeMode ColumnHeadersHeightSizeMode
    {
        get;
        set;
    }

    /// <summary>
    /// Gets the collection of columns displayed by the grid.
    /// </summary>
    public DataGridViewColumnCollection Columns
    {
        get
        {
            return _columns;
        }
    }

    /// <summary>
    /// Gets the collection of data bindings associated with the grid.
    /// </summary>
    public override ControlBindingsCollection DataBindings { get => _collect; }

    /// <summary>
    /// Gets or sets the name of the list or table in the data source.
    /// </summary>
    public string? DataMember
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the data source for the grid. Accepts DataTable, DataSet, DataView, or IEnumerable.
    /// When set and the control is visible, data is loaded into the grid.
    /// </summary>
    public object? DataSource
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

    /// <summary>
    /// Gets or sets the default cell style for the grid.
    /// </summary>
    public DataGridViewCellStyle? DefaultCellStyle
    {
        get;
        set;
    }

    /// <summary>
    /// Gets the underlying GTK TreeView used for rendering.
    /// </summary>
    public Gtk.TreeView GridView { get { return self.GridView; } }

    /// <summary>
    /// Gets the GTK-backed control object for interop.
    /// </summary>
    public override object GtkControl => self;

    /// <summary>
    /// Gets or sets a placeholder markup string used by the control.
    /// </summary>
    public string Markup
    {
        get;
        set;
    } = "...";

    /// <summary>
    /// Gets or sets a value indicating whether the grid allows multiple selection.
    /// Setting this will adjust the GTK selection mode accordingly.
    /// </summary>
    public bool MultiSelect
    {
        get => _MultiSelect;
        set
        {
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

    /// <summary>
    /// Gets or sets a value indicating whether the grid is read-only.
    /// </summary>
    public bool ReadOnly
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the default style for row headers.
    /// </summary>
    public DataGridViewCellStyle? RowHeadersDefaultCellStyle
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the width, in pixels, of the row header column.
    /// </summary>
    public int RowHeadersWidth
    {
        get;
        set;
    }

    /// <summary>
    /// Gets the collection of rows in the grid.
    /// </summary>
    public DataGridViewRowCollection Rows { get { return _rows; } }

    /// <summary>
    /// Gets or sets the default style applied to all rows.
    /// </summary>
    public DataGridViewCellStyle? RowsDefaultCellStyle
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the template used when creating new rows.
    /// </summary>
    public DataGridViewRow RowTemplate
    {
        get { return _RowTemplate ??= new DataGridViewRow(); }
        set { _RowTemplate = value; }
    }

    /// <summary>
    /// Gets the collection of currently selected cells.
    /// The contents depend on the current SelectionMode.
    /// </summary>
    [Browsable(false)]
    public DataGridViewSelectedCellCollection SelectedCells
    {
        get
        {
            var stcc = new DataGridViewSelectedCellCollection();
            switch (SelectionMode)
            {
                case DataGridViewSelectionMode.CellSelect:
                    {
                        var cols = Store.NColumns;
                        Store.Foreach(new TreeModelForeachFunc((model, path, iter) =>
                        {
                            for (var i = 0; i < cols; i++)
                            {
                                var cell = (DataGridViewCell)model.GetValue(iter, i);
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
                        foreach (var columnIndex in _selectedBandIndexes)
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
                        foreach (var rowIndex in _selectedBandIndexes)
                        {
                            var dataGridViewRow = (DataGridViewRow)Rows[rowIndex];
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

    /// <summary>
    /// Gets the collection of currently selected columns.
    /// The returned collection depends on the current SelectionMode.
    /// </summary>
    [Browsable(false)]
    public DataGridViewSelectedColumnCollection SelectedColumns
    {
        get
        {
            var strc = new DataGridViewSelectedColumnCollection();
            switch (SelectionMode)
            {
                case DataGridViewSelectionMode.CellSelect:
                case DataGridViewSelectionMode.FullRowSelect:
                case DataGridViewSelectionMode.RowHeaderSelect:
                    break;
                case DataGridViewSelectionMode.FullColumnSelect:
                case DataGridViewSelectionMode.ColumnHeaderSelect:
                    foreach (var columnIndex in _selectedBandIndexes)
                    {
                        strc.Add(Columns[columnIndex]);
                    }

                    break;
            }

            return strc;
        }
    }

    /// <summary>
    /// Gets the collection of currently selected rows.
    /// The returned collection depends on the current SelectionMode.
    /// </summary>
    [Browsable(false)]
    public DataGridViewSelectedRowCollection SelectedRows
    {
        get
        {
            var strc = new DataGridViewSelectedRowCollection();
            switch (SelectionMode)
            {
                case DataGridViewSelectionMode.CellSelect:
                case DataGridViewSelectionMode.FullColumnSelect:
                case DataGridViewSelectionMode.ColumnHeaderSelect:
                    break;
                case DataGridViewSelectionMode.FullRowSelect:
                case DataGridViewSelectionMode.RowHeaderSelect:
                    foreach (var rowIndex in _selectedBandIndexes)
                    {
                        strc.Add(Rows[rowIndex]);
                    }
                    break;
            }

            return strc;
        }
    }

    /// <summary>
    /// Gets or sets the current selection mode for the grid.
    /// Setting this updates the underlying GTK selection behavior.
    /// </summary>
    public DataGridViewSelectionMode SelectionMode
    {
        get => _SelectionMode;
        set
        {
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
        }
    }

    /// <summary>
    /// Begins initialization. Sets internal created flag to false.
    /// </summary>
    public override void BeginInit()
    {
        _Created = false;
    }

    /// <summary>
    /// Clears the current selection in the grid.
    /// </summary>
    public void ClearSelection()
    {
        GridView.Selection.UnselectAll();
    }

    /// <summary>
    /// Ends initialization. Sets internal created flag to true.
    /// </summary>
    public override void EndInit()
    {
        _Created = true;
    }

    /// <summary>
    /// Returns whether the native (GTK) row at the given index is selected.
    /// </summary>
    /// <param name="rowindex">
    /// Row index to test.
    /// </param>
    /// <returns>
    /// True if the row is selected; otherwise false.
    /// </returns>
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

    /// <summary>
    /// Selects or unselects the native GTK row at the specified index.
    /// Uses Gtk.Application.Invoke to ensure the selection change is performed on the UI thread.
    /// </summary>
    /// <param name="rowindex">
    /// Row index to operate on.
    /// </param>
    /// <param name="selected">
    /// True to select, false to unselect.
    /// </param>
    public void NativeRowSetSelected(int rowindex, bool selected)
    {
        Gtk.Application.Invoke(delegate
        {
            if (selected)
                GridView.Selection.SelectIter(Rows[rowindex].TreeIter);
            else
                GridView.Selection.UnselectIter(Rows[rowindex].TreeIter);
        });
    }

    /// <summary>
    /// Raises the CellPainting event (invoked internally).
    /// </summary>
    /// <param name="sender">
    /// Event sender.
    /// </param>
    /// <param name="e">
    /// Cell painting event args.
    /// </param>
    public void OnCellPainting(object sender, DataGridViewCellPaintingEventArgs e)
    {
        CellPainting?.Invoke(sender, e);
    }

    /// <summary>
    /// Collapses the specified row in the tree view.
    /// </summary>
    /// <param name="row">
    /// Row to collapse.
    /// </param>
    public void SetCollapseRow(DataGridViewRow row)
    {
        GridView.CollapseRow(Store.GetPath(row.TreeIter));
    }

    /// <summary>
    /// Expands the specified row in the tree view.
    /// </summary>
    /// <param name="row">
    /// Row to expand.
    /// </param>
    /// <param name="all">
    /// True to expand all descendants.
    /// </param>
    public void SetExpandRow(DataGridViewRow row, bool all)
    {
        GridView.ExpandRow(Store.GetPath(row.TreeIter), all);
    }

    internal void CellValueChanagedHandler(int column, int row)
    {
        OnCellValueChanged(new DataGridViewCellEventArgs(column, row));
    }

    /// <summary>
    /// Raises the AllowUserToAddRowsChanged event.
    /// </summary>
    protected virtual void OnAllowUserToAddRowsChanged(System.EventArgs e)
    {
        AllowUserToAddRowsChanged?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the AllowUserToDeleteRowsChanged event.
    /// </summary>
    protected virtual void OnAllowUserToDeleteRowsChanged(System.EventArgs e)
    {
        AllowUserToDeleteRowsChanged?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the AllowUserToOrderColumnsChanged event.
    /// </summary>
    protected virtual void OnAllowUserToOrderColumnsChanged(System.EventArgs e)
    {
        AllowUserToOrderColumnsChanged?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the AllowUserToResizeColumnsChanged event.
    /// </summary>
    protected virtual void OnAllowUserToResizeColumnsChanged(System.EventArgs e)
    {
        AllowUserToResizeColumnsChanged?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the AllowUserToResizeRowsChanged event.
    /// </summary>
    protected virtual void OnAllowUserToResizeRowsChanged(System.EventArgs e)
    {
        AllowUserToResizeRowsChanged?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the AlternatingRowsDefaultCellStyleChanged event.
    /// </summary>
    protected virtual void OnAlternatingRowsDefaultCellStyleChanged(System.EventArgs e)
    {
        AlternatingRowsDefaultCellStyleChanged?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the AutoGenerateColumnsChanged event.
    /// </summary>
    protected virtual void OnAutoGenerateColumnsChanged(System.EventArgs e)
    {
        AutoGenerateColumnsChanged?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the AutoSizeColumnModeChanged event.
    /// </summary>
    protected virtual void OnAutoSizeColumnModeChanged(System.Windows.Forms.DataGridViewAutoSizeColumnModeEventArgs e)
    {
        AutoSizeColumnModeChanged?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the AutoSizeColumnsModeChanged event.
    /// </summary>
    protected virtual void OnAutoSizeColumnsModeChanged(System.Windows.Forms.DataGridViewAutoSizeColumnsModeEventArgs e)
    {
        AutoSizeColumnsModeChanged?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the AutoSizeRowsModeChanged event.
    /// </summary>
    protected virtual void OnAutoSizeRowsModeChanged(System.Windows.Forms.DataGridViewAutoSizeModeEventArgs e)
    {
        AutoSizeRowsModeChanged?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the BackgroundColorChanged event.
    /// </summary>
    protected virtual void OnBackgroundColorChanged(System.EventArgs e)
    {
        BackgroundColorChanged?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the BorderStyleChanged event.
    /// </summary>
    protected virtual void OnBorderStyleChanged(System.EventArgs e)
    {
        BorderStyleChanged?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the CancelRowEdit event.
    /// </summary>
    protected virtual void OnCancelRowEdit(System.Windows.Forms.QuestionEventArgs e)
    {
        CancelRowEdit?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the CellBeginEdit event.
    /// </summary>
    protected virtual void OnCellBeginEdit(System.Windows.Forms.DataGridViewCellCancelEventArgs e)
    {
        CellBeginEdit?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the CellBorderStyleChanged event.
    /// </summary>
    protected virtual void OnCellBorderStyleChanged(System.EventArgs e)
    {
        CellBorderStyleChanged?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the CellClick event.
    /// </summary>
    protected virtual void OnCellClick(System.Windows.Forms.DataGridViewCellEventArgs e)
    {
        CellClick?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the CellContentClick event.
    /// </summary>
    protected virtual void OnCellContentClick(System.Windows.Forms.DataGridViewCellEventArgs e)
    {
        CellContentClick?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the CellContentDoubleClick event.
    /// </summary>
    protected virtual void OnCellContentDoubleClick(System.Windows.Forms.DataGridViewCellEventArgs e)
    {
        CellContentDoubleClick?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the CellContextMenuStripChanged event.
    /// </summary>
    protected virtual void OnCellContextMenuStripChanged(System.Windows.Forms.DataGridViewCellEventArgs e)
    {
        CellContextMenuStripChanged?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the CellContextMenuStripNeeded event.
    /// </summary>
    protected virtual void OnCellContextMenuStripNeeded(System.Windows.Forms.DataGridViewCellContextMenuStripNeededEventArgs e)
    {
        CellContextMenuStripNeeded?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the CellDoubleClick event.
    /// </summary>
    protected virtual void OnCellDoubleClick(System.Windows.Forms.DataGridViewCellEventArgs e)
    {
        CellDoubleClick?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the CellEndEdit event.
    /// </summary>
    protected virtual void OnCellEndEdit(System.Windows.Forms.DataGridViewCellEventArgs e)
    {
        CellEndEdit?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the CellEnter event.
    /// </summary>
    protected virtual void OnCellEnter(System.Windows.Forms.DataGridViewCellEventArgs e)
    {
        CellEnter?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the CellErrorTextChanged event.
    /// </summary>
    protected virtual void OnCellErrorTextChanged(System.Windows.Forms.DataGridViewCellEventArgs e)
    {
        CellErrorTextChanged?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the CellErrorTextNeeded event.
    /// </summary>
    protected virtual void OnCellErrorTextNeeded(System.Windows.Forms.DataGridViewCellErrorTextNeededEventArgs e)
    {
        CellErrorTextNeeded?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the CellFormatting event.
    /// </summary>
    protected virtual void OnCellFormatting(System.Windows.Forms.DataGridViewCellFormattingEventArgs e)
    {
        CellFormatting?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the CellLeave event.
    /// </summary>
    protected virtual void OnCellLeave(System.Windows.Forms.DataGridViewCellEventArgs e)
    {
        CellLeave?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the CellMouseClick event.
    /// </summary>
    protected virtual void OnCellMouseClick(System.Windows.Forms.DataGridViewCellMouseEventArgs e)
    {
        CellMouseClick?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the CellMouseDoubleClick event.
    /// </summary>
    protected virtual void OnCellMouseDoubleClick(System.Windows.Forms.DataGridViewCellMouseEventArgs e)
    {
        CellMouseDoubleClick?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the CellMouseDown event.
    /// </summary>
    protected virtual void OnCellMouseDown(System.Windows.Forms.DataGridViewCellMouseEventArgs e)
    {
        CellMouseDown?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the CellMouseEnter event.
    /// </summary>
    protected virtual void OnCellMouseEnter(System.Windows.Forms.DataGridViewCellEventArgs e)
    {
        CellMouseEnter?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the CellMouseLeave event.
    /// </summary>
    protected virtual void OnCellMouseLeave(System.Windows.Forms.DataGridViewCellEventArgs e)
    {
        CellMouseLeave?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the CellMouseMove event.
    /// </summary>
    protected virtual void OnCellMouseMove(System.Windows.Forms.DataGridViewCellMouseEventArgs e)
    {
        CellMouseMove?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the CellMouseUp event.
    /// </summary>
    protected virtual void OnCellMouseUp(System.Windows.Forms.DataGridViewCellMouseEventArgs e)
    {
        CellMouseUp?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the CellParsing event.
    /// </summary>
    protected virtual void OnCellParsing(System.Windows.Forms.DataGridViewCellParsingEventArgs e)
    {
        CellParsing?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the CellStateChanged event.
    /// </summary>
    protected virtual void OnCellStateChanged(System.Windows.Forms.DataGridViewCellStateChangedEventArgs e)
    {
        CellStateChanged?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the CellStyleChanged event.
    /// </summary>
    protected virtual void OnCellStyleChanged(System.Windows.Forms.DataGridViewCellEventArgs e)
    {
        CellStyleChanged?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the CellStyleContentChanged event.
    /// </summary>
    protected virtual void OnCellStyleContentChanged(System.Windows.Forms.DataGridViewCellStyleContentChangedEventArgs e)
    {
        CellStyleContentChanged?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the CellToolTipTextChanged event.
    /// </summary>
    protected virtual void OnCellToolTipTextChanged(System.Windows.Forms.DataGridViewCellEventArgs e)
    {
        CellToolTipTextChanged?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the CellToolTipTextNeeded event.
    /// </summary>
    protected virtual void OnCellToolTipTextNeeded(System.Windows.Forms.DataGridViewCellToolTipTextNeededEventArgs e)
    {
        CellToolTipTextNeeded?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the CellValidated event.
    /// </summary>
    protected virtual void OnCellValidated(System.Windows.Forms.DataGridViewCellEventArgs e)
    {
        CellValidated?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the CellValidating event.
    /// </summary>
    protected virtual void OnCellValidating(System.Windows.Forms.DataGridViewCellValidatingEventArgs e)
    {
        CellValidating?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the CellValueChanged event.
    /// </summary>
    protected virtual void OnCellValueChanged(System.Windows.Forms.DataGridViewCellEventArgs e)
    {
        CellValueChanged?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the CellValueNeeded event.
    /// </summary>
    protected virtual void OnCellValueNeeded(System.Windows.Forms.DataGridViewCellValueEventArgs e)
    {
        CellValueNeeded?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the CellValuePushed event.
    /// </summary>
    protected virtual void OnCellValuePushed(System.Windows.Forms.DataGridViewCellValueEventArgs e)
    {
        CellValuePushed?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the ColumnAdded event.
    /// </summary>
    protected virtual void OnColumnAdded(System.Windows.Forms.DataGridViewColumnEventArgs e)
    {
        ColumnAdded?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the ColumnContextMenuStripChanged event.
    /// </summary>
    protected virtual void OnColumnContextMenuStripChanged(System.Windows.Forms.DataGridViewColumnEventArgs e)
    {
        ColumnContextMenuStripChanged?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the ColumnDataPropertyNameChanged event.
    /// </summary>
    protected virtual void OnColumnDataPropertyNameChanged(System.Windows.Forms.DataGridViewColumnEventArgs e)
    {
        ColumnDataPropertyNameChanged?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the ColumnDefaultCellStyleChanged event.
    /// </summary>
    protected virtual void OnColumnDefaultCellStyleChanged(System.Windows.Forms.DataGridViewColumnEventArgs e)
    {
        ColumnDefaultCellStyleChanged?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the ColumnDisplayIndexChanged event.
    /// </summary>
    protected virtual void OnColumnDisplayIndexChanged(System.Windows.Forms.DataGridViewColumnEventArgs e)
    {
        ColumnDisplayIndexChanged?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the ColumnDividerDoubleClick event.
    /// </summary>
    protected virtual void OnColumnDividerDoubleClick(System.Windows.Forms.DataGridViewColumnDividerDoubleClickEventArgs e)
    {
        ColumnDividerDoubleClick?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the ColumnDividerWidthChanged event.
    /// </summary>
    protected virtual void OnColumnDividerWidthChanged(System.Windows.Forms.DataGridViewColumnEventArgs e)
    {
        ColumnDividerWidthChanged?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the ColumnHeaderCellChanged event.
    /// </summary>
    protected virtual void OnColumnHeaderCellChanged(System.Windows.Forms.DataGridViewColumnEventArgs e)
    {
        ColumnHeaderCellChanged?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the ColumnHeaderMouseClick event.
    /// </summary>
    protected virtual void OnColumnHeaderMouseClick(System.Windows.Forms.DataGridViewCellMouseEventArgs e)
    {
        ColumnHeaderMouseClick?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the ColumnHeaderMouseDoubleClick event.
    /// </summary>
    protected virtual void OnColumnHeaderMouseDoubleClick(System.Windows.Forms.DataGridViewCellMouseEventArgs e)
    {
        ColumnHeaderMouseDoubleClick?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the ColumnHeadersBorderStyleChanged event.
    /// </summary>
    protected virtual void OnColumnHeadersBorderStyleChanged(System.EventArgs e)
    {
        ColumnHeadersBorderStyleChanged?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the ColumnHeadersDefaultCellStyleChanged event.
    /// </summary>
    protected virtual void OnColumnHeadersDefaultCellStyleChanged(System.EventArgs e)
    {
        ColumnHeadersDefaultCellStyleChanged?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the ColumnHeadersHeightChanged event.
    /// </summary>
    protected virtual void OnColumnHeadersHeightChanged(System.EventArgs e)
    {
        ColumnHeadersHeightChanged?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the ColumnHeadersHeightSizeModeChanged event.
    /// </summary>
    protected virtual void OnColumnHeadersHeightSizeModeChanged(System.Windows.Forms.DataGridViewAutoSizeModeEventArgs e)
    {
        ColumnHeadersHeightSizeModeChanged?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the ColumnMinimumWidthChanged event.
    /// </summary>
    protected virtual void OnColumnMinimumWidthChanged(System.Windows.Forms.DataGridViewColumnEventArgs e)
    {
        ColumnMinimumWidthChanged?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the ColumnNameChanged event.
    /// </summary>
    protected virtual void OnColumnNameChanged(System.Windows.Forms.DataGridViewColumnEventArgs e)
    {
        ColumnNameChanged?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the ColumnRemoved event.
    /// </summary>
    protected virtual void OnColumnRemoved(System.Windows.Forms.DataGridViewColumnEventArgs e)
    {
        ColumnRemoved?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the ColumnSortModeChanged event.
    /// </summary>
    protected virtual void OnColumnSortModeChanged(System.Windows.Forms.DataGridViewColumnEventArgs e)
    {
        ColumnSortModeChanged?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the ColumnStateChanged event.
    /// </summary>
    protected virtual void OnColumnStateChanged(System.Windows.Forms.DataGridViewColumnStateChangedEventArgs e)
    {
        ColumnStateChanged?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the ColumnToolTipTextChanged event.
    /// </summary>
    protected virtual void OnColumnToolTipTextChanged(System.Windows.Forms.DataGridViewColumnEventArgs e)
    {
        ColumnToolTipTextChanged?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the ColumnWidthChanged event.
    /// </summary>
    protected virtual void OnColumnWidthChanged(System.Windows.Forms.DataGridViewColumnEventArgs e)
    {
        ColumnWidthChanged?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the CurrentCellChanged event.
    /// </summary>
    protected virtual void OnCurrentCellChanged(System.EventArgs e)
    {
        CurrentCellChanged?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the CurrentCellDirtyStateChanged event.
    /// </summary>
    protected virtual void OnCurrentCellDirtyStateChanged(System.EventArgs e)
    {
        CurrentCellDirtyStateChanged?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the DataBindingComplete event.
    /// </summary>
    protected virtual void OnDataBindingComplete(System.Windows.Forms.DataGridViewBindingCompleteEventArgs e)
    {
        DataBindingComplete?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the DataError event.
    /// </summary>
    protected virtual void OnDataError(System.Windows.Forms.DataGridViewDataErrorEventArgs e)
    {
        DataError?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the DataMemberChanged event.
    /// </summary>
    protected virtual void OnDataMemberChanged(System.EventArgs e)
    {
        DataMemberChanged?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the DataSourceChanged event.
    /// </summary>
    protected virtual void OnDataSourceChanged(System.EventArgs e)
    {
        DataSourceChanged?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the DefaultCellStyleChanged event.
    /// </summary>
    protected virtual void OnDefaultCellStyleChanged(System.EventArgs e)
    {
        DefaultCellStyleChanged?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the DefaultValuesNeeded event.
    /// </summary>
    protected virtual void OnDefaultValuesNeeded(System.Windows.Forms.DataGridViewRowEventArgs e)
    {
        DefaultValuesNeeded?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the EditingControlShowing event.
    /// </summary>
    protected virtual void OnEditingControlShowing(System.Windows.Forms.DataGridViewEditingControlShowingEventArgs e)
    {
        EditingControlShowing?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the EditModeChanged event.
    /// </summary>
    protected virtual void OnEditModeChanged(System.EventArgs e)
    {
        EditModeChanged?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the GridColorChanged event.
    /// </summary>
    protected virtual void OnGridColorChanged(System.EventArgs e)
    {
        GridColorChanged?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the MultiSelectChanged event.
    /// </summary>
    protected virtual void OnMultiSelectChanged(System.EventArgs e)
    {
        MultiSelectChanged?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the NewRowNeeded event.
    /// </summary>
    protected virtual void OnNewRowNeeded(System.Windows.Forms.DataGridViewRowEventArgs e)
    {
        NewRowNeeded?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the ReadOnlyChanged event.
    /// </summary>
    protected virtual void OnReadOnlyChanged(System.EventArgs e)
    {
        ReadOnlyChanged?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the RowContextMenuStripChanged event.
    /// </summary>
    protected virtual void OnRowContextMenuStripChanged(System.Windows.Forms.DataGridViewRowEventArgs e)
    {
        RowContextMenuStripChanged?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the RowContextMenuStripNeeded event.
    /// </summary>
    protected virtual void OnRowContextMenuStripNeeded(System.Windows.Forms.DataGridViewRowContextMenuStripNeededEventArgs e)
    {
        RowContextMenuStripNeeded?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the RowDefaultCellStyleChanged event.
    /// </summary>
    protected virtual void OnRowDefaultCellStyleChanged(System.Windows.Forms.DataGridViewRowEventArgs e)
    {
        RowDefaultCellStyleChanged?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the RowDirtyStateNeeded event.
    /// </summary>
    protected virtual void OnRowDirtyStateNeeded(System.Windows.Forms.QuestionEventArgs e)
    {
        RowDirtyStateNeeded?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the RowDividerDoubleClick event.
    /// </summary>
    protected virtual void OnRowDividerDoubleClick(System.Windows.Forms.DataGridViewRowDividerDoubleClickEventArgs e)
    {
        RowDividerDoubleClick?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the RowDividerHeightChanged event.
    /// </summary>
    protected virtual void OnRowDividerHeightChanged(System.Windows.Forms.DataGridViewRowEventArgs e)
    {
        RowDividerHeightChanged?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the RowEnter event.
    /// </summary>
    protected virtual void OnRowEnter(System.Windows.Forms.DataGridViewCellEventArgs e)
    {
        RowEnter?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the RowErrorTextChanged event.
    /// </summary>
    protected virtual void OnRowErrorTextChanged(System.Windows.Forms.DataGridViewRowEventArgs e)
    {
        RowErrorTextChanged?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the RowErrorTextNeeded event.
    /// </summary>
    protected virtual void OnRowErrorTextNeeded(System.Windows.Forms.DataGridViewRowErrorTextNeededEventArgs e)
    {
        RowErrorTextNeeded?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the RowHeaderCellChanged event.
    /// </summary>
    protected virtual void OnRowHeaderCellChanged(System.Windows.Forms.DataGridViewRowEventArgs e)
    {
        RowHeaderCellChanged?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the RowHeaderMouseClick event.
    /// </summary>
    protected virtual void OnRowHeaderMouseClick(System.Windows.Forms.DataGridViewCellMouseEventArgs e)
    {
        RowHeaderMouseClick?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the RowHeaderMouseDoubleClick event.
    /// </summary>
    protected virtual void OnRowHeaderMouseDoubleClick(System.Windows.Forms.DataGridViewCellMouseEventArgs e)
    {
        RowHeaderMouseDoubleClick?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the RowHeadersBorderStyleChanged event.
    /// </summary>
    protected virtual void OnRowHeadersBorderStyleChanged(System.EventArgs e)
    {
        RowHeadersBorderStyleChanged?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the RowHeadersDefaultCellStyleChanged event.
    /// </summary>
    protected virtual void OnRowHeadersDefaultCellStyleChanged(System.EventArgs e)
    {
        RowHeadersDefaultCellStyleChanged?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the RowHeadersWidthChanged event.
    /// </summary>
    protected virtual void OnRowHeadersWidthChanged(System.EventArgs e)
    {
        RowHeadersWidthChanged?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the RowHeadersWidthSizeModeChanged event.
    /// </summary>
    protected virtual void OnRowHeadersWidthSizeModeChanged(System.Windows.Forms.DataGridViewAutoSizeModeEventArgs e)
    {
        RowHeadersWidthSizeModeChanged?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the RowHeightChanged event.
    /// </summary>
    protected virtual void OnRowHeightChanged(System.Windows.Forms.DataGridViewRowEventArgs e)
    {
        RowHeightChanged?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the RowHeightInfoNeeded event.
    /// </summary>
    protected virtual void OnRowHeightInfoNeeded(System.Windows.Forms.DataGridViewRowHeightInfoNeededEventArgs e)
    {
        RowHeightInfoNeeded?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the RowHeightInfoPushed event.
    /// </summary>
    protected virtual void OnRowHeightInfoPushed(System.Windows.Forms.DataGridViewRowHeightInfoPushedEventArgs e)
    {
        RowHeightInfoPushed?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the RowLeave event.
    /// </summary>
    protected virtual void OnRowLeave(System.Windows.Forms.DataGridViewCellEventArgs e)
    {
        RowLeave?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the RowMinimumHeightChanged event.
    /// </summary>
    protected virtual void OnRowMinimumHeightChanged(System.Windows.Forms.DataGridViewRowEventArgs e)
    {
        RowMinimumHeightChanged?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the RowPostPaint event.
    /// </summary>
    protected virtual void OnRowPostPaint(System.Windows.Forms.DataGridViewRowPostPaintEventArgs e)
    {
        RowPostPaint?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the RowPrePaint event.
    /// </summary>
    protected virtual void OnRowPrePaint(System.Windows.Forms.DataGridViewRowPrePaintEventArgs e)
    {
        RowPrePaint?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the RowsAdded event.
    /// </summary>
    protected virtual void OnRowsAdded(System.Windows.Forms.DataGridViewRowsAddedEventArgs e)
    {
        RowsAdded?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the RowsDefaultCellStyleChanged event.
    /// </summary>
    protected virtual void OnRowsDefaultCellStyleChanged(System.EventArgs e)
    {
        RowsDefaultCellStyleChanged?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the RowsRemoved event.
    /// </summary>
    protected virtual void OnRowsRemoved(System.Windows.Forms.DataGridViewRowsRemovedEventArgs e)
    {
        RowsRemoved?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the RowStateChanged event.
    /// </summary>
    protected virtual void OnRowStateChanged(System.Windows.Forms.DataGridViewRowStateChangedEventArgs e)
    {
        RowStateChanged?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the RowUnshared event.
    /// </summary>
    protected virtual void OnRowUnshared(System.Windows.Forms.DataGridViewRowEventArgs e)
    {
        RowUnshared?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the RowValidated event.
    /// </summary>
    protected virtual void OnRowValidated(System.Windows.Forms.DataGridViewCellEventArgs e)
    {
        RowValidated?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the RowValidating event.
    /// </summary>
    protected virtual void OnRowValidating(System.Windows.Forms.DataGridViewCellCancelEventArgs e)
    {
        RowValidating?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the SelectionChanged event.
    /// </summary>
    protected virtual void OnSelectionChanged(System.EventArgs e)
    {
        SelectionChanged?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the SortCompare event.
    /// </summary>
    protected virtual void OnSortCompare(System.Windows.Forms.DataGridViewSortCompareEventArgs e)
    {
        SortCompare?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the Sorted event.
    /// </summary>
    protected virtual void OnSorted(System.EventArgs e)
    {
        Sorted?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the UserAddedRow event.
    /// </summary>
    protected virtual void OnUserAddedRow(System.Windows.Forms.DataGridViewRowEventArgs e)
    {
        UserAddedRow?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the UserDeletedRow event.
    /// </summary>
    protected virtual void OnUserDeletedRow(System.Windows.Forms.DataGridViewRowEventArgs e)
    {
        UserDeletedRow?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the UserDeletingRow event.
    /// </summary>
    protected virtual void OnUserDeletingRow(System.Windows.Forms.DataGridViewRowCancelEventArgs e)
    {
        UserDeletingRow?.Invoke(this, e);
    }

    private void GridView_Realized(object sender, EventArgs e)
    {
        OnSetDataSource();
        _columns.Invalidate();
        foreach (Binding binding in DataBindings)
            GridView.AddNotification(binding.PropertyName, propertyNotity);
    }

    private void GridView_RowActivated(object o, RowActivatedArgs args)
    {
        // single-line selection is effective
        if (CellClick != null)
        {
            if (args.Column is DataGridViewColumn column)
            {
                OnCellClick(new DataGridViewCellEventArgs(column.Index, args.Path.Indices.Last()));
            }
        }
    }

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
                var newRow = new DataGridViewRow();
                foreach (var col in _columns)
                {
                    var cellvalue = dt.Columns.Contains(col.DataPropertyName) ? dr[col.DataPropertyName] : null;
                    var dataGridViewCell = col.NewCell(cellvalue, col.ValueType);
                    if (dataGridViewCell != null) newRow.Cells.Add(dataGridViewCell);
                }
                _rows.Add(newRow);
            }
        }
    }

    private void LoadListSource()
    {
        var _type = _DataSource?.GetType();
        if (_type != null)
        {
            var _entityType = _type.GetGenericArguments();
            if (_entityType.Length == 1)
            {
                var pros = _entityType[0].GetProperties(BindingFlags.Public|BindingFlags.Instance);
                foreach (var pro in pros)
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
                    var reader = ((IEnumerable?)_DataSource)?.GetEnumerator();
                    while (reader != null && reader.MoveNext())
                    {
                        var obj = reader.Current;
                        var type = obj.GetType();
                        var newRow = new DataGridViewRow();
                        foreach (var col in _columns)
                        {
                            var cellvalue = type.GetProperty(col.DataPropertyName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty)?.GetValue(obj);
                            var dataGridViewCell = col.NewCell(cellvalue, col.ValueType);
                            if (dataGridViewCell != null) newRow.Cells.Add(dataGridViewCell);   
                        }
                        _rows.Add(newRow);
                    }
                }
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

    private void propertyNotity(object o, NotifyArgs args)
    {
        var binding = DataBindings[args.Property];
        binding?.WriteValue();
    }

    private void Selection_Changed(object sender, EventArgs e)
    {
        _selectedBandIndexes.Clear();
        var treePaths = GridView.Selection.GetSelectedRows();
        foreach (var path in treePaths)
        {
            var idx = path.Indices.Last();
            _selectedBandIndexes.Add(idx);
        }
        if (SelectionChanged != null && Created)
            OnSelectionChanged(e);
    }
}