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
/// Class CellRendererButtonValue. Implements the
/// <see cref="Gtk.CellRendererText" /> Implements the
/// <see cref="T:System.Windows.Forms.GtkRender.ICellRenderer" />
/// </summary>
/// <seealso cref="Gtk.CellRendererText" />
/// <seealso cref="T:System.Windows.Forms.GtkRender.ICellRenderer" />
public class CellRendererButtonValue : CellRendererText, ICellRenderer
{
    DataGridViewColumn Column;

    int ColumnIndex;

    private CellReandererUtility utility;

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.GtkRender.CellRendererButtonValue" /> class.
    /// </summary>
    /// <param name="view">
    /// The view.
    /// </param>
    public CellRendererButtonValue(DataGridViewColumn view)
    {
        Column = view;
        ColumnIndex = view.Index;
        utility = new CellReandererUtility();
        this.SetAlignment(0.5f, 0.5f);
        this.Ellipsize = Pango.EllipsizeMode.End;
        this.Editable = false;
    }

    static CellRendererButtonValue()
    {
        typeof(CellRendererButtonValue).GetMembers();
    }

    /// <summary>
    /// Gets or sets a value indicating whether this
    /// <see cref="T:System.Windows.Forms.GtkRender.CellRendererButtonValue" /> is activatable.
    /// </summary>
    /// <value>
    /// <c>
    /// true
    /// </c> if activatable; otherwise,
    /// <c>
    /// false
    /// </c>.
    /// </value>
    public bool Activatable
    {
        get;
        set;
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
            var style = utility.GetStyle(this.Column, cell);
            utility.SetTextWithStyle(this, style, cell.Value);
            base.SetAlignment(0.5f, 0.5f);
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
    /// <see cref="T:System.Windows.Forms.GtkRender.CellRendererButtonValue" /> is editable.
    /// </summary>
    /// <value>
    /// <c>
    /// true
    /// </c> if editable; otherwise,
    /// <c>
    /// false
    /// </c>.
    /// </value>
    public new bool Editable
    {
        get;
        set;
    }

    string? ICellRenderer.Markup
    {
        get;
        set;
    }

    /// <summary>
    /// Called when render.
    /// </summary>
    /// <param name="cr">
    /// The cr.
    /// </param>
    /// <param name="widget">
    /// The widget.
    /// </param>
    /// <param name="background_area">
    /// The background area.
    /// </param>
    /// <param name="cell_area">
    /// The cell area.
    /// </param>
    /// <param name="flags">
    /// The flags.
    /// </param>
    protected override void OnRender(Cairo.Context cr, Widget widget, Gdk.Rectangle background_area, Gdk.Rectangle cell_area, CellRendererState flags)
    {
        widget.StyleContext.Save();
        widget.StyleContext.AddClass("GridViewCell-Button");
        var areax = cell_area.X + 5;
        var width = cell_area.Width - 10;
        if (width < 1)
            width = 1;
        widget.StyleContext.RenderFrame(cr, areax, cell_area.Y, width, cell_area.Height);
        widget.StyleContext.RenderBackground(cr, areax, cell_area.Y, width, cell_area.Height);
        widget.StyleContext.Restore();
        base.OnRender(cr, widget, background_area, new Gdk.Rectangle(areax, cell_area.Y, width, cell_area.Height), flags);
    }
}