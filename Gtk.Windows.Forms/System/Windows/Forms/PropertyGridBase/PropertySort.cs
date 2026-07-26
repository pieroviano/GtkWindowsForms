// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;

/// <summary>
/// Specifies how properties are sorted in the
/// <see cref="T:System.Windows.Forms.PropertyGrid" />
/// </summary>
/// <filterpriority> 2
/// </filterpriority>
[Flags]
public enum PropertySort
{
    /// <summary>
    /// Properties will not be sorted, rather they will be displayed in the
    /// order that they are retrieved from the TypeDescriptor.
    /// </summary>
    NoSort = 0,

    /// <summary>
    /// Properties are sorted as a flat, alphabetical list.
    /// </summary>
    Alphabetical = 1,

    /// <summary>
    /// Properties are under category groups, which are defined by the
    /// properties themselves.
    /// </summary>
    Categorized = 2,

    /// <summary>
    /// Properties are under category groups, which are defined by the
    /// properties themselves, and are alphabetical within those groups.
    /// </summary>
    CategorizedAlphabetical = Alphabetical | Categorized,
}