/*
 * Based on GTK components, it is a cross-platform interface component that is compatible with the native C# control winform interface.
 * Use this component Gtk.Windows.Forms instead of Microsoft.WindowsDesktop.App.WindowsForms, compile once, and run across Windows, Linux, and macOS
 * Technical support 438865652@qq.com，https://www.gtkapp.com, https://gitee.com/easywebfactory, https://github.com/easywebfactory438865652@qq.com，https://www.gtkapp.com, https://gitee.com/easywebfactory, https://github.com/easywebfactory
 * author:chenhongjin
 */
using System.ComponentModel;
using System.Drawing;
using Gtk;
using Gtk.Windows.Forms.Controls.ControlBase;

namespace System.Windows.Forms;

/// <summary>
/// Represents a Windows control that displays a frame around a group of controls with an optional caption.
/// </summary>
[DesignerCategory("Component")]
public partial class GroupBox : ContainerControl
{
    /// <summary>
    /// The self
    /// </summary>
    public readonly GroupBoxBase self = new();

    private readonly ControlCollection _controls = null!;

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.GroupBox" /> class.
    /// </summary>
    public GroupBox() : base()
    {
        self.Override.sender = this;
        _controls = new ControlCollection(this, self.contaner);
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
            // Apply padding to the content overlay, not the scrolled wrapper (self.fixedcontaner). Setting
            // a margin on the ScrolledWindow forces a relayout that maps its internal scrollbar, which is
            // invisible under VscrollbarPolicy.External, aborting with a fatal "gtk_widget_map" GTK-CRITICAL
            // once the group box is shown. The overlay carried the padding pre-alignment; restore that.
            self.contaner.MarginStart = value.Left;
            self.contaner.MarginTop = value.Top;
            self.contaner.MarginEnd = value.Right;
            self.contaner.MarginBottom = value.Bottom;
        }
    }

    /// <summary>
    /// Gets or sets the size.
    /// </summary>
    /// <value>
    /// The size.
    /// </value>
    public override Size Size
    {
        get => base.Size;
        set
        {
            base.Size = value;
            self.contaner.WidthRequest = Math.Max(1, value.Width - self.fixedcontaner.MarginStart - self.fixedcontaner.MarginEnd);
            self.contaner.HeightRequest = Math.Max(1, value.Height - self.fixedcontaner.MarginStart - self.fixedcontaner.MarginEnd);
            self.fixedcontaner.Vadjustment.Value = 24;
        }
    }

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
            self.Label = Mnemonics.ToGtk(_text);
            // Setting Label makes the frame build its caption widget; tell that widget the '_' is a marker.
            if (self.LabelWidget is Gtk.Label caption)
            {
                Mnemonics.Apply(caption, _text);
            }
        }
    }

    /// <summary>
    /// Forces the control to perform layout logic.
    /// </summary>
    public override void PerformLayout()
    {
        _Created = true;
    }

    /// <summary>
    /// Resumes layout logic and optionally forces an immediate layout.
    /// </summary>
    /// <param name="resume">
    /// if set to
    /// <c>
    /// true
    /// </c>resume.
    /// </param>
    public override void ResumeLayout(bool resume)
    {
        _Created = resume == false;
        LeaveLayoutSuspension();
    }

    /// <summary>
    /// Suspends layout logic for the control.
    /// </summary>
    public override void SuspendLayout()
    {
        _Created = false;
        EnterLayoutSuspension();
    }
}
