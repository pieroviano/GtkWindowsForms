
using System.Drawing;

namespace System.Windows.Forms;

/// <summary>
/// Provides focus-management functionality for controls that can function as a container for other controls.
/// </summary>
/// <filterpriority>
/// 2
/// </filterpriority>
public class ContainerControl : ScrollableControl, IContainerControl
{

    private Control? _ActiveControl;

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.ContainerControl" /> class.
    /// </summary>
    public ContainerControl() : base()
    {
    }

    /// <summary>
    /// Gets or sets the dimensions the control was designed against.
    /// </summary>
    /// <remarks>
    /// This and <see cref="P:System.Windows.Forms.ContainerControl.AutoScaleMode" /> used to be inert
    /// auto-properties on <see cref="T:System.Windows.Forms.Form" />. Every designer-generated form sets
    /// them, so a layout produced at one font metric or DPI was never rescaled at another - exactly the
    /// case a cross-platform layer hits hardest, because Linux and macOS font metrics differ from those
    /// of Windows.
    /// </remarks>
    public SizeF AutoScaleDimensions
    {
        get => _autoScaleDimensions;
        set => _autoScaleDimensions = value;
    }

    /// <summary>
    /// Gets or sets how the control scales itself relative to the dimensions it was designed against.
    /// </summary>
    public AutoScaleMode AutoScaleMode
    {
        get => _autoScaleMode;
        set
        {
            if (_autoScaleMode == value)
            {
                return;
            }

            _autoScaleMode = value;
            _autoScaleApplied = false;
        }
    }

    /// <summary>
    /// Gets the dimensions this control is currently running against, in the units implied by
    /// <see cref="P:System.Windows.Forms.ContainerControl.AutoScaleMode" />.
    /// </summary>
    public SizeF CurrentAutoScaleDimensions => GetCurrentAutoScaleDimensions(AutoScaleMode);

    /// <summary>
    /// Gets the ratio between the current dimensions and the design-time dimensions.
    /// </summary>
    public SizeF AutoScaleFactor
    {
        get
        {
            var design = AutoScaleDimensions;
            if (design.Width <= 0f || design.Height <= 0f)
            {
                return new SizeF(1f, 1f);
            }

            var current = CurrentAutoScaleDimensions;
            if (current.Width <= 0f || current.Height <= 0f)
            {
                return new SizeF(1f, 1f);
            }

            return new SizeF(current.Width / design.Width, current.Height / design.Height);
        }
    }

    /// <summary>
    /// Scales this container and its children by the auto-scale factor.
    /// </summary>
    public void PerformAutoScale() => PerformAutoScaleCore(true);

    /// <summary>
    /// Scales this container by the auto-scale factor, optionally including its own bounds.
    /// </summary>
    /// <param name="includeBounds">Whether the bounds of the container itself are scaled as well.</param>
    internal void PerformAutoScaleCore(bool includeBounds)
    {
        if (AutoScaleMode is AutoScaleMode.None or AutoScaleMode.Inherit)
        {
            return;
        }

        var factor = AutoScaleFactor;
        if (IsUnityFactor(factor))
        {
            _autoScaleApplied = true;

            return;
        }

        SuspendLayout();
        try
        {
            var children = Controls;
            if (children != null)
            {
                for (var i = 0; i < children.Count; i++)
                {
                    children[i].Scale(factor.Width, factor.Height);
                }
            }

            if (includeBounds)
            {
                var size = ClientSize;
                var scaled = new Size(
                    (int)Math.Round(size.Width * factor.Width),
                    (int)Math.Round(size.Height * factor.Height));

                // Scaling is not a resize: the children are scaled with the container, so they are
                // re-based to its new size instead of following the growth a second time.
                if (this is Form form)
                {
                    form.SetClientSizeAsScaling(scaled);
                }
                else
                {
                    ClientSize = scaled;
                    RebaseAllChildAnchorBaselines();
                }
            }
        }
        finally
        {
            base.ResumeLayout(false);
        }

        // The container is now laid out against the current metric, so that becomes the new baseline.
        AutoScaleDimensions = CurrentAutoScaleDimensions;
        _autoScaleApplied = true;
    }

    // NOTE: auto-scaling is deliberately NOT applied automatically when layout resumes.
    //
    // A designer-generated form carries AutoScaleDimensions baked in by the Windows designer, e.g.
    // (9F, 20F) for Segoe UI 9pt. Measuring the running font through Pango and dividing gives the
    // difference between two *different typefaces*, not the change in environment that WinForms scales
    // against - so applying it rescales every form by an arbitrary ratio. Doing so shrank the sample
    // application's controls to about 79% while leaving the window size untouched, clipping button
    // captions and wrapping labels.
    //
    // The members below are therefore live and usable - PerformAutoScale() really scales, and the factor
    // is really computed - but nothing scales a form behind the caller's back until there is a design-time
    // metric that is commensurate with the runtime one.

    /// <summary>
    /// The dimensions the current environment presents for the given mode.
    /// </summary>
    /// <param name="mode">The scaling mode to measure for.</param>
    private SizeF GetCurrentAutoScaleDimensions(AutoScaleMode mode)
    {
        switch (mode)
        {
            case AutoScaleMode.Dpi:
                return new SizeF(DeviceDpi, DeviceDpi);

            case AutoScaleMode.Font:
                return GetFontAutoScaleDimensions();

            default:
                return new SizeF(0f, 0f);
        }
    }

    /// <summary>
    /// Measures the average character width and the line height of the current font, which is what
    /// <see cref="F:System.Windows.Forms.AutoScaleMode.Font" /> scales against.
    /// </summary>
    private SizeF GetFontAutoScaleDimensions()
    {
        // Control.Font is null until one is assigned, and it is deliberately left that way: returning a
        // hardcoded fallback from the property overrides the GTK theme font and shifts text metrics
        // across the whole application. Fall back only here, where a font is genuinely needed.
        var font = Font ?? DefaultFont;

        try
        {
            using var surface = new Drawing.Bitmap(1, 1);
            using var graphics = Drawing.Graphics.FromImage(surface);
            const string sample = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
            var measured = graphics.MeasureString(sample, font);
            if (measured.Width <= 0f || measured.Height <= 0f)
            {
                return new SizeF(0f, 0f);
            }

            return new SizeF(measured.Width / sample.Length, measured.Height);
        }
        catch (Exception ex)
        {
            Diagnostics.GtkLogger.Debug("AutoScale could not measure the font: " + ex.Message);

            return new SizeF(0f, 0f);
        }
    }

    /// <summary>Indicates whether the factor is close enough to 1 that scaling would be a no-op.</summary>
    private static bool IsUnityFactor(SizeF factor) =>
        Math.Abs(factor.Width - 1f) < 0.01f && Math.Abs(factor.Height - 1f) < 0.01f;

    private SizeF _autoScaleDimensions;

    private AutoScaleMode _autoScaleMode = AutoScaleMode.None;

    private bool _autoScaleApplied;

    /// <summary>
    /// Gets or sets the control that is active on the container control.
    /// </summary>
    public Control? ActiveControl
    {
        get
        {
            var controlCollection = this.Controls;
            if (controlCollection != null)
                foreach (var control in controlCollection)
                {
                    if (control is Control con)
                    {
                        if (con.Widget != null && ((con.Widget.StateFlags & Gtk.StateFlags.Active) != 0 || con.Widget.IsFocus))
                            return con;
                    }
                }

            return _ActiveControl;
        }
        set { ActivateControl(value); }
    }

    /// <summary>
    /// Gets or sets the status strip.
    /// </summary>
    /// <value>
    /// The status strip.
    /// </value>
    public virtual StatusStrip? StatusStrip
    {
        get;
        set;
    }

    /// <summary>
    /// Activates a specified control.
    /// </summary>
    /// <param name="active">
    /// The
    /// <see cref="T:System.Windows.Forms.Control" /> being activated.
    /// </param>
    /// <returns>
    /// true if the control is successfully activated; otherwise, false.
    /// </returns>
    public bool ActivateControl(Control? active)
    {
        _ActiveControl = active;
        if (active != null)
        {
            if (active.GtkControl is Gtk.Widget widget)
            {
                return widget.Activate();
            }
        }

        return false;
    }
}