// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;

namespace System.Windows.Forms
{
    /// <summary>
    /// Provides data for the
    /// <see cref="System.Windows.Forms.PropertyGrid.PropertyValueChanged" />
    /// event of a
    /// <see cref="System.Windows.Forms.PropertyGrid" />
    /// </summary>
    /// <filterpriority> 2
    /// </filterpriority>
    [ComVisible(true)]
    public class PropertyValueChangedEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="System.Windows.Forms.PropertyValueChangedEventArgs" /> class.
        /// </summary>
        /// <param name="changedItem">The item in the grid that changed.
        /// </param>
        /// <param name="oldValue">The old property value.
        /// </param>
        public PropertyValueChangedEventArgs(GridItem changedItem, object oldValue)
        {
            ChangedItem = changedItem;
            OldValue = oldValue;
        }
        /// <summary>
        /// Gets the
        /// <see cref="System.Windows.Forms.GridItem" /> that was changed.
        /// </summary>
        /// <returns>A
        /// <see cref="System.Windows.Forms.GridItem" /> in the
        /// <see cref="System.Windows.Forms.PropertyGrid" />
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public GridItem ChangedItem { get; }

        /// <summary>
        /// The value of the grid item before it was changed.
        /// </summary>
        /// <returns>A object representing the old value of the property.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public object OldValue { get; }
    }
}
