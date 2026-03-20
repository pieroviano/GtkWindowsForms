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
        this.Override.AddClass("Label");
        this.Xalign = 0.0f;
        this.Yalign = 0.0f;
        this.Valign = Gtk.Align.Start;
        this.Halign = Gtk.Align.Start;
        this.Wrap = true;
        this.LineWrap = true;
        this.LineWrapMode = Pango.WrapMode.WordChar;
    }

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="GtkSystem.Windows.Forms.GtkControls.ControlBase.LabelBase" /> class.
    /// </summary>
    /// <param name="text">
    /// The text.
    /// </param>
    public LabelBase(string text) : base(text)
    {
        this.Override = new GtkControlOverride(this);
        this.Override.AddClass("Label");
        this.Xalign = 0.0f;
        this.Yalign = 0.0f;
        this.Valign = Gtk.Align.Start;
        this.Halign = Gtk.Align.Start;
        this.Wrap = true;
        this.LineWrap = true;
        this.LineWrapMode = Pango.WrapMode.WordChar;
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
    /// Called when the control is drawn.
    /// </summary>
    /// <param name="cr">
    /// The Cairo context.
    /// </param>
    /// <returns>
    /// </returns>
    protected override bool OnDrawn(Cairo.Context cr)
    {
        var rec = new Gdk.Rectangle(0, 0, this.AllocatedWidth, this.AllocatedHeight);
        Override.OnDrawnBackground(cr, rec);
        Override.OnPaint(cr, rec);

        return base.OnDrawn(cr);
    }

    /// <summary>
    /// Called when the control is shown.
    /// </summary>
    protected override void OnShown()
    {
        Override.OnAddClass();
        base.OnShown();
    }

}