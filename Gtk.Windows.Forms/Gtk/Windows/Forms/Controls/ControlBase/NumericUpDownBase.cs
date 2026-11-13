namespace Gtk.Windows.Forms.Controls.ControlBase;

/// <summary>
/// NumericUpDownBase is a base class for NumericUpDown controls in the Gtk.Windows.Forms library. It provides a foundation for creating numeric input controls that allow users to select a value from a range by clicking up and down buttons. The NumericUpDownBase class inherits from Gtk.SpinButton, which is a widget that provides this functionality, and it implements the IControlGtk interface for integration with the Gtk.Windows.Forms framework. This class can be extended to create more specific numeric up-down controls that can be used in applications built with the Gtk.Windows.Forms library.
/// </summary>
/// <seealso cref="Gtk.SpinButton" />
/// <seealso cref="Gtk.Windows.Forms.Controls.ControlBase.IControlGtk" />
public sealed class NumericUpDownBase : Gtk.SpinButton, IControlGtk
{
    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="Gtk.Windows.Forms.Controls.ControlBase.NumericUpDownBase" /> class.
    /// </summary>
    public NumericUpDownBase() : base(0, 100, 1)
    {
        this.Override = new GtkControlOverride(this);
        this.StyleContext.AddClass("NumericUpDown");
        this.Value = 0;
        this.Orientation = Gtk.Orientation.Horizontal;
        base.Valign = Gtk.Align.Start;
        base.Halign = Gtk.Align.Start;
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
}