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
/// Represents a Windows
/// <see cref="T:System.Windows.Forms.CheckBox" />
/// </summary>
/// <filterpriority> 1
/// </filterpriority>
[DesignerCategory("Component")]
public partial class CheckBox : Control
{
    /// <summary>
    /// The self
    /// </summary>
    public readonly CheckBoxBase self = new CheckBoxBase();

    /// <summary>
    /// Occurs when checked is changed.
    /// </summary>
    public virtual event EventHandler CheckedChanged;

    /// <summary>
    /// Occurs when the check state is changed.
    /// </summary>
    public virtual event EventHandler CheckStateChanged;

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.CheckBox" /> class.
    /// </summary>
    public CheckBox()
    {
        self.Toggled += Self_Toggled;
        self.ButtonReleaseEvent += Self_ButtonReleaseEvent;
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
        get { return self.Label; }
        set { self.Label = value; }
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

    }

    private void Self_ButtonReleaseEvent(object o, Gtk.ButtonReleaseEventArgs args)
    {
        if (self.Inconsistent == true)
            self.Inconsistent = false;
    }

    private void Self_Toggled(object sender, EventArgs e)
    {
        if (CheckedChanged != null && self.IsVisible)
            CheckedChanged(this, EventArgs.Empty);
        if (CheckStateChanged != null && self.IsVisible)
            CheckStateChanged(this, EventArgs.Empty);
    }
}