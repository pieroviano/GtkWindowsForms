namespace GtkSystem.Windows.Forms.GtkControls.ControlBase;

/// <summary>
/// DataGridViewBase is a base class for GTK data grid view controls, providing common functionality and properties for derived classes. It implements the IControlGtk interface, allowing it to integrate with the GTK system and provide consistent behavior across different types of data grid view controls.
/// </summary>
/// <seealso cref="GtkSystem.Windows.Forms.GtkControls.ControlBase.ScrollableBoxBase" />
public sealed class DataGridViewBase : ScrollableBoxBase
{

    internal Gtk.TreeView GridView = new Gtk.TreeView();

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="GtkSystem.Windows.Forms.GtkControls.ControlBase.DataGridViewBase" /> class.
    /// </summary>
    public DataGridViewBase() : base()
    {
        this.Override = new GtkControlOverride(this);
        this.Override.AddClass("DataGridView");
        this.Override.BackColor = System.Drawing.Color.White;
        this.BorderWidth = 0;
        this.ShadowType = Gtk.ShadowType.Out;
        GridView.Valign = Gtk.Align.Start;
        GridView.Halign = Gtk.Align.Start;
        GridView.BorderWidth = 0;
        GridView.EnableGridLines = Gtk.TreeViewGridLines.Both;
        GridView.EnableTreeLines = true;
        this.AutoScroll = true;
        this.Add(GridView);
    }

}