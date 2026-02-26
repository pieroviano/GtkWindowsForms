// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Drawing;

namespace System.Windows.Forms
{
    /// <summary>
    /// Provides data for the
    /// <see cref="!:System.Windows.Forms.ToolTip.Popup" /> event.
    /// </summary>
    /// <filterpriority>
    /// 2
    /// </filterpriority>
    public class PopupEventArgs : CancelEventArgs
    {
        /// <summary>
        /// Initializes an instance of the
        /// <see cref="System.Windows.Forms.PopupEventArgs" /> class.
        /// </summary>
        /// <param name="associatedWindow">The
        /// <see cref="System.Windows.Forms.IWin32Window" /> that the ToolTip is bound to.
        /// </param>
        /// <param name="associatedControl">The
        /// <see cref="System.Windows.Forms.Control" /> that the ToolTip is being created for.
        /// </param>
        /// <param name="isBalloon">true to indicate that the associated ToolTip window has a balloon-style appearance; otherwise, false to indicate that the ToolTip window has a standard rectangular appearance.
        /// </param>
        /// <param name="size">The
        /// <see cref="System.Drawing.Size" /> of the ToolTip.
        /// </param>
        public PopupEventArgs(IWin32Window associatedWindow, Control associatedControl, bool isBalloon, Size size)
        {
            AssociatedWindow = associatedWindow;
            AssociatedControl = associatedControl;
            ToolTipSize = size;
            IsBalloon = isBalloon;
        }

        /// <summary>
        /// Gets the window to which this
        /// <see cref="System.Windows.Forms.ToolTip" /> is bound.
        /// </summary>
        /// <returns>The window which owns the
        /// <see cref="System.Windows.Forms.ToolTip" />
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public IWin32Window AssociatedWindow { get; }

        /// <summary>
        /// Gets the control for which the
        /// <see cref="System.Windows.Forms.ToolTip" /> is being drawn.
        /// </summary>
        /// <returns>The
        /// <see cref="System.Windows.Forms.Control" /> that is associated with the
        /// <see cref="System.Windows.Forms.ToolTip" />, or null if the ToolTip is not associated with a control.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public Control AssociatedControl { get; }

        /// <summary>
        /// Gets or sets the size of the ToolTip.
        /// </summary>
        /// <returns>The
        /// <see cref="System.Drawing.Size" /> of the
        /// <see cref="System.Windows.Forms.ToolTip" /> window.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public Size ToolTipSize { get; set; }

        /// <summary>
        /// Gets a value indicating whether the ToolTip is displayed as a standard rectangular or a balloon window.
        /// </summary>
        /// <returns>true if the ToolTip is displayed in a balloon window; otherwise, false if a standard rectangular window is used.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public bool IsBalloon { get; }
    }
}
