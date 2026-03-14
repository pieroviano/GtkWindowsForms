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
    /// <param name="ownerContainer">The underlying GTK container used to host children.
    /// </param>
    public TableLayoutControlCollection(Control owner, Gtk.Container ownerContainer) : base(owner, ownerContainer)
    {
        Container = owner as TableLayoutPanel;
    }

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.TableLayoutControlCollection" /> class.
    /// </summary>
    /// <param name="container">The container.
    /// </param>
    public TableLayoutControlCollection(TableLayoutPanel container) : base(container)
    {
        Container = container;
    }

    /// <summary>
    /// Gets the container.
    /// </summary>
    /// <value>The container.
    /// </value>
    public TableLayoutPanel Container { get; }

    /// <summary>
    /// Adds the specified control.
    /// </summary>
    /// <param name="control">The control.
    /// </param>
    /// <param name="column">The column.
    /// </param>
    /// <param name="row">The row.
    /// </param>
    public virtual void Add(Control control, int column, int row)
    {
        control.Location = new Drawing.Point(0, 0);
        control.LockLocation = true;
        control.Parent = Container;
        control.Widget.Margin = 4;
        control.Widget.Valign = Align.Start;
        control.Widget.Halign = Align.Start;
        control.Widget.Hexpand = false;
        if (Container.grid.GetChildAt(column, row) is Gtk.Viewport view)
        {
            view.Child = control.Widget;
        }
        else
        {
            Gtk.Viewport viewport = new Gtk.Viewport
            {
                Vexpand = false, Hexpand = false, Valign = Align.Fill,
                Halign = Align.Fill,
                BorderWidth = 0,
                Child = control.Widget
            };
            Container.grid.Attach(viewport, column, row, 1, 1);
        }
        base.Add(control);
        Container.SetColumn(control, column);
        Container.SetRow(control, row);
    }

}