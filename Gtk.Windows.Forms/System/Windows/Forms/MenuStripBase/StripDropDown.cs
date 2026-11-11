using Gtk;

namespace System.Windows.Forms;

public class StripDropDown : Gtk.Popover, IToolMenuItem
{

    /// <summary>Gtk 4: widgets are visible by default; ShowAll maps to Show().</summary>
    public new void ShowAll() => Show();

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
    } = null!;

    public Gtk.Widget MenuItem { get => this; }

    public string Text
    {
        get;
        set;
    } = null!;

    public Gtk.Widget ToolItem { get; } = null!;
}