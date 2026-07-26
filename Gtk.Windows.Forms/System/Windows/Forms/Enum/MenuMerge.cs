// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms;/// <summary>
/// Specifies the behavior of a <see cref="T:System.Windows.Forms.ToolStripMenuItem" /> when it is merged with another menu.
/// </summary>
public enum MenuMerge
{
    /// <summary>Adds the item to the collection of existing items.</summary>
    Add = 0,

    /// <summary>Replaces the existing item with this item, at the same position.</summary>
    Replace = 1,

    /// <summary>Merges the subitems of this item with those of existing items at the corresponding position.</summary>
    MergeItems = 2,

    /// <summary>Removes the item from the collection.</summary>
    Remove = 3,
}
