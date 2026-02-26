// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    /// <summary>
    /// Provides data for the
    /// <see cref="System.Windows.Forms.DataGridView.AutoSizeColumnsModeChanged" /> event.
    /// </summary>
    public class DataGridViewAutoSizeColumnsModeEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="System.Windows.Forms.DataGridViewAutoSizeColumnsModeEventArgs" /> class.
        /// </summary>
        /// <param name="previousModes">An array of
        /// <see cref="System.Windows.Forms.DataGridViewAutoSizeColumnMode" /> values representing the previous
        /// <see cref="System.Windows.Forms.DataGridViewColumn.AutoSizeMode" /> property values of each column.
        /// </param>
        public DataGridViewAutoSizeColumnsModeEventArgs(DataGridViewAutoSizeColumnMode[] previousModes)
        {
            PreviousModes = previousModes;
        }
        /// <summary>
        /// Gets an array of the previous values of the column
        /// <see cref="System.Windows.Forms.DataGridViewColumn.AutoSizeMode" /> properties.
        /// </summary>
        /// <returns>An array of
        /// <see cref="System.Windows.Forms.DataGridViewAutoSizeColumnMode" /> values representing the previous values of the column
        /// <see cref="System.Windows.Forms.DataGridViewColumn.AutoSizeMode" /> properties.
        /// </returns>
        public DataGridViewAutoSizeColumnMode[] PreviousModes { get; }
    }
}
