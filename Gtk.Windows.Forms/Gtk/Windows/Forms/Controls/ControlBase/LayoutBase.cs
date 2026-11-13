using Gtk;

namespace Gtk.Windows.Forms.Controls.ControlBase;

/// <summary>
/// LayoutBase is a base class for GTK layout controls, providing common functionality and properties for derived classes. It implements the IControlGtk interface, allowing it to integrate with the GTK system and provide consistent behavior across different types of layout controls.
/// </summary>
/// <seealso cref="Gtk.Layout" />
/// <seealso cref="Gtk.Windows.Forms.Controls.ControlBase.IControlGtk" />
public sealed class LayoutBase : Gtk.Fixed, IControlGtk
{
    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="Gtk.Windows.Forms.Controls.ControlBase.LayoutBase" /> class.
    /// </summary>
    /// <param name="hadjustment">
    /// The hadjustment.
    /// </param>
    /// <param name="vadjustment">
    /// The vadjustment.
    /// </param>
    public LayoutBase(Adjustment hadjustment, Adjustment vadjustment) : base()
    {
        this.Override = new GtkControlOverride(this);
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

    // Gtk 4 removed the OnDrawn draw vfunc and exposes no overridable OnSnapshot; the WinForms
    // BackColor/background is rendered via CSS in the styling migration instead of a draw override.

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