namespace GtkSystem.Windows.Forms.GtkControls.ControlBase;

/// <summary>
/// TabControlBase class is a base class for GTK tab control components, providing common functionality and properties for derived classes. It inherits from Gtk.Notebook and implements the IControlGtk interface. The class includes properties for enabling scrolling and disabling the popup menu, as well as an Override property for customizing the appearance of the control. By using TabControlBase, developers can easily create custom tab controls that are compatible with the GTK system and provide consistent behavior across different platforms.
/// </summary>
/// <seealso cref="Gtk.Notebook" />
/// <seealso cref="GtkSystem.Windows.Forms.GtkControls.ControlBase.IControlGtk" />
public sealed class TabControlBase : Gtk.Notebook, IControlGtk
{
    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="GtkSystem.Windows.Forms.GtkControls.ControlBase.TabControlBase" /> class.
    /// </summary>
    public TabControlBase() : base()
    {
        this.Scrollable = true;
        this.EnablePopup = false;
        this.Override = new GtkControlOverride(this);
        this.Override.AddClass("TabControl");
        base.Halign = Gtk.Align.Start;
        base.Valign = Gtk.Align.Start;
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
    /// Adds the class.
    /// </summary>
    /// <param name="cssClass">
    /// The CSS class.
    /// </param>
    public void AddClass(string cssClass)
    {
        this.Override.AddClass(cssClass);
    }

    /// <summary>
    /// Called when the control is drawn.
    /// </summary>
    /// <param name="cr">
    /// The Cairo context.
    /// </param>
    /// <returns>
    /// </returns>
    // Gtk 4 removed the OnDrawn draw vfunc and exposes no overridable OnSnapshot; the WinForms
    // BackColor/background is rendered via CSS in the styling migration instead of a draw override.

    /// <summary>
    /// Called when the control is shown.
    /// </summary>
    protected override void OnShown()
    {
        Override.OnAddClass();
        base.OnShown();
    }
}