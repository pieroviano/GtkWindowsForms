
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
        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="System.Windows.Forms.ContainerControl" /> class.
        /// </summary>
        public ContainerControl() : base()
        {
        }
        /// <summary>
        /// Gets or sets the active control on the container control.
        /// </summary>
        /// <returns>The
        /// <see cref="System.Windows.Forms.Control" /> that is currently active on the
        /// <see cref="System.Windows.Forms.ContainerControl" />
        /// </returns>
        /// <exception cref="System.ArgumentException">The
        /// <see cref="System.Windows.Forms.Control" /> assigned could not be activated.
        /// </exception>
        /// <filterpriority>2
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
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
