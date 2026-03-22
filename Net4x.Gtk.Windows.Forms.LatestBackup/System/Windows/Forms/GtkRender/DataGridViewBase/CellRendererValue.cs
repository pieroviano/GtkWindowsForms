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
/// Class CellRendererValue. Implements the
/// <see cref="Gtk.CellRendererText" />
/// Implements the
/// <see cref="T:System.Windows.Forms.GtkRender.ICellRenderer" />
/// </summary>
/// <seealso cref="Gtk.CellRendererText" />
/// <seealso cref="T:System.Windows.Forms.GtkRender.ICellRenderer" />
public class CellRendererValue : CellRendererText, ICellRenderer
{

    private readonly DataGridViewColumn Column;

    private readonly CellReandererUtility utility;

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.GtkRender.CellRendererValue" /> class.
    /// </summary>
    /// <param name="view">
    /// The view.
    /// </param>
    public CellRendererValue(DataGridViewColumn view)
    {
        Column = view;
        utility = new CellReandererUtility();
    }

    /// <summary>
    /// Gets or sets a value indicating whether this
    /// <see cref="T:System.Windows.Forms.GtkRender.ICellRenderer" /> is activatable.
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
                DataGridViewCellStyle style = value.Style ?? ColumnStyle;
                if (style == null)
                    style = value.InheritedStyle;

                utility.SetTextWithStyle(this, style, value.Value);
            }
        }
    }

    /// <summary>
    /// Sets the column style.
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
}