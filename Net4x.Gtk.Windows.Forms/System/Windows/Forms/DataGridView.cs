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
    public readonly DataGridViewBase self = new DataGridViewBase();

    internal Gtk.TreeStore Store = new TreeStore(typeof(DataGridViewCell));

    private readonly ControlBindingsCollection _collect;

    private readonly DataGridViewColumnCollection _columns;

    private object _DataSource;

    private bool _MultiSelect = true;

    private readonly DataGridViewRowCollection _rows;

    private DataGridViewRow _RowTemplate;

    private readonly List<int> _selectedBandIndexes = new List<int>();

    private DataGridViewSelectionMode _SelectionMode = DataGridViewSelectionMode.RowHeaderSelect;

    /// <summary>
    /// Not implemented in this GTK-backed control. Placeholder for compatibility.
    /// </summary>
    [Obsolete(" this event is not implemented， develop independently")]
    public event EventHandler AllowUserToAddRowsChanged;

    /// <summary>
    /// Not implemented in this GTK-backed control. Placeholder for compatibility.
    /// </summary>
    [Obsolete(" this event is not implemented， develop independently")]
    public event EventHandler AllowUserToDeleteRowsChanged;

    /// <summary>
    /// Not implemented in this GTK-backed control. Placeholder for compatibility.
    /// </summary>
    //[Obsolete(" this event is not implemented， develop independently")]
    //public event EventHandler SelectionChanged;
    [Obsolete(" this event is not implemented， develop independently")]
    public event EventHandler AllowUserToOrderColumnsChanged;

    /// <summary>
    /// Not implemented in this GTK-backed control. Placeholder for compatibility.
    /// </summary>
    [Obsolete(" this event is not implemented， develop independently")]
    public event EventHandler AllowUserToResizeColumnsChanged;

    /// <summary>
    /// Not implemented in this GTK-backed control. Placeholder for compatibility.
    /// </summary>
    [Obsolete(" this event is not implemented， develop independently")]
    public event EventHandler AllowUserToResizeRowsChanged;

    /// <summary>
    /// Not implemented - placeholder for alternating rows default cell style change notifications.
    /// </summary>
    [Obsolete(" this event is not implemented， develop independently")]
    public event EventHandler AlternatingRowsDefaultCellStyleChanged;

    /// <summary>
    /// Raised when AutoGenerateColumns setting changes - not implemented here.
    /// </summary>
    [Obsolete(" this event is not implemented， develop independently")]
    public event EventHandler AutoGenerateColumnsChanged;

    /// <summary>
    /// Raised when the AutoSizeColumnMode changes - not implemented here.
    /// </summary>
    [Obsolete(" this event is not implemented， develop independently")]
    public event DataGridViewAutoSizeColumnModeEventHandler AutoSizeColumnModeChanged;

    /// <summary>
    /// Raised when the AutoSizeColumnsMode changes - not implemented here.
    /// </summary>
    [Obsolete(" this event is not implemented， develop independently")]
    public event DataGridViewAutoSizeColumnsModeEventHandler AutoSizeColumnsModeChanged;

    /// <summary>
    /// Raised when AutoSizeRowsMode changes - not implemented here.
    /// </summary>
    //[Obsolete(" this event is not implemented， develop independently")]
    //public event EventHandler BackColorChanged;
    [Obsolete(" this event is not implemented， develop independently")]
    public event DataGridViewAutoSizeModeEventHandler AutoSizeRowsModeChanged;

    /// <summary>
    /// Raised when the background color changes - not implemented here.
    /// </summary>
    //[Obsolete(" this event is not implemented， develop independently")]
    //public event EventHandler BackgroundImageChanged;
    [Obsolete(" this event is not implemented， develop independently")]
    public event EventHandler BackgroundColorChanged;

    /// <summary>
    /// Raised when the border style changes - not implemented here.
    /// </summary>
    [Obsolete(" this event is not implemented， develop independently")]
    public event EventHandler BorderStyleChanged;

    /// <summary>
    /// Raised to cancel a row edit - not implemented here.
    /// </summary>
    [Obsolete(" this event is not implemented， develop independently")]
    public event QuestionEventHandler CancelRowEdit;

    /// <summary>
    /// Raised when a cell begins edit. Placeholder for compatibility.
    /// </summary>
    //[Obsolete(" this event is not implemented， develop independently")]
    //public event EventHandler PaddingChanged;
    [Obsolete(" this event is not implemented， develop independently")]
    public event DataGridViewCellCancelEventHandler CellBeginEdit;

    /// <summary>
    /// Raised when the cell border style changes - not implemented here.
    /// </summary>
    //[Obsolete(" this event is not implemented， develop independently")]
    //public event EventHandler StyleChanged;
    //[Obsolete(" this event is not implemented， develop independently")]
    //public event EventHandler BackgroundImageLayoutChanged;
    [Obsolete(" this event is not implemented， develop independently")]
    public event EventHandler CellBorderStyleChanged;

    /// <summary>
    /// Raised when a cell is clicked.
    /// </summary>
    public event DataGridViewCellEventHandler CellClick;

    /// <summary>
    /// Raised when a cell content is clicked - placeholder for compatibility.
    /// </summary>
    //[Obsolete(" this event is not implemented， develop independently")]
    //public event DataGridViewCellEventHandler CellClick;
    [Obsolete(" this event is not implemented， develop independently")]
    public event DataGridViewCellEventHandler CellContentClick;

    /// <summary>
    /// Raised when a cell content is double-clicked - placeholder for compatibility.
    /// </summary>
    [Obsolete(" this event is not implemented， develop independently")]
    public event DataGridViewCellEventHandler CellContentDoubleClick;

    /// <summary>
    /// Raised when a cell's context menu strip is changed - placeholder for compatibility.
    /// </summary>
    [Obsolete(" this event is not implemented， develop independently")]
    public event DataGridViewCellEventHandler CellContextMenuStripChanged;

    /// <summary>
    /// Raised when a context menu strip is needed for a cell - placeholder for compatibility.
    /// </summary>
    [Obsolete(" this event is not implemented， develop independently")]
    public event DataGridViewCellContextMenuStripNeededEventHandler CellContextMenuStripNeeded;

    /// <summary>
    /// Raised when a cell is double-clicked - placeholder for compatibility.
    /// </summary>
    [Obsolete(" this event is not implemented， develop independently")]
    public event DataGridViewCellEventHandler CellDoubleClick;

    /// <summary>
    /// Raised when a cell edit ends - placeholder for compatibility.
    /// </summary>
    [Obsolete(" this event is not implemented， develop independently")]
    public event DataGridViewCellEventHandler CellEndEdit;

    /// <summary>
    /// Raised when a cell receives focus - placeholder for compatibility.
    /// </summary>
    [Obsolete(" this event is not implemented， develop independently")]
    public event DataGridViewCellEventHandler CellEnter;

    /// <summary>
    /// Raised when a cell's error text is changed - placeholder for compatibility.
    /// </summary>
    [Obsolete(" this event is not implemented， develop independently")]
    public event DataGridViewCellEventHandler CellErrorTextChanged;

    /// <summary>
    /// Raised when a cell's error text is needed - placeholder for compatibility.
    /// </summary>
    [Obsolete(" this event is not implemented， develop independently")]
    public event DataGridViewCellErrorTextNeededEventHandler CellErrorTextNeeded;

    /// <summary>
    /// Raised when a cell requires formatting - placeholder for compatibility.
    /// </summary>
    [Obsolete(" this event is not implemented， develop independently")]
    public event DataGridViewCellFormattingEventHandler CellFormatting;

    /// <summary>
    /// Raised when a cell loses focus - placeholder for compatibility.
    /// </summary>
    [Obsolete(" this event is not implemented， develop independently")]
    public event DataGridViewCellEventHandler CellLeave;

    /// <summary>
    /// Raised for cell mouse click events - placeholder for compatibility.
    /// </summary>
    [Obsolete(" this event is not implemented， develop independently")]
    public event DataGridViewCellMouseEventHandler CellMouseClick;

    /// <summary>
    /// Raised for cell mouse double-click events - placeholder for compatibility.
    /// </summary>
    [Obsolete(" this event is not implemented， develop independently")]
    public event DataGridViewCellMouseEventHandler CellMouseDoubleClick;

    /// <summary>
    /// Raised for cell mouse down events - placeholder for compatibility.
    /// </summary>
    [Obsolete(" this event is not implemented， develop independently")]
    public event DataGridViewCellMouseEventHandler CellMouseDown;

    /// <summary>
    /// Raised when mouse enters a cell - placeholder for compatibility.
    /// </summary>
    [Obsolete(" this event is not implemented， develop independently")]
    public event DataGridViewCellEventHandler CellMouseEnter;

    /// <summary>
    /// Raised when mouse leaves a cell - placeholder for compatibility.
    /// </summary>
    [Obsolete(" this event is not implemented， develop independently")]
    public event DataGridViewCellEventHandler CellMouseLeave;

    /// <summary>
    /// Raised for cell mouse move events - placeholder for compatibility.
    /// </summary>
    [Obsolete(" this event is not implemented， develop independently")]
    public event DataGridViewCellMouseEventHandler CellMouseMove;

    /// <summary>
    /// Raised for cell mouse up events - placeholder for compatibility.
    /// </summary>
    [Obsolete(" this event is not implemented， develop independently")]
    public event DataGridViewCellMouseEventHandler CellMouseUp;

    /// <summary>
    /// Raised when a cell needs custom painting. Use this to customize cell rendering.
    /// </summary>
    public event DataGridViewCellPaintingEventHandler CellPainting;

    /// <summary>
    /// Raised when a cell parsing event occurs - placeholder for compatibility.
    /// </summary>
    [Obsolete(" this event is not implemented， develop independently")]
    public event DataGridViewCellParsingEventHandler CellParsing;

    /// <summary>
    /// Raised when cell state changes - placeholder for compatibility.
    /// </summary>
    [Obsolete(" this event is not implemented， develop independently")]
    public event DataGridViewCellStateChangedEventHandler CellStateChanged;

    /// <summary>
    /// Raised when a cell's style changes - placeholder for compatibility.
    /// </summary>
    [Obsolete(" this event is not implemented， develop independently")]
    public event DataGridViewCellEventHandler CellStyleChanged;

    /// <summary>
    /// Raised when the cell style content changes - placeholder for compatibility.
    /// </summary>
    [Obsolete(" this event is not implemented， develop independently")]
    public event DataGridViewCellStyleContentChangedEventHandler CellStyleContentChanged;

    /// <summary>
    /// Raised when cell tooltip text changes - placeholder for compatibility.
    /// </summary>
    [Obsolete(" this event is not implemented， develop independently")]
    public event DataGridViewCellEventHandler CellToolTipTextChanged;

    /// <summary>
    /// Raised when a cell tooltip is needed - placeholder for compatibility.
    /// </summary>
    [Obsolete(" this event is not implemented， develop independently")]
    public event DataGridViewCellToolTipTextNeededEventHandler CellToolTipTextNeeded;

    /// <summary>
    /// Raised when a cell is validated - placeholder for compatibility.
    /// </summary>
    [Obsolete(" this event is not implemented， develop independently")]
    public event DataGridViewCellEventHandler CellValidated;

    /// <summary>
    /// Raised when a cell is validating - placeholder for compatibility.
    /// </summary>
    //[Obsolete(" this event is not implemented， develop independently")]
    //public event DataGridViewCellEventHandler CellValueChanged;
    [Obsolete(" this event is not implemented， develop independently")]
    public event DataGridViewCellValidatingEventHandler CellValidating;

    /// <summary>
    /// Raised when a cell value changes.
    /// </summary>
    public event DataGridViewCellEventHandler CellValueChanged;

    /// <summary>
    /// Raised when a cell value is required - placeholder for virtual-mode support.
    /// </summary>
    [Obsolete(" this event is not implemented， develop independently")]
    public event DataGridViewCellValueEventHandler CellValueNeeded;

    /// <summary>
    /// Raised when a cell value is pushed back to the data cache - placeholder for virtual-mode support.
    /// </summary>
    [Obsolete(" this event is not implemented， develop independently")]
    public event DataGridViewCellValueEventHandler CellValuePushed;

    /// <summary>
    /// Raised when a column is added - placeholder for compatibility.
    /// </summary>
    [Obsolete(" this event is not implemented， develop independently")]
    public event DataGridViewColumnEventHandler ColumnAdded;

    /// <summary>
    /// Raised when a column's context menu strip changes - placeholder for compatibility.
    /// </summary>
    [Obsolete(" this event is not implemented， develop independently")]
    public event DataGridViewColumnEventHandler ColumnContextMenuStripChanged;

    /// <summary>
    /// Raised when a column's data property name changes - placeholder for compatibility.
    /// </summary>
    [Obsolete(" this event is not implemented， develop independently")]
    public event DataGridViewColumnEventHandler ColumnDataPropertyNameChanged;

    /// <summary>
    /// Raised when a column's default cell style changes - placeholder for compatibility.
    /// </summary>
    [Obsolete(" this event is not implemented， develop independently")]
    public event DataGridViewColumnEventHandler ColumnDefaultCellStyleChanged;

    /// <summary>
    /// Raised when a column's display index changes - placeholder for compatibility.
    /// </summary>
    [Obsolete(" this event is not implemented， develop independently")]
    public event DataGridViewColumnEventHandler ColumnDisplayIndexChanged;

    /// <summary>
    /// Raised when a column divider is double-clicked - placeholder for compatibility.
    /// </summary>
    [Obsolete(" this event is not implemented， develop independently")]
    public event DataGridViewColumnDividerDoubleClickEventHandler ColumnDividerDoubleClick;

    /// <summary>
    /// Raised when a column divider width changes - placeholder for compatibility.
    /// </summary>
    [Obsolete(" this event is not implemented， develop independently")]
    public event DataGridViewColumnEventHandler ColumnDividerWidthChanged;

    /// <summary>
    /// Raised when a column header cell changes - placeholder for compatibility.
    /// </summary>
    [Obsolete(" this event is not implemented， develop independently")]
    public event DataGridViewColumnEventHandler ColumnHeaderCellChanged;

    /// <summary>
    /// Raised for column header mouse click events - placeholder for compatibility.
    /// </summary>
    [Obsolete(" this event is not implemented， develop independently")]
    public event DataGridViewCellMouseEventHandler ColumnHeaderMouseClick;

    /// <summary>
    /// Raised for column header mouse double-click events - placeholder for compatibility.
    /// </summary>
    [Obsolete(" this event is not implemented， develop independently")]
    public event DataGridViewCellMouseEventHandler ColumnHeaderMouseDoubleClick;

    /// <summary>
    /// Raised when column headers border style changes - placeholder for compatibility.
    /// </summary>
    [Obsolete(" this event is not implemented， develop independently")]
    public event EventHandler ColumnHeadersBorderStyleChanged;

    /// <summary>
    /// Raised when column headers default cell style changes - placeholder for compatibility.
    /// </summary>
    [Obsolete(" this event is not implemented， develop independently")]
    public event EventHandler ColumnHeadersDefaultCellStyleChanged;

    /// <summary>
    /// Raised when column headers height changes - placeholder for compatibility.
    /// </summary>
    [Obsolete(" this event is not implemented， develop independently")]
    public event EventHandler ColumnHeadersHeightChanged;

    /// <summary>
    /// Raised when the column headers height size mode changes - placeholder for compatibility.
    /// </summary>
    [Obsolete(" this event is not implemented， develop independently")]
    public event DataGridViewAutoSizeModeEventHandler ColumnHeadersHeightSizeModeChanged;

    /// <summary>
    /// Raised when a column's minimum width changes - placeholder for compatibility.
    /// </summary>
    [Obsolete(" this event is not implemented， develop independently")]
    public event DataGridViewColumnEventHandler ColumnMinimumWidthChanged;

    /// <summary>
    /// Raised when a column's name changes - placeholder for compatibility.
    /// </summary>
    [Obsolete(" this event is not implemented， develop independently")]
    public event DataGridViewColumnEventHandler ColumnNameChanged;

    /// <summary>
    /// Raised when a column is removed - placeholder for compatibility.
    /// </summary>
    [Obsolete(" this event is not implemented， develop independently")]
    public event DataGridViewColumnEventHandler ColumnRemoved;

    /// <summary>
    /// Raised when a column's sort mode changes - placeholder for compatibility.
    /// </summary>
    [Obsolete(" this event is not implemented， develop independently")]
    public event DataGridViewColumnEventHandler ColumnSortModeChanged;

    /// <summary>
    /// Raised when a column state changes - placeholder for compatibility.
    /// </summary>
    [Obsolete(" this event is not implemented， develop independently")]
    public event DataGridViewColumnStateChangedEventHandler ColumnStateChanged;

    /// <summary>
    /// Raised when a column's tooltip text changes - placeholder for compatibility.
    /// </summary>
    [Obsolete(" this event is not implemented， develop independently")]
    public event DataGridViewColumnEventHandler ColumnToolTipTextChanged;

    /// <summary>
    /// Raised when a column width changes - placeholder for compatibility.
    /// </summary>
    [Obsolete(" this event is not implemented， develop independently")]
    public event DataGridViewColumnEventHandler ColumnWidthChanged;

    /// <summary>
    /// Raised when the current cell changes - placeholder for compatibility.
    /// </summary>
    [Obsolete(" this event is not implemented， develop independently")]
    public event EventHandler CurrentCellChanged;

    /// <summary>
    /// Raised when the current cell dirty state changes - placeholder for compatibility.
    /// </summary>
    [Obsolete(" this event is not implemented， develop independently")]
    public event EventHandler CurrentCellDirtyStateChanged;

    /// <summary>
    /// Raised when data binding completes - placeholder for compatibility.
    /// </summary>
    [Obsolete(" this event is not implemented， develop independently")]
    public event DataGridViewBindingCompleteEventHandler DataBindingComplete;

    /// <summary>
    /// Raised when a data error occurs - placeholder for compatibility.
    /// </summary>
    [Obsolete(" this event is not implemented， develop independently")]
    public event DataGridViewDataErrorEventHandler DataError;

    /// <summary>
    /// Raised when DataMember changes - placeholder for compatibility.
    /// </summary>
    [Obsolete(" this event is not implemented， develop independently")]
    public event EventHandler DataMemberChanged;

    /// <summary>
    /// Raised when DataSource changes - placeholder for compatibility.
    /// </summary>
    [Obsolete(" this event is not implemented， develop independently")]
    public event EventHandler DataSourceChanged;

    /// <summary>
    /// Raised when default cell style changes - placeholder for compatibility.
    /// </summary>
    [Obsolete(" this event is not implemented， develop independently")]
    public event EventHandler DefaultCellStyleChanged;

    /// <summary>
    /// Raised when default values are needed for new rows - placeholder for compatibility.
    /// </summary>
    [Obsolete(" this event is not implemented， develop independently")]
    public event DataGridViewRowEventHandler DefaultValuesNeeded;

    /// <summary>
    /// Raised when an editing control is shown - placeholder for compatibility.
    /// </summary>
    [Obsolete(" this event is not implemented， develop independently")]
    public event DataGridViewEditingControlShowingEventHandler EditingControlShowing;

    /// <summary>
    /// Raised when edit mode changes - placeholder for compatibility.
    /// </summary>
    //[Obsolete(" this event is not implemented， develop independently")]
    //public event EventHandler FontChanged;
    //[Obsolete(" this event is not implemented， develop independently")]
    //public event EventHandler ForeColorChanged;
    [Obsolete(" this event is not implemented， develop independently")]
    public event EventHandler EditModeChanged;

    /// <summary>
    /// Raised when grid color changes - placeholder for compatibility.
    /// </summary>
    [Obsolete(" this event is not implemented， develop independently")]
    public event EventHandler GridColorChanged;

    /// <summary>
    /// Raised when MultiSelect changes - placeholder for compatibility.
    /// </summary>
    [Obsolete(" this event is not implemented， this event is not implemented， develop independently")]
    public event EventHandler MultiSelectChanged;

    /// <summary>
    /// Raised when new rows are required - placeholder for compatibility.
    /// </summary>
    [Obsolete(" this event is not implemented， develop independently")]
    public event DataGridViewRowEventHandler NewRowNeeded;

    /// <summary>
    /// Raised when ReadOnly mode changes - placeholder for compatibility.
    /// </summary>
    [Obsolete(" this event is not implemented， develop independently")]
    public event EventHandler ReadOnlyChanged;

    /// <summary>
    /// Raised when row context menu strip changes - placeholder for compatibility.
    /// </summary>
    [Obsolete(" this event is not implemented， develop independently")]
    public event DataGridViewRowEventHandler RowContextMenuStripChanged;

    /// <summary>
    /// Raised when a row context menu strip is needed - placeholder for compatibility.
    /// </summary>
    [Obsolete(" this event is not implemented， develop independently")]
    public event DataGridViewRowContextMenuStripNeededEventHandler RowContextMenuStripNeeded;

    /// <summary>
    /// Raised when a row's default style changes - placeholder for compatibility.
    /// </summary>
    [Obsolete(" this event is not implemented， develop independently")]
    public event DataGridViewRowEventHandler RowDefaultCellStyleChanged;

    /// <summary>
    /// Raised when a row's dirty state is needed - placeholder for compatibility.
    /// </summary>
    [Obsolete(" this event is not implemented， develop independently")]
    public event QuestionEventHandler RowDirtyStateNeeded;

    /// <summary>
    /// Raised when a row divider is double-clicked - placeholder for compatibility.
    /// </summary>
    [Obsolete(" this event is not implemented， develop independently")]
    public event DataGridViewRowDividerDoubleClickEventHandler RowDividerDoubleClick;

    /// <summary>
    /// Raised when a row divider height changes - placeholder for compatibility.
    /// </summary>
    [Obsolete(" this event is not implemented， develop independently")]
    public event DataGridViewRowEventHandler RowDividerHeightChanged;

    /// <summary>
    /// Raised when entering a row - placeholder for compatibility.
    /// </summary>
    [Obsolete(" this event is not implemented， develop independently")]
    public event DataGridViewCellEventHandler RowEnter;

    /// <summary>
    /// Raised when a row error text changes - placeholder for compatibility.
    /// </summary>
    [Obsolete(" this event is not implemented， develop independently")]
    public event DataGridViewRowEventHandler RowErrorTextChanged;

    /// <summary>
    /// Raised when a row's error text is needed - placeholder for compatibility.
    /// </summary>
    [Obsolete(" this event is not implemented， develop independently")]
    public event DataGridViewRowErrorTextNeededEventHandler RowErrorTextNeeded;

    /// <summary>
    /// Raised when the row header cell changes - placeholder for compatibility.
    /// </summary>
    [Obsolete(" this event is not implemented， develop independently")]
    public event DataGridViewRowEventHandler RowHeaderCellChanged;

    /// <summary>
    /// Raised for row header mouse click events - placeholder for compatibility.
    /// </summary>
    [Obsolete(" this event is not implemented， develop independently")]
    public event DataGridViewCellMouseEventHandler RowHeaderMouseClick;

    /// <summary>
    /// Raised for row header mouse double-click events - placeholder for compatibility.
    /// </summary>
    [Obsolete(" this event is not implemented， develop independently")]
    public event DataGridViewCellMouseEventHandler RowHeaderMouseDoubleClick;

    /// <summary>
    /// Raised when row headers border style changes - placeholder for compatibility.
    /// </summary>
    [Obsolete(" this event is not implemented， develop independently")]
    public event EventHandler RowHeadersBorderStyleChanged;

    /// <summary>
    /// Raised when row headers default cell style changes - placeholder for compatibility.
    /// </summary>
    [Obsolete(" this event is not implemented， develop independently")]
    public event EventHandler RowHeadersDefaultCellStyleChanged;

    /// <summary>
    /// Raised when row headers width changes - placeholder for compatibility.
    /// </summary>
    [Obsolete(" this event is not implemented， develop independently")]
    public event EventHandler RowHeadersWidthChanged;

    /// <summary>
    /// Raised when row headers width size mode changes - placeholder for compatibility.
    /// </summary>
    [Obsolete(" this event is not implemented， develop independently")]
    public event DataGridViewAutoSizeModeEventHandler RowHeadersWidthSizeModeChanged;

    /// <summary>
    /// Raised when row height changes - placeholder for compatibility.
    /// </summary>
    [Obsolete(" this event is not implemented， develop independently")]
    public event DataGridViewRowEventHandler RowHeightChanged;

    /// <summary>
    /// Raised when row height info is needed - placeholder for compatibility.
    /// </summary>
    [Obsolete(" this event is not implemented， develop independently")]
    public event DataGridViewRowHeightInfoNeededEventHandler RowHeightInfoNeeded;

    /// <summary>
    /// Raised when row height info is pushed - placeholder for compatibility.
    /// </summary>
    [Obsolete(" this event is not implemented， develop independently")]
    public event DataGridViewRowHeightInfoPushedEventHandler RowHeightInfoPushed;

    /// <summary>
    /// Raised when a row loses focus - placeholder for compatibility.
    /// </summary>
    [Obsolete(" this event is not implemented， develop independently")]
    public event DataGridViewCellEventHandler RowLeave;

    /// <summary>
    /// Raised when the minimum height of a row changes - placeholder for compatibility.
    /// </summary>
    [Obsolete(" this event is not implemented， develop independently")]
    public event DataGridViewRowEventHandler RowMinimumHeightChanged;

    /// <summary>
    /// Raised after painting a row - placeholder for compatibility.
    /// </summary>
    [Obsolete(" this event is not implemented， develop independently")]
    public event DataGridViewRowPostPaintEventHandler RowPostPaint;

    /// <summary>
    /// Raised before painting a row - placeholder for compatibility.
    /// </summary>
    [Obsolete(" this event is not implemented， develop independently")]
    public event DataGridViewRowPrePaintEventHandler RowPrePaint;

    /// <summary>
    /// Raised when rows are added.
    /// </summary>
    [Obsolete(" this event is not implemented， develop independently")]
    public event DataGridViewRowsAddedEventHandler RowsAdded;

    /// <summary>
    /// Raised when rows default cell style changes - placeholder for compatibility.
    /// </summary>
    //[Obsolete(" this event is not implemented， develop independently")]
    //public event EventHandler TextChanged;
    [Obsolete(" this event is not implemented， develop independently")]
    public event EventHandler RowsDefaultCellStyleChanged;

    /// <summary>
    /// Raised when rows are removed.
    /// </summary>
    [Obsolete(" this event is not implemented， develop independently")]
    public event DataGridViewRowsRemovedEventHandler RowsRemoved;

    /// <summary>
    /// Raised when the row state changes - placeholder for compatibility.
    /// </summary>
    [Obsolete(" this event is not implemented， develop independently")]
    public event DataGridViewRowStateChangedEventHandler RowStateChanged;

    /// <summary>
    /// Raised when a row is unshared - placeholder for compatibility.
    /// </summary>
    [Obsolete(" this event is not implemented， develop independently")]
    public event DataGridViewRowEventHandler RowUnshared;

    /// <summary>
    /// Raised when a row is validated - placeholder for compatibility.
    /// </summary>
    [Obsolete(" this event is not implemented， develop independently")]
    public event DataGridViewCellEventHandler RowValidated;

    /// <summary>
    /// Raised when a row is validating - placeholder for compatibility.
    /// </summary>
    [Obsolete(" this event is not implemented， develop independently")]
    public event DataGridViewCellCancelEventHandler RowValidating;

    /// <summary>
    /// Raised when selection changes.
    /// </summary>
    public event EventHandler SelectionChanged;

    /// <summary>
    /// Raised when sort comparison is requested - placeholder for compatibility.
    /// </summary>
    [Obsolete(" this event is not implemented， develop independently")]
    public event DataGridViewSortCompareEventHandler SortCompare;

    /// <summary>
    /// Raised when the grid is sorted - placeholder for compatibility.
    /// </summary>
    [Obsolete(" this event is not implemented， develop independently")]
    public event EventHandler Sorted;

    /// <summary>
    /// Raised when the user adds a row - placeholder for compatibility.
    /// </summary>
    [Obsolete(" this event is not implemented， develop independently")]
    public event DataGridViewRowEventHandler UserAddedRow;

    /// <summary>
    /// Raised when the user deletes a row - placeholder for compatibility.
    /// </summary>
    [Obsolete(" this event is not implemented， develop independently")]
    public event DataGridViewRowEventHandler UserDeletedRow;

    /// <summary>
    /// Raised when a user is deleting a row - placeholder for compatibility.
    /// </summary>
    [Obsolete(" this event is not implemented， develop independently")]
    public event DataGridViewRowCancelEventHandler UserDeletingRow;

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
    public DataGridViewCellStyle AlternatingRowsDefaultCellStyle { get; set; }

    /// <summary>
    /// Gets or sets the mode in which row heights are automatically determined.
    /// </summary>
    public DataGridViewAutoSizeRowsMode AutoSizeRowsMode { get; set; }

    /// <summary>
    /// Gets or sets the style applied to column headers.
    /// </summary>
    public DataGridViewCellStyle ColumnHeadersDefaultCellStyle { get; set; }

    /// <summary>
    /// Gets or sets the height, in pixels, of the column headers.
    /// </summary>
    public int ColumnHeadersHeight { get; set; }

    /// <summary>
    /// Gets or sets the sizing mode for column header height.
    /// </summary>
    public DataGridViewColumnHeadersHeightSizeMode ColumnHeadersHeightSizeMode
    {
        get; set;
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
    public string DataMember { get; set; }

    /// <summary>
    /// Gets or sets the data source for the grid. Accepts DataTable, DataSet, DataView, or IEnumerable.
    /// When set and the control is visible, data is loaded into the grid.
    /// </summary>
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

    /// <summary>
    /// Gets or sets the default cell style for the grid.
    /// </summary>
    public DataGridViewCellStyle DefaultCellStyle { get; set; }

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
    public string Markup { get; set; } = "...";

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
    public bool ReadOnly { get; set; }

    /// <summary>
    /// Gets or sets the default style for row headers.
    /// </summary>
    public DataGridViewCellStyle RowHeadersDefaultCellStyle { get; set; }

    /// <summary>
    /// Gets or sets the width, in pixels, of the row header column.
    /// </summary>
    public int RowHeadersWidth { get; set; }

    /// <summary>
    /// Gets the collection of rows in the grid.
    /// </summary>
    public DataGridViewRowCollection Rows { get { return _rows; } }

    /// <summary>
    /// Gets or sets the default style applied to all rows.
    /// </summary>
    public DataGridViewCellStyle RowsDefaultCellStyle { get; set; }

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
                        int cols = Store.NColumns;
                        Store.Foreach(new TreeModelForeachFunc((model, path, iter) =>
                        {
                            for (int i = 0; i < cols; i++)
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
                    foreach (int columnIndex in _selectedBandIndexes)
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
                    foreach (int rowIndex in _selectedBandIndexes)
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
        CellValueChanged?.Invoke(this, new DataGridViewCellEventArgs(column, row));
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
            var column = args.Column as DataGridViewColumn;
            CellClick(this, new DataGridViewCellEventArgs(column.Index, args.Path.Indices.Last()));
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
        var _type = _DataSource.GetType();
        var _entityType = _type.GetGenericArguments();
        if (_entityType.Length == 1)
        {
            var pros = _entityType[0].GetProperties(BindingFlags.Public|BindingFlags.Instance);
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
                var reader = ((IEnumerable)_DataSource).GetEnumerator();
                while (reader.MoveNext())
                {
                    var obj = reader.Current;
                    var type = obj.GetType();
                    var newRow = new DataGridViewRow();
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
        Binding binding = DataBindings[args.Property];
        binding.WriteValue();
    }

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

}