using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace System.Windows.Forms
{
    /// <summary>
    /// Implements the basic functionality of a scroll bar control.
    /// </summary>
    /// <filterpriority>
    /// 1
    /// </filterpriority>
    [DefaultProperty(nameof(Value))]
    [DefaultEvent(nameof(Scroll))]
    public abstract partial class ScrollBar : Control
    {
        double _oldValue = 0;

        public event ScrollEventHandler Scroll;

        public event EventHandler ValueChanged;

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="System.Windows.Forms.ScrollBar" /> class.
        /// </summary>
        public ScrollBar() : base()
        {
            Adjustment.ValueChanged += Control_ValueChanged;
        }

        public virtual Gtk.Adjustment Adjustment { get; }

        /// <summary>
        /// Gets or sets a value to be added to or subtracted from the
        /// <see cref="System.Windows.Forms.ScrollBar.Value" /> property when the scroll box is moved a large distance.
        /// </summary>
        /// <returns>A numeric value. The default value is 10.
        /// </returns>
        /// <exception cref="System.ArgumentOutOfRangeException">The assigned value is less than 0.
        /// </exception>
        /// <filterpriority>2
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public int LargeChange { get => (int)Adjustment.PageIncrement; set => Adjustment.PageIncrement = value; }

        /// <summary>
        /// Gets or sets the upper limit of values of the scrollable range.
        /// </summary>
        /// <returns>A numeric value. The default value is 100.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public int Maximum { get => (int)Adjustment.Upper; set => Adjustment.Upper = value; }

        /// <summary>
        /// Gets or sets the lower limit of values of the scrollable range.
        /// </summary>
        /// <returns>A numeric value. The default value is 0.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public int Minimum { get => (int)Adjustment.Lower; set => Adjustment.Lower = value; }

        /// <summary>
        /// Gets or sets the value to be added to or subtracted from the
        /// <see cref="System.Windows.Forms.ScrollBar.Value" /> property when the scroll box is moved a small distance.
        /// </summary>
        /// <returns>A numeric value. The default value is 1.
        /// </returns>
        /// <exception cref="System.ArgumentOutOfRangeException">The assigned value is less than 0.
        /// </exception>
        /// <filterpriority>2
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public int SmallChange { get => (int)Adjustment.StepIncrement; set => Adjustment.StepIncrement = value; }

        /// <summary>
        /// Gets or sets a numeric value that represents the current position of the scroll box on the scroll bar control.
        /// </summary>
        /// <returns>A numeric value that is within the
        /// <see cref="System.Windows.Forms.ScrollBar.Minimum" /> and
        /// <see cref="System.Windows.Forms.ScrollBar.Maximum" /> range. The default value is 0.
        /// </returns>
        /// <exception cref="System.ArgumentOutOfRangeException">The assigned value is less than the
        /// <see cref="System.Windows.Forms.ScrollBar.Minimum" /> property value.-or- The assigned value is greater than the
        /// <see cref="System.Windows.Forms.ScrollBar.Maximum" /> property value.
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public int Value { get => (int)Adjustment.Value; set { Adjustment.Value = value; } }

        private void Control_ValueChanged(object sender, EventArgs e)
        {
            double newValue = Adjustment.Value;
            ValueChanged?.Invoke(this, e);
            var args = new ScrollEventArgs(newValue >= _oldValue ? ScrollEventType.SmallIncrement : ScrollEventType.SmallDecrement, (int)_oldValue, (int)newValue);
            Scroll?.Invoke(this, args);
            _oldValue = newValue;
        }

    }
}
