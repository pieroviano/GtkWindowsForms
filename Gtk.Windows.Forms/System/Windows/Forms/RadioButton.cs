/*
 * Based on GTK components, it is a cross-platform interface component that is compatible with the native C# control winform interface.
 * Use this component GtkSystem.Windows.Forms instead of Microsoft.WindowsDesktop.App.WindowsForms, compile once, and run across Windows, Linux, and macOS
 * Technical support 438865652@qq.com，https://www.gtkapp.com, https://gitee.com/easywebfactory, https://github.com/easywebfactory438865652@qq.com，https://www.gtkapp.com, https://gitee.com/easywebfactory, https://github.com/easywebfactory
 * author:chenhongjin
 */
using System.ComponentModel;
using System.Drawing;
using GtkSystem.Windows.Forms.GtkControls.ControlBase;

namespace System.Windows.Forms;

/// <summary>
/// Enables the user to select a single option from a group of choices when paired with other
/// <see cref="T:System.Windows.Forms.RadioButton" /> controls.
/// </summary>
[DesignerCategory("Component")]
public partial class RadioButton : ButtonBase
{
    /// <summary>
    /// The self
    /// </summary>
    public readonly RadioButtonBase self = new();

    private bool _Checked;

    /// <summary>
    /// Occurs when checked changed.
    /// </summary>
    public virtual event EventHandler? CheckedChanged;

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.RadioButton" /> class.
    /// </summary>
    public RadioButton() : base()
    {
        self.Override.sender = this;
        // Gtk4 (deferred): auto-grouping radios by shared parent used Gtk.Container.Children +
        // Gtk.RadioButton (removed). Group explicitly via RadioButtonBase(group) until reimplemented
        // over CheckButton.Group + FirstChild/NextSibling traversal.
    }

    /// <summary>
    /// Gets or sets the appearance of the radio button.
    /// </summary>
    /// <value>
    /// The appearance.
    /// </value>
    // Gtk4 removed CheckButton.DrawIndicator; back Appearance with a field (see CheckBox).
    private bool _appearanceNormal = true;

    public System.Windows.Forms.Appearance Appearance
    {
        get => _appearanceNormal ? Appearance.Normal : Appearance.Button;
        set { _appearanceNormal = value == Appearance.Normal; }
    }

    /// <summary>
    /// Gets or sets a value indicating whether this
    /// <see cref="T:System.Windows.Forms.RadioButton" /> is checked.
    /// </summary>
    /// <value>
    /// <c>
    /// true
    /// </c> if checked; otherwise,
    /// <c>
    /// false
    /// </c>.
    /// </value>
    public bool Checked
    {
        get { return self.Active; }
        set { _Checked = value; self.Active = value; }
    }

    /// <summary>
    /// Gets the GTK control.
    /// </summary>
    /// <value>
    /// The GTK control.
    /// </value>
    public override object GtkControl => self;

    /// <summary>
    /// Gets or sets the text.
    /// </summary>
    /// <value>
    /// The text.
    /// </value>
    private string _text = string.Empty;

    public override string Text
    {
        get => _text;
        set
        {
            _text = value ?? string.Empty;
            self.Label = Mnemonics.ToGtk(_text, UseMnemonic);
            self.UseUnderline = UseMnemonic;
            // The button's own caption label gets the underlined spelling; Alt+key still selects the button.
            if (self.GetLabelWidget() is { } caption)
            {
                Mnemonics.Apply(caption, _text, UseMnemonic);
                caption.MnemonicWidget = self;
            }
        }
    }

    /// <summary>
    /// Applies the alignment factors produced from
    /// <see cref="P:System.Windows.Forms.ButtonBase.TextAlign" /> to the GTK radio button.
    /// </summary>
    /// <param name="xAlign">The horizontal alignment factor.</param>
    /// <param name="yAlign">The vertical alignment factor.</param>
    protected override void SetContentAlignmentCore(float xAlign, float yAlign)
    {
        // Gtk 4 removed GtkButton's own xalign/yalign; the caption label carries the equivalent.
        self.AlignCaption(xAlign, yAlign);
    }

    /// <summary>
    /// Raises the CheckedChanged event.
    /// </summary>
    protected virtual void OnCheckedChanged(System.EventArgs e)
    {
        CheckedChanged?.Invoke(this, e);
    }


    private void Self_Toggled(object sender, EventArgs e)
    {
        if (CheckedChanged != null && self.IsVisible)
            OnCheckedChanged(e);
    }
}