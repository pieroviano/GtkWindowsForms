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
    /// <summary>
    /// Displays data in a customizable grid.
    /// </summary>
    /// <filterpriority>
    /// 1
    /// </filterpriority>
    [DesignerCategory("Component")]
    public partial class DataGridView : ScrollableControl
    {

        public readonly DataGridViewBase self = new DataGridViewBase();

        internal Gtk.TreeStore Store = new TreeStore(typeof(DataGridViewCell));

        private ControlBindingsCollection _collect;

        private DataGridViewColumnCollection _columns;

        private object _DataSource;

        private bool _MultiSelect = true;

        private DataGridViewRowCollection _rows;

        private DataGridViewRow _RowTemplate;

        private List<int> _selectedBandIndexes = new List<int>();

        private DataGridViewSelectionMode _SelectionMode = DataGridViewSelectionMode.RowHeaderSelect;
        [Obsolete(" this event is not implemented， develop independently")]

        public event EventHandler AllowUserToAddRowsChanged;

        [Obsolete(" this event is not implemented， develop independently")]

        public event EventHandler AllowUserToDeleteRowsChanged;

        //[Obsolete(" this event is not implemented， develop independently")]
        //public event EventHandler SelectionChanged;
        [Obsolete(" this event is not implemented， develop independently")]
        public event EventHandler AllowUserToOrderColumnsChanged;

        [Obsolete(" this event is not implemented， develop independently")]

        public event EventHandler AllowUserToResizeColumnsChanged;

        [Obsolete(" this event is not implemented， develop independently")]

        public event EventHandler AllowUserToResizeRowsChanged;

        [Obsolete(" this event is not implemented， develop independently")]

        public event EventHandler AlternatingRowsDefaultCellStyleChanged;

        [Obsolete(" this event is not implemented， develop independently")]

        public event EventHandler AutoGenerateColumnsChanged;

        [Obsolete(" this event is not implemented， develop independently")]
        public event DataGridViewAutoSizeColumnModeEventHandler AutoSizeColumnModeChanged;

        [Obsolete(" this event is not implemented， develop independently")]

        public event DataGridViewAutoSizeColumnsModeEventHandler AutoSizeColumnsModeChanged;

        //[Obsolete(" this event is not implemented， develop independently")]
        //public event EventHandler BackColorChanged;
        [Obsolete(" this event is not implemented， develop independently")]

        public event DataGridViewAutoSizeModeEventHandler AutoSizeRowsModeChanged;

        //[Obsolete(" this event is not implemented， develop independently")]
        //public event EventHandler BackgroundImageChanged;
        [Obsolete(" this event is not implemented， develop independently")]
        public event EventHandler BackgroundColorChanged;

        [Obsolete(" this event is not implemented， develop independently")]

        public event EventHandler BorderStyleChanged;

        [Obsolete(" this event is not implemented， develop independently")]

        public event QuestionEventHandler CancelRowEdit;

        //[Obsolete(" this event is not implemented， develop independently")]
        //public event EventHandler PaddingChanged;
        [Obsolete(" this event is not implemented， develop independently")]

        public event DataGridViewCellCancelEventHandler CellBeginEdit;

        //[Obsolete(" this event is not implemented， develop independently")]
        //public event EventHandler StyleChanged;
        //[Obsolete(" this event is not implemented， develop independently")]
        //public event EventHandler BackgroundImageLayoutChanged;
        [Obsolete(" this event is not implemented， develop independently")]

        public event EventHandler CellBorderStyleChanged;

        public event DataGridViewCellEventHandler CellClick;

        //[Obsolete(" this event is not implemented， develop independently")]
        //public event DataGridViewCellEventHandler CellClick;
        [Obsolete(" this event is not implemented， develop independently")]

        public event DataGridViewCellEventHandler CellContentClick;

        [Obsolete(" this event is not implemented， develop independently")]

        public event DataGridViewCellEventHandler CellContentDoubleClick;

        [Obsolete(" this event is not implemented， develop independently")]

        public event DataGridViewCellEventHandler CellContextMenuStripChanged;

        [Obsolete(" this event is not implemented， develop independently")]

        public event DataGridViewCellContextMenuStripNeededEventHandler CellContextMenuStripNeeded;

        [Obsolete(" this event is not implemented， develop independently")]

        public event DataGridViewCellEventHandler CellDoubleClick;

        [Obsolete(" this event is not implemented， develop independently")]

        public event DataGridViewCellEventHandler CellEndEdit;

        [Obsolete(" this event is not implemented， develop independently")]

        public event DataGridViewCellEventHandler CellEnter;

        [Obsolete(" this event is not implemented， develop independently")]

        public event DataGridViewCellEventHandler CellErrorTextChanged;

        [Obsolete(" this event is not implemented， develop independently")]

        public event DataGridViewCellErrorTextNeededEventHandler CellErrorTextNeeded;

        [Obsolete(" this event is not implemented， develop independently")]

        public event DataGridViewCellFormattingEventHandler CellFormatting;

        [Obsolete(" this event is not implemented， develop independently")]

        public event DataGridViewCellEventHandler CellLeave;

        [Obsolete(" this event is not implemented， develop independently")]

        public event DataGridViewCellMouseEventHandler CellMouseClick;

        [Obsolete(" this event is not implemented， develop independently")]

        public event DataGridViewCellMouseEventHandler CellMouseDoubleClick;

        [Obsolete(" this event is not implemented， develop independently")]

        public event DataGridViewCellMouseEventHandler CellMouseDown;

        [Obsolete(" this event is not implemented， develop independently")]

        public event DataGridViewCellEventHandler CellMouseEnter;

        [Obsolete(" this event is not implemented， develop independently")]

        public event DataGridViewCellEventHandler CellMouseLeave;

        [Obsolete(" this event is not implemented， develop independently")]

        public event DataGridViewCellMouseEventHandler CellMouseMove;

        [Obsolete(" this event is not implemented， develop independently")]

        public event DataGridViewCellMouseEventHandler CellMouseUp;

        public event DataGridViewCellPaintingEventHandler CellPainting;

        [Obsolete(" this event is not implemented， develop independently")]

        public event DataGridViewCellParsingEventHandler CellParsing;

        [Obsolete(" this event is not implemented， develop independently")]

        public event DataGridViewCellStateChangedEventHandler CellStateChanged;

        [Obsolete(" this event is not implemented， develop independently")]

        public event DataGridViewCellEventHandler CellStyleChanged;

        [Obsolete(" this event is not implemented， develop independently")]

        public event DataGridViewCellStyleContentChangedEventHandler CellStyleContentChanged;

        [Obsolete(" this event is not implemented， develop independently")]

        public event DataGridViewCellEventHandler CellToolTipTextChanged;

        [Obsolete(" this event is not implemented， develop independently")]

        public event DataGridViewCellToolTipTextNeededEventHandler CellToolTipTextNeeded;

        [Obsolete(" this event is not implemented， develop independently")]

        public event DataGridViewCellEventHandler CellValidated;

        //[Obsolete(" this event is not implemented， develop independently")]
        //public event DataGridViewCellEventHandler CellValueChanged;
        [Obsolete(" this event is not implemented， develop independently")]

        public event DataGridViewCellValidatingEventHandler CellValidating;

        public event DataGridViewCellEventHandler CellValueChanged;

        [Obsolete(" this event is not implemented， develop independently")]

        public event DataGridViewCellValueEventHandler CellValueNeeded;

        [Obsolete(" this event is not implemented， develop independently")]

        public event DataGridViewCellValueEventHandler CellValuePushed;

        [Obsolete(" this event is not implemented， develop independently")]

        public event DataGridViewColumnEventHandler ColumnAdded;

        [Obsolete(" this event is not implemented， develop independently")]

        public event DataGridViewColumnEventHandler ColumnContextMenuStripChanged;

        [Obsolete(" this event is not implemented， develop independently")]

        public event DataGridViewColumnEventHandler ColumnDataPropertyNameChanged;

        [Obsolete(" this event is not implemented， develop independently")]

        public event DataGridViewColumnEventHandler ColumnDefaultCellStyleChanged;

        [Obsolete(" this event is not implemented， develop independently")]

        public event DataGridViewColumnEventHandler ColumnDisplayIndexChanged;

        [Obsolete(" this event is not implemented， develop independently")]

        public event DataGridViewColumnDividerDoubleClickEventHandler ColumnDividerDoubleClick;

        [Obsolete(" this event is not implemented， develop independently")]

        public event DataGridViewColumnEventHandler ColumnDividerWidthChanged;

        [Obsolete(" this event is not implemented， develop independently")]

        public event DataGridViewColumnEventHandler ColumnHeaderCellChanged;

        [Obsolete(" this event is not implemented， develop independently")]

        public event DataGridViewCellMouseEventHandler ColumnHeaderMouseClick;

        [Obsolete(" this event is not implemented， develop independently")]

        public event DataGridViewCellMouseEventHandler ColumnHeaderMouseDoubleClick;

        [Obsolete(" this event is not implemented， develop independently")]

        public event EventHandler ColumnHeadersBorderStyleChanged;

        [Obsolete(" this event is not implemented， develop independently")]

        public event EventHandler ColumnHeadersDefaultCellStyleChanged;

        [Obsolete(" this event is not implemented， develop independently")]

        public event EventHandler ColumnHeadersHeightChanged;

        [Obsolete(" this event is not implemented， develop independently")]

        public event DataGridViewAutoSizeModeEventHandler ColumnHeadersHeightSizeModeChanged;

        [Obsolete(" this event is not implemented， develop independently")]

        public event DataGridViewColumnEventHandler ColumnMinimumWidthChanged;

        [Obsolete(" this event is not implemented， develop independently")]

        public event DataGridViewColumnEventHandler ColumnNameChanged;

        [Obsolete(" this event is not implemented， develop independently")]

        public event DataGridViewColumnEventHandler ColumnRemoved;

        [Obsolete(" this event is not implemented， develop independently")]

        public event DataGridViewColumnEventHandler ColumnSortModeChanged;

        [Obsolete(" this event is not implemented， develop independently")]

        public event DataGridViewColumnStateChangedEventHandler ColumnStateChanged;

        [Obsolete(" this event is not implemented， develop independently")]

        public event DataGridViewColumnEventHandler ColumnToolTipTextChanged;

        [Obsolete(" this event is not implemented， develop independently")]

        public event DataGridViewColumnEventHandler ColumnWidthChanged;

        [Obsolete(" this event is not implemented， develop independently")]

        public event EventHandler CurrentCellChanged;

        [Obsolete(" this event is not implemented， develop independently")]

        public event EventHandler CurrentCellDirtyStateChanged;

        [Obsolete(" this event is not implemented， develop independently")]

        public event DataGridViewBindingCompleteEventHandler DataBindingComplete;

        [Obsolete(" this event is not implemented， develop independently")]

        public event DataGridViewDataErrorEventHandler DataError;

        [Obsolete(" this event is not implemented， develop independently")]

        public event EventHandler DataMemberChanged;

        [Obsolete(" this event is not implemented， develop independently")]

        public event EventHandler DataSourceChanged;

        [Obsolete(" this event is not implemented， develop independently")]

        public event EventHandler DefaultCellStyleChanged;

        [Obsolete(" this event is not implemented， develop independently")]

        public event DataGridViewRowEventHandler DefaultValuesNeeded;

        [Obsolete(" this event is not implemented， develop independently")]

        public event DataGridViewEditingControlShowingEventHandler EditingControlShowing;

        //[Obsolete(" this event is not implemented， develop independently")]
        //public event EventHandler FontChanged;
        //[Obsolete(" this event is not implemented， develop independently")]
        //public event EventHandler ForeColorChanged;
        [Obsolete(" this event is not implemented， develop independently")]

        public event EventHandler EditModeChanged;

        [Obsolete(" this event is not implemented， develop independently")]
        public event EventHandler GridColorChanged;

        [Obsolete(" this event is not implemented， this event is not implemented， develop independently")]
        public event EventHandler MultiSelectChanged;

        [Obsolete(" this event is not implemented， develop independently")]

        public event DataGridViewRowEventHandler NewRowNeeded;

        [Obsolete(" this event is not implemented， develop independently")]
        public event EventHandler ReadOnlyChanged;

        [Obsolete(" this event is not implemented， develop independently")]

        public event DataGridViewRowEventHandler RowContextMenuStripChanged;

        [Obsolete(" this event is not implemented， develop independently")]

        public event DataGridViewRowContextMenuStripNeededEventHandler RowContextMenuStripNeeded;

        [Obsolete(" this event is not implemented， develop independently")]

        public event DataGridViewRowEventHandler RowDefaultCellStyleChanged;

        [Obsolete(" this event is not implemented， develop independently")]

        public event QuestionEventHandler RowDirtyStateNeeded;

        [Obsolete(" this event is not implemented， develop independently")]

        public event DataGridViewRowDividerDoubleClickEventHandler RowDividerDoubleClick;

        [Obsolete(" this event is not implemented， develop independently")]

        public event DataGridViewRowEventHandler RowDividerHeightChanged;

        [Obsolete(" this event is not implemented， develop independently")]

        public event DataGridViewCellEventHandler RowEnter;

        [Obsolete(" this event is not implemented， develop independently")]

        public event DataGridViewRowEventHandler RowErrorTextChanged;

        [Obsolete(" this event is not implemented， develop independently")]

        public event DataGridViewRowErrorTextNeededEventHandler RowErrorTextNeeded;

        [Obsolete(" this event is not implemented， develop independently")]
        public event DataGridViewRowEventHandler RowHeaderCellChanged;

        [Obsolete(" this event is not implemented， develop independently")]

        public event DataGridViewCellMouseEventHandler RowHeaderMouseClick;

        [Obsolete(" this event is not implemented， develop independently")]

        public event DataGridViewCellMouseEventHandler RowHeaderMouseDoubleClick;

        [Obsolete(" this event is not implemented， develop independently")]

        public event EventHandler RowHeadersBorderStyleChanged;

        [Obsolete(" this event is not implemented， develop independently")]

        public event EventHandler RowHeadersDefaultCellStyleChanged;

        [Obsolete(" this event is not implemented， develop independently")]

        public event EventHandler RowHeadersWidthChanged;

        [Obsolete(" this event is not implemented， develop independently")]

        public event DataGridViewAutoSizeModeEventHandler RowHeadersWidthSizeModeChanged;

        [Obsolete(" this event is not implemented， develop independently")]

        public event DataGridViewRowEventHandler RowHeightChanged;

        [Obsolete(" this event is not implemented， develop independently")]

        public event DataGridViewRowHeightInfoNeededEventHandler RowHeightInfoNeeded;

        [Obsolete(" this event is not implemented， develop independently")]

        public event DataGridViewRowHeightInfoPushedEventHandler RowHeightInfoPushed;

        [Obsolete(" this event is not implemented， develop independently")]

        public event DataGridViewCellEventHandler RowLeave;

        [Obsolete(" this event is not implemented， develop independently")]

        public event DataGridViewRowEventHandler RowMinimumHeightChanged;

        [Obsolete(" this event is not implemented， develop independently")]

        public event DataGridViewRowPostPaintEventHandler RowPostPaint;

        [Obsolete(" this event is not implemented， develop independently")]

        public event DataGridViewRowPrePaintEventHandler RowPrePaint;

        [Obsolete(" this event is not implemented， develop independently")]

        public event DataGridViewRowsAddedEventHandler RowsAdded;

        //[Obsolete(" this event is not implemented， develop independently")]
        //public event EventHandler TextChanged;
        [Obsolete(" this event is not implemented， develop independently")]

        public event EventHandler RowsDefaultCellStyleChanged;

        [Obsolete(" this event is not implemented， develop independently")]

        public event DataGridViewRowsRemovedEventHandler RowsRemoved;

        [Obsolete(" this event is not implemented， develop independently")]

        public event DataGridViewRowStateChangedEventHandler RowStateChanged;

        [Obsolete(" this event is not implemented， develop independently")]

        public event DataGridViewRowEventHandler RowUnshared;

        [Obsolete(" this event is not implemented， develop independently")]

        public event DataGridViewCellEventHandler RowValidated;

        [Obsolete(" this event is not implemented， develop independently")]

        public event DataGridViewCellCancelEventHandler RowValidating;

        public event EventHandler SelectionChanged;

        [Obsolete(" this event is not implemented， develop independently")]
        public event DataGridViewSortCompareEventHandler SortCompare;

        [Obsolete(" this event is not implemented， develop independently")]

        public event EventHandler Sorted;

        [Obsolete(" this event is not implemented， develop independently")]
        public event DataGridViewRowEventHandler UserAddedRow;

        [Obsolete(" this event is not implemented， develop independently")]

        public event DataGridViewRowEventHandler UserDeletedRow;

        [Obsolete(" this event is not implemented， develop independently")]

        public event DataGridViewRowCancelEventHandler UserDeletingRow;
        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="System.Windows.Forms.DataGridView" /> class.
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
        /// Gets or sets the default cell style applied to odd-numbered rows of the
        /// <see cref="System.Windows.Forms.DataGridView" />
        /// </summary>
        /// <returns>A
        /// <see cref="System.Windows.Forms.DataGridViewCellStyle" /> to apply to the odd-numbered rows.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public DataGridViewCellStyle AlternatingRowsDefaultCellStyle { get; set; }

        /// <summary>
        /// Gets or sets a value indicating how row heights are determined.
        /// </summary>
        /// <returns>A
        /// <see cref="System.Windows.Forms.DataGridViewAutoSizeRowsMode" /> value indicating the sizing mode. The default is
        /// <see cref="System.Windows.Forms.DataGridViewAutoSizeRowsMode.None" />
        /// </returns>
        /// <exception cref="System.ComponentModel.InvalidEnumArgumentException">The specified value when setting this property is not a valid
        /// <see cref="System.Windows.Forms.DataGridViewAutoSizeRowsMode" /> value.
        /// </exception>
        /// <exception cref="System.InvalidOperationException">The specified value when setting this property is
        /// <see cref="System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllHeaders" /> or
        /// <see cref="System.Windows.Forms.DataGridViewAutoSizeRowsMode.DisplayedHeaders" /> and row headers are hidden.
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public DataGridViewAutoSizeRowsMode AutoSizeRowsMode { get; set; }

        /// <summary>
        /// Gets or sets the default column header style.
        /// </summary>
        /// <returns>A
        /// <see cref="System.Windows.Forms.DataGridViewCellStyle" /> that represents the default column header style.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public DataGridViewCellStyle ColumnHeadersDefaultCellStyle { get; set; }

        /// <summary>
        /// Gets or sets the height, in pixels, of the column headers row
        /// </summary>
        /// <returns>The height, in pixels, of the row that contains the column headers. The default is 23.
        /// </returns>
        /// <exception cref="System.ArgumentOutOfRangeException">The specified value when setting this property is less than the minimum height of 4 pixels or is greater than the maximum height of 32768 pixels.
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        public int ColumnHeadersHeight { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the height of the column headers is adjustable and whether it can be adjusted by the user or is automatically adjusted to fit the contents of the headers.
        /// </summary>
        /// <returns>A
        /// <see cref="System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode" /> value indicating the mode by which the height of the column headers row can be adjusted. The default is
        /// <see cref="System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.EnableResizing" />
        /// </returns>
        /// <exception cref="System.ComponentModel.InvalidEnumArgumentException">The specified value when setting this property is not a valid
        /// <see cref="System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode" /> value.
        /// </exception>
        public DataGridViewColumnHeadersHeightSizeMode ColumnHeadersHeightSizeMode
        {
            get; set;
        }

        /// <summary>
        /// Gets a collection that contains all the columns in the control.
        /// </summary>
        /// <returns>The
        /// <see cref="System.Windows.Forms.DataGridViewColumnCollection" /> that contains all the columns in the
        /// <see cref="System.Windows.Forms.DataGridView" /> control.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public DataGridViewColumnCollection Columns
        {
            get
            {
                return _columns;
            }
        }

        public override ControlBindingsCollection DataBindings { get => _collect; }

        /// <summary>
        /// Gets or sets the name of the list or table in the data source for which the
        /// <see cref="System.Windows.Forms.DataGridView" /> is displaying data.
        /// </summary>
        /// <returns>The name of the table or list in the
        /// <see cref="System.Windows.Forms.DataGridView.DataSource" /> for which the
        /// <see cref="System.Windows.Forms.DataGridView" /> is displaying data. The default is
        /// <see cref="System.String.Empty" />
        /// </returns>
        /// <exception cref="System.Exception">An error occurred in the data source and either there is no handler for the
        /// <see cref="System.Windows.Forms.DataGridView.DataError" /> event or the handler has set the
        /// <see cref="System.Windows.Forms.DataGridViewDataErrorEventArgs.ThrowException" /> property to true. The exception object can typically be cast to type
        /// <see cref="System.FormatException" />
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public string DataMember { get; set; }

        /// <summary>
        /// Gets or sets the data source that the
        /// <see cref="System.Windows.Forms.DataGridView" /> is displaying data for.
        /// </summary>
        /// <returns>The object that contains data for the
        /// <see cref="System.Windows.Forms.DataGridView" /> to display.
        /// </returns>
        /// <exception cref="System.Exception">An error occurred in the data source and either there is no handler for the
        /// <see cref="System.Windows.Forms.DataGridView.DataError" /> event or the handler has set the
        /// <see cref="System.Windows.Forms.DataGridViewDataErrorEventArgs.ThrowException" /> property to true. The exception object can typically be cast to type
        /// <see cref="System.FormatException" />
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
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
        /// Gets or sets the default cell style to be applied to the cells in the
        /// <see cref="System.Windows.Forms.DataGridView" /> if no other cell style properties are set.
        /// </summary>
        /// <returns>The
        /// <see cref="System.Windows.Forms.DataGridViewCellStyle" /> to be applied as the default style.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public DataGridViewCellStyle DefaultCellStyle { get; set; }

        public Gtk.TreeView GridView { get { return self.GridView; } }

        public override object GtkControl => self;

        public string Markup { get; set; } = "...";

        /// <summary>
        /// Gets or sets a value indicating whether the user is allowed to select more than one cell, row, or column of the
        /// <see cref="System.Windows.Forms.DataGridView" /> at a time.
        /// </summary>
        /// <returns>true if the user can select more than one cell, row, or column at a time; otherwise, false. The default is true.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
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
        /// Gets or sets a value indicating whether the user can edit the cells of the
        /// <see cref="System.Windows.Forms.DataGridView" /> control.
        /// </summary>
        /// <returns>true if the user cannot edit the cells of the
        /// <see cref="System.Windows.Forms.DataGridView" /> control; otherwise, false. The default is false.
        /// </returns>
        /// <exception cref="System.InvalidOperationException">The specified value when setting this property is true, the current cell is in edit mode, and the current cell contains changes that cannot be committed.
        /// </exception>
        /// <exception cref="System.Exception">The specified value when setting this property would cause the control to enter edit mode, but initialization of the editing cell value failed and either there is no handler for the
        /// <see cref="System.Windows.Forms.DataGridView.DataError" /> event or the handler has set the
        /// <see cref="System.Windows.Forms.DataGridViewDataErrorEventArgs.ThrowException" /> property to true. The exception object can typically be cast to type
        /// <see cref="System.FormatException" />
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public bool ReadOnly { get; set; }

        /// <summary>
        /// Gets or sets the default style applied to the row header cells.
        /// </summary>
        /// <returns>The
        /// <see cref="System.Windows.Forms.DataGridViewCellStyle" /> that represents the default style applied to the row header cells.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public DataGridViewCellStyle RowHeadersDefaultCellStyle { get; set; }

        /// <summary>
        /// Gets or sets the width, in pixels, of the column that contains the row headers.
        /// </summary>
        /// <returns>The width, in pixels, of the column that contains row headers. The default is 43.
        /// </returns>
        /// <exception cref="System.ArgumentOutOfRangeException">The specified value when setting this property is less than the minimum width of 4 pixels or is greater than the maximum width of 32768 pixels.
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public int RowHeadersWidth { get; set; }

        /// <summary>
        /// Gets a collection that contains all the rows in the
        /// <see cref="System.Windows.Forms.DataGridView" /> control.
        /// </summary>
        /// <returns>A
        /// <see cref="System.Windows.Forms.DataGridViewRowCollection" /> that contains all the rows in the
        /// <see cref="System.Windows.Forms.DataGridView" />
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public DataGridViewRowCollection Rows { get { return _rows; } }

        /// <summary>
        /// Gets or sets the default style applied to the row cells of the
        /// <see cref="System.Windows.Forms.DataGridView" />
        /// </summary>
        /// <returns>The
        /// <see cref="System.Windows.Forms.DataGridViewCellStyle" /> to apply to the row cells of the
        /// <see cref="System.Windows.Forms.DataGridView" />
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public DataGridViewCellStyle RowsDefaultCellStyle { get; set; }

        /// <summary>
        /// Gets or sets the row that represents the template for all the rows in the control.
        /// </summary>
        /// <returns>A
        /// <see cref="System.Windows.Forms.DataGridViewRow" /> representing the row template.
        /// </returns>
        /// <exception cref="System.InvalidOperationException">The specified row when setting this property has its
        /// <see cref="System.Windows.Forms.DataGridViewElement.DataGridView" /> property set.
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public DataGridViewRow RowTemplate
        {
            get { return _RowTemplate ??= new DataGridViewRow(); }
            set { _RowTemplate = value; }
        }

        /// <summary>
        /// Gets the collection of cells selected by the user.
        /// </summary>
        /// <returns>A
        /// <see cref="System.Windows.Forms.DataGridViewSelectedCellCollection" /> that represents the cells selected by the user.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
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
                            Store.Foreach(new TreeModelForeachFunc((model, path, iter) =>
                            {
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

        /// <summary>
        /// Gets the collection of columns selected by the user.
        /// </summary>
        /// <returns>A
        /// <see cref="System.Windows.Forms.DataGridViewSelectedColumnCollection" /> that represents the columns selected by the user.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
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

        /// <summary>
        /// Gets the collection of rows selected by the user.
        /// </summary>
        /// <returns>A
        /// <see cref="System.Windows.Forms.DataGridViewSelectedRowCollection" /> that contains the rows selected by the user.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
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

        /// <summary>
        /// Gets or sets a value indicating how the cells of the
        /// <see cref="System.Windows.Forms.DataGridView" /> can be selected.
        /// </summary>
        /// <returns>One of the
        /// <see cref="System.Windows.Forms.DataGridViewSelectionMode" /> values. The default is
        /// <see cref="System.Windows.Forms.DataGridViewSelectionMode.RowHeaderSelect" />
        /// </returns>
        /// <exception cref="System.ComponentModel.InvalidEnumArgumentException">The specified value when setting this property is not a valid
        /// <see cref="System.Windows.Forms.DataGridViewSelectionMode" /> value.
        /// </exception>
        /// <exception cref="System.InvalidOperationException">The specified value when setting this property is
        /// <see cref="System.Windows.Forms.DataGridViewSelectionMode.FullColumnSelect" /> or
        /// <see cref="System.Windows.Forms.DataGridViewSelectionMode.ColumnHeaderSelect" /> and the
        /// <see cref="System.Windows.Forms.DataGridViewColumn.SortMode" /> property of one or more columns is set to
        /// <see cref="System.Windows.Forms.DataGridViewColumnSortMode.Automatic" />
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
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

        public override void BeginInit()
        {
            _Created = false;
        }

        /// <summary>
        /// Clears the current selection by unselecting all selected cells.
        /// </summary>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public void ClearSelection()
        {
            GridView.Selection.UnselectAll();
        }

        public override void EndInit()
        {
            _Created = true;
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

        public void OnCellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (CellPainting != null)
                CellPainting(sender, e);
        }

        public void SetCollapseRow(DataGridViewRow row)
        {
            GridView.CollapseRow(Store.GetPath(row.TreeIter));
        }

        public void SetExpandRow(DataGridViewRow row, bool all)
        {
            GridView.ExpandRow(Store.GetPath(row.TreeIter), all);
        }
        internal void CellValueChanagedHandler(int column, int row)
        {
            if (CellValueChanged != null)
            {
                CellValueChanged(this, new DataGridViewCellEventArgs(column, row));
            }
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
                DataGridViewColumn column = args.Column as DataGridViewColumn;
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
                PropertyInfo[] pros = _entityType[0].GetProperties(BindingFlags.Public | BindingFlags.Instance);
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
}
