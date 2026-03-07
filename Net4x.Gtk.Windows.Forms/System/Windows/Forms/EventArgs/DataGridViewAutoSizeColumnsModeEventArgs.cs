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
        public DataGridViewAutoSizeColumnsModeEventArgs(DataGridViewAutoSizeColumnMode[] previousModes)
        {
            PreviousModes = previousModes;
        }

        public DataGridViewAutoSizeColumnMode[] PreviousModes { get; }
    }
}
