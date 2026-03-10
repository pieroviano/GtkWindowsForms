namespace GtkSystem.Windows.Forms.GtkControls.ControlBase;

/// <summary>
/// ButtonBase is a base class for GTK button controls, providing common functionality and properties for derived classes. It implements the IControlGtk interface, allowing it to integrate with the GTK system and provide consistent behavior across different types of button controls.
/// </summary>
/// <seealso cref="Gtk.Button" />
/// <seealso cref="GtkSystem.Windows.Forms.GtkControls.ControlBase.IControlGtk" />
public sealed class ButtonBase : Gtk.Button, IControlGtk
{
    public ButtonBase() : base(new Gtk.Label() { Wrap = true, SingleLineMode = false, LineWrap = true, LineWrapMode = Pango.WrapMode.WordChar })
    {
        this.Override = new GtkControlOverride(this);
        this.Override.AddClass("Button");
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
