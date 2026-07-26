// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;

namespace System.Windows.Forms;

/// <summary>Provides data for the separator rendering event on a tool strip.</summary>
public class ToolStripSeparatorRenderEventArgs : ToolStripItemRenderEventArgs
{
    /// <summary>Initializes a new instance.</summary>
    /// <param name="g">The graphics used to paint.</param>
    /// <param name="separator">The separator to paint.</param>
    /// <param name="vertical">Whether the separator is drawn vertically.</param>
    public ToolStripSeparatorRenderEventArgs(Graphics g, ToolStripSeparator separator, bool vertical)
        : base(g, separator)
    {
        Vertical = vertical;
    }

    /// <summary>Gets a value indicating whether the separator is drawn vertically.</summary>
    public bool Vertical { get; }
}
