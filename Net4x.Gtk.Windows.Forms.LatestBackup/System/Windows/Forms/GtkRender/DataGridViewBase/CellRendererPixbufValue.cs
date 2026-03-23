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
/// Class CellRendererPixbufValue. Implements the
/// <see cref="Gtk.CellRendererPixbuf" /> Implements the
/// <see cref="T:System.Windows.Forms.GtkRender.ICellRenderer" />
/// </summary>
/// <seealso cref="Gtk.CellRendererPixbuf" />
/// <seealso cref="T:System.Windows.Forms.GtkRender.ICellRenderer" />
public class CellRendererPixbufValue : CellRendererPixbuf, ICellRenderer
{
    readonly DataGridViewColumn Column;

    private readonly CellReandererUtility utility;

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.GtkRender.CellRendererPixbufValue" /> class.
    /// </summary>
    /// <param name="view">
    /// The view.
    /// </param>
    public CellRendererPixbufValue(DataGridViewColumn view)
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
                try
                {
                    var style = value.Style ?? ColumnStyle;
                    if (style == null)
                        style = value.InheritedStyle;
                    utility.SetControlWithStyle(this, style);
                    if (typeof(Drawing.Image).Equals(value.ValueType) || typeof(Drawing.Bitmap).Equals(value.ValueType))
                    {
                        this.Pixbuf = ((Drawing.Image)value.Value).Pixbuf;
                    }
                    else if (value.ValueType == typeof(byte[]))
                    {
                        this.Pixbuf = new Pixbuf((byte[])value.Value);
                    }
                    else
                    {
                        this.Pixbuf = new Gdk.Pixbuf(value.Value.ToString().Replace("\\\\", "//").Replace("\\", "/"));
                    }
                }
                catch
                {
                    this.IconName = "image-missing";
                }
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

    /// <summary>
    /// Gets or sets a value indicating whether this
    /// <see cref="T:System.Windows.Forms.GtkRender.ICellRenderer" /> is editable.
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
    public string Markup
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