using System.ComponentModel;
using Gtk;

namespace System.Windows.Forms;

/// <summary>
/// Represents a collection of child controls in a table layout container.
/// </summary>
[ListBindable(false)]
public class TableLayoutControlCollection : Control.ControlCollection
{
    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.TableLayoutControlCollection" /> class.
    /// </summary>
    /// <param name="owner">
    /// The owning
    /// <see cref="T:System.Windows.Forms.Control" />.
    /// </param>
    /// <param name="ownerContainer">
    /// The underlying GTK container used to host children.
    /// </param>
    public TableLayoutControlCollection(Control owner, Gtk.Widget ownerContainer) : base(owner, ownerContainer)
    {
        Container = owner as TableLayoutPanel;
    }

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.TableLayoutControlCollection" /> class.
    /// </summary>
    /// <param name="container">
    /// The container.
    /// </param>
    public TableLayoutControlCollection(TableLayoutPanel container) : base(container)
    {
        Container = container;
    }

    /// <summary>
    /// Gets the container.
    /// </summary>
    /// <value>
    /// The container.
    /// </value>
    public TableLayoutPanel? Container { get; }

    /// <summary>
    /// Adds the specified control.
    /// </summary>
    /// <param name="control">
    /// The control.
    /// </param>
    /// <param name="column">
    /// The column.
    /// </param>
    /// <param name="row">
    /// The row.
    /// </param>
    public virtual void Add(Control control, int column, int row)
    {
        control.Location = new Drawing.Point(0, 0);
        control.LockLocation = true;
        control.Parent = Container;
        if (control.Widget != null)
        {
            control.Widget.Margin = 4;
            control.Widget.Valign = Align.Start;
            control.Widget.Halign = Align.Start;
            control.Widget.Hexpand = false;
            if (Container?.grid.GetChildAt(column, row) is Gtk.Viewport view)
            {
                view.Child = control.Widget;
            }
            else
            {
                var viewport = new Gtk.Viewport
                {
                    Vexpand = false, Hexpand = false, Valign = Align.Fill,
                    Halign = Align.Fill,
                    Child = control.Widget
                };
                Container?.grid.Attach(viewport, column, row, 1, 1);
            }
        }

        base.Add(control);
        if (Container != null)
        {
            Container.SetColumn(control, column);
            Container.SetRow(control, row);
        }
    }

    /// <summary>
    /// Removes the specified control from the collection, unwrapping the
    /// <see cref="T:Gtk.Viewport" /> cell that hosts it when present.
    /// </summary>
    /// <param name="value">
    /// The control to remove.
    /// </param>
    public override void Remove(Control? value)
    {
        if (value is null)
        {
            return;
        }

        if (value.Widget?.Parent is Gtk.Viewport view)
        {
            if (view.Child != null)
            {
                view.Remove(view.Child);
            }

            InnerList.Remove(value);
        }
        else
        {
            base.Remove(value);
        }
    }
}