// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    /// <summary>
    /// The event class that is created when the selection state of a ListViewItem is changed.
    /// </summary>
    public class ListViewItemSelectionChangedEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="System.Windows.Forms.ListViewItemSelectionChangedEventArgs" /> class.
        /// </summary>
        /// <param name="item">The
        /// <see cref="System.Windows.Forms.ListViewItem" /> whose selection state has changed.
        /// </param>
        /// <param name="itemIndex">The index of the
        /// <see cref="System.Windows.Forms.ListViewItem" /> whose selection state has changed.
        /// </param>
        /// <param name="isSelected">true to indicate the item's state has changed to selected; false to indicate the item's state has changed to deselected.
        /// </param>
        public ListViewItemSelectionChangedEventArgs(ListViewItem item, int itemIndex, bool isSelected)
        {
            Item = item;
            ItemIndex = itemIndex;
            IsSelected = isSelected;
        }

        /// <summary>
        /// Gets the item whose selection state has changed.
        /// </summary>
        /// <returns>The
        /// <see cref="System.Windows.Forms.ListViewItem" /> whose selection state has changed.
        /// </returns>
        public ListViewItem Item { get; }

        /// <summary>
        /// Gets the index of the item whose selection state has changed.
        /// </summary>
        /// <returns>The index of the
        /// <see cref="System.Windows.Forms.ListViewItem" /> whose selection state has changed.
        /// </returns>
        public int ItemIndex { get; }

        /// <summary>
        /// Gets a value indicating whether the item's state has changed to selected.
        /// </summary>
        /// <returns>true if the item's state has changed to selected; false if the item's state has changed to deselected.
        /// </returns>
        public bool IsSelected { get; }
    }
}
