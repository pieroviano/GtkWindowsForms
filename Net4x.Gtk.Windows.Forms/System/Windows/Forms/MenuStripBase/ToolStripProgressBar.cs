using System.ComponentModel;
using System.Drawing;
using Gtk;

namespace System.Windows.Forms;

/// <summary>
/// Represents a Windows progress bar control contained in a
/// <see cref="T:System.Windows.Forms.StatusStrip" />
/// </summary>
[DefaultProperty("Value")]
public class ToolStripProgressBar : WidgetToolStrip<Gtk.MenuItem>
{

    private readonly int _step;

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.ToolStripProgressBar" /> class.
    /// </summary>
    public ToolStripProgressBar() : base("ToolStripProgressBar", null)
    {
        base.MenuItem.Realized += Control_Realized;
    }

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.ToolStripProgressBar" /> class.
    /// </summary>
    /// <param name="name">
    /// The name.
    /// </param>
    public ToolStripProgressBar(string name) : this()
    {

    }

    /// <summary>
    /// Gets or sets the marquee animation speed.
    /// </summary>
    /// <value>
    /// The marquee animation speed.
    /// </value>
    public int MarqueeAnimationSpeed
    {
        get;
        set;
    } = 100;

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
    } = 100;

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
    } = 0;

    /// <summary>
    /// Gets the progress bar.
    /// </summary>
    /// <value>
    /// The progress bar.
    /// </value>
    public ProgressBar ProgressBar
    {
        get
        {
            return new ProgressBar() { Value = this.Value, Maximum = this.Maximum, Minimum = this.Minimum, Step = this.Step };
        }
    }

    /// <summary>
    /// Gets or sets the size.
    /// </summary>
    /// <value>
    /// The size.
    /// </value>
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
    /// Gets or sets the step.
    /// </summary>
    /// <value>
    /// The step.
    /// </value>
    [DefaultValue(10)]
    public int Step
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the style.
    /// </summary>
    /// <value>
    /// The style.
    /// </value>
    public ProgressBarStyle Style
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
        get { return (int)Math.Round(progressBar.Value, 0); }
        set
        {
            progressBar.Value = value;
        }
    }

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
        progressBar.MaxValue = Maximum;
        progressBar.MinValue = Minimum;
    }
}