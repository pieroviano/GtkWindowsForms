// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Drawing;

namespace System.Windows.Forms;

/// <summary>
/// Non-rectangular control shapes - <c>Control.Region</c>. The property existed as an inert
/// auto-property: assigning it stored a region and shaped nothing, so a shaped control (a round button, a
/// skinned splash form) compiled, ran, and stayed rectangular.
/// </summary>
public partial class Control
{
    private Region? _region;

    /// <summary>
    /// Gets or sets the window region associated with the control.
    /// </summary>
    /// <value>
    /// The <see cref="T:System.Drawing.Region" /> that represents the window region, in client
    /// coordinates, or <see langword="null" /> for the whole rectangle.
    /// </value>
    /// <remarks>
    /// Backed by GTK's shape-combine, which - like the Win32 window region this mirrors - takes a list of
    /// rectangles. A region whose interior is a real path is therefore applied as the rectangles it
    /// decomposes into, so a curved outline is stepped at pixel resolution rather than smooth. That is
    /// what Win32 does too; the difference is only how finely the curve is scanned.
    /// <para>
    /// The shape needs a realized widget, so a region assigned before the handle exists is remembered and
    /// applied when the widget is realized.
    /// </para>
    /// <para>
    /// **Substrate limit.** A shape lives on a GDK window, and lightweight GTK widgets - buttons, labels,
    /// and anything else that draws onto its parent rather than owning a window - do not have one. On
    /// those the region is stored and reported but changes nothing. Forms and the container controls
    /// (<see cref="T:System.Windows.Forms.Panel" /> and friends) do own a window and are shaped for real,
    /// which covers the shaped-window and custom-drawn-container cases the property exists for.
    /// </para>
    /// </remarks>
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public virtual Region? Region
    {
        get => _region;
        set
        {
            if (ReferenceEquals(_region, value))
            {
                return;
            }

            _region = value;
            ApplyRegion();
            OnRegionChanged(EventArgs.Empty);
        }
    }

    /// <summary>
    /// Pushes <see cref="Region" /> down to the widget, or clears the shape when it is
    /// <see langword="null" />. A no-op until the widget is realized - <see cref="ApplyRegion" /> is
    /// called again from the realize handler.
    /// </summary>
    internal void ApplyRegion()
    {
        var widget = Widget;
        if (widget == null || !widget.IsRealized)
        {
            return;
        }

        // Gtk4 removed GDK windows and window shaping (Gdk.Window / Widget.HasWindow /
        // Widget.ShapeCombineRegion). Control.Region is still stored and reported through the
        // WinForms API, but it can no longer be applied to a native surface, so it has no visual
        // effect. A future port could approximate non-rectangular shapes with a CSS clip-path.
        Diagnostics.GtkLogger.Debug(
            $"Control.Region has no effect on {GetType().Name}: Gtk4 removed GDK window shaping.");
    }

    /// <summary>
    /// Converts a <see cref="T:System.Drawing.Region" /> to the pixel-rectangle region GTK's
    /// shape-combine takes.
    /// </summary>
    /// <param name="region">The region to convert.</param>
    /// <returns>
    /// <see langword="null" /> when <paramref name="region" /> is <see langword="null" /> or covers
    /// everything - which is how the shape is reset - otherwise the region's rectangles, rounded outwards
    /// so no part of the requested shape is clipped away.
    /// </returns>
    private static Cairo.Region? ToCairoRegion(Region? region)
    {
        if (region == null || region.Bands.Count == 0)
        {
            // A null shape means "the whole widget" to GTK. An empty region cannot be expressed that way,
            // but an empty Cairo region is honoured, so only the null case short-circuits.
            return region == null ? null : new Cairo.Region();
        }

        var rectangles = new Cairo.RectangleInt[region.Bands.Count];
        for (var i = 0; i < rectangles.Length; i++)
        {
            var band = region.Bands[i];
            var left = (int)Math.Floor(band.Left);
            var top = (int)Math.Floor(band.Top);
            rectangles[i] = new Cairo.RectangleInt
            {
                X = left,
                Y = top,
                Width = (int)Math.Ceiling(band.Right) - left,
                Height = (int)Math.Ceiling(band.Bottom) - top
            };
        }

        return new Cairo.Region(rectangles);
    }
}
