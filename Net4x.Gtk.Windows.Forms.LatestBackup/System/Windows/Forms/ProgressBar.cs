/*
 * Based on GTK components, it is a cross-platform interface component that is compatible with the native C# control winform interface.
 * Use this component GtkSystem.Windows.Forms instead of Microsoft.WindowsDesktop.App.WindowsForms, compile once, and run across Windows, Linux, and macOS
 * Technical support 438865652@qq.com，https://www.gtkapp.com, https://gitee.com/easywebfactory, https://github.com/easywebfactory438865652@qq.com，https://www.gtkapp.com, https://gitee.com/easywebfactory, https://github.com/easywebfactory
 * author:chenhongjin
 */
using GtkSystem.Windows.Forms.GtkControls.ControlBase;
using System.ComponentModel;

namespace System.Windows.Forms
{
    /// <summary>
    /// Represents a Windows progress bar control.
    /// </summary>
    /// <filterpriority>
    /// 1
    /// </filterpriority>
    [DesignerCategory("Component")]
    [DefaultProperty("Value")]
    public class ProgressBar : Control
    {

        private ProgressBarBase self = new ProgressBarBase();

        public override object GtkControl => self;
        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="System.Windows.Forms.ProgressBar" /> class.
        /// </summary>
        public ProgressBar() : base()
        {
            self.Realized += Control_Realized;
        }

        private void Control_Realized(object sender, EventArgs e)
        {
            self.MaxValue = Maximum;
            self.MinValue = Minimum;
            self.Value = Value;
        }
        /// <summary>
        /// Gets or sets the manner in which progress should be indicated on the progress bar.
        /// </summary>
        /// <returns>One of the
        /// <see cref="System.Windows.Forms.ProgressBarStyle" /> values. The default is
        /// <see cref="System.Windows.Forms.ProgressBarStyle.Blocks" />
        /// </returns>
        /// <exception cref="System.ComponentModel.InvalidEnumArgumentException">The value is not a member of the
        /// <see cref="System.Windows.Forms.ProgressBarStyle" /> enumeration.
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public ProgressBarStyle Style { get; set; }
        /// <summary>
        /// Gets or sets the time period, in milliseconds, that it takes the progress block to scroll across the progress bar.
        /// </summary>
        /// <returns>The time period, in milliseconds, that it takes the progress block to scroll across the progress bar.
        /// </returns>
        /// <exception cref="System.ArgumentOutOfRangeException">The indicated time period is less than 0.
        /// </exception>
        [DefaultValue(100)]
        public int MarqueeAnimationSpeed { get; set; } = 100;
        /// <summary>
        /// Gets or sets the maximum value of the range of the control.
        /// </summary>
        /// <returns>The maximum value of the range. The default is 100.
        /// </returns>
        /// <exception cref="System.ArgumentException">The value specified is less than 0.
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        [DefaultValue(100)]
        public int Maximum { get; set; } = 100;
        /// <summary>
        /// Gets or sets the minimum value of the range of the control.
        /// </summary>
        /// <returns>The minimum value of the range. The default is 0.
        /// </returns>
        /// <exception cref="System.ArgumentException">The value specified for the property is less than 0.
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        [DefaultValue(0)]
        public int Minimum { get; set; } = 0;
        /// <summary>
        /// Gets or sets the space between the edges of a
        /// <see cref="System.Windows.Forms.ProgressBar" /> control and its contents.
        /// </summary>
        /// <returns>
        /// <see cref="System.Windows.Forms.Padding.Empty" /> in all cases.
        /// </returns>
        public new Padding Padding { get; set; }
        /// <summary>
        /// Gets or sets the amount by which a call to the
        /// <see cref="System.Windows.Forms.ProgressBar.PerformStep" /> method increases the current position of the progress bar.
        /// </summary>
        /// <returns>The amount by which to increment the progress bar with each call to the
        /// <see cref="System.Windows.Forms.ProgressBar.PerformStep" /> method. The default is 10.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        [DefaultValue(10)]
        public int Step { get; set; }
        /// <summary>
        /// Gets or sets the current position of the progress bar.
        /// </summary>
        /// <returns>The position within the range of the progress bar. The default is 0.
        /// </returns>
        /// <exception cref="System.ArgumentException">The value specified is greater than the value of the
        /// <see cref="System.Windows.Forms.ProgressBar.Maximum" /> property.-or- The value specified is less than the value of the
        /// <see cref="System.Windows.Forms.ProgressBar.Minimum" /> property.
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        [DefaultValue(0)]
        public int Value { get => (int)self.Value; set => self.Value = value; }
        /// <summary>
        /// Advances the current position of the progress bar by the specified amount.
        /// </summary>
        /// <param name="value">The amount by which to increment the progress bar's current position.
        /// </param>
        /// <exception cref="System.InvalidOperationException">The
        /// <see cref="System.Windows.Forms.ProgressBar.Style" /> property is set to
        /// <see cref="System.Windows.Forms.ProgressBarStyle.Marquee" />
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public void Increment(int value)
        {

        }
        /// <summary>
        /// Advances the current position of the progress bar by the amount of the
        /// <see cref="System.Windows.Forms.ProgressBar.Step" /> property.
        /// </summary>
        /// <exception cref="System.InvalidOperationException">
        /// <see cref="System.Windows.Forms.ProgressBar.Style" /> is set to
        /// <see cref="System.Windows.Forms.ProgressBarStyle.Marquee" />
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public void PerformStep()
        {

        }
    }
}
