/*
 * Based on GTK components, it is a cross-platform interface component that is compatible with the native C# control winform interface.
 * Use this component GtkSystem.Windows.Forms instead of Microsoft.WindowsDesktop.App.WindowsForms, compile once, and run across Windows, Linux, and macOS
 * Technical support 438865652@qq.com，https://www.gtkapp.com, https://gitee.com/easywebfactory, https://github.com/easywebfactory438865652@qq.com，https://www.gtkapp.com, https://gitee.com/easywebfactory, https://github.com/easywebfactory
 * author:chenhongjin
 */

namespace System.Windows.Forms.GtkRender;

/// <summary>
/// Interface ICellRenderer
/// </summary>
public interface ICellRenderer
{
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
    bool Activatable
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
    [GLib.Property("cellvalue")]
    DataGridViewRow CellValue { set; }

    /// <summary>
    /// Sets the column style.
    /// </summary>
    /// <value>
    /// The column style.
    /// </value>
    DataGridViewCellStyle? ColumnStyle { set; }

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
    bool Editable
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the height.
    /// </summary>
    /// <value>
    /// The height.
    /// </value>
    int Height
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
    string? Markup
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the mode.
    /// </summary>
    /// <value>
    /// The mode.
    /// </value>
    Gtk.CellRendererMode Mode
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
    int WidthChars
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
    Pango.WrapMode WrapMode
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
    int WrapWidth
    {
        get;
        set;
    }
}