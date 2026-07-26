// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms;

/// <summary>Provides information about operating system features. On the GTK layer no Windows features are
/// reported present.</summary>
public class OSFeature : FeatureSupport
{
    private static OSFeature? feature;

    /// <summary>Represents the layered, top-level windows feature. Value is a well-known object token.</summary>
    public static readonly object LayeredWindows = new object();

    /// <summary>Represents the theming feature.</summary>
    public static readonly object Themes = new object();

    /// <summary>Initializes a new instance of the <see cref="T:System.Windows.Forms.OSFeature" /> class.</summary>
    protected OSFeature() { }

    /// <summary>Gets the static <see cref="T:System.Windows.Forms.OSFeature" /> instance.</summary>
    public static OSFeature Feature => feature ??= new OSFeature();

    /// <summary>Gets the version of the specified feature that is present. Always <see langword="null" /> on GTK.</summary>
    /// <param name="feature">The feature to look for.</param>
    /// <returns><see langword="null" />.</returns>
    public override Version? GetVersionPresent(object feature) => null;

    /// <summary>Retrieves whether the specified system parameter is present. Always <see langword="false" /> on GTK.</summary>
    /// <param name="enumVal">The system parameter to look for.</param>
    /// <returns><see langword="false" />.</returns>
    public bool IsPresent(SystemParameter enumVal) => false;
}
