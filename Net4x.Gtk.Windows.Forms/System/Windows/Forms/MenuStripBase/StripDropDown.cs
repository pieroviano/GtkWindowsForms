using Gtk;

namespace System.Windows.Forms
{

    public class StripDropDown : Gtk.Menu, IToolMenuItem
    {

        public StripDropDown() { }

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
        }

        public Gtk.Widget MenuItem { get => this; }

        public string Text
        {
            get;
            set;
        }

        public Gtk.Widget ToolItem { get; }
    }
}