// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;

namespace System.Windows.Forms;

/// <summary>Holds ownership drawing properties (font and colors) for a list-based control item.</summary>
public class OwnerDrawPropertyBag : MarshalByRefObject
{
    internal OwnerDrawPropertyBag() { }

    /// <summary>Gets or sets the font.</summary>
    public Font? Font { get; set; }

    /// <summary>Gets or sets the foreground color.</summary>
    public Color ForeColor { get; set; } = Color.Empty;

    /// <summary>Gets or sets the background color.</summary>
    public Color BackColor { get; set; } = Color.Empty;

    /// <summary>Indicates whether the property bag holds no non-default values.</summary>
    /// <returns><see langword="true" /> if empty; otherwise <see langword="false" />.</returns>
    public bool IsEmpty() => Font == null && ForeColor.IsEmpty && BackColor.IsEmpty;

    /// <summary>Returns a copy of the specified property bag.</summary>
    /// <param name="value">The property bag to copy.</param>
    /// <returns>A new <see cref="T:System.Windows.Forms.OwnerDrawPropertyBag" />.</returns>
    public static OwnerDrawPropertyBag Copy(OwnerDrawPropertyBag value)
    {
        var bag = new OwnerDrawPropertyBag();
        if (value != null)
        {
            bag.Font = value.Font;
            bag.ForeColor = value.ForeColor;
            bag.BackColor = value.BackColor;
        }

        return bag;
    }
}
