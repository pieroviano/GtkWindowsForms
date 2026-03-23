/*
 * Based on GTK components, it is a cross-platform interface component that is compatible with the native C# control winform interface.
 * Use this component GtkSystem.Windows.Forms instead of Microsoft.WindowsDesktop.App.WindowsForms, compile once, and run across Windows, Linux, and macOS
 * Technical support 438865652@qq.com，https://www.gtkapp.com, https://gitee.com/easywebfactory, https://github.com/easywebfactory438865652@qq.com，https://www.gtkapp.com, https://gitee.com/easywebfactory, https://github.com/easywebfactory
 * author:chenhongjin
 */
using Atk;
using Gdk;
using GLib;
using Gtk;
using Pango;

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
    readonly DataGridViewColumn Column;

    private readonly CellReandererUtility utility;

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
        utility = new CellReandererUtility();
        this.SetAlignment(0.5f, 0.5f);
        this.Ellipsize = Pango.EllipsizeMode.End;
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
    public DataGridViewCell CellValue
    {
        set
        {
            if (value != null)
            {
                var style = value.Style ?? ColumnStyle;
                if (style == null)
                    style = value.InheritedStyle;
                utility.SetTextWithStyle(this, style, value.Value);
                base.SetAlignment(0.5f, 0.5f);
            }
        }
    }

    /// <summary>
    /// Gets or sets the column style.
    /// </summary>
    /// <value>
    /// The column style.
    /// </value>
    public DataGridViewCellStyle ColumnStyle
    {
        get;
        set;
    }

    string ICellRenderer.Markup
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
        widget.StyleContext.AddClass("GridViewCell-Button");
        widget.StyleContext.Save();
        var height = cell_area.Height;
        var y = cell_area.Y;
        if (height > 36)
        {
            y = y + (cell_area.Height - 36) / 2;
            height = 36;
        }
        widget.StyleContext.RenderFrame(cr, cell_area.X + 3, y, cell_area.Width - 6, height);
        widget.StyleContext.RenderBackground(cr, cell_area.X + 3, y, cell_area.Width - 6, height);
        widget.StyleContext.Restore();
        base.OnRender(cr, widget, new Gdk.Rectangle(background_area.X, background_area.Y, background_area.Width, background_area.Height), new Gdk.Rectangle(cell_area.X, cell_area.Y, cell_area.Width, cell_area.Height), flags);
    }
}