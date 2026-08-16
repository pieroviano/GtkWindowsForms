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
    /// Horizontal breathing space left on either side of the caption when the top border is interrupted.
    /// </summary>
    private const double CaptionGap = 2;

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
        // Gtk4-removed: background.Events |= Gdk.EventMask.ButtonPressMask;
        // Gtk4-removed: background.Events |= Gdk.EventMask.ButtonReleaseMask;
        background.DrawFunc = Background_DrawFunc;
        // Paint is raised from this child, not from the frame, so an invalidation has to land here.
        this.Override.PaintSurface = background;
        contaner.AddChild(background);
        // Gtk4-removed: fixedcontaner.Events &= EventMask.PointerMotionMask;
        // Gtk4-removed: fixedcontaner.Events &= EventMask.ButtonMotionMask;
        // Gtk4-removed: fixedcontaner.Events &= EventMask.ScrollMask;
        fixedcontaner.HscrollbarPolicy = PolicyType.Never;
        fixedcontaner.VscrollbarPolicy = PolicyType.External;
        fixedcontaner.AddChild(contaner);
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
        base.Child = child; // Gtk4: Frame.Add -> Child
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

    /// <summary>
    /// Re-applies the caption-band compensation once GTK has allocated the frame.
    /// </summary>
    /// <param name="width">
    /// The width the parent allocated to this frame.
    /// </param>
    /// <param name="height">
    /// The height the parent allocated to this frame.
    /// </param>
    /// <param name="baseline">
    /// The baseline the parent allocated to this frame, or -1.
    /// </param>
    /// <remarks>
    /// A <see cref="Gtk.Frame" /> lays its caption out <em>above</em> the child, so the content overlay
    /// starts a whole label band below the frame's top-left corner. WinForms places a group box child at
    /// its <c>Location</c> relative to the control's corner, caption or no caption, so every child would
    /// otherwise be pushed down by that band and the bottom-most one clipped. The band is cancelled by
    /// scrolling the viewport back up by exactly its height - the Gtk3 build did the same from the
    /// <c>::size-allocate</c> signal, which Gtk4 replaced with this vfunc. The height is measured rather
    /// than assumed because it follows the caption font.
    /// </remarks>
    protected override void OnSizeAllocate(int width, int height, int baseline)
    {
        base.OnSizeAllocate(width, height, baseline);

        // The child's own origin *is* the band: whatever the frame reserved for border and caption.
        var band = fixedcontaner.Allocation.Y;
        if (band <= 0)
            return;

        // Scrolling only has room to move if the content is at least as tall as the whole frame, which
        // is also what keeps a child at the designed height from being cut short by the band.
        var content = Math.Max(1, height - contaner.MarginTop - contaner.MarginBottom);
        if (contaner.HeightRequest < content)
            contaner.HeightRequest = content;

        // Guarded so writing the adjustment from inside an allocation cannot re-enter it indefinitely.
        var vadjustment = fixedcontaner.Vadjustment;
        if (vadjustment != null && Math.Abs(vadjustment.Value - band) > 0.5)
            vadjustment.Value = band;
    }

    /// <summary>
    /// Draws the group box border with the caption notched out of its top edge.
    /// </summary>
    /// <param name="snapshot">
    /// The snapshot GTK is building.
    /// </param>
    /// <remarks>
    /// A WinForms group box interrupts its top border and sets the caption into the gap. Gtk3 got that for
    /// free: <c>gtk_frame_draw</c> painted the border with <c>gtk_render_frame_gap</c>, which takes the
    /// caption's extent and leaves it out of the edge. Gtk4 removed that primitive - a frame now allocates
    /// its caption inside the content box, above the child, and lets ordinary CSS box rendering draw an
    /// uninterrupted rectangle - so the notch has to be drawn here or not at all. The theme's own rectangle
    /// is made transparent by the <c>.GroupBox</c> rule in
    /// <see cref="P:System.Windows.Forms.Application.UserPriorityCss" />, which leaves the box model, and
    /// therefore every child's position, untouched.
    /// </remarks>
    protected override void OnSnapshot(Gtk.Snapshot snapshot)
    {
        base.OnSnapshot(snapshot);
        SnapshotNotchedFrame(snapshot);
    }

    /// <summary>
    /// Strokes the notched border of a group box onto a Cairo context.
    /// </summary>
    /// <param name="cr">
    /// The context to stroke onto. Its source colour and line width are used as they are.
    /// </param>
    /// <param name="width">
    /// The width of the group box.
    /// </param>
    /// <param name="height">
    /// The height of the group box.
    /// </param>
    /// <param name="top">
    /// The vertical position of the top edge, normally the middle of the caption.
    /// </param>
    /// <param name="gapStart">
    /// The left end of the interruption in the top edge.
    /// </param>
    /// <param name="gapEnd">
    /// The right end of the interruption in the top edge. A value at or below
    /// <paramref name="gapStart" /> means there is no caption and the border closes.
    /// </param>
    /// <remarks>
    /// Square corners, as WinForms and the Gtk3 build both drew them, and half-pixel offsets so a one pixel
    /// line lands on a pixel row rather than straddling two.
    /// </remarks>
    internal static void StrokeNotchedFrame(Cairo.Context cr, double width, double height,
        double top, double gapStart, double gapEnd)
    {
        var left = 0.5;
        var right = width - 0.5;
        var bottom = height - 0.5;

        if (right <= left || bottom <= top || top < 0)
        {
            return;
        }

        gapStart = Math.Max(left, Math.Min(right, gapStart));
        gapEnd = Math.Max(left, Math.Min(right, gapEnd));

        if (gapEnd <= gapStart)
        {
            // No caption to make room for, so the border closes - as a captionless group box does.
            cr.MoveTo(left, top);
            cr.LineTo(right, top);
            cr.LineTo(right, bottom);
            cr.LineTo(left, bottom);
            cr.ClosePath();
        }
        else
        {
            // One continuous path from the right end of the notch, round the box, back to its left end.
            cr.MoveTo(gapEnd, top);
            cr.LineTo(right, top);
            cr.LineTo(right, bottom);
            cr.LineTo(left, bottom);
            cr.LineTo(left, top);
            cr.LineTo(gapStart, top);
        }

        cr.Stroke();
    }

    private void Background_DrawFunc(Gtk.DrawingArea area, Cairo.Context cr, int width, int height)
    {
        Override.OnPaint(cr);
    }

    private void SnapshotNotchedFrame(Gtk.Snapshot snapshot)
    {
        var width = AllocatedWidth;
        var height = AllocatedHeight;

        if (width <= 0 || height <= 0)
        {
            return;
        }

        // The top edge runs through the middle of the caption, which is where WinForms puts it. Taken from
        // the caption widget rather than from a constant so it follows the font, the alignment and the
        // theme's own padding; before the first allocation there is nothing to measure and the border
        // degenerates to a plain rectangle at the top of the frame.
        var top = 0.5;
        double gapStart = 0;
        double gapEnd = 0;
        var caption = LabelWidget;

        if (caption != null && caption.Visible)
        {
            var area = caption.Allocation;
            if (area is { Width: > 0, Height: > 0 })
            {
                top = area.Y + (area.Height / 2.0) + 0.5;
                gapStart = area.X - CaptionGap;
                gapEnd = area.X + area.Width + CaptionGap;
            }
        }

        var rect = Graphene.Rect.Alloc();
        rect.Init(0, 0, width, height);

        using var cr = snapshot.AppendCairo(rect);

        // The theme's own frame colour, so the shim's border is the one the platform would have drawn.
        if (!StyleContext.LookupColor("borders", out var border))
        {
            border = new Gdk.RGBA { Red = 0, Green = 0, Blue = 0, Alpha = 0.18f };
        }

        cr.SetSourceRGBA(border.Red, border.Green, border.Blue, border.Alpha);
        cr.LineWidth = 1;
        StrokeNotchedFrame(cr, width, height, top, gapStart, gapEnd);
    }

    // Gtk 4: IScrollableBoxBase.Add is explicit now that Gtk.Container.Add is not inherited.
    public void Add(Gtk.Widget child) => base.Child = child; // Gtk4: Frame.Add -> Child



    /// <summary>
    /// Raises the Scroll event.
    /// </summary>
    private void OnScroll(System.Windows.Forms.ScrollEventArgs e)
    {
        Scroll?.Invoke(this, e);
    }
}
