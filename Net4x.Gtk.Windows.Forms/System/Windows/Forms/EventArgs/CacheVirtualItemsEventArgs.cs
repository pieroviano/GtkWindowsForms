// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms;

/// <summary>
/// Provides data for the
/// <see cref="!:System.Windows.Forms.ListView.CacheVirtualItems" /> event.
/// </summary>
/// <filterpriority>
/// 2
/// </filterpriority>
public class CacheVirtualItemsEventArgs : EventArgs
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CacheVirtualItemsEventArgs"/> class.
    /// </summary>
    /// <param name="startIndex">The start index.</param>
    /// <param name="endIndex">The end index.</param>
    public CacheVirtualItemsEventArgs(int startIndex, int endIndex)
    {
        StartIndex = startIndex;
        EndIndex = endIndex;
    }

    /// <summary>
    /// Gets the end index.
    /// </summary>
    /// <value>The end index.</value>
    public int EndIndex { get; }

    /// <summary>
    /// Gets the start index.
    /// </summary>
    /// <value>The start index.</value>
    public int StartIndex { get; }

}
