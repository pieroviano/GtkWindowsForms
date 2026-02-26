// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;

namespace System.Windows.Forms
{
    /// <summary>
    /// Provides data for the
    /// <see cref="System.Windows.Forms.ListView.ColumnReordered" /> event.
    /// </summary>
    public class ColumnReorderedEventArgs : CancelEventArgs
    {
        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="System.Windows.Forms.ColumnReorderedEventArgs" /> class.
        /// </summary>
        /// <param name="oldDisplayIndex">The previous display position of the
        /// <see cref="System.Windows.Forms.ColumnHeader" />
        /// </param>
        /// <param name="newDisplayIndex">The new display position for the
        /// <see cref="System.Windows.Forms.ColumnHeader" />
        /// </param>
        /// <param name="header">The
        /// <see cref="System.Windows.Forms.ColumnHeader" /> that is being reordered.
        /// </param>
        public ColumnReorderedEventArgs(int oldDisplayIndex, int newDisplayIndex, ColumnHeader header) : base()
        {
            OldDisplayIndex = oldDisplayIndex;
            NewDisplayIndex = newDisplayIndex;
            Header = header;
        }
        /// <summary>
        /// Gets the previous display position of the
        /// <see cref="System.Windows.Forms.ColumnHeader" />
        /// </summary>
        /// <returns>The previous display position of the
        /// <see cref="System.Windows.Forms.ColumnHeader" />
        /// </returns>
        public int OldDisplayIndex { get; }
        /// <summary>
        /// Gets the new display position of the
        /// <see cref="System.Windows.Forms.ColumnHeader" />
        /// </summary>
        /// <returns>The new display position of the
        /// <see cref="System.Windows.Forms.ColumnHeader" />
        /// </returns>
        public int NewDisplayIndex { get; }
        /// <summary>
        /// Gets the
        /// <see cref="System.Windows.Forms.ColumnHeader" /> that is being reordered.
        /// </summary>
        /// <returns>The
        /// <see cref="System.Windows.Forms.ColumnHeader" /> that is being reordered.
        /// </returns>
        public ColumnHeader Header { get; }
    }
}
