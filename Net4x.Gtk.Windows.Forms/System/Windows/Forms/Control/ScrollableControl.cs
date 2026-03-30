using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms.Layout;
using GtkSystem.Windows.Forms.GtkControls.ControlBase;

namespace System.Windows.Forms;

/// <summary>
/// Defines a base class for controls that support auto-scrolling behavior.
/// </summary>
/// <filterpriority>
/// 2
/// </filterpriority>
[Designer("System.Windows.Forms.Design.ScrollableControlDesigner, System.Design, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
public class ScrollableControl : Control, IArrangedElement, IComponent, IDisposable
{
    /// <summary>
    /// Flag indicating the control is performing an auto-scroll operation.
    /// </summary>
    protected const int ScrollStateAutoScrolling = 1;

    /// <summary>
    /// Flag indicating full-drag behaviour is enabled.
    /// </summary>
    protected const int ScrollStateFullDrag = 16;

    /// <summary>
    /// Flag indicating the horizontal scrollbar is visible.
    /// </summary>
    protected const int ScrollStateHScrollVisible = 2;

    /// <summary>
    /// Flag indicating the user has scrolled the control.
    /// </summary>
    protected const int ScrollStateUserHasScrolled = 8;

    /// <summary>
    /// Flag indicating the vertical scrollbar is visible.
    /// </summary>
    protected const int ScrollStateVScrollVisible = 4;

    private bool _AutoScroll;

    private IScrollableBoxBase? scrollbase;

    /// <summary>
    /// Occurs when the control or its scrollable child is scrolled.
    /// </summary>
    public virtual event ScrollEventHandler Scroll
    {
        add { scrollbase?.Scroll += value; }
        remove { scrollbase?.Scroll -= value; }
    }

    /// <summary>
    /// Initializes a new instance of
    /// <see cref="T:System.Windows.Forms.ScrollableControl" />.
    /// </summary>
    public ScrollableControl() : base()
    {
        scrollbase = GtkControl as IScrollableBoxBase;
    }

    /// <summary>
    /// Gets or sets a value indicating whether the control is automatically scrolled when its contents extend beyond its client area.
    /// </summary>
    public virtual bool AutoScroll
    {
        get => _AutoScroll;
        set
        {
            _AutoScroll = value;
            if (AutoSize == false)
                scrollbase?.AutoScroll = value;
        }
    }

    /// <summary>
    /// Gets or sets the margin around the auto-scroll area.
    /// </summary>
    public Size AutoScrollMargin
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the minimum size of the auto-scroll area.
    /// </summary>
    public Size AutoScrollMinSize
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the current scroll position of the auto-scrollable control.
    /// </summary>
    public Point AutoScrollPosition
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets a value indicating whether the control is automatically resized to fit its contents.
    /// When set, auto-scroll behaviour may be disabled to allow natural sizing.
    /// </summary>
    public override bool AutoSize
    {
        get => base.AutoSize;
        set
        {
            base.AutoSize = value;
            if (value == true)
            {
                scrollbase?.AutoScroll = false;
            }
            else
            {
                scrollbase?.AutoScroll = _AutoScroll;
            }
        }
    }

    /// <summary>
    /// Gets the rectangle that represents the display area of the control (client area).
    /// </summary>
    public override Rectangle DisplayRectangle { get; }

    /// <summary>
    /// Gets or sets whether the horizontal scrollbar is visible.
    /// </summary>
    protected bool HScroll
    {
        get => scrollbase?.HScroll ?? false;
        set => scrollbase?.HScroll = value;
    }

    /// <summary>
    /// Gets or sets whether the vertical scrollbar is visible.
    /// </summary>
    //public VScrollProperties VerticalScroll { get; }
    //public DockPaddingEdges DockPadding { get; }
    //public HScrollProperties HorizontalScroll { get; }
    protected bool VScroll
    {
        get => scrollbase?.VScroll ?? false;
        set => scrollbase?.VScroll = value;
    }

    /// <summary>
    /// Associates an external scrollable window implementation with this control.
    /// </summary>
    /// <param name="scrolledwindow">
    /// An implementation of
    /// <see cref="GtkSystem.Windows.Forms.GtkControls.ControlBase.IScrollableBoxBase" /> used to drive scrolling behavior.
    /// </param>
    public void SetScrolledWindow(IScrollableBoxBase scrolledwindow)
    {
        scrollbase = scrolledwindow;
    }
}