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
/// Represents a standard Windows track bar.
/// </summary>
[DesignerCategory("Component")]
public class TrackBar : Control
{
    /// <summary>
    /// The self
    /// </summary>
    public readonly TrackBarBase self = new TrackBarBase();

    readonly Gtk.Adjustment adjustment = new Gtk.Adjustment(10, 0, 100, 1, 1, 0);

    Gtk.Scale scale;

    /// <summary>
    /// Occurs when scroll.
    /// </summary>
    public virtual event EventHandler Scroll;

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.TrackBar" /> class.
    /// </summary>
    public TrackBar() : base()
    {
        self.Realized += Control_Realized;
    }

    /// <summary>
    /// Gets the GTK control.
    /// </summary>
    /// <value>
    /// The GTK control.
    /// </value>
    public override object GtkControl => self;

    /// <summary>
    /// Gets or sets the large change.
    /// </summary>
    /// <value>
    /// The large change.
    /// </value>
    public int LargeChange
    {
        get;
        set;
    } = 5;

    /// <summary>
    /// Gets or sets the maximum.
    /// </summary>
    /// <value>
    /// The maximum.
    /// </value>
    public int Maximum
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the minimum.
    /// </summary>
    /// <value>
    /// The minimum.
    /// </value>
    public int Minimum
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the orientation.
    /// </summary>
    /// <value>
    /// The orientation.
    /// </value>
    public System.Windows.Forms.Orientation Orientation
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the tick frequency.
    /// </summary>
    /// <value>
    /// The tick frequency.
    /// </value>
    public int TickFrequency
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the tick style.
    /// </summary>
    /// <value>
    /// The tick style.
    /// </value>
    public System.Windows.Forms.TickStyle TickStyle
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the value.
    /// </summary>
    /// <value>
    /// The value.
    /// </value>
    public int Value
    {
        get => (int)adjustment.Value;
        set { adjustment.Value = value; }
    }

    /// <summary>
    /// Raises the Scroll event.
    /// </summary>
    protected virtual void OnScroll(System.EventArgs e)
    {
        Scroll?.Invoke(this, e);
    }

    private void Control_Realized(object sender, EventArgs e)
    {
        scale = new Gtk.Scale(Orientation == Orientation.Horizontal ? Gtk.Orientation.Horizontal : Gtk.Orientation.Vertical, adjustment)
        {
            ShowFillLevel = true,
            DrawValue = false,
            RoundDigits = 1,
            Visible = true,
            Inverted = Orientation == Orientation.Vertical
        };
        adjustment.Lower = Minimum;
        adjustment.Upper = Maximum;
        adjustment.Value = Value;
        adjustment.ValueChanged += Control_ValueChanged;
        self.Child = scale;
    }

    private void Control_ValueChanged(object sender, EventArgs e)
    {
        Value = (int)adjustment.Value;
        Scroll?.Invoke(this, e);
    }
}