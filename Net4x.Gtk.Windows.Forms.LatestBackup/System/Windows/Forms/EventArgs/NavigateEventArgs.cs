// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;

namespace System.Windows.Forms;

/// <summary>
/// Provides data for the
/// <see cref="!:System.Windows.Forms.DataGrid.Navigate" /> event.
/// </summary>
[ComVisible(true)]
public class NavigateEventArgs : EventArgs
{
    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.NavigateEventArgs" /> class.
    /// </summary>
    /// <param name="isForward">
    /// if set to
    /// <c>
    /// true
    /// </c>is forward.
    /// </param>
    public NavigateEventArgs(bool isForward)
    {
        Forward = isForward;
    }

    /// <summary>
    /// Gets a value indicating whether this
    /// <see cref="T:System.Windows.Forms.NavigateEventArgs" /> is forward.
    /// </summary>
    /// <value>
    /// <c>
    /// true
    /// </c> if forward; otherwise,
    /// <c>
    /// false
    /// </c>.
    /// </value>
    public bool Forward { get; }

}