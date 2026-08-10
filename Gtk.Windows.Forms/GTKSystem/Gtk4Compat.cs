using System.Collections.Generic;

namespace Gtk
{
    /// <summary>
    /// Backend-only helpers that bridge the Gtk 3 <c>Gtk.Container</c> API this shim was
    /// originally written against to Gtk 4's per-widget child model. Gtk 4 removed
    /// <c>Container.Add</c>/<c>Remove</c>/<c>Children</c>; each container type now exposes its
    /// own child API (<c>Append</c>, a <c>Child</c> property, <c>Put</c>, <c>Attach</c>, ...).
    /// These extensions dispatch on the runtime container type so the generated GTK-backing
    /// code keeps a single uniform call shape. No <c>System.Windows.Forms</c> /
    /// <c>System.Drawing</c> API is affected — this is purely the GTK backend.
    /// </summary>
    public static class Gtk4Compat
    {
        /// <summary>
        /// Materializes a widget's direct children into an array. Replaces Gtk 3's
        /// <c>Container.Children</c> (removed in Gtk 4) using the <see cref="Widget.FirstChild"/>
        /// / <see cref="Widget.NextSibling"/> sibling chain.
        /// </summary>
        /// <param name="parent">The widget whose direct children are enumerated.</param>
        /// <returns>The direct child widgets, in order; an empty array when there are none.</returns>
        public static Widget[] GetChildrenArray(this Widget parent)
        {
            var list = new List<Widget>();
            if (parent != null)
            {
                for (var child = parent.FirstChild; child != null; child = child.NextSibling)
                {
                    list.Add(child);
                }
            }

            return list.ToArray();
        }

        /// <summary>
        /// Best-effort replacement for Gtk 3's <c>widget.Window.GetOrigin(out x, out y)</c>
        /// (Gtk 4 removed <c>Gdk.Window</c> and, on Wayland, has no global screen coordinates).
        /// Returns the widget origin translated into its top-level root's coordinate space,
        /// falling back to the widget allocation. Used by the WinForms <c>PointToScreen</c> /
        /// caret / tooltip anchor code paths, whose signatures are unchanged.
        /// </summary>
        /// <param name="widget">The widget whose origin is requested.</param>
        /// <param name="x">Receives the X origin.</param>
        /// <param name="y">Receives the Y origin.</param>
        public static void GetOrigin(this Widget widget, out int x, out int y)
        {
            x = 0;
            y = 0;
            if (widget == null)
            {
                return;
            }

            var root = widget;
            while (root.Parent != null)
            {
                root = root.Parent;
            }

            if (!ReferenceEquals(root, widget) &&
                widget.TranslateCoordinates(root, 0, 0, out var dx, out var dy))
            {
                x = (int)dx;
                y = (int)dy;
                return;
            }

            var allocation = widget.Allocation;
            x = allocation.X;
            y = allocation.Y;
        }

        /// <summary>
        /// Adds <paramref name="child"/> to <paramref name="parent"/>, dispatching to the
        /// Gtk 4 child API appropriate to the container's runtime type. Replaces Gtk 3's
        /// universal <c>Container.Add</c>.
        /// </summary>
        /// <param name="parent">The container widget to add to.</param>
        /// <param name="child">The child widget to add.</param>
        public static void AddChild(this Widget parent, Widget child)
        {
            switch (parent)
            {
                case Box box:
                    box.Append(child);
                    break;
                case FlowBox flowBox:
                    flowBox.Append(child);
                    break;
                case ListBox listBox:
                    listBox.Append(child);
                    break;
                case Fixed @fixed:
                    @fixed.Put(child, 0, 0);
                    break;
                case Grid grid:
                    grid.Attach(child, 0, 0, 1, 1);
                    break;
                case Overlay overlay:
                    if (overlay.Child == null)
                    {
                        overlay.Child = child;
                    }
                    else
                    {
                        overlay.AddOverlay(child);
                    }

                    break;
                case ScrolledWindow scrolledWindow:
                    scrolledWindow.Child = child;
                    break;
                case Viewport viewport:
                    viewport.Child = child;
                    break;
                case Frame frame:
                    frame.Child = child;
                    break;
                case FlowBoxChild flowBoxChild:
                    flowBoxChild.Child = child;
                    break;
                case ListBoxRow listBoxRow:
                    listBoxRow.Child = child;
                    break;
                case Notebook notebook:
                    notebook.AppendPage(child, null);
                    break;
                case Expander expander:
                    expander.Child = child;
                    break;
                case Button button:
                    button.Child = child;
                    break;
                case Window window:
                    window.Child = child;
                    break;
                case Popover popover:
                    popover.Child = child;
                    break;
                default:
                    // Last resort: reparent by hand. Works for any single-child container
                    // exposing the GtkWidget parent slot.
                    if (child.Parent != null)
                    {
                        child.Unparent();
                    }

                    child.Parent = parent;
                    break;
            }
        }

        /// <summary>
        /// Removes <paramref name="child"/> from <paramref name="parent"/>, dispatching to the
        /// Gtk 4 child API appropriate to the container's runtime type. Replaces Gtk 3's
        /// universal <c>Container.Remove</c>.
        /// </summary>
        /// <param name="parent">The container widget to remove from.</param>
        /// <param name="child">The child widget to remove.</param>
        public static void RemoveChild(this Widget parent, Widget child)
        {
            switch (parent)
            {
                case Box box:
                    box.Remove(child);
                    break;
                case FlowBox flowBox:
                    flowBox.Remove(child);
                    break;
                case ListBox listBox:
                    listBox.Remove(child);
                    break;
                case Fixed @fixed:
                    @fixed.Remove(child);
                    break;
                case Grid grid:
                    grid.Remove(child);
                    break;
                case Overlay overlay when overlay.Child == child:
                    overlay.Child = null;
                    break;
                case Overlay overlay:
                    overlay.RemoveOverlay(child);
                    break;
                case ScrolledWindow scrolledWindow when scrolledWindow.Child == child:
                    scrolledWindow.Child = null;
                    break;
                case Viewport viewport when viewport.Child == child:
                    viewport.Child = null;
                    break;
                case Frame frame when frame.Child == child:
                    frame.Child = null;
                    break;
                case FlowBoxChild flowBoxChild when flowBoxChild.Child == child:
                    flowBoxChild.Child = null;
                    break;
                case ListBoxRow listBoxRow when listBoxRow.Child == child:
                    listBoxRow.Child = null;
                    break;
                case Notebook notebook:
                    var pageNum = notebook.PageNum(child);
                    if (pageNum >= 0)
                    {
                        notebook.RemovePage(pageNum);
                    }

                    break;
                case Window window when window.Child == child:
                    window.Child = null;
                    break;
                default:
                    child.Unparent();
                    break;
            }
        }
    }
}
