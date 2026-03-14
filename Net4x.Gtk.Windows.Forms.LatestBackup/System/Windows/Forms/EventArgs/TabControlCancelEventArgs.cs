// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;

namespace System.Windows.Forms;

/// <summary>
/// Provides data for the
/// <see cref="!:System.Windows.Forms.TabControl.Selecting" /> and
/// <see cref="!:System.Windows.Forms.TabControl.Deselecting" /> events of a
/// <see cref="T:System.Windows.Forms.TabControl" /> control.
/// </summary>
public class TabControlCancelEventArgs : CancelEventArgs
{
    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.TabControlCancelEventArgs" /> class.
    /// </summary>
    /// <param name="tabPage">The tab page.
    /// </param>
    /// <param name="tabPageIndex">Index of the tab page.
    /// </param>
    /// <param name="cancel">
    /// if set to
    /// <c>true
    /// </c> [cancel].
    /// </param>
    /// <param name="action">The action.
    /// </param>
    public TabControlCancelEventArgs(TabPage tabPage, int tabPageIndex, bool cancel, TabControlAction action) : base(cancel)
    {
        TabPage = tabPage;
        TabPageIndex = tabPageIndex;
        Action = action;
    }

    /// <summary>
    /// Stores the TabControl action which instigated the TabControl event.
    /// </summary>
    /// <value>The action.
    /// </value>
    public TabControlAction Action { get; }

    /// <summary>
    /// Stores the referemce to the tabpage that is undergoing the TabControl event.
    /// </summary>
    /// <value>The tab page.
    /// </value>
    public TabPage TabPage { get; }

    /// <summary>
    /// Stores the index to the tabpage that is undergoing the TabControl event.
    /// </summary>
    /// <value>The index of the tab page.
    /// </value>
    public int TabPageIndex { get; }

}