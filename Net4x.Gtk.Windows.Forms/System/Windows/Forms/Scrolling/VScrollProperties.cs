// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms;

/// <summary>Provides properties that control the vertical scrolling behavior of a <see cref="T:System.Windows.Forms.ScrollableControl" />.</summary>
public class VScrollProperties : ScrollProperties
{
    /// <summary>Initializes a new instance of the <see cref="T:System.Windows.Forms.VScrollProperties" /> class.</summary>
    /// <param name="container">The <see cref="T:System.Windows.Forms.ScrollableControl" /> that owns these properties.</param>
    public VScrollProperties(ScrollableControl? container) : base(container)
    {
    }
}
