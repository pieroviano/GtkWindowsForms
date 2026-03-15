
namespace System.Windows.Forms;

/// <summary>
/// Provides focus-management functionality for controls that can function as a container for other controls.
/// </summary>
/// <filterpriority>
/// 2
/// </filterpriority>
public class ContainerControl : ScrollableControl, IContainerControl
{

    private Control _ActiveControl;

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.ContainerControl" /> class.
    /// </summary>
    public ContainerControl() : base()
    {
    }

    /// <summary>
    /// Gets or sets the control that is active on the container control.
    /// </summary>
    public Control ActiveControl
    {
        get
        {
            foreach (object control in this.Controls)
            {
                if (control is Control con)
                {
                    if ((con.Widget.StateFlags & Gtk.StateFlags.Active) != 0 || con.Widget.IsFocus)
                        return con;
                }
            }

            return _ActiveControl;
        }
        set { ActivateControl(value); }
    }

    /// <summary>
    /// Gets or sets the status strip.
    /// </summary>
    /// <value>
    /// The status strip.
    /// </value>
    public virtual StatusStrip StatusStrip { get; set; }

    /// <summary>
    /// Activates a specified control.
    /// </summary>
    /// <param name="active">
    /// The
    /// <see cref="T:System.Windows.Forms.Control" /> being activated.
    /// </param>
    /// <returns>
    /// true if the control is successfully activated; otherwise, false.
    /// </returns>
    public bool ActivateControl(Control active)
    {
        _ActiveControl = active;
        if (active != null)
        {
            if (active.GtkControl is Gtk.Widget widget)
            {
                return widget.Activate();
            }
        }

        return false;
    }

}