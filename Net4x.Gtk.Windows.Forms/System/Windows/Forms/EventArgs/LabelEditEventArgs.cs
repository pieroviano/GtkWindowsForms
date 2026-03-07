// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    /// <summary>
    /// Provides data for the
    /// <see cref="!:System.Windows.Forms.ListView.BeforeLabelEdit" /> and
    /// <see cref="!:System.Windows.Forms.ListView.AfterLabelEdit" /> events.
    /// </summary>
    /// <filterpriority>
    /// 2
    /// </filterpriority>
    public class LabelEditEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="System.Windows.Forms.LabelEditEventArgs" /> class with the specified index to the
        /// <see cref="System.Windows.Forms.ListViewItem" /> to edit.
        /// </summary>
        /// <param name="item">The zero-based index of the
        /// <see cref="System.Windows.Forms.ListViewItem" />, containing the label to edit.
        /// </param>
        public LabelEditEventArgs(int item) : this(item, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="System.Windows.Forms.LabelEditEventArgs" /> class with the specified index to the
        /// <see cref="System.Windows.Forms.ListViewItem" /> being edited and the new text for the label of the
        /// <see cref="System.Windows.Forms.ListViewItem" />
        /// </summary>
        /// <param name="item">The zero-based index of the
        /// <see cref="System.Windows.Forms.ListViewItem" />, containing the label to edit.
        /// </param>
        /// <param name="label">The new text assigned to the label of the
        /// <see cref="System.Windows.Forms.ListViewItem" />
        /// </param>
        public LabelEditEventArgs(int item, string label)
        {
            Item = item;
            Label = label;
        }

        /// <summary>
        /// Gets or sets a value indicating whether changes made to the label of the
        /// <see cref="System.Windows.Forms.ListViewItem" /> should be canceled.
        /// </summary>
        /// <returns>true if the edit operation of the label for the
        /// <see cref="System.Windows.Forms.ListViewItem" /> should be canceled; otherwise false.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public bool CancelEdit { get; set; }

        /// <summary>
        /// Gets the zero-based index of the
        /// <see cref="System.Windows.Forms.ListViewItem" /> containing the label to edit.
        /// </summary>
        /// <returns>The zero-based index of the
        /// <see cref="System.Windows.Forms.ListViewItem" />
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public int Item { get; }

        /// <summary>
        /// Gets the new text assigned to the label of the
        /// <see cref="System.Windows.Forms.ListViewItem" />
        /// </summary>
        /// <returns>The new text to associate with the
        /// <see cref="System.Windows.Forms.ListViewItem" /> or null if the text is unchanged.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public string Label { get; }
    }
}
