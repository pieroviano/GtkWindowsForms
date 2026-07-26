// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms;

/// <summary>Handles the painting functionality for <see cref="T:System.Windows.Forms.ToolStrip" /> objects, using the operating-system theme. On this GTK layer the theme draws the chrome directly.</summary>
public class ToolStripSystemRenderer : ToolStripRenderer
{
    /// <summary>Initializes a new instance of the <see cref="T:System.Windows.Forms.ToolStripSystemRenderer" /> class.</summary>
    public ToolStripSystemRenderer()
    {
    }
}
