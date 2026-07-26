/*
 * Based on GTK components, it is a cross-platform interface component that is compatible with the native C# control winform interface.
 * Use this component GtkSystem.Windows.Forms instead of Microsoft.WindowsDesktop.App.WindowsForms, compile once, and run across Windows, Linux, and macOS
 * Technical support 438865652@qq.com，https://www.gtkapp.com, https://gitee.com/easywebfactory, https://github.com/easywebfactory438865652@qq.com，https://www.gtkapp.com, https://gitee.com/easywebfactory, https://github.com/easywebfactory
 * author:chenhongjin
 */
using System.ComponentModel;
using Gtk;
using GtkSystem.Windows.Forms.GtkControls.ControlBase;

namespace System.Windows.Forms;

/// <summary>
/// Represents a Windows rich text box control.
/// </summary>
[DesignerCategory("Component")]
public partial class RichTextBox : TextBoxBase
{
    /// <summary>
    /// The self
    /// </summary>
    public readonly RichTextBoxBase self = new();

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.RichTextBox" /> class.
    /// </summary>
    public RichTextBox() : base()
    {
        self.Override.sender = this;
        self.TextView.Buffer.Changed += Buffer_Changed;
        this.BorderStyle = BorderStyle.Fixed3D;
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
            return self.TextView.Buffer.Text.Split([Environment.NewLine], StringSplitOptions.None);
        }
        set
        {
            if (value is null || value.Length == 0)
            {
                Text = string.Empty;
            }
            else
            {
                Text = string.Join(Environment.NewLine, value);
            }
        }
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
        get { return self.TextView.CanFocus; }
        set
        {
            var changed = self.TextView.CanFocus != value;
            self.TextView.CanFocus = value;
            if (changed)
            {
                OnReadOnlyChanged(EventArgs.Empty);
            }
        }
    }

    /// <summary>
    /// Gets or sets the selected text.
    /// </summary>
    /// <value>
    /// The selected text.
    /// </value>
    public override string SelectedText
    {
        get
        {
            if (self.TextView.Buffer.GetSelectionBounds(out TextIter start, out TextIter end))
                return self.TextView.Buffer.GetText(start, end, false);
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
            self.TextView.Buffer.GetSelectionBounds(out var start, out var end);

            return end.Offset - start.Offset;
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
            self.TextView.Buffer.GetSelectionBounds(out var start, out var end);

            return start.Offset;
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
        get => self.TextView.Buffer.Text;
        set => SetTextCore(() => self.TextView.Buffer.Text = value ?? string.Empty);
    }

    /// <summary>
    /// Appends <paramref name="text" /> at the end of the buffer.
    /// </summary>
    /// <param name="text">
    /// The text to append.
    /// </param>
    protected override void AppendTextCore(string text)
    {
        var enditer = self.TextView.Buffer.EndIter;
        self.TextView.Buffer.Insert(ref enditer, text);
    }

    /// <summary>
    /// Replaces the current selection (or inserts at the caret when nothing is selected) with
    /// <paramref name="text" />.
    /// </summary>
    /// <param name="text">The replacement text.</param>
    protected override void ReplaceSelectedTextCore(string text)
    {
        if (self.TextView.Buffer.GetSelectionBounds(out TextIter start, out TextIter end))
        {
            self.TextView.Buffer.DeleteSelection(false, false);
        }

        self.TextView.Buffer.InsertAtCursor(text ?? string.Empty);
    }

    /// <summary>
    /// Deselects all text.
    /// </summary>
    public override void DeselectAll()
    {
        SelectionLength = 0;
    }

    /// <summary>
    /// Sets input focus to the control.
    /// </summary>
    /// <returns>
    /// <c>
    /// true
    /// </c> if the input focus request was successful; otherwise,
    /// <c>
    /// false
    /// </c>.
    /// </returns>
    public override bool Focus()
    {
        self.TextView.Buffer.GetSelectionBounds(out TextIter start, out TextIter end);
        self.TextView.ScrollToIter(start, 0, false, 0, 1);
        self.TextView.IsFocus = true;

        return self.TextView.IsFocus;
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
        self.TextView.Buffer.InsertAtCursor(text);
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
        var startiter = self.TextView.Buffer.GetIterAtOffset(start);
        var enditer = self.TextView.Buffer.GetIterAtOffset(start + length);
        self.TextView.Buffer.SelectRange(startiter, enditer);
    }

    /// <summary>
    /// Selects all text.
    /// </summary>
    public override void SelectAll()
    {
        Select(0, Text.Length);
    }

    /// <summary>
    /// Applies styling to the GTK widget based on control properties.
    /// </summary>
    /// <param name="widget">
    /// GTK widget to style.
    /// </param>
    protected override void SetStyle(Widget widget)
    {
        self.TextView.Name = this.Name;
        base.SetStyle(self.TextView);
        self.TextView.FocusVadjustment = self.Vadjustment;
        self.TextView.FocusHadjustment = self.Hadjustment;
    }

    private void Buffer_Changed(object sender, EventArgs e)
    {
        if (self.IsVisible)
        {
            OnTextChanged(e);
        }
    }
}