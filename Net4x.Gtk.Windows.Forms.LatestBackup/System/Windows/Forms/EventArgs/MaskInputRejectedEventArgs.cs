// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;

namespace System.Windows.Forms
{
    /// <summary>
    /// Provides data for the
    /// <see cref="!:System.Windows.Forms.MaskedTextBox.MaskInputRejected" /> event.
    /// </summary>
    /// <filterpriority>
    /// 2
    /// </filterpriority>
    public class MaskInputRejectedEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="System.Windows.Forms.MaskInputRejectedEventArgs" /> class.
        /// </summary>
        /// <param name="position">An
        /// <see cref="System.Int32" /> value that contains the zero-based position of the character that failed the mask. The position includes literal characters.
        /// </param>
        /// <param name="rejectionHint">A
        /// <see cref="System.ComponentModel.MaskedTextResultHint" /> that generally describes why the character was rejected.
        /// </param>
        public MaskInputRejectedEventArgs(int position, MaskedTextResultHint rejectionHint)
        {
            Position = position;
            RejectionHint = rejectionHint;
        }

        /// <summary>
        /// Gets the position in the mask corresponding to the invalid input character.
        /// </summary>
        /// <returns>An
        /// <see cref="System.Int32" /> value that contains the zero-based position of the character that failed the mask. The position includes literal characters.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public int Position { get; }

        /// <summary>
        /// Gets an enumerated value that describes why the input character was rejected.
        /// </summary>
        /// <returns>A
        /// <see cref="System.ComponentModel.MaskedTextResultHint" /> that generally describes why the character was rejected.
        /// </returns>
        public MaskedTextResultHint RejectionHint { get; }
    }
}
