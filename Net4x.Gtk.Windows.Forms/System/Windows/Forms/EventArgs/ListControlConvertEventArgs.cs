// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    /// <summary>
    /// Provides data for the
    /// <see cref="System.Windows.Forms.ListControl.Format" /> event.
    /// </summary>
    /// <filterpriority>
    /// 2
    /// </filterpriority>
    public class ListControlConvertEventArgs : ConvertEventArgs
    {
        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="System.Windows.Forms.ListControlConvertEventArgs" /> class with the specified object, type, and list item.
        /// </summary>
        /// <param name="value">The value displayed in the
        /// <see cref="System.Windows.Forms.ListControl" />
        /// </param>
        /// <param name="desiredType">The
        /// <see cref="System.Type" /> for the displayed item.
        /// </param>
        /// <param name="listItem">The data source item to be displayed in the
        /// <see cref="System.Windows.Forms.ListControl" />
        /// </param>
        public ListControlConvertEventArgs(object value, Type desiredType, object listItem) : base(value, desiredType)
        {
            ListItem = listItem;
        }
        /// <summary>
        /// Gets a data source item.
        /// </summary>
        /// <returns>The
        /// <see cref="System.Object" /> that represents an item in the data source.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public object ListItem { get; }
    }
}
