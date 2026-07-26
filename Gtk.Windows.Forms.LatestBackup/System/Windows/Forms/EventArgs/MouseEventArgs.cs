// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;
using System.Runtime.InteropServices;

namespace System.Windows.Forms;

/// <summary>
/// Provides data for the
/// <see cref="T:System.Windows.Forms.Control.MouseUp" />,
/// <see cref="T:System.Windows.Forms.Control.MouseDown" />, and
/// <see cref="T:System.Windows.Forms.Control.MouseMove" /> events.
/// </summary>
/// <filterpriority>
/// 2
/// </filterpriority>
[ComVisible(true)]
public class MouseEventArgs : EventArgs
{
    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.MouseEventArgs" /> class.
    /// </summary>
    public MouseEventArgs(MouseButtons button, int clicks, int x, int y, int delta)
    {
        Button = button;
        Clicks = clicks;
        X = x;
        Y = y;
        Delta = delta;
    }

    /// <summary>
    /// Gets which mouse button was pressed.
    /// </summary>
    public MouseButtons Button { get; }

    /// <summary>
    /// Gets the number of times the mouse button was pressed and released.
    /// </summary>
    public int Clicks { get; }

    /// <summary>
    /// Gets a signed count of the number of detents the mouse wheel has rotated.
    /// </summary>
    public int Delta { get; }

    /// <summary>
    /// Gets the location of the mouse during MouseEvent.
    /// </summary>
    public Point Location => new(X, Y);

    /// <summary>
    /// Gets the x-coordinate of a mouse click.
    /// </summary>
    public int X { get; }

    /// <summary>
    /// Gets the y-coordinate of a mouse click.
    /// </summary>
    public int Y { get; }
}