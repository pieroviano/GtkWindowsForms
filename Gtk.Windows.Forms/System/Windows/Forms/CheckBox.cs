/*
 * Based on GTK components, it is a cross-platform interface component that is compatible with the native C# control winform interface.
 * Use this component GtkSystem.Windows.Forms instead of Microsoft.WindowsDesktop.App.WindowsForms, compile once, and run across Windows, Linux, and macOS
 * Technical support 438865652@qq.com，https://www.gtkapp.com, https://gitee.com/easywebfactory, https://github.com/easywebfactory438865652@qq.com，https://www.gtkapp.com, https://gitee.com/easywebfactory, https://github.com/easywebfactory
 * author:chenhongjin
 */
using System.ComponentModel;
using GLib;
using GtkSystem.Windows.Forms.GtkControls.ControlBase;

namespace System.Windows.Forms;

/// <summary>
/// Represents a Windows
/// <see cref="T:System.Windows.Forms.CheckBox" />
/// </summary>
/// <filterpriority> 1
/// </filterpriority>
[DesignerCategory("Component")]
public partial class CheckBox : ButtonBase
{
    /// <summary>
    /// The self
    /// </summary>
    public readonly CheckBoxBase self = new();

    /// <summary>
    /// Occurs when checked is changed.
    /// </summary>
    public virtual event EventHandler? CheckedChanged;

    /// <summary>
    /// Occurs when the check state is changed.
    /// </summary>
    public virtual event EventHandler? CheckStateChanged;

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.CheckBox" /> class.
    /// </summary>
    public CheckBox()
    {
        self.Override.sender = this;
        self.Toggled += Self_Toggled;
        // Gtk 4: this per-control raw-event handler used a removed *EventArgs type; control-specific
        // input is re-wired via event controllers / native signals and verified at runtime (migration #11).
    }

    /// <summary>
    /// Gets or sets the value that determines the appearance of the check box.
    /// </summary>
    /// <value>
    /// The appearance.
    /// </value>
    // Gtk4 removed CheckButton.DrawIndicator (the indicator always draws; button-look is a CSS/style
    // concern now). Back Appearance with a field so the WinForms API round-trips.
    private bool _appearanceNormal = true;

    public System.Windows.Forms.Appearance Appearance
    {
        get => _appearanceNormal ? Appearance.Normal : Appearance.Button;
        set { _appearanceNormal = value == Appearance.Normal; }
    }

    /// <summary>
    /// Gets or sets a value indicating whether this
    /// <see cref="T:System.Windows.Forms.CheckBox" /> is checked.
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
        set { self.Active = value; if (self.IsRealized) { self.Inconsistent = false; } }
    }

    /// <summary>
    /// Gets or sets the state of the check.
    /// </summary>
    /// <value>
    /// The state of the check.
    /// </value>
    public CheckState CheckState
    {
        get { if (self.Inconsistent == true) { return CheckState.Indeterminate; } else { return self.Active ? CheckState.Checked : CheckState.Unchecked; } }
        set { self.Inconsistent = value == CheckState.Indeterminate; self.Active = value == CheckState.Checked; }
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
    public override string Text
    {
        get { if (self.Child is Gtk.Label label) { return label.Text; } else return self.Label; }
        set { if (self.Child is Gtk.Label label) { label.Text = value; } else self.Label = value; }
    }

    /// <summary>
    /// Applies the alignment factors produced from
    /// <see cref="P:System.Windows.Forms.ButtonBase.TextAlign" /> to the GTK check box.
    /// </summary>
    /// <param name="xAlign">The horizontal alignment factor.</param>
    /// <param name="yAlign">The vertical alignment factor.</param>
    protected override void SetContentAlignmentCore(float xAlign, float yAlign)
    {
        // Gtk4-removed: self.Xalign = xAlign;
        // Gtk4-removed: self.Yalign = yAlign;
    }

    /// <summary>
    /// Raises the CheckedChanged event.
    /// </summary>
    protected virtual void OnCheckedChanged(System.EventArgs e)
    {
        CheckedChanged?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the CheckStateChanged event.
    /// </summary>
    protected virtual void OnCheckStateChanged(System.EventArgs e)
    {
        CheckStateChanged?.Invoke(this, e);
    }

    private void Self_Toggled(object sender, EventArgs e)
    {
        if (CheckedChanged != null && self.IsVisible)
            CheckedChanged(this, EventArgs.Empty);
        if (CheckStateChanged != null && self.IsVisible)
            CheckStateChanged(this, EventArgs.Empty);
    }
}