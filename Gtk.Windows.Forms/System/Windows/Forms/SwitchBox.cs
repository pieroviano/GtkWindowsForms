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
/// Class SwitchBox. Implements the
/// <see cref="T:System.Windows.Forms.Control" />
/// </summary>
/// <seealso cref="T:System.Windows.Forms.Control" />
[DesignerCategory("Component")]
public partial class SwitchBox : Control
{
    /// <summary>
    /// The self
    /// </summary>
    public readonly SwitchBoxBase self = new();

    /// <summary>
    /// Occurs when checked changed.
    /// </summary>
    public virtual event EventHandler? CheckedChanged;

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.SwitchBox" /> class.
    /// </summary>
    public SwitchBox()
    {
        self.Override.sender = this;

        // Gtk4 removed the button-press signal the old code toggled on; observe the switch's active state
        // directly instead. notify::active fires for both a user toggle (mouse/touch/keyboard) and a
        // programmatic Active change, so it is the single source for CheckedChanged - no separate click
        // gesture, and no manual raise in the Checked setter (which would double-fire on a physical toggle).
        self.AddNotification("active", Self_ActiveNotify);
    }

    /// <summary>
    /// Gets or sets a value indicating whether this
    /// <see cref="T:System.Windows.Forms.SwitchBox" /> is checked.
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
        // notify::active (wired in the ctor) raises CheckedChanged when the value actually changes; setting
        // it here is all that is needed.
        set { self.Active = value; }
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
        get { return self.TooltipText; }
        set { self.TooltipText = value; }
    }

    /// <summary>
    /// Raises the CheckedChanged event.
    /// </summary>
    protected internal virtual void OnCheckedChanged(System.EventArgs e)
    {
        CheckedChanged?.Invoke(this, e);
    }

    private void Self_ActiveNotify(object o, GLib.NotifyArgs args)
    {
        // Only surface the change while the switch is actually part of a shown UI. A Gtk4 widget is
        // "visible" by default even before it is added to anything, so IsVisible cannot be the test -
        // being parented (or mapped/realized) is the WinForms "is on a form" condition, and an unparented
        // switch whose Checked is set programmatically must stay silent.
        if (CheckedChanged != null && (self.Parent != null || self.IsMapped || self.IsRealized))
        {
            OnCheckedChanged(EventArgs.Empty);
        }
    }
}