// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms;

/// <summary>Defines constants that represent areas in a <see cref="T:System.Windows.Forms.TreeView" /> or <see cref="T:System.Windows.Forms.TreeNode" />.</summary>
[Flags]
public enum TreeViewHitTestLocations
{
    /// <summary>A position in the client area of the TreeView control, but not on a node.</summary>
    None = 1,

    /// <summary>A position within the bounds of an image contained in a TreeView or TreeNode.</summary>
    Image = 2,

    /// <summary>A position within the bounds of a label contained in a TreeView or TreeNode.</summary>
    Label = 4,

    /// <summary>A position within the indentation for a TreeNode.</summary>
    Indent = 8,

    /// <summary>A position within the bounds of the plus or minus indicator of a TreeNode.</summary>
    PlusMinus = 16,

    /// <summary>A position to the right of the text area of a TreeNode.</summary>
    RightOfLabel = 32,

    /// <summary>A position within the bounds of a state image for a TreeNode.</summary>
    StateImage = 64,

    /// <summary>A position above the client portion of a TreeView control.</summary>
    AboveClientArea = 256,

    /// <summary>A position below the client portion of a TreeView control.</summary>
    BelowClientArea = 512,

    /// <summary>A position to the right of the client area of a TreeView control.</summary>
    RightOfClientArea = 1024,

    /// <summary>A position to the left of the client area of a TreeView control.</summary>
    LeftOfClientArea = 2048,
}
