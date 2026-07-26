using System.Drawing;

namespace System.Windows.Forms;

/// <summary>
/// Specifies how a <see cref="T:System.Windows.Forms.StatusBarPanel" /> is sized on a status bar.
/// </summary>
/// <remarks>
/// Part of the legacy status API, which was missing entirely.
/// </remarks>
public enum StatusBarPanelAutoSize
{
    /// <summary>The panel does not change size.</summary>
    None = 1,

    /// <summary>The panel shares the free space on the status bar.</summary>
    Spring = 2,

    /// <summary>The panel is sized to fit its contents.</summary>
    Contents = 3
}

/// <summary>
/// Specifies the border style of a <see cref="T:System.Windows.Forms.StatusBarPanel" />.
/// </summary>
public enum StatusBarPanelBorderStyle
{
    /// <summary>No border.</summary>
    None = 1,

    /// <summary>A raised border.</summary>
    Raised = 2,

    /// <summary>A sunken border.</summary>
    Sunken = 3
}

/// <summary>
/// Specifies whether a <see cref="T:System.Windows.Forms.StatusBarPanel" /> draws itself.
/// </summary>
public enum StatusBarPanelStyle
{
    /// <summary>The panel displays text and an optional icon.</summary>
    Text = 1,

    /// <summary>The panel is drawn by the application.</summary>
    OwnerDraw = 2
}

/// <summary>
/// Provides data for the <see cref="E:System.Windows.Forms.StatusBar.PanelClick" /> event.
/// </summary>
public class StatusBarPanelClickEventArgs : MouseEventArgs
{
    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.StatusBarPanelClickEventArgs" /> class.
    /// </summary>
    /// <param name="statusBarPanel">The panel that was clicked.</param>
    /// <param name="button">The mouse button that was pressed.</param>
    /// <param name="clicks">The number of times the button was pressed.</param>
    /// <param name="x">The x-coordinate of the click.</param>
    /// <param name="y">The y-coordinate of the click.</param>
    public StatusBarPanelClickEventArgs(StatusBarPanel statusBarPanel, MouseButtons button, int clicks, int x, int y)
        : base(button, clicks, x, y, 0)
    {
        StatusBarPanel = statusBarPanel;
    }

    /// <summary>
    /// Gets the panel that was clicked.
    /// </summary>
    public StatusBarPanel StatusBarPanel
    {
        get;
    }
}

/// <summary>
/// Represents the method that handles the
/// <see cref="E:System.Windows.Forms.StatusBar.PanelClick" /> event.
/// </summary>
/// <param name="sender">The status bar that raised the event.</param>
/// <param name="e">The event data.</param>
public delegate void StatusBarPanelClickEventHandler(object sender, StatusBarPanelClickEventArgs e);
