using Gdk;

namespace Gtk.Windows.Forms.Controls.ControlBase;

/// <summary>
/// ButtonBase is a base class for GTK button controls, providing common functionality and properties for derived classes. It implements the IControlGtk interface, allowing it to integrate with the GTK system and provide consistent behavior across different types of button controls.
/// </summary>
/// <seealso cref="Gtk.Button" />
/// <seealso cref="Gtk.Windows.Forms.Controls.ControlBase.IControlGtk" />
public sealed class ButtonBase : Gtk.Button, IControlGtk
{
    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="Gtk.Windows.Forms.Controls.ControlBase.ButtonBase" /> class.
    /// </summary>
    public ButtonBase() : base()
    {
        this.Child = new Gtk.Label() { MaxWidthChars = 0, Wrap = false, Ellipsize = Pango.EllipsizeMode.End }; // Gtk4: Button(Widget) ctor removed
        this.Override = new GtkControlOverride(this);
        this.StyleContext.AddClass("Button");
        this.Valign = Gtk.Align.Start;
        this.Halign = Gtk.Align.Start;
        // Gtk4-removed: this.Events |= EventMask.PointerMotionMask;
        // Gtk4-removed: this.Events |= EventMask.ButtonMotionMask;
    }

    /// <summary>
    /// Draws the WinForms background and raises <c>Paint</c> around GTK's own rendering of the button.
    /// </summary>
    /// <param name="snapshot">The snapshot GTK is building.</param>
    /// <remarks>
    /// <para>
    /// Gtk4 removed <c>Widget.Drawn</c>, and the port took that to mean custom painting had to be dropped
    /// until it could move to CSS. It does not: <c>GtkWidget</c>'s <c>snapshot</c> vfunc is overridable from
    /// C# as <see cref="Gtk.Widget.OnSnapshot" />, and <see cref="Gtk.Snapshot.AppendCairo" /> hands back a
    /// Cairo context - so the existing drawing code, which is unchanged from the Gtk3 build, can simply be
    /// called again. Without this a control's <c>BackgroundImage</c> was never painted and its
    /// <c>Paint</c> handlers never ran.
    /// </para>
    /// <para>
    /// Order matches WinForms: background first, then the control's own appearance, then user paint on top.
    /// </para>
    /// </remarks>
    protected override void OnSnapshot(Gtk.Snapshot snapshot)
    {
        DrawOverride(snapshot, this, Override, background: true);
        base.OnSnapshot(snapshot);
        DrawOverride(snapshot, this, Override, background: false);
    }

    /// <summary>
    /// Bridges a snapshot to the Cairo-based override drawing.
    /// </summary>
    /// <param name="snapshot">The snapshot being built.</param>
    /// <param name="widget">The widget being drawn.</param>
    /// <param name="paintOverride">The control's drawing override.</param>
    /// <param name="background">
    /// <see langword="true" /> to paint the WinForms background, <see langword="false" /> to raise Paint.
    /// </param>
    internal static void DrawOverride(Gtk.Snapshot snapshot, Gtk.Widget widget,
        GtkControlOverride? paintOverride, bool background)
    {
        if (paintOverride == null)
        {
            return;
        }

        var width = widget.AllocatedWidth;
        var height = widget.AllocatedHeight;

        if (width <= 0 || height <= 0)
        {
            return;
        }

        var rect = Graphene.Rect.Alloc();
        rect.Init(0, 0, width, height);

        using var cr = snapshot.AppendCairo(rect);
        var area = new Gdk.Rectangle(0, 0, width, height);

        if (background)
        {
            paintOverride.OnDrawnBackground(cr, area);
        }
        else
        {
            paintOverride.OnPaint(cr, area);
        }
    }

    /// <summary>
    /// Gets or sets the dialog result that is assigned to the owning form when this button is released.
    /// </summary>
    /// <value>
    /// The <see cref="!:DialogResult" /> produced by this button.
    /// </value>
    public System.Windows.Forms.DialogResult DialogResult
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the
    /// <see cref="Gtk.Windows.Forms.Controls.ControlBase.GtkControlOverride" /> instance that centralizes drawing and styling behavior for this control. This override encapsulates background painting (color and image), image drawing, CSS class management and raises paint events so the control can handle custom painting (both direct Cairo via
    /// <see cref="Gtk.Windows.Forms.Controls.ControlBase.PaintGraphicsEventHandler" /> and System.Drawing via
    /// <see cref="!:PaintEventHandler" />). The override is typically created and configured in the control's constructor and invoked from the GTK drawing/show handlers.
    /// </summary>
    /// <value>
    /// The active
    /// <see cref="Gtk.Windows.Forms.Controls.ControlBase.GtkControlOverride" /> used by the control.
    /// </value>
    public GtkControlOverride Override
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the text displayed by the button's child label.
    /// </summary>
    /// <value>
    /// The button caption.
    /// </value>
    public string Text
    {
        get => ((Gtk.Label)Child).Text;
        set => ((Gtk.Label)Child).Text = value;
    }
}