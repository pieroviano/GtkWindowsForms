using Gtk;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;

namespace System.Windows.Forms
{
    /// <summary>
    /// Represents a Windows progress bar control contained in a
    /// <see cref="System.Windows.Forms.StatusStrip" />
    /// </summary>
    /// <filterpriority>
    /// 2
    /// </filterpriority>
    [DefaultProperty("Value")]
    public class ToolStripProgressBar : WidgetToolStrip<Gtk.MenuItem>
    {
        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="System.Windows.Forms.ToolStripProgressBar" /> class.
        /// </summary>
        public ToolStripProgressBar() : base("ToolStripProgressBar", null)
        {
            base.MenuItem.Realized += Control_Realized;
        }

        private void Control_Realized(object sender, EventArgs e)
        {
            progressBar.MaxValue = Maximum;
            progressBar.MinValue = Minimum;
        }
        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="System.Windows.Forms.ToolStripProgressBar" /> class with specified name.
        /// </summary>
        /// <param name="name">The name of the
        /// <see cref="System.Windows.Forms.ToolStripProgressBar" />
        /// </param>
        public ToolStripProgressBar(string name) : this()
        {

        }
        /// <summary>
        /// Gets the
        /// <see cref="System.Windows.Forms.ProgressBar" />
        /// </summary>
        /// <returns>A
        /// <see cref="System.Windows.Forms.ProgressBar" />
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public ProgressBar ProgressBar
        {
            get
            {
                return new ProgressBar() { Value = this.Value, Maximum = this.Maximum, Minimum = this.Minimum, Step = this.Step };
            }
        }
        /// <summary>
        /// Gets or sets a value representing the delay between each
        /// <see cref="System.Windows.Forms.ProgressBarStyle.Marquee" /> display update, in milliseconds.
        /// </summary>
        /// <returns>An integer representing the delay, in milliseconds.
        /// </returns>
        public int MarqueeAnimationSpeed { get; set; } = 100;
        /// <summary>
        /// Gets or sets the upper bound of the range that is defined for this
        /// <see cref="System.Windows.Forms.ToolStripProgressBar" />
        /// </summary>
        /// <returns>An integer representing the upper bound of the range. The default is 100.
        /// </returns>
        public int Maximum { get; set; } = 100;
        /// <summary>
        /// Gets or sets the lower bound of the range that is defined for this
        /// <see cref="System.Windows.Forms.ToolStripProgressBar" />
        /// </summary>
        /// <returns>An integer representing the lower bound of the range. The default is 0.
        /// </returns>
        public int Minimum { get; set; } = 0;


        private int _step;
        /// <summary>
        /// Gets or sets the amount by which to increment the current value of the
        /// <see cref="System.Windows.Forms.ToolStripProgressBar" /> when the
        /// <see cref="System.Windows.Forms.ToolStripProgressBar.PerformStep" /> method is called.
        /// </summary>
        /// <returns>An integer representing the incremental amount. The default value is 10.
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
        /// Gets or sets the style of the
        /// <see cref="System.Windows.Forms.ToolStripProgressBar" />
        /// </summary>
        /// <returns>One of the
        /// <see cref="System.Windows.Forms.ProgressBarStyle" /> values. The default value is
        /// <see cref="System.Windows.Forms.ProgressBarStyle.Blocks" />
        /// </returns>
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
        /// Gets or sets the current value of the
        /// <see cref="System.Windows.Forms.ToolStripProgressBar" />
        /// </summary>
        /// <returns>An integer representing the current value.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public int Value
        {
            get { return (int)Math.Round(progressBar.Value, 0); }
            set
            {
                progressBar.Value = value;
            }
        }

        public override Size Size
        {
            get
            {
                return base.Size;
            }
            set
            {
                base.Size = value;
                progressBar.SetSizeRequest(value.Width, value.Height);
            }
        }
        /// <summary>
        /// Advances the current position of the progress bar by the specified amount.
        /// </summary>
        /// <param name="value">The amount by which to increment the progress bar's current position.
        /// </param>
        public void Increment(int value)
        {

        }
        /// <summary>
        /// Advances the current position of the progress bar by the amount of the
        /// <see cref="System.Windows.Forms.ToolStripProgressBar.Step" /> property.
        /// </summary>
        public void PerformStep()
        {

        }

    }
}
