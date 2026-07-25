// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;

/// <summary>
/// Defines values for specifying how the width of the row headers is adjusted.
/// </summary>
public enum DataGridViewRowHeadersWidthSizeMode
{
    /// <summary>The row headers can be resized by the user and do not automatically size.</summary>
    EnableResizing = 0,
    /// <summary>The row headers cannot be resized by the user and do not automatically size.</summary>
    DisableResizing = 1,
    /// <summary>The row headers size to fit the content of all row headers.</summary>
    AutoSizeToAllHeaders = 2,
    /// <summary>The row headers size to fit the content of the displayed row headers.</summary>
    AutoSizeToDisplayedHeaders = 3,
    /// <summary>The row headers size to fit the content of the first row header.</summary>
    AutoSizeToFirstHeader = 4
}
