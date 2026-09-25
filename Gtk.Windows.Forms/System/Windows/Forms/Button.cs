/*
 * Based on GTK components, it is a cross-platform interface component that is compatible with the native C# control winform interface.
 * Use this component GtkSystem.Windows.Forms instead of Microsoft.WindowsDesktop.App.WindowsForms, compile once, and run across Windows, Linux, and macOS
 * Technical support 438865652@qq.com，https://www.gtkapp.com, https://gitee.com/easywebfactory, https://github.com/easywebfactory438865652@qq.com，https://www.gtkapp.com, https://gitee.com/easywebfactory, https://github.com/easywebfactory
 * author:chenhongjin
 */

using System.ComponentModel;
using GtkSystem.Windows.Forms.GtkControls.ControlBase;
using GtkButtonBase = GtkSystem.Windows.Forms.GtkControls.ControlBase.ButtonBase;

namespace System.Windows.Forms;

/// <summary>
/// Represents a Windows button control.
/// </summary>
/// <filterpriority>
/// 1
/// </filterpriority>
[DesignerCategory("Component")]
public partial class Button : ButtonBase
{
    /// <summary>
    /// The self
    /// </summary>
    public readonly GtkButtonBase self = new();

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.Button" /> class.
    /// </summary>
    public Button() : base()
    {
        self.Override.sender = this;
    }

    /// <summary>
    /// Gets or sets the value that is returned to the parent form when the button is clicked.
    /// </summary>
    /// <value>
    /// The dialog result.
    /// </value>
    public DialogResult DialogResult
    {
        get => self.DialogResult;
        set => self.DialogResult = value;
    }

    /// <summary>
    /// Gets the GTK control.
    /// </summary>
    /// <value>
    /// The GTK control.
    /// </value>
    public override object GtkControl => self;

    /// <summary>
    /// Gets or sets the right to left.
    /// </summary>
    /// <value>
    /// The right to left.
    /// </value>
    public override RightToLeft RightToLeft
    {
        get { return self.Direction == Gtk.TextDirection.Rtl ? RightToLeft.Yes : RightToLeft.No; }
        set { self.Direction = value == RightToLeft.Yes ? Gtk.TextDirection.Rtl : Gtk.TextDirection.Ltr; }
    }

    /// <summary>
    /// Gets or sets the text.
    /// </summary>
    /// <value>
    /// The text.
    /// </value>
    private string _text = string.Empty;

    public override string Text
    {
        // The caption is kept as WinForms wrote it: the widget holds GTK's spelling of the mnemonic, which would
        // read back without the ampersand.
        get => _text;
        set
        {
            _text = value ?? string.Empty;
            var label = (Gtk.Label)self.Child;
            label.Text = Mnemonics.ToGtk(_text, UseMnemonic);
            label.UseUnderline = UseMnemonic; // set_text clears the flag, so it goes on afterwards
        }
    }

    /// <summary>
    /// Applies the alignment factors produced from
    /// <see cref="P:System.Windows.Forms.ButtonBase.TextAlign" /> to the GTK button.
    /// </summary>
    /// <param name="xAlign">The horizontal alignment factor.</param>
    /// <param name="yAlign">The vertical alignment factor.</param>
    protected override void SetContentAlignmentCore(float xAlign, float yAlign)
    {
        // Gtk 4 removed GtkButton's own xalign/yalign; the caption label carries the equivalent.
        self.AlignCaption(xAlign, yAlign);
    }
}