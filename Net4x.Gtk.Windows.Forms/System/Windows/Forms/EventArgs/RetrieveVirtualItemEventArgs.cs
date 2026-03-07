// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    /// <summary>
    /// Provides data for the
    /// <see cref="!:System.Windows.Forms.ListView.RetrieveVirtualItem" /> event.
    /// </summary>
    /// <filterpriority>
    /// 2
    /// </filterpriority>
    public class RetrieveVirtualItemEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="System.Windows.Forms.RetrieveVirtualItemEventArgs" /> class.
        /// </summary>
        /// <param name="itemIndex">The index of the item to retrieve.
        /// </param>
        public RetrieveVirtualItemEventArgs(int itemIndex)
        {
            ItemIndex = itemIndex;
        }
        /// <summary>
        /// Gets or sets the item retrieved from the cache.
        /// </summary>
        /// <returns>The
        /// <see cref="System.Windows.Forms.ListViewItem" /> retrieved from the cache.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public ListViewItem Item { get; set; }

        /// <summary>
        /// Gets the index of the item to retrieve from the cache.
        /// </summary>
        /// <returns>The index of the item to retrieve from the cache.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public int ItemIndex { get; }
    }
}
