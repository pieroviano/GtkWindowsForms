// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;

namespace System.Windows.Forms
{
    /// <summary>
    /// Provides data for the
    /// <see cref="System.Windows.Forms.Control.KeyDown" /> or
    /// <see cref="System.Windows.Forms.Control.KeyUp" /> event.
    /// </summary>
    [ComVisible(true)]
    public class KeyEventArgs : EventArgs
    {
        private bool _suppressKeyPress = false;

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="System.Windows.Forms.KeyEventArgs" /> class.
        /// </summary>
        /// <param name="keyData">A
        /// <see cref="System.Windows.Forms.Keys" /> representing the key that was pressed, combined with any modifier flags that indicate which CTRL, SHIFT, and ALT keys were pressed at the same time. Possible values are obtained be applying the bitwise OR (|) operator to constants from the
        /// <see cref="System.Windows.Forms.Keys" /> enumeration.
        /// </param>
        public KeyEventArgs(Keys keyData)
        {
            KeyData = keyData;
        }

        /// <summary>
        /// Gets a value indicating whether the ALT key was pressed.
        /// </summary>
        /// <returns>true if the ALT key was pressed; otherwise, false.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public virtual bool Alt => (KeyData & Keys.Alt) == Keys.Alt;

        /// <summary>
        /// Gets a value indicating whether the CTRL key was pressed.
        /// </summary>
        /// <returns>true if the CTRL key was pressed; otherwise, false.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public bool Control => (KeyData & Keys.Control) == Keys.Control;

        /// <summary>
        /// Gets or sets a value indicating whether the event was handled.
        /// </summary>
        /// <returns>true to bypass the control's default handling; otherwise, false to also pass the event along to the default control handler.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public bool Handled { get; set; }

        /// <summary>
        /// Gets the keyboard code for a
        /// <see cref="System.Windows.Forms.Control.KeyDown" /> or
        /// <see cref="System.Windows.Forms.Control.KeyUp" /> event.
        /// </summary>
        /// <returns>A
        /// <see cref="System.Windows.Forms.Keys" /> value that is the key code for the event.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public Keys KeyCode
        {
            get
            {
                Keys keyGenerated = KeyData & Keys.KeyCode;

                // since Keys can be discontiguous, keeping Enum.IsDefined.
                if (!Enum.IsDefined(typeof(Keys), (int)keyGenerated))
                {
                    return Keys.None;
                }

                return keyGenerated;
            }
        }

        /// <summary>
        /// Gets the key data for a
        /// <see cref="System.Windows.Forms.Control.KeyDown" /> or
        /// <see cref="System.Windows.Forms.Control.KeyUp" /> event.
        /// </summary>
        /// <returns>A
        /// <see cref="System.Windows.Forms.Keys" /> representing the key code for the key that was pressed, combined with modifier flags that indicate which combination of CTRL, SHIFT, and ALT keys was pressed at the same time.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public Keys KeyData { get; }

        /// <summary>
        /// Gets the keyboard value for a
        /// <see cref="System.Windows.Forms.Control.KeyDown" /> or
        /// <see cref="System.Windows.Forms.Control.KeyUp" /> event.
        /// </summary>
        /// <returns>The integer representation of the
        /// <see cref="System.Windows.Forms.KeyEventArgs.KeyCode" /> property.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public int KeyValue => (int)(KeyData & Keys.KeyCode);

        /// <summary>
        /// Gets the modifier flags for a
        /// <see cref="System.Windows.Forms.Control.KeyDown" /> or
        /// <see cref="System.Windows.Forms.Control.KeyUp" /> event. The flags indicate which combination of CTRL, SHIFT, and ALT keys was pressed.
        /// </summary>
        /// <returns>A
        /// <see cref="System.Windows.Forms.Keys" /> value representing one or more modifier flags.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public Keys Modifiers => KeyData & Keys.Modifiers;

        /// <summary>
        /// Gets a value indicating whether the SHIFT key was pressed.
        /// </summary>
        /// <returns>true if the SHIFT key was pressed; otherwise, false.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public virtual bool Shift => (KeyData & Keys.Shift) == Keys.Shift;

        /// <summary>
        /// Gets or sets a value indicating whether the key event should be passed on to the underlying control.
        /// </summary>
        /// <returns>true if the key event should not be sent to the control; otherwise, false.
        /// </returns>
        public bool SuppressKeyPress
        {
            get => _suppressKeyPress;
            set
            {
                _suppressKeyPress = value;
                Handled = value;
            }
        }
    }
}
