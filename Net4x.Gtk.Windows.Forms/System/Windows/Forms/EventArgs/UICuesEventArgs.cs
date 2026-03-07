// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    /// <summary>
    /// Provides data for the
    /// <see cref="System.Windows.Forms.Control.ChangeUICues" /> event.
    /// </summary>
    /// <filterpriority>
    /// 2
    /// </filterpriority>
    public class UICuesEventArgs : EventArgs
    {
        private readonly UICues _uicues;
        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="System.Windows.Forms.UICuesEventArgs" /> class with the specified
        /// <see cref="System.Windows.Forms.UICues" />
        /// </summary>
        /// <param name="uicues">A bitwise combination of the
        /// <see cref="System.Windows.Forms.UICues" /> values.
        /// </param>
        public UICuesEventArgs(UICues uicues)
        {
            _uicues = uicues;
        }
        /// <summary>
        /// Gets the bitwise combination of the
        /// <see cref="System.Windows.Forms.UICues" /> values.
        /// </summary>
        /// <returns>A bitwise combination of the
        /// <see cref="System.Windows.Forms.UICues" /> values. The default is
        /// <see cref="System.Windows.Forms.UICues.Changed" />
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public UICues Changed => (_uicues & UICues.Changed);

        /// <summary>
        /// Gets a value indicating whether the state of the focus cues has changed.
        /// </summary>
        /// <returns>true if the state of the focus cues has changed; otherwise, false.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public bool ChangeFocus => (_uicues & UICues.ChangeFocus) != 0;

        /// <summary>
        /// Gets a value indicating whether the state of the keyboard cues has changed.
        /// </summary>
        /// <returns>true if the state of the keyboard cues has changed; otherwise, false.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public bool ChangeKeyboard => (_uicues & UICues.ChangeKeyboard) != 0;

        /// <summary>
        /// Gets a value indicating whether focus rectangles are shown after the change.
        /// </summary>
        /// <returns>true if focus rectangles are shown after the change; otherwise, false.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public bool ShowFocus => (_uicues & UICues.ShowFocus) != 0;

        /// <summary>
        /// Gets a value indicating whether keyboard cues are underlined after the change.
        /// </summary>
        /// <returns>true if keyboard cues are underlined after the change; otherwise, false.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public bool ShowKeyboard => (_uicues & UICues.ShowKeyboard) != 0;
    }
}
