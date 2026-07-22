using System.Windows.Forms;

namespace GtkControls;

internal static class ControlExtension
{
    public static GridCell? GetGridCell(this Control control, Gtk.Grid grid)
    {
        var cell = new GridCell();
        var widget = control.Widget?.Parent;

        if (widget == null)
        {
            return null;
        }
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
        // Grid child-properties only exist for direct children of the grid. Asking for one on a widget
        // that is not a child makes GTK emit an "invalid child property id" warning - fatal in the test
        // host, and (firing during idle layout) it crashes whichever test happens to be running. Return
        // the default 0 without querying GTK, matching the prior behaviour (ChildGetProperty yielded 0 for
        // a non-child) minus the warning, so GetGridCell still produces a non-null zero cell.
        if (Array.IndexOf(grid.Children, widget) < 0)
        {
            success = true;
            return 0;
        }

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