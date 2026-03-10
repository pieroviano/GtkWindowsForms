namespace GtkSystem.Windows.Forms.GtkControls.ControlBase;

/// <summary>
/// LinkLabelBase is a base class for GTK link label controls, providing common functionality and properties for derived classes. It implements the IControlGtk interface, allowing it to integrate with the GTK system and provide consistent behavior across different types of link label controls.
/// </summary>
/// <seealso cref="Gtk.LinkButton" />
/// <seealso cref="GtkSystem.Windows.Forms.GtkControls.ControlBase.IControlGtk" />
public sealed class LinkLabelBase : Gtk.LinkButton, IControlGtk
{
    public LinkLabelBase() : base("")
    {
        this.Override = new GtkControlOverride(this);
        this.Override.AddClass("LinkLabel");
        this.BorderWidth = 0;
        base.Valign = Gtk.Align.Start;
        base.Halign = Gtk.Align.Start;
    }

    public GtkControlOverride Override { get; set; }

    protected override bool OnDrawn(Cairo.Context cr)
    {
        Gdk.Rectangle rec = new Gdk.Rectangle(0, 0, this.AllocatedWidth, this.AllocatedHeight);
        Override.OnPaint(cr, rec);

        return base.OnDrawn(cr);
    }

    protected override void OnShown()
    {
        Override.OnAddClass();
        base.OnShown();
    }

}
