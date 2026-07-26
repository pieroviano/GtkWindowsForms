// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;

/// <summary>
/// The chevron button a <see cref="ToolStrip" /> shows when it has more items than fit; clicking it opens
/// the <see cref="ToolStripOverflow" />. Absent before, so the overflow chevron could not be named. Its
/// surface comes from <see cref="ToolStripDropDownButton" />.
/// </summary>
public class ToolStripOverflowButton : ToolStripDropDownButton
{
    /// <summary>Initializes a new instance of the <see cref="ToolStripOverflowButton" /> class.</summary>
    public ToolStripOverflowButton() : base()
    {
    }
}
