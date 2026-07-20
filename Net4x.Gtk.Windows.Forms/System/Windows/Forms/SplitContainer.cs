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
/// Represents a control consisting of a movable bar that divides a container's display area into two resizable panels.
/// </summary>
[DesignerCategory("Component")]
public partial class SplitContainer : ContainerControl
{
    /// <summary>
    /// The splitter distance
    /// </summary>
    public int _SplitterDistance;

    /// <summary>
    /// The self
    /// </summary>
    public readonly SplitContainerBase self = new();

    private FixedPanel _fixedPanel = FixedPanel.Panel1;

    private SplitterPanel _panel1;

    private SplitterPanel _panel2;

    private int _SplitterWidth;

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.SplitContainer" /> class.
    /// </summary>
    public SplitContainer() : base()
    {
        self.Override.sender = this;
        _panel1 = new SplitterPanel(this);
        _panel1.self.contaner.Name = "Child1";
        _panel2 = new SplitterPanel(this);
        _panel2.self.contaner.Name = "Child2";
        self.Pack1(_panel1.Widget, false, true);
        self.Pack2(_panel2.Widget, true, true);
    }

    /// <summary>
    /// Gets or sets the fixed panel.
    /// </summary>
    /// <value>
    /// The fixed panel.
    /// </value>
    public FixedPanel FixedPanel
    {
        get => _fixedPanel;
        set
        {
            _fixedPanel = value;
            var resize = value == FixedPanel.Panel2;
            ((Paned.PanedChild)self[_panel1.Widget]).Resize = resize;
            ((Paned.PanedChild)self[_panel2.Widget]).Resize = !resize;
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
    /// Gets or sets a value indicating whether the splitter is fixed.
    /// </summary>
    /// <value>
    /// <c>
    /// true
    /// </c> if the splitter is fixed; otherwise,
    /// <c>
    /// false
    /// </c>.
    /// </value>
    public bool IsSplitterFixed
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the orientation.
    /// </summary>
    /// <value>
    /// The orientation.
    /// </value>
    public Orientation Orientation
    {
        get { return self.Orientation == Gtk.Orientation.Horizontal ? Orientation.Vertical : Orientation.Horizontal; }
        set
        {
            self.Orientation = value == Orientation.Horizontal ? Gtk.Orientation.Vertical : Gtk.Orientation.Horizontal;
        }
    }

    /// <summary>
    /// Gets or sets the panel1.
    /// </summary>
    /// <value>
    /// The panel1.
    /// </value>
    public SplitterPanel Panel1
    {
        get
        {
            return _panel1;
        }
        set
        {
            _panel1 = value;
        }
    }

    /// <summary>
    /// Gets or sets the panel2.
    /// </summary>
    /// <value>
    /// The panel2.
    /// </value>
    public SplitterPanel Panel2
    {
        get
        {
            return _panel2;
        }
        set
        {
            _panel2 = value;
        }
    }

    /// <summary>
    /// Gets or sets the splitter distance.
    /// </summary>
    /// <value>
    /// The splitter distance.
    /// </value>
    public int SplitterDistance
    {
        get => self.Position + 5;
        set { _SplitterDistance = Math.Max(1, value - 5); self.Position = _SplitterDistance; }
    }

    /// <summary>
    /// Gets or sets the splitter increment.
    /// </summary>
    /// <value>
    /// The splitter increment.
    /// </value>
    public int SplitterIncrement
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the width of the splitter.
    /// </summary>
    /// <value>
    /// The width of the splitter.
    /// </value>
    public int SplitterWidth
    {
        get { return _SplitterWidth; }
        set { _SplitterWidth = value; self.WideHandle = value > 2; }
    }
}