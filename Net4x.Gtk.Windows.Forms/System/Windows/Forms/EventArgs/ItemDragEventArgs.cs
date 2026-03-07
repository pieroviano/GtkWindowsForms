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
    /// <see cref="System.Windows.Forms.ListView" /> and
    /// <see cref="System.Windows.Forms.TreeView" /> controls.
    /// </summary>
    /// <filterpriority>
    /// 2
    /// </filterpriority>
    [ComVisible(true)]
    public class ItemDragEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="System.Windows.Forms.ItemDragEventArgs" /> class with a specified mouse button.
        /// </summary>
        /// <param name="button">A bitwise combination of
        /// <see cref="System.Windows.Forms.MouseButtons" /> values that indicates which mouse buttons were pressed.
        /// </param>
        public ItemDragEventArgs(MouseButtons button) : this(button, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="System.Windows.Forms.ItemDragEventArgs" /> class with a specified mouse button and the item that is being dragged.
        /// </summary>
        /// <param name="button">A bitwise combination of
        /// <see cref="System.Windows.Forms.MouseButtons" /> values that indicates which mouse buttons were pressed.
        /// </param>
        /// <param name="item">The item being dragged.
        /// </param>
        public ItemDragEventArgs(MouseButtons button, object item)
        {
            Button = button;
            Item = item;
        }
        /// <summary>
        /// Gets a value that indicates which mouse buttons were pressed during the drag operation.
        /// </summary>
        /// <returns>A bitwise combination of
        /// <see cref="System.Windows.Forms.MouseButtons" /> values.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public MouseButtons Button { get; }

        /// <summary>
        /// Gets the item that is being dragged.
        /// </summary>
        /// <returns>An object that represents the item being dragged.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public object Item { get; }
    }
}
