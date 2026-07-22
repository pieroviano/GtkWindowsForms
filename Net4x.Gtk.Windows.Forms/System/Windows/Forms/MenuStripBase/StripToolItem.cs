using Gtk;

namespace System.Windows.Forms
{

    public class StripToolItem : Gtk.ToolItem, IToolMenuItem
    {

        public StripToolItem() : base() { this.Homogeneous = false; }

        public bool Checked
        {
            get;
            set;
        }

        public CheckState CheckState
        {
            get;
            set;
        }

        public ToolStripItemDisplayStyle DisplayStyle
        {
            get;
            set;
        }

        public Gdk.Pixbuf Image
        {
            get;
            set;
        } = null!;

        public Gtk.Widget MenuItem { get; } = null!;

        public string Text
        {
            get;
            set;
        } = null!;

        public Gtk.Widget ToolItem { get => this; }
    }
}