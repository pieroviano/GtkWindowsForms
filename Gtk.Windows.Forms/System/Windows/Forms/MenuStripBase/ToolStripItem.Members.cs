// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;

/// <summary>
/// Common <see cref="ToolStripItem" /> members that ported source references directly — accessibility,
/// selection/placement state and drag flags. They were absent from the surface; because they live on the
/// base they reach every item type (buttons, menu items, labels, the hosted text/combo boxes). They compile
/// and round-trip; the GTK substrate drives the visual placement, so the placement-ish ones are stored
/// settings rather than live layout inputs.
/// </summary>
public partial class ToolStripItem
{
    private AccessibleObject? _accessibilityObject;

    /// <summary>Gets or sets a value indicating whether the item accepts data dropped onto it.</summary>
    public virtual bool AllowDrop { get; set; }

    /// <summary>Gets or sets the edges the item is anchored to.</summary>
    public AnchorStyles Anchor { get; set; } = AnchorStyles.Top | AnchorStyles.Left;

    /// <summary>Gets or sets a value indicating whether the item is available (shown when its strip is).</summary>
    public bool Available { get; set; } = true;

    /// <summary>Gets or sets which container edge the item docks to.</summary>
    public virtual DockStyle Dock { get; set; } = DockStyle.None;

    /// <summary>Gets or sets a value indicating whether the item raises double-click events.</summary>
    public bool DoubleClickEnabled { get; set; }

    /// <summary>Gets a value indicating whether the item can be selected.</summary>
    public virtual bool CanSelect => true;

    /// <summary>Gets a value indicating whether the item has been disposed.</summary>
    public bool IsDisposed { get; internal set; }

    /// <summary>Gets a value indicating whether the item is hosted on a drop-down.</summary>
    public bool IsOnDropDown => Owner is ToolStripDropDown;

    /// <summary>Gets a value indicating whether the item is currently on the overflow menu.</summary>
    public bool IsOnOverflow => false;

    /// <summary>Gets or sets the accessible name of the item.</summary>
    public string AccessibleName { get; set; } = string.Empty;

    /// <summary>Gets or sets the accessible description of the item.</summary>
    public string AccessibleDescription { get; set; } = string.Empty;

    /// <summary>Gets or sets the default-action description reported to accessibility clients.</summary>
    public string AccessibleDefaultActionDescription { get; set; } = string.Empty;

    /// <summary>Gets or sets the accessible role of the item.</summary>
    public AccessibleRole AccessibleRole { get; set; } = AccessibleRole.Default;

    /// <summary>Gets the accessibility object for the item.</summary>
    public AccessibleObject AccessibilityObject => _accessibilityObject ??= new AccessibleObject();

    /// <summary>Gets a value indicating whether an owning drop-down closes when this item is clicked.</summary>
    protected internal virtual bool DismissWhenClicked => true;

    /// <summary>Gets a value indicating whether keyboard cues (mnemonic underlines) are shown.</summary>
    protected internal virtual bool ShowKeyboardCues => false;
}
