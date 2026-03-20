// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms;

/// <summary>
/// Allows a custom control to prevent the
/// <see cref="T:System.Windows.Forms.Control.MouseWheel" />
/// event from being sent to its parent container.
/// </summary>
public class HandledMouseEventArgs : MouseEventArgs
{
    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.HandledMouseEventArgs" /> class.
    /// </summary>
    /// <param name="button">
    /// The button.
    /// </param>
    /// <param name="clicks">
    /// The clicks.
    /// </param>
    /// <param name="x">
    /// The x.
    /// </param>
    /// <param name="y">
    /// The y.
    /// </param>
    /// <param name="delta">
    /// The delta.
    /// </param>
    public HandledMouseEventArgs(MouseButtons button, int clicks, int x, int y, int delta) : this(button, clicks, x, y, delta, false)
    {
    }

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.HandledMouseEventArgs" /> class.
    /// </summary>
    /// <param name="button">
    /// The button.
    /// </param>
    /// <param name="clicks">
    /// The clicks.
    /// </param>
    /// <param name="x">
    /// The x.
    /// </param>
    /// <param name="y">
    /// The y.
    /// </param>
    /// <param name="delta">
    /// The delta.
    /// </param>
    /// <param name="defaultHandledValue">
    /// if set to
    /// <c>
    /// true
    /// </c>default handled value.
    /// </param>
    public HandledMouseEventArgs(MouseButtons button, int clicks, int x, int y, int delta, bool defaultHandledValue) : base(button, clicks, x, y, delta)
    {
        Handled = defaultHandledValue;
    }

    /// <summary>
    /// Gets or sets a value indicating whether the event is handled.
    /// </summary>
    /// <value>
    /// <c>
    /// true
    /// </c> if handled; otherwise,
    /// <c>
    /// false
    /// </c>.
    /// </value>
    public bool Handled
    {
        get;
        set;
    }

}