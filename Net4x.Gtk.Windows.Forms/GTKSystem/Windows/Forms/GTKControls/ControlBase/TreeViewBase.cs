using Gtk;

namespace GtkSystem.Windows.Forms.GtkControls.ControlBase
{
    /// <summary>
    /// TreeViewBase class is a base class for GTK tree view controls, providing common functionality and properties for derived classes. It inherits from ScrollableBoxBase, allowing it to support scrolling functionality for the content within the tree view. The class includes a Gtk.TreeView control, which can be used to display hierarchical data in a tree-like structure. By using TreeViewBase, developers can easily create custom tree view controls that are compatible with the GTK system and provide consistent behavior across different platforms.
    /// </summary>
    /// <seealso cref="GtkSystem.Windows.Forms.GtkControls.ControlBase.ScrollableBoxBase" />
    public sealed class TreeViewBase : ScrollableBoxBase
    {
        internal Gtk.TreeView TreeView = new Gtk.TreeView();

        /// <summary>
        /// Initializes a new instance of the <see cref="TreeViewBase"/> class.
        /// </summary>
        public TreeViewBase() : base()
        {
            this.Override.AddClass("TreeView");
            this.TreeView.Valign = Align.Fill;
            this.TreeView.Halign = Align.Fill;
            this.TreeView.BorderWidth = 1;
            this.TreeView.Margin = 0;
            this.TreeView.EnableGridLines = Gtk.TreeViewGridLines.None;
            this.TreeView.EnableTreeLines = true;
            this.TreeView.HeadersVisible = false;
            this.TreeView.ActivateOnSingleClick = false;

            this.AutoScroll = true;
            this.Add(this.TreeView);
        }

    }
}
