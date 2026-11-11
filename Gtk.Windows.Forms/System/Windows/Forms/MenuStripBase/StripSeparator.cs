using Gtk;

namespace System.Windows.Forms;

public class StripSeparator : Gtk.Separator, IToolMenuItem
{

    public Gtk.Separator? menuItem = null;

    public Gtk.Separator? toolItem = null;

    public StripSeparator() : base(Gtk.Orientation.Horizontal) { } // Gtk4: Separator needs orientation

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

    public Gtk.Widget MenuItem
    {
        get
        {
            if (menuItem == null)
                menuItem = new Gtk.Separator(Gtk.Orientation.Horizontal) { Visible = true };

            return menuItem;
        }
    }

    public new string Name
    {
        get;
        set;
    } = null!;

    public string Text
    {
        get;
        set;
    } = null!;

    public Gtk.Widget ToolItem
    {
        get
        {
            if (toolItem == null)
                toolItem = new Gtk.Separator(Gtk.Orientation.Horizontal) { Visible = true };

            return toolItem;
        }
    }

    public new void ShowAll()
    {
        toolItem?.Show();
        menuItem?.Show();
    }
}