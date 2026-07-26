// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;

/// <summary>
/// Defines values for specifying whether and how content is copied to the Clipboard.
/// </summary>
public enum DataGridViewClipboardCopyMode
{
    /// <summary>Copying to the Clipboard is disabled.</summary>
    Disable = 0,
    /// <summary>Header text is included when copying if headers are shown.</summary>
    EnableWithAutoHeaderText = 1,
    /// <summary>Header text is not included when copying.</summary>
    EnableWithoutHeaderText = 2,
    /// <summary>Header text is always included when copying.</summary>
    EnableAlwaysIncludeHeaderText = 3
}
