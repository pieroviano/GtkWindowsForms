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
        public ColumnReorderedEventArgs(int oldDisplayIndex, int newDisplayIndex, ColumnHeader header) : base()
        {
            OldDisplayIndex = oldDisplayIndex;
            NewDisplayIndex = newDisplayIndex;
            Header = header;
        }

        public ColumnHeader Header { get; }

        public int NewDisplayIndex { get; }

        public int OldDisplayIndex { get; }

    }
}
