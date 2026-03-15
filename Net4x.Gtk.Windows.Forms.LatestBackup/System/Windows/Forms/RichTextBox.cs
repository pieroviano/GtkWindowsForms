/*
 * Based on GTK components, it is a cross-platform interface component that is compatible with the native C# control winform interface.
 * Use this component GtkSystem.Windows.Forms instead of Microsoft.WindowsDesktop.App.WindowsForms, compile once, and run across Windows, Linux, and macOS
 * Technical support 438865652@qq.com，https://www.gtkapp.com, https://gitee.com/easywebfactory, https://github.com/easywebfactory438865652@qq.com，https://www.gtkapp.com, https://gitee.com/easywebfactory, https://github.com/easywebfactory
 * author:chenhongjin
 */
using System.ComponentModel;
using GLib;
using Gtk;
using GtkSystem.Windows.Forms.GtkControls.ControlBase;

namespace System.Windows.Forms;

/// <summary>
/// Represents a Windows rich text box control.
/// </summary>
[DesignerCategory("Component")]
public partial class RichTextBox : ScrollableControl
{
    /// <summary>
    /// The self
    /// </summary>
    public readonly RichTextBoxBase self = new RichTextBoxBase();

    /// <summary>
    /// Occurs when the Text property value changes.
    /// </summary>
    public override event EventHandler TextChanged;

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.RichTextBox" /> class.
    /// </summary>
    public RichTextBox() : base()
    {
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
    public string[] Lines
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
    public virtual bool ReadOnly { get { return self.TextView.CanFocus; } set { self.TextView.CanFocus = value; } }

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
            self.TextView.Buffer.GetSelectionBounds(out TextIter start, out TextIter end);

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
    public int SelectionStart
    {
        get
        {
            self.TextView.Buffer.GetSelectionBounds(out TextIter start, out TextIter end);

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
    public override string Text { get => self.TextView.Buffer.Text; set => self.TextView.Buffer.Text = value; }

    /// <summary>
    /// Appends the text.
    /// </summary>
    /// <param name="text">
    /// The text.
    /// </param>
    public void AppendText(string text)
    {
        var enditer = self.TextView.Buffer.EndIter;
        self.TextView.Buffer.Insert(ref enditer, text);
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
    public void Select(int start, int length)
    {
        TextIter startiter = self.TextView.Buffer.GetIterAtOffset(start);
        TextIter enditer = self.TextView.Buffer.GetIterAtOffset(start + length);
        self.TextView.Buffer.SelectRange(startiter, enditer);
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
    }

    private void Buffer_Changed(object sender, EventArgs e)
    {
        if (TextChanged != null && self.IsVisible)
        {
            TextChanged(this, e);
        }
    }

}