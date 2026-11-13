/*
 * Based on GTK components, it is a cross-platform interface component that is compatible with the native C# control winform interface.
 * Use this component Gtk.Windows.Forms instead of Microsoft.WindowsDesktop.App.WindowsForms, compile once, and run across Windows, Linux, and macOS
 * Technical support 438865652@qq.com，https://www.gtkapp.com, https://gitee.com/easywebfactory, https://github.com/easywebfactory438865652@qq.com，https://www.gtkapp.com, https://gitee.com/easywebfactory, https://github.com/easywebfactory
 * author:chenhongjin
 */
using System.ComponentModel;
using Gtk.Windows.Forms.Controls.ControlBase;

namespace System.Windows.Forms;

/// <summary>
/// Represents a Windows spin box (also known as an up-down control) that displays numeric values.
/// </summary>
[DesignerCategory("Component")]
public partial class NumericUpDown : UpDownBase
{
    /// <summary>
    /// The self
    /// </summary>
    public readonly NumericUpDownBase self = new();

    /// <summary>
    /// Occurs when value changed.
    /// </summary>
    public virtual event EventHandler? ValueChanged;

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.NumericUpDown" /> class.
    /// </summary>
    public NumericUpDown() : base()
    {
        self.Override.sender = this;
        self.ValueChanged += Self_ValueChanged;
    }

    /// <summary>
    /// Gets a collection of sorted acceleration objects that determine how the value changes when the up or
    /// down button is held down.
    /// </summary>
    public NumericUpDownAccelerationCollection Accelerations { get; } = new NumericUpDownAccelerationCollection();

    /// <summary>
    /// Gets or sets the decimal places.
    /// </summary>
    /// <value>
    /// The decimal places.
    /// </value>
    public int DecimalPlaces
    {
        get => Convert.ToInt32(self.Digits);
        set => self.Digits = Convert.ToUInt32(value);
    }

    /// <summary>
    /// Gets the GTK control.
    /// </summary>
    /// <value>
    /// The GTK control.
    /// </value>
    public override object GtkControl => self;

    /// <summary>
    /// Gets or sets the increment.
    /// </summary>
    /// <value>
    /// The increment.
    /// </value>
    public decimal Increment
    {
        get => Convert.ToDecimal(self.Adjustment.StepIncrement);
        set => self.Adjustment.StepIncrement = Convert.ToDouble(value);
    }

    /// <summary>
    /// Gets or sets the maximum.
    /// </summary>
    /// <value>
    /// The maximum.
    /// </value>
    public decimal Maximum
    {
        get => Convert.ToDecimal(self.Adjustment.Upper);
        set => self.Adjustment.Upper = Convert.ToDouble(value);
    }

    /// <summary>
    /// Gets or sets the minimum.
    /// </summary>
    /// <value>
    /// The minimum.
    /// </value>
    public decimal Minimum
    {
        get => Convert.ToDecimal(self.Adjustment.Lower);
        set => self.Adjustment.Lower = Convert.ToDouble(value);
    }

    /// <summary>
    /// Gets or sets the value.
    /// </summary>
    /// <value>
    /// The value.
    /// </value>
    public decimal Value
    {
        get { return Convert.ToDecimal(self.Value); }
        set { self.Value = Convert.ToDouble(value); }
    }

    /// <summary>
    /// Gets or sets a value indicating whether the numeric value can be edited only with the up and down
    /// buttons.
    /// </summary>
    public override bool ReadOnly
    {
        get => self.IsEditable == false;
        set => self.IsEditable = value == false;
    }

    /// <summary>
    /// Gets or sets the text displayed in the spin box.
    /// </summary>
    public override string Text
    {
        get => self.Text;
        set => self.Text = value ?? string.Empty;
    }

    /// <summary>
    /// Increments the value of the spin box by the amount specified by the
    /// <see cref="P:System.Windows.Forms.NumericUpDown.Increment" /> property.
    /// </summary>
    public override void UpButton()
    {
        Value = Math.Min(Maximum, Value + Increment);
    }

    /// <summary>
    /// Decrements the value of the spin box by the amount specified by the
    /// <see cref="P:System.Windows.Forms.NumericUpDown.Increment" /> property.
    /// </summary>
    public override void DownButton()
    {
        Value = Math.Max(Minimum, Value - Increment);
    }

    /// <summary>
    /// Raises the ValueChanged event.
    /// </summary>
    protected virtual void OnValueChanged(System.EventArgs e)
    {
        ValueChanged?.Invoke(this, e);
    }

    private void Self_ValueChanged(object sender, EventArgs e)
    {
        if (ValueChanged != null && self.IsVisible)
            OnValueChanged(e);
    }
}