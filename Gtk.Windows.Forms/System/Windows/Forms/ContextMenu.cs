using System.ComponentModel;
using System.Drawing;

namespace System.Windows.Forms;

/// <summary>
/// Represents a shortcut menu.
/// </summary>
/// <remarks>
/// An adapter over <see cref="T:System.Windows.Forms.ContextMenuStrip" />, which is what actually renders.
/// Assign it to <see cref="P:System.Windows.Forms.Control.ContextMenu" /> the way the legacy API does.
/// </remarks>
public class ContextMenu : Menu
{
    /// <summary>
    /// Occurs when the menu is about to be displayed.
    /// </summary>
    public event EventHandler? Popup;

    /// <summary>
    /// Occurs when the menu collapses.
    /// </summary>
    public event EventHandler? Collapse;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:System.Windows.Forms.ContextMenu" /> class.
    /// </summary>
    public ContextMenu()
    {
        Strip = new ContextMenuStrip();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:System.Windows.Forms.ContextMenu" /> class holding
    /// the specified items.
    /// </summary>
    /// <param name="menuItems">The items the menu starts with.</param>
    public ContextMenu(MenuItem[]? menuItems) : this()
    {
        if (menuItems != null)
        {
            MenuItems.AddRange(menuItems);
        }
    }

    /// <summary>
    /// The modern strip this legacy menu drives.
    /// </summary>
    internal ContextMenuStrip Strip { get; }

    /// <summary>
    /// Gets the control that last displayed this menu.
    /// </summary>
    [Browsable(false)]
    public Control? SourceControl
    {
        get;
        private set;
    }

    /// <summary>
    /// Gets or sets the reading order of the menu.
    /// </summary>
    [DefaultValue(RightToLeft.Inherit)]
    public virtual RightToLeft RightToLeft
    {
        get;
        set;
    } = RightToLeft.Inherit;

    /// <summary>
    /// Displays the menu at the specified position relative to the given control.
    /// </summary>
    /// <param name="control">The control the menu is displayed over.</param>
    /// <param name="pos">The position, in the control's coordinates, to display at.</param>
    public void Show(Control control, Point pos)
    {
        if (control == null)
        {
            throw new ArgumentNullException(nameof(control));
        }

        SourceControl = control;
        OnPopup(EventArgs.Empty);

        // Route through ContextMenuStrip.Show(control, position) -> ToolStripDropDown.ShowCore, which parents
        // the drop-down popover to the source control (gtk_widget_set_parent) and only calls Popup() once that
        // control is realized and on screen. Setting Strip.Visible = true instead reached Control.Visible ->
        // gtk_widget_show(), which realizes the popover outside any toplevel window ("gtk_widget_get_native ...
        // GTK_IS_NATIVE failed") and aborts the process. Showing over a control that has no surface yet (e.g.
        // one not added to a form) is a safe no-op there - SourceControl and the Popup event above still hold.
        Strip.Show(control, pos);
    }

    /// <summary>
    /// Displays the menu at the specified position relative to the given control, with the given alignment.
    /// </summary>
    /// <param name="control">The control the menu is displayed over.</param>
    /// <param name="pos">The position, in the control's coordinates, to display at.</param>
    /// <param name="alignment">How the menu is aligned relative to <paramref name="pos" />.</param>
    public void Show(Control control, Point pos, LeftRightAlignment alignment) => Show(control, pos);

    /// <summary>
    /// Raises the <see cref="E:System.Windows.Forms.ContextMenu.Popup" /> event.
    /// </summary>
    /// <param name="e">The event data.</param>
    protected internal virtual void OnPopup(EventArgs e) => Popup?.Invoke(this, e);

    /// <summary>
    /// Raises the <see cref="E:System.Windows.Forms.ContextMenu.Collapse" /> event.
    /// </summary>
    /// <param name="e">The event data.</param>
    protected internal virtual void OnCollapse(EventArgs e) => Collapse?.Invoke(this, e);

    /// <inheritdoc />
    internal override void HostAdd(MenuItem item) => Strip.Items.Add(item.Item);

    /// <inheritdoc />
    internal override void HostRemove(MenuItem item) => Strip.Items.Remove(item.Item);

    /// <inheritdoc />
    internal override void HostInsert(int index, MenuItem item) => Strip.Items.Insert(index, item.Item);

    /// <inheritdoc />
    internal override void HostClear() => Strip.Items.Clear();
}
