// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;

namespace System.Windows.Forms
{
    /// <summary>
    /// Provides data for the
    /// <see cref="!:System.Windows.Forms.ListView.ColumnWidthChanging" /> event.
    /// </summary>
    public class ColumnWidthChangingEventArgs : CancelEventArgs
    {
        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="System.Windows.Forms.ColumnWidthChangingEventArgs" /> class with the specified column index and width.
        /// </summary>
        /// <param name="columnIndex">The index of the column whose width is changing.
        /// </param>
        /// <param name="newWidth">The new width for the column.
        /// </param>
        public ColumnWidthChangingEventArgs(int columnIndex, int newWidth) : this(columnIndex, newWidth, false)
        {
        }
        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="System.Windows.Forms.ColumnWidthChangingEventArgs" /> class, specifying the column index and width and whether to cancel the event.
        /// </summary>
        /// <param name="columnIndex">The index of the column whose width is changing.
        /// </param>
        /// <param name="newWidth">The new width of the column.
        /// </param>
        /// <param name="cancel">true to cancel the width change; otherwise, false.
        /// </param>
        public ColumnWidthChangingEventArgs(int columnIndex, int newWidth, bool cancel) : base(cancel)
        {
            ColumnIndex = columnIndex;
            NewWidth = newWidth;
        }

        /// <summary>
        /// Gets the index of the column whose width is changing.
        /// </summary>
        /// <returns>The index of the column whose width is changing.
        /// </returns>
        public int ColumnIndex { get; }

        /// <summary>
        /// Returns the new width for the column header who is changing
        /// </summary>
        public int NewWidth { get; set; }
    }
}
