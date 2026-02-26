// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    /// <summary>
    /// Provides data for the
    /// <see cref="System.Windows.Forms.DataGridView.CellStyleContentChanged" /> event.
    /// </summary>
    /// <filterpriority>
    /// 2
    /// </filterpriority>
    public class DataGridViewCellStyleContentChangedEventArgs : EventArgs
    {
        internal DataGridViewCellStyleContentChangedEventArgs(DataGridViewCellStyle dataGridViewCellStyle, bool changeAffectsPreferredSize)
        {
            CellStyle = dataGridViewCellStyle;
            ChangeAffectsPreferredSize = changeAffectsPreferredSize;
        }
        /// <summary>
        /// Gets the changed
        /// <see cref="System.Windows.Forms.DataGridViewCellStyle" />
        /// </summary>
        /// <returns>The changed
        /// <see cref="System.Windows.Forms.DataGridViewCellStyle" />
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public DataGridViewCellStyle CellStyle { get; }
        /// <summary>
        /// Gets the scope that is affected by the changed cell style.
        /// </summary>
        /// <returns>A
        /// <see cref="System.Windows.Forms.DataGridViewCellStyleScopes" /> that indicates which
        /// <see cref="System.Windows.Forms.DataGridView" /> entity owns the cell style that changed.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public DataGridViewCellStyleScopes CellStyleScope => CellStyle.Scope;

        internal bool ChangeAffectsPreferredSize { get; }
    }
}
