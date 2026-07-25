// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;

/// <summary>
/// The drop-down that holds the items a <see cref="ToolStrip" /> cannot fit and shows behind the overflow
/// chevron. It derives from <see cref="ToolStripDropDown" /> (now a <see cref="Control" />), so it carries
/// the full control surface. Absent before, so a strip that overflows could not expose it by name.
/// </summary>
public class ToolStripOverflow : ToolStripDropDown
{
    private readonly ToolStripItem? _ownerItem;

    /// <summary>Initializes a new instance owned by the given parent item (the overflow button).</summary>
    /// <param name="parentItem">The item that owns this overflow drop-down.</param>
    public ToolStripOverflow(ToolStripItem parentItem) : base()
    {
        _ownerItem = parentItem;
    }

    /// <summary>Gets the items currently shown in the overflow.</summary>
    public ToolStripItemCollection DisplayedItems => Items;

    /// <summary>Recomputes which items are displayed. The overflow mirrors <see cref="Items" /> on this layer.</summary>
    protected void SetDisplayedItems()
    {
    }
}
