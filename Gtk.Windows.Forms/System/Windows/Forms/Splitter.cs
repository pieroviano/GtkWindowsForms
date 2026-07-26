using System.ComponentModel;
using System.Drawing;

namespace System.Windows.Forms;

/// <summary>
/// Represents a splitter control that lets the user resize docked controls.
/// </summary>
/// <remarks>
/// The legacy splitter, distinct from the <see cref="T:System.Windows.Forms.SplitContainer" /> that
/// replaced it, was missing. An application old enough to be worth porting will very likely dock one of
/// these against a panel, so its absence was a compile break.
/// <para>
/// It is a docked <see cref="T:System.Windows.Forms.Control" /> carrying the legacy surface — the
/// <see cref="P:System.Windows.Forms.Splitter.MinSize" />/<see cref="P:System.Windows.Forms.Splitter.MinExtra" />
/// limits, <see cref="P:System.Windows.Forms.Splitter.SplitPosition" /> and the
/// <see cref="E:System.Windows.Forms.Splitter.SplitterMoving" />/<see cref="E:System.Windows.Forms.Splitter.SplitterMoved" />
/// events — and resizes the control it is docked against when the position changes. Interactive dragging is
/// not wired to GTK input; setting <see cref="P:System.Windows.Forms.Splitter.SplitPosition" /> from code
/// performs the resize.
/// </para>
/// </remarks>
public class Splitter : Control
{
    /// <summary>The width or height a splitter occupies by default.</summary>
    private const int DefaultThickness = 3;

    /// <summary>
    /// Occurs while the splitter is being moved.
    /// </summary>
    public event SplitterEventHandler? SplitterMoving;

    /// <summary>
    /// Occurs after the splitter has been moved.
    /// </summary>
    public event SplitterEventHandler? SplitterMoved;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:System.Windows.Forms.Splitter" /> class.
    /// </summary>
    public Splitter()
    {
        Dock = DockStyle.Left;
        Width = DefaultThickness;
    }

    /// <summary>
    /// Gets or sets the minimum size, in pixels, of the control being resized.
    /// </summary>
    [DefaultValue(25)]
    public int MinSize
    {
        get;
        set;
    } = 25;

    /// <summary>
    /// Gets or sets the minimum size, in pixels, left for the remaining area.
    /// </summary>
    [DefaultValue(25)]
    public int MinExtra
    {
        get;
        set;
    } = 25;

    /// <summary>
    /// Gets or sets the position of the splitter, in pixels.
    /// </summary>
    /// <remarks>
    /// Assigning this resizes the control the splitter is docked against, clamped by
    /// <see cref="P:System.Windows.Forms.Splitter.MinSize" /> and
    /// <see cref="P:System.Windows.Forms.Splitter.MinExtra" />.
    /// </remarks>
    [DefaultValue(-1)]
    public int SplitPosition
    {
        get
        {
            var target = FindTarget();
            if (target == null)
            {
                return -1;
            }

            return IsVertical ? target.Width : target.Height;
        }
        set
        {
            var target = FindTarget();
            if (target == null)
            {
                return;
            }

            var limit = Parent == null
                ? int.MaxValue
                : (IsVertical ? Parent.Width : Parent.Height) - MinExtra - Width;
            var position = Math.Max(MinSize, Math.Min(value, Math.Max(MinSize, limit)));

            var args = new SplitterEventArgs(Left, Top, position, position);
            OnSplitterMoving(args);

            if (IsVertical)
            {
                target.Width = position;
            }
            else
            {
                target.Height = position;
            }

            OnSplitterMoved(args);
        }
    }

    /// <summary>
    /// Gets or sets which edge of its container the splitter is docked to.
    /// </summary>
    /// <remarks>
    /// A splitter cannot fill its container; assigning <see cref="F:System.Windows.Forms.DockStyle.Fill" />
    /// or <see cref="F:System.Windows.Forms.DockStyle.None" /> is rejected, as WinForms does.
    /// </remarks>
    [DefaultValue(DockStyle.Left)]
    public override DockStyle Dock
    {
        get => base.Dock;
        set
        {
            if (value is DockStyle.Fill or DockStyle.None)
            {
                throw new ArgumentException("A Splitter must be docked to an edge.", nameof(value));
            }

            base.Dock = value;
        }
    }

    /// <summary>
    /// Gets a value indicating whether the splitter runs vertically, resizing width rather than height.
    /// </summary>
    [Browsable(false)]
    public bool IsVertical => Dock is DockStyle.Left or DockStyle.Right;

    /// <summary>
    /// Gets or sets the border style of the splitter.
    /// </summary>
    [DefaultValue(BorderStyle.None)]
    public new BorderStyle BorderStyle
    {
        get => base.BorderStyle;
        set => base.BorderStyle = value;
    }

    /// <summary>
    /// Raises the <see cref="E:System.Windows.Forms.Splitter.SplitterMoving" /> event.
    /// </summary>
    /// <param name="e">The event data.</param>
    protected virtual void OnSplitterMoving(SplitterEventArgs e) => SplitterMoving?.Invoke(this, e);

    /// <summary>
    /// Raises the <see cref="E:System.Windows.Forms.Splitter.SplitterMoved" /> event.
    /// </summary>
    /// <param name="e">The event data.</param>
    protected virtual void OnSplitterMoved(SplitterEventArgs e) => SplitterMoved?.Invoke(this, e);

    /// <summary>
    /// The sibling the splitter resizes: the control docked to the same edge just before it.
    /// </summary>
    private Control? FindTarget()
    {
        var siblings = Parent?.Controls;
        if (siblings == null)
        {
            return null;
        }

        Control? candidate = null;
        for (var i = 0; i < siblings.Count; i++)
        {
            var sibling = siblings[i];
            if (ReferenceEquals(sibling, this))
            {
                continue;
            }

            if (sibling.Dock == Dock)
            {
                candidate = sibling;
            }
        }

        return candidate;
    }
}
