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
/// Represents a standard Windows horizontal scroll bar.
/// </summary>
[DesignerCategory("Component")]
public class HScrollBar : ScrollBar
{
    /// <summary>
    /// The self
    /// </summary>
    public ScrollbarBase<Gtk.HScrollbar> self = new ScrollbarBase<Gtk.HScrollbar>(Gtk.Orientation.Horizontal);

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.HScrollBar" /> class.
    /// </summary>
    public HScrollBar() : base()
    {

    }

    /// <summary>
    /// Gets the adjustment.
    /// </summary>
    /// <value>
    /// The adjustment.
    /// </value>
    public override Gtk.Adjustment Adjustment { get => self.Adjustment; }

    /// <summary>
    /// Gets the GTK control.
    /// </summary>
    /// <value>
    /// The GTK control.
    /// </value>
    public override object GtkControl => self;

}