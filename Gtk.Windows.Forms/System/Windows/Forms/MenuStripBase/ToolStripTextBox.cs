/*
 * Based on GTK components, it is a cross-platform interface component that is compatible with the native C# control winform interface.
 * Use this component GtkSystem.Windows.Forms instead of Microsoft.WindowsDesktop.App.WindowsForms, compile once, and run across Windows, Linux, and macOS
 * Technical support 438865652@qq.com，https://www.gtkapp.com, https://gitee.com/easywebfactory, https://github.com/easywebfactory438865652@qq.com，https://www.gtkapp.com, https://gitee.com/easywebfactory, https://github.com/easywebfactory
 * author:chenhongjin
 * date: 2024/1/3
 */

using System.Drawing;

namespace System.Windows.Forms;

/// <summary>
/// Represents a text box in a
/// <see cref="T:System.Windows.Forms.ToolStrip" />
/// that allows the user to enter text.
/// </summary>
public class ToolStripTextBox : ToolStripItem
{

    public StripToolItem self = new StripToolItem();

    internal Gtk.Entry entry = new Gtk.Entry();

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.ToolStripTextBox" /> class.
    /// </summary>
    public ToolStripTextBox() : base()
    {
        entry.MaxWidthChars = 1;
        entry.WidthChars = 0;
        self.AddChild(entry);
    }

    /// <summary>
    /// Gets or sets the maximum length.
    /// </summary>
    /// <value>
    /// The maximum length.
    /// </value>
    public int MaxLength
    {
        get => entry.MaxLength;
        set => entry.MaxLength = value;
    }

    /// <summary>
    /// Gets or sets the text.
    /// </summary>
    /// <value>
    /// The text.
    /// </value>
    public override string Text
    {
        get => entry.Text;
        set => entry.Text = value;
    }

    public override IToolMenuItem Widget { get => self; }
}