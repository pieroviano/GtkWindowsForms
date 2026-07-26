using System.ComponentModel;
using System.Drawing;

namespace System.Windows.Forms;

/// <summary>
/// Represents a panel in a <see cref="T:System.Windows.Forms.StatusBar" />.
/// </summary>
/// <remarks>
/// An adapter over <see cref="T:System.Windows.Forms.ToolStripStatusLabel" />, which is what actually
/// renders inside the strip.
/// </remarks>
public class StatusBarPanel : Component
{
    /// <summary>
    /// Initializes a new instance of the <see cref="T:System.Windows.Forms.StatusBarPanel" /> class.
    /// </summary>
    public StatusBarPanel()
    {
        Label = new ToolStripStatusLabel();
    }

    /// <summary>
    /// The modern label this legacy panel drives.
    /// </summary>
    internal ToolStripStatusLabel Label { get; }

    /// <summary>
    /// Gets or sets the text of the panel.
    /// </summary>
    public string Text
    {
        get => Label.Text ?? string.Empty;
        set => Label.Text = value;
    }

    /// <summary>
    /// Gets or sets the name of the panel.
    /// </summary>
    public string Name
    {
        get => Label.Name ?? string.Empty;
        set => Label.Name = value;
    }

    /// <summary>
    /// Gets or sets the tooltip shown for the panel.
    /// </summary>
    public string ToolTipText
    {
        get => Label.ToolTipText ?? string.Empty;
        set => Label.ToolTipText = value;
    }

    /// <summary>
    /// Gets or sets the width of the panel.
    /// </summary>
    [DefaultValue(100)]
    public int Width
    {
        get => Label.Width;
        set => Label.Width = value;
    }

    /// <summary>
    /// Gets or sets the minimum width of the panel.
    /// </summary>
    [DefaultValue(10)]
    public int MinWidth
    {
        get;
        set;
    } = 10;

    /// <summary>
    /// Gets or sets how the text is aligned within the panel.
    /// </summary>
    [DefaultValue(HorizontalAlignment.Left)]
    public HorizontalAlignment Alignment
    {
        get;
        set;
    } = HorizontalAlignment.Left;

    /// <summary>
    /// Gets or sets the border style of the panel.
    /// </summary>
    [DefaultValue(StatusBarPanelBorderStyle.Sunken)]
    public StatusBarPanelBorderStyle BorderStyle
    {
        get;
        set;
    } = StatusBarPanelBorderStyle.Sunken;

    /// <summary>
    /// Gets or sets how the panel is sized within the status bar.
    /// </summary>
    [DefaultValue(StatusBarPanelAutoSize.None)]
    public StatusBarPanelAutoSize AutoSize
    {
        get;
        set;
    } = StatusBarPanelAutoSize.None;

    /// <summary>
    /// Gets or sets the style of the panel.
    /// </summary>
    [DefaultValue(StatusBarPanelStyle.Text)]
    public StatusBarPanelStyle Style
    {
        get;
        set;
    } = StatusBarPanelStyle.Text;

    /// <summary>
    /// Gets or sets the icon displayed in the panel.
    /// </summary>
    public Icon? Icon
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets user-defined data associated with the panel.
    /// </summary>
    [DefaultValue(null)]
    public object? Tag
    {
        get;
        set;
    }

    /// <summary>
    /// Gets the status bar that contains this panel.
    /// </summary>
    [Browsable(false)]
    public StatusBar? Parent
    {
        get;
        internal set;
    }

    /// <summary>
    /// Begins initialisation of the panel.
    /// </summary>
    public void BeginInit()
    {
    }

    /// <summary>
    /// Ends initialisation of the panel.
    /// </summary>
    public void EndInit()
    {
    }

    /// <summary>
    /// Returns a string that represents this panel.
    /// </summary>
    public override string ToString() => $"{GetType().Name}: {{{Text}}}";
}
