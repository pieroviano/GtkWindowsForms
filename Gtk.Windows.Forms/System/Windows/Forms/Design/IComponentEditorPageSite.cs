// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms.Design;

/// <summary>Provides the site for a <c>ComponentEditorPage</c> hosted by a component editor.</summary>
public interface IComponentEditorPageSite
{
    /// <summary>Gets the control hosting the page.</summary>
    /// <returns>The host control.</returns>
    Control GetControl();

    /// <summary>Notifies the site that the page has been modified.</summary>
    void SetDirty();
}
