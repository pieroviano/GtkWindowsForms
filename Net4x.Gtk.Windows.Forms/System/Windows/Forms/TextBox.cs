/*
 * Based on GTK components, it is a cross-platform interface component that is compatible with the native C# control winform interface.
 * Use this component GtkSystem.Windows.Forms instead of Microsoft.WindowsDesktop.App.WindowsForms, compile once, and run across Windows, Linux, and macOS
 * Technical support 438865652@qq.com，https://www.gtkapp.com, https://gitee.com/easywebfactory, https://github.com/easywebfactory438865652@qq.com，https://www.gtkapp.com, https://gitee.com/easywebfactory, https://github.com/easywebfactory
 * author:chenhongjin
 */
using System.ComponentModel;
using Gtk;
using GtkSystem.Windows.Forms.GtkControls.ControlBase;
using GtkTextBoxBase = GtkSystem.Windows.Forms.GtkControls.ControlBase.TextBoxBase;

namespace System.Windows.Forms;

/// <summary>
/// Represents a Windows text box control.
/// </summary>
[DesignerCategory("Component")]
public partial class TextBox : TextBoxBase
{
    /// <summary>
    /// The self
    /// </summary>
    public readonly GtkTextBoxBase self = new();

    /// <summary>
    /// Occurs when a key is pressed while the control has focus.
    /// </summary>
    public override event KeyEventHandler? KeyDown;

    private HorizontalAlignment textAlign;

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.TextBox" /> class.
    /// </summary>
    public TextBox() : base()
    {
        self.Override.sender = this;
        self.MaxWidthChars = 1;
        self.WidthChars = 0;
        self.Valign = Gtk.Align.Start;
        self.Halign = Gtk.Align.Start;
        self.Changed += Self_Changed;
        self.TextInserted += Self_TextInserted;
        self.KeyPressEvent += Self_KeyPressEvent;
    }

    /// <summary>
    /// Gets the GTK control.
    /// </summary>
    /// <value>
    /// The GTK control.
    /// </value>
    public override object GtkControl => self;

    /// <summary>
    /// Gets or sets the lines.
    /// </summary>
    /// <value>
    /// The lines.
    /// </value>
    public override string[] Lines
    {
        get
        {
            return [self.Text];
        }
        set
        {
            if (value is null || value.Length == 0)
            {
                Text = string.Empty;
            }
            else
            {
                Text = string.Join(string.Empty, value);
            }
        }
    }

    /// <summary>
    /// Gets or sets the maximum length.
    /// </summary>
    /// <value>
    /// The maximum length.
    /// </value>
    public int MaxLength
    {
        get => self.MaxLength;
        set => self.MaxLength = value;
    }

    /// <summary>
    /// Gets or sets a value indicating whether this
    /// <see cref="T:System.Windows.Forms.TextBox" /> is multiline.
    /// </summary>
    /// <value>
    /// <c>
    /// true
    /// </c> if multiline; otherwise,
    /// <c>
    /// false
    /// </c>.
    /// </value>
    public override bool Multiline
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the password character.
    /// </summary>
    /// <value>
    /// The password character.
    /// </value>
    public virtual char PasswordChar
    {
        get => self.InvisibleChar;
        set { self.InvisibleChar = value; self.Visibility = false; }
    }

    /// <summary>
    /// Gets or sets the placeholder text.
    /// </summary>
    /// <value>
    /// The placeholder text.
    /// </value>
    public string PlaceholderText
    {
        get { return self.PlaceholderText; }
        set { self.PlaceholderText = value ?? ""; }
    }

    /// <summary>
    /// Gets or sets a value indicating whether read only.
    /// </summary>
    /// <value>
    /// <c>
    /// true
    /// </c> if read only; otherwise,
    /// <c>
    /// false
    /// </c>.
    /// </value>
    public override bool ReadOnly
    {
        get { return self.IsEditable == false; }
        set
        {
            var changed = ReadOnly != value;
            self.IsEditable = value == false;
            if (changed)
            {
                OnReadOnlyChanged(EventArgs.Empty);
            }
        }
    }

    /// <summary>
    /// Gets or sets the currently selected text.
    /// </summary>
    /// <value>
    /// The selected text.
    /// </value>
    public override string SelectedText
    {
        get
        {
            if (self.GetSelectionBounds(out var start, out var end))
                return self.Text.Substring(start, end - start);
            else
                return string.Empty;
        }
        set
        {
            PushUndo();
            ReplaceSelectedTextCore(value ?? string.Empty);
            MarkModified();
        }
    }

    /// <summary>
    /// Gets or sets the length of the selection.
    /// </summary>
    /// <value>
    /// The length of the selection.
    /// </value>
    public override int SelectionLength
    {
        get
        {
            self.GetSelectionBounds(out var start, out var end);

            return end - start;
        }
        set
        {
            Select(SelectionStart, value);
        }
    }

    /// <summary>
    /// Gets or sets the selection start.
    /// </summary>
    /// <value>
    /// The selection start.
    /// </value>
    public override int SelectionStart
    {
        get
        {
            self.GetSelectionBounds(out var start, out var end);

            return start;
        }
        set { Select(value, SelectionLength); }
    }

    /// <summary>
    /// Gets or sets the text.
    /// </summary>
    /// <value>
    /// The text.
    /// </value>
    public override string Text
    {
        get { return self.Text; }
        set { SetTextCore(() => self.Text = value ?? string.Empty); }
    }

    /// <summary>
    /// Gets or sets how text is aligned in the control.
    /// </summary>
    /// <value>
    /// The text alignment.
    /// </value>
    public HorizontalAlignment TextAlign
    {
        get { return textAlign; }
        set
        {
            textAlign = value;
            if (value == HorizontalAlignment.Left)
            {
                self.Xalign = 0.0f;
            }
            else if (value == HorizontalAlignment.Center)
            {
                self.Xalign = 0.5f;
            }
            else if (value == HorizontalAlignment.Right)
            {
                self.Xalign = 1.0f;
            }
        }
    }

    /// <summary>
    /// Clears the current selection.
    /// </summary>
    public override void DeselectAll()
    {
        SelectionLength = 0;
    }

    /// <summary>
    /// Selects all the text in the control.
    /// </summary>
    public override void SelectAll()
    {
        Select(0, -1);
    }

    /// <summary>
    /// Replaces the current selection (or inserts at the caret when nothing is selected) with
    /// <paramref name="text" />.
    /// </summary>
    /// <param name="text">The replacement text.</param>
    protected override void ReplaceSelectedTextCore(string text)
    {
        int pos;
        if (self.GetSelectionBounds(out var start, out var end))
        {
            self.DeleteText(start, end);
            pos = start;
        }
        else
        {
            pos = self.Position;
        }

        self.InsertText(text ?? string.Empty, ref pos);
        self.Position = pos;
    }

    /// <summary>
    /// Appends <paramref name="text" /> at the end of the entry.
    /// </summary>
    /// <param name="text">The text to append.</param>
    protected override void AppendTextCore(string text)
    {
        var pos = self.Text?.Length ?? 0;
        self.InsertText(text, ref pos);
        self.Position = pos;
    }

    /// <summary>
    /// Inserts the text at cursor.
    /// </summary>
    /// <param name="text">
    /// The text.
    /// </param>
    public void InsertTextAtCursor(string text)
    {
        if (text == null) return;
        var posi = self.CursorPosition;
        self.InsertText(text, ref posi);
    }

    /// <summary>
    /// Selects the specified start.
    /// </summary>
    /// <param name="start">
    /// The start.
    /// </param>
    /// <param name="length">
    /// The length.
    /// </param>
    public override void Select(int start, int length)
    {
        self.SelectRegion(start, start + length);
    }

    /// <summary>
    /// Raises the KeyDown event.
    /// </summary>
    protected override void OnKeyDown(System.Windows.Forms.KeyEventArgs e)
    {
        KeyDown?.Invoke(this, e);
    }

    private void Self_Changed(object sender, EventArgs e)
    {
        // TextChanged used to be suppressed while the widget was not visible, so bindings with
        // DataSourceUpdateMode.OnPropertyChanged (and any logic keyed on TextChanged before the
        // form is shown) never heard programmatic changes. WinForms raises it unconditionally.
        OnTextChanged(EventArgs.Empty);
    }

    private void Self_KeyPressEvent(object o, Gtk.KeyPressEventArgs args)
    {
        if (KeyDown != null)
        {
            if (args.Event is Gdk.EventKey eventkey)
            {
                var keys = (Keys)eventkey.HardwareKeycode;
                OnKeyDown(new KeyEventArgs(keys));
            }
        }
    }

    private void Self_TextInserted(object o, TextInsertedArgs args)
    {
        if (KeyDown != null && this.GetType().Name == "TextBox")
        {
            var keytext = args.NewText.ToUpper();
            if (char.IsNumber(args.NewText[0]))
                keytext = "D" + keytext;
            var keyv = Enum.GetValues(typeof(Keys)).Cast<Keys>().Where(k =>
            {
                return Enum.GetName(typeof(Keys), k) == keytext;
            });
            foreach (var key in keyv)
                OnKeyDown(new KeyEventArgs(key));
        }
    }
}