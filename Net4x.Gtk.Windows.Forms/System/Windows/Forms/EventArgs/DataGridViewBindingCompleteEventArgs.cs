// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;

namespace System.Windows.Forms
{
    /// <summary>
    /// Provides data for the
    /// <see cref="System.Windows.Forms.DataGridView.DataBindingComplete" /> event.
    /// </summary>
    /// <filterpriority>
    /// 2
    /// </filterpriority>
    public class DataGridViewBindingCompleteEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="System.Windows.Forms.DataGridViewBindingCompleteEventArgs" /> class.
        /// </summary>
        /// <param name="listChangedType">One of the
        /// <see cref="System.ComponentModel.ListChangedType" /> values.
        /// </param>
        public DataGridViewBindingCompleteEventArgs(ListChangedType listChangedType)
        {
            ListChangedType = listChangedType;
        }
        /// <summary>
        /// Gets a value specifying how the list changed.
        /// </summary>
        /// <returns>One of the
        /// <see cref="System.ComponentModel.ListChangedType" /> values.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public ListChangedType ListChangedType { get; }
    }
}
