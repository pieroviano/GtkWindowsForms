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

    private bool _AutoScroll = true;

    /// <summary>
    /// Occurs when the control or its scrollable child is scrolled.
    /// </summary>
    public virtual event ScrollEventHandler Scroll
    {
        add { if (scrollbase != null) { scrollbase.Scroll += value; } }
        remove { if (scrollbase != null) { scrollbase.Scroll -= value; } }
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
            if (AutoSize == false && scrollbase != null)
            {
                scrollbase.AutoScroll = value;
            }
            PerformLayout(this, "AutoScroll");
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
                if (scrollbase != null) { scrollbase.AutoScroll = false; }
            }
            else
            {
                if (scrollbase != null) { scrollbase.AutoScroll = _AutoScroll; }
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
        set { if (scrollbase != null) { scrollbase.HScroll = value; } }
    }

    /// <summary>
    /// Gets or sets the scrollable window implementation backing this control.
    /// </summary>
    protected virtual IScrollableBoxBase? scrollbase
    {
        get;
        set;
    }

    private HScrollProperties? _horizontalScroll;
    private VScrollProperties? _verticalScroll;

    /// <summary>
    /// Gets the characteristics associated with the horizontal scroll bar.
    /// </summary>
    public HScrollProperties HorizontalScroll => _horizontalScroll ??= new HScrollProperties(this);

    /// <summary>
    /// Gets the characteristics associated with the vertical scroll bar.
    /// </summary>
    public VScrollProperties VerticalScroll => _verticalScroll ??= new VScrollProperties(this);

    /// <summary>
    /// Gets or sets whether the vertical scrollbar is visible.
    /// </summary>
    protected bool VScroll
    {
        get => scrollbase?.VScroll ?? false;
        set { if (scrollbase != null) { scrollbase.VScroll = value; } }
    }

    /// <summary>
    /// Forces the control to perform layout logic for a specific affected control/property.
    /// </summary>
    /// <param name="affectedControl">
    /// Control affected by the layout.
    /// </param>
    /// <param name="affectedProperty">
    /// Property name that changed.
    /// </param>
    public override void PerformLayout(Control affectedControl, string affectedProperty)
    {
        if (affectedProperty == "AutoScroll" && affectedControl.Widget is Gtk.ScrolledWindow sw)
        {
            if (sw.Child is Gtk.Viewport view)
            {
                if (view.Child is Gtk.Overlay lay)
                {
                    UpdatePerformLayout(sw, lay, _AutoScroll);
                }
            }
            else if (sw.Child is Gtk.Overlay lay)
            {
                UpdatePerformLayout(sw, lay, _AutoScroll);
            }
        }
        base.PerformLayout(affectedControl, affectedProperty);
    }

    /// <summary>
    /// Scrolls the specified child control into view.
    /// </summary>
    /// <param name="activeControl">
    /// The control to bring into view.
    /// </param>
    public void ScrollControlIntoView(Control activeControl)
    {
        if (activeControl != null && activeControl.Widget != null && scrollbase != null && AutoScroll == true)
        {
            if (this.Widget != null)
            {
                activeControl.Widget.Window.GetOrigin(out var sx, out var sy);
                this.Widget.Window.GetOrigin(out var px, out var py);
                var hvalue = HScroll ? sx - px : -1;
                var vvalue = VScroll ? sy - py : -1;
                if (this.Widget is Gtk.Window win)
                {
                    vvalue -= 20;
                    if (win.Titlebar != null)
                        vvalue -= win.Titlebar.AllocatedHeight;
                }
                var scrolled = scrollbase.ScrolledWindow;
                if (scrolled != null)
                    scrollbase.ScrollView(hvalue + scrolled.Hadjustment.Value, vvalue + scrolled.Vadjustment.Value);
            }
        }
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

    internal void UpdatePerformLayout(Gtk.ScrolledWindow scrolled, Gtk.Overlay lay, bool autoscroll)
    {
        if (lay == null || lay.Child == null || scrolled == null)
            return;
        if (autoscroll == false)
            lay.SetSizeRequest(-1, -1);
        else
        {
            lay.SetSizeRequest(1, 1);
            if (lay.IsRealized)
            {
                foreach (Gtk.Widget widget in lay.Children)
                {
                    if (lay.Child.Handle.Equals(widget.Handle))
                    {
                        lay.WidthRequest = Math.Max(lay.WidthRequest, widget.MarginStart + widget.WidthRequest + scrolled.Allocation.Left + 20);
                        lay.HeightRequest = Math.Max(lay.HeightRequest, widget.MarginTop + widget.HeightRequest + scrolled.Allocation.Top + 20);
                    }
                    else
                    {
                        lay.WidthRequest = Math.Max(lay.WidthRequest, widget.MarginStart + widget.AllocatedWidth + scrolled.Allocation.Left + 20);
                        lay.HeightRequest = Math.Max(lay.HeightRequest, widget.MarginTop + widget.AllocatedHeight + scrolled.Allocation.Top + 20);
                    }
                }
            }
            else
            {
                foreach (Gtk.Widget widget in lay.Children)
                {
                    lay.WidthRequest = Math.Max(lay.WidthRequest, widget.MarginStart + widget.WidthRequest + scrolled.Allocation.Left + 20);
                    lay.HeightRequest = Math.Max(lay.HeightRequest, widget.MarginTop + widget.HeightRequest + scrolled.Allocation.Top + 20);
                }
            }
            lay.QueueResize();
        }
    }
}