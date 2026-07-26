// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;

/// <summary>
/// The base class of a <see cref="DataGridViewRow" /> and (in WinForms) a <see cref="DataGridViewColumn" />
/// — a shared band of a <see cref="DataGridView" />. It declares the band state a row carries (index,
/// frozen/visible/read-only/resizable, selection, styles, header). It was absent, so <c>is DataGridViewBand</c>
/// did not compile. The concrete <see cref="DataGridViewRow" /> overrides these to keep its behaviour;
/// <see cref="DataGridViewColumn" /> is backed by a GTK <c>TreeViewColumn</c> and does not derive from this
/// on the shim.
/// </summary>
public abstract class DataGridViewBand : DataGridViewElement
{
    /// <summary>Gets or sets the shortcut menu for the band.</summary>
    public virtual ContextMenuStrip? ContextMenuStrip { get; set; }

    /// <summary>Gets or sets the default style of the band's cells.</summary>
    public virtual DataGridViewCellStyle? DefaultCellStyle { get; set; }

    /// <summary>Gets the type used to create new header cells.</summary>
    public Type DefaultHeaderCellType { get; set; } = typeof(DataGridViewHeaderCell);

    /// <summary>Gets a value indicating whether the band is currently displayed.</summary>
    public virtual bool Displayed { get; }

    /// <summary>Gets or sets a value indicating whether the band is frozen.</summary>
    public virtual bool Frozen { get; set; }

    /// <summary>Gets a value indicating whether a non-inherited default cell style has been set.</summary>
    public bool HasDefaultCellStyle => DefaultCellStyle != null;

    /// <summary>Gets the position of the band within the collection.</summary>
    public virtual int Index { get; }

    /// <summary>Gets the style applied to the band, cascaded from the grid.</summary>
    public virtual DataGridViewCellStyle? InheritedStyle { get; }

    /// <summary>Gets a value indicating whether the band is a row.</summary>
    public virtual bool IsRow { get; }

    /// <summary>Gets or sets a value indicating whether the band is read-only.</summary>
    public virtual bool ReadOnly { get; set; }

    /// <summary>Gets or sets a value indicating whether the band can be resized.</summary>
    public virtual DataGridViewTriState Resizable { get; set; }

    /// <summary>Gets or sets a value indicating whether the band is selected.</summary>
    public virtual bool Selected { get; set; }

    /// <summary>Gets or sets the object that contains data about the band.</summary>
    public object? Tag { get; set; }

    /// <summary>Gets or sets a value indicating whether the band is visible.</summary>
    public virtual bool Visible { get; set; } = true;

    /// <summary>Gets or sets the header cell of the band.</summary>
    protected DataGridViewHeaderCell? HeaderCellCore { get; set; }
}
