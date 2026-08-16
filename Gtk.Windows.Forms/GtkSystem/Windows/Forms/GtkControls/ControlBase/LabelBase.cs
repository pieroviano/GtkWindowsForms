namespace GtkSystem.Windows.Forms.GtkControls.ControlBase;

/// <summary>
/// LabelBase is a base class for GTK label controls, providing common functionality and properties for derived classes. It implements the IControlGtk interface, allowing it to integrate with the GTK system and provide consistent behavior across different types of label controls. The LabelBase class includes features such as text alignment, word wrapping, and custom drawing capabilities.
/// </summary>
/// <seealso cref="Gtk.Label" />
/// <seealso cref="GtkSystem.Windows.Forms.GtkControls.ControlBase.IControlGtk" />
public sealed class LabelBase : Gtk.Label, IControlGtk
{
    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="GtkSystem.Windows.Forms.GtkControls.ControlBase.LabelBase" /> class.
    /// </summary>
    public LabelBase() : base()
    {
        this.Override = new GtkControlOverride(this);
        this.StyleContext.AddClass("Label");
        this.Xalign = 0.0f;
        this.Yalign = 0.0f;
        this.Valign = Gtk.Align.Start;
        this.Halign = Gtk.Align.Start;
        this.Wrap = true;
        this.Wrap = true;
        this.LineWrapMode = Pango.WrapMode.WordChar;
        MatchWinFormsSelectionBehaviour(this);
        // Gtk 4: Widget.Drawn is gone and there is no overridable OnSnapshot; custom paint moves to CSS (deferred).
    }

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="GtkSystem.Windows.Forms.GtkControls.ControlBase.LabelBase" /> class.
    /// </summary>
    /// <param name="text">
    /// The text.
    /// </param>
    public LabelBase(string? text) : base(text)
    {
        this.Override = new GtkControlOverride(this);
        this.StyleContext.AddClass("Label");
        this.Xalign = 0.0f;
        this.Yalign = 0.0f;
        this.Valign = Gtk.Align.Start;
        this.Halign = Gtk.Align.Start;
        this.Wrap = true;
        this.Wrap = true;
        this.LineWrapMode = Pango.WrapMode.WordChar;
        MatchWinFormsSelectionBehaviour(this);
        // Gtk4-removed (custom paint -> CSS/DrawFunc, deferred): this.Drawn += LabelBase_Drawn;
    }

    /// <summary>
    /// Takes a label out of the focus chain and out of the text-selection business, the way WinForms has it.
    /// </summary>
    /// <param name="label">
    /// The label to configure.
    /// </param>
    /// <remarks>
    /// A WinForms <see cref="T:System.Windows.Forms.Label" /> has <c>ControlStyles.Selectable</c> clear and
    /// <c>TabStop</c> false: it never takes focus and its text can never be selected. Upstream nonetheless
    /// set <c>Selectable</c>, which under Gtk3 was merely a mouse affordance. Gtk4 makes a selectable label
    /// focusable, so the window's initial focus could land on one, and
    /// <c>gtk_label_grab_focus</c> honours <c>gtk-label-select-on-focus</c> by selecting the whole caption -
    /// which the theme then paints as an opaque selection box behind the text. Clearing both settings is a
    /// deliberate divergence from the vendored Gtk3 build; it is the WinForms contract that governs here.
    /// </remarks>
    private static void MatchWinFormsSelectionBehaviour(Gtk.Label label)
    {
        label.Selectable = false;
        label.Focusable = false;
    }

    /// <summary>
    /// Gets or sets the
    /// <see cref="GtkSystem.Windows.Forms.GtkControls.ControlBase.GtkControlOverride" /> instance that centralizes drawing and styling behavior for this control. This override encapsulates background painting (color and image), image drawing, CSS class management and raises paint events so the control can handle custom painting (both direct Cairo via
    /// <see cref="GtkSystem.Windows.Forms.GtkControls.ControlBase.PaintGraphicsEventHandler" /> and System.Drawing via
    /// <see cref="!:PaintEventHandler" />). The override is typically created and configured in the control's constructor and invoked from the GTK drawing/show handlers.
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
    /// Draws the WinForms background and raises <c>Paint</c> around GTK's own rendering.
    /// </summary>
    /// <param name="snapshot">The snapshot GTK is building.</param>
    /// <remarks>
    /// Gtk4 removed <c>Widget.Drawn</c>, but the <c>snapshot</c> vfunc is overridable and
    /// <see cref="Gtk.Snapshot.AppendCairo" /> yields a Cairo context - so the existing drawing code runs
    /// unchanged. Without this the control's <c>BackgroundImage</c> was never painted and its <c>Paint</c>
    /// handlers never ran.
    /// </remarks>
    protected override void OnSnapshot(Gtk.Snapshot snapshot)
    {
        ButtonBase.DrawOverride(snapshot, this, Override, background: true);
        base.OnSnapshot(snapshot);
        ButtonBase.DrawOverride(snapshot, this, Override, background: false);
    }
}
