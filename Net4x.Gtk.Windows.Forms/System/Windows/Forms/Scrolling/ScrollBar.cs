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
        public ScrollBar() : base()
        {
            Adjustment.ValueChanged += Control_ValueChanged;
        }

        public virtual Gtk.Adjustment Adjustment { get; }

        public int LargeChange { get => (int)Adjustment.PageIncrement; set => Adjustment.PageIncrement = value; }

        public int Maximum { get => (int)Adjustment.Upper; set => Adjustment.Upper = value; }

        public int Minimum { get => (int)Adjustment.Lower; set => Adjustment.Lower = value; }

        public int SmallChange { get => (int)Adjustment.StepIncrement; set => Adjustment.StepIncrement = value; }

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
