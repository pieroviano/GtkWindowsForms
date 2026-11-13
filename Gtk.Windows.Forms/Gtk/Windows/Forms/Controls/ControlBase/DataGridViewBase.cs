namespace Gtk.Windows.Forms.Controls.ControlBase;

/// <summary>
/// DataGridViewBase is a base class for GTK data grid view controls, providing common functionality and properties for derived classes. It implements the IControlGtk interface, allowing it to integrate with the GTK system and provide consistent behavior across different types of data grid view controls.
/// </summary>
/// <seealso cref="Gtk.Windows.Forms.Controls.ControlBase.ScrollableBoxBase" />
public sealed class DataGridViewBase : ScrollableBoxBase
{

    internal Gtk.TreeView GridView = new();

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="Gtk.Windows.Forms.Controls.ControlBase.DataGridViewBase" /> class.
    /// </summary>
    public DataGridViewBase() : base()
    {
        this.Override = new GtkControlOverride(this);
        this.StyleContext.AddClass("DataGridView");
        this.Override.BackColor = System.Drawing.Color.White;
        GridView.Valign = Gtk.Align.Start;
        GridView.Halign = Gtk.Align.Fill;
        GridView.EnableGridLines = Gtk.TreeViewGridLines.Both;
        GridView.EnableTreeLines = true;
        this.AutoScroll = true;
        this.Add(GridView);
    }
}