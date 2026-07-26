using System.ComponentModel;

namespace System.Windows.Forms;

/// <summary>
/// Represents the menu structure of a form.
/// </summary>
/// <remarks>
/// An adapter over <see cref="T:System.Windows.Forms.MenuStrip" />, which is what actually renders. Assign
/// it to <see cref="P:System.Windows.Forms.Form.Menu" /> the way the legacy API does.
/// </remarks>
public class MainMenu : Menu
{
    /// <summary>
    /// Initializes a new instance of the <see cref="T:System.Windows.Forms.MainMenu" /> class.
    /// </summary>
    public MainMenu()
    {
        Strip = new MenuStrip();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:System.Windows.Forms.MainMenu" /> class holding the
    /// specified items.
    /// </summary>
    /// <param name="items">The items the menu starts with.</param>
    public MainMenu(MenuItem[]? items) : this()
    {
        if (items != null)
        {
            MenuItems.AddRange(items);
        }
    }

    /// <summary>
    /// The modern strip this legacy menu drives.
    /// </summary>
    internal MenuStrip Strip { get; }

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
    /// Gets the form this menu is attached to, if any.
    /// </summary>
    [Browsable(false)]
    public Form? GetForm() => Strip.Parent as Form;

    /// <summary>
    /// Creates a copy of this menu and its items.
    /// </summary>
    public virtual MainMenu CloneMenu()
    {
        var clone = new MainMenu { Name = Name, Tag = Tag, RightToLeft = RightToLeft };
        for (var i = 0; i < MenuItems.Count; i++)
        {
            clone.MenuItems.Add(MenuItems[i].CloneMenu());
        }

        return clone;
    }

    /// <inheritdoc />
    internal override void HostAdd(MenuItem item) => Strip.Items.Add(item.Item);

    /// <inheritdoc />
    internal override void HostRemove(MenuItem item) => Strip.Items.Remove(item.Item);

    /// <inheritdoc />
    internal override void HostInsert(int index, MenuItem item) => Strip.Items.Insert(index, item.Item);

    /// <inheritdoc />
    internal override void HostClear() => Strip.Items.Clear();
}
