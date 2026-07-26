namespace GtkSystem.Windows.Forms.GtkControls.ControlBase;

/// <summary>
/// RadioButtonBase class
/// </summary>
/// <seealso cref="Gtk.RadioButton" />
/// <seealso cref="GtkSystem.Windows.Forms.GtkControls.ControlBase.IControlGtk" />
public sealed class RadioButtonBase : Gtk.RadioButton, IControlGtk
{
    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="GtkSystem.Windows.Forms.GtkControls.ControlBase.RadioButtonBase" /> class.
    /// </summary>
    public RadioButtonBase() : base(new Gtk.RadioButton("baseradio"))
    {
        this.Override = new GtkControlOverride(this);
        this.StyleContext.AddClass("RadioButton");
        base.Valign = Gtk.Align.Start;
        base.Halign = Gtk.Align.Start;
    }

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="GtkSystem.Windows.Forms.GtkControls.ControlBase.RadioButtonBase" /> class.
    /// </summary>
    /// <param name="radio_group_member">
    /// The radio group member.
    /// </param>
    public RadioButtonBase(Gtk.RadioButton radio_group_member) : base(radio_group_member)
    {
        this.Override = new GtkControlOverride(this);
        this.StyleContext.AddClass("RadioButton");
        base.Valign = Gtk.Align.Start;
        base.Halign = Gtk.Align.Start;
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
}