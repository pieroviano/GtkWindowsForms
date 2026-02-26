using Gtk;
using System.ComponentModel;

namespace System.Windows.Forms
{
    /// <summary>
    /// Represents a collection of child controls in a table layout container.
    /// </summary>
    /// <filterpriority>
    /// 2
    /// </filterpriority>
    [ListBindable(false)]
    public class TableLayoutControlCollection : Control.ControlCollection
    {
        public TableLayoutControlCollection(Control owner, Gtk.Container ownerContainer) : base(owner, ownerContainer)
        {
            Container = owner as TableLayoutPanel;
        }
        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="System.Windows.Forms.TableLayoutControlCollection" /> class.
        /// </summary>
        /// <param name="container">The
        /// <see cref="System.Windows.Forms.TableLayoutPanel" /> control that contains the control collection.
        /// </param>
        public TableLayoutControlCollection(TableLayoutPanel container) : base(container)
        {
            Container = container;
        }
        /// <summary>
        /// Adds the specified control to the collection and positions it at the specified cell.
        /// </summary>
        /// <param name="control">The control to add.
        /// </param>
        /// <param name="column">The column in which
        /// <paramref name="control" /> will be placed.
        /// </param>
        /// <param name="row">The row in which
        /// <paramref name="control" /> will be placed.
        /// </param>
        /// <exception cref="System.ArgumentException">Either
        /// <paramref name=" column" /> or
        /// <paramref name="row" /> is less than -1.
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
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
                Gtk.Viewport viewport = new Gtk.Viewport() { Vexpand = false, Hexpand = false };
                viewport.Valign = Align.Fill;
                viewport.Halign = Align.Fill;
                viewport.BorderWidth = 0;
                viewport.Child = control.Widget;
                Container.grid.Attach(viewport, column, row, 1, 1);
            }
            base.Add(control);
            Container.SetColumn(control, column);
            Container.SetRow(control, row);
        }
        /// <summary>
        /// Gets the parent
        /// <see cref="System.Windows.Forms.TableLayoutPanel" /> that contains the controls in the collection.
        /// </summary>
        /// <returns>The
        /// <see cref="System.Windows.Forms.TableLayoutPanel" /> that contains the controls in the current collection.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public TableLayoutPanel Container { get; }
    }
}
