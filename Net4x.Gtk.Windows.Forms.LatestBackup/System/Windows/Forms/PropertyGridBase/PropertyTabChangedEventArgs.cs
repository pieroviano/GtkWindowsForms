// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Windows.Forms.Design;

namespace System.Windows.Forms
{
    /// <summary>
    /// Provides data for the
    /// <see cref="System.Windows.Forms.PropertyGrid.PropertyTabChanged" /> event of a
    /// <see cref="System.Windows.Forms.PropertyGrid" />
    /// </summary>
    /// <filterpriority> 2
    /// </filterpriority>
    public class PropertyTabChangedEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="System.Windows.Forms.PropertyTabChangedEventArgs" /> class.
        /// </summary>
        /// <param name="oldTab">The Previously selected property tab.
        /// </param>
        /// <param name="newTab">The newly selected property tab.
        /// </param>
        public PropertyTabChangedEventArgs(PropertyTab? oldTab, PropertyTab? newTab)
        {
            OldTab = oldTab;
            NewTab = newTab;
        }
        /// <summary>
        /// Gets the old
        /// <see cref="System.Windows.Forms.Design.PropertyTab" /> selected.
        /// </summary>
        /// <returns>The old
        /// <see cref="System.Windows.Forms.Design.PropertyTab" /> that was selected.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public PropertyTab? OldTab { get; }
        /// <summary>
        /// Gets the new
        /// <see cref="System.Windows.Forms.Design.PropertyTab" /> selected.
        /// </summary>
        /// <returns>The newly selected
        /// <see cref="System.Windows.Forms.Design.PropertyTab" />
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public PropertyTab? NewTab { get; }
    }
}
