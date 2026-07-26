// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms;

/// <summary>Specifies the default docking behavior for a control.</summary>
[AttributeUsage(AttributeTargets.Class)]
public sealed class DockingAttribute : Attribute
{
    /// <summary>Initializes a new instance with the default docking behavior.</summary>
    public DockingAttribute() { DockingBehavior = DockingBehavior.Never; }

    /// <summary>Initializes a new instance with the specified docking behavior.</summary>
    /// <param name="dockingBehavior">The docking behavior.</param>
    public DockingAttribute(DockingBehavior dockingBehavior) { DockingBehavior = dockingBehavior; }

    /// <summary>Gets the docking behavior specified by this attribute.</summary>
    public DockingBehavior DockingBehavior { get; }

    /// <summary>Determines whether the specified object equals this attribute.</summary>
    /// <param name="obj">The object to compare.</param>
    /// <returns><see langword="true" /> if equal; otherwise <see langword="false" />.</returns>
    public override bool Equals(object? obj) => obj is DockingAttribute a && a.DockingBehavior == DockingBehavior;

    /// <summary>Returns the hash code for this attribute.</summary>
    /// <returns>A hash code.</returns>
    public override int GetHashCode() => (int)DockingBehavior;

    /// <summary>Indicates whether this attribute holds the default value.</summary>
    /// <returns><see langword="true" /> if default; otherwise <see langword="false" />.</returns>
    public override bool IsDefaultAttribute() => DockingBehavior == DockingBehavior.Never;
}
