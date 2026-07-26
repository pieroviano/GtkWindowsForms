// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;

namespace System.Windows.Forms;

/// <summary>
/// Provides data for the
/// <see cref="T:System.Windows.Forms.Control.Layout" />
/// event. This class cannot be inherited.
/// </summary>
public sealed class LayoutEventArgs : EventArgs
{
    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.LayoutEventArgs" /> class.
    /// </summary>
    /// <param name="affectedComponent">
    /// The affected component.
    /// </param>
    /// <param name="affectedProperty">
    /// The affected property.
    /// </param>
    public LayoutEventArgs(IComponent affectedComponent, string affectedProperty)
    {
        AffectedComponent = affectedComponent;
        AffectedProperty = affectedProperty;
    }

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.LayoutEventArgs" /> class.
    /// </summary>
    /// <param name="affectedControl">
    /// The affected control.
    /// </param>
    /// <param name="affectedProperty">
    /// The affected property.
    /// </param>
    public LayoutEventArgs(Control affectedControl, string affectedProperty) : this((IComponent)affectedControl, affectedProperty)
    {
    }

    /// <summary>
    /// Gets the affected component.
    /// </summary>
    /// <value>
    /// The affected component.
    /// </value>
    public IComponent AffectedComponent { get; }

    /// <summary>
    /// Gets the affected control.
    /// </summary>
    /// <value>
    /// The affected control.
    /// </value>
    public Control AffectedControl => AffectedComponent as Control;

    /// <summary>
    /// Gets the affected property.
    /// </summary>
    /// <value>
    /// The affected property.
    /// </value>
    public string AffectedProperty { get; }
}