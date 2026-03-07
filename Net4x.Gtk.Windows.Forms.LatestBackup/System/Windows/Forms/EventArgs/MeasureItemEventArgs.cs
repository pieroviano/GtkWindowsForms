// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;

namespace System.Windows.Forms
{
    /// <summary>
    /// Provides data for the MeasureItem event of the
    /// <see cref="System.Windows.Forms.ListBox" /> ,
    /// <see cref="System.Windows.Forms.ComboBox" /> ,
    /// <see cref="System.Windows.Forms.CheckedListBox" /> , and
    /// <see cref="!:System.Windows.Forms.MenuItem" /> controls.
    /// </summary>
    /// <filterpriority>
    /// 2
    /// </filterpriority>
    public class MeasureItemEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="System.Windows.Forms.MeasureItemEventArgs" /> class.
        /// </summary>
        /// <param name="graphics">The
        /// <see cref="System.Drawing.Graphics" /> object being written to.
        /// </param>
        /// <param name="index">The index of the item for which you need the height or width.
        /// </param>
        public MeasureItemEventArgs(Graphics graphics, int index) : this(graphics, index, 0)
        {
        }

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="System.Windows.Forms.MeasureItemEventArgs" /> class providing a parameter for the item height.
        /// </summary>
        /// <param name="graphics">The
        /// <see cref="System.Drawing.Graphics" /> object being written to.
        /// </param>
        /// <param name="index">The index of the item for which you need the height or width.
        /// </param>
        /// <param name="itemHeight">The height of the item to measure relative to the
        /// <paramref name="graphics" /> object.
        /// </param>
        public MeasureItemEventArgs(Graphics graphics, int index, int itemHeight)
        {
            Graphics = graphics;
            Index = index;
            ItemHeight = itemHeight;
            ItemWidth = 0;
        }

        /// <summary>
        /// Gets the
        /// <see cref="System.Drawing.Graphics" /> object to measure against.
        /// </summary>
        /// <returns>The
        /// <see cref="System.Drawing.Graphics" /> object to use to determine the scale of the item you are drawing.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public Graphics Graphics { get; }

        /// <summary>
        /// Gets the index of the item for which the height and width is needed.
        /// </summary>
        /// <returns>The index of the item to be measured.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public int Index { get; }

        /// <summary>
        /// Gets or sets the height of the item specified by the
        /// <see cref="System.Windows.Forms.MeasureItemEventArgs.Index" />
        /// </summary>
        /// <returns>The height of the item measured.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public int ItemHeight { get; set; }

        /// <summary>
        /// Gets or sets the width of the item specified by the
        /// <see cref="System.Windows.Forms.MeasureItemEventArgs.Index" />
        /// </summary>
        /// <returns>The width of the item measured.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public int ItemWidth { get; set; }

    }
}
