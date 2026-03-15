using System;
using Gtk;

namespace GtkSystem.Windows.Forms.GtkControls.ControlBase;

/// <summary>
/// TabPageBase class is a base class for GTK tab page controls, providing common functionality and properties for derived classes. It inherits from ScrollableBoxBase, allowing it to support scrolling functionality for the content within the tab page. The class includes a Gtk.Overlay control, which can be used to display additional content or controls on top of the main content of the tab page. By using TabPageBase, developers can easily create custom tab page controls that are compatible with the GTK system and provide consistent behavior across different platforms.
/// </summary>
/// <seealso cref="GtkSystem.Windows.Forms.GtkControls.ControlBase.ScrollableBoxBase" />
public sealed class TabPageBase : ScrollableBoxBase
{
    /// <summary>
    /// The content
    /// </summary>
    public Gtk.Overlay Content = new Gtk.Overlay();

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="GtkSystem.Windows.Forms.GtkControls.ControlBase.TabPageBase" /> class.
    /// </summary>
    public TabPageBase() : base()
    {
        this.Override = new GtkControlOverride(this);
        this.Override.AddClass("TabPage");
        this.BorderWidth = 0;
        this.Content.Margin = 0;
        this.Content.Halign = Align.Fill;
        this.Content.Valign = Align.Fill;
        this.Content.Expand = false;
        this.Content.Add(new Gtk.Fixed() { Halign = Align.Fill, Valign = Align.Fill });
        base.Halign = Align.Fill;
        base.Valign = Align.Fill;
        base.Add(Content);
    }

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
        var rec = new Gdk.Rectangle(0, 0, this.AllocatedWidth, this.AllocatedHeight);
        Override.OnPaint(cr, rec);

        return base.OnDrawn(cr);
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