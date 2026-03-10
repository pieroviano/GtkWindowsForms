// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms;

/// <summary>
/// Provides data for the
/// <see cref="T:System.Windows.Forms.CurrencyManager.ItemChanged" /> event.
/// </summary>
public class ItemChangedEventArgs : EventArgs
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ItemChangedEventArgs"/> class.
    /// </summary>
    /// <param name="index">The index.</param>
    internal ItemChangedEventArgs(int index)
    {
        Index = index;
    }

    /// <summary>
    /// Gets the index.
    /// </summary>
    /// <value>The index.</value>
    public int Index { get; }

}
