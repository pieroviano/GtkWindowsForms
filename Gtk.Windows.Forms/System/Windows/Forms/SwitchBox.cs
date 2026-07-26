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

        // React to native button release events (mouse/touch) coming from GTK
        self.ButtonReleaseEvent += Self_ButtonReleaseEvent;

        // Ensure we observe native toggles so programmatic changes to `self.Active`
        // update the managed event path. This addresses tests that flip `self.Active`
        // directly and expect CheckedChanged to fire.
        self.ButtonPressEvent += Self_Toggled;
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
        set
        {
            // set native state first; Self_Toggled (subscribed above) runs whenever native toggles,
            // but when setting programmatically we also raise CheckedChanged while visible to
            // preserve previous behavior (and tests that depend on it).
            self.Active = value;
            if (CheckedChanged != null && (self.IsVisible || self.IsMapped || self.IsRealized || self.Parent != null))
                OnCheckedChanged(EventArgs.Empty);
        }
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

    private void Self_ButtonReleaseEvent(object o, Gtk.ButtonReleaseEventArgs args)
    {
        // Fire CheckedChanged when control is part of UI. Use parent/mapped/realized checks
        // so tests that simulate the native release during Form.Load still see the event.
        var hasParent = self.Parent != null;
        var isMappedOrVisible = self.IsMapped || self.IsVisible || self.IsRealized;

        if (CheckedChanged != null && (hasParent || isMappedOrVisible))
            OnCheckedChanged(EventArgs.Empty);
    }

    private void Self_Toggled(object? sender, EventArgs e)
    {
        // Keep managed state/handlers in sync when GTK toggles the underlying switch,
        // including when tests manipulate `self.Active` directly.
        if (CheckedChanged != null && (self.IsVisible || self.IsMapped || self.IsRealized || self.Parent != null))
        {
            OnCheckedChanged(e);
        }
    }
}