// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    /// <summary>
    /// Provides data for the
    /// <see cref="!:System.Windows.Forms.ListView.CacheVirtualItems" /> event.
    /// </summary>
    /// <filterpriority>
    /// 2
    /// </filterpriority>
    public class CacheVirtualItemsEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="System.Windows.Forms.CacheVirtualItemsEventArgs" /> class with the specified starting and ending indices.
        /// </summary>
        /// <param name="startIndex">The starting index of a range of items needed by the
        /// <see cref="System.Windows.Forms.ListView" /> for the next
        /// <see cref="System.Windows.Forms.ListView.RetrieveVirtualItem" /> event that occurs.
        /// </param>
        /// <param name="endIndex">The ending index of a range of items needed by the
        /// <see cref="System.Windows.Forms.ListView" /> for the next
        /// <see cref="System.Windows.Forms.ListView.RetrieveVirtualItem" /> event that occurs.
        /// </param>
        public CacheVirtualItemsEventArgs(int startIndex, int endIndex)
        {
            StartIndex = startIndex;
            EndIndex = endIndex;
        }
        /// <summary>
        /// Gets the ending index for the range of values needed by a
        /// <see cref="System.Windows.Forms.ListView" /> control in virtual mode.
        /// </summary>
        /// <returns>The index at the end of the range of values needed by the
        /// <see cref="System.Windows.Forms.ListView" /> control.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public int EndIndex { get; }

        /// <summary>
        /// Gets the starting index for a range of values needed by a
        /// <see cref="System.Windows.Forms.ListView" /> control in virtual mode.
        /// </summary>
        /// <returns>The index at the start of the range of values needed by the
        /// <see cref="System.Windows.Forms.ListView" /> control.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public int StartIndex { get; }
    }
}
