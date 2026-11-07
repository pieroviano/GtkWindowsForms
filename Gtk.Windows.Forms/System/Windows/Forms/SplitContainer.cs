/*
 * Based on GTK components, it is a cross-platform interface component that is compatible with the native C# control winform interface.
 * Use this component GtkSystem.Windows.Forms instead of Microsoft.WindowsDesktop.App.WindowsForms, compile once, and run across Windows, Linux, and macOS
 * Technical support 438865652@qq.com，https://www.gtkapp.com, https://gitee.com/easywebfactory, https://github.com/easywebfactory438865652@qq.com，https://www.gtkapp.com, https://gitee.com/easywebfactory, https://github.com/easywebfactory
 * author:chenhongjin
 */

using System.ComponentModel;
using Gtk;
using GtkSystem.Windows.Forms.GtkControls.ControlBase;

namespace System.Windows.Forms;

/// <summary>
/// Represents a control consisting of a movable bar that divides a container's display area into two resizable panels.
/// </summary>
[DesignerCategory("Component")]
public partial class SplitContainer : ContainerControl
{
    /// <summary>
    /// The splitter distance
    /// </summary>
    public int _SplitterDistance;

    /// <summary>
    /// The self
    /// </summary>
    public readonly SplitContainerBase self = new();

    private FixedPanel _fixedPanel = FixedPanel.Panel1;

    private SplitterPanel _panel1;

    private SplitterPanel _panel2;

    private int _SplitterWidth;

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.SplitContainer" /> class.
    /// </summary>
    public SplitContainer() : base()
    {
        self.Override.sender = this;
        _panel1 = new SplitterPanel(this);
        _panel1.self.contaner.Name = "Child1";
        _panel2 = new SplitterPanel(this);
        _panel2.self.contaner.Name = "Child2";
        self.StartChild = _panel1.Widget;
        self.EndChild = _panel2.Widget;

        // GTK clamps the paned's position while it has no allocation (hidden parent, or not yet laid out) and
        // never restores it, so the WinForms distance is kept here and re-applied once there is room.
        self.AddNotification("position", (_, _) => OnGtkPositionChanged());
        self.Mapped += (_, _) => BeginReapplyDistance();
    }

    /// <summary>
    /// Gets or sets the fixed panel.
    /// </summary>
    /// <value>
    /// The fixed panel.
    /// </value>
    public FixedPanel FixedPanel
    {
        get => _fixedPanel;
        set
        {
            _fixedPanel = value;
            // Gtk4: Paned.PanedChild/indexer removed; per-child resize is ResizeStart/EndChild. The fixed panel
            // keeps its size; with None both panels resize.
            self.ResizeStartChild = value != FixedPanel.Panel1;
            self.ResizeEndChild = value != FixedPanel.Panel2;
        }
    }

    /// <summary>
    /// Gets the GTK control.
    /// </summary>
    /// <value>
    /// The GTK control.
    /// </value>
    public override object GtkControl => self;

    /// <summary>
    /// Gets or sets a value indicating whether the splitter is fixed.
    /// </summary>
    /// <value>
    /// <c>
    /// true
    /// </c> if the splitter is fixed; otherwise,
    /// <c>
    /// false
    /// </c>.
    /// </value>
    public bool IsSplitterFixed
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
    public Orientation Orientation
    {
        get { return self.Orientation == Gtk.Orientation.Horizontal ? Orientation.Vertical : Orientation.Horizontal; }
        set
        {
            self.Orientation = value == Orientation.Horizontal ? Gtk.Orientation.Vertical : Gtk.Orientation.Horizontal;
        }
    }

    /// <summary>
    /// Gets or sets the panel1.
    /// </summary>
    /// <value>
    /// The panel1.
    /// </value>
    public SplitterPanel Panel1
    {
        get
        {
            return _panel1;
        }
        set
        {
            _panel1 = value;
        }
    }

    /// <summary>
    /// Gets or sets the panel2.
    /// </summary>
    /// <value>
    /// The panel2.
    /// </value>
    public SplitterPanel Panel2
    {
        get
        {
            return _panel2;
        }
        set
        {
            _panel2 = value;
        }
    }

    /// <summary>
    /// Gets or sets the splitter distance.
    /// </summary>
    /// <value>
    /// The splitter distance.
    /// </value>
    /// <remarks>
    /// The value persists while the control is hidden or has no size yet (GTK clamps its own position then) and
    /// is applied as soon as the control is allocated, again after it is shown anew. Setting it raises
    /// <see cref="SplitterMoved" /> when the value changes, as WinForms does; a user drag raises
    /// <see cref="SplitterMoving" /> (cancellable) and <see cref="SplitterMoved" />. GTK's own clamping is
    /// not a move and raises nothing. The paned's position is the distance less the legacy 5 px handle offset.
    /// </remarks>
    public int SplitterDistance
    {
        get
        {
            if (_requestedDistance >= 0 && (!IsAllocatedAlongAxis() || self.Position == _appliedPosition))
            {
                return _requestedDistance;
            }

            return self.Position + 5;
        }
        set
        {
            var previous = SplitterDistance;
            _requestedDistance = Math.Max(1, value);
            _SplitterDistance = Math.Max(1, value - 5);
            ApplyDistance();
            var current = SplitterDistance;
            if (current != previous)
            {
                OnSplitterMoved(new SplitterEventArgs(0, 0, current, 0));
            }
        }
    }

    /// <summary>Occurs when the splitter is being moved by the user; cancelling puts it back.</summary>
    public event SplitterCancelEventHandler? SplitterMoving;

    /// <summary>Occurs when the splitter has moved, by the user or by setting <see cref="SplitterDistance" />.</summary>
    public event SplitterEventHandler? SplitterMoved;

    /// <summary>Raises the <see cref="SplitterMoving" /> event.</summary>
    /// <param name="e">The event data.</param>
    protected virtual void OnSplitterMoving(SplitterCancelEventArgs e) => SplitterMoving?.Invoke(this, e);

    /// <summary>Raises the <see cref="SplitterMoved" /> event.</summary>
    /// <param name="e">The event data.</param>
    protected virtual void OnSplitterMoved(SplitterEventArgs e) => SplitterMoved?.Invoke(this, e);

    /// <summary>The distance last asked for, in WinForms terms (-1 = never set).</summary>
    private int _requestedDistance = -1;

    /// <summary>The paned position this control last wrote.</summary>
    private int _appliedPosition = -1;

    /// <summary>Set while this control writes the paned position, so its notification is not a move.</summary>
    private bool _applyingDistance;

    /// <summary>The allocation along the splitter's axis at the last notification.</summary>
    private int _lastAxisAllocation;

    /// <summary>While true GTK's position notifications are clamps, not moves (mapped, not yet re-applied).</summary>
    private bool _reapplyPending;

    private int AxisAllocation() =>
        self.Orientation == Gtk.Orientation.Horizontal ? self.AllocatedWidth : self.AllocatedHeight;

    private bool IsAllocatedAlongAxis() => AxisAllocation() > 1;

    /// <summary>Writes the requested distance to the paned, clamped to what the allocation can hold.</summary>
    private void ApplyDistance()
    {
        if (_requestedDistance < 0)
        {
            return;
        }

        var target = Math.Max(1, _requestedDistance - 5);
        var axis = AxisAllocation();
        if (axis > 1)
        {
            target = Math.Min(target, Math.Max(1, axis - 1));
        }

        _appliedPosition = target;
        if (self.Position == target)
        {
            return;
        }

        _applyingDistance = true;
        try
        {
            self.Position = target;
        }
        finally
        {
            _applyingDistance = false;
        }
    }

    /// <summary>The control was mapped (first show, or shown again): re-apply the distance once it has room.</summary>
    private void BeginReapplyDistance()
    {
        if (_requestedDistance < 0)
        {
            return;
        }

        _reapplyPending = true;
        _lastAxisAllocation = 0;
        var tries = 0;
        GLib.Timeout.Add(30, () =>
        {
            if (self.Handle == IntPtr.Zero)
            {
                return false;
            }

            if (IsAllocatedAlongAxis())
            {
                _lastAxisAllocation = AxisAllocation();
                ApplyDistance();
                _reapplyPending = false;

                return false;
            }

            if (++tries < 100)
            {
                return true;
            }

            _reapplyPending = false;

            return false;
        });
    }

    /// <summary>GTK reported a new paned position: a user move, a resize, or a clamp.</summary>
    private void OnGtkPositionChanged()
    {
        if (_applyingDistance || _reapplyPending || !self.IsMapped)
        {
            return;
        }

        var axis = AxisAllocation();
        if (axis <= 1)
        {
            // GTK clamping a control that has no room (yet).
            return;
        }

        var distance = self.Position + 5;
        if (axis != _lastAxisAllocation)
        {
            // The control was resized: GTK moved the position by its own rule (the fixed panel keeps its
            // size, with None the panels share the change). Not a move; just follow.
            _lastAxisAllocation = axis;
            _requestedDistance = distance;
            _appliedPosition = self.Position;

            return;
        }

        if (distance == _requestedDistance)
        {
            return;
        }

        var previousPosition = _appliedPosition >= 0 ? _appliedPosition : _requestedDistance - 5;
        var args = new SplitterCancelEventArgs(0, 0, distance, 0);
        OnSplitterMoving(args);
        if (args.Cancel)
        {
            _applyingDistance = true;
            try
            {
                self.Position = Math.Max(1, previousPosition);
            }
            finally
            {
                _applyingDistance = false;
            }

            return;
        }

        _requestedDistance = distance;
        _appliedPosition = self.Position;
        OnSplitterMoved(new SplitterEventArgs(0, 0, distance, 0));
    }

    /// <summary>
    /// Gets or sets the splitter increment.
    /// </summary>
    /// <value>
    /// The splitter increment.
    /// </value>
    public int SplitterIncrement
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the width of the splitter.
    /// </summary>
    /// <value>
    /// The width of the splitter.
    /// </value>
    public int SplitterWidth
    {
        get { return _SplitterWidth; }
        set { _SplitterWidth = value; self.WideHandle = value > 2; }
    }
}