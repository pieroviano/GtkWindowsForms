// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;

namespace System.Windows.Forms
{
    /// <summary>
    /// Provides data for the
    /// <see cref="!:System.Windows.Forms.ListView.ItemDrag" />
    /// event of the
    /// <see cref="T:System.Windows.Forms.ListView" /> and
    /// <see cref="T:System.Windows.Forms.TreeView" /> controls.
    /// </summary>
    /// <filterpriority>
    /// 2
    /// </filterpriority>
    [ComVisible(true)]
    public class ItemDragEventArgs : EventArgs
    {
        public ItemDragEventArgs(MouseButtons button) : this(button, null)
        {
        }

        public ItemDragEventArgs(MouseButtons button, object item)
        {
            Button = button;
            Item = item;
        }

        public MouseButtons Button { get; }

        public object Item { get; }

    }
}
