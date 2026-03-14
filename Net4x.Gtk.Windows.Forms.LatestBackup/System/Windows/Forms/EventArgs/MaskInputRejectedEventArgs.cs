// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;

namespace System.Windows.Forms;

/// <summary>
/// Provides data for the
/// <see cref="!:System.Windows.Forms.MaskedTextBox.MaskInputRejected" /> event.
/// </summary>
public class MaskInputRejectedEventArgs : EventArgs
{
    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.MaskInputRejectedEventArgs" /> class.
    /// </summary>
    /// <param name="position">The position.
    /// </param>
    /// <param name="rejectionHint">The rejection hint.
    /// </param>
    public MaskInputRejectedEventArgs(int position, MaskedTextResultHint rejectionHint)
    {
        Position = position;
        RejectionHint = rejectionHint;
    }

    /// <summary>
    /// The position where the test failed the mask constraint.
    /// </summary>
    /// <value>The position.
    /// </value>
    public int Position { get; }

    /// <summary>
    /// Retreives a hint on why the input is rejected.
    /// </summary>
    /// <value>The rejection hint.
    /// </value>
    public MaskedTextResultHint RejectionHint { get; }

}