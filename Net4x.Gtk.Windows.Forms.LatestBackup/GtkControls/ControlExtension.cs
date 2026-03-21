using System.Windows.Forms;

namespace GtkControls;

internal static class ControlExtension
{
    public static GridCell GetGridCell(this Control control, Gtk.Grid grid)
    {
        var cell = new GridCell();
        var widget = control.Widget.Parent;

        cell.LeftAttach = ReadProperty(grid, widget, "left-attach", out var success);
        if (!success)
        {
            return null;
        }
        cell.TopAttach = ReadProperty(grid, widget, "top-attach", out success);
        if (!success)
        {
            return null;
        }
        cell.Width = ReadProperty(grid, widget, "width", out success);
        if (!success)
        {
            return null;
        }
        cell.Height = ReadProperty(grid, widget, "height", out success);
        if (!success)
        {
            return null;
        }

        return cell;
    }

    private static int ReadProperty(Gtk.Grid grid, Gtk.Widget widget, string propertyName, out bool success)
    {
        var value = grid.ChildGetProperty(widget, propertyName);

        if (value.Val is int col)
        {
            success = true;
            return col;
        }
        else
        {
            success = false;
            return 0;
        }
    }
}