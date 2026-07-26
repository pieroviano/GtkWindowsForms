// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms;

/// <summary>Defines the way a <see cref="T:System.Windows.Forms.TreeView" /> is drawn.</summary>
public enum TreeViewDrawMode
{
    /// <summary>The TreeView is drawn by the operating system.</summary>
    Normal = 0,

    /// <summary>The label portion of the tree nodes are drawn manually.</summary>
    OwnerDrawText = 1,

    /// <summary>All elements of a tree node are drawn manually.</summary>
    OwnerDrawAll = 2,
}
