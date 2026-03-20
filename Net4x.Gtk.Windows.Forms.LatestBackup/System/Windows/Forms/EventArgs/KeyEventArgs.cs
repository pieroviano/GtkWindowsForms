// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;

namespace System.Windows.Forms;

/// <summary>
/// Provides data for the
/// <see cref="T:System.Windows.Forms.Control.KeyDown" /> or
/// <see cref="T:System.Windows.Forms.Control.KeyUp" /> event.
/// </summary>
[ComVisible(true)]
public class KeyEventArgs : EventArgs
{
    private bool _suppressKeyPress = false;

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.KeyEventArgs" /> class.
    /// </summary>
    /// <param name="keyData">
    /// The key data.
    /// </param>
    public KeyEventArgs(Keys keyData)
    {
        KeyData = keyData;
    }

    /// <summary>
    /// Gets a value indicating whether the ALT key was pressed.
    /// </summary>
    /// <value>
    /// <c>
    /// true
    /// </c> if alt; otherwise,
    /// <c>
    /// false
    /// </c>.
    /// </value>
    public virtual bool Alt => (KeyData & Keys.Alt) == Keys.Alt;

    /// <summary>
    /// Gets a value indicating whether the CTRL key was pressed.
    /// </summary>
    /// <value>
    /// <c>
    /// true
    /// </c> if control; otherwise,
    /// <c>
    /// false
    /// </c>.
    /// </value>
    public bool Control => (KeyData & Keys.Control) == Keys.Control;

    /// <summary>
    /// Gets or sets a value indicating whether the event was handled.
    /// </summary>
    /// <value>
    /// <c>
    /// true
    /// </c> if handled; otherwise,
    /// <c>
    /// false
    /// </c>.
    /// </value>
    public bool Handled
    {
        get;
        set;
    }

    /// <summary>
    /// Gets the keyboard code for a
    /// <see cref="T:System.Windows.Forms.Control.KeyDown" /> or
    /// <see cref="T:System.Windows.Forms.Control.KeyUp" /> event.
    /// </summary>
    /// <value>
    /// The key code.
    /// </value>
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
    /// <see cref="T:System.Windows.Forms.Control.KeyDown" /> or
    /// <see cref="T:System.Windows.Forms.Control.KeyUp" /> event.
    /// </summary>
    /// <value>
    /// The key data.
    /// </value>
    public Keys KeyData { get; }

    /// <summary>
    /// Gets the keyboard value for a
    /// <see cref="T:System.Windows.Forms.Control.KeyDown" /> or
    /// <see cref="T:System.Windows.Forms.Control.KeyUp" /> event.
    /// </summary>
    /// <value>
    /// The key value.
    /// </value>
    public int KeyValue => (int)(KeyData & Keys.KeyCode);

    /// <summary>
    /// Gets the modifier flags for a
    /// <see cref="T:System.Windows.Forms.Control.KeyDown" /> or
    /// <see cref="T:System.Windows.Forms.Control.KeyUp" /> event.
    /// This indicates which modifier keys (CTRL, SHIFT, and/or ALT) were pressed.
    /// </summary>
    /// <value>
    /// The modifiers.
    /// </value>
    public Keys Modifiers => KeyData & Keys.Modifiers;

    /// <summary>
    /// Gets a value indicating whether the SHIFT key was pressed.
    /// </summary>
    /// <value>
    /// <c>
    /// true
    /// </c> if shift; otherwise,
    /// <c>
    /// false
    /// </c>.
    /// </value>
    public virtual bool Shift => (KeyData & Keys.Shift) == Keys.Shift;

    /// <summary>
    /// Gets or sets a value indicating whether suppress key press.
    /// </summary>
    /// <value>
    /// <c>
    /// true
    /// </c> if suppress key press; otherwise,
    /// <c>
    /// false
    /// </c>.
    /// </value>
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