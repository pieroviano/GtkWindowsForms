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

    private readonly Gtk.Overlay contaner = new();

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.GroupBox" /> class.
    /// </summary>
    public GroupBox() : base()
    {
        _controls = new ControlCollection(this, contaner);
        _controls.Offset.Offset(0, -20);
        contaner.MarginStart = 0;
        contaner.MarginTop = 0;
        contaner.Halign = Align.Fill;
        contaner.Valign = Align.Fill;
        contaner.Add(new Gtk.Fixed() { Halign = Align.Fill, Valign = Align.Fill });
        self.Child = contaner;
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
            contaner.MarginStart = value.Left;
            contaner.MarginTop = value.Top;
            contaner.MarginEnd = value.Right;
            contaner.MarginBottom = value.Bottom;
        }
    }

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
    }

    /// <summary>
    /// Suspends layout logic for the control.
    /// </summary>
    public override void SuspendLayout()
    {
        _Created = false;
    }
}