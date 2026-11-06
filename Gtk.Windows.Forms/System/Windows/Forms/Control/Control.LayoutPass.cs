// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.Drawing;

namespace System.Windows.Forms;

/// <summary>
/// The dock/anchor arrangement pass — WinForms' <c>DefaultLayout</c> arithmetic, run over the GTK
/// children of a container.
/// </summary>
/// <remarks>
/// <para>
/// Dock and anchor used to be expressed by handing each child a GTK alignment: a top-docked child was
/// given <c>Halign=Fill, Valign=Start</c>, and that was that. GTK then aligned every such child to the
/// same edge, so two top-docked panels landed on top of each other instead of stacking, and
/// <see cref="P:System.Windows.Forms.DockStyle.Fill" /> covered the whole container rather than what was
/// left of it. Anchoring was computed once when the widget was realized and never again, so resizing the
/// container moved nothing.
/// </para>
/// <para>
/// WinForms instead computes child rectangles itself: docked children consume edges of a shrinking
/// remainder, in reverse order of the control collection, and anchored children follow the container's
/// size delta. That arithmetic is reproduced here and pushed to GTK as absolute margins with
/// <c>Align.Start</c>, so the widget tree carries positions rather than alignment rules.
/// </para>
/// <para>
/// The anchor delta is tracked <em>per child</em>, as WinForms does (it stores each control's distances to
/// the parent's edges): every anchored child remembers the container client size its current bounds
/// correspond to. A pass moves a child by (container size now - that baseline), then advances the
/// baseline. Consequences: a child that was invisible during a resize catches up on the pass that runs
/// when it becomes visible; a child whose bounds are assigned by code is re-based to the container's
/// current size; and a size change made before the window's first real allocation (the baseline is then
/// the container's <em>requested</em> size, see <c>AnchorBaselineKind</c>) is still applied as a resize.
/// </para>
/// <para>
/// The expected values are not invented: they were taken from real <c>System.Windows.Forms</c> for the
/// same layouts, and the cases are pinned in <c>LayoutArrangementFunctionalTests</c>.
/// </para>
/// </remarks>
public partial class Control
{
    /// <summary>
    /// Gets the rectangle that child controls are arranged in: the client area less
    /// <see cref="P:System.Windows.Forms.Control.Padding" />.
    /// </summary>
    /// <remarks>
    /// This was an auto-property that nothing ever assigned, so it returned <see cref="Rectangle.Empty" />
    /// for every control - and any application that positioned children against it put them all at the
    /// origin with no size. It now reports what the arrangement pass actually lays out into.
    /// </remarks>
    public virtual Rectangle DisplayRectangle
    {
        get
        {
            var client = LayoutClientSize();
            var padding = Padding;

            return new Rectangle(
                padding.Left,
                padding.Top,
                Math.Max(0, client.Width - padding.Horizontal),
                Math.Max(0, client.Height - padding.Vertical));
        }
    }

    /// <summary>
    /// The size of the area children are actually placed in.
    /// </summary>
    /// <remarks>
    /// Children of a wrapper live in a GTK container inside it, not in the wrapper's own widget, and the
    /// two allocations differ by whatever chrome sits between them. Layout and
    /// <see cref="DisplayRectangle" /> both go through here so they cannot disagree about the area they
    /// describe; <see cref="P:System.Windows.Forms.Control.ClientSize" /> is the fallback for a control
    /// that has no children yet.
    /// </remarks>
    private Size LayoutClientSize() => RealLayoutClientSize() ?? new Size(Width, Height);

    /// <summary>The overlay's real allocation, or <see langword="null" /> while it has none.</summary>
    private Size? RealLayoutClientSize()
    {
        var controls = Controls;
        if (controls != null)
        {
            foreach (var entry in controls)
            {
                if (entry is Control child
                    && child.Widget?.Parent is Gtk.Overlay overlay
                    && overlay.AllocatedWidth > 1
                    && overlay.AllocatedHeight > 1)
                {
                    return new Size(overlay.AllocatedWidth, overlay.AllocatedHeight);
                }
            }
        }

        // No real container allocation (an unmapped window, or the display-less test host where the window
        // never maps). LayoutClientSize then uses the requested Width/Height, which reflect the assigned
        // Size, rather than ClientSize: a Form's ClientSize falls back to its GTK default size, whose
        // height is not updated by the Height setter and so would report a stale, too-small client area.
        return null;
    }

    /// <summary>
    /// The real container client size the last real pass ran against. Only used to measure how much the
    /// container grew between two passes (see <see cref="AbsorbLayoutGrowth" />); anchoring itself follows
    /// each child's own baseline.
    /// </summary>
    private Size _lastLayoutSize = Size.Empty;

    /// <summary>
    /// What a child's anchor baseline measures: a real GTK allocation, or the container's requested size
    /// (the only size that exists before the window is mapped).
    /// </summary>
    private enum AnchorBaselineKind
    {
        /// <summary>No baseline yet; the next pass seeds one at the container's current size.</summary>
        None,

        /// <summary>The container's requested size; for a window the height is in client units.</summary>
        Requested,

        /// <summary>The container's requested size, whose height was set explicitly (window units).</summary>
        RequestedWindowHeight,

        /// <summary>A real allocation of the container's overlay.</summary>
        Real,
    }

    /// <summary>The container client size this child's current bounds correspond to.</summary>
    private Size _anchorBaseline = Size.Empty;

    /// <summary>What <see cref="_anchorBaseline" /> measures.</summary>
    private AnchorBaselineKind _anchorBaselineKind;

    /// <summary>
    /// The client height up to which growth of the container is compensation, not a resize (0 = none).
    /// </summary>
    private int _absorbHeightTarget;

    /// <summary>
    /// Declares that, until <see cref="StopAbsorbingLayoutGrowth" />, growth of the container's client
    /// height up to <paramref name="clientHeight" /> compensates for GTK squeezing the viewport (the
    /// content-fit and designed-client-size passes) and must keep every anchored child at its designed
    /// position.
    /// </summary>
    /// <param name="clientHeight">The client height the container is being grown to.</param>
    /// <remarks>
    /// The next real passes rebase every child's baseline (visible or not) by the absorbed growth rather
    /// than forgetting it. Growth beyond <paramref name="clientHeight" />, and any width change, is a
    /// genuine resize (e.g. the application restoring a saved window size in <c>Shown</c>) and is applied.
    /// </remarks>
    internal void AbsorbLayoutGrowth(int clientHeight) =>
        _absorbHeightTarget = Math.Max(_absorbHeightTarget, clientHeight);

    /// <summary>Ends <see cref="AbsorbLayoutGrowth" />; later growth is treated as a resize.</summary>
    internal void StopAbsorbingLayoutGrowth() => _absorbHeightTarget = 0;

    /// <summary>
    /// The size this container asks GTK for, which is the only size that exists before it is mapped.
    /// </summary>
    /// <param name="windowHeight">
    /// Whether the height is an explicit request on a window (window units, titlebar included) rather than
    /// the client-height default set by <c>ClientSize</c>.
    /// </param>
    private Size RequestedLayoutSize(out bool windowHeight)
    {
        windowHeight = false;
        var widget = Widget;
        if (widget == null)
        {
            return Size.Empty;
        }

        if (widget is Gtk.Window window)
        {
            windowHeight = window.HeightRequest != -1;

            // GTK rewrites a window's default size once it has sized the window (the titlebar ends up in the
            // default height), so the size the form was designed at is the reliable client-unit default.
            var designed = (this as Form)?.DesignedClientSize ?? Size.Empty;
            var defaultWidth = designed.Width > 0 ? designed.Width : window.DefaultWidth;
            var defaultHeight = designed.Height > 0 ? designed.Height : window.DefaultHeight;

            return new Size(
                window.WidthRequest != -1 ? window.WidthRequest : defaultWidth,
                windowHeight ? window.HeightRequest : defaultHeight);
        }

        return new Size(widget.WidthRequest, widget.HeightRequest);
    }

    /// <summary>The container's current layout size as an anchor baseline: real if it has one, else requested.</summary>
    private bool TryGetLayoutBaseline(out Size size, out AnchorBaselineKind kind)
    {
        var real = RealLayoutClientSize();
        if (real is Size realSize)
        {
            size = realSize;
            kind = AnchorBaselineKind.Real;

            return true;
        }

        size = RequestedLayoutSize(out var windowHeight);
        kind = windowHeight ? AnchorBaselineKind.RequestedWindowHeight : AnchorBaselineKind.Requested;

        return size.Width > 1 && size.Height > 1;
    }

    /// <summary>
    /// Gives every child that has no anchor baseline one at the container's current size - "the size the
    /// children were positioned for". Called when layout is resumed (the designer's end-of-InitializeComponent
    /// hook), so a size change made after construction but before the first pass still counts as a resize,
    /// and from each pass, so hidden children are covered too.
    /// </summary>
    private void SeedAnchorBaselines()
    {
        if (Controls is not { } controls || !TryGetLayoutBaseline(out var size, out var kind))
        {
            return;
        }

        foreach (var entry in controls)
        {
            if (entry is Control { Widget: not null } child
                && child._anchorBaselineKind == AnchorBaselineKind.None
                && child.Dock == DockStyle.None
                && entry is not StatusStrip)
            {
                child._anchorBaseline = size;
                child._anchorBaselineKind = kind;
            }
        }
    }

    /// <summary>
    /// Re-bases this child's anchor baseline to its container's current size. Code assigning a child's
    /// bounds positions it for the container as it is now, so a later (or catch-up) pass must measure the
    /// container's change from here. A child that has no baseline yet is left alone: before layout is first
    /// resumed the designer assigns a container's children before the container's own size.
    /// </summary>
    private void RebaseAnchorBaseline()
    {
        if (_anchorBaselineKind == AnchorBaselineKind.None
            || Parent is not { _inLayoutPass: false } parent
            || !parent.TryGetLayoutBaseline(out var size, out var kind))
        {
            return;
        }

        _anchorBaseline = size;
        _anchorBaselineKind = kind;
    }

    /// <summary>
    /// How far the container has moved since this child's bounds were set; advances the baseline.
    /// </summary>
    private Size TakeAnchorDelta(
        Size client, bool isRealAllocation, Size requestedNow, bool requestedWindowHeight)
    {
        if (!isRealAllocation)
        {
            // Before the first real allocation nothing moves; just make sure the child has a baseline.
            if (_anchorBaselineKind == AnchorBaselineKind.None && requestedNow.Width > 1 && requestedNow.Height > 1)
            {
                _anchorBaseline = requestedNow;
                _anchorBaselineKind = requestedWindowHeight
                    ? AnchorBaselineKind.RequestedWindowHeight
                    : AnchorBaselineKind.Requested;
            }

            return Size.Empty;
        }

        var delta = Size.Empty;
        switch (_anchorBaselineKind)
        {
            case AnchorBaselineKind.Real:
                delta = new Size(client.Width - _anchorBaseline.Width, client.Height - _anchorBaseline.Height);

                break;

            case AnchorBaselineKind.Requested:
            case AnchorBaselineKind.RequestedWindowHeight:
                // The first real allocation. The real client size differs from the requested one by the
                // window chrome (titlebar), so the chrome is not a resize: only what the requested size
                // changed by since the children were positioned is.
                var width = requestedNow.Width > 1 ? requestedNow.Width - _anchorBaseline.Width : 0;
                var height = 0;
                if (requestedNow.Height > 1)
                {
                    if (_anchorBaselineKind == AnchorBaselineKind.Requested)
                    {
                        // Baseline in client units. A height set explicitly since is in window units, so
                        // compare the real client height instead (it already excludes the chrome).
                        height = requestedWindowHeight
                            ? client.Height - _anchorBaseline.Height
                            : requestedNow.Height - _anchorBaseline.Height;
                    }
                    else if (requestedWindowHeight)
                    {
                        height = requestedNow.Height - _anchorBaseline.Height;
                    }
                }

                delta = new Size(width, height);

                break;
        }

        _anchorBaseline = client;
        _anchorBaselineKind = AnchorBaselineKind.Real;

        return delta;
    }

    /// <summary>Guards against a pass re-entering through the size changes it makes.</summary>
    private bool _inLayoutPass;

    /// <summary>Whether this container has already hooked its GTK layout parent's allocation.</summary>
    private bool _layoutParentHooked;

    /// <summary>
    /// Arranges this container's children the way WinForms would: docked children first, from the end of
    /// the collection towards the start, then the anchored ones.
    /// </summary>
    /// <remarks>
    /// A no-op until the children have a realized GTK parent with a real allocation - it runs again on
    /// every allocation, so nothing is lost by returning early.
    /// </remarks>
    internal void PerformDockAnchorLayout()
    {
        if (_inLayoutPass)
        {
            return;
        }

        // Not IsRealized: an Overlay is a windowless container, so it reports false even once GTK has
        // given it a real allocation - which is the thing layout actually needs. The overlay is found through
        // any child, hidden ones included: a container whose children are all hidden must still seed their
        // anchor baselines, or a resize before they are shown would be lost.
        var children = LayoutChildren();
        var anchorChild = children.Count > 0 ? children[0] : AnyChildWithWidget();
        if (anchorChild?.Widget?.Parent is not Gtk.Overlay overlay)
        {
            return;
        }

        HookLayoutParent(overlay);

        var client = new Size(overlay.AllocatedWidth, overlay.AllocatedHeight);
        var isRealAllocation = client.Width > 1 && client.Height > 1;
        if (!isRealAllocation)
        {
            // No real allocation yet - the window is realized but not mapped (the state at Form.Load, and the
            // permanent state in a display-less test host where the window never maps). Fall back through
            // LayoutClientSize so the arrangement uses exactly the same client size DisplayRectangle does;
            // the two must agree, since a Fill child is asserted to cover the display rectangle exactly.
            client = LayoutClientSize();
        }

        if (client.Width <= 1 || client.Height <= 1)
        {
            return;
        }

        _inLayoutPass = true;
        try
        {
            Arrange(children, client, isRealAllocation);
        }
        catch (Exception ex)
        {
            Diagnostics.GtkLogger.Debug("Unexpected exception arranging child controls", ex);
        }
        finally
        {
            _inLayoutPass = false;
        }
    }

    /// <summary>The first child control that has a widget, visible or not (the pass's handle on the overlay).</summary>
    private Control? AnyChildWithWidget()
    {
        if (Controls is { } controls)
        {
            foreach (var entry in controls)
            {
                if (entry is Control { Widget: not null } child && entry is not StatusStrip)
                {
                    return child;
                }
            }
        }

        return null;
    }

    /// <summary>
    /// Subscribes to the GTK container the children actually live in. The wrapper's own allocation and
    /// that container's are not the same event, and it is the latter that bounds the children.
    /// </summary>
    private void HookLayoutParent(Gtk.Overlay overlay)
    {
        if (_layoutParentHooked)
        {
            return;
        }

        _layoutParentHooked = true;

        // Gtk4 removed the container size-allocate signal that used to re-run this pass on every resize. Every
        // dock/anchor container carries a Gtk.DrawingArea as its main child (the WinForms background), and
        // Gtk.DrawingArea raises Resize on each re-allocation. Subscribing here - which happens at realize
        // time, before the real allocation exists - means the pass runs again once the overlay is actually
        // sized (the realize-time call returns early at the client.Width <= 1 guard) and on every later
        // resize, which is what makes docked children track the container and anchored children follow its
        // size delta. Re-entrancy from the margins/size-requests the pass sets is guarded by _inLayoutPass.
        if (overlay.Child is Gtk.DrawingArea background)
        {
            background.Resize += (_, _) => PerformDockAnchorLayout();
        }
    }

    /// <summary>
    /// The child controls that take part in layout, in collection order. Non-control entries (raw widgets
    /// added straight to the GTK container) and invisible controls do not: WinForms skips invisible
    /// children, so a hidden docked panel consumes no space.
    /// </summary>
    private List<Control> LayoutChildren()
    {
        var result = new List<Control>();
        var controls = Controls;
        if (controls == null)
        {
            return result;
        }

        foreach (var entry in controls)
        {
            // A StatusStrip is not laid out in this container's overlay: a Form docks it into the dialog
            // content area (below the scrolled content) itself. Its widget therefore lives in a different
            // GTK parent, so feeding it to the arrangement here would (a) let it be picked as children[0] and
            // drive the whole pass off the wrong overlay's (tiny) allocation, and (b) stamp margins/sizes
            // meant for this overlay's coordinate space onto a widget in another container - which collapsed
            // the real content once the strip became visible. Skip it; the content area positions it.
            if (entry is StatusStrip)
            {
                continue;
            }

            if (entry is Control child && child.Visible && child.Widget != null)
            {
                result.Add(child);
            }
        }

        return result;
    }

    /// <summary>
    /// The arithmetic itself.
    /// </summary>
    /// <param name="children">The participating (visible) children, in collection order.</param>
    /// <param name="client">The container's current client size.</param>
    /// <param name="isRealAllocation">
    /// Whether <paramref name="client" /> is the container overlay's real GTK allocation rather than the
    /// requested-size fallback. Anchoring is measured only between real allocations: the fallback is the
    /// container's outer requested size, which does not match the real client area, so it is kept as a
    /// separate kind of baseline: the first real pass applies only the change of the requested size, not the
    /// difference to the real one, which would shove bottom/right-anchored controls off their designed
    /// positions (e.g. over a fixed neighbour).
    /// </param>
    private void Arrange(List<Control> children, Size client, bool isRealAllocation)
    {
        var padding = Padding;
        var remaining = new Rectangle(
            padding.Left,
            padding.Top,
            Math.Max(0, client.Width - padding.Horizontal),
            Math.Max(0, client.Height - padding.Vertical));

        // Anchoring follows each child's own baseline (see the class remarks). A fallback-sized pass only
        // gives children a requested-size baseline and moves nothing, so anchored children keep their
        // designed positions; the first real pass applies whatever the *requested* size changed by since the
        // children were positioned (not the window chrome); later passes apply the real size delta.
        var requestedNow = RequestedLayoutSize(out var requestedWindowHeight);
        if (isRealAllocation)
        {
            // Growth the content-fit / designed-size passes asked for keeps every child's position, hidden
            // ones included: shift their baselines by it instead of forgetting them.
            if (_absorbHeightTarget > 0 && !_lastLayoutSize.IsEmpty && client.Height > _lastLayoutSize.Height)
            {
                var growth = Math.Min(client.Height, _absorbHeightTarget) - _lastLayoutSize.Height;
                if (growth > 0 && Controls is { } all)
                {
                    foreach (var entry in all)
                    {
                        if (entry is Control { _anchorBaselineKind: AnchorBaselineKind.Real } known)
                        {
                            known._anchorBaseline = new Size(
                                known._anchorBaseline.Width, known._anchorBaseline.Height + growth);
                        }
                    }
                }
            }

            _lastLayoutSize = client;
        }

        // Hidden children are not in the list, but must still have a baseline so that a resize while they
        // are hidden reaches them once they are shown.
        if (Controls is { } controls)
        {
            foreach (var entry in controls)
            {
                if (entry is Control { Widget: not null } hidden
                    && hidden._anchorBaselineKind == AnchorBaselineKind.None
                    && !children.Contains(hidden)
                    && hidden.Dock == DockStyle.None
                    && entry is not StatusStrip)
                {
                    hidden.TakeAnchorDelta(client, isRealAllocation, requestedNow, requestedWindowHeight);
                }
            }
        }

        // WinForms docks from the end of the collection towards the start: the control added last is
        // docked first and takes its edge from the full display rectangle, and Fill gets whatever is left
        // when its turn comes.
        for (var i = children.Count - 1; i >= 0; i--)
        {
            var child = children[i];
            if (child.Dock != DockStyle.None)
            {
                // A docked child has no anchor distances; it is seeded afresh if it is ever undocked.
                child._anchorBaselineKind = AnchorBaselineKind.None;
            }

            switch (child.Dock)
            {
                case DockStyle.Top:
                    child.SetLayoutBounds(remaining.X, remaining.Y, remaining.Width, child.Height);
                    remaining.Y += child.Height;
                    remaining.Height -= child.Height;

                    break;

                case DockStyle.Bottom:
                    child.SetLayoutBounds(
                        remaining.X, remaining.Bottom - child.Height, remaining.Width, child.Height);
                    remaining.Height -= child.Height;

                    break;

                case DockStyle.Left:
                    child.SetLayoutBounds(remaining.X, remaining.Y, child.Width, remaining.Height);
                    remaining.X += child.Width;
                    remaining.Width -= child.Width;

                    break;

                case DockStyle.Right:
                    child.SetLayoutBounds(
                        remaining.Right - child.Width, remaining.Y, child.Width, remaining.Height);
                    remaining.Width -= child.Width;

                    break;

                case DockStyle.Fill:
                    child.SetLayoutBounds(remaining.X, remaining.Y, remaining.Width, remaining.Height);

                    break;

                default:
                    ApplyAnchor(
                        child, child.TakeAnchorDelta(client, isRealAllocation, requestedNow, requestedWindowHeight));

                    break;
            }
        }
    }

    /// <summary>
    /// Moves and resizes an undocked child by the container's size delta, according to its
    /// <see cref="P:System.Windows.Forms.Control.Anchor" />.
    /// </summary>
    /// <param name="child">The child to re-anchor.</param>
    /// <param name="delta">How much the container grew (or shrank) since the child's bounds were set.</param>
    /// <remarks>
    /// The unobvious case is <see cref="F:System.Windows.Forms.AnchorStyles.None" />: WinForms moves such
    /// a control by *half* the delta, keeping it proportionally placed rather than pinned. Measured
    /// against real WinForms, not assumed.
    /// </remarks>
    private static void ApplyAnchor(Control child, Size delta)
    {
        if (delta.IsEmpty)
        {
            return;
        }

        var bounds = AnchorBounds(
            new Rectangle(child.Left, child.Top, child.Width, child.Height), child.Anchor, delta);
        child.SetLayoutBounds(bounds.X, bounds.Y, bounds.Width, bounds.Height);
    }

    /// <summary>
    /// Where an anchored control ends up when its container's size changes by <paramref name="delta" />.
    /// </summary>
    /// <param name="bounds">The control's current bounds.</param>
    /// <param name="anchor">Which container edges it is anchored to.</param>
    /// <param name="delta">How much the container grew (negative when it shrank).</param>
    /// <returns>The new bounds.</returns>
    /// <remarks>
    /// Separated from the GTK plumbing so the rule can be checked against the numbers real WinForms
    /// produces, which is what `LayoutArrangementFunctionalTests` does. An edge anchored on both sides
    /// stretches, one side moves, and - the unobvious case - anchored to *neither* moves by half the
    /// delta, keeping the control proportionally placed rather than pinned.
    /// </remarks>
    internal static Rectangle AnchorBounds(Rectangle bounds, AnchorStyles anchor, Size delta)
    {
        var left = (anchor & AnchorStyles.Left) != 0;
        var right = (anchor & AnchorStyles.Right) != 0;
        var top = (anchor & AnchorStyles.Top) != 0;
        var bottom = (anchor & AnchorStyles.Bottom) != 0;

        var x = bounds.X;
        var y = bounds.Y;
        var width = bounds.Width;
        var height = bounds.Height;

        if (left && right)
        {
            width += delta.Width;
        }
        else if (right)
        {
            x += delta.Width;
        }
        else if (!left)
        {
            x += delta.Width / 2;
        }

        if (top && bottom)
        {
            height += delta.Height;
        }
        else if (bottom)
        {
            y += delta.Height;
        }
        else if (!top)
        {
            y += delta.Height / 2;
        }

        return new Rectangle(x, y, width, height);
    }

    /// <summary>
    /// Places this control at an absolute position inside its GTK parent.
    /// </summary>
    /// <param name="x">Left edge, relative to the parent's display rectangle.</param>
    /// <param name="y">Top edge.</param>
    /// <param name="width">Width.</param>
    /// <param name="height">Height.</param>
    /// <remarks>
    /// Alignment is forced to <c>Start</c> so the margins act as coordinates rather than as gaps GTK is
    /// free to distribute, which is what makes the computed rectangle the one that shows up.
    /// </remarks>
    internal void SetLayoutBounds(int x, int y, int width, int height)
    {
        var widget = Widget;
        if (widget == null)
        {
            return;
        }

        widget.Halign = Gtk.Align.Start;
        widget.Valign = Gtk.Align.Start;

        var left = Math.Max(0, x);
        var top = Math.Max(0, y);
        if (widget.MarginStart != left)
        {
            widget.MarginStart = left;
        }

        if (widget.MarginTop != top)
        {
            widget.MarginTop = top;
        }

        if (widget.MarginEnd != 0)
        {
            widget.MarginEnd = 0;
        }

        if (widget.MarginBottom != 0)
        {
            widget.MarginBottom = 0;
        }

        var requestedWidth = Math.Max(0, width);
        var requestedHeight = Math.Max(0, height);
        if (widget.WidthRequest != requestedWidth || widget.HeightRequest != requestedHeight)
        {
            widget.SetSizeRequest(requestedWidth, requestedHeight);
        }
    }
}
