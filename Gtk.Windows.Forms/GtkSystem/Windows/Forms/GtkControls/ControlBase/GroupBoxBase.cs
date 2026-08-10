using Gdk;
using Gtk;
using ScrollEventHandler = System.Windows.Forms.ScrollEventHandler;

namespace GtkSystem.Windows.Forms.GtkControls.ControlBase;

/// <summary>
/// GroupBoxBase is a base class for GTK group box controls, providing common functionality and properties for derived classes. It implements the IControlGtk and IScrollableBoxBase interfaces, allowing it to integrate with the GTK system and provide consistent behavior across different types of group box controls. The GroupBoxBase class includes features such as auto-scrolling, event handling for scrolling, and custom drawing capabilities.
/// </summary>
/// <seealso cref="Gtk.Frame" />
/// <seealso cref="GtkSystem.Windows.Forms.GtkControls.ControlBase.IControlGtk" />
/// <seealso cref="GtkSystem.Windows.Forms.GtkControls.ControlBase.IScrollableBoxBase" />
public sealed class GroupBoxBase : Gtk.Frame, IControlGtk, IScrollableBoxBase
{
    /// <summary>
    /// The overlay container that hosts the background drawing area.
    /// </summary>
    public Gtk.Overlay contaner = new();

    /// <summary>
    /// The scrolled container that hosts the overlay container.
    /// </summary>
    public Gtk.ScrolledWindow fixedcontaner = new();

    /// <summary>
    /// Occurs when a scroll event occurs..
    /// </summary>
    public event ScrollEventHandler? Scroll;

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="GtkSystem.Windows.Forms.GtkControls.ControlBase.GroupBoxBase" /> class.
    /// </summary>
    public GroupBoxBase() : base()
    {
        this.Override = new GtkControlOverride(this);
        this.StyleContext.AddClass("GroupBox");
        this.LabelXalign = 0.03f;
        base.Valign = Gtk.Align.Start;
        base.Halign = Gtk.Align.Start;
        contaner.Halign = Align.Fill;
        contaner.Valign = Align.Fill;
        var background = new Gtk.DrawingArea();
        background.Events |= Gdk.EventMask.ButtonPressMask;
        background.Events |= Gdk.EventMask.ButtonReleaseMask;
        background.DrawFunc = Background_DrawFunc;
        contaner.Add(background);
        fixedcontaner.Events &= EventMask.PointerMotionMask;
        fixedcontaner.Events &= EventMask.ButtonMotionMask;
        fixedcontaner.Events &= EventMask.ScrollMask;
        fixedcontaner.HscrollbarPolicy = PolicyType.Never;
        fixedcontaner.VscrollbarPolicy = PolicyType.External;
        fixedcontaner.Add(contaner);
        this.Add(fixedcontaner);
    }

    /// <summary>
    /// Gets or sets a value indicating whether automatic scroll.
    /// </summary>
    /// <value>
    /// <c>
    /// true
    /// </c> if automatic scroll; otherwise,
    /// <c>
    /// false
    /// </c>.
    /// </value>
    public bool AutoScroll
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets a value indicating whether  horizontal scroll.
    /// </summary>
    /// <value>
    /// <c>
    /// true
    /// </c> if  horizontal scroll; otherwise,
    /// <c>
    /// false
    /// </c>.
    /// </value>
    public bool HScroll
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the
    /// <see cref="GtkSystem.Windows.Forms.GtkControls.ControlBase.GtkControlOverride" /> instance that centralizes drawing and styling behavior for this control. This override encapsulates background painting (color and image), image drawing, CSS class management and raises paint events so the control can handle custom painting (both direct Cairo via
    /// <see cref="GtkSystem.Windows.Forms.GtkControls.ControlBase.PaintGraphicsEventHandler" /> and System.Drawing via
    /// <see cref="T:System.Windows.Forms.PaintEventHandler" />). The override is typically created and configured in the control's constructor and invoked from the GTK drawing/show handlers.
    /// </summary>
    /// <value>
    /// The active
    /// <see cref="GtkSystem.Windows.Forms.GtkControls.ControlBase.GtkControlOverride" /> used by the control.
    /// </value>
    public GtkControlOverride Override
    {
        get;
        set;
    }

    /// <summary>
    /// Gets the scrolled window of this control.
    /// </summary>
    /// <value>
    /// The scrolled window.
    /// </value>
    public Gtk.ScrolledWindow ScrolledWindow { get; } = null!;

    /// <summary>
    /// Gets or sets a value indicating whether vertical scroll.
    /// </summary>
    /// <value>
    /// <c>
    /// true
    /// </c> if vertical scroll; otherwise,
    /// <c>
    /// false
    /// </c>.
    /// </value>
    public bool VScroll
    {
        get;
        set;
    }

    /// <summary>
    /// Packs the specified child.
    /// </summary>
    /// <param name="child">
    /// The child.
    /// </param>
    /// <param name="align">
    /// The align.
    /// </param>
    /// <param name="expand">
    /// if set to
    /// <c>
    /// true
    /// </c>expand.
    /// </param>
    public void Pack(Widget child, Align align, bool expand)
    {
        child.Valign = align;
        child.Halign = align;
        child.Hexpand = expand;
        child.Vexpand = expand;
        base.Add(child);
    }

    /// <summary>
    /// Scrolls the view to the specified values.
    /// </summary>
    /// <param name="hscrollValue">
    /// The horizontal scroll value.
    /// </param>
    /// <param name="vscrollValue">
    /// The vertical scroll value.
    /// </param>
    /// <returns>
    /// </returns>
    public bool ScrollView(double hscrollValue, double vscrollValue)
    {
        return false;
    }

    private void Background_DrawFunc(Gtk.DrawingArea area, Cairo.Context cr, int width, int height)
    {
        Override.OnPaint(cr);
    }

    // Gtk 4: IScrollableBoxBase.Add is explicit now that Gtk.Container.Add is not inherited.
    public void Add(Gtk.Widget child) => base.Add(child);



    /// <summary>
    /// Raises the Scroll event.
    /// </summary>
    private void OnScroll(System.Windows.Forms.ScrollEventArgs e)
    {
        Scroll?.Invoke(this, e);
    }
}
