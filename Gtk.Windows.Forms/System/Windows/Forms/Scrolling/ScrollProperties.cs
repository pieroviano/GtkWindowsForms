// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms;

/// <summary>Encapsulates properties related to scrolling for controls with scrollable regions.</summary>
public abstract class ScrollProperties
{
    /// <summary>Initializes a new instance of the <see cref="T:System.Windows.Forms.ScrollProperties" /> class.</summary>
    /// <param name="container">The <see cref="T:System.Windows.Forms.ScrollableControl" /> whose scrolling properties this object describes.</param>
    protected ScrollProperties(ScrollableControl? container)
    {
        ParentControl = container;
    }

    /// <summary>Gets the <see cref="T:System.Windows.Forms.ScrollableControl" /> that owns these scroll properties.</summary>
    protected ScrollableControl? ParentControl { get; }

    /// <summary>Gets or sets a value indicating whether the scroll bar can be used.</summary>
    public bool Enabled { get; set; } = true;

    /// <summary>Gets or sets the distance to move a scroll bar in response to a large scroll command.</summary>
    public int LargeChange { get; set; } = 10;

    /// <summary>Gets or sets the upper limit of the scrollable range.</summary>
    public int Maximum { get; set; } = 100;

    /// <summary>Gets or sets the lower limit of the scrollable range.</summary>
    public int Minimum { get; set; }

    /// <summary>Gets or sets the distance to move a scroll bar in response to a small scroll command.</summary>
    public int SmallChange { get; set; } = 1;

    /// <summary>Gets or sets a numeric value that represents the current position of the scroll box.</summary>
    public int Value { get; set; }

    /// <summary>Gets or sets a value indicating whether the scroll bar is visible.</summary>
    public bool Visible { get; set; }
}
