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
        public DataGridViewBindingCompleteEventArgs(ListChangedType listChangedType)
        {
            ListChangedType = listChangedType;
        }

        public ListChangedType ListChangedType { get; }
    }
}
