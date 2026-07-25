// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows.Forms.Layout;

namespace System.Windows.Forms;

/// <summary>Provides the base class for collecting layout settings for a container control.</summary>
public abstract class LayoutSettings
{
    /// <summary>Initializes a new instance of the <see cref="T:System.Windows.Forms.LayoutSettings" /> class.</summary>
    protected LayoutSettings() { }

    /// <summary>Gets the layout engine that applies these settings.</summary>
    public virtual LayoutEngine? LayoutEngine => null;
}
