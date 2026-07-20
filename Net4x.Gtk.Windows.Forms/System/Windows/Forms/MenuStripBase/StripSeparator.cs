using Gtk;

namespace System.Windows.Forms
{

    public class StripSeparator : IToolMenuItem
    {

        public Gtk.SeparatorMenuItem menuItem = null;

        public Gtk.SeparatorToolItem toolItem = null;

        public StripSeparator() : base() { }

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

        public Gtk.Widget MenuItem
        {
            get
            {
                if (menuItem == null)
                    menuItem = new Gtk.SeparatorMenuItem() { Visible = true, BorderWidth = 1 };

                return menuItem;
            }
        }

        public string Name
        {
            get;
            set;
        }

        public string Text
        {
            get;
            set;
        }

        public Gtk.Widget ToolItem
        {
            get
            {
                if (toolItem == null)
                    toolItem = new Gtk.SeparatorToolItem() { Visible = true, BorderWidth = 1 };

                return toolItem;
            }
        }

        public void ShowAll()
        {
            toolItem?.ShowAll();
            menuItem?.ShowAll();
        }
    }
}