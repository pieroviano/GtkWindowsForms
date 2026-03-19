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
/// Class CellReandererUtility.
/// </summary>
public class CellReandererUtility
{
    internal string GetFormatText(object text, DataGridViewCellStyle cellstyle)
    {
        if (text == null && cellstyle != null)
            text = Convert.ToString(cellstyle.NullValue) ?? string.Empty;
        if (cellstyle != null && string.IsNullOrWhiteSpace(cellstyle.Format) == false)
            return string.Format(cellstyle.Format, text);
        else
            return text?.ToString();
    }

    internal void SetControlWithStyle(CellRenderer cell, DataGridViewCellStyle cellstyle)
    {
        if (cellstyle != null)
        {
            if (cellstyle.BackColor.Name != "0")
                cell.CellBackgroundRgba = new Gdk.RGBA() { Alpha = cellstyle.BackColor.A / 255f, Blue = cellstyle.BackColor.B / 255f, Green = cellstyle.BackColor.G / 255f, Red = cellstyle.BackColor.R / 255f };
            if (cellstyle.Alignment == DataGridViewContentAlignment.TopLeft)
                cell.SetAlignment(0, 0);
            else if (cellstyle.Alignment == DataGridViewContentAlignment.TopCenter)
                cell.SetAlignment(0.5f, 0);
            else if (cellstyle.Alignment == DataGridViewContentAlignment.TopRight)
                cell.SetAlignment(1.0f, 0);
            else if (cellstyle.Alignment == DataGridViewContentAlignment.MiddleLeft)
                cell.SetAlignment(0, 0.5f);
            else if (cellstyle.Alignment == DataGridViewContentAlignment.MiddleCenter)
                cell.SetAlignment(0.5f, 0.5f);
            else if (cellstyle.Alignment == DataGridViewContentAlignment.MiddleRight)
                cell.SetAlignment(1.0f, 0.5f);
            else if (cellstyle.Alignment == DataGridViewContentAlignment.BottomLeft)
                cell.SetAlignment(0, 1f);
            else if (cellstyle.Alignment == DataGridViewContentAlignment.BottomCenter)
                cell.SetAlignment(0.5f, 1f);
            else if (cellstyle.Alignment == DataGridViewContentAlignment.BottomRight)
                cell.SetAlignment(1.0f, 1f);
        }
    }

    internal void SetTextWithStyle(CellRendererText cell, DataGridViewCellStyle cellstyle, object value)
    {
        SetTextWithStyle(cell, cellstyle);
        cell.Text = GetFormatText(value, cellstyle);
    }

    internal void SetTextWithStyle(CellRendererText cell, DataGridViewCellStyle cellstyle)
    {
        if (cellstyle != null)
        {
            if (cellstyle?.WrapMode == DataGridViewTriState.True)
            {
                cell.WrapMode = Pango.WrapMode.WordChar;
                cell.WrapWidth = 0;
                cell.WidthChars = 0;
            }
            if (cellstyle.ForeColor.Name != "0")
                cell.ForegroundRgba = new Gdk.RGBA() { Alpha = cellstyle.ForeColor.A / 255f, Blue = cellstyle.ForeColor.B / 255f, Green = cellstyle.ForeColor.G / 255f, Red = cellstyle.ForeColor.R / 255f };
            if (cellstyle.BackColor.Name != "0")
                cell.CellBackgroundRgba = new Gdk.RGBA() { Alpha = cellstyle.BackColor.A / 255f, Blue = cellstyle.BackColor.B / 255f, Green = cellstyle.BackColor.G / 255f, Red = cellstyle.BackColor.R / 255f };
            if (cellstyle.Alignment == DataGridViewContentAlignment.TopLeft)
                cell.SetAlignment(0, 0);
            else if (cellstyle.Alignment == DataGridViewContentAlignment.TopCenter)
                cell.SetAlignment(0.5f, 0);
            else if (cellstyle.Alignment == DataGridViewContentAlignment.TopRight)
                cell.SetAlignment(1.0f, 0);
            else if (cellstyle.Alignment == DataGridViewContentAlignment.MiddleLeft)
                cell.SetAlignment(0, 0.5f);
            else if (cellstyle.Alignment == DataGridViewContentAlignment.MiddleCenter)
                cell.SetAlignment(0.5f, 0.5f);
            else if (cellstyle.Alignment == DataGridViewContentAlignment.MiddleRight)
                cell.SetAlignment(1.0f, 0.5f);
            else if (cellstyle.Alignment == DataGridViewContentAlignment.BottomLeft)
                cell.SetAlignment(0, 1f);
            else if (cellstyle.Alignment == DataGridViewContentAlignment.BottomCenter)
                cell.SetAlignment(0.5f, 1f);
            else if (cellstyle.Alignment == DataGridViewContentAlignment.BottomRight)
                cell.SetAlignment(1.0f, 1f);

            if (cellstyle.WrapMode != DataGridViewTriState.NotSet)
                cell.WrapMode = cellstyle.WrapMode == DataGridViewTriState.True ? Pango.WrapMode.WordChar : Pango.WrapMode.Word;
        }
    }

}

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
    public bool Activatable { get; set; }

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
    public DataGridViewCellStyle ColumnStyle { get; set; }

    string ICellRenderer.Markup { get; set; }

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
        int height = cell_area.Height;
        int y = cell_area.Y;
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

/// <summary>
/// Class CellRendererComboValue. Implements the
/// <see cref="Gtk.CellRendererCombo" /> Implements the
/// <see cref="T:System.Windows.Forms.GtkRender.ICellRenderer" />
/// </summary>
/// <seealso cref="Gtk.CellRendererCombo" />
/// <seealso cref="T:System.Windows.Forms.GtkRender.ICellRenderer" />
public class CellRendererComboValue : CellRendererCombo, ICellRenderer
{
    readonly DataGridViewColumn Column;

    private readonly CellReandererUtility utility;

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.GtkRender.CellRendererComboValue" /> class.
    /// </summary>
    /// <param name="view">
    /// The view.
    /// </param>
    public CellRendererComboValue(DataGridViewColumn view)
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
    public bool Activatable { get; set; }

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
    public DataGridViewCellStyle ColumnStyle { get; set; }

    string ICellRenderer.Markup { get; set; }

}

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
    public bool Activatable { get; set; }

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
                    DataGridViewCellStyle style = value.Style ?? ColumnStyle;
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
    public DataGridViewCellStyle ColumnStyle { get; set; }

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
    public bool Editable { get; set; }

    /// <summary>
    /// Gets or sets the markup.
    /// </summary>
    /// <value>
    /// The markup.
    /// </value>
    public string Markup { get; set; }

    /// <summary>
    /// Gets or sets the width chars.
    /// </summary>
    /// <value>
    /// The width chars.
    /// </value>
    public int WidthChars { get; set; }

    /// <summary>
    /// Gets or sets the wrap mode.
    /// </summary>
    /// <value>
    /// The wrap mode.
    /// </value>
    public Pango.WrapMode WrapMode { get; set; }

    /// <summary>
    /// Gets or sets the width of the wrap.
    /// </summary>
    /// <value>
    /// The width of the wrap.
    /// </value>
    public int WrapWidth { get; set; }

}

/// <summary>
/// Class CellRendererToggleValue. Implements the
/// <see cref="Gtk.CellRendererToggle" /> Implements the
/// <see cref="T:System.Windows.Forms.GtkRender.ICellRenderer" />
/// </summary>
/// <seealso cref="Gtk.CellRendererToggle" />
/// <seealso cref="T:System.Windows.Forms.GtkRender.ICellRenderer" />
public class CellRendererToggleValue : CellRendererToggle, ICellRenderer
{
    readonly DataGridViewColumn Column;

    private readonly CellReandererUtility utility;

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
        utility = new CellReandererUtility();
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
                this.Active = (value.Value?.ToString() == "1" || value.Value?.ToString()?.ToLower() == "true");
                DataGridViewCellStyle style = value.Style ?? ColumnStyle;
                if (style == null)
                    style = value.InheritedStyle;
                utility.SetControlWithStyle(this, style);
            }
        }
    }

    /// <summary>
    /// Gets or sets the column style.
    /// </summary>
    /// <value>
    /// The column style.
    /// </value>
    public DataGridViewCellStyle ColumnStyle { get; set; }

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
    public bool Editable { get; set; }

    /// <summary>
    /// Gets or sets the markup.
    /// </summary>
    /// <value>
    /// The markup.
    /// </value>
    public string Markup { get; set; }

    /// <summary>
    /// Gets or sets the width chars.
    /// </summary>
    /// <value>
    /// The width chars.
    /// </value>
    public int WidthChars { get; set; }

    /// <summary>
    /// Gets or sets the wrap mode.
    /// </summary>
    /// <value>
    /// The wrap mode.
    /// </value>
    public Pango.WrapMode WrapMode { get; set; }

    /// <summary>
    /// Gets or sets the width of the wrap.
    /// </summary>
    /// <value>
    /// The width of the wrap.
    /// </value>
    public int WrapWidth { get; set; }

}

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
    public bool Activatable { get; set; }

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
    public DataGridViewCellStyle ColumnStyle { get; set; }

    string ICellRenderer.Markup { get; set; }

}

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
    bool Activatable { get; set; }

    /// <summary>
    /// Sets the cell value.
    /// </summary>
    /// <value>
    /// The cell value.
    /// </value>
    DataGridViewCell CellValue { set; }

    /// <summary>
    /// Sets the column style.
    /// </summary>
    /// <value>
    /// The column style.
    /// </value>
    DataGridViewCellStyle ColumnStyle { set; }

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
    bool Editable { get; set; }

    /// <summary>
    /// Gets or sets the height.
    /// </summary>
    /// <value>
    /// The height.
    /// </value>
    int Height { get; set; }

    /// <summary>
    /// Gets or sets the markup.
    /// </summary>
    /// <value>
    /// The markup.
    /// </value>
    string Markup { get; set; }

    /// <summary>
    /// Gets or sets the mode.
    /// </summary>
    /// <value>
    /// The mode.
    /// </value>
    Gtk.CellRendererMode Mode { get; set; }

    /// <summary>
    /// Gets or sets the width chars.
    /// </summary>
    /// <value>
    /// The width chars.
    /// </value>
    int WidthChars { get; set; }

    /// <summary>
    /// Gets or sets the wrap mode.
    /// </summary>
    /// <value>
    /// The wrap mode.
    /// </value>
    Pango.WrapMode WrapMode { get; set; }

    /// <summary>
    /// Gets or sets the width of the wrap.
    /// </summary>
    /// <value>
    /// The width of the wrap.
    /// </value>
    int WrapWidth { get; set; }

}