// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms;/// <summary>
/// Specifies whether a control, when added to a container, should extend its edges to fill the empty space of the container.
/// </summary>
public enum DockingBehavior
{
    /// <summary>The control does not automatically dock when added to a container.</summary>
    Never = 0,

    /// <summary>The control's docking behavior is determined by the developer.</summary>
    Ask = 1,

    /// <summary>The control docks to the parent container's edges automatically.</summary>
    AutoDock = 2,
}
