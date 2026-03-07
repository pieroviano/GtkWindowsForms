// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    /// <summary>
    /// Provides data for the
    /// <see cref="System.Windows.Forms.ListView.ItemChecked" />
    /// event of the
    /// <see cref="System.Windows.Forms.ListView" /> control.
    /// </summary>
    /// <filterpriority>
    /// 2
    /// </filterpriority>
    public class ItemCheckedEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="System.Windows.Forms.ItemCheckedEventArgs" /> class.
        /// </summary>
        /// <param name="item">The
        /// <see cref="System.Windows.Forms.ListViewItem" /> that is being checked or unchecked.
        /// </param>
        public ItemCheckedEventArgs(ListViewItem item)
        {
            Item = item;
        }

        /// <summary>
        /// Gets the
        /// <see cref="System.Windows.Forms.ListViewItem" /> whose checked state is changing.
        /// </summary>
        /// <returns>The
        /// <see cref="System.Windows.Forms.ListViewItem" /> whose checked state is changing.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public ListViewItem Item { get; }
    }
}
