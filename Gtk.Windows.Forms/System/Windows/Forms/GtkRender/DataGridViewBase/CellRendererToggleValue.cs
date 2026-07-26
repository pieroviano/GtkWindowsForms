/*
 * Based on GTK components, it is a cross-platform interface component that is compatible with the native C# control winform interface.
 * Use this component GtkSystem.Windows.Forms instead of Microsoft.WindowsDesktop.App.WindowsForms, compile once, and run across Windows, Linux, and macOS
 * Technical support 438865652@qq.com，https://www.gtkapp.com, https://gitee.com/easywebfactory, https://github.com/easywebfactory438865652@qq.com，https://www.gtkapp.com, https://gitee.com/easywebfactory, https://github.com/easywebfactory
 * author:chenhongjin
 */

using GLib;
using Gtk;

namespace System.Windows.Forms.GtkRender;

/// <summary>
/// Class CellRendererToggleValue. Implements the
/// <see cref="Gtk.CellRendererToggle" /> Implements the
/// <see cref="T:System.Windows.Forms.GtkRender.ICellRenderer" />
/// </summary>
/// <seealso cref="Gtk.CellRendererToggle" />
/// <seealso cref="T:System.Windows.Forms.GtkRender.ICellRenderer" />
public class CellRendererToggleValue : CellRendererToggle, ICellRenderer
{
    DataGridViewColumn Column;

    int ColumnIndex;

    private CellReandererUtility utility;

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.GtkRender.CellRendererToggleValue" /> class.
    /// </summary>
    /// <param name="view">
    /// The view.
    /// </param>
    public CellRendererToggleValue(DataGridViewColumn view)
    {
        Column = view;
        ColumnIndex = view.Index;
        utility = new CellReandererUtility();
    }

    static CellRendererToggleValue()
    {
        typeof(CellRendererToggleValue).GetMembers();
    }

    /// <summary>
    /// Sets the cell value.
    /// </summary>
    /// <value>
    /// The cell value.
    /// </value>
    [Property("cellvalue")]
    public DataGridViewRow CellValue
    {
        set
        {
            DataGridViewCell cell = value.Cells[Column.Index];
            this.Active = (cell.Value?.ToString() == "True" || cell.Value?.ToString() == "true" || cell.Value?.ToString() == "1");
            var style = utility.GetStyle(this.Column, cell);
            utility.SetControlWithStyle(this, style);
        }
    }

    /// <summary>
    /// Gets or sets the column style.
    /// </summary>
    /// <value>
    /// The column style.
    /// </value>
    public DataGridViewCellStyle? ColumnStyle
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets a value indicating whether this
    /// <see cref="T:System.Windows.Forms.GtkRender.CellRendererToggleValue" /> is editable.
    /// </summary>
    /// <value>
    /// <c>
    /// true
    /// </c> if editable; otherwise,
    /// <c>
    /// false
    /// </c>.
    /// </value>
    public bool Editable
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the markup.
    /// </summary>
    /// <value>
    /// The markup.
    /// </value>
    public string? Markup
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the width chars.
    /// </summary>
    /// <value>
    /// The width chars.
    /// </value>
    public int WidthChars
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the wrap mode.
    /// </summary>
    /// <value>
    /// The wrap mode.
    /// </value>
    public Pango.WrapMode WrapMode
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the width of the wrap.
    /// </summary>
    /// <value>
    /// The width of the wrap.
    /// </value>
    public int WrapWidth
    {
        get;
        set;
    }
}