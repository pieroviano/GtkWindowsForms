using System.ComponentModel;
using System.Drawing;
using Gtk;

namespace System.Windows.Forms;

/// <summary>
/// Represents a Windows progress bar control contained in a
/// <see cref="T:System.Windows.Forms.StatusStrip" />
/// </summary>
[DefaultProperty("Value")]
public class ToolStripProgressBar : ToolStripItem
{

    public StripToolItem self = new StripToolItem();

    internal Gtk.LevelBar progressBar = new Gtk.LevelBar();

    private int _step;

    private ProgressBarStyle style;

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.ToolStripProgressBar" /> class.
    /// </summary>
    public ToolStripProgressBar() : base()
    {
        progressBar.MaxValue = 100;
        progressBar.MinValue = 0;
        progressBar.Orientation = Gtk.Orientation.Horizontal;
        progressBar.Valign = Align.Center;
        self.AddChild(progressBar);
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
        get => (int)progressBar.MaxValue;
        set => progressBar.MaxValue = value;
    }

    /// <summary>
    /// Gets or sets the minimum.
    /// </summary>
    /// <value>
    /// The minimum.
    /// </value>
    public int Minimum
    {
        get => (int)progressBar.MinValue;
        set => progressBar.MinValue = value;
    }

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
            progressBar.SetSizeRequest(value.Width, value.Height);
            base.Size = value;
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
        get => style;
        set
        {
            style = value;
            if (style == ProgressBarStyle.Continuous)
                progressBar.Mode = LevelBarMode.Continuous;
            else
                progressBar.Mode = LevelBarMode.Discrete;
        }
    }

    /// <summary>
    /// Gets or sets the value.
    /// </summary>
    /// <value>
    /// The value.
    /// </value>
    public int Value
    {
        get { return (int)progressBar.Value; }
        set
        {
            progressBar.MaxValue = Maximum;
            progressBar.MinValue = Minimum;
            progressBar.Value = value;
        }
    }

    public override IToolMenuItem Widget { get => self; }

    /// <summary>
    /// Increments the specified value.
    /// </summary>
    /// <param name="value">
    /// The value.
    /// </param>
    public void Increment(int value)
    {
        progressBar.Value = Math.Min(Maximum, Math.Max(Minimum, progressBar.Value + value));
    }

    /// <summary>
    /// Performs the step.
    /// </summary>
    public void PerformStep()
    {

    }
}
