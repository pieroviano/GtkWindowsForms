// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;

namespace System.Windows.Forms
{
    /// <summary>
    /// Provides data for the
    /// <see cref="System.Windows.Forms.CheckedListBox.ItemCheck" /> event of the
    /// <see cref="System.Windows.Forms.CheckedListBox" /> and
    /// <see cref="System.Windows.Forms.ListView" /> controls.
    /// </summary>
    /// <filterpriority>
    /// 2
    /// </filterpriority>
    [ComVisible(true)]
    public class ItemCheckEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="System.Windows.Forms.ItemCheckEventArgs" /> class.
        /// </summary>
        /// <param name="index">The zero-based index of the item to change.
        /// </param>
        /// <param name="newCheckValue">One of the
        /// <see cref="System.Windows.Forms.CheckState" /> values that indicates whether to change the check box for the item to be checked, unchecked, or indeterminate.
        /// </param>
        /// <param name="currentValue">One of the
        /// <see cref="System.Windows.Forms.CheckState" /> values that indicates whether the check box for the item is currently checked, unchecked, or indeterminate.
        /// </param>
        public ItemCheckEventArgs(int index, CheckState newCheckValue, CheckState currentValue)
        {
            Index = index;
            NewValue = newCheckValue;
            CurrentValue = currentValue;
        }

        /// <summary>
        /// Gets a value indicating the current state of the item's check box.
        /// </summary>
        /// <returns>One of the
        /// <see cref="System.Windows.Forms.CheckState" /> values.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public CheckState CurrentValue { get; }

        /// <summary>
        /// Gets the zero-based index of the item to change.
        /// </summary>
        /// <returns>The zero-based index of the item to change.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public int Index { get; }

        /// <summary>
        /// Gets or sets a value indicating whether to set the check box for the item to be checked, unchecked, or indeterminate.
        /// </summary>
        /// <returns>One of the
        /// <see cref="System.Windows.Forms.CheckState" /> values.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public CheckState NewValue { get; set; }
    }
}
