// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;

namespace System.Windows.Forms
{
    /// <summary>
    /// Provides data for the
    /// <see cref="T:System.Windows.Forms.ListView.ColumnReordered" /> event.
    /// </summary>
    public class ColumnReorderedEventArgs : CancelEventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ColumnReorderedEventArgs"/> class.
        /// </summary>
        /// <param name="oldDisplayIndex">Old index of the display.</param>
        /// <param name="newDisplayIndex">New index of the display.</param>
        /// <param name="header">The header.</param>
        public ColumnReorderedEventArgs(int oldDisplayIndex, int newDisplayIndex, ColumnHeader header) : base()
        {
            OldDisplayIndex = oldDisplayIndex;
            NewDisplayIndex = newDisplayIndex;
            Header = header;
        }

        /// <summary>
        /// Gets the header.
        /// </summary>
        /// <value>The header.</value>
        public ColumnHeader Header { get; }

        /// <summary>
        /// Creates new displayindex.
        /// </summary>
        /// <value>The new index of the display.</value>
        public int NewDisplayIndex { get; }

        /// <summary>
        /// Gets the old index of the display.
        /// </summary>
        /// <value>The old index of the display.</value>
        public int OldDisplayIndex { get; }

    }
}
