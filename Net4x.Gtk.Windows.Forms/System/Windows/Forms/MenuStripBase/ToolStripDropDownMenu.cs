using System.ComponentModel;
using System.Drawing;

namespace System.Windows.Forms;

/// <summary>
/// Provides basic functionality for the
/// <see cref="T:System.Windows.Forms.ContextMenuStrip" /> control. Although
/// <see cref="T:System.Windows.Forms.ToolStripDropDownMenu" /> and
/// <see cref="T:System.Windows.Forms.ToolStripDropDown" />
/// replace and add functionality to the
/// <see cref="!:System.Windows.Forms.Menu" />
/// control of previous versions,
/// <see cref="!:System.Windows.Forms.Menu" />
/// is retained for both backward compatibility and future use if you choose.
/// </summary>
public class ToolStripDropDownMenu : ToolStripDropDown
{
    /// <summary>
    /// Occurs when font changed.
    /// </summary>
    public virtual event EventHandler<EventArgs>? FontChanged;

    /// <summary>
    /// Occurs when layout.
    /// </summary>
    public virtual event EventHandler<LayoutEventArgs>? Layout;

    /// <summary>
    /// Occurs when paint background.
    /// </summary>
    public virtual event EventHandler<PaintEventArgs>? PaintBackground;

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.ToolStripDropDownMenu" /> class.
    /// </summary>
    public ToolStripDropDownMenu()
    {

    }

    /// <summary>
    /// Gets or sets the layout style.
    /// </summary>
    /// <value>
    /// The layout style.
    /// </value>
    //public Rectangle DisplayRectangle { get; }
    //public LayoutEngine LayoutEngine { get; }
    public ToolStripLayoutStyle LayoutStyle
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets a value indicating whether show check margin.
    /// </summary>
    /// <value>
    /// <c>
    /// true
    /// </c> if show check margin; otherwise,
    /// <c>
    /// false
    /// </c>.
    /// </value>
    [DefaultValue(false)]
    public bool ShowCheckMargin
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets a value indicating whether show image margin.
    /// </summary>
    /// <value>
    /// <c>
    /// true
    /// </c> if show image margin; otherwise,
    /// <c>
    /// false
    /// </c>.
    /// </value>
    [DefaultValue(true)]
    public bool ShowImageMargin
    {
        get;
        set;
    } = true;

    /// <summary>
    /// Gets the maximum size of the item.
    /// </summary>
    /// <value>
    /// The maximum size of the item.
    /// </value>
    protected internal Size MaxItemSize { get; }

    /// <summary>
    /// Gets the default padding.
    /// </summary>
    /// <value>
    /// The default padding.
    /// </value>
    protected Padding DefaultPadding { get; }

    /// <summary>
    /// Creates the default item.
    /// </summary>
    /// <param name="text">
    /// The text.
    /// </param>
    /// <param name="image">
    /// The image.
    /// </param>
    /// <param name="onClick">
    /// The on click.
    /// </param>
    /// <returns>
    /// ToolStripItem.
    /// </returns>
    protected internal ToolStripItem? CreateDefaultItem(string text, Image image, EventHandler onClick) { return null; }

    /// <summary>
    /// Handles the
    /// <see cref="!:FontChanged" /> event.
    /// </summary>
    /// <param name="e">
    /// The
    /// <see cref="T:System.EventArgs" /> instance containing the event data.
    /// </param>
    protected void OnFontChanged(EventArgs e)
    {
        FontChanged?.Invoke(this, e);
    }

    /// <summary>
    /// Handles the
    /// <see cref="T:System.Windows.Forms.Layout" /> event.
    /// </summary>
    /// <param name="e">
    /// The
    /// <see cref="T:System.Windows.Forms.LayoutEventArgs" /> instance containing the event data.
    /// </param>
    protected void OnLayout(LayoutEventArgs e)
    {
        Layout?.Invoke(this, e);
    }

    /// <summary>
    /// Handles the
    /// <see cref="!:PaintBackground" /> event.
    /// </summary>
    /// <param name="e">
    /// The
    /// <see cref="T:System.Windows.Forms.PaintEventArgs" /> instance containing the event data.
    /// </param>
    protected void OnPaintBackground(PaintEventArgs e)
    {
        PaintBackground?.Invoke(this, e);
    }

    /// <summary>
    /// Sets the displayed items.
    /// </summary>
    protected void SetDisplayedItems() { }
}