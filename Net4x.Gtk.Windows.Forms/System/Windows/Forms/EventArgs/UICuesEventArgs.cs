// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms;

/// <summary>
/// Provides data for the
/// <see cref="T:System.Windows.Forms.Control.ChangeUICues" /> event.
/// </summary>
public class UICuesEventArgs : EventArgs
{
    private readonly UICues _uicues;

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.UICuesEventArgs" /> class.
    /// </summary>
    /// <param name="uicues">
    /// The uicues.
    /// </param>
    public UICuesEventArgs(UICues uicues)
    {
        _uicues = uicues;
    }

    /// <summary>
    /// Gets the changed.
    /// </summary>
    /// <value>
    /// The changed.
    /// </value>
    public UICues Changed => _uicues & UICues.Changed;

    /// <summary>
    /// The state of the focus cues has changed.
    /// </summary>
    /// <value>
    /// <c>
    /// true
    /// </c> if change focus; otherwise,
    /// <c>
    /// false
    /// </c>.
    /// </value>
    public bool ChangeFocus => (_uicues & UICues.ChangeFocus) != 0;

    /// <summary>
    /// The state of the keyboard cues has changed.
    /// </summary>
    /// <value>
    /// <c>
    /// true
    /// </c> if change keyboard; otherwise,
    /// <c>
    /// false
    /// </c>.
    /// </value>
    public bool ChangeKeyboard => (_uicues & UICues.ChangeKeyboard) != 0;

    /// <summary>
    /// Focus rectangles are shown after the change.
    /// </summary>
    /// <value>
    /// <c>
    /// true
    /// </c> if show focus; otherwise,
    /// <c>
    /// false
    /// </c>.
    /// </value>
    public bool ShowFocus => (_uicues & UICues.ShowFocus) != 0;

    /// <summary>
    /// Keyboard cues are underlined after the change.
    /// </summary>
    /// <value>
    /// <c>
    /// true
    /// </c> if show keyboard; otherwise,
    /// <c>
    /// false
    /// </c>.
    /// </value>
    public bool ShowKeyboard => (_uicues & UICues.ShowKeyboard) != 0;

}