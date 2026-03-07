// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    /// <summary>
    /// Provides data for the
    /// <see cref="System.Windows.Forms.ListView.ColumnClick" /> event.
    /// </summary>
    /// <filterpriority>
    /// 2
    /// </filterpriority>
    public class ColumnClickEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="System.Windows.Forms.ColumnClickEventArgs" /> class.
        /// </summary>
        /// <param name="column">The zero-based index of the column that is clicked.
        /// </param>
        public ColumnClickEventArgs(int column)
        {
            Column = column;
        }

        /// <summary>
        /// Gets the zero-based index of the column that is clicked.
        /// </summary>
        /// <returns>The zero-based index within the
        /// <see cref="System.Windows.Forms.ListView.ColumnHeaderCollection" /> of the column that is clicked.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public int Column { get; }

    }
}
