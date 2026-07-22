using System.ComponentModel;

namespace System.Windows.Forms;

/// <summary>
/// Represents an individual item displayed in a <see cref="T:System.Windows.Forms.MainMenu" /> or
/// <see cref="T:System.Windows.Forms.ContextMenu" />.
/// </summary>
/// <remarks>
/// Implemented as an adapter over <see cref="T:System.Windows.Forms.ToolStripMenuItem" />, which is what
/// actually renders. The legacy type is kept in its own hierarchy (<c>MenuItem : Menu</c>) so that source
/// written against the old API — including <c>Menu m = someMenuItem;</c> — compiles unchanged.
/// </remarks>
public class MenuItem : Menu
{
    /// <summary>
    /// Occurs when the item is clicked.
    /// </summary>
    public event EventHandler? Click;

    /// <summary>
    /// Occurs when the item is about to be displayed.
    /// </summary>
    public event EventHandler? Popup;

    /// <summary>
    /// Occurs when the item is selected.
    /// </summary>
    public event EventHandler? Select;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:System.Windows.Forms.MenuItem" /> class.
    /// </summary>
    public MenuItem() : this(string.Empty)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:System.Windows.Forms.MenuItem" /> class with the
    /// specified caption.
    /// </summary>
    /// <param name="text">The caption of the item.</param>
    public MenuItem(string? text)
    {
        Item = new ToolStripMenuItem(text ?? string.Empty);
        Item.Click += OnItemClick;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:System.Windows.Forms.MenuItem" /> class with the
    /// specified caption and click handler.
    /// </summary>
    /// <param name="text">The caption of the item.</param>
    /// <param name="onClick">The handler invoked when the item is clicked.</param>
    public MenuItem(string? text, EventHandler onClick) : this(text)
    {
        if (onClick != null)
        {
            Click += onClick;
        }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:System.Windows.Forms.MenuItem" /> class with the
    /// specified caption and sub-items.
    /// </summary>
    /// <param name="text">The caption of the item.</param>
    /// <param name="items">The sub-items of the item.</param>
    public MenuItem(string? text, MenuItem[] items) : this(text)
    {
        if (items != null)
        {
            MenuItems.AddRange(items);
        }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:System.Windows.Forms.MenuItem" /> class with the
    /// specified caption, click handler and shortcut.
    /// </summary>
    /// <param name="text">The caption of the item.</param>
    /// <param name="onClick">The handler invoked when the item is clicked.</param>
    /// <param name="shortcut">The shortcut key for the item.</param>
    public MenuItem(string? text, EventHandler onClick, Shortcut shortcut) : this(text, onClick)
    {
        Shortcut = shortcut;
    }

    /// <summary>
    /// The modern item this legacy item drives.
    /// </summary>
    internal ToolStripMenuItem Item { get; }

    /// <summary>
    /// Gets or sets the caption of the item.
    /// </summary>
    public string Text
    {
        get => Item.Text ?? string.Empty;
        set => Item.Text = value;
    }

    /// <summary>
    /// Gets or sets a value indicating whether the item is enabled.
    /// </summary>
    [DefaultValue(true)]
    public bool Enabled
    {
        get => Item.Enabled;
        set => Item.Enabled = value;
    }

    /// <summary>
    /// Gets or sets a value indicating whether a check mark appears beside the caption.
    /// </summary>
    [DefaultValue(false)]
    public bool Checked
    {
        get => Item.Checked;
        set => Item.Checked = value;
    }

    /// <summary>
    /// Gets or sets a value indicating whether the item is visible.
    /// </summary>
    [DefaultValue(true)]
    public bool Visible
    {
        get => Item.Visible;
        set => Item.Visible = value;
    }

    /// <summary>
    /// Gets or sets the shortcut key associated with the item.
    /// </summary>
    [DefaultValue(Shortcut.None)]
    public Shortcut Shortcut
    {
        get;
        set;
    } = Shortcut.None;

    /// <summary>
    /// Gets or sets a value indicating whether the shortcut is shown beside the caption.
    /// </summary>
    [DefaultValue(true)]
    public bool ShowShortcut
    {
        get;
        set;
    } = true;

    /// <summary>
    /// Gets or sets a value indicating whether the item is drawn by the application.
    /// </summary>
    [DefaultValue(false)]
    public bool OwnerDraw
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets a value indicating whether the item is placed on a new line or column.
    /// </summary>
    [DefaultValue(false)]
    public bool BarBreak
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets a value indicating whether the item is placed in a new column.
    /// </summary>
    [DefaultValue(false)]
    public bool Break
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets a value indicating whether the item is the default item of its menu.
    /// </summary>
    [DefaultValue(false)]
    public bool DefaultItem
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the zero-based position of the item within its parent menu.
    /// </summary>
    public int Index
    {
        get => Parent?.MenuItems.IndexOf(this) ?? -1;
        set
        {
            var parent = Parent;
            if (parent == null)
            {
                return;
            }

            parent.MenuItems.Remove(this);
            parent.MenuItems.Insert(Math.Max(0, Math.Min(value, parent.MenuItems.Count)), this);
        }
    }

    /// <summary>
    /// Gets the menu that contains this item.
    /// </summary>
    [Browsable(false)]
    public Menu? Parent
    {
        get;
        internal set;
    }

    /// <summary>
    /// Gets the <see cref="T:System.Windows.Forms.MainMenu" /> that contains this item, if any.
    /// </summary>
    public MainMenu? GetMainMenu()
    {
        for (Menu? menu = this; menu != null; menu = (menu as MenuItem)?.Parent)
        {
            if (menu is MainMenu main)
            {
                return main;
            }
        }

        return null;
    }

    /// <summary>
    /// Gets the <see cref="T:System.Windows.Forms.ContextMenu" /> that contains this item, if any.
    /// </summary>
    public ContextMenu? GetContextMenu()
    {
        for (Menu? menu = this; menu != null; menu = (menu as MenuItem)?.Parent)
        {
            if (menu is ContextMenu context)
            {
                return context;
            }
        }

        return null;
    }

    /// <summary>
    /// Raises the <see cref="E:System.Windows.Forms.MenuItem.Click" /> event.
    /// </summary>
    public void PerformClick() => OnClick(EventArgs.Empty);

    /// <summary>
    /// Raises the <see cref="E:System.Windows.Forms.MenuItem.Select" /> event.
    /// </summary>
    public void PerformSelect() => OnSelect(EventArgs.Empty);

    /// <summary>
    /// Raises the <see cref="E:System.Windows.Forms.MenuItem.Click" /> event.
    /// </summary>
    /// <param name="e">The event data.</param>
    protected virtual void OnClick(EventArgs e) => Click?.Invoke(this, e);

    /// <summary>
    /// Raises the <see cref="E:System.Windows.Forms.MenuItem.Popup" /> event.
    /// </summary>
    /// <param name="e">The event data.</param>
    protected virtual void OnPopup(EventArgs e) => Popup?.Invoke(this, e);

    /// <summary>
    /// Raises the <see cref="E:System.Windows.Forms.MenuItem.Select" /> event.
    /// </summary>
    /// <param name="e">The event data.</param>
    protected virtual void OnSelect(EventArgs e) => Select?.Invoke(this, e);

    /// <summary>
    /// Creates a copy of this item and its sub-items.
    /// </summary>
    public virtual MenuItem CloneMenu()
    {
        var clone = new MenuItem(Text)
        {
            Enabled = Enabled,
            Checked = Checked,
            Visible = Visible,
            Shortcut = Shortcut,
            ShowShortcut = ShowShortcut,
            Name = Name,
            Tag = Tag
        };

        for (var i = 0; i < MenuItems.Count; i++)
        {
            clone.MenuItems.Add(MenuItems[i].CloneMenu());
        }

        return clone;
    }

    /// <inheritdoc />
    internal override void HostAdd(MenuItem item) => Item.DropDownItems.Add(item.Item);

    /// <inheritdoc />
    internal override void HostRemove(MenuItem item) => Item.DropDownItems.Remove(item.Item);

    /// <inheritdoc />
    internal override void HostInsert(int index, MenuItem item) => Item.DropDownItems.Insert(index, item.Item);

    /// <inheritdoc />
    internal override void HostClear() => Item.DropDownItems.Clear();

    /// <summary>
    /// Forwards a click on the modern item to the legacy event.
    /// </summary>
    private void OnItemClick(object? sender, EventArgs e) => OnClick(e);

    /// <summary>
    /// Returns a string that represents this item.
    /// </summary>
    public override string ToString() => $"{GetType().Name}: {Text}";
}
