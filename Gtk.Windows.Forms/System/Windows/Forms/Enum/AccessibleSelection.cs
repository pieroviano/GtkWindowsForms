// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms;

/// <summary>Specifies AccessibleSelection.</summary>
[Flags]
public enum AccessibleSelection
{
    /// <summary>The None value.</summary>
    None = 0,

    /// <summary>The TakeFocus value.</summary>
    TakeFocus = 1,

    /// <summary>The TakeSelection value.</summary>
    TakeSelection = 2,

    /// <summary>The ExtendSelection value.</summary>
    ExtendSelection = 4,

    /// <summary>The AddSelection value.</summary>
    AddSelection = 8,

    /// <summary>The RemoveSelection value.</summary>
    RemoveSelection = 16,
}
