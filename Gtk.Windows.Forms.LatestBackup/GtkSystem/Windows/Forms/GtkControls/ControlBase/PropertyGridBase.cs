/*
 * Based on GTK components, it is a cross-platform interface component that is compatible with the native C# control winform interface.
 * Use this component GtkSystem.Windows.Forms instead of Microsoft.WindowsDesktop.App.WindowsForms, compile once, and run across Windows, Linux, and macOS
 * Technical support 438865652@qq.com，https://www.gtkapp.com, https://gitee.com/easywebfactory, https://github.com/easywebfactory438865652@qq.com，https://www.gtkapp.com, https://gitee.com/easywebfactory, https://github.com/easywebfactory
 * author:chenhongjin
 */

namespace GtkSystem.Windows.Forms.GtkControls.ControlBase;

/// <summary>
/// PropertyGridBase class is a base class for GTK property grid controls, providing common functionality and properties for derived classes. It implements the IControlGtk interface, allowing it to integrate with the GTK system and provide consistent behavior across different types of property grid controls.
/// </summary>
/// <seealso cref="Gtk.Paned" />
/// <seealso cref="GtkSystem.Windows.Forms.GtkControls.ControlBase.IControlGtk" />
public sealed class PropertyGridBase : Gtk.Paned, IControlGtk
{
    /// <summary>
    /// The child1
    /// </summary>
    public Gtk.ScrolledWindow child1 = new();

    /// <summary>
    /// The child2
    /// </summary>
    public Gtk.Box child2 = new(Gtk.Orientation.Vertical,2);

    readonly Gtk.Label descriptionLabel = new("description") { Xalign = 0, Yalign = 0 };

    readonly Gtk.Label titleLabel = new("name") { Xalign = 0, Yalign = 0 };

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="GtkSystem.Windows.Forms.GtkControls.ControlBase.PropertyGridBase" /> class.
    /// </summary>
    public PropertyGridBase() : base(Gtk.Orientation.Vertical)
    {
        this.Override = new GtkControlOverride(this);
        this.Override.AddClass("PropertyGrid");
        this.Valign = Gtk.Align.Start;
        this.Halign = Gtk.Align.Start;
        this.WideHandle = true;
        child2.BorderWidth = 1;
        var attrList = new Pango.AttrList();
        attrList.Insert(new Pango.AttrWeight(Pango.Weight.Bold));
        titleLabel.Attributes = attrList;
        child2.PackStart(titleLabel, false, true, 2);
        child2.PackStart(descriptionLabel, true, true, 2);

        this.Pack1(child1, true, true);
        this.Pack2(child2, false, true);
    }

    /// <summary>
    /// Gets or sets the
    /// <see cref="GtkSystem.Windows.Forms.GtkControls.ControlBase.GtkControlOverride" /> instance that centralizes drawing and styling behavior for this control. This override encapsulates background painting (color and image), image drawing, CSS class management and raises paint events so the control can handle custom painting (both direct Cairo via
    /// <see cref="GtkSystem.Windows.Forms.GtkControls.ControlBase.PaintGraphicsEventHandler" /> and System.Drawing via
    /// <see cref="T:System.Windows.Forms.PaintEventHandler" />). The override is typically created and configured in the control's constructor and invoked from the GTK drawing/show handlers.
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
    /// Shows the description.
    /// </summary>
    /// <param name="title">
    /// The title.
    /// </param>
    /// <param name="description">
    /// The description.
    /// </param>
    public void ShowDescription(string title, string description)
    {
        titleLabel.Text = title;
        descriptionLabel.Text = description;
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