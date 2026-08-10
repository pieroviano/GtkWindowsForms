/*
 * Based on GTK components, it is a cross-platform interface component that is compatible with the native C# control winform interface.
 * Use this component GtkSystem.Windows.Forms instead of Microsoft.WindowsDesktop.App.WindowsForms, compile once, and run across Windows, Linux, and macOS
 */

using System.Collections.Generic;
using System.ComponentModel;

namespace System.Windows.Forms;

/// <summary>
/// Implements the basic functionality required by text controls
/// (<see cref="T:System.Windows.Forms.TextBox" />, <see cref="T:System.Windows.Forms.RichTextBox" /> and
/// <see cref="T:System.Windows.Forms.MaskedTextBox" />). This mirrors the real
/// <c>System.Windows.Forms.TextBoxBase</c> so that source written against the shared editing surface — and
/// type tests such as <c>control is TextBoxBase</c> — compile and behave.
/// </summary>
/// <remarks>
/// The concrete text storage differs per control (a GTK entry versus a text view), so the primitives that
/// touch the widget are abstract and supplied by the derived control; the clipboard, undo and modified-state
/// behaviour is implemented once here on top of them.
/// <para>
/// Undo covers the content-editing operations routed through this class
/// (<see cref="M:System.Windows.Forms.TextBoxBase.AppendText(System.String)" />,
/// <see cref="M:System.Windows.Forms.TextBoxBase.Cut" />,
/// <see cref="M:System.Windows.Forms.TextBoxBase.Paste" />,
/// <see cref="M:System.Windows.Forms.TextBoxBase.Clear" /> and setting
/// <see cref="P:System.Windows.Forms.TextBoxBase.SelectedText" />). It does not capture individual keystrokes,
/// which GTK handles internally.
/// </para>
/// </remarks>
[DesignerCategory("Component")]
public abstract class TextBoxBase : Control
{
    private readonly Stack<string> _undo = new();
    private bool _modified;
    private bool _acceptsTab;
    private bool _hideSelection = true;
    private bool _wordWrap = true;
    private bool _shortcutsEnabled = true;

    /// <summary>
    /// When set, a resulting text-change notification is treated as a programmatic assignment of the
    /// <see cref="P:System.Windows.Forms.TextBoxBase.Text" /> property rather than a content edit, so it does
    /// not raise <see cref="P:System.Windows.Forms.TextBoxBase.Modified" />.
    /// </summary>
    private bool _inTextAssignment;

    /// <summary>
    /// Occurs when the <see cref="P:System.Windows.Forms.TextBoxBase.Modified" /> property changes.
    /// </summary>
    public event EventHandler? ModifiedChanged;

    /// <summary>
    /// Occurs when the <see cref="P:System.Windows.Forms.TextBoxBase.ReadOnly" /> property changes.
    /// </summary>
    public event EventHandler? ReadOnlyChanged;

    /// <summary>
    /// Occurs when the <see cref="P:System.Windows.Forms.TextBoxBase.Multiline" /> property changes.
    /// </summary>
    public event EventHandler? MultilineChanged;

    /// <summary>
    /// Occurs when the text alignment changes.
    /// </summary>
    public event EventHandler? TextAlignChanged;

    /// <summary>
    /// Occurs when the <see cref="P:System.Windows.Forms.TextBoxBase.BorderStyle" /> property changes.
    /// </summary>
    public event EventHandler? BorderStyleChanged;

    /// <summary>
    /// Occurs when the <see cref="P:System.Windows.Forms.TextBoxBase.AcceptsTab" /> property changes.
    /// </summary>
    public event EventHandler? AcceptsTabChanged;

    /// <summary>
    /// Occurs when the <see cref="P:System.Windows.Forms.TextBoxBase.HideSelection" /> property changes.
    /// </summary>
    public event EventHandler? HideSelectionChanged;

    /// <summary>
    /// Gets or sets the text associated with this control.
    /// </summary>
    public abstract override string Text { get; set; }

    /// <summary>
    /// Gets or sets the lines of text in the control.
    /// </summary>
    public abstract string[] Lines { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether text in the control is read-only.
    /// </summary>
    public abstract bool ReadOnly { get; set; }

    /// <summary>
    /// Gets or sets a value indicating which characters are selected in the control.
    /// </summary>
    public abstract string SelectedText { get; set; }

    /// <summary>
    /// Gets or sets the number of characters selected in the control.
    /// </summary>
    public abstract int SelectionLength { get; set; }

    /// <summary>
    /// Gets or sets the starting point of text selected in the control.
    /// </summary>
    public abstract int SelectionStart { get; set; }

    /// <summary>
    /// Selects a range of text in the control.
    /// </summary>
    /// <param name="start">The position of the first character in the current text selection.</param>
    /// <param name="length">The number of characters to select.</param>
    public abstract void Select(int start, int length);

    /// <summary>
    /// Selects all text in the control.
    /// </summary>
    public abstract void SelectAll();

    /// <summary>
    /// Clears all selection, setting the selected text to an empty string.
    /// </summary>
    public abstract void DeselectAll();

    /// <summary>
    /// Replaces the currently selected text (or inserts at the caret when nothing is selected) with
    /// <paramref name="text" />. Implemented per widget; used by cut and paste.
    /// </summary>
    /// <param name="text">The replacement text.</param>
    protected abstract void ReplaceSelectedTextCore(string text);

    /// <summary>
    /// Gets the length of text in the control.
    /// </summary>
    public virtual int TextLength => Text?.Length ?? 0;

    /// <summary>
    /// Gets or sets a value indicating whether this is a multiline control. Overridden where a control
    /// supports multiple lines.
    /// </summary>
    public virtual bool Multiline
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets a value indicating whether the control modifies the case of characters as they are typed.
    /// </summary>
    public bool Modified
    {
        get => _modified;
        set
        {
            if (_modified != value)
            {
                _modified = value;
                OnModifiedChanged(EventArgs.Empty);
            }
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether pressing the TAB key types a TAB character in the control
    /// instead of moving focus.
    /// </summary>
    public virtual bool AcceptsTab
    {
        get => _acceptsTab;
        set
        {
            if (_acceptsTab != value)
            {
                _acceptsTab = value;
                OnAcceptsTabChanged(EventArgs.Empty);
            }
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether the selected text remains highlighted when the control loses
    /// focus.
    /// </summary>
    public virtual bool HideSelection
    {
        get => _hideSelection;
        set
        {
            if (_hideSelection != value)
            {
                _hideSelection = value;
                OnHideSelectionChanged(EventArgs.Empty);
            }
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether text wraps onto the next line when it reaches the edge of a
    /// multiline control.
    /// </summary>
    public virtual bool WordWrap
    {
        get => _wordWrap;
        set => _wordWrap = value;
    }

    /// <summary>
    /// Gets or sets a value indicating whether the defined shortcuts are enabled.
    /// </summary>
    public virtual bool ShortcutsEnabled
    {
        get => _shortcutsEnabled;
        set => _shortcutsEnabled = value;
    }

    /// <summary>
    /// Gets or sets the border style of the control.
    /// </summary>
    public override BorderStyle BorderStyle
    {
        get => base.BorderStyle;
        set
        {
            var changed = base.BorderStyle != value;
            base.BorderStyle = value;
            if (changed)
            {
                OnBorderStyleChanged(EventArgs.Empty);
            }
        }
    }

    /// <summary>
    /// Gets a value indicating whether the user can undo the previous operation in the control.
    /// </summary>
    public bool CanUndo => _undo.Count > 0;

    /// <summary>
    /// Appends text to the current text of the control.
    /// </summary>
    /// <param name="text">The text to append.</param>
    public virtual void AppendText(string text)
    {
        if (string.IsNullOrEmpty(text))
        {
            return;
        }

        PushUndo();
        // Route through the widget rather than the Text setter so this registers as a content edit
        // (setting Modified), not a programmatic re-assignment.
        AppendTextCore(text);
        MarkModified();
    }

    /// <summary>
    /// Appends <paramref name="text" /> to the underlying widget. Overridden per control; the default
    /// concatenates onto <see cref="P:System.Windows.Forms.TextBoxBase.Text" />.
    /// </summary>
    /// <param name="text">The text to append.</param>
    protected virtual void AppendTextCore(string text)
    {
        SetTextInternal((Text ?? string.Empty) + text);
    }

    /// <summary>
    /// Clears all text from the control.
    /// </summary>
    public virtual void Clear()
    {
        PushUndo();
        Text = string.Empty;
    }

    /// <summary>
    /// Clears information about the most recent operation from the undo buffer of the control.
    /// </summary>
    public virtual void ClearUndo() => _undo.Clear();

    /// <summary>
    /// Copies the current selection in the control to the Clipboard.
    /// </summary>
    public virtual void Copy()
    {
        var selected = SelectedText;
        if (!string.IsNullOrEmpty(selected))
        {
            Gtk4Compat.SetClipboardText(selected);
        }
    }

    /// <summary>
    /// Moves the current selection in the control to the Clipboard.
    /// </summary>
    public virtual void Cut()
    {
        var selected = SelectedText;
        if (!string.IsNullOrEmpty(selected))
        {
            Gtk4Compat.SetClipboardText(selected);
            PushUndo();
            ReplaceSelectedTextCore(string.Empty);
            MarkModified();
        }
    }

    /// <summary>
    /// Replaces the current selection in the control with the contents of the Clipboard.
    /// </summary>
    public virtual void Paste()
    {
        var text = Gtk4Compat.GetClipboardTextSync();
        if (!string.IsNullOrEmpty(text))
        {
            PushUndo();
            ReplaceSelectedTextCore(text);
            MarkModified();
        }
    }

    /// <summary>
    /// Undoes the last edit operation captured in the undo buffer of the control.
    /// </summary>
    public virtual void Undo()
    {
        if (_undo.Count > 0)
        {
            Text = _undo.Pop();
        }
    }

    /// <summary>
    /// Captures the current text so a later <see cref="M:System.Windows.Forms.TextBoxBase.Undo" /> can
    /// restore it.
    /// </summary>
    protected void PushUndo() => _undo.Push(Text ?? string.Empty);

    /// <summary>
    /// Marks the control's contents as modified, raising
    /// <see cref="E:System.Windows.Forms.TextBoxBase.ModifiedChanged" /> on the transition.
    /// </summary>
    protected void MarkModified() => Modified = true;

    /// <summary>
    /// Assigns the text of the control as a programmatic assignment: the resulting change notification does
    /// not flip <see cref="P:System.Windows.Forms.TextBoxBase.Modified" />, matching WinForms, where setting
    /// the <c>Text</c> property clears the modified state.
    /// </summary>
    /// <param name="assign">The action that writes the value into the underlying widget.</param>
    protected void SetTextCore(Action assign)
    {
        _inTextAssignment = true;
        try
        {
            assign();
        }
        finally
        {
            _inTextAssignment = false;
        }

        Modified = false;
    }

    /// <summary>
    /// Writes <paramref name="value" /> into the control as a content edit (updating the modified state).
    /// Overridden per control; the default routes through the <see cref="P:System.Windows.Forms.TextBoxBase.Text" />
    /// setter but keeps the change registered as an edit.
    /// </summary>
    /// <param name="value">The new text.</param>
    protected virtual void SetTextInternal(string value)
    {
        Text = value;
        MarkModified();
    }

    /// <summary>
    /// Raises the <see cref="E:System.Windows.Forms.Control.TextChanged" /> event and, unless the change was a
    /// programmatic <see cref="P:System.Windows.Forms.TextBoxBase.Text" /> assignment, marks the control
    /// modified.
    /// </summary>
    /// <param name="e">An <see cref="T:System.EventArgs" /> that contains the event data.</param>
    protected override void OnTextChanged(EventArgs e)
    {
        if (!_inTextAssignment)
        {
            Modified = true;
        }

        base.OnTextChanged(e);
    }

    /// <summary>
    /// Raises the <see cref="E:System.Windows.Forms.TextBoxBase.ModifiedChanged" /> event.
    /// </summary>
    /// <param name="e">An <see cref="T:System.EventArgs" /> that contains the event data.</param>
    protected virtual void OnModifiedChanged(EventArgs e) => ModifiedChanged?.Invoke(this, e);

    /// <summary>
    /// Raises the <see cref="E:System.Windows.Forms.TextBoxBase.ReadOnlyChanged" /> event.
    /// </summary>
    /// <param name="e">An <see cref="T:System.EventArgs" /> that contains the event data.</param>
    protected virtual void OnReadOnlyChanged(EventArgs e) => ReadOnlyChanged?.Invoke(this, e);

    /// <summary>
    /// Raises the <see cref="E:System.Windows.Forms.TextBoxBase.MultilineChanged" /> event.
    /// </summary>
    /// <param name="e">An <see cref="T:System.EventArgs" /> that contains the event data.</param>
    protected virtual void OnMultilineChanged(EventArgs e) => MultilineChanged?.Invoke(this, e);

    /// <summary>
    /// Raises the <see cref="E:System.Windows.Forms.TextBoxBase.TextAlignChanged" /> event.
    /// </summary>
    /// <param name="e">An <see cref="T:System.EventArgs" /> that contains the event data.</param>
    protected virtual void OnTextAlignChanged(EventArgs e) => TextAlignChanged?.Invoke(this, e);

    /// <summary>
    /// Raises the <see cref="E:System.Windows.Forms.TextBoxBase.BorderStyleChanged" /> event.
    /// </summary>
    /// <param name="e">An <see cref="T:System.EventArgs" /> that contains the event data.</param>
    protected virtual void OnBorderStyleChanged(EventArgs e) => BorderStyleChanged?.Invoke(this, e);

    /// <summary>
    /// Raises the <see cref="E:System.Windows.Forms.TextBoxBase.AcceptsTabChanged" /> event.
    /// </summary>
    /// <param name="e">An <see cref="T:System.EventArgs" /> that contains the event data.</param>
    protected virtual void OnAcceptsTabChanged(EventArgs e) => AcceptsTabChanged?.Invoke(this, e);

    /// <summary>
    /// Raises the <see cref="E:System.Windows.Forms.TextBoxBase.HideSelectionChanged" /> event.
    /// </summary>
    /// <param name="e">An <see cref="T:System.EventArgs" /> that contains the event data.</param>
    protected virtual void OnHideSelectionChanged(EventArgs e) => HideSelectionChanged?.Invoke(this, e);
}
