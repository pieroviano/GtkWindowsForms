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
/// Enables the user to select a single option from a group of choices when paired with other
/// <see cref="T:System.Windows.Forms.RadioButton" /> controls.
/// </summary>
[DesignerCategory("Component")]
public partial class RadioButton : Control
{
    /// <summary>
    /// The self
    /// </summary>
    public readonly RadioButtonBase self = new RadioButtonBase();

    private bool _Checked;

    /// <summary>
    /// Occurs when checked changed.
    /// </summary>
    public virtual event EventHandler CheckedChanged;

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.RadioButton" /> class.
    /// </summary>
    public RadioButton() : base()
    {
        self.ParentSet += Self_ParentSet;
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
        set { _Checked = true; self.Active = true; }
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

    private void Self_ParentSet(object o, Gtk.ParentSetArgs args)
    {
        var con = self.Parent as Gtk.Container;
        foreach (var widget in con.Children)
        {
            if (widget is Gtk.RadioButton group)
            {
                ((Gtk.RadioButton)o).Group = new Gtk.RadioButton[0];
                // the first one added into the containerradio配组
                ((Gtk.RadioButton)o).JoinGroup(group);
                break;
            }
        }
        self.Active = _Checked;
        self.Toggled += Self_Toggled;
    }

    private void Self_Toggled(object sender, EventArgs e)
    {
        if (CheckedChanged != null && self.IsVisible)
            OnCheckedChanged(e);
    }
}