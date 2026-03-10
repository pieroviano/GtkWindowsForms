namespace GtkSystem.Windows.Forms.GtkControls.ControlBase;

/// <summary>
/// CheckBoxBase is a base class for GTK check button controls, providing common functionality and properties for derived classes. It implements the IControlGtk interface, allowing it to integrate with the GTK system and provide consistent behavior across different types of check button controls.
/// </summary>
/// <seealso cref="Gtk.CheckButton" />
/// <seealso cref="GtkSystem.Windows.Forms.GtkControls.ControlBase.IControlGtk" />
public sealed class CheckBoxBase : Gtk.CheckButton, IControlGtk
{
    public CheckBoxBase() : base()
    {
        this.Override = new GtkControlOverride(this);
        this.Override.AddClass("CheckBox");
        base.Valign = Gtk.Align.Start;
        base.Halign = Gtk.Align.Start;
    }

    public GtkControlOverride Override { get; set; }

    protected override void OnShown()
    {
        Override.OnAddClass();
        base.OnShown();
    }

}
