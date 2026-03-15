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
/// Represents a Windows progress bar control.
/// </summary>
[DesignerCategory("Component")]
[DefaultProperty("Value")]
public class ProgressBar : Control
{

    private readonly ProgressBarBase self = new ProgressBarBase();

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.ProgressBar" /> class.
    /// </summary>
    public ProgressBar() : base()
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
    /// Gets or sets the marquee animation speed.
    /// </summary>
    /// <value>
    /// The marquee animation speed.
    /// </value>
    [DefaultValue(100)]
    public int MarqueeAnimationSpeed { get; set; } = 100;

    /// <summary>
    /// Gets or sets the maximum.
    /// </summary>
    /// <value>
    /// The maximum.
    /// </value>
    [DefaultValue(100)]
    public int Maximum { get; set; } = 100;

    /// <summary>
    /// Gets or sets the minimum.
    /// </summary>
    /// <value>
    /// The minimum.
    /// </value>
    [DefaultValue(0)]
    public int Minimum { get; set; } = 0;

    /// <summary>
    /// Gets or sets the padding.
    /// </summary>
    /// <value>
    /// The padding.
    /// </value>
    public new Padding Padding { get; set; }

    /// <summary>
    /// Gets or sets the step.
    /// </summary>
    /// <value>
    /// The step.
    /// </value>
    [DefaultValue(10)]
    public int Step { get; set; }

    /// <summary>
    /// Gets or sets the style.
    /// </summary>
    /// <value>
    /// The style.
    /// </value>
    public ProgressBarStyle Style { get; set; }

    /// <summary>
    /// Gets or sets the value.
    /// </summary>
    /// <value>
    /// The value.
    /// </value>
    [DefaultValue(0)]
    public int Value { get => (int)self.Value; set => self.Value = value; }

    /// <summary>
    /// Increments the specified value.
    /// </summary>
    /// <param name="value">
    /// The value.
    /// </param>
    public void Increment(int value)
    {

    }

    /// <summary>
    /// Performs the step.
    /// </summary>
    public void PerformStep()
    {

    }

    private void Control_Realized(object sender, EventArgs e)
    {
        self.MaxValue = Maximum;
        self.MinValue = Minimum;
        self.Value = Value;
    }

}