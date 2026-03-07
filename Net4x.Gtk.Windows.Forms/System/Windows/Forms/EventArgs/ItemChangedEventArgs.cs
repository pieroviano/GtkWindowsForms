// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    /// <summary>
    /// Provides data for the
    /// <see cref="System.Windows.Forms.CurrencyManager.ItemChanged" /> event.
    /// </summary>
    /// <filterpriority>
    /// 2
    /// </filterpriority>
    public class ItemChangedEventArgs : EventArgs
    {
        internal ItemChangedEventArgs(int index)
        {
            Index = index;
        }

        /// <summary>
        /// Indicates the position of the item being changed within the list.
        /// </summary>
        /// <returns>The zero-based index to the item being changed.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public int Index { get; }

    }
}
