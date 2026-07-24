// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;

namespace System.Windows.Forms;

/// <summary>
/// Keyboard-command routing, right-to-left alignment translation and parent-change notification override
/// points that custom controls ported from WinForms rely on. These mirror the real
/// <c>System.Windows.Forms.Control</c> members so overriding source compiles and behaves.
/// </summary>
public partial class Control
{
    // Windows message identifiers used by the key-processing chain.
    private const int WM_KEYDOWN = 0x0100;
    private const int WM_KEYUP = 0x0101;
    private const int WM_CHAR = 0x0102;
    private const int WM_SYSKEYDOWN = 0x0104;
    private const int WM_SYSKEYUP = 0x0105;
    private const int WM_SYSCHAR = 0x0106;

    /// <summary>
    /// Processes a command key. Override to intercept a shortcut before it reaches the focused control; the
    /// default implementation forwards to the parent so a container can handle keys for its children.
    /// </summary>
    /// <param name="msg">A <see cref="T:System.Windows.Forms.Message" />, passed by reference, that represents the message to process.</param>
    /// <param name="keyData">One of the <see cref="T:System.Windows.Forms.Keys" /> values that represents the key to process.</param>
    /// <returns><see langword="true" /> if the key was processed; otherwise <see langword="false" />.</returns>
    protected virtual bool ProcessCmdKey(ref Message msg, Keys keyData)
        => Parent != null && Parent.ProcessCmdKey(ref msg, keyData);

    /// <summary>Processes a dialog key. The default implementation forwards to the parent.</summary>
    /// <param name="keyData">The key to process.</param>
    /// <returns><see langword="true" /> if the key was processed; otherwise <see langword="false" />.</returns>
    protected virtual bool ProcessDialogKey(Keys keyData)
        => Parent != null && Parent.ProcessDialogKey(keyData);

    /// <summary>Processes a dialog character. The default implementation forwards to the parent.</summary>
    /// <param name="charCode">The character to process.</param>
    /// <returns><see langword="true" /> if the character was processed; otherwise <see langword="false" />.</returns>
    protected virtual bool ProcessDialogChar(char charCode)
        => Parent != null && Parent.ProcessDialogChar(charCode);

    /// <summary>Processes a mnemonic character. The default implementation returns <see langword="false" />.</summary>
    /// <param name="charCode">The mnemonic character to process.</param>
    /// <returns><see langword="true" /> if the mnemonic was consumed; otherwise <see langword="false" />.</returns>
    protected internal virtual bool ProcessMnemonic(char charCode) => false;

    /// <summary>
    /// Previews a key message. The default implementation forwards to the parent, giving a container the
    /// first chance to inspect the key.
    /// </summary>
    /// <param name="m">A <see cref="T:System.Windows.Forms.Message" />, passed by reference, that represents the message to process.</param>
    /// <returns><see langword="true" /> if the message was processed; otherwise <see langword="false" />.</returns>
    protected virtual bool ProcessKeyPreview(ref Message m)
        => Parent != null && Parent.ProcessKeyPreview(ref m);

    /// <summary>
    /// Processes a key message by raising the appropriate key event
    /// (<see cref="M:System.Windows.Forms.Control.OnKeyDown(System.Windows.Forms.KeyEventArgs)" />,
    /// <see cref="M:System.Windows.Forms.Control.OnKeyPress(System.Windows.Forms.KeyPressEventArgs)" /> or
    /// <see cref="M:System.Windows.Forms.Control.OnKeyUp(System.Windows.Forms.KeyEventArgs)" />).
    /// </summary>
    /// <param name="m">A <see cref="T:System.Windows.Forms.Message" />, passed by reference, that represents the message to process.</param>
    /// <returns><see langword="true" /> if the message was handled by an event; otherwise <see langword="false" />.</returns>
    protected virtual bool ProcessKeyEventArgs(ref Message m)
    {
        switch (m.Msg)
        {
            case WM_KEYDOWN:
            case WM_SYSKEYDOWN:
                var down = new KeyEventArgs((Keys)(int)m.WParam);
                OnKeyDown(down);
                return down.Handled;
            case WM_KEYUP:
            case WM_SYSKEYUP:
                var up = new KeyEventArgs((Keys)(int)m.WParam);
                OnKeyUp(up);
                return up.Handled;
            case WM_CHAR:
            case WM_SYSCHAR:
                var press = new KeyPressEventArgs((char)(int)m.WParam);
                OnKeyPress(press);
                return press.Handled;
            default:
                return false;
        }
    }

    /// <summary>
    /// Processes a keyboard message, giving the parent a preview first and then raising the key events.
    /// </summary>
    /// <param name="m">A <see cref="T:System.Windows.Forms.Message" />, passed by reference, that represents the message to process.</param>
    /// <returns><see langword="true" /> if the message was processed; otherwise <see langword="false" />.</returns>
    protected internal virtual bool ProcessKeyMessage(ref Message m)
    {
        if (Parent != null && Parent.ProcessKeyPreview(ref m))
        {
            return true;
        }

        return ProcessKeyEventArgs(ref m);
    }

    /// <summary>
    /// Determines whether the specified key is a regular input key that the control handles itself rather
    /// than a special key handled by dialog processing. The default implementation returns
    /// <see langword="false" />.
    /// </summary>
    /// <param name="keyData">One of the <see cref="T:System.Windows.Forms.Keys" /> values.</param>
    /// <returns><see langword="true" /> if the key is a regular input key; otherwise <see langword="false" />.</returns>
    protected virtual bool IsInputKey(Keys keyData) => false;

    /// <summary>
    /// Determines whether a character is an input character that the control recognizes. The default
    /// implementation returns <see langword="true" />.
    /// </summary>
    /// <param name="charCode">The character to test.</param>
    /// <returns><see langword="true" /> if the character is an input character; otherwise <see langword="false" />.</returns>
    protected virtual bool IsInputChar(char charCode) => true;

    /// <summary>Converts the specified <see cref="T:System.Windows.Forms.HorizontalAlignment" /> when the control's layout is right-to-left.</summary>
    /// <param name="align">The alignment to convert.</param>
    /// <returns>The converted alignment.</returns>
    protected HorizontalAlignment RtlTranslateAlignment(HorizontalAlignment align) => RtlTranslateHorizontal(align);

    /// <summary>Converts the specified <see cref="T:System.Windows.Forms.LeftRightAlignment" /> when the control's layout is right-to-left.</summary>
    /// <param name="align">The alignment to convert.</param>
    /// <returns>The converted alignment.</returns>
    protected LeftRightAlignment RtlTranslateAlignment(LeftRightAlignment align) => RtlTranslateLeftRight(align);

    /// <summary>Converts the specified <see cref="T:System.Drawing.ContentAlignment" /> when the control's layout is right-to-left.</summary>
    /// <param name="align">The alignment to convert.</param>
    /// <returns>The converted alignment.</returns>
    protected ContentAlignment RtlTranslateAlignment(ContentAlignment align) => RtlTranslateContent(align);

    /// <summary>Converts the specified <see cref="T:System.Windows.Forms.HorizontalAlignment" /> when the control's layout is right-to-left.</summary>
    /// <param name="align">The alignment to convert.</param>
    /// <returns>The converted alignment.</returns>
    protected HorizontalAlignment RtlTranslateHorizontal(HorizontalAlignment align)
    {
        if (RightToLeft == RightToLeft.Yes)
        {
            if (align == HorizontalAlignment.Left)
            {
                return HorizontalAlignment.Right;
            }

            if (align == HorizontalAlignment.Right)
            {
                return HorizontalAlignment.Left;
            }
        }

        return align;
    }

    /// <summary>Converts the specified <see cref="T:System.Windows.Forms.LeftRightAlignment" /> when the control's layout is right-to-left.</summary>
    /// <param name="align">The alignment to convert.</param>
    /// <returns>The converted alignment.</returns>
    protected LeftRightAlignment RtlTranslateLeftRight(LeftRightAlignment align)
    {
        if (RightToLeft == RightToLeft.Yes)
        {
            return align == LeftRightAlignment.Left ? LeftRightAlignment.Right : LeftRightAlignment.Left;
        }

        return align;
    }

    /// <summary>Converts the specified <see cref="T:System.Drawing.ContentAlignment" /> when the control's layout is right-to-left.</summary>
    /// <param name="align">The alignment to convert.</param>
    /// <returns>The converted alignment.</returns>
    protected ContentAlignment RtlTranslateContent(ContentAlignment align)
    {
        if (RightToLeft != RightToLeft.Yes)
        {
            return align;
        }

        return align switch
        {
            ContentAlignment.TopLeft => ContentAlignment.TopRight,
            ContentAlignment.TopRight => ContentAlignment.TopLeft,
            ContentAlignment.MiddleLeft => ContentAlignment.MiddleRight,
            ContentAlignment.MiddleRight => ContentAlignment.MiddleLeft,
            ContentAlignment.BottomLeft => ContentAlignment.BottomRight,
            ContentAlignment.BottomRight => ContentAlignment.BottomLeft,
            _ => align,
        };
    }

    /// <summary>Raises the <see cref="E:System.Windows.Forms.Control.BackColorChanged" /> event when the parent's back color changes.</summary>
    /// <param name="e">An <see cref="T:System.EventArgs" /> that contains the event data.</param>
    protected virtual void OnParentBackColorChanged(EventArgs e) => OnBackColorChanged(e);

    /// <summary>Raises the <see cref="E:System.Windows.Forms.Control.BackgroundImageChanged" /> event when the parent's background image changes.</summary>
    /// <param name="e">An <see cref="T:System.EventArgs" /> that contains the event data.</param>
    protected virtual void OnParentBackgroundImageChanged(EventArgs e) => OnBackgroundImageChanged(e);

    /// <summary>Raises the <see cref="E:System.Windows.Forms.Control.BindingContextChanged" /> event when the parent's binding context changes.</summary>
    /// <param name="e">An <see cref="T:System.EventArgs" /> that contains the event data.</param>
    protected virtual void OnParentBindingContextChanged(EventArgs e) => OnBindingContextChanged(e);

    /// <summary>Raises the <see cref="E:System.Windows.Forms.Control.CursorChanged" /> event when the parent's cursor changes.</summary>
    /// <param name="e">An <see cref="T:System.EventArgs" /> that contains the event data.</param>
    protected virtual void OnParentCursorChanged(EventArgs e) => OnCursorChanged(e);

    /// <summary>Raises the <see cref="E:System.Windows.Forms.Control.EnabledChanged" /> event when the parent's enabled state changes.</summary>
    /// <param name="e">An <see cref="T:System.EventArgs" /> that contains the event data.</param>
    protected virtual void OnParentEnabledChanged(EventArgs e) => OnEnabledChanged(e);

    /// <summary>Raises the <see cref="E:System.Windows.Forms.Control.FontChanged" /> event when the parent's font changes.</summary>
    /// <param name="e">An <see cref="T:System.EventArgs" /> that contains the event data.</param>
    protected virtual void OnParentFontChanged(EventArgs e) => OnFontChanged(e);

    /// <summary>Raises the <see cref="E:System.Windows.Forms.Control.ForeColorChanged" /> event when the parent's fore color changes.</summary>
    /// <param name="e">An <see cref="T:System.EventArgs" /> that contains the event data.</param>
    protected virtual void OnParentForeColorChanged(EventArgs e) => OnForeColorChanged(e);

    /// <summary>Raises the <see cref="E:System.Windows.Forms.Control.RightToLeftChanged" /> event when the parent's right-to-left value changes.</summary>
    /// <param name="e">An <see cref="T:System.EventArgs" /> that contains the event data.</param>
    protected virtual void OnParentRightToLeftChanged(EventArgs e) => OnRightToLeftChanged(e);

    /// <summary>Raises the <see cref="E:System.Windows.Forms.Control.VisibleChanged" /> event when the parent's visibility changes.</summary>
    /// <param name="e">An <see cref="T:System.EventArgs" /> that contains the event data.</param>
    protected virtual void OnParentVisibleChanged(EventArgs e) => OnVisibleChanged(e);
}
