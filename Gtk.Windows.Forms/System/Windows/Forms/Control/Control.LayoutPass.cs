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
/// baseline. Invisible children are anchored too (WinForms keeps their bounds current), so nothing has to
/// catch up when they are shown; a child whose bounds are assigned by code, or that is added once layout
/// has started, is baselined at the container's current size; and a size change made before the window's
/// first real allocation (the baseline is then the container's <em>requested</em> size, see
/// <c>AnchorBaselineKind</c>) is still applied as a resize.
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

        /// <summary>The container's requested size; for a form, its designed client size.</summary>
        Requested,

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
    /// <param name="clientHeight">The overlay client height the container is being grown to.</param>
    /// <remarks>
    /// The next real passes rebase every child's baseline (visible or not) by the absorbed growth rather
    /// than forgetting it. Growth beyond <paramref name="clientHeight" />, and any width change, is a
    /// genuine resize (e.g. the application restoring a saved window size in <c>Shown</c>) and is applied.
    /// </remarks>
    internal void AbsorbLayoutGrowth(int clientHeight) => _absorbHeightTarget = clientHeight;

    /// <summary>
    /// For a window: the size of its real client area as the user sees it, and whether the window's height
    /// was set explicitly (so its client height differing from the designed one is a resize, not titlebar
    /// compensation). False for every other container.
    /// </summary>
    internal virtual bool TryGetWindowClientSize(out Size client, out bool heightAssigned)
    {
        client = Size.Empty;
        heightAssigned = false;

        return false;
    }

    /// <summary>Ends <see cref="AbsorbLayoutGrowth" />; later growth is treated as a resize.</summary>
    internal void StopAbsorbingLayoutGrowth() => _absorbHeightTarget = 0;

    /// <summary>
    /// The size this container asks GTK for, which is the only size that exists before it is mapped. For a
    /// form that is the client size it was designed at (GTK rewrites a window's default size once it has
    /// sized the window, titlebar included, so that is not reliable).
    /// </summary>
    private Size RequestedLayoutSize()
    {
        var widget = Widget;
        if (widget == null)
        {
            return Size.Empty;
        }

        if (widget is Gtk.Window window)
        {
            var designed = (this as Form)?.DesignedClientSize ?? Size.Empty;
            if (designed.Width > 0 && designed.Height > 0)
            {
                return designed;
            }

            return new Size(
                window.WidthRequest != -1 ? window.WidthRequest : window.DefaultWidth,
                window.HeightRequest != -1 ? window.HeightRequest : window.DefaultHeight);
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

        size = RequestedLayoutSize();
        kind = AnchorBaselineKind.Requested;

        return size.Width > 1 && size.Height > 1;
    }

    /// <summary>Whether layout has started: layout was resumed or a pass ran, so children have baselines.</summary>
    private bool _anchorLayoutStarted;

    /// <summary>
    /// Gives every child that has no anchor baseline one at the container's current size - "the size the
    /// children were positioned for". Called when layout is resumed (the designer's end-of-InitializeComponent
    /// hook), so a size change made after construction but before the first pass still counts as a resize.
    /// Children added later are seeded by <see cref="SeedAnchorBaselineFor" />.
    /// </summary>
    internal void SeedAnchorBaselines()
    {
        _anchorLayoutStarted = true;
        if (Controls is not { } controls)
        {
            return;
        }

        foreach (var entry in controls)
        {
            if (entry is Control child)
            {
                SeedAnchorBaselineFor(child);
            }
        }
    }

    /// <summary>
    /// Baselines a child that has none at this container's current size. Used when a control is added after
    /// layout started (WinForms computes its anchors against the parent's display rectangle as it is when the
    /// control is added). Before layout starts nothing is seeded: the designer assigns a container's children
    /// before the container's own size.
    /// </summary>
    internal void SeedAnchorBaselineFor(Control child)
    {
        if (!_anchorLayoutStarted
            || child._anchorBaselineKind != AnchorBaselineKind.None
            || child.Widget == null
            || child.Dock != DockStyle.None
            || child is StatusStrip
            || !TryGetLayoutBaseline(out var size, out var kind))
        {
            return;
        }

        child._anchorBaseline = size;
        child._anchorBaselineKind = kind;
    }

    /// <summary>
    /// Re-bases this child's anchor baseline to its container's current size. Code assigning a child's
    /// bounds positions it for the container as it is now, so a later pass must measure the container's
    /// change from here. A child that has no baseline yet is left alone: before layout is first
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
    private Size TakeAnchorDelta(Size client, bool isRealAllocation, Size requestedNow)
    {
        if (!isRealAllocation)
        {
            // Before the first real allocation nothing moves; just make sure the child has a baseline.
            if (_anchorBaselineKind == AnchorBaselineKind.None && requestedNow.Width > 1 && requestedNow.Height > 1)
            {
                _anchorBaseline = requestedNow;
                _anchorBaselineKind = AnchorBaselineKind.Requested;
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
                delta = RequestedDelta(requestedNow);

                break;
        }

        _anchorBaseline = client;
        _anchorBaselineKind = AnchorBaselineKind.Real;

        return delta;
    }

    /// <summary>
    /// The first real allocation: how far the container moved from the size the children were positioned for.
    /// The real client size differs from the requested one by window chrome (titlebar, status strip), so the
    /// chrome is not a resize.
    /// </summary>
    private Size RequestedDelta(Size requestedNow)
    {
        if (Parent is not { } container)
        {
            return Size.Empty;
        }

        if (container.TryGetWindowClientSize(out var windowClient, out var heightAssigned))
        {
            // A window: measure what the window actually opened at (a requested size is only a minimum
            // and may not be honoured), in the units the baseline is in - the form's client area.
            return new Size(
                windowClient.Width - _anchorBaseline.Width,
                heightAssigned ? windowClient.Height - _anchorBaseline.Height : 0);
        }

        return new Size(
            requestedNow.Width > 1 ? requestedNow.Width - _anchorBaseline.Width : 0,
            requestedNow.Height > 1 ? requestedNow.Height - _anchorBaseline.Height : 0);
    }

    /// <summary>The nesting depth of <see cref="SuspendLayout" /> calls not yet matched by a resume.</summary>
    private int _layoutSuspendDepth;

    /// <summary>A visibility-triggered pass was deferred by a suspension.</summary>
    private bool _layoutPassPending;

    /// <summary>
    /// A child's visibility changed: dock layout (which skips invisible children) has to re-run, now or on
    /// <see cref="ResumeLayout(bool)" /> if layout is suspended.
    /// </summary>
    internal void ChildVisibilityChanged()
    {
        if (_layoutSuspendDepth > 0)
        {
            _layoutPassPending = true;

            return;
        }

        PerformDockAnchorLayout();
    }

    /// <summary>Counts a suspension.</summary>
    internal void EnterLayoutSuspension() => _layoutSuspendDepth++;

    /// <summary>Counts a resume, and runs the pass a suspension deferred once the last one is matched.</summary>
    private void LeaveLayoutSuspension()
    {
        if (_layoutSuspendDepth > 0)
        {
            _layoutSuspendDepth--;
        }

        if (_layoutSuspendDepth == 0 && _layoutPassPending)
        {
            _layoutPassPending = false;
            PerformDockAnchorLayout();
        }
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
        var children = LayoutChildren(out var hiddenChildren);
        var anchorChild = children.Count > 0 ? children[0] : AnyChildWithWidget();
        if (anchorChild?.Widget?.Parent is not Gtk.Overlay overlay)
        {
            return;
        }

        HookLayoutParent(overlay);
        _anchorLayoutStarted = true;

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
            Arrange(children, hiddenChildren, client, isRealAllocation);
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
    /// children, so a hidden docked panel consumes no space. The invisible ones are returned separately: they
    /// take no part in docking, but are anchored like the others so their bounds stay current.
    /// </summary>
    private List<Control> LayoutChildren(out List<Control> hidden)
    {
        var result = new List<Control>();
        hidden = new List<Control>();
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

            if (entry is Control { Widget: not null } child)
            {
                (child.Visible ? result : hidden).Add(child);
            }
        }

        return result;
    }

    /// <summary>
    /// The arithmetic itself.
    /// </summary>
    /// <param name="children">The participating (visible) children, in collection order.</param>
    /// <param name="hiddenChildren">The invisible children: not docked, but anchored.</param>
    /// <param name="client">The container's current client size.</param>
    /// <param name="isRealAllocation">
    /// Whether <paramref name="client" /> is the container overlay's real GTK allocation rather than the
    /// requested-size fallback. Anchoring is measured only between real allocations: the fallback is the
    /// container's outer requested size, which does not match the real client area, so it is kept as a
    /// separate kind of baseline: the first real pass applies only the change of the requested size, not the
    /// difference to the real one, which would shove bottom/right-anchored controls off their designed
    /// positions (e.g. over a fixed neighbour).
    /// </param>
    private void Arrange(List<Control> children, List<Control> hiddenChildren, Size client, bool isRealAllocation)
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
        var requestedNow = RequestedLayoutSize();
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

        // Invisible children are anchored too, so their bounds are current when they are shown.
        foreach (var hidden in hiddenChildren)
        {
            if (hidden.Dock == DockStyle.None)
            {
                ApplyAnchor(hidden, hidden.TakeAnchorDelta(client, isRealAllocation, requestedNow));
            }
            else
            {
                hidden._anchorBaselineKind = AnchorBaselineKind.None;
            }
        }

        // WinForms docks from the end of the collection towards the start: the control added last is
        // docked first and takes its edge from the full display rectangle, and Fill gets whatever is left
        // when its turn comes.
        for (var i = children.Count - 1; i >= 0; i--)
        {
            var child = children[i];
            if (child.Dock != DockStyle.None && child._anchorBaselineKind != AnchorBaselineKind.None)
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
                    ApplyAnchor(child, child.TakeAnchorDelta(client, isRealAllocation, requestedNow));

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
