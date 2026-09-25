/*
 * Based on GTK components, it is a cross-platform interface component that is compatible with the native C# control winform interface.
 * Use this component GtkSystem.Windows.Forms instead of Microsoft.WindowsDesktop.App.WindowsForms, compile once, and run across Windows, Linux, and macOS
 * Technical support 438865652@qq.com，https://www.gtkapp.com, https://gitee.com/easywebfactory, https://github.com/easywebfactory438865652@qq.com，https://www.gtkapp.com, https://gitee.com/easywebfactory, https://github.com/easywebfactory
 * author:chenhongjin
 */
using System.ComponentModel;
using GtkSystem.Windows.Forms.GtkControls.ControlBase;

namespace System.Windows.Forms;

/// <summary>
/// Represents a standard Windows label.
/// </summary>
[DesignerCategory("Component")]
public partial class Label : Control
{
    /// <summary>
    /// Underlying GTK-backed label implementation used by this WinForms wrapper.
    /// This field provides the actual GTK widget and properties that the Label
    /// surface forwards to (text, alignment, direction, etc.).
    /// </summary>
    public readonly LabelBase self = new();

    private System.Drawing.ContentAlignment textAlign;

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.Label" /> class.
    /// </summary>
    public Label() : base()
    {
        self.Override.sender = this;
    }

    /// <summary>
    /// Gets the GTK control.
    /// </summary>
    /// <value>
    /// The GTK control.
    /// </value>
    public override object GtkControl => self;

    /// <summary>
    /// Attempts to set focus to the label's underlying GTK widget.
    /// Override ensures Focus() works for Label even if widget focus plumbing differs.
    /// </summary>
    public override bool Focus()
    {
        // If the underlying GTK-backed widget is available, request/mark focus on it.
        if (self is Gtk.Widget w)
        {
            // Some GTK wrapper implementations expose explicit IsFocus flag; try to set it.
            try
            {
                w.GrabFocus();
                return w.IsFocus;
            }
            catch
            {
                // Fall back to base behavior if direct set fails.
            }
        }

        return base.Focus();
    }

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

    /// <summary>
    /// Gets or sets a value indicating whether <c>&amp;</c> in <see cref="Text" /> marks a mnemonic.
    /// </summary>
    [DefaultValue(true)]
    public bool UseMnemonic { get; set; } = true;

    public override string Text
    {
        get => _text;
        set
        {
            _text = value ?? string.Empty;
            self.Text = Mnemonics.ToGtk(_text, UseMnemonic);
            self.UseUnderline = UseMnemonic; // set_text clears the flag, so it goes on afterwards
        }
    }

    /// <summary>
    /// Gets or sets the text align.
    /// </summary>
    /// <value>
    /// The text align.
    /// </value>
    public System.Drawing.ContentAlignment TextAlign
    {
        get { return textAlign; }
        set
        {
            textAlign = value;
            if (value == System.Drawing.ContentAlignment.TopLeft)
            {
                self.Xalign = 0.0f;
                self.Yalign = 0.0f;
            }
            else if (value == System.Drawing.ContentAlignment.TopCenter)
            {
                self.Xalign = 0.5f;
                self.Yalign = 0.0f;
            }
            else if (value == System.Drawing.ContentAlignment.TopRight)
            {
                self.Xalign = 1.0f;
                self.Yalign = 0.0f;
            }
            else if (value == System.Drawing.ContentAlignment.MiddleLeft)
            {
                self.Xalign = 0.0f;
                self.Yalign = 0.5f;
            }
            else if (value == System.Drawing.ContentAlignment.MiddleCenter)
            {
                self.Xalign = 0.5f;
                self.Yalign = 0.5f;
            }
            else if (value == System.Drawing.ContentAlignment.MiddleRight)
            {
                self.Xalign = 1.0f;
                self.Yalign = 0.5f;
            }
            else if (value == System.Drawing.ContentAlignment.BottomLeft)
            {
                self.Xalign = 0.0f;
                self.Yalign = 1.0f;
            }
            else if (value == System.Drawing.ContentAlignment.BottomCenter)
            {
                self.Xalign = 0.5f;
                self.Yalign = 1.0f;
            }
            else if (value == System.Drawing.ContentAlignment.BottomRight)
            {
                self.Xalign = 1.0f;
                self.Yalign = 1.0f;
            }
        }
    }
}
