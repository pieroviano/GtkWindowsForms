// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms;

/// <summary>Specifies a standard interface for retrieving feature information from the current system.</summary>
public interface IFeatureSupport
{
    /// <summary>Determines whether the specified feature is installed.</summary>
    /// <param name="feature">The feature to look for.</param>
    /// <returns><see langword="true" /> if the feature is present; otherwise <see langword="false" />.</returns>
    bool IsPresent(object feature);

    /// <summary>Determines whether the specified feature is installed at or above a version.</summary>
    /// <param name="feature">The feature to look for.</param>
    /// <param name="minimumVersion">The minimum version.</param>
    /// <returns><see langword="true" /> if present at that version; otherwise <see langword="false" />.</returns>
    bool IsPresent(object feature, Version minimumVersion);

    /// <summary>Gets the version of the specified feature that is present.</summary>
    /// <param name="feature">The feature to look for.</param>
    /// <returns>The version present, or <see langword="null" />.</returns>
    Version? GetVersionPresent(object feature);
}
