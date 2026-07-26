// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms;/// <summary>
/// Specifies the behaviors of a link in a <see cref="T:System.Windows.Forms.LinkLabel" />.
/// </summary>
public enum LinkBehavior
{
    /// <summary>The behavior of this setting depends on the options set using the Internet Options dialog box.</summary>
    SystemDefault = 0,

    /// <summary>The link is displayed with underlined text.</summary>
    AlwaysUnderline = 1,

    /// <summary>The link text is underlined only when the mouse pointer hovers over the link text.</summary>
    HoverUnderline = 2,

    /// <summary>The link text is never underlined.</summary>
    NeverUnderline = 3,
}
