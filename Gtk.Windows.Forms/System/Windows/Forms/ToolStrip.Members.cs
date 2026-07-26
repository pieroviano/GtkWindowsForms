// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;

/// <summary>
/// Common <see cref="ToolStrip" /> configuration members that ported source references directly. They were
/// absent from the surface; the layout/rendering they name is driven by GTK on this substrate, so most are
/// stored settings that the shim's renderer does not fully honour, but they compile and round-trip.
/// </summary>
public partial class ToolStrip
{
    private ToolStripRenderer? _renderer;
    private ToolStripOverflowButton? _overflowButton;

    /// <summary>Gets or sets a value indicating whether drag-and-drop reordering of items is allowed.</summary>
    public bool AllowItemReorder { get; set; }

    /// <summary>Gets or sets a value indicating whether this strip participates in toolbar merging.</summary>
    public bool AllowMerge { get; set; } = true;

    /// <summary>Gets or sets a value indicating whether items that do not fit move to an overflow menu.</summary>
    public bool CanOverflow { get; set; } = true;

    /// <summary>Gets or sets a value indicating whether item tooltips are shown.</summary>
    public bool ShowItemToolTips { get; set; } = true;

    /// <summary>Gets or sets a value indicating whether the strip stretches to fill its container.</summary>
    public bool Stretch { get; set; }

    /// <summary>Gets a value indicating whether this strip is a drop-down. Overridden by <see cref="ToolStripDropDown" />.</summary>
    public virtual bool IsDropDown => false;

    /// <summary>Gets the orientation in which the strip lays out its items.</summary>
    public Orientation Orientation { get; } = Orientation.Horizontal;

    /// <summary>Gets or sets the direction in which item text is drawn.</summary>
    public ToolStripTextDirection TextDirection { get; set; } = ToolStripTextDirection.Horizontal;

    /// <summary>Gets or sets the default direction a drop-down opens relative to its owner.</summary>
    public ToolStripDropDownDirection DefaultDropDownDirection { get; set; } = ToolStripDropDownDirection.BelowRight;

    /// <summary>Gets or sets the image list from which items draw their images by index.</summary>
    public ImageList? ImageList { get; set; }

    /// <summary>Gets or sets the renderer used to paint the strip and its items.</summary>
    public ToolStripRenderer Renderer
    {
        get => _renderer ??= new ToolStripProfessionalRenderer();
        set => _renderer = value;
    }

    /// <summary>Gets the overflow button that opens the items that do not fit.</summary>
    public ToolStripOverflowButton OverflowButton => _overflowButton ??= new ToolStripOverflowButton();
}
