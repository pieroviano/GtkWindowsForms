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

        // Gtk 4 removed Widget.ShowAll (widgets are visible by default). Each implementor — now a plain
        // Gtk 4 widget — maps it to Show(). (Default interface methods are unavailable on netstandard2.0.)
        void ShowAll();

    }
}