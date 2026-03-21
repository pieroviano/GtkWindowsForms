// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms;

/// <summary>
/// Provides data for controls that derive from the
/// <see cref="!:System.Windows.Forms.UpDownBase" /> control.
/// </summary>
public class UpDownEventArgs : EventArgs
{
    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.UpDownEventArgs" /> class.
    /// </summary>
    /// <param name="buttonPushed">
    /// The button pushed.
    /// </param>
    public UpDownEventArgs(int buttonPushed)
    {
        ButtonID = buttonPushed;
    }

    /// <summary>
    /// Gets the button identifier.
    /// </summary>
    /// <value>
    /// The button identifier.
    /// </value>
    public int ButtonID { get; }
}