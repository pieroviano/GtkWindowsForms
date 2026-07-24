// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms;

/// <summary>Specifies which paint styles are applied to <see cref="T:System.Windows.Forms.ToolStrip" /> elements.</summary>
public enum ToolStripManagerRenderMode
{
    /// <summary>Custom rendering is applied.</summary>
    Custom = 0,

    /// <summary>The operating system renders the elements.</summary>
    System = 1,

    /// <summary>The elements are rendered with a professional (Office-style) appearance.</summary>
    Professional = 2,
}
