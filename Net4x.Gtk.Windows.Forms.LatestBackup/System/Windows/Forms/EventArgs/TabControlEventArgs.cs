// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms;

/// <summary>
/// Provides data for the
/// <see cref="!:System.Windows.Forms.TabControl.Selected" /> and
/// <see cref="!:System.Windows.Forms.TabControl.Deselected" /> events of a
/// <see cref="T:System.Windows.Forms.TabControl" /> control.
/// </summary>
/// <filterpriority>
/// 2
/// </filterpriority>
public class TabControlEventArgs : EventArgs
{
    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.TabControlEventArgs" /> class.
    /// </summary>
    public TabControlEventArgs(TabPage tabPage, int tabPageIndex, TabControlAction action)
    {
        TabPage = tabPage;
        TabPageIndex = tabPageIndex;
        Action = action;
    }

    /// <summary>
    /// Stores the action which instigated the event.
    /// </summary>
    public TabControlAction Action { get; }

    /// <summary>
    /// Stores the referemce to the tabpage that is undergoing the event.
    /// </summary>
    public TabPage TabPage { get; }

    /// <summary>
    /// Stores the index to the tabpage that is undergoing the event.
    /// </summary>
    public int TabPageIndex { get; }

}