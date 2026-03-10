// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Windows.Forms.Design;

namespace System.Windows.Forms
{
    /// <summary>
    /// Provides data for the
    /// <see cref="T:System.Windows.Forms.PropertyGrid.PropertyTabChanged" /> event of a
    /// <see cref="T:System.Windows.Forms.PropertyGrid" />
    /// </summary>
    /// <filterpriority> 2
    /// </filterpriority>
    public class PropertyTabChangedEventArgs : EventArgs
    {
        public PropertyTabChangedEventArgs(PropertyTab? oldTab, PropertyTab? newTab)
        {
            OldTab = oldTab;
            NewTab = newTab;
        }

        public PropertyTab? NewTab { get; }

        public PropertyTab? OldTab { get; }

    }
}
