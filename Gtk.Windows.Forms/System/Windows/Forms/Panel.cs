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
/// Used to group collections of controls.
/// </summary>
[DesignerCategory("Component")]
public partial class Panel : ScrollableControl
{
    /// <summary>
    /// The self
    /// </summary>
    public readonly PanelBase self = new();

    private ControlCollection _controls = null!;

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.Panel" /> class.
    /// </summary>
    public Panel() : base()
    {
        self.Override.sender = this;
        _controls = new ControlCollection(this, self.contaner);
        this.AutoScroll = false;
    }

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.Panel" /> class.
    /// </summary>
    /// <param name="type">
    /// The CSS style class applied to the underlying widget.
    /// </param>
    internal Panel(string type) : base()
    {
        self.Override.sender = this;
        self.StyleContext.AddClass(type);
        this.AutoScroll = false;
    }

    /// <summary>
    /// Gets the controls.
    /// </summary>
    /// <value>
    /// The controls.
    /// </value>
    public override ControlCollection Controls => _controls;

    /// <summary>
    /// Gets the GTK control.
    /// </summary>
    /// <value>
    /// The GTK control.
    /// </value>
    public override object GtkControl => self;

    /// <summary>
    /// Gets or sets the padding.
    /// </summary>
    /// <value>
    /// The padding.
    /// </value>
    public override Padding Padding
    {
        get => base.Padding;
        set
        {
            base.Padding = value;
            self.contaner.MarginStart = value.Left;
            self.contaner.MarginTop = value.Top;
            self.contaner.MarginEnd = value.Right;
            self.contaner.MarginBottom = value.Bottom;
        }
    }
}
