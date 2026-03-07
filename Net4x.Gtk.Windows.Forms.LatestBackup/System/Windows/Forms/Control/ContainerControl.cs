
namespace System.Windows.Forms
{
    /// <summary>
    /// Provides focus-management functionality for controls that can function as a container for other controls.
    /// </summary>
    /// <filterpriority>
    /// 2
    /// </filterpriority>
    public class ContainerControl : ScrollableControl, IContainerControl
    {

        private Control _ActiveControl;
        public ContainerControl() : base()
        {
        }
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

        public virtual StatusStrip StatusStrip { get; set; }
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
}
