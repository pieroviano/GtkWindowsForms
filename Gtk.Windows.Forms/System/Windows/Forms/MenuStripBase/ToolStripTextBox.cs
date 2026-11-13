/*
 * Based on GTK components, it is a cross-platform interface component that is compatible with the native C# control winform interface.
 * Use this component Gtk.Windows.Forms instead of Microsoft.WindowsDesktop.App.WindowsForms, compile once, and run across Windows, Linux, and macOS
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
        // A size request is only a minimum, so it cannot pull the entry below Adwaita's own entry metrics
        // (34px min-height plus padding). This class carries the rule that collapses those, loaded at User
        // priority where the theme cannot win - see Application.UserPriorityCss.
        entry.StyleContext.AddClass("ToolStripEntry");
        self.AddChild(entry);
    }

    /// <summary>
    /// Gets or sets the size of this item.
    /// </summary>
    /// <value>The item's size, which is applied to the underlying entry.</value>
    /// <remarks>
    /// WinForms sizes a strip item by its <see cref="P:System.Windows.Forms.ToolStripItem.Size" />, and
    /// without passing that on the bare <c>GtkEntry</c> falls back to natural sizing. That had two visible
    /// effects: the box rendered only a couple of characters wide (the char hints in the constructor keep
    /// its natural width tiny), and it demanded the full height of Adwaita's entry metrics - 42px against a
    /// 33px designed strip - which, being the tallest item, inflated the whole toolstrip and pushed the
    /// items past the right edge. <see cref="ToolStripComboBox" /> has always applied its size this way;
    /// the text box was simply missing the counterpart.
    /// </remarks>
    public override Size Size
    {
        get => base.Size;
        set
        {
            entry.WidthRequest = value.Width;
            entry.HeightRequest = value.Height;
            base.Size = value;
        }
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