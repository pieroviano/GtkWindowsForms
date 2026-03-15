// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms;

internal class DataGridViewCellStyleChangedEventArgs : EventArgs
{
    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.DataGridView" /> class.
    /// Configures the underlying GTK TreeView and prepares columns/rows collections.
    /// </summary>
    public DataGridViewCellStyleChangedEventArgs()
    {
    }

    public bool ChangeAffectsPreferredSize { get; set; }

}