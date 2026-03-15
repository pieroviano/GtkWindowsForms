using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace System.Windows.Forms;

/// <summary>
/// Implements the basic functionality of a scroll bar control.
/// </summary>
[DefaultProperty(nameof(Value))]
[DefaultEvent(nameof(Scroll))]
public abstract partial class ScrollBar : Control
{
    double _oldValue = 0;

    /// <summary>
    /// Occurs when [scroll.
    /// </summary>
    public event ScrollEventHandler Scroll;

    /// <summary>
    /// Occurs when [value changed.
    /// </summary>
    public event EventHandler ValueChanged;

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.ScrollBar" /> class.
    /// </summary>
    public ScrollBar() : base()
    {
        Adjustment.ValueChanged += Control_ValueChanged;
    }

    /// <summary>
    /// Gets the adjustment.
    /// </summary>
    /// <value>
    /// The adjustment.
    /// </value>
    public virtual Gtk.Adjustment Adjustment { get; }

    /// <summary>
    /// Gets or sets the large change.
    /// </summary>
    /// <value>
    /// The large change.
    /// </value>
    public int LargeChange { get => (int)Adjustment.PageIncrement; set => Adjustment.PageIncrement = value; }

    /// <summary>
    /// Gets or sets the maximum.
    /// </summary>
    /// <value>
    /// The maximum.
    /// </value>
    public int Maximum { get => (int)Adjustment.Upper; set => Adjustment.Upper = value; }

    /// <summary>
    /// Gets or sets the minimum.
    /// </summary>
    /// <value>
    /// The minimum.
    /// </value>
    public int Minimum { get => (int)Adjustment.Lower; set => Adjustment.Lower = value; }

    /// <summary>
    /// Gets or sets the small change.
    /// </summary>
    /// <value>
    /// The small change.
    /// </value>
    public int SmallChange { get => (int)Adjustment.StepIncrement; set => Adjustment.StepIncrement = value; }

    /// <summary>
    /// Gets or sets the value.
    /// </summary>
    /// <value>
    /// The value.
    /// </value>
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