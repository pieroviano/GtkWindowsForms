namespace GtkSystem.Windows.Forms.GtkControls.ControlBase;

/// <summary>
/// FlowLayoutPanelBase is a base class for GTK flow layout panel controls, providing common functionality and properties for derived classes. It implements the IControlGtk interface, allowing it to integrate with the GTK system and provide consistent behavior across different types of flow layout panel controls.
/// </summary>
/// <seealso cref="Gtk.FlowBox" />
/// <seealso cref="GtkSystem.Windows.Forms.GtkControls.ControlBase.IControlGtk" />
public sealed class FlowLayoutPanelBase : Gtk.FlowBox, IControlGtk
{
    public FlowLayoutPanelBase() : base()
    {
        this.Override = new GtkControlOverride(this);
        this.Override.AddClass("FlowLayoutPanel");
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
