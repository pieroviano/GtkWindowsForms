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

        return ClientSize;
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
        if (client.Width <= 1 || client.Height <= 1)
        {
            return;
        }

        _inLayoutPass = true;
        try
        {
            Arrange(children, client);
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
        // Gtk4-removed: overlay.SizeAllocated (re-layout on resize). TODO(gtk4): notify::default-width or a size-allocate override.
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
    private void Arrange(List<Control> children, Size client)
    {
        var padding = Padding;
        var remaining = new Rectangle(
            padding.Left,
            padding.Top,
            Math.Max(0, client.Width - padding.Horizontal),
            Math.Max(0, client.Height - padding.Vertical));

        // The first pass establishes the baseline; anchoring is relative to a previous size, and there is
        // none yet.
        var delta = _lastLayoutSize.IsEmpty
            ? Size.Empty
            : new Size(client.Width - _lastLayoutSize.Width, client.Height - _lastLayoutSize.Height);
        _lastLayoutSize = client;

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
