using Gtk;

namespace System.Windows.Forms
{
    public interface IToolMenuItem
    {
        bool Checked
        {
            get;
            set;
        }

        CheckState CheckState
        {
            get;
            set;
        }

        ToolStripItemDisplayStyle DisplayStyle
        {
            get;
            set;
        }

        Gdk.Pixbuf Image
        {
            get;
            set;
        }

        Gtk.Widget MenuItem { get; }

        string Name
        {
            get;
            set;
        }

        string Text
        {
            get;
            set;
        }

        Gtk.Widget ToolItem { get; }

        void ShowAll();

    }
}