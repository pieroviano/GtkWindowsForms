// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    /// <summary>
    /// Provides data for the
    /// <see cref="System.Windows.Forms.Control.PreviewKeyDown" /> event.
    /// </summary>
    public class PreviewKeyDownEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="System.Windows.Forms.PreviewKeyDownEventArgs" /> class with the specified key.
        /// </summary>
        /// <param name="keyData">One of the
        /// <see cref="System.Windows.Forms.Keys" /> values.
        /// </param>
        public PreviewKeyDownEventArgs(Keys keyData)
        {
            KeyData = keyData;
        }

        /// <summary>
        /// Gets the key data for a
        /// <see cref="System.Windows.Forms.Control.KeyDown" /> or
        /// <see cref="System.Windows.Forms.Control.KeyUp" /> event.
        /// </summary>
        /// <returns>One of the
        /// <see cref="System.Windows.Forms.Keys" /> values.
        /// </returns>
        public Keys KeyData { get; }
        /// <summary>
        /// Gets a value indicating whether the ALT key was pressed.
        /// </summary>
        /// <returns>true if the ALT key was pressed; otherwise, false.
        /// </returns>
        public bool Alt => (KeyData & Keys.Alt) == Keys.Alt;

        /// <summary>
        /// Gets a value indicating whether the CTRL key was pressed.
        /// </summary>
        /// <returns>true if the CTRL key was pressed; otherwise, false.
        /// </returns>
        public bool Control => (KeyData & Keys.Control) == Keys.Control;

        /// <summary>
        /// Gets the keyboard code for a
        /// <see cref="System.Windows.Forms.Control.KeyDown" /> or
        /// <see cref="System.Windows.Forms.Control.KeyUp" /> event.
        /// </summary>
        /// <returns>One of the
        /// <see cref="System.Windows.Forms.Keys" /> values.
        /// </returns>
        public Keys KeyCode
        {
            get
            {
                Keys keyGenerated = KeyData & Keys.KeyCode;
                if (!Enum.IsDefined(typeof(Keys), (int)keyGenerated))
                {
                    return Keys.None;
                }

                return keyGenerated;
            }
        }

        /// <summary>
        /// Gets the keyboard value for a
        /// <see cref="System.Windows.Forms.Control.KeyDown" /> or
        /// <see cref="System.Windows.Forms.Control.KeyUp" /> event.
        /// </summary>
        /// <returns>An
        /// <see cref="System.Int32" /> representing the keyboard value.
        /// </returns>
        public int KeyValue => (int)(KeyData & Keys.KeyCode);

        /// <summary>
        /// Gets the modifier flags for a
        /// <see cref="Forms.Control.KeyDown" />
        /// or
        /// <see cref="Forms.Control.KeyUp" /> event.
        /// This indicates which modifier keys (CTRL, SHIFT, and/or ALT) were pressed.
        /// </summary>
        public Keys Modifiers => KeyData & Keys.Modifiers;

        /// <summary>
        /// Gets a value indicating whether the SHIFT key was pressed.
        /// </summary>
        /// <returns>true if the SHIFT key was pressed; otherwise, false.
        /// </returns>
        public bool Shift => (KeyData & Keys.Shift) == Keys.Shift;
        /// <summary>
        /// Gets or sets a value indicating whether a key is a regular input key.
        /// </summary>
        /// <returns>true if the key is a regular input key; otherwise, false.
        /// </returns>
        public bool IsInputKey { get; set; }
    }
}
