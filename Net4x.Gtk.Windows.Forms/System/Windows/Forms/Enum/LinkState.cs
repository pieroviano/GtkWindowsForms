// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms;

/// <summary>
/// Specifies constants that define the state of the link.
/// </summary>
public enum LinkState
{
    /// <summary>
    /// The normal
    /// </summary>
    Normal = 0x00,
    /// <summary>
    /// The hover
    /// </summary>
    Hover = 0x01,
    /// <summary>
    /// The active
    /// </summary>
    Active = 0x02,
    /// <summary>
    /// The visited
    /// </summary>
    Visited = 0x04
}