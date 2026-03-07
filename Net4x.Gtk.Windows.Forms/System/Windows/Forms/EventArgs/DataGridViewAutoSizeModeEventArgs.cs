// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    /// <summary>
    /// Provides data for the
    /// <see cref="System.Windows.Forms.DataGridView" />
    /// <see cref="System.Windows.Forms.DataGridView.AutoSizeRowsModeChanged" /> and
    /// <see cref="System.Windows.Forms.DataGridView.RowHeadersWidthSizeModeChanged" /> events.
    /// </summary>
    /// <filterpriority>
    /// 2
    /// </filterpriority>
    public class DataGridViewAutoSizeModeEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="System.Windows.Forms.DataGridViewAutoSizeModeEventArgs" /> class.
        /// </summary>
        /// <param name="previousModeAutoSized">true if the
        /// <see cref="System.Windows.Forms.DataGridView.AutoSizeRowsMode" /> property was previously set to any
        /// <see cref="System.Windows.Forms.DataGridViewAutoSizeRowsMode" /> value other than
        /// <see cref="System.Windows.Forms.DataGridViewAutoSizeRowsMode.None" /> or the
        /// <see cref="System.Windows.Forms.DataGridView.RowHeadersWidthSizeMode" /> property was previously set to any
        /// <see cref="System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode" /> value other than
        /// <see cref="System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing" /> or
        /// <see cref="System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.EnableResizing" />; otherwise, false.
        /// </param>
        public DataGridViewAutoSizeModeEventArgs(bool previousModeAutoSized)
        {
            PreviousModeAutoSized = previousModeAutoSized;
        }
        /// <summary>
        /// Gets a value specifying whether the
        /// <see cref="System.Windows.Forms.DataGridView" /> was previously set to automatically resize.
        /// </summary>
        /// <returns>true if the
        /// <see cref="System.Windows.Forms.DataGridView.AutoSizeRowsMode" /> property was previously set to any
        /// <see cref="System.Windows.Forms.DataGridViewAutoSizeRowsMode" /> value other than
        /// <see cref="System.Windows.Forms.DataGridViewAutoSizeRowsMode.None" /> or the
        /// <see cref="System.Windows.Forms.DataGridView.RowHeadersWidthSizeMode" /> property was previously set to any
        /// <see cref="System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode" /> value other than
        /// <see cref="System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing" /> or
        /// <see cref="System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.EnableResizing" />; otherwise, false.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public bool PreviousModeAutoSized { get; }
    }
}
