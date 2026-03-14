// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms;

/// <summary>
/// Provides data for the
/// <see cref="T:System.Windows.Forms.Form.FormClosed" /> event.
/// </summary>
public class FormClosedEventArgs : EventArgs
{
    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.FormClosedEventArgs" /> class.
    /// </summary>
    /// <param name="closeReason">
    /// The close reason.
    /// </param>
    public FormClosedEventArgs(CloseReason closeReason)
    {
        CloseReason = closeReason;
    }

    /// <summary>
    /// Provides the reason for the Form Close.
    /// </summary>
    /// <value>
    /// The close reason.
    /// </value>
    public CloseReason CloseReason { get; }

}