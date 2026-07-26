using System.Collections;
using System.ComponentModel;

namespace System.Windows.Forms;

/// <summary>
/// The base class for <see cref="T:System.Windows.Forms.MainMenu" />,
/// <see cref="T:System.Windows.Forms.ContextMenu" /> and <see cref="T:System.Windows.Forms.MenuItem" />.
/// </summary>
/// <remarks>
/// The whole legacy menu family was missing. These types are obsolete in modern WinForms, but an
/// application old enough to be worth porting is very likely to use them, so their absence was a hard
/// compile break at the start of a migration rather than a runtime surprise later. They are implemented as
/// adapters over the ToolStrip family, which is what actually renders.
/// </remarks>
public abstract class Menu : Component
{
    private MenuItemCollection? _menuItems;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:System.Windows.Forms.Menu" /> class.
    /// </summary>
    protected Menu()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:System.Windows.Forms.Menu" /> class holding the
    /// specified items.
    /// </summary>
    /// <param name="items">The items the menu starts with.</param>
    protected Menu(MenuItem[]? items)
    {
        if (items != null)
        {
            MenuItems.AddRange(items);
        }
    }

    /// <summary>
    /// Gets the items contained in this menu.
    /// </summary>
    public MenuItemCollection MenuItems => _menuItems ??= new MenuItemCollection(this);

    /// <summary>
    /// Gets or sets the name of the menu.
    /// </summary>
    public string Name
    {
        get;
        set;
    } = string.Empty;

    /// <summary>
    /// Gets or sets user-defined data associated with the menu.
    /// </summary>
    [DefaultValue(null)]
    public object? Tag
    {
        get;
        set;
    }

    /// <summary>
    /// Gets a value indicating whether the menu holds any items.
    /// </summary>
    [Browsable(false)]
    public bool IsParent => _menuItems is { Count: > 0 };

    /// <summary>
    /// Adds the item to whatever hosts this menu. Each derived menu attaches its items differently.
    /// </summary>
    /// <param name="item">The item being added.</param>
    internal abstract void HostAdd(MenuItem item);

    /// <summary>
    /// Removes the item from whatever hosts this menu.
    /// </summary>
    /// <param name="item">The item being removed.</param>
    internal abstract void HostRemove(MenuItem item);

    /// <summary>
    /// Inserts the item into whatever hosts this menu at the given position.
    /// </summary>
    /// <param name="index">The position to insert at.</param>
    /// <param name="item">The item being inserted.</param>
    internal abstract void HostInsert(int index, MenuItem item);

    /// <summary>
    /// Removes every item from whatever hosts this menu.
    /// </summary>
    internal abstract void HostClear();

    /// <summary>
    /// Returns a string that represents this menu.
    /// </summary>
    public override string ToString() => $"{GetType().Name}, Items.Count: {MenuItems.Count}";

    /// <summary>
    /// Represents the collection of items in a <see cref="T:System.Windows.Forms.Menu" />.
    /// </summary>
    public class MenuItemCollection : IList
    {
        private readonly List<MenuItem> items = new();

        private readonly Menu owner;

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="T:System.Windows.Forms.Menu.MenuItemCollection" /> class.
        /// </summary>
        /// <param name="owner">The menu that owns the collection.</param>
        public MenuItemCollection(Menu owner)
        {
            this.owner = owner ?? throw new ArgumentNullException(nameof(owner));
        }

        /// <summary>Gets the number of items in the collection.</summary>
        public int Count => items.Count;

        /// <summary>Gets a value indicating whether the collection is read-only.</summary>
        public bool IsReadOnly => false;

        /// <summary>Gets a value indicating whether the collection has a fixed size.</summary>
        public bool IsFixedSize => false;

        /// <summary>Gets a value indicating whether access to the collection is synchronised.</summary>
        public bool IsSynchronized => false;

        /// <summary>Gets an object that can be used to synchronise access to the collection.</summary>
        public object SyncRoot => ((ICollection)items).SyncRoot;

        /// <summary>Gets or sets the item at the specified position.</summary>
        /// <param name="index">The zero-based position of the item.</param>
        public virtual MenuItem this[int index]
        {
            get => items[index];
            set
            {
                RemoveAt(index);
                Insert(index, value);
            }
        }

        /// <summary>
        /// Gets the item with the specified name, or <see langword="null" /> when there is none.
        /// </summary>
        /// <param name="key">The name to look for.</param>
        public virtual MenuItem? this[string key]
        {
            get
            {
                var index = IndexOfKey(key);

                return index == -1 ? null : items[index];
            }
        }

        object? IList.this[int index]
        {
            get => items[index];
            set => this[index] = value as MenuItem ?? throw new ArgumentException("Value must be a MenuItem.", nameof(value));
        }

        /// <summary>Adds an item displaying the given caption.</summary>
        /// <param name="caption">The caption of the new item.</param>
        /// <returns>The item that was added.</returns>
        public virtual MenuItem Add(string caption) => Add(new MenuItem(caption));

        /// <summary>Adds an item displaying the given caption and raising the given handler when clicked.</summary>
        /// <param name="caption">The caption of the new item.</param>
        /// <param name="onClick">The handler invoked when the item is clicked.</param>
        /// <returns>The item that was added.</returns>
        public virtual MenuItem Add(string caption, EventHandler onClick) => Add(new MenuItem(caption, onClick));

        /// <summary>Adds an item displaying the given caption and holding the given sub-items.</summary>
        /// <param name="caption">The caption of the new item.</param>
        /// <param name="items">The sub-items of the new item.</param>
        /// <returns>The item that was added.</returns>
        public virtual MenuItem Add(string caption, MenuItem[] items) => Add(new MenuItem(caption, items));

        /// <summary>Adds the specified item.</summary>
        /// <param name="item">The item to add.</param>
        /// <returns>The item that was added.</returns>
        public virtual MenuItem Add(MenuItem item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            items.Add(item);
            item.Parent = owner;
            owner.HostAdd(item);

            return item;
        }

        /// <summary>Adds the specified item at the given position.</summary>
        /// <param name="index">The position to add at.</param>
        /// <param name="item">The item to add.</param>
        /// <returns>The position the item was added at.</returns>
        public virtual int Add(int index, MenuItem item)
        {
            Insert(index, item);

            return index;
        }

        int IList.Add(object? value)
        {
            Add(value as MenuItem ?? throw new ArgumentException("Value must be a MenuItem.", nameof(value)));

            return items.Count - 1;
        }

        /// <summary>Adds every one of the given items.</summary>
        /// <param name="items">The items to add.</param>
        public virtual void AddRange(MenuItem[] items)
        {
            if (items == null)
            {
                throw new ArgumentNullException(nameof(items));
            }

            foreach (var item in items)
            {
                Add(item);
            }
        }

        /// <summary>Removes every item.</summary>
        public virtual void Clear()
        {
            foreach (var item in items)
            {
                item.Parent = null;
            }

            items.Clear();
            owner.HostClear();
        }

        /// <summary>Indicates whether the specified item is in the collection.</summary>
        /// <param name="item">The item to look for.</param>
        public bool Contains(MenuItem item) => items.Contains(item);

        bool IList.Contains(object? value) => value is MenuItem item && items.Contains(item);

        /// <summary>Indicates whether an item with the specified name is in the collection.</summary>
        /// <param name="key">The name to look for.</param>
        public virtual bool ContainsKey(string? key) => IndexOfKey(key) != -1;

        /// <summary>Copies the items into the given array.</summary>
        /// <param name="dest">The array to copy into.</param>
        /// <param name="index">The position in the array to start at.</param>
        public void CopyTo(Array dest, int index) => ((ICollection)items).CopyTo(dest, index);

        /// <summary>Returns an enumerator over the items.</summary>
        public IEnumerator GetEnumerator() => items.GetEnumerator();

        /// <summary>Returns the position of the specified item, or -1 when it is not present.</summary>
        /// <param name="item">The item to locate.</param>
        public int IndexOf(MenuItem item) => items.IndexOf(item);

        int IList.IndexOf(object? value) => value is MenuItem item ? items.IndexOf(item) : -1;

        /// <summary>Returns the position of the item with the specified name, or -1.</summary>
        /// <param name="key">The name to look for.</param>
        public virtual int IndexOfKey(string? key)
        {
            if (string.IsNullOrEmpty(key))
            {
                return -1;
            }

            return items.FindIndex(item => string.Equals(item.Name, key, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>Inserts the item at the given position.</summary>
        /// <param name="index">The position to insert at.</param>
        /// <param name="item">The item to insert.</param>
        public void Insert(int index, MenuItem item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            items.Insert(index, item);
            item.Parent = owner;
            owner.HostInsert(index, item);
        }

        void IList.Insert(int index, object? value) =>
            Insert(index, value as MenuItem ?? throw new ArgumentException("Value must be a MenuItem.", nameof(value)));

        /// <summary>Removes the specified item.</summary>
        /// <param name="item">The item to remove.</param>
        public void Remove(MenuItem item)
        {
            if (item == null || !items.Remove(item))
            {
                return;
            }

            item.Parent = null;
            owner.HostRemove(item);
        }

        void IList.Remove(object? value)
        {
            if (value is MenuItem item)
            {
                Remove(item);
            }
        }

        /// <summary>Removes the item at the given position.</summary>
        /// <param name="index">The position of the item to remove.</param>
        public virtual void RemoveAt(int index) => Remove(items[index]);

        /// <summary>Removes the item with the specified name, if there is one.</summary>
        /// <param name="key">The name of the item to remove.</param>
        public virtual void RemoveByKey(string? key)
        {
            var index = IndexOfKey(key);
            if (index != -1)
            {
                RemoveAt(index);
            }
        }
    }
}
