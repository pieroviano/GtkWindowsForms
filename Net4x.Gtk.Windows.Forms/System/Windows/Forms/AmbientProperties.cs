// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;

namespace System.Windows.Forms;

/// <summary>Provides ambient property values to top-level controls.</summary>
public sealed class AmbientProperties
{
    /// <summary>Initializes a new instance of the <see cref="T:System.Windows.Forms.AmbientProperties" /> class.</summary>
    public AmbientProperties() { }

    /// <summary>Gets or sets the ambient background color.</summary>
    public Color BackColor { get; set; } = Color.Empty;

    /// <summary>Gets or sets the ambient cursor.</summary>
    public Cursor? Cursor { get; set; }

    /// <summary>Gets or sets the ambient font.</summary>
    public Font? Font { get; set; }

    /// <summary>Gets or sets the ambient foreground color.</summary>
    public Color ForeColor { get; set; } = Color.Empty;
}
