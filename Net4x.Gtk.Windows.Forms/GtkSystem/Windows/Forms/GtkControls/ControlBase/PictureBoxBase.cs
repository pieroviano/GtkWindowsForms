using System;

namespace GtkSystem.Windows.Forms.GtkControls.ControlBase;

/// <summary>
/// PictureBoxBase class.
/// </summary>
/// <seealso cref="Gtk.Image" />
/// <seealso cref="GtkSystem.Windows.Forms.GtkControls.ControlBase.IControlGtk" />
public sealed class PictureBoxBase : Gtk.Image, IControlGtk
{
    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="GtkSystem.Windows.Forms.GtkControls.ControlBase.PictureBoxBase" /> class.
    /// </summary>
    public PictureBoxBase() : base()
    {
        this.Override = new GtkControlOverride(this);
        this.Override.AddClass("PictureBox");
        base.Valign = Gtk.Align.Fill;
        base.Halign = Gtk.Align.Fill;
        base.Expand = true;
        base.Xalign = 0;
        base.Yalign = 0;
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
    public GtkControlOverride Override { get; set; }

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
        if (this.Pixbuf != null)
        {
            cr.Scale(this.AllocatedWidth * 1f / this.Pixbuf.Width * 1f, this.AllocatedHeight * 1f / this.Pixbuf.Height * 1f);
        }
        var rec = new Gdk.Rectangle(0, 0, this.AllocatedWidth, this.AllocatedHeight);
        Override.OnDrawnBackground(cr, rec);
        Override.OnPaint(cr, rec);
        base.OnDrawn(cr);

        return true;
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