// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;

namespace System.Windows.Forms
{
    /// <summary>
    /// Provides data for the
    /// <see cref="!:System.Windows.Forms.ListView.ItemMouseHover" /> event.
    /// </summary>
    /// <filterpriority>
    /// 2
    /// </filterpriority>
    [ComVisible(true)]
    public class ListViewItemMouseHoverEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="System.Windows.Forms.ListViewItemMouseHoverEventArgs" /> class.
        /// </summary>
        /// <param name="item">The
        /// <see cref="System.Windows.Forms.ListViewItem" /> the mouse pointer is currently hovering over.
        /// </param>
        public ListViewItemMouseHoverEventArgs(ListViewItem item)
        {
            Item = item;
        }
        /// <summary>
        /// Gets the item the mouse pointer is currently hovering over.
        /// </summary>
        /// <returns>The
        /// <see cref="System.Windows.Forms.ListViewItem" /> that the mouse pointer is currently hovering over.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public ListViewItem Item { get; }
    }
}
