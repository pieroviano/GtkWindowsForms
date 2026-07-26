// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;

namespace System.Windows.Forms;

/// <summary>
/// Window-placement surface: desktop-relative bounds, the maximize/restore rectangles and right-to-left
/// layout. These were absent, so designer or application code touching <c>DesktopBounds</c>,
/// <c>RestoreBounds</c>, <c>MaximizedBounds</c> or <c>RightToLeftLayout</c> did not compile.
/// </summary>
public partial class Form
{
    private Rectangle? _restoreBounds;
    private Rectangle _maximizedBounds;
    private bool _rightToLeftLayout;

    /// <summary>Occurs when the value of the <see cref="MaximizedBounds" /> property changes.</summary>
    public event EventHandler? MaximizedBoundsChanged;

    /// <summary>Occurs when the value of the <see cref="RightToLeftLayout" /> property changes.</summary>
    public event EventHandler? RightToLeftLayoutChanged;

    /// <summary>
    /// Gets the bounds of the form in the normal window state, i.e. the bounds it restores to when a
    /// minimized or maximized form returns to <see cref="FormWindowState.Normal" />. While the form is
    /// already normal this is its current bounds.
    /// </summary>
    public Rectangle RestoreBounds => _restoreBounds ?? Bounds;

    /// <summary>Gets or sets the size and location of the form when it is maximized.</summary>
    public Rectangle MaximizedBounds
    {
        get => _maximizedBounds;
        set
        {
            if (_maximizedBounds == value)
            {
                return;
            }

            _maximizedBounds = value;
            OnMaximizedBoundsChanged(EventArgs.Empty);
        }
    }

    /// <summary>Gets or sets a value indicating whether right-to-left mirroring is applied to the form's layout.</summary>
    public virtual bool RightToLeftLayout
    {
        get => _rightToLeftLayout;
        set
        {
            if (_rightToLeftLayout == value)
            {
                return;
            }

            _rightToLeftLayout = value;
            OnRightToLeftLayoutChanged(EventArgs.Empty);
        }
    }

    /// <summary>
    /// Gets or sets the size and location of the form on the Windows desktop. On this layer desktop
    /// coordinates coincide with the form's <see cref="Control.Bounds" />.
    /// </summary>
    public Rectangle DesktopBounds
    {
        get => Bounds;
        set => SetBounds(value.X, value.Y, value.Width, value.Height, BoundsSpecified.All);
    }

    /// <summary>Gets or sets the location of the form on the Windows desktop.</summary>
    public Point DesktopLocation
    {
        get => Location;
        set => Location = value;
    }

    /// <summary>Sets the bounds of the form in desktop coordinates.</summary>
    /// <param name="x">The desktop x-coordinate.</param>
    /// <param name="y">The desktop y-coordinate.</param>
    /// <param name="width">The width.</param>
    /// <param name="height">The height.</param>
    public void SetDesktopBounds(int x, int y, int width, int height) =>
        SetBounds(x, y, width, height, BoundsSpecified.All);

    /// <summary>Sets the location of the form in desktop coordinates.</summary>
    /// <param name="x">The desktop x-coordinate.</param>
    /// <param name="y">The desktop y-coordinate.</param>
    public void SetDesktopLocation(int x, int y) => Location = new Point(x, y);

    /// <summary>Raises the <see cref="MaximizedBoundsChanged" /> event.</summary>
    /// <param name="e">The event data.</param>
    protected virtual void OnMaximizedBoundsChanged(EventArgs e) => MaximizedBoundsChanged?.Invoke(this, e);

    /// <summary>Raises the <see cref="RightToLeftLayoutChanged" /> event.</summary>
    /// <param name="e">The event data.</param>
    protected virtual void OnRightToLeftLayoutChanged(EventArgs e) => RightToLeftLayoutChanged?.Invoke(this, e);
}
