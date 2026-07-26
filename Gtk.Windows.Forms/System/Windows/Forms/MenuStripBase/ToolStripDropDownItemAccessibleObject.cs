// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;

/// <summary>
/// The accessibility object for a <see cref="ToolStripDropDownItem" /> (a menu item that owns a
/// drop-down). It was absent, so code referencing the type by name did not compile. WinForms derives it
/// from <c>ToolStripItem.ToolStripItemAccessibleObject</c>, which the shim does not project; here it
/// derives directly from <see cref="AccessibleObject" /> and exposes the owning item's accessible role and
/// text.
/// </summary>
public class ToolStripDropDownItemAccessibleObject : AccessibleObject
{
    private readonly ToolStripDropDownItem _ownerItem;

    /// <summary>Initializes a new instance for the given owning item.</summary>
    /// <param name="item">The drop-down item this accessible object represents.</param>
    public ToolStripDropDownItemAccessibleObject(ToolStripDropDownItem item)
    {
        _ownerItem = item ?? throw new ArgumentNullException(nameof(item));
    }

    /// <summary>Gets the accessible role — a menu item that opens a sub-menu.</summary>
    public override AccessibleRole Role => AccessibleRole.MenuItem;

    /// <summary>Gets the accessible name — the owning item's text.</summary>
    public override string? Name
    {
        get => _ownerItem.Text;
        set => base.Name = value;
    }
}
