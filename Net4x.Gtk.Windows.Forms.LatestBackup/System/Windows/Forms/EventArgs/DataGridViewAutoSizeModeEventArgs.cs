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
        public DataGridViewAutoSizeModeEventArgs(bool previousModeAutoSized)
        {
            PreviousModeAutoSized = previousModeAutoSized;
        }

        public bool PreviousModeAutoSized { get; }
    }
}
