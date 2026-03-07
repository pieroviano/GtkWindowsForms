/*
 * Based on GTK components, it is a cross-platform interface component that is compatible with the native C# control winform interface.
 * Use this component GtkSystem.Windows.Forms instead of Microsoft.WindowsDesktop.App.WindowsForms, compile once, and run across Windows, Linux, and macOS
 * Technical support 438865652@qq.com，https://www.gtkapp.com, https://gitee.com/easywebfactory, https://github.com/easywebfactory438865652@qq.com，https://www.gtkapp.com, https://gitee.com/easywebfactory, https://github.com/easywebfactory
 * author:chenhongjin
 */

using System.ComponentModel;

namespace System.Windows.Forms
{
    /// <summary>
    /// Implements a timer that raises an event at user-defined intervals. This timer is optimized for use in Windows Forms applications and must be used in a window.
    /// </summary>
    /// <filterpriority>
    /// 1
    /// </filterpriority>
    [DefaultEvent("Tick")]
    [DefaultProperty("Interval")]
    [ToolboxItemFilter("System.Windows.Forms")]
    public class Timer : Component
    {

        readonly System.Timers.Timer TimersTimer = new Timers.Timer();

        public event EventHandler Tick;

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="System.Windows.Forms.Timer" /> class.
        /// </summary>
        public Timer()
        {
            TimersTimer.Elapsed += TimersTimer_Elapsed;
        }

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="System.Windows.Forms.Timer" /> class together with the specified container.
        /// </summary>
        /// <param name="container">An
        /// <see cref="System.ComponentModel.IContainer" /> that represents the container for the timer.
        /// </param>
        public Timer(IContainer container) : this()
        {
        }

        /// <summary>
        /// Gets or sets whether the timer is running.
        /// </summary>
        /// <returns>true if the timer is currently enabled; otherwise, false. The default is false.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        [DefaultValue(false)]
        public virtual bool Enabled { get => TimersTimer.Enabled; set => TimersTimer.Enabled = value; }

        /// <summary>
        /// Gets or sets the time, in milliseconds, before the
        /// <see cref="System.Windows.Forms.Timer.Tick" /> event is raised relative to the last occurrence of the
        /// <see cref="System.Windows.Forms.Timer.Tick" /> event.
        /// </summary>
        /// <returns>An
        /// <see cref="System.Int32" /> specifying the number of milliseconds before the
        /// <see cref="System.Windows.Forms.Timer.Tick" /> event is raised relative to the last occurrence of the
        /// <see cref="System.Windows.Forms.Timer.Tick" /> event. The value cannot be less than one.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        [DefaultValue(100)]
        public int Interval { get => (int)TimersTimer.Interval; set => TimersTimer.Interval = value; }

        /// <summary>
        /// Gets or sets an arbitrary string representing some type of user state.
        /// </summary>
        /// <returns>An arbitrary string representing some type of user state.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        [Bindable(true)]
        [DefaultValue(null)]
        [Localizable(false)]
        [TypeConverter(typeof(StringConverter))]
        public object Tag { get; set; }

        /// <summary>
        /// Starts the timer.
        /// </summary>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public void Start()
        {
            TimersTimer.Start();
        }

        /// <summary>
        /// Stops the timer.
        /// </summary>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public void Stop()
        {
            TimersTimer.Stop();
        }

        /// <summary>
        /// Returns a string that represents the
        /// <see cref="System.Windows.Forms.Timer" />
        /// </summary>
        /// <returns>A string that represents the current
        /// <see cref="System.Windows.Forms.Timer" />
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public override string ToString() { return "System.Windows.Forms.Timer"; }

        /// <summary>
        /// Disposes of the resources, other than memory, used by the timer.
        /// </summary>
        /// <param name="disposing">true to release both managed and unmanaged resources. false to release only the unmanaged resources.
        /// </param>
        protected override void Dispose(bool disposing)
        {
            TimersTimer.Dispose();
            base.Dispose();
        }

        protected new void Dispose()
        {
            Dispose(true);
        }

        private void TimersTimer_Elapsed(object sender, Timers.ElapsedEventArgs e)
        {
            if (Tick != null)
                Gtk.Application.Invoke(Tick);
        }

    }
}
