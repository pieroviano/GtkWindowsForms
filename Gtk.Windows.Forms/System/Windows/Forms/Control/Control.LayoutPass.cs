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
    private Size LayoutClientSize()
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
        // never maps). Use the requested Width/Height, which reflect the assigned Size, rather than
        // ClientSize: a Form's ClientSize falls back to its GTK default size, whose height is not updated by
        // the Height setter and so would report a stale, too-small client area here.
        return new Size(Width, Height);
    }

    /// <summary>The container size the last pass ran against; anchoring follows the delta from it.</summary>
    private Size _lastLayoutSize = Size.Empty;

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

        var children = LayoutChildren();
        if (children.Count == 0)
        {
            return;
        }

        // Not IsRealized: an Overlay is a windowless container, so it reports false even once GTK has
        // given it a real allocation - which is the thing layout actually needs.
        if (children[0].Widget?.Parent is not Gtk.Overlay overlay)
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
    /// <param name="children">The participating children, in collection order.</param>
    /// <param name="client">The container's current client size.</param>
    /// <param name="isRealAllocation">
    /// Whether <paramref name="client" /> is the container overlay's real GTK allocation rather than the
    /// requested-size fallback. Anchoring is measured only between real allocations: the fallback is the
    /// container's outer requested size, which does not match the real client area, so letting it seed the
    /// anchor baseline would make the first real pass compute a bogus size delta and shove
    /// bottom/right-anchored controls off their designed positions (e.g. over a fixed neighbour).
    /// </param>
    private void Arrange(List<Control> children, Size client, bool isRealAllocation)
    {
        var padding = Padding;
        var remaining = new Rectangle(
            padding.Left,
            padding.Top,
            Math.Max(0, client.Width - padding.Horizontal),
            Math.Max(0, client.Height - padding.Vertical));

        // Anchoring follows the delta from the previous *real* client size. A fallback-sized pass neither
        // establishes nor consumes that baseline (delta stays zero, so anchored children keep their designed
        // positions); the first real allocation becomes the baseline (still zero delta) and only later real
        // resizes move anchored children.
        var delta = Size.Empty;
        if (isRealAllocation)
        {
            if (!_lastLayoutSize.IsEmpty)
            {
                delta = new Size(client.Width - _lastLayoutSize.Width, client.Height - _lastLayoutSize.Height);
            }

            _lastLayoutSize = client;
        }

        // WinForms docks from the end of the collection towards the start: the control added last is
        // docked first and takes its edge from the full display rectangle, and Fill gets whatever is left
        // when its turn comes.
        for (var i = children.Count - 1; i >= 0; i--)
        {
            var child = children[i];
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
                    ApplyAnchor(child, delta);

                    break;
            }
        }
    }

    /// <summary>
    /// Moves and resizes an undocked child by the container's size delta, according to its
    /// <see cref="P:System.Windows.Forms.Control.Anchor" />.
    /// </summary>
    /// <param name="child">The child to re-anchor.</param>
    /// <param name="delta">How much the container grew (or shrank).</param>
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
