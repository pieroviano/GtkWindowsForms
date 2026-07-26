// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms;

/// <summary>Handles the painting functionality for <see cref="T:System.Windows.Forms.ToolStrip" /> objects, using a professional (Office-style) color table. Chrome is rendered by the GTK theme; the color table is exposed for source that reads it.</summary>
public class ToolStripProfessionalRenderer : ToolStripRenderer
{
    /// <summary>Initializes a new instance with a default color table.</summary>
    public ToolStripProfessionalRenderer() : this(new ProfessionalColorTable())
    {
    }

    /// <summary>Initializes a new instance with the specified color table.</summary>
    /// <param name="professionalColorTable">The color table to use.</param>
    public ToolStripProfessionalRenderer(ProfessionalColorTable professionalColorTable)
    {
        ColorTable = professionalColorTable;
    }

    /// <summary>Gets the color table used for painting.</summary>
    public ProfessionalColorTable ColorTable { get; }
}
