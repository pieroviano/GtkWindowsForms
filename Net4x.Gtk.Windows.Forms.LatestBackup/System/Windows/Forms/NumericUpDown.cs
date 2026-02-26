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
    /// Represents a Windows spin box (also known as an up-down control) that displays numeric values.
    /// </summary>
    /// <filterpriority>
    /// 2
    /// </filterpriority>
    [DesignerCategory("Component")]
    public partial class NumericUpDown : Control
    {

        public readonly NumericUpDownBase self = new NumericUpDownBase();

        public override object GtkControl => self;
        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="System.Windows.Forms.NumericUpDown" /> class.
        /// </summary>
        public NumericUpDown() : base()
        {
            self.ValueChanged += Self_ValueChanged;
        }

        private void Self_ValueChanged(object sender, EventArgs e)
        {
            if (ValueChanged != null && self.IsVisible)
                ValueChanged(this, e);
        }


        public event EventHandler ValueChanged;
        /// <summary>
        /// Gets or sets the number of decimal places to display in the spin box (also known as an up-down control).
        /// </summary>
        /// <returns>The number of decimal places to display in the spin box. The default is 0.
        /// </returns>
        /// <exception cref="System.ArgumentOutOfRangeException">The value assigned is less than 0.-or- The value assigned is greater than 99.
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public int DecimalPlaces { get => Convert.ToInt32(self.Digits); set => self.Digits = Convert.ToUInt32(value); }
        /// <summary>
        /// Gets or sets the value to increment or decrement the spin box (also known as an up-down control) when the up or down buttons are clicked.
        /// </summary>
        /// <returns>The value to increment or decrement the
        /// <see cref="System.Windows.Forms.NumericUpDown.Value" /> property when the up or down buttons are clicked on the spin box. The default value is 1.
        /// </returns>
        /// <exception cref="System.ArgumentOutOfRangeException">The assigned value is not greater than or equal to zero.
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        public decimal Increment
        {
            get => Convert.ToDecimal(self.Adjustment.StepIncrement);
            set => self.Adjustment.StepIncrement = Convert.ToDouble(value);
        }
        /// <summary>
        /// Gets or sets the maximum value for the spin box (also known as an up-down control).
        /// </summary>
        /// <returns>The maximum value for the spin box. The default value is 100.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public decimal Maximum { get => Convert.ToDecimal(self.Adjustment.Upper); set => self.Adjustment.Upper = Convert.ToDouble(value); }
        /// <summary>
        /// Gets or sets the minimum allowed value for the spin box (also known as an up-down control).
        /// </summary>
        /// <returns>The minimum allowed value for the spin box. The default value is 0.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public decimal Minimum { get => Convert.ToDecimal(self.Adjustment.Lower); set => self.Adjustment.Lower = Convert.ToDouble(value); }
        /// <summary>
        /// Gets or sets the value assigned to the spin box (also known as an up-down control).
        /// </summary>
        /// <returns>The numeric value of the
        /// <see cref="System.Windows.Forms.NumericUpDown" /> control.
        /// </returns>
        /// <exception cref="System.ArgumentOutOfRangeException">The assigned value is less than the
        /// <see cref="System.Windows.Forms.NumericUpDown.Minimum" /> property value.-or- The assigned value is greater than the
        /// <see cref="System.Windows.Forms.NumericUpDown.Maximum" /> property value.
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public decimal Value { get { return Convert.ToDecimal(self.Value); } set { self.Value = Convert.ToDouble(value); } }
    }
}
