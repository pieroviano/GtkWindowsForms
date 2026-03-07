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
    /// Represents a standard Windows track bar.
    /// </summary>
    /// <filterpriority>
    /// 2
    /// </filterpriority>
    [DesignerCategory("Component")]
    public class TrackBar : Control
    {

        public readonly TrackBarBase self = new TrackBarBase();
        Gtk.Adjustment adjustment = new Gtk.Adjustment(10, 0, 100, 1, 1, 0);

        Gtk.Scale scale;
        public event EventHandler Scroll;
        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="System.Windows.Forms.TrackBar" /> class.
        /// </summary>
        public TrackBar() : base()
        {
            self.Realized += Control_Realized;
        }
        public override object GtkControl => self;

        /// <summary>
        /// Gets or sets a value to be added to or subtracted from the
        /// <see cref="System.Windows.Forms.TrackBar.Value" /> property when the scroll box is moved a large distance.
        /// </summary>
        /// <returns>A numeric value. The default is 5.
        /// </returns>
        /// <exception cref="System.ArgumentException">The assigned value is less than 0.
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public int LargeChange { get; set; } = 5;

        /// <summary>
        /// Gets or sets the upper limit of the range this
        /// <see cref="System.Windows.Forms.TrackBar" /> is working with.
        /// </summary>
        /// <returns>The maximum value for the
        /// <see cref="System.Windows.Forms.TrackBar" /> The default is 10.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public int Maximum { get; set; }

        /// <summary>
        /// Gets or sets the lower limit of the range this
        /// <see cref="System.Windows.Forms.TrackBar" /> is working with.
        /// </summary>
        /// <returns>The minimum value for the
        /// <see cref="System.Windows.Forms.TrackBar" /> The default is 0.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public int Minimum { get; set; }

        /// <summary>
        /// Gets or sets a value indicating the horizontal or vertical orientation of the track bar.
        /// </summary>
        /// <returns>One of the
        /// <see cref="System.Windows.Forms.Orientation" /> values.
        /// </returns>
        /// <exception cref="System.ComponentModel.InvalidEnumArgumentException">The assigned value is not one of the
        /// <see cref="System.Windows.Forms.Orientation" /> values.
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public System.Windows.Forms.Orientation Orientation { get; set; }

        /// <summary>
        /// Gets or sets a value that specifies the delta between ticks drawn on the control.
        /// </summary>
        /// <returns>The numeric value representing the delta between ticks. The default is 1.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public int TickFrequency { get; set; }

        /// <summary>
        /// Gets or sets a value indicating how to display the tick marks on the track bar.
        /// </summary>
        /// <returns>One of the
        /// <see cref="System.Windows.Forms.TickStyle" /> values. The default is
        /// <see cref="System.Windows.Forms.TickStyle.BottomRight" />
        /// </returns>
        /// <exception cref="System.ComponentModel.InvalidEnumArgumentException">The assigned value is not a valid
        /// <see cref="System.Windows.Forms.TickStyle" />
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public System.Windows.Forms.TickStyle TickStyle { get; set; }

        /// <summary>
        /// Gets or sets a numeric value that represents the current position of the scroll box on the track bar.
        /// </summary>
        /// <returns>A numeric value that is within the
        /// <see cref="System.Windows.Forms.TrackBar.Minimum" /> and
        /// <see cref="System.Windows.Forms.TrackBar.Maximum" /> range. The default value is 0.
        /// </returns>
        /// <exception cref="System.ArgumentException">The assigned value is less than the value of
        /// <see cref="System.Windows.Forms.TrackBar.Minimum" />-or- The assigned value is greater than the value of
        /// <see cref="System.Windows.Forms.TrackBar.Maximum" />
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public int Value { get => (int)adjustment.Value; set { adjustment.Value = value; } }

        private void Control_Realized(object sender, EventArgs e)
        {
            scale = new Gtk.Scale(Orientation == Orientation.Horizontal ? Gtk.Orientation.Horizontal : Gtk.Orientation.Vertical, adjustment);
            scale.ShowFillLevel = true;
            scale.DrawValue = false;
            scale.RoundDigits = 1;
            scale.Visible = true;
            scale.Inverted = Orientation == Orientation.Vertical;
            adjustment.Lower = Minimum;
            adjustment.Upper = Maximum;
            adjustment.Value = Value;
            adjustment.ValueChanged += Control_ValueChanged;
            self.Child = scale;
        }

        private void Control_ValueChanged(object sender, EventArgs e)
        {
            Value = (int)adjustment.Value;
            if (Scroll != null)
                Scroll(this, e);
        }
    }
}
