// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    /// <summary>
    /// The event class that is created when the selection state of a ListViewItem is changed.
    /// </summary>
    public class ListViewVirtualItemsSelectionRangeChangedEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="System.Windows.Forms.ListViewVirtualItemsSelectionRangeChangedEventArgs" /> class.
        /// </summary>
        /// <param name="startIndex">The index of the first item in the range that has changed.
        /// </param>
        /// <param name="endIndex">The index of the last item in the range that has changed.
        /// </param>
        /// <param name="isSelected">true to indicate the items are selected; false to indicate the items are deselected.
        /// </param>
        /// <exception cref="System.ArgumentException">
        /// <paramref name="startIndex" /> is larger than
        /// <paramref name="endIndex.." />
        /// </exception>
        public ListViewVirtualItemsSelectionRangeChangedEventArgs(int startIndex, int endIndex, bool isSelected)
        {
            if (startIndex > endIndex)
            {
                throw new ArgumentException("SR.ListViewStartIndexCannotBeLargerThanEndIndex");
            }

            StartIndex = startIndex;
            EndIndex = endIndex;
            IsSelected = isSelected;
        }

        /// <summary>
        /// Gets the index for the last item in the range of items whose selection state has changed
        /// </summary>
        /// <returns>The index of the last item in the range of items whose selection state has changed.
        /// </returns>
        public int EndIndex { get; }

        /// <summary>
        /// Gets a value indicating whether the range of items is selected.
        /// </summary>
        /// <returns>true if the range of items is selected; false if the range of items is deselected.
        /// </returns>
        public bool IsSelected { get; }

        /// <summary>
        /// Gets the index for the first item in the range of items whose selection state has changed.
        /// </summary>
        /// <returns>The index of the first item in the range of items whose selection state has changed.
        /// </returns>
        public int StartIndex { get; }
    }
}
