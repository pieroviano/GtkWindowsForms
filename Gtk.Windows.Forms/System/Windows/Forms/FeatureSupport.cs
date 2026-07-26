// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms;

/// <summary>Provides a base class for feature support detection.</summary>
public abstract class FeatureSupport : IFeatureSupport
{
    /// <summary>Determines, for a static feature class, whether the feature is present.</summary>
    /// <param name="featureClassName">The fully qualified type name of the feature support class.</param>
    /// <param name="featureConstName">The name of the feature constant.</param>
    /// <returns><see langword="true" /> if present; otherwise <see langword="false" />.</returns>
    public static bool IsPresent(string featureClassName, string featureConstName) => false;

    /// <summary>Determines, for a static feature class, whether the feature is present at a minimum version.</summary>
    /// <param name="featureClassName">The fully qualified type name of the feature support class.</param>
    /// <param name="featureConstName">The name of the feature constant.</param>
    /// <param name="minimumVersion">The minimum version.</param>
    /// <returns><see langword="true" /> if present at that version; otherwise <see langword="false" />.</returns>
    public static bool IsPresent(string featureClassName, string featureConstName, Version minimumVersion) => false;

    /// <summary>Gets the version present for a static feature class.</summary>
    /// <param name="featureClassName">The fully qualified type name of the feature support class.</param>
    /// <param name="featureConstName">The name of the feature constant.</param>
    /// <returns>The version present, or <see langword="null" />.</returns>
    public static Version? GetVersionPresent(string featureClassName, string featureConstName) => null;

    /// <summary>Determines whether the specified feature is present.</summary>
    /// <param name="feature">The feature to look for.</param>
    /// <returns><see langword="true" /> if present; otherwise <see langword="false" />.</returns>
    public virtual bool IsPresent(object feature) => GetVersionPresent(feature) != null;

    /// <summary>Determines whether the specified feature is present at a minimum version.</summary>
    /// <param name="feature">The feature to look for.</param>
    /// <param name="minimumVersion">The minimum version.</param>
    /// <returns><see langword="true" /> if present at that version; otherwise <see langword="false" />.</returns>
    public virtual bool IsPresent(object feature, Version minimumVersion)
    {
        var v = GetVersionPresent(feature);
        return v != null && v >= minimumVersion;
    }

    /// <summary>When overridden in a derived class, gets the version of the specified feature that is present.</summary>
    /// <param name="feature">The feature to look for.</param>
    /// <returns>The version present, or <see langword="null" />.</returns>
    public abstract Version? GetVersionPresent(object feature);
}
