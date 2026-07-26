// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;

namespace System.Windows.Forms;

/// <summary>
/// The bounds-core, scaling and handle-lifecycle override surface. These are the protected methods a
/// custom control overrides to intercept sizing, DPI/font scaling and handle recreation; they were absent,
/// so <c>class MyControl : Control { protected override void SetBoundsCore(...) }</c> did not compile.
/// </summary>
public partial class Control
{
    /// <summary>
    /// Performs the work of setting the control's bounds. Every bounds change routes through here, so an
    /// override sees them all.
    /// </summary>
    /// <param name="x">The new <see cref="Left" />.</param>
    /// <param name="y">The new <see cref="Top" />.</param>
    /// <param name="width">The new width.</param>
    /// <param name="height">The new height.</param>
    /// <param name="specified">Which of the four components <paramref name="specified" /> actually applies to.</param>
    protected virtual void SetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified)
    {
        if (this.Widget == null)
        {
            return;
        }

        // Start from the current bounds and overwrite only the components the caller specified, so an
        // X-only or Size-only change leaves the rest intact.
        var current = Bounds;
        var rect = new Gdk.Rectangle(current.X, current.Y, current.Width, current.Height);

        if ((specified & BoundsSpecified.X) != 0)
        {
            rect.X = x;
        }

        if ((specified & BoundsSpecified.Y) != 0)
        {
            rect.Y = y;
        }

        if ((specified & BoundsSpecified.Width) != 0)
        {
            rect.Width = width;
        }

        if ((specified & BoundsSpecified.Height) != 0)
        {
            rect.Height = height;
        }

        this.Left = rect.X;
        this.Top = rect.Y;
        this.Size = new Size(rect.Width, rect.Height);
    }

    /// <summary>
    /// Performs the work of setting the client size. The shim does not model a non-client border, so the
    /// client size maps to the control size through <see cref="SizeFromClientSize(Size)" />.
    /// </summary>
    /// <param name="x">The client-area width.</param>
    /// <param name="y">The client-area height.</param>
    protected virtual void SetClientSizeCore(int x, int y)
    {
        var size = SizeFromClientSize(new Size(x, y));
        SetBoundsCore(Left, Top, size.Width, size.Height, BoundsSpecified.Size);
    }

    /// <summary>
    /// Returns the control size that yields the given client size. With no non-client border modelled the
    /// two are equal; a control that adds chrome overrides this.
    /// </summary>
    /// <param name="clientSize">The desired client-area size.</param>
    /// <returns>The control size.</returns>
    protected virtual Size SizeFromClientSize(Size clientSize) => clientSize;

    /// <summary>
    /// Returns <paramref name="bounds" /> scaled by <paramref name="factor" />, honouring which components
    /// <paramref name="specified" /> selects. Location and size scale independently, as in WinForms.
    /// </summary>
    /// <param name="bounds">The unscaled bounds.</param>
    /// <param name="factor">The horizontal and vertical scale factors.</param>
    /// <param name="specified">Which components to scale.</param>
    /// <returns>The scaled bounds.</returns>
    protected virtual Rectangle GetScaledBounds(Rectangle bounds, SizeF factor, BoundsSpecified specified)
    {
        var x = bounds.X;
        var y = bounds.Y;
        var width = bounds.Width;
        var height = bounds.Height;

        if ((specified & BoundsSpecified.X) != 0)
        {
            x = (int)Math.Round(bounds.X * factor.Width);
        }

        if ((specified & BoundsSpecified.Y) != 0)
        {
            y = (int)Math.Round(bounds.Y * factor.Height);
        }

        if ((specified & BoundsSpecified.Width) != 0)
        {
            width = (int)Math.Round(bounds.Width * factor.Width);
        }

        if ((specified & BoundsSpecified.Height) != 0)
        {
            height = (int)Math.Round(bounds.Height * factor.Height);
        }

        return new Rectangle(x, y, width, height);
    }

    /// <summary>Scales the control by the given factor for the specified bounds components.</summary>
    /// <param name="factor">The horizontal and vertical scale factors.</param>
    /// <param name="specified">Which components to scale.</param>
    protected virtual void ScaleControl(SizeF factor, BoundsSpecified specified)
    {
        var scaled = GetScaledBounds(Bounds, factor, specified);
        SetBoundsCore(scaled.X, scaled.Y, scaled.Width, scaled.Height, specified);
    }

    /// <summary>Scales the entire control by the given horizontal and vertical ratios (legacy entry point).</summary>
    /// <param name="dx">The horizontal scale ratio.</param>
    /// <param name="dy">The vertical scale ratio.</param>
    protected virtual void ScaleCore(float dx, float dy) =>
        ScaleControl(new SizeF(dx, dy), BoundsSpecified.All);

    /// <summary>
    /// Ensures the underlying GTK widget is realized (the shim's equivalent of a window handle). Safe to
    /// call at any time; a widget with no parent context simply stays unrealized.
    /// </summary>
    protected virtual void CreateHandle()
    {
        try
        {
            if (this.Widget is { IsRealized: false })
            {
                this.Widget.Realize();
            }
        }
        catch
        {
            // Realizing out of a container context can fail in GTK; the handle stays deferred, as it was.
        }
    }

    /// <summary>Unrealizes the underlying GTK widget, the shim's equivalent of destroying the window handle.</summary>
    protected virtual void DestroyHandle()
    {
        try
        {
            if (this.Widget is { IsRealized: true })
            {
                this.Widget.Unrealize();
            }
        }
        catch
        {
            // A widget that refuses to unrealize is left as-is rather than crashing the caller.
        }
    }

    /// <summary>Recreates the handle by destroying and re-creating it, as WinForms does after a style change.</summary>
    protected void RecreateHandle()
    {
        DestroyHandle();
        CreateHandle();
    }
}
