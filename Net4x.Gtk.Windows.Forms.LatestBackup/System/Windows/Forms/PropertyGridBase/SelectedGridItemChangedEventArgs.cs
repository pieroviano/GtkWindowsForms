// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    /// <summary>
    /// Provides data for the
    /// <see cref="System.Windows.Forms.PropertyGrid.SelectedGridItemChanged" />
    /// event of the
    /// <see cref="System.Windows.Forms.PropertyGrid" /> control.
    /// </summary>
    /// <filterpriority>
    /// 2
    /// </filterpriority>
    public class SelectedGridItemChangedEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="System.Windows.Forms.SelectedGridItemChangedEventArgs" /> class.
        /// </summary>
        /// <param name="oldSel">The previously selected grid item.
        /// </param>
        /// <param name="newSel">The newly selected grid item.
        /// </param>
        public SelectedGridItemChangedEventArgs(GridItem oldSel, GridItem newSel)
        {
            OldSelection = oldSel;
            NewSelection = newSel;
        }

        /// <summary>
        /// Gets the newly selected
        /// <see cref="System.Windows.Forms.GridItem" />
        /// </summary>
        /// <returns>The new
        /// <see cref="System.Windows.Forms.GridItem" />
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public GridItem NewSelection { get; }

        /// <summary>
        /// Gets the previously selected
        /// <see cref="System.Windows.Forms.GridItem" />
        /// </summary>
        /// <returns>The old
        /// <see cref="System.Windows.Forms.GridItem" /> This can be null.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public GridItem OldSelection { get; }

    }
}
