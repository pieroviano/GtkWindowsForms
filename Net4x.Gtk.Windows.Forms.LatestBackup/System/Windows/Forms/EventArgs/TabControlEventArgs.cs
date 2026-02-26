// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    /// <summary>
    /// Provides data for the
    /// <see cref="!:System.Windows.Forms.TabControl.Selected" /> and
    /// <see cref="!:System.Windows.Forms.TabControl.Deselected" />
    /// events of a
    /// <see cref="System.Windows.Forms.TabControl" /> control.
    /// </summary>
    /// <filterpriority>
    /// 2
    /// </filterpriority>
    public class TabControlEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="System.Windows.Forms.TabControlEventArgs" /> class.
        /// </summary>
        /// <param name="tabPage">The
        /// <see cref="System.Windows.Forms.TabPage" /> the event is occurring for.
        /// </param>
        /// <param name="tabPageIndex">The zero-based index of
        /// <paramref name="tabPage" /> in the
        /// <see cref="System.Windows.Forms.TabControl.TabPages" /> collection.
        /// </param>
        /// <param name="action">One of the
        /// <see cref="System.Windows.Forms.TabControlAction" /> values.
        /// </param>
        public TabControlEventArgs(TabPage tabPage, int tabPageIndex, TabControlAction action)
        {
            TabPage = tabPage;
            TabPageIndex = tabPageIndex;
            Action = action;
        }

        /// <summary>
        /// Gets the
        /// <see cref="System.Windows.Forms.TabPage" /> the event is occurring for.
        /// </summary>
        /// <returns>The
        /// <see cref="System.Windows.Forms.TabPage" /> the event is occurring for.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public TabPage TabPage { get; }

        /// <summary>
        /// Gets the zero-based index of the
        /// <see cref="System.Windows.Forms.TabControlEventArgs.TabPage" /> in the
        /// <see cref="System.Windows.Forms.TabControl.TabPages" /> collection.
        /// </summary>
        /// <returns>The zero-based index of the
        /// <see cref="System.Windows.Forms.TabControlEventArgs.TabPage" /> in the
        /// <see cref="System.Windows.Forms.TabControl.TabPages" /> collection.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public int TabPageIndex { get; }

        /// <summary>
        /// Gets a value indicating which event is occurring.
        /// </summary>
        /// <returns>One of the
        /// <see cref="System.Windows.Forms.TabControlAction" /> values.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public TabControlAction Action { get; }
    }
}
