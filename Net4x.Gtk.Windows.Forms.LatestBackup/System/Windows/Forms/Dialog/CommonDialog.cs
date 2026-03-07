/*
 * Based on GTK components, it is a cross-platform interface component that is compatible with the native C# control winform interface.
 * Use this component GtkSystem.Windows.Forms instead of Microsoft.WindowsDesktop.App.WindowsForms, compile once, and run across Windows, Linux, and macOS
 * Technical support 438865652@qq.com，https://www.gtkapp.com, https://gitee.com/easywebfactory, https://github.com/easywebfactory438865652@qq.com，https://www.gtkapp.com, https://gitee.com/easywebfactory, https://github.com/easywebfactory
 * author:chenhongjin
 */
using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;

namespace System.Windows.Forms
{
    /// <summary>
    /// Specifies the base class used for displaying dialog boxes on the screen.
    /// </summary>
    /// <filterpriority>
    /// 1
    /// </filterpriority>
    [ToolboxItemFilter("System.Windows.Forms")]
    public abstract class CommonDialog : Component
    {
        private static readonly object s_helpRequestEvent = new object();

        public event EventHandler HelpRequest { add => Events.AddHandler(s_helpRequestEvent, value); remove => Events.RemoveHandler(s_helpRequestEvent, value); }
        public CommonDialog()
        {
        }

        /// <summary>
        /// Gets or sets an object that contains data about the control.
        /// </summary>
        /// <returns>The object that contains data about the
        /// <see cref = "System.Windows.Forms.CommonDialog"/>
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public object Tag { get; set; }

        /// <summary>
        /// When overridden in a derived class, resets the properties of a common dialog box to their default values.
        /// </summary>
        /// <filterpriority>1
        /// </filterpriority>
        public abstract void Reset();

        /// <summary>
        /// Runs a common dialog box with a default owner.
        /// </summary>
        /// <returns>
        /// <see cref = "System.Windows.Forms.DialogResult.OK"/> if the user clicks OK in the dialog box; otherwise,
        /// <see cref = "System.Windows.Forms.DialogResult.Cancel"/>
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence"/>
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        /// </PermissionSet>
        public virtual DialogResult ShowDialog() => ShowDialog(owner: null);

        /// <summary>
        /// Runs a common dialog box with the specified owner.
        /// </summary>
        /// <returns>
        /// <see cref = "System.Windows.Forms.DialogResult.OK"/> if the user clicks OK in the dialog box; otherwise,
        /// <see cref = "System.Windows.Forms.DialogResult.Cancel"/>
        /// </returns>
        /// <param name = "owner">Any object that implements
        /// <see cref = "System.Windows.Forms.IWin32Window"/> that represents the top-level window that will own the modal dialog box.
        /// </param>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence"/>
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        /// </PermissionSet>
        public virtual DialogResult ShowDialog(IWin32Window owner)
        {
            DialogResult result = DialogResult.Cancel;
            bool runresult = RunDialog(owner);
            if (runresult)
            {
                result = DialogResult.OK;
            }

            return result;
        }

        /// <summary>
        /// Raises the
        /// <see cref = "System.Windows.Forms.CommonDialog.HelpRequest"/> event.
        /// </summary>
        /// <param name = "e">An
        /// <see cref = "System.Windows.Forms.HelpEventArgs"/> that provides the event data.
        /// </param>
        protected virtual void OnHelpRequest(EventArgs e)
        {
            EventHandler handler = (EventHandler)Events[s_helpRequestEvent];
            handler?.Invoke(this, e);
        }

        protected abstract bool RunDialog(IWin32Window owner);
    }
}
