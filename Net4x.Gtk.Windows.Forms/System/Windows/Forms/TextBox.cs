/*
 * Based on GTK components, it is a cross-platform interface component that is compatible with the native C# control winform interface.
 * Use this component GtkSystem.Windows.Forms instead of Microsoft.WindowsDesktop.App.WindowsForms, compile once, and run across Windows, Linux, and macOS
 * Technical support 438865652@qq.com，https://www.gtkapp.com, https://gitee.com/easywebfactory, https://github.com/easywebfactory438865652@qq.com，https://www.gtkapp.com, https://gitee.com/easywebfactory, https://github.com/easywebfactory
 * author:chenhongjin
 */
using System.ComponentModel;
using Atk;
using GLib;
using Gtk;
using GtkSystem.Windows.Forms.GtkControls.ControlBase;

namespace System.Windows.Forms;

/// <summary>
/// Represents a Windows text box control.
/// </summary>
[DesignerCategory("Component")]
public partial class TextBox : Control
{
    /// <summary>
    /// The self
    /// </summary>
    public readonly TextBoxBase self = new TextBoxBase();

    /// <summary>
    /// Occurs when a key is pressed while the control has focus.
    /// </summary>
    public override event KeyEventHandler KeyDown;

    /// <summary>
    /// Occurs when the Text property value changes.
    /// </summary>
    public override event EventHandler TextChanged;

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.TextBox" /> class.
    /// </summary>
    public TextBox() : base()
    {
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
    public string[] Lines
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
    public bool Multiline
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
    public virtual bool ReadOnly
    {
        get { return self.IsEditable == false; }
        set { self.IsEditable = value == false; }
    }

    /// <summary>
    /// Gets or sets the length of the selection.
    /// </summary>
    /// <value>
    /// The length of the selection.
    /// </value>
    [System.ComponentModel.Browsable(false)]
    public virtual int SelectionLength
    {
        get
        {
            self.GetSelectionBounds(out int start, out int end);

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
    public int SelectionStart
    {
        get
        {
            self.GetSelectionBounds(out int start, out int end);

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
        set { self.Text = value ?? ""; }
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
        int posi = self.CursorPosition;
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
    public void Select(int start, int length)
    {
        self.SelectRegion(start, start + length);
    }

    /// <summary>
    /// Raises the KeyDown event.
    /// </summary>
    protected virtual void OnKeyDown(System.Windows.Forms.KeyEventArgs e) { KeyDown?.Invoke(this, e); }

    /// <summary>
    /// Raises the TextChanged event.
    /// </summary>
    protected virtual void OnTextChanged(System.EventArgs e) { TextChanged?.Invoke(this, e); }

    private void Self_Changed(object sender, EventArgs e)
    {
        if (TextChanged != null && self.IsVisible) { TextChanged(this, EventArgs.Empty); }
    }

    private void Self_KeyPressEvent(object o, Gtk.KeyPressEventArgs args)
    {
        if (KeyDown != null)
        {
            if (args.Event is Gdk.EventKey eventkey)
            {
                var keys = (Keys)eventkey.HardwareKeycode;
                KeyDown(this, new KeyEventArgs(keys));
            }
        }
    }

    private void Self_TextInserted(object o, TextInsertedArgs args)
    {
        if (KeyDown != null && this.GetType().Name == "TextBox")
        {
            string keytext = args.NewText.ToUpper();
            if (char.IsNumber(args.NewText[0]))
                keytext = "D" + keytext;
            var keyv = Enum.GetValues(typeof(Keys)).Cast<Keys>().Where(k =>
            {
                return Enum.GetName(typeof(Keys), k) == keytext;
            });
            foreach (var key in keyv)
                KeyDown(this, new KeyEventArgs(key));
        }
    }

}