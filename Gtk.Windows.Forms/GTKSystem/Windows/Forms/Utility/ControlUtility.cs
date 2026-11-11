using System.Windows.Forms;
using GtkSystem.Windows.Forms.GtkControls.ControlBase;

namespace GtkSystem.Windows.Forms;

public static class ControlUtility
{
    public static Gtk.Widget? GetIControl(Gtk.Widget widget)
    {
        if (widget == null)
            return null;
        else if (widget is IControlGtk)
            return widget;
        else
            return GetIControl(widget.Parent);
    }

    public static Control? ToControl(this Gtk.Widget widget)
    {
        var pwidget = GetIControl(widget);
        return pwidget?.Data["Control"] as Control;
    }

    public static bool TryGetIControl(this Gtk.Widget widget, out Gtk.Widget? result)
    {
        result = GetIControl(widget);

        return result != null;
    }

    public static bool TryToControl<T>(this Gtk.Widget widget, GLib.GType gtype, out T? control)
    {
        Gtk.Widget pwidget = widget.GetAncestor(gtype);
        if (pwidget != null && pwidget.Data["Control"] is T tcontrol)
        {
            control = tcontrol;
            return true;
        }
        control = default(T);

        return false;
    }

    public static bool TryToControl<T>(this Gtk.Widget widget, out T? control)
    {
        var pwidget = GetIControl(widget);
        if (pwidget != null && pwidget.Data["Control"] is T tcontrol)
        {
            control = tcontrol;
            return true;
        }
        control = default(T);

        return false;
    }
}