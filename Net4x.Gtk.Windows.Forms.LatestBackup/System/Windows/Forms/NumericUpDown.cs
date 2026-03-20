/*
 * Based on GTK components, it is a cross-platform interface component that is compatible with the native C# control winform interface.
 * Use this component GtkSystem.Windows.Forms instead of Microsoft.WindowsDesktop.App.WindowsForms, compile once, and run across Windows, Linux, and macOS
 * Technical support 438865652@qq.com，https://www.gtkapp.com, https://gitee.com/easywebfactory, https://github.com/easywebfactory438865652@qq.com，https://www.gtkapp.com, https://gitee.com/easywebfactory, https://github.com/easywebfactory
 * author:chenhongjin
 */
using System.ComponentModel;
using GtkSystem.Windows.Forms.GtkControls.ControlBase;

namespace System.Windows.Forms;

/// <summary>
/// Represents a Windows spin box (also known as an up-down control) that displays numeric values.
/// </summary>
[DesignerCategory("Component")]
public partial class NumericUpDown : Control
{
    /// <summary>
    /// The self
    /// </summary>
    public readonly NumericUpDownBase self = new NumericUpDownBase();

    /// <summary>
    /// Occurs when value changed.
    /// </summary>
    public virtual event EventHandler ValueChanged;

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.NumericUpDown" /> class.
    /// </summary>
    public NumericUpDown() : base()
    {
        self.ValueChanged += Self_ValueChanged;
    }

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
    /// Raises the ValueChanged event.
    /// </summary>
    protected virtual void OnValueChanged(System.EventArgs e)
    {
        ValueChanged?.Invoke(this, e);
    }

    private void Self_ValueChanged(object sender, EventArgs e)
    {
        if (ValueChanged != null && self.IsVisible)
            ValueChanged(this, e);
    }

}